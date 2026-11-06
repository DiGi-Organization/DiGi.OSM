using DiGi.GIS.IO.Classes;
using DiGi.Geometry.Planar;
using DiGi.Geometry.Planar.Classes;
using DiGi.Geometry.Planar.Interfaces;
using DiGi.OSM.Classes;
using DiGi.OSM.Enums;
using OsmSharp;
using OsmSharp.Streams;
using OsmSharp.Tags;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json.Nodes;

namespace DiGi.OSM
{
    public static partial class Query
    {
        /// <summary>
        /// Streams building footprints from an OpenStreetMap PBF extract (a dated Geofabrik file such as <c>poland-YYMMDD.osm.pbf</c>) as a lazily enumerated sequence of DiGi footprints in EPSG:4326.
        /// <para>A footprint is produced per closed way carrying <c>building=*</c> (except <c>building=no</c>, which is counted as a rejection) and per <c>type=multipolygon</c> relation carrying <c>building=*</c>: a relation with one outer ring becomes one <see cref="PolygonalFace2D"/>, a relation with several outer rings becomes one <see cref="GeometryCollection2D"/> of faces, each holding its own holes. The source identifier is <c>way/&lt;id&gt;</c> or <c>relation/&lt;id&gt;</c>; the attribute bag carries every tag whose key starts with <c>building</c> plus <c>source</c>; the provenance object carries the OSM type and identifier together with the PBF header timestamp and replication sequence.</para>
        /// <para><b>Bounded node cache.</b> The sequence performs six passes over the file: scout the building multipolygon relations for their member ways, count the node identifiers those relations and every candidate way reference, fill an exactly sized identifier array with them, resolve the referenced nodes into parallel <c>int32</c> microdegree coordinate arrays (sixteen bytes per referenced node, no node objects and no tags, and no resizing of the large arrays after they are allocated), then emit the ways and the relations. Peak memory therefore scales with the number of nodes the buildings reference, not with the size of the extract; unrelated nodes are never held.</para>
        /// <para><b>Rejections.</b> An object that cannot become a footprint is counted under its <see cref="BuildingReadRejectionReason"/> in <see cref="BuildingReadResult.Summary"/> (with a bounded sample of source identifiers) and is never dropped silently; the same applies to nodes the extract no longer contains. The bounding box filter is not a rejection: filtered footprints simply are not emitted, while every other summary count stays complete.</para>
        /// <para>The footprint sequence runs its passes during enumeration, so <see cref="BuildingReadResult.Summary"/> is complete only once the sequence has been enumerated to its end.</para>
        /// <para><b>Measured on <c>poland-261104.osm.pbf</c> (2 115 837 171 bytes, MD5 verified, 2026-11-06):</b> peak working set 2 989 649 920, 2 990 034 944 and 2 989 744 128 bytes over three consecutive runs, sampled managed memory about 2 422 000 000 bytes, 97 722 280 referenced nodes held at sixteen bytes each (1.56 GB of store), 17 815 662 building ways and 5 634 building relations producing 17 820 957 footprints plus 339 rejections - the in-scope objects balance exactly, nothing is dropped - in 312-337 s. The DiGi.Test budget fact asserts 3 300 000 000 bytes.</para>
        /// </summary>
        /// <param name="path">The path of the PBF file, or null or whitespace to return null. The file must exist; a missing file fails with <see cref="FileNotFoundException"/> when enumeration starts.</param>
        /// <param name="boundingBox">An optional bounding box, in EPSG:4326 (x = longitude), that a footprint's envelope must intersect to be emitted.</param>
        /// <returns>The read result (header, footprints, summary), or null when <paramref name="path"/> is null or whitespace.</returns>
        /// <exception cref="InvalidDataException">Thrown when the extract is not sorted by node identifier, or when its header blob is malformed.</exception>
        public static BuildingReadResult? Footprints_Pbf(string? path, BoundingBox2D? boundingBox = null)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return null;
            }

            string filePath = path!;
            PbfHeader? pbfHeader = PbfHeader(filePath);
            BuildingReadCounter counter = new();

            IEnumerable<Footprint> Iterator()
            {
                const int MissingLatitude = int.MinValue;
                const int InvalidLatitude = int.MaxValue;
                const double Microdegree = 10000000.0;

                HashSet<long> memberWayIds = [];
                Dictionary<long, long[]> memberWayNodes = [];
                NodeCoordinateStore nodeCoordinateStore = null!;

                bool HasBuildingTag(TagsCollectionBase? tags)
                {
                    return tags != null && tags.ContainsKey("building");
                }

                bool IsBuilding(TagsCollectionBase? tags)
                {
                    if (tags == null || !tags.TryGetValue("building", out string? building) || building == null)
                    {
                        return false;
                    }

                    return !string.Equals(building.Trim(), "no", StringComparison.OrdinalIgnoreCase);
                }

                bool IsMultipolygon(TagsCollectionBase? tags)
                {
                    if (tags == null || !tags.TryGetValue("type", out string? type) || type == null)
                    {
                        return false;
                    }

                    return string.Equals(type.Trim(), "multipolygon", StringComparison.OrdinalIgnoreCase);
                }

                string? SourceId(OsmGeoType type, long? id)
                {
                    if (id is not long id_Temp)
                    {
                        return null;
                    }

                    if (type == OsmGeoType.Way)
                    {
                        return string.Format(CultureInfo.InvariantCulture, "way/{0}", id_Temp);
                    }

                    if (type == OsmGeoType.Relation)
                    {
                        return string.Format(CultureInfo.InvariantCulture, "relation/{0}", id_Temp);
                    }

                    return string.Format(CultureInfo.InvariantCulture, "node/{0}", id_Temp);
                }

                void Reject(string? sourceId, BuildingReadRejectionReason reason)
                {
                    long count;
                    switch (reason)
                    {
                        case BuildingReadRejectionReason.NotBuilding:
                            count = ++counter.NotBuildingCount;
                            break;

                        case BuildingReadRejectionReason.UnclosedGeometry:
                            count = ++counter.UnclosedGeometryCount;
                            break;

                        case BuildingReadRejectionReason.TooFewNodes:
                            count = ++counter.TooFewNodesCount;
                            break;

                        case BuildingReadRejectionReason.MissingNode:
                            count = ++counter.MissingNodeCount;
                            break;

                        case BuildingReadRejectionReason.InvalidCoordinate:
                            count = ++counter.InvalidCoordinateCount;
                            break;

                        case BuildingReadRejectionReason.MissingMemberWay:
                            count = ++counter.MissingMemberWayCount;
                            break;

                        case BuildingReadRejectionReason.RingNotAssembled:
                            count = ++counter.RingNotAssembledCount;
                            break;

                        case BuildingReadRejectionReason.MalformedObject:
                            count = ++counter.MalformedObjectCount;
                            break;

                        default:
                            throw new ArgumentOutOfRangeException(nameof(reason), reason, "Unknown rejection reason.");
                    }

                    if (sourceId != null && count <= 20)
                    {
                        counter.Rejections.Add(string.Format(CultureInfo.InvariantCulture, "{0}: {1}", sourceId, reason));
                    }
                }

                int IndexOf(long nodeId)
                {
                    int low = 0;
                    int high = nodeCoordinateStore.Count - 1;
                    while (low <= high)
                    {
                        int middle = low + ((high - low) / 2);
                        long middleId = nodeCoordinateStore.Ids[middle];
                        if (middleId == nodeId)
                        {
                            return middle;
                        }

                        if (middleId < nodeId)
                        {
                            low = middle + 1;
                        }
                        else
                        {
                            high = middle - 1;
                        }
                    }

                    return -1;
                }

                List<Point2D>? Points(long[] nodeIds, string? sourceId)
                {
                    List<Point2D> point2Ds = new(nodeIds.Length);
                    foreach (long nodeId in nodeIds)
                    {
                        int index = IndexOf(nodeId);
                        if (index < 0)
                        {
                            Reject(sourceId, BuildingReadRejectionReason.MissingNode);
                            return null;
                        }

                        int latitude = nodeCoordinateStore.Latitudes[index];
                        if (latitude == MissingLatitude)
                        {
                            Reject(sourceId, BuildingReadRejectionReason.MissingNode);
                            return null;
                        }

                        if (latitude == InvalidLatitude)
                        {
                            Reject(sourceId, BuildingReadRejectionReason.InvalidCoordinate);
                            return null;
                        }

                        point2Ds.Add(new Point2D(nodeCoordinateStore.Longitudes[index] / Microdegree, latitude / Microdegree));
                    }

                    return point2Ds;
                }

                BoundingBox2D? BoundingBox(List<Point2D> point2Ds)
                {
                    if (point2Ds.Count == 0)
                    {
                        return null;
                    }

                    double minX = point2Ds[0].X;
                    double minY = point2Ds[0].Y;
                    double maxX = point2Ds[0].X;
                    double maxY = point2Ds[0].Y;

                    for (int i = 1; i < point2Ds.Count; i++)
                    {
                        Point2D point2D = point2Ds[i];
                        if (point2D.X < minX)
                        {
                            minX = point2D.X;
                        }

                        if (point2D.X > maxX)
                        {
                            maxX = point2D.X;
                        }

                        if (point2D.Y < minY)
                        {
                            minY = point2D.Y;
                        }

                        if (point2D.Y > maxY)
                        {
                            maxY = point2D.Y;
                        }
                    }

                    return new BoundingBox2D(new Point2D(minX, minY), new Point2D(maxX, maxY));
                }

                JsonObject Attributes(TagsCollectionBase? tags)
                {
                    JsonObject jsonObject = new();
                    if (tags != null)
                    {
                        foreach (Tag tag in tags)
                        {
                            if (tag.Key == null || tag.Value == null)
                            {
                                continue;
                            }

                            if (tag.Key.StartsWith("building", StringComparison.Ordinal) || string.Equals(tag.Key, "source", StringComparison.Ordinal))
                            {
                                jsonObject[tag.Key] = tag.Value;
                            }
                        }
                    }

                    return jsonObject;
                }

                JsonObject Provenance(OsmGeoType type, long? id)
                {
                    JsonObject jsonObject = new();
                    jsonObject["dataset"] = "osm";
                    jsonObject["osm_type"] = type == OsmGeoType.Way ? "way" : type == OsmGeoType.Relation ? "relation" : "node";

                    if (id is long id_Temp)
                    {
                        jsonObject["osm_id"] = id_Temp;
                    }

                    if (pbfHeader != null)
                    {
                        if (pbfHeader.Timestamp is DateTimeOffset timestamp)
                        {
                            jsonObject["header_timestamp"] = timestamp.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture);
                        }

                        if (pbfHeader.ReplicationSequence is long sequence)
                        {
                            jsonObject["replication_sequence"] = sequence;
                        }

                        if (!string.IsNullOrWhiteSpace(pbfHeader.ReplicationBaseUrl))
                        {
                            jsonObject["replication_base_url"] = pbfHeader.ReplicationBaseUrl;
                        }
                    }

                    return jsonObject;
                }

                List<long[]>? Rings(List<long[]> ways)
                {
                    List<long[]> rings = [];
                    List<long[]> remaining = [.. ways];

                    while (remaining.Count > 0)
                    {
                        long[] chain = remaining[0];
                        remaining.RemoveAt(0);

                        if (chain.Length == 0)
                        {
                            return null;
                        }

                        List<long> ring = [.. chain];
                        while (ring[0] != ring[ring.Count - 1])
                        {
                            int index = -1;
                            bool reverse = false;

                            for (int i = 0; i < remaining.Count; i++)
                            {
                                long[] candidate = remaining[i];
                                if (candidate.Length == 0)
                                {
                                    continue;
                                }

                                if (candidate[0] == ring[ring.Count - 1])
                                {
                                    index = i;
                                    break;
                                }

                                if (candidate[candidate.Length - 1] == ring[ring.Count - 1])
                                {
                                    index = i;
                                    reverse = true;
                                    break;
                                }
                            }

                            if (index < 0)
                            {
                                break;
                            }

                            long[] next = remaining[index];
                            remaining.RemoveAt(index);

                            if (!reverse)
                            {
                                for (int j = 1; j < next.Length; j++)
                                {
                                    ring.Add(next[j]);
                                }
                            }
                            else
                            {
                                for (int j = next.Length - 2; j >= 0; j--)
                                {
                                    ring.Add(next[j]);
                                }
                            }
                        }

                        if (ring[0] != ring[ring.Count - 1])
                        {
                            return null;
                        }

                        rings.Add(ring.ToArray());
                    }

                    return rings;
                }

                Footprint? WayFootprint(Way way, string? sourceId)
                {
                    if (sourceId == null)
                    {
                        Reject(null, BuildingReadRejectionReason.MalformedObject);
                        return null;
                    }

                    if (way.Nodes is not long[] nodes)
                    {
                        Reject(sourceId, BuildingReadRejectionReason.MalformedObject);
                        return null;
                    }

                    if (nodes.Length == 0 || nodes[0] != nodes[nodes.Length - 1])
                    {
                        Reject(sourceId, BuildingReadRejectionReason.UnclosedGeometry);
                        return null;
                    }

                    if (nodes.Length < 4)
                    {
                        Reject(sourceId, BuildingReadRejectionReason.TooFewNodes);
                        return null;
                    }

                    List<Point2D>? point2Ds = Points(nodes, sourceId);
                    if (point2Ds == null)
                    {
                        return null;
                    }

                    Polygon2D? polygon2D = Create.Polygon2D(point2Ds, DiGi.Core.Constants.Tolerance.Distance);
                    if (polygon2D == null)
                    {
                        Reject(sourceId, BuildingReadRejectionReason.TooFewNodes);
                        return null;
                    }

                    PolygonalFace2D? polygonalFace2D = Create.PolygonalFace2D(polygon2D);
                    if (polygonalFace2D == null)
                    {
                        Reject(sourceId, BuildingReadRejectionReason.RingNotAssembled);
                        return null;
                    }

                    if (boundingBox != null)
                    {
                        BoundingBox2D? boundingBox_Footprint = BoundingBox(point2Ds);
                        if (boundingBox_Footprint == null || !boundingBox.InRange(boundingBox_Footprint))
                        {
                            return null;
                        }
                    }

                    return new Footprint(sourceId, 4326, polygonalFace2D, Attributes(way.Tags), Provenance(OsmGeoType.Way, way.Id), null);
                }

                Footprint? RelationFootprint(Relation relation, string? sourceId)
                {
                    if (sourceId == null)
                    {
                        Reject(null, BuildingReadRejectionReason.MalformedObject);
                        return null;
                    }

                    if (relation.Members is not RelationMember[] members)
                    {
                        Reject(sourceId, BuildingReadRejectionReason.MalformedObject);
                        return null;
                    }

                    List<long[]> outerWays = [];
                    List<long[]> innerWays = [];

                    foreach (RelationMember member in members)
                    {
                        if (member.Type != OsmGeoType.Way)
                        {
                            continue;
                        }

                        if (!memberWayNodes.TryGetValue(member.Id, out long[]? memberNodes) || memberNodes == null)
                        {
                            Reject(sourceId, BuildingReadRejectionReason.MissingMemberWay);
                            return null;
                        }

                        if (string.Equals(member.Role?.Trim(), "inner", StringComparison.OrdinalIgnoreCase))
                        {
                            innerWays.Add(memberNodes);
                        }
                        else
                        {
                            outerWays.Add(memberNodes);
                        }
                    }

                    if (outerWays.Count == 0)
                    {
                        Reject(sourceId, BuildingReadRejectionReason.RingNotAssembled);
                        return null;
                    }

                    List<long[]>? outerRings = Rings(outerWays);
                    if (outerRings == null)
                    {
                        Reject(sourceId, BuildingReadRejectionReason.RingNotAssembled);
                        return null;
                    }

                    List<long[]>? innerRings = null;
                    if (innerWays.Count > 0)
                    {
                        innerRings = Rings(innerWays);
                        if (innerRings == null)
                        {
                            Reject(sourceId, BuildingReadRejectionReason.RingNotAssembled);
                            return null;
                        }
                    }

                    List<Polygon2D> outerPolygons = [];
                    List<List<Point2D>> outerPoint2Ds = [];

                    foreach (long[] outerRing in outerRings)
                    {
                        List<Point2D>? outerPoint2Ds_Temp = Points(outerRing, sourceId);
                        if (outerPoint2Ds_Temp == null)
                        {
                            return null;
                        }

                        Polygon2D? outerPolygon = Create.Polygon2D(outerPoint2Ds_Temp, DiGi.Core.Constants.Tolerance.Distance);
                        if (outerPolygon == null)
                        {
                            Reject(sourceId, BuildingReadRejectionReason.TooFewNodes);
                            return null;
                        }

                        outerPolygons.Add(outerPolygon);
                        outerPoint2Ds.Add(outerPoint2Ds_Temp);
                    }

                    List<Polygon2D> innerPolygons = [];
                    List<List<Point2D>> innerPoint2Ds = [];

                    if (innerRings != null)
                    {
                        foreach (long[] innerRing in innerRings)
                        {
                            List<Point2D>? innerPoint2Ds_Temp = Points(innerRing, sourceId);
                            if (innerPoint2Ds_Temp == null)
                            {
                                return null;
                            }

                            Polygon2D? innerPolygon = Create.Polygon2D(innerPoint2Ds_Temp, DiGi.Core.Constants.Tolerance.Distance);
                            if (innerPolygon == null)
                            {
                                Reject(sourceId, BuildingReadRejectionReason.TooFewNodes);
                                return null;
                            }

                            innerPolygons.Add(innerPolygon);
                            innerPoint2Ds.Add(innerPoint2Ds_Temp);
                        }
                    }

                    IGeometry2D? geometry;

                    if (outerPolygons.Count == 1)
                    {
                        PolygonalFace2D? polygonalFace2D = Create.PolygonalFace2D(outerPolygons[0], innerPolygons);
                        if (polygonalFace2D == null || (polygonalFace2D.InternalEdges?.Count ?? 0) != innerPolygons.Count)
                        {
                            Reject(sourceId, BuildingReadRejectionReason.RingNotAssembled);
                            return null;
                        }

                        geometry = polygonalFace2D;
                    }
                    else
                    {
                        List<Polygon2D>[] innersByOuter = new List<Polygon2D>[outerPolygons.Count];
                        for (int i = 0; i < innersByOuter.Length; i++)
                        {
                            innersByOuter[i] = [];
                        }

                        for (int i = 0; i < innerPolygons.Count; i++)
                        {
                            int owner = -1;
                            for (int j = 0; j < outerPoint2Ds.Count; j++)
                            {
                                if (DiGi.Geometry.Planar.Query.Inside(outerPoint2Ds[j], innerPoint2Ds[i][0]))
                                {
                                    owner = j;
                                    break;
                                }
                            }

                            if (owner < 0)
                            {
                                Reject(sourceId, BuildingReadRejectionReason.RingNotAssembled);
                                return null;
                            }

                            innersByOuter[owner].Add(innerPolygons[i]);
                        }

                        GeometryCollection2D geometryCollection2D = [];
                        for (int j = 0; j < outerPolygons.Count; j++)
                        {
                            PolygonalFace2D? polygonalFace2D = Create.PolygonalFace2D(outerPolygons[j], innersByOuter[j]);
                            if (polygonalFace2D == null || (polygonalFace2D.InternalEdges?.Count ?? 0) != innersByOuter[j].Count)
                            {
                                Reject(sourceId, BuildingReadRejectionReason.RingNotAssembled);
                                return null;
                            }

                            geometryCollection2D.Add(polygonalFace2D);
                        }

                        geometry = geometryCollection2D;
                    }

                    if (boundingBox != null)
                    {
                        List<Point2D> allPoint2Ds = [];
                        foreach (List<Point2D> point2Ds in outerPoint2Ds)
                        {
                            allPoint2Ds.AddRange(point2Ds);
                        }

                        BoundingBox2D? boundingBox_Footprint = BoundingBox(allPoint2Ds);
                        if (boundingBox_Footprint == null || !boundingBox.InRange(boundingBox_Footprint))
                        {
                            return null;
                        }
                    }

                    return new Footprint(sourceId, 4326, geometry, Attributes(relation.Tags), Provenance(OsmGeoType.Relation, relation.Id), null);
                }

                using FileStream fileStream = new(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                using PBFOsmStreamSource osmStreamSource = new(fileStream);

                // Pass 1: scout the building multipolygon relations for the way identifiers their geometry is assembled from.
                while (osmStreamSource.MoveNext(true, true, false))
                {
                    if (osmStreamSource.Current() is not Relation relation)
                    {
                        continue;
                    }

                    if (!IsMultipolygon(relation.Tags) || !IsBuilding(relation.Tags))
                    {
                        continue;
                    }

                    if (relation.Members is not RelationMember[] members)
                    {
                        continue;
                    }

                    foreach (RelationMember member in members)
                    {
                        if (member.Type == OsmGeoType.Way)
                        {
                            memberWayIds.Add(member.Id);
                        }
                    }
                }

                osmStreamSource.Reset();

                // Pass 2: count the node identifiers every candidate way references, so the identifier array is allocated exactly once - a growing array would leave multi-gigabyte resize transients on the large object heap.
                long referencedNodeIdTotal = 0;

                while (osmStreamSource.MoveNext(true, false, true))
                {
                    if (osmStreamSource.Current() is not Way way)
                    {
                        continue;
                    }

                    bool candidate = IsBuilding(way.Tags);
                    if (!candidate && way.Id is long wayId && memberWayIds.Contains(wayId))
                    {
                        candidate = true;
                    }

                    if (!candidate || way.Nodes is not long[] nodes)
                    {
                        continue;
                    }

                    referencedNodeIdTotal += nodes.Length;
                }

                if (referencedNodeIdTotal > int.MaxValue)
                {
                    throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "The extract references {0} nodes, more than a single pass can index.", referencedNodeIdTotal));
                }

                // Pass 3: fill the exactly sized identifier array.
                osmStreamSource.Reset();

                long[] referencedNodeIds = new long[(int)referencedNodeIdTotal];
                int referencedNodeIdCount = 0;

                while (osmStreamSource.MoveNext(true, false, true))
                {
                    if (osmStreamSource.Current() is not Way way)
                    {
                        continue;
                    }

                    bool candidate = IsBuilding(way.Tags);
                    if (!candidate && way.Id is long wayId && memberWayIds.Contains(wayId))
                    {
                        candidate = true;
                    }

                    if (!candidate || way.Nodes is not long[] nodes)
                    {
                        continue;
                    }

                    foreach (long nodeId in nodes)
                    {
                        referencedNodeIds[referencedNodeIdCount] = nodeId;
                        referencedNodeIdCount++;
                    }
                }

                Array.Sort(referencedNodeIds, 0, referencedNodeIdCount);

                int Dedupe()
                {
                    int unique = 0;
                    for (int i = 0; i < referencedNodeIdCount; i++)
                    {
                        if (i == 0 || referencedNodeIds[i] != referencedNodeIds[i - 1])
                        {
                            referencedNodeIds[unique] = referencedNodeIds[i];
                            unique++;
                        }
                    }

                    return unique;
                }

                int uniqueCount = Dedupe();

                nodeCoordinateStore = new(referencedNodeIds, uniqueCount);
                counter.ReferencedNodeCount = uniqueCount;

                osmStreamSource.Reset();

                // Pass 4: resolve the referenced identifiers to microdegree coordinates; unrelated nodes are skipped and never held.
                int resolveIndex = 0;
                long lastNodeId = long.MinValue;

                while (osmStreamSource.MoveNext(false, true, true))
                {
                    if (osmStreamSource.Current() is not Node node || node.Id is not long nodeId)
                    {
                        continue;
                    }

                    if (nodeId < lastNodeId)
                    {
                        throw new InvalidDataException(string.Format(CultureInfo.InvariantCulture, "The extract is not sorted by node id: {0} follows {1}.", nodeId, lastNodeId));
                    }

                    lastNodeId = nodeId;

                    while (resolveIndex < nodeCoordinateStore.Count && nodeCoordinateStore.Ids[resolveIndex] < nodeId)
                    {
                        resolveIndex++;
                    }

                    if (resolveIndex >= nodeCoordinateStore.Count)
                    {
                        break;
                    }

                    if (nodeCoordinateStore.Ids[resolveIndex] != nodeId)
                    {
                        continue;
                    }

                    double? latitude = node.Latitude;
                    double? longitude = node.Longitude;

                    if (latitude is double latitude_Temp && longitude is double longitude_Temp && !double.IsNaN(latitude_Temp) && !double.IsInfinity(latitude_Temp) && latitude_Temp >= -90 && latitude_Temp <= 90 && !double.IsNaN(longitude_Temp) && !double.IsInfinity(longitude_Temp) && longitude_Temp >= -180 && longitude_Temp <= 180)
                    {
                        nodeCoordinateStore.Latitudes[resolveIndex] = (int)Math.Round(latitude_Temp * Microdegree);
                        nodeCoordinateStore.Longitudes[resolveIndex] = (int)Math.Round(longitude_Temp * Microdegree);
                    }
                    else
                    {
                        nodeCoordinateStore.Latitudes[resolveIndex] = InvalidLatitude;
                        nodeCoordinateStore.Longitudes[resolveIndex] = InvalidLatitude;
                    }

                    resolveIndex++;
                }

                osmStreamSource.Reset();

                // Pass 5: emit a footprint per building way, and buffer the node lists of the relation member ways.
                while (osmStreamSource.MoveNext(true, false, true))
                {
                    if (osmStreamSource.Current() is not Way way)
                    {
                        continue;
                    }

                    string? sourceId = SourceId(OsmGeoType.Way, way.Id);

                    if (HasBuildingTag(way.Tags))
                    {
                        counter.WayCount++;

                        if (!IsBuilding(way.Tags))
                        {
                            Reject(sourceId, BuildingReadRejectionReason.NotBuilding);
                        }
                        else
                        {
                            Footprint? footprint = WayFootprint(way, sourceId);
                            if (footprint != null)
                            {
                                counter.FootprintCount++;
                                yield return footprint;
                            }
                        }
                    }

                    if (way.Id is long wayId && memberWayIds.Contains(wayId) && way.Nodes is long[] memberNodes)
                    {
                        memberWayNodes[wayId] = memberNodes;
                    }
                }

                osmStreamSource.Reset();

                // Pass 6: emit a footprint per building multipolygon relation from the buffered member ways.
                while (osmStreamSource.MoveNext(true, true, false))
                {
                    if (osmStreamSource.Current() is not Relation relation)
                    {
                        continue;
                    }

                    if (!IsMultipolygon(relation.Tags) || !HasBuildingTag(relation.Tags))
                    {
                        continue;
                    }

                    counter.RelationCount++;

                    string? sourceId = SourceId(OsmGeoType.Relation, relation.Id);
                    if (!IsBuilding(relation.Tags))
                    {
                        Reject(sourceId, BuildingReadRejectionReason.NotBuilding);
                        continue;
                    }

                    Footprint? footprint = RelationFootprint(relation, sourceId);
                    if (footprint != null)
                    {
                        counter.FootprintCount++;
                        yield return footprint;
                    }
                }
            }

            return new BuildingReadResult(pbfHeader, Iterator(), counter);
        }
    }
}

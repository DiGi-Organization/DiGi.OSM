using DiGi.Core.Classes;
using DiGi.Core.Interfaces;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.OSM.Classes
{
    /// <summary>
    /// An immutable snapshot of one footprint read: objects read, footprints emitted and one count per <see cref="Enums.BuildingReadRejectionReason"/>, plus a bounded sample of rejected source identifiers.
    /// <para>The snapshot is taken from the run's <see cref="BuildingReadCounter"/>, which the reader fills while the footprint sequence is enumerated - reading <see cref="DiGi.OSM.Classes.BuildingReadResult.Summary"/> before enumeration has finished reports the partial state.</para>
    /// </summary>
    public class BuildingReadSummary : SerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(WayCount))]
        private readonly long wayCount;

        [JsonInclude, JsonPropertyName(nameof(RelationCount))]
        private readonly long relationCount;

        [JsonInclude, JsonPropertyName(nameof(FootprintCount))]
        private readonly long footprintCount;

        [JsonInclude, JsonPropertyName(nameof(ReferencedNodeCount))]
        private readonly long referencedNodeCount;

        [JsonInclude, JsonPropertyName(nameof(NotBuildingCount))]
        private readonly long notBuildingCount;

        [JsonInclude, JsonPropertyName(nameof(UnclosedGeometryCount))]
        private readonly long unclosedGeometryCount;

        [JsonInclude, JsonPropertyName(nameof(TooFewNodesCount))]
        private readonly long tooFewNodesCount;

        [JsonInclude, JsonPropertyName(nameof(MissingNodeCount))]
        private readonly long missingNodeCount;

        [JsonInclude, JsonPropertyName(nameof(InvalidCoordinateCount))]
        private readonly long invalidCoordinateCount;

        [JsonInclude, JsonPropertyName(nameof(MissingMemberWayCount))]
        private readonly long missingMemberWayCount;

        [JsonInclude, JsonPropertyName(nameof(RingNotAssembledCount))]
        private readonly long ringNotAssembledCount;

        [JsonInclude, JsonPropertyName(nameof(MalformedObjectCount))]
        private readonly long malformedObjectCount;

        [JsonInclude, JsonPropertyName(nameof(Rejections))]
        private readonly List<string>? rejections;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingReadSummary"/> class as a snapshot of the given counter.
        /// </summary>
        /// <param name="buildingReadCounter">The counter to snapshot, or null for an all-zero summary.</param>
        public BuildingReadSummary(BuildingReadCounter? buildingReadCounter)
        {
            if (buildingReadCounter == null)
            {
                return;
            }

            wayCount = buildingReadCounter.WayCount;
            relationCount = buildingReadCounter.RelationCount;
            footprintCount = buildingReadCounter.FootprintCount;
            referencedNodeCount = buildingReadCounter.ReferencedNodeCount;
            notBuildingCount = buildingReadCounter.NotBuildingCount;
            unclosedGeometryCount = buildingReadCounter.UnclosedGeometryCount;
            tooFewNodesCount = buildingReadCounter.TooFewNodesCount;
            missingNodeCount = buildingReadCounter.MissingNodeCount;
            invalidCoordinateCount = buildingReadCounter.InvalidCoordinateCount;
            missingMemberWayCount = buildingReadCounter.MissingMemberWayCount;
            ringNotAssembledCount = buildingReadCounter.RingNotAssembledCount;
            malformedObjectCount = buildingReadCounter.MalformedObjectCount;
            rejections = buildingReadCounter.Rejections.Count == 0 ? null : new List<string>(buildingReadCounter.Rejections);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingReadSummary"/> class by copying another instance.
        /// </summary>
        /// <param name="buildingReadSummary">The source summary to copy.</param>
        public BuildingReadSummary(BuildingReadSummary? buildingReadSummary) : base(buildingReadSummary)
        {
            if (buildingReadSummary != null)
            {
                wayCount = buildingReadSummary.wayCount;
                relationCount = buildingReadSummary.relationCount;
                footprintCount = buildingReadSummary.footprintCount;
                referencedNodeCount = buildingReadSummary.referencedNodeCount;
                notBuildingCount = buildingReadSummary.notBuildingCount;
                unclosedGeometryCount = buildingReadSummary.unclosedGeometryCount;
                tooFewNodesCount = buildingReadSummary.tooFewNodesCount;
                missingNodeCount = buildingReadSummary.missingNodeCount;
                invalidCoordinateCount = buildingReadSummary.invalidCoordinateCount;
                missingMemberWayCount = buildingReadSummary.missingMemberWayCount;
                ringNotAssembledCount = buildingReadSummary.ringNotAssembledCount;
                malformedObjectCount = buildingReadSummary.malformedObjectCount;
                rejections = buildingReadSummary.rejections == null ? null : new List<string>(buildingReadSummary.rejections);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingReadSummary"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the summary data.</param>
        public BuildingReadSummary(JsonObject? jsonObject) : base(jsonObject)
        {
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="BuildingReadSummary"/> instance.
        /// </summary>
        /// <returns>A new <see cref="BuildingReadSummary"/> instance with the same data, or null if cloning fails.</returns>
        public override ISerializableObject? Clone()
        {
            return new BuildingReadSummary(this);
        }

        /// <summary>
        /// Gets the number of ways carrying a <c>building</c> tag of any value.
        /// </summary>
        [JsonIgnore]
        public long WayCount => wayCount;

        /// <summary>
        /// Gets the number of <c>type=multipolygon</c> relations carrying a <c>building</c> tag of any value.
        /// </summary>
        [JsonIgnore]
        public long RelationCount => relationCount;

        /// <summary>
        /// Gets the number of footprints emitted by the read (after any bounding box filter).
        /// </summary>
        [JsonIgnore]
        public long FootprintCount => footprintCount;

        /// <summary>
        /// Gets the number of distinct node identifiers the candidate ways and relations reference - the size the bounded node cache scales with; zero before the identifiers have been collected.
        /// </summary>
        [JsonIgnore]
        public long ReferencedNodeCount => referencedNodeCount;

        /// <summary>
        /// Gets the number of objects excluded because they carry <c>building=no</c>.
        /// </summary>
        [JsonIgnore]
        public long NotBuildingCount => notBuildingCount;

        /// <summary>
        /// Gets the number of ways rejected because their node references do not form a closed outline.
        /// </summary>
        [JsonIgnore]
        public long UnclosedGeometryCount => unclosedGeometryCount;

        /// <summary>
        /// Gets the number of objects rejected because fewer than three distinct corners remain.
        /// </summary>
        [JsonIgnore]
        public long TooFewNodesCount => tooFewNodesCount;

        /// <summary>
        /// Gets the number of objects rejected because a referenced node is absent from the extract.
        /// </summary>
        [JsonIgnore]
        public long MissingNodeCount => missingNodeCount;

        /// <summary>
        /// Gets the number of objects rejected because a referenced node carries an invalid coordinate.
        /// </summary>
        [JsonIgnore]
        public long InvalidCoordinateCount => invalidCoordinateCount;

        /// <summary>
        /// Gets the number of relations rejected because a member way is absent from the extract.
        /// </summary>
        [JsonIgnore]
        public long MissingMemberWayCount => missingMemberWayCount;

        /// <summary>
        /// Gets the number of relations rejected because their member ways do not assemble into closed rings, or a hole is not contained by any outer ring.
        /// </summary>
        [JsonIgnore]
        public long RingNotAssembledCount => ringNotAssembledCount;

        /// <summary>
        /// Gets the number of objects rejected because they arrived without the structure the reader needs.
        /// </summary>
        [JsonIgnore]
        public long MalformedObjectCount => malformedObjectCount;

        /// <summary>
        /// Gets all rejection reasons summed.
        /// </summary>
        [JsonIgnore]
        public long RejectionCount => notBuildingCount + unclosedGeometryCount + tooFewNodesCount + missingNodeCount + invalidCoordinateCount + missingMemberWayCount + ringNotAssembledCount + malformedObjectCount;

        /// <summary>
        /// Gets a bounded sample of rejected source identifiers (at most twenty entries per rejection reason), or null when nothing was rejected.
        /// </summary>
        [JsonIgnore]
        public List<string>? Rejections => rejections == null ? null : new List<string>(rejections);
    }
}

#### [DiGi\.OSM](DiGi.OSM.Overview.md 'DiGi\.OSM\.Overview')

## DiGi\.OSM Namespace
### Classes

<a name='DiGi.OSM.Query'></a>

## Query Class

```csharp
public static class Query
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → Query
### Methods

<a name='DiGi.OSM.Query.Footprints_Pbf(string,DiGi.Geometry.Planar.Classes.BoundingBox2D)'></a>

## Query\.Footprints\_Pbf\(string, BoundingBox2D\) Method

Streams building footprints from an OpenStreetMap PBF extract \(a dated Geofabrik file such as `poland-YYMMDD.osm.pbf`\) as a lazily enumerated sequence of DiGi footprints in EPSG:4326\.

A footprint is produced per closed way carrying `building=*` (except `building=no`, which is counted as a rejection) and per `type=multipolygon` relation carrying `building=*`: a relation with one outer ring becomes one [DiGi\.Geometry\.Planar\.Classes\.PolygonalFace2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.polygonalface2d 'DiGi\.Geometry\.Planar\.Classes\.PolygonalFace2D'), a relation with several outer rings becomes one [DiGi\.Geometry\.Planar\.Classes\.GeometryCollection2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.geometrycollection2d 'DiGi\.Geometry\.Planar\.Classes\.GeometryCollection2D') of faces, each holding its own holes. The source identifier is `way/<id>` or `relation/<id>`; the attribute bag carries every tag whose key starts with `building` plus `source`; the provenance object carries the OSM type and identifier together with the PBF header timestamp and replication sequence.

<b>Bounded node cache.</b> The sequence performs six passes over the file: scout the building multipolygon relations for their member ways, count the node identifiers those relations and every candidate way reference, fill an exactly sized identifier array with them, resolve the referenced nodes into parallel `int32` microdegree coordinate arrays (sixteen bytes per referenced node, no node objects and no tags, and no resizing of the large arrays after they are allocated), then emit the ways and the relations. Peak memory therefore scales with the number of nodes the buildings reference, not with the size of the extract; unrelated nodes are never held.

<b>Rejections.</b> An object that cannot become a footprint is counted under its [BuildingReadRejectionReason](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason') in [Summary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadResult.Summary 'DiGi\.OSM\.Classes\.BuildingReadResult\.Summary') (with a bounded sample of source identifiers) and is never dropped silently; the same applies to nodes the extract no longer contains. The bounding box filter is not a rejection: filtered footprints simply are not emitted, while every other summary count stays complete.

The footprint sequence runs its passes during enumeration, so [Summary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadResult.Summary 'DiGi\.OSM\.Classes\.BuildingReadResult\.Summary') is complete only once the sequence has been enumerated to its end.

<b>Measured on <c>poland-261104.osm.pbf</c> (2 115 837 171 bytes, MD5 verified, 2026-11-06):</b> peak working set 2 989 649 920, 2 990 034 944 and 2 989 744 128 bytes over three consecutive runs, sampled managed memory about 2 422 000 000 bytes, 97 722 280 referenced nodes held at sixteen bytes each (1.56 GB of store), 17 815 662 building ways and 5 634 building relations producing 17 820 957 footprints plus 339 rejections - the in-scope objects balance exactly, nothing is dropped - in 312-337 s. The DiGi.Test budget fact asserts 3 300 000 000 bytes.

```csharp
public static DiGi.OSM.Classes.BuildingReadResult? Footprints_Pbf(string? path, DiGi.Geometry.Planar.Classes.BoundingBox2D? boundingBox=null);
```
#### Parameters

<a name='DiGi.OSM.Query.Footprints_Pbf(string,DiGi.Geometry.Planar.Classes.BoundingBox2D).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The path of the PBF file, or null or whitespace to return null\. The file must exist; a missing file fails with [System\.IO\.FileNotFoundException](https://learn.microsoft.com/en-us/dotnet/api/system.io.filenotfoundexception 'System\.IO\.FileNotFoundException') when enumeration starts\.

<a name='DiGi.OSM.Query.Footprints_Pbf(string,DiGi.Geometry.Planar.Classes.BoundingBox2D).boundingBox'></a>

`boundingBox` [DiGi\.Geometry\.Planar\.Classes\.BoundingBox2D](https://learn.microsoft.com/en-us/dotnet/api/digi.geometry.planar.classes.boundingbox2d 'DiGi\.Geometry\.Planar\.Classes\.BoundingBox2D')

An optional bounding box, in EPSG:4326 \(x = longitude\), that a footprint's envelope must intersect to be emitted\.

#### Returns
[BuildingReadResult](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadResult 'DiGi\.OSM\.Classes\.BuildingReadResult')  
The read result \(header, footprints, summary\), or null when [path](DiGi.OSM.md#DiGi.OSM.Query.Footprints_Pbf(string,DiGi.Geometry.Planar.Classes.BoundingBox2D).path 'DiGi\.OSM\.Query\.Footprints\_Pbf\(string, DiGi\.Geometry\.Planar\.Classes\.BoundingBox2D\)\.path') is null or whitespace\.

#### Exceptions

[System\.IO\.InvalidDataException](https://learn.microsoft.com/en-us/dotnet/api/system.io.invaliddataexception 'System\.IO\.InvalidDataException')  
Thrown when the extract is not sorted by node identifier, or when its header blob is malformed\.

<a name='DiGi.OSM.Query.PbfHeader(string)'></a>

## Query\.PbfHeader\(string\) Method

Reads the replication state declared in the header blob of a PBF file\.

OsmSharp 6.2.0 does not expose these fields, so the first blob is decoded here: the four-byte big-endian `BlobHeader` length, the `BlobHeader` protobuf, the `Blob` (zlib inflated, or used raw when uncompressed), and a scan of the `HeaderBlock` protobuf that picks fields 32, 33 and 34 while skipping every unknown field - the fields the OSM PBF specification leaves open and OsmSharp's contract omits. Absent fields stay null; nothing is defaulted.

Only the standard blob types are accepted: a blob whose declared type is not `OSMHeader`, an unsupported compression, or a truncated file fails loudly with [System\.IO\.InvalidDataException](https://learn.microsoft.com/en-us/dotnet/api/system.io.invaliddataexception 'System\.IO\.InvalidDataException') rather than reporting an empty header.

```csharp
public static DiGi.OSM.Classes.PbfHeader? PbfHeader(string? path);
```
#### Parameters

<a name='DiGi.OSM.Query.PbfHeader(string).path'></a>

`path` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The path of the PBF file, or null, empty or whitespace, or a path that does not exist, to return null\.

#### Returns
[PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader')  
The header the file declared, or null when no readable file was given\.

<a name='DiGi.OSM.Query.PbfHeader(System.IO.Stream)'></a>

## Query\.PbfHeader\(Stream\) Method

Reads the replication state declared in the header blob of a PBF stream, leaving the stream position as it was found\.

```csharp
public static DiGi.OSM.Classes.PbfHeader? PbfHeader(System.IO.Stream? stream);
```
#### Parameters

<a name='DiGi.OSM.Query.PbfHeader(System.IO.Stream).stream'></a>

`stream` [System\.IO\.Stream](https://learn.microsoft.com/en-us/dotnet/api/system.io.stream 'System\.IO\.Stream')

The stream to read, or null to return null\.

#### Returns
[PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader')  
The header the stream declared, or null when no readable stream was given\.

#### Exceptions

[System\.IO\.InvalidDataException](https://learn.microsoft.com/en-us/dotnet/api/system.io.invaliddataexception 'System\.IO\.InvalidDataException')  
Thrown when the stream is not a PBF file with a standard header blob\.
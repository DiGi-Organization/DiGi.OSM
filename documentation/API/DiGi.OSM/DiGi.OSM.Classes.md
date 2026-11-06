#### [DiGi\.OSM](DiGi.OSM.Overview.md 'DiGi\.OSM\.Overview')

## DiGi\.OSM\.Classes Namespace
### Classes

<a name='DiGi.OSM.Classes.BuildingReadCounter'></a>

## BuildingReadCounter Class

The mutable tallies a footprint read fills while its footprint sequence is enumerated: objects read, footprints emitted, and one counter plus a bounded sample per [BuildingReadRejectionReason](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason')\.

The counter exists because [BuildingReadSummary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadSummary 'DiGi\.OSM\.Classes\.BuildingReadSummary') stays an immutable `SerializableObject`: the reader increments the counter during enumeration and a caller reads the completed snapshot from [Summary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadResult.Summary 'DiGi\.OSM\.Classes\.BuildingReadResult\.Summary') once the sequence has been fully enumerated.

```csharp
public class BuildingReadCounter
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → BuildingReadCounter
### Properties

<a name='DiGi.OSM.Classes.BuildingReadCounter.FootprintCount'></a>

## BuildingReadCounter\.FootprintCount Property

Gets the number of footprints the reader emitted \(after any bounding box filter\)\.

```csharp
public long FootprintCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.InvalidCoordinateCount'></a>

## BuildingReadCounter\.InvalidCoordinateCount Property

Gets the number of objects rejected as [InvalidCoordinate](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason.InvalidCoordinate 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason\.InvalidCoordinate')\.

```csharp
public long InvalidCoordinateCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.MalformedObjectCount'></a>

## BuildingReadCounter\.MalformedObjectCount Property

Gets the number of objects rejected as [MalformedObject](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason.MalformedObject 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason\.MalformedObject')\.

```csharp
public long MalformedObjectCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.MissingMemberWayCount'></a>

## BuildingReadCounter\.MissingMemberWayCount Property

Gets the number of objects rejected as [MissingMemberWay](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason.MissingMemberWay 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason\.MissingMemberWay')\.

```csharp
public long MissingMemberWayCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.MissingNodeCount'></a>

## BuildingReadCounter\.MissingNodeCount Property

Gets the number of objects rejected as [MissingNode](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason.MissingNode 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason\.MissingNode')\.

```csharp
public long MissingNodeCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.NotBuildingCount'></a>

## BuildingReadCounter\.NotBuildingCount Property

Gets the number of objects rejected as [NotBuilding](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason.NotBuilding 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason\.NotBuilding')\.

```csharp
public long NotBuildingCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.ReferencedNodeCount'></a>

## BuildingReadCounter\.ReferencedNodeCount Property

Gets the number of distinct node identifiers the candidate ways and relations reference \- the size the bounded node cache scales with, recorded once the identifiers are collected\.

```csharp
public long ReferencedNodeCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.RejectionCount'></a>

## BuildingReadCounter\.RejectionCount Property

Gets all rejection reasons summed\.

```csharp
public long RejectionCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.Rejections'></a>

## BuildingReadCounter\.Rejections Property

Gets a bounded sample of rejected source identifiers, at most twenty entries per rejection reason, formatted as `<sourceId>: <reason>`\.

```csharp
public System.Collections.Generic.List<string> Rejections { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.OSM.Classes.BuildingReadCounter.RelationCount'></a>

## BuildingReadCounter\.RelationCount Property

Gets the number of `type=multipolygon` relations carrying a `building` tag of any value \(including `building=no`\)\.

```csharp
public long RelationCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.RingNotAssembledCount'></a>

## BuildingReadCounter\.RingNotAssembledCount Property

Gets the number of objects rejected as [RingNotAssembled](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason.RingNotAssembled 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason\.RingNotAssembled')\.

```csharp
public long RingNotAssembledCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.TooFewNodesCount'></a>

## BuildingReadCounter\.TooFewNodesCount Property

Gets the number of objects rejected as [TooFewNodes](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason.TooFewNodes 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason\.TooFewNodes')\.

```csharp
public long TooFewNodesCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.UnclosedGeometryCount'></a>

## BuildingReadCounter\.UnclosedGeometryCount Property

Gets the number of objects rejected as [UnclosedGeometry](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason.UnclosedGeometry 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason\.UnclosedGeometry')\.

```csharp
public long UnclosedGeometryCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadCounter.WayCount'></a>

## BuildingReadCounter\.WayCount Property

Gets the number of ways carrying a `building` tag of any value \(including `building=no`\)\.

```csharp
public long WayCount { get; set; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadResult'></a>

## BuildingReadResult Class

The outcome of one footprint read: the PBF header the file declared, the lazily enumerated footprints, and the run summary\.

The footprint sequence performs its passes over the file while it is enumerated, so [Summary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadResult.Summary 'DiGi\.OSM\.Classes\.BuildingReadResult\.Summary') is complete only once [Footprints](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadResult.Footprints 'DiGi\.OSM\.Classes\.BuildingReadResult\.Footprints') has been enumerated to its end; abandoning the sequence early leaves the summary partial by design rather than materialising a whole country file.

```csharp
public class BuildingReadResult
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → BuildingReadResult
### Constructors

<a name='DiGi.OSM.Classes.BuildingReadResult.BuildingReadResult(DiGi.OSM.Classes.PbfHeader,System.Collections.Generic.IEnumerable_DiGi.GIS.IO.Classes.Footprint_,DiGi.OSM.Classes.BuildingReadCounter)'></a>

## BuildingReadResult\(PbfHeader, IEnumerable\<Footprint\>, BuildingReadCounter\) Constructor

Initializes a new instance of the [BuildingReadResult](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadResult 'DiGi\.OSM\.Classes\.BuildingReadResult') class\.

```csharp
internal BuildingReadResult(DiGi.OSM.Classes.PbfHeader? pbfHeader, System.Collections.Generic.IEnumerable<DiGi.GIS.IO.Classes.Footprint>? footprints, DiGi.OSM.Classes.BuildingReadCounter? buildingReadCounter);
```
#### Parameters

<a name='DiGi.OSM.Classes.BuildingReadResult.BuildingReadResult(DiGi.OSM.Classes.PbfHeader,System.Collections.Generic.IEnumerable_DiGi.GIS.IO.Classes.Footprint_,DiGi.OSM.Classes.BuildingReadCounter).pbfHeader'></a>

`pbfHeader` [PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader')

The header the file declared, or null when it could not be read\.

<a name='DiGi.OSM.Classes.BuildingReadResult.BuildingReadResult(DiGi.OSM.Classes.PbfHeader,System.Collections.Generic.IEnumerable_DiGi.GIS.IO.Classes.Footprint_,DiGi.OSM.Classes.BuildingReadCounter).footprints'></a>

`footprints` [System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[DiGi\.GIS\.IO\.Classes\.Footprint](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.io.classes.footprint 'DiGi\.GIS\.IO\.Classes\.Footprint')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

The lazily enumerated footprints, or null when no file was given\.

<a name='DiGi.OSM.Classes.BuildingReadResult.BuildingReadResult(DiGi.OSM.Classes.PbfHeader,System.Collections.Generic.IEnumerable_DiGi.GIS.IO.Classes.Footprint_,DiGi.OSM.Classes.BuildingReadCounter).buildingReadCounter'></a>

`buildingReadCounter` [BuildingReadCounter](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadCounter 'DiGi\.OSM\.Classes\.BuildingReadCounter')

The counter the enumeration fills\.
### Properties

<a name='DiGi.OSM.Classes.BuildingReadResult.Footprints'></a>

## BuildingReadResult\.Footprints Property

Gets the footprints, emitted lazily while the sequence is enumerated\.

```csharp
public System.Collections.Generic.IEnumerable<DiGi.GIS.IO.Classes.Footprint>? Footprints { get; }
```

#### Property Value
[System\.Collections\.Generic\.IEnumerable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')[DiGi\.GIS\.IO\.Classes\.Footprint](https://learn.microsoft.com/en-us/dotnet/api/digi.gis.io.classes.footprint 'DiGi\.GIS\.IO\.Classes\.Footprint')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1 'System\.Collections\.Generic\.IEnumerable\`1')

<a name='DiGi.OSM.Classes.BuildingReadResult.Header'></a>

## BuildingReadResult\.Header Property

Gets the header the file declared \(timestamp, replication sequence, base URL, writing program\), or null when the header could not be read\.

```csharp
public DiGi.OSM.Classes.PbfHeader? Header { get; }
```

#### Property Value
[PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader')

<a name='DiGi.OSM.Classes.BuildingReadResult.Summary'></a>

## BuildingReadResult\.Summary Property

Gets a snapshot of the run so far; complete after [Footprints](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadResult.Footprints 'DiGi\.OSM\.Classes\.BuildingReadResult\.Footprints') has been fully enumerated\.

```csharp
public DiGi.OSM.Classes.BuildingReadSummary Summary { get; }
```

#### Property Value
[BuildingReadSummary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadSummary 'DiGi\.OSM\.Classes\.BuildingReadSummary')

<a name='DiGi.OSM.Classes.BuildingReadSummary'></a>

## BuildingReadSummary Class

An immutable snapshot of one footprint read: objects read, footprints emitted and one count per [BuildingReadRejectionReason](DiGi.OSM.Enums.md#DiGi.OSM.Enums.BuildingReadRejectionReason 'DiGi\.OSM\.Enums\.BuildingReadRejectionReason'), plus a bounded sample of rejected source identifiers\.

The snapshot is taken from the run's [BuildingReadCounter](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadCounter 'DiGi\.OSM\.Classes\.BuildingReadCounter'), which the reader fills while the footprint sequence is enumerated - reading [Summary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadResult.Summary 'DiGi\.OSM\.Classes\.BuildingReadResult\.Summary') before enumeration has finished reports the partial state.

```csharp
public class BuildingReadSummary : DiGi.Core.Classes.SerializableObject
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [DiGi\.Core\.Classes\.Object](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.object 'DiGi\.Core\.Classes\.Object') → [DiGi\.Core\.Classes\.SerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.serializableobject 'DiGi\.Core\.Classes\.SerializableObject') → BuildingReadSummary
### Constructors

<a name='DiGi.OSM.Classes.BuildingReadSummary.BuildingReadSummary(DiGi.OSM.Classes.BuildingReadCounter)'></a>

## BuildingReadSummary\(BuildingReadCounter\) Constructor

Initializes a new instance of the [BuildingReadSummary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadSummary 'DiGi\.OSM\.Classes\.BuildingReadSummary') class as a snapshot of the given counter\.

```csharp
public BuildingReadSummary(DiGi.OSM.Classes.BuildingReadCounter? buildingReadCounter);
```
#### Parameters

<a name='DiGi.OSM.Classes.BuildingReadSummary.BuildingReadSummary(DiGi.OSM.Classes.BuildingReadCounter).buildingReadCounter'></a>

`buildingReadCounter` [BuildingReadCounter](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadCounter 'DiGi\.OSM\.Classes\.BuildingReadCounter')

The counter to snapshot, or null for an all\-zero summary\.

<a name='DiGi.OSM.Classes.BuildingReadSummary.BuildingReadSummary(DiGi.OSM.Classes.BuildingReadSummary)'></a>

## BuildingReadSummary\(BuildingReadSummary\) Constructor

Initializes a new instance of the [BuildingReadSummary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadSummary 'DiGi\.OSM\.Classes\.BuildingReadSummary') class by copying another instance\.

```csharp
public BuildingReadSummary(DiGi.OSM.Classes.BuildingReadSummary? buildingReadSummary);
```
#### Parameters

<a name='DiGi.OSM.Classes.BuildingReadSummary.BuildingReadSummary(DiGi.OSM.Classes.BuildingReadSummary).buildingReadSummary'></a>

`buildingReadSummary` [BuildingReadSummary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadSummary 'DiGi\.OSM\.Classes\.BuildingReadSummary')

The source summary to copy\.

<a name='DiGi.OSM.Classes.BuildingReadSummary.BuildingReadSummary(System.Text.Json.Nodes.JsonObject)'></a>

## BuildingReadSummary\(JsonObject\) Constructor

Initializes a new instance of the [BuildingReadSummary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadSummary 'DiGi\.OSM\.Classes\.BuildingReadSummary') class from a JSON object\.

```csharp
public BuildingReadSummary(System.Text.Json.Nodes.JsonObject? jsonObject);
```
#### Parameters

<a name='DiGi.OSM.Classes.BuildingReadSummary.BuildingReadSummary(System.Text.Json.Nodes.JsonObject).jsonObject'></a>

`jsonObject` [System\.Text\.Json\.Nodes\.JsonObject](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject 'System\.Text\.Json\.Nodes\.JsonObject')

The JSON object containing the summary data\.
### Properties

<a name='DiGi.OSM.Classes.BuildingReadSummary.FootprintCount'></a>

## BuildingReadSummary\.FootprintCount Property

Gets the number of footprints emitted by the read \(after any bounding box filter\)\.

```csharp
public long FootprintCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.InvalidCoordinateCount'></a>

## BuildingReadSummary\.InvalidCoordinateCount Property

Gets the number of objects rejected because a referenced node carries an invalid coordinate\.

```csharp
public long InvalidCoordinateCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.MalformedObjectCount'></a>

## BuildingReadSummary\.MalformedObjectCount Property

Gets the number of objects rejected because they arrived without the structure the reader needs\.

```csharp
public long MalformedObjectCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.MissingMemberWayCount'></a>

## BuildingReadSummary\.MissingMemberWayCount Property

Gets the number of relations rejected because a member way is absent from the extract\.

```csharp
public long MissingMemberWayCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.MissingNodeCount'></a>

## BuildingReadSummary\.MissingNodeCount Property

Gets the number of objects rejected because a referenced node is absent from the extract\.

```csharp
public long MissingNodeCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.NotBuildingCount'></a>

## BuildingReadSummary\.NotBuildingCount Property

Gets the number of objects excluded because they carry `building=no`\.

```csharp
public long NotBuildingCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.ReferencedNodeCount'></a>

## BuildingReadSummary\.ReferencedNodeCount Property

Gets the number of distinct node identifiers the candidate ways and relations reference \- the size the bounded node cache scales with; zero before the identifiers have been collected\.

```csharp
public long ReferencedNodeCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.RejectionCount'></a>

## BuildingReadSummary\.RejectionCount Property

Gets all rejection reasons summed\.

```csharp
public long RejectionCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.Rejections'></a>

## BuildingReadSummary\.Rejections Property

Gets a bounded sample of rejected source identifiers \(at most twenty entries per rejection reason\), or null when nothing was rejected\.

```csharp
public System.Collections.Generic.List<string>? Rejections { get; }
```

#### Property Value
[System\.Collections\.Generic\.List&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.list-1 'System\.Collections\.Generic\.List\`1')

<a name='DiGi.OSM.Classes.BuildingReadSummary.RelationCount'></a>

## BuildingReadSummary\.RelationCount Property

Gets the number of `type=multipolygon` relations carrying a `building` tag of any value\.

```csharp
public long RelationCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.RingNotAssembledCount'></a>

## BuildingReadSummary\.RingNotAssembledCount Property

Gets the number of relations rejected because their member ways do not assemble into closed rings, or a hole is not contained by any outer ring\.

```csharp
public long RingNotAssembledCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.TooFewNodesCount'></a>

## BuildingReadSummary\.TooFewNodesCount Property

Gets the number of objects rejected because fewer than three distinct corners remain\.

```csharp
public long TooFewNodesCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.UnclosedGeometryCount'></a>

## BuildingReadSummary\.UnclosedGeometryCount Property

Gets the number of ways rejected because their node references do not form a closed outline\.

```csharp
public long UnclosedGeometryCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')

<a name='DiGi.OSM.Classes.BuildingReadSummary.WayCount'></a>

## BuildingReadSummary\.WayCount Property

Gets the number of ways carrying a `building` tag of any value\.

```csharp
public long WayCount { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')
### Methods

<a name='DiGi.OSM.Classes.BuildingReadSummary.Clone()'></a>

## BuildingReadSummary\.Clone\(\) Method

Creates a deep copy of this [BuildingReadSummary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadSummary 'DiGi\.OSM\.Classes\.BuildingReadSummary') instance\.

```csharp
public override DiGi.Core.Interfaces.ISerializableObject? Clone();
```

Implements [Clone\(\)](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.icloneableobject-1.clone 'DiGi\.Core\.Interfaces\.ICloneableObject\`1\.Clone')

#### Returns
[DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject')  
A new [BuildingReadSummary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadSummary 'DiGi\.OSM\.Classes\.BuildingReadSummary') instance with the same data, or null if cloning fails\.

<a name='DiGi.OSM.Classes.NodeCoordinateStore'></a>

## NodeCoordinateStore Class

The bounded node cache of a footprint read: one sorted array of referenced node identifiers plus two parallel arrays of coordinates, at sixteen bytes per referenced node\.

Coordinates are stored as `int32` microdegrees (units of 1e-7 degrees), which is exactly the granularity a Geofabrik extract carries, so the conversion is lossless and the whole cache stays compact: no node objects, no tags and no per-entry hash overhead are ever held.

Two markers live outside the valid microdegree range: [System\.Int32\.MinValue](https://learn.microsoft.com/en-us/dotnet/api/system.int32.minvalue 'System\.Int32\.MinValue') in [Latitudes](DiGi.OSM.Classes.md#DiGi.OSM.Classes.NodeCoordinateStore.Latitudes 'DiGi\.OSM\.Classes\.NodeCoordinateStore\.Latitudes') means the node identifier is absent from the extract, and [System\.Int32\.MaxValue](https://learn.microsoft.com/en-us/dotnet/api/system.int32.maxvalue 'System\.Int32\.MaxValue') means the node was present but its coordinate was unusable. A marker never collides with a real latitude, whose magnitude cannot exceed 900 000 000.

```csharp
internal sealed class NodeCoordinateStore
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → NodeCoordinateStore
### Constructors

<a name='DiGi.OSM.Classes.NodeCoordinateStore.NodeCoordinateStore(long[],int)'></a>

## NodeCoordinateStore\(long\[\], int\) Constructor

Initializes a new instance of the [NodeCoordinateStore](DiGi.OSM.Classes.md#DiGi.OSM.Classes.NodeCoordinateStore 'DiGi\.OSM\.Classes\.NodeCoordinateStore') class with freshly allocated, unresolved coordinate arrays\.

```csharp
public NodeCoordinateStore(long[] ids, int count);
```
#### Parameters

<a name='DiGi.OSM.Classes.NodeCoordinateStore.NodeCoordinateStore(long[],int).ids'></a>

`ids` [System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

The referenced node identifiers, sorted ascending\.

<a name='DiGi.OSM.Classes.NodeCoordinateStore.NodeCoordinateStore(long[],int).count'></a>

`count` [System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

The number of identifiers in [ids](DiGi.OSM.Classes.md#DiGi.OSM.Classes.NodeCoordinateStore.NodeCoordinateStore(long[],int).ids 'DiGi\.OSM\.Classes\.NodeCoordinateStore\.NodeCoordinateStore\(long\[\], int\)\.ids') that are actually in use\.
### Properties

<a name='DiGi.OSM.Classes.NodeCoordinateStore.Count'></a>

## NodeCoordinateStore\.Count Property

Gets the number of identifiers in use\.

```csharp
public int Count { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')

<a name='DiGi.OSM.Classes.NodeCoordinateStore.Ids'></a>

## NodeCoordinateStore\.Ids Property

Gets the referenced node identifiers, sorted ascending; only the first [Count](DiGi.OSM.Classes.md#DiGi.OSM.Classes.NodeCoordinateStore.Count 'DiGi\.OSM\.Classes\.NodeCoordinateStore\.Count') entries are in use\.

```csharp
public long[] Ids { get; }
```

#### Property Value
[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

<a name='DiGi.OSM.Classes.NodeCoordinateStore.Latitudes'></a>

## NodeCoordinateStore\.Latitudes Property

Gets the parallel array of latitudes in microdegrees, holding a marker until the node is resolved\.

```csharp
public int[] Latitudes { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

<a name='DiGi.OSM.Classes.NodeCoordinateStore.Longitudes'></a>

## NodeCoordinateStore\.Longitudes Property

Gets the parallel array of longitudes in microdegrees; only meaningful where [Latitudes](DiGi.OSM.Classes.md#DiGi.OSM.Classes.NodeCoordinateStore.Latitudes 'DiGi\.OSM\.Classes\.NodeCoordinateStore\.Latitudes') holds a real value\.

```csharp
public int[] Longitudes { get; }
```

#### Property Value
[System\.Int32](https://learn.microsoft.com/en-us/dotnet/api/system.int32 'System\.Int32')[\[\]](https://learn.microsoft.com/en-us/dotnet/api/system.array 'System\.Array')

<a name='DiGi.OSM.Classes.PbfHeader'></a>

## PbfHeader Class

The replication state a PBF file declares in its header blob: the instant the extract was cut, the replication sequence it was cut at and where the replication stream is served from\.

Geofabrik writes all three fields (`osmosis_replication_timestamp`, `osmosis_replication_sequence_number`, `osmosis_replication_base_url`); other producers may omit them, so every member is nullable and an absent field stays null instead of being reported as a default. The values are read by [PbfHeader\(string\)](DiGi.OSM.md#DiGi.OSM.Query.PbfHeader(string) 'DiGi\.OSM\.Query\.PbfHeader\(string\)') because OsmSharp 6.2.0 does not expose them.

```csharp
public class PbfHeader : DiGi.Core.Classes.SerializableObject
```

Inheritance [System\.Object](https://learn.microsoft.com/en-us/dotnet/api/system.object 'System\.Object') → [DiGi\.Core\.Classes\.Object](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.object 'DiGi\.Core\.Classes\.Object') → [DiGi\.Core\.Classes\.SerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.classes.serializableobject 'DiGi\.Core\.Classes\.SerializableObject') → PbfHeader
### Constructors

<a name='DiGi.OSM.Classes.PbfHeader.PbfHeader(DiGi.OSM.Classes.PbfHeader)'></a>

## PbfHeader\(PbfHeader\) Constructor

Initializes a new instance of the [PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader') class by copying another instance\.

```csharp
public PbfHeader(DiGi.OSM.Classes.PbfHeader? pbfHeader);
```
#### Parameters

<a name='DiGi.OSM.Classes.PbfHeader.PbfHeader(DiGi.OSM.Classes.PbfHeader).pbfHeader'></a>

`pbfHeader` [PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader')

The source header to copy\.

<a name='DiGi.OSM.Classes.PbfHeader.PbfHeader(System.Nullable_System.DateTimeOffset_,System.Nullable_long_,string,string)'></a>

## PbfHeader\(Nullable\<DateTimeOffset\>, Nullable\<long\>, string, string\) Constructor

Initializes a new instance of the [PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader') class\.

```csharp
public PbfHeader(System.Nullable<System.DateTimeOffset> timestamp, System.Nullable<long> replicationSequence, string? replicationBaseUrl, string? writingProgram);
```
#### Parameters

<a name='DiGi.OSM.Classes.PbfHeader.PbfHeader(System.Nullable_System.DateTimeOffset_,System.Nullable_long_,string,string).timestamp'></a>

`timestamp` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset 'System\.DateTimeOffset')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The instant the extract was cut, or null when the header carries no replication timestamp\.

<a name='DiGi.OSM.Classes.PbfHeader.PbfHeader(System.Nullable_System.DateTimeOffset_,System.Nullable_long_,string,string).replicationSequence'></a>

`replicationSequence` [System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

The replication sequence the extract was cut at, or null when the header carries none\.

<a name='DiGi.OSM.Classes.PbfHeader.PbfHeader(System.Nullable_System.DateTimeOffset_,System.Nullable_long_,string,string).replicationBaseUrl'></a>

`replicationBaseUrl` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The base URL of the replication stream, or null when the header carries none\.

<a name='DiGi.OSM.Classes.PbfHeader.PbfHeader(System.Nullable_System.DateTimeOffset_,System.Nullable_long_,string,string).writingProgram'></a>

`writingProgram` [System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

The program that wrote the file, or null when the header carries none\.

<a name='DiGi.OSM.Classes.PbfHeader.PbfHeader(System.Text.Json.Nodes.JsonObject)'></a>

## PbfHeader\(JsonObject\) Constructor

Initializes a new instance of the [PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader') class from a JSON object\.

```csharp
public PbfHeader(System.Text.Json.Nodes.JsonObject? jsonObject);
```
#### Parameters

<a name='DiGi.OSM.Classes.PbfHeader.PbfHeader(System.Text.Json.Nodes.JsonObject).jsonObject'></a>

`jsonObject` [System\.Text\.Json\.Nodes\.JsonObject](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.nodes.jsonobject 'System\.Text\.Json\.Nodes\.JsonObject')

The JSON object containing the header data\.
### Properties

<a name='DiGi.OSM.Classes.PbfHeader.ReplicationBaseUrl'></a>

## PbfHeader\.ReplicationBaseUrl Property

Gets the base URL of the replication stream, or null when the header carries none\.

```csharp
public string? ReplicationBaseUrl { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')

<a name='DiGi.OSM.Classes.PbfHeader.ReplicationSequence'></a>

## PbfHeader\.ReplicationSequence Property

Gets the replication sequence the extract was cut at, or null when the header carries none\.

```csharp
public System.Nullable<long> ReplicationSequence { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.Int64](https://learn.microsoft.com/en-us/dotnet/api/system.int64 'System\.Int64')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.OSM.Classes.PbfHeader.Timestamp'></a>

## PbfHeader\.Timestamp Property

Gets the instant the extract was cut \(UTC\), or null when the header carries no replication timestamp\.

```csharp
public System.Nullable<System.DateTimeOffset> Timestamp { get; }
```

#### Property Value
[System\.Nullable&lt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')[System\.DateTimeOffset](https://learn.microsoft.com/en-us/dotnet/api/system.datetimeoffset 'System\.DateTimeOffset')[&gt;](https://learn.microsoft.com/en-us/dotnet/api/system.nullable-1 'System\.Nullable\`1')

<a name='DiGi.OSM.Classes.PbfHeader.WritingProgram'></a>

## PbfHeader\.WritingProgram Property

Gets the program that wrote the file, or null when the header carries none\.

```csharp
public string? WritingProgram { get; }
```

#### Property Value
[System\.String](https://learn.microsoft.com/en-us/dotnet/api/system.string 'System\.String')
### Methods

<a name='DiGi.OSM.Classes.PbfHeader.Clone()'></a>

## PbfHeader\.Clone\(\) Method

Creates a deep copy of this [PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader') instance\.

```csharp
public override DiGi.Core.Interfaces.ISerializableObject? Clone();
```

Implements [Clone\(\)](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.icloneableobject-1.clone 'DiGi\.Core\.Interfaces\.ICloneableObject\`1\.Clone')

#### Returns
[DiGi\.Core\.Interfaces\.ISerializableObject](https://learn.microsoft.com/en-us/dotnet/api/digi.core.interfaces.iserializableobject 'DiGi\.Core\.Interfaces\.ISerializableObject')  
A new [PbfHeader](DiGi.OSM.Classes.md#DiGi.OSM.Classes.PbfHeader 'DiGi\.OSM\.Classes\.PbfHeader') instance with the same data, or null if cloning fails\.
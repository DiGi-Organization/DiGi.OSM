#### [DiGi\.OSM](DiGi.OSM.Overview.md 'DiGi\.OSM\.Overview')

## DiGi\.OSM\.Enums Namespace
### Enums

<a name='DiGi.OSM.Enums.BuildingReadRejectionReason'></a>

## BuildingReadRejectionReason Enum

States why an in\-scope OpenStreetMap object did not become a footprint\. Every reason has its own counter in [BuildingReadSummary](DiGi.OSM.Classes.md#DiGi.OSM.Classes.BuildingReadSummary 'DiGi\.OSM\.Classes\.BuildingReadSummary'), so an object the reader refuses is always reported rather than dropped silently\.

```csharp
public enum BuildingReadRejectionReason
```
### Fields

<a name='DiGi.OSM.Enums.BuildingReadRejectionReason.NotBuilding'></a>

`NotBuilding` 0

The object carries `building=no`, which states it is not a building; the object is excluded from the footprints and counted\.

<a name='DiGi.OSM.Enums.BuildingReadRejectionReason.UnclosedGeometry'></a>

`UnclosedGeometry` 1

A way whose first node identifier differs from its last one, so it does not form a closed outline\.

<a name='DiGi.OSM.Enums.BuildingReadRejectionReason.TooFewNodes'></a>

`TooFewNodes` 2

A closed way or a closed ring with fewer than four node references, or one whose coordinates collapse below three distinct corners\.

<a name='DiGi.OSM.Enums.BuildingReadRejectionReason.MissingNode'></a>

`MissingNode` 3

A way or ring references a node identifier the extract does not contain, so its position cannot be resolved\.

<a name='DiGi.OSM.Enums.BuildingReadRejectionReason.InvalidCoordinate'></a>

`InvalidCoordinate` 4

A referenced node carries a null, non\-finite or out\-of\-range coordinate\.

<a name='DiGi.OSM.Enums.BuildingReadRejectionReason.MissingMemberWay'></a>

`MissingMemberWay` 5

A multipolygon relation references a member way the extract does not contain\.

<a name='DiGi.OSM.Enums.BuildingReadRejectionReason.RingNotAssembled'></a>

`RingNotAssembled` 6

The member ways of a multipolygon relation do not chain head\-to\-tail into closed rings, or a hole is not contained by any outer ring\.

<a name='DiGi.OSM.Enums.BuildingReadRejectionReason.MalformedObject'></a>

`MalformedObject` 7

An in\-scope object arrived without the structure the reader needs \(no identifier, no node list, no member list\)\.
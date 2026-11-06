namespace DiGi.OSM.Enums
{
    /// <summary>
    /// States why an in-scope OpenStreetMap object did not become a footprint. Every reason has its own counter in <see cref="DiGi.OSM.Classes.BuildingReadSummary"/>, so an object the reader refuses is always reported rather than dropped silently.
    /// </summary>
    public enum BuildingReadRejectionReason
    {
        /// <summary>
        /// The object carries <c>building=no</c>, which states it is not a building; the object is excluded from the footprints and counted.
        /// </summary>
        NotBuilding,

        /// <summary>
        /// A way whose first node identifier differs from its last one, so it does not form a closed outline.
        /// </summary>
        UnclosedGeometry,

        /// <summary>
        /// A closed way or a closed ring with fewer than four node references, or one whose coordinates collapse below three distinct corners.
        /// </summary>
        TooFewNodes,

        /// <summary>
        /// A way or ring references a node identifier the extract does not contain, so its position cannot be resolved.
        /// </summary>
        MissingNode,

        /// <summary>
        /// A referenced node carries a null, non-finite or out-of-range coordinate.
        /// </summary>
        InvalidCoordinate,

        /// <summary>
        /// A multipolygon relation references a member way the extract does not contain.
        /// </summary>
        MissingMemberWay,

        /// <summary>
        /// The member ways of a multipolygon relation do not chain head-to-tail into closed rings, or a hole is not contained by any outer ring.
        /// </summary>
        RingNotAssembled,

        /// <summary>
        /// An in-scope object arrived without the structure the reader needs (no identifier, no node list, no member list).
        /// </summary>
        MalformedObject
    }
}

using DiGi.OSM.Enums;
using System.Collections.Generic;

namespace DiGi.OSM.Classes
{
    /// <summary>
    /// The mutable tallies a footprint read fills while its footprint sequence is enumerated: objects read, footprints emitted, and one counter plus a bounded sample per <see cref="BuildingReadRejectionReason"/>.
    /// <para>The counter exists because <see cref="BuildingReadSummary"/> stays an immutable <c>SerializableObject</c>: the reader increments the counter during enumeration and a caller reads the completed snapshot from <see cref="BuildingReadResult.Summary"/> once the sequence has been fully enumerated.</para>
    /// </summary>
    public class BuildingReadCounter
    {
        /// <summary>
        /// Gets the number of ways carrying a <c>building</c> tag of any value (including <c>building=no</c>).
        /// </summary>
        public long WayCount { get; set; }

        /// <summary>
        /// Gets the number of <c>type=multipolygon</c> relations carrying a <c>building</c> tag of any value (including <c>building=no</c>).
        /// </summary>
        public long RelationCount { get; set; }

        /// <summary>
        /// Gets the number of footprints the reader emitted (after any bounding box filter).
        /// </summary>
        public long FootprintCount { get; set; }

        /// <summary>
        /// Gets the number of distinct node identifiers the candidate ways and relations reference - the size the bounded node cache scales with, recorded once the identifiers are collected.
        /// </summary>
        public long ReferencedNodeCount { get; set; }

        /// <summary>
        /// Gets the number of objects rejected as <see cref="BuildingReadRejectionReason.NotBuilding"/>.
        /// </summary>
        public long NotBuildingCount { get; set; }

        /// <summary>
        /// Gets the number of objects rejected as <see cref="BuildingReadRejectionReason.UnclosedGeometry"/>.
        /// </summary>
        public long UnclosedGeometryCount { get; set; }

        /// <summary>
        /// Gets the number of objects rejected as <see cref="BuildingReadRejectionReason.TooFewNodes"/>.
        /// </summary>
        public long TooFewNodesCount { get; set; }

        /// <summary>
        /// Gets the number of objects rejected as <see cref="BuildingReadRejectionReason.MissingNode"/>.
        /// </summary>
        public long MissingNodeCount { get; set; }

        /// <summary>
        /// Gets the number of objects rejected as <see cref="BuildingReadRejectionReason.InvalidCoordinate"/>.
        /// </summary>
        public long InvalidCoordinateCount { get; set; }

        /// <summary>
        /// Gets the number of objects rejected as <see cref="BuildingReadRejectionReason.MissingMemberWay"/>.
        /// </summary>
        public long MissingMemberWayCount { get; set; }

        /// <summary>
        /// Gets the number of objects rejected as <see cref="BuildingReadRejectionReason.RingNotAssembled"/>.
        /// </summary>
        public long RingNotAssembledCount { get; set; }

        /// <summary>
        /// Gets the number of objects rejected as <see cref="BuildingReadRejectionReason.MalformedObject"/>.
        /// </summary>
        public long MalformedObjectCount { get; set; }

        /// <summary>
        /// Gets all rejection reasons summed.
        /// </summary>
        public long RejectionCount => NotBuildingCount + UnclosedGeometryCount + TooFewNodesCount + MissingNodeCount + InvalidCoordinateCount + MissingMemberWayCount + RingNotAssembledCount + MalformedObjectCount;

        /// <summary>
        /// Gets a bounded sample of rejected source identifiers, at most twenty entries per rejection reason, formatted as <c>&lt;sourceId&gt;: &lt;reason&gt;</c>.
        /// </summary>
        public List<string> Rejections { get; } = [];
    }
}

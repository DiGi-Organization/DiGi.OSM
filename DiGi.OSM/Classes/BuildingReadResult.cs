using DiGi.GIS.IO.Classes;
using System.Collections.Generic;

namespace DiGi.OSM.Classes
{
    /// <summary>
    /// The outcome of one footprint read: the PBF header the file declared, the lazily enumerated footprints, and the run summary.
    /// <para>The footprint sequence performs its passes over the file while it is enumerated, so <see cref="Summary"/> is complete only once <see cref="Footprints"/> has been enumerated to its end; abandoning the sequence early leaves the summary partial by design rather than materialising a whole country file.</para>
    /// </summary>
    public class BuildingReadResult
    {
        private readonly PbfHeader? pbfHeader;
        private readonly IEnumerable<Footprint>? footprints;
        private readonly BuildingReadCounter? buildingReadCounter;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildingReadResult"/> class.
        /// </summary>
        /// <param name="pbfHeader">The header the file declared, or null when it could not be read.</param>
        /// <param name="footprints">The lazily enumerated footprints, or null when no file was given.</param>
        /// <param name="buildingReadCounter">The counter the enumeration fills.</param>
        internal BuildingReadResult(PbfHeader? pbfHeader, IEnumerable<Footprint>? footprints, BuildingReadCounter? buildingReadCounter)
        {
            this.pbfHeader = pbfHeader;
            this.footprints = footprints;
            this.buildingReadCounter = buildingReadCounter;
        }

        /// <summary>
        /// Gets the header the file declared (timestamp, replication sequence, base URL, writing program), or null when the header could not be read.
        /// </summary>
        public PbfHeader? Header => pbfHeader;

        /// <summary>
        /// Gets the footprints, emitted lazily while the sequence is enumerated.
        /// </summary>
        public IEnumerable<Footprint>? Footprints => footprints;

        /// <summary>
        /// Gets a snapshot of the run so far; complete after <see cref="Footprints"/> has been fully enumerated.
        /// </summary>
        public BuildingReadSummary Summary => new(buildingReadCounter);
    }
}

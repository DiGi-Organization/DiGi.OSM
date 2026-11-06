namespace DiGi.OSM.Classes
{
    /// <summary>
    /// The bounded node cache of a footprint read: one sorted array of referenced node identifiers plus two parallel arrays of coordinates, at sixteen bytes per referenced node.
    /// <para>Coordinates are stored as <c>int32</c> microdegrees (units of 1e-7 degrees), which is exactly the granularity a Geofabrik extract carries, so the conversion is lossless and the whole cache stays compact: no node objects, no tags and no per-entry hash overhead are ever held.</para>
    /// <para>Two markers live outside the valid microdegree range: <see cref="int.MinValue"/> in <see cref="Latitudes"/> means the node identifier is absent from the extract, and <see cref="int.MaxValue"/> means the node was present but its coordinate was unusable. A marker never collides with a real latitude, whose magnitude cannot exceed 900 000 000.</para>
    /// </summary>
    internal sealed class NodeCoordinateStore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NodeCoordinateStore"/> class with freshly allocated, unresolved coordinate arrays.
        /// </summary>
        /// <param name="ids">The referenced node identifiers, sorted ascending.</param>
        /// <param name="count">The number of identifiers in <paramref name="ids"/> that are actually in use.</param>
        public NodeCoordinateStore(long[] ids, int count)
        {
            Ids = ids;
            Count = count;
            Latitudes = new int[count];
            Longitudes = new int[count];

            for (int i = 0; i < count; i++)
            {
                Latitudes[i] = int.MinValue;
            }
        }

        /// <summary>
        /// Gets the referenced node identifiers, sorted ascending; only the first <see cref="Count"/> entries are in use.
        /// </summary>
        public long[] Ids { get; }

        /// <summary>
        /// Gets the number of identifiers in use.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the parallel array of latitudes in microdegrees, holding a marker until the node is resolved.
        /// </summary>
        public int[] Latitudes { get; }

        /// <summary>
        /// Gets the parallel array of longitudes in microdegrees; only meaningful where <see cref="Latitudes"/> holds a real value.
        /// </summary>
        public int[] Longitudes { get; }
    }
}

using DiGi.Core.Classes;
using DiGi.Core.Interfaces;
using System;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace DiGi.OSM.Classes
{
    /// <summary>
    /// The replication state a PBF file declares in its header blob: the instant the extract was cut, the replication sequence it was cut at and where the replication stream is served from.
    /// <para>Geofabrik writes all three fields (<c>osmosis_replication_timestamp</c>, <c>osmosis_replication_sequence_number</c>, <c>osmosis_replication_base_url</c>); other producers may omit them, so every member is nullable and an absent field stays null instead of being reported as a default. The values are read by <see cref="DiGi.OSM.Query.PbfHeader(string)"/> because OsmSharp 6.2.0 does not expose them.</para>
    /// </summary>
    public class PbfHeader : SerializableObject
    {
        [JsonInclude, JsonPropertyName(nameof(Timestamp))]
        private readonly DateTimeOffset? timestamp;

        [JsonInclude, JsonPropertyName(nameof(ReplicationSequence))]
        private readonly long? replicationSequence;

        [JsonInclude, JsonPropertyName(nameof(ReplicationBaseUrl))]
        private readonly string? replicationBaseUrl;

        [JsonInclude, JsonPropertyName(nameof(WritingProgram))]
        private readonly string? writingProgram;

        /// <summary>
        /// Initializes a new instance of the <see cref="PbfHeader"/> class.
        /// </summary>
        /// <param name="timestamp">The instant the extract was cut, or null when the header carries no replication timestamp.</param>
        /// <param name="replicationSequence">The replication sequence the extract was cut at, or null when the header carries none.</param>
        /// <param name="replicationBaseUrl">The base URL of the replication stream, or null when the header carries none.</param>
        /// <param name="writingProgram">The program that wrote the file, or null when the header carries none.</param>
        public PbfHeader(DateTimeOffset? timestamp, long? replicationSequence, string? replicationBaseUrl, string? writingProgram)
        {
            this.timestamp = timestamp;
            this.replicationSequence = replicationSequence;
            this.replicationBaseUrl = replicationBaseUrl;
            this.writingProgram = writingProgram;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PbfHeader"/> class by copying another instance.
        /// </summary>
        /// <param name="pbfHeader">The source header to copy.</param>
        public PbfHeader(PbfHeader? pbfHeader) : base(pbfHeader)
        {
            if (pbfHeader != null)
            {
                timestamp = pbfHeader.timestamp;
                replicationSequence = pbfHeader.replicationSequence;
                replicationBaseUrl = pbfHeader.replicationBaseUrl;
                writingProgram = pbfHeader.writingProgram;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PbfHeader"/> class from a JSON object.
        /// </summary>
        /// <param name="jsonObject">The JSON object containing the header data.</param>
        public PbfHeader(JsonObject? jsonObject) : base(jsonObject)
        {
        }

        /// <summary>
        /// Creates a deep copy of this <see cref="PbfHeader"/> instance.
        /// </summary>
        /// <returns>A new <see cref="PbfHeader"/> instance with the same data, or null if cloning fails.</returns>
        public override ISerializableObject? Clone()
        {
            return new PbfHeader(this);
        }

        /// <summary>
        /// Gets the instant the extract was cut (UTC), or null when the header carries no replication timestamp.
        /// </summary>
        [JsonIgnore]
        public DateTimeOffset? Timestamp => timestamp;

        /// <summary>
        /// Gets the replication sequence the extract was cut at, or null when the header carries none.
        /// </summary>
        [JsonIgnore]
        public long? ReplicationSequence => replicationSequence;

        /// <summary>
        /// Gets the base URL of the replication stream, or null when the header carries none.
        /// </summary>
        [JsonIgnore]
        public string? ReplicationBaseUrl => replicationBaseUrl;

        /// <summary>
        /// Gets the program that wrote the file, or null when the header carries none.
        /// </summary>
        [JsonIgnore]
        public string? WritingProgram => writingProgram;
    }
}

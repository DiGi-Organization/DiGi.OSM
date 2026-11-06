using DiGi.OSM.Classes;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace DiGi.OSM
{
    public static partial class Query
    {
        /// <summary>
        /// Reads the replication state declared in the header blob of a PBF file.
        /// <para>OsmSharp 6.2.0 does not expose these fields, so the first blob is decoded here: the four-byte big-endian <c>BlobHeader</c> length, the <c>BlobHeader</c> protobuf, the <c>Blob</c> (zlib inflated, or used raw when uncompressed), and a scan of the <c>HeaderBlock</c> protobuf that picks fields 32, 33 and 34 while skipping every unknown field - the fields the OSM PBF specification leaves open and OsmSharp's contract omits. Absent fields stay null; nothing is defaulted.</para>
        /// <para>Only the standard blob types are accepted: a blob whose declared type is not <c>OSMHeader</c>, an unsupported compression, or a truncated file fails loudly with <see cref="InvalidDataException"/> rather than reporting an empty header.</para>
        /// </summary>
        /// <param name="path">The path of the PBF file, or null, empty or whitespace, or a path that does not exist, to return null.</param>
        /// <returns>The header the file declared, or null when no readable file was given.</returns>
        public static PbfHeader? PbfHeader(string? path)
        {
            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return null;
            }

            using FileStream fileStream = new(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            return PbfHeader(fileStream);
        }

        /// <summary>
        /// Reads the replication state declared in the header blob of a PBF stream, leaving the stream position as it was found.
        /// </summary>
        /// <param name="stream">The stream to read, or null to return null.</param>
        /// <returns>The header the stream declared, or null when no readable stream was given.</returns>
        /// <exception cref="InvalidDataException">Thrown when the stream is not a PBF file with a standard header blob.</exception>
        public static PbfHeader? PbfHeader(Stream? stream)
        {
            if (stream == null || !stream.CanRead)
            {
                return null;
            }

            long position = stream.CanSeek ? stream.Position : 0;

            try
            {
                byte[] ReadBytes(int count)
                {
                    if (count < 0)
                    {
                        throw new InvalidDataException($"Negative PBF blob length: {count}.");
                    }

                    byte[] buffer = new byte[count];
                    int offset = 0;
                    while (offset < count)
                    {
                        int read = stream.Read(buffer, offset, count - offset);
                        if (read <= 0)
                        {
                            throw new InvalidDataException($"Truncated PBF file: expected {count} bytes, got {offset}.");
                        }

                        offset += read;
                    }

                    return buffer;
                }

                int ReadLengthPrefix()
                {
                    byte[] buffer = ReadBytes(4);
                    int length = (buffer[0] << 24) | (buffer[1] << 16) | (buffer[2] << 8) | buffer[3];
                    if (length <= 0 || length > 65536)
                    {
                        throw new InvalidDataException($"Unexpected PBF BlobHeader length: {length}.");
                    }

                    return length;
                }

                ulong ReadVarInt(byte[] buffer, ref int offset)
                {
                    ulong result = 0;
                    int shift = 0;
                    while (offset < buffer.Length && shift < 64)
                    {
                        byte current = buffer[offset];
                        offset++;
                        result |= (ulong)(current & 0x7F) << shift;
                        if ((current & 0x80) == 0)
                        {
                            return result;
                        }

                        shift += 7;
                    }

                    throw new InvalidDataException("Malformed PBF varint field.");
                }

                byte[] InflateZlib(byte[] buffer)
                {
                    if (buffer.Length < 3 || (buffer[0] & 0x0F) != 8)
                    {
                        throw new InvalidDataException("Expected a zlib deflate stream in the PBF header blob.");
                    }

                    using MemoryStream memoryStream = new(buffer, 2, buffer.Length - 2);
                    using DeflateStream deflateStream = new(memoryStream, CompressionMode.Decompress);
                    using MemoryStream result = new();
                    deflateStream.CopyTo(result);
                    return result.ToArray();
                }

                byte[] blobHeaderBytes = ReadBytes(ReadLengthPrefix());

                string? blobType = null;
                long dataLength = -1;

                int offset = 0;
                while (offset < blobHeaderBytes.Length)
                {
                    ulong key = ReadVarInt(blobHeaderBytes, ref offset);
                    int field = (int)(key >> 3);
                    int wireType = (int)(key & 7);

                    if (wireType == 0)
                    {
                        ulong value = ReadVarInt(blobHeaderBytes, ref offset);
                        if (field == 3)
                        {
                            dataLength = value <= long.MaxValue ? (long)value : -1;
                        }
                    }
                    else if (wireType == 2)
                    {
                        ulong lengthValue = ReadVarInt(blobHeaderBytes, ref offset);
                        if (lengthValue > int.MaxValue)
                        {
                            throw new InvalidDataException($"Unreasonable PBF BlobHeader field length: {lengthValue}.");
                        }

                        int length = (int)lengthValue;
                        if (offset + length > blobHeaderBytes.Length)
                        {
                            throw new InvalidDataException("Truncated PBF BlobHeader field.");
                        }

                        if (field == 1)
                        {
                            blobType = Encoding.UTF8.GetString(blobHeaderBytes, offset, length);
                        }

                        offset += length;
                    }
                    else
                    {
                        throw new InvalidDataException($"Unsupported wire type {wireType} in the PBF BlobHeader.");
                    }
                }

                if (!string.Equals(blobType, "OSMHeader", StringComparison.Ordinal))
                {
                    throw new InvalidDataException($"Expected an OSMHeader blob at the start of the file, found '{blobType ?? "<none>"}'.");
                }

                if (dataLength <= 0 || dataLength > int.MaxValue)
                {
                    throw new InvalidDataException($"Unexpected PBF blob data length: {dataLength}.");
                }

                byte[] blobBytes = ReadBytes((int)dataLength);

                byte[]? raw = null;
                byte[]? zlib = null;

                offset = 0;
                while (offset < blobBytes.Length)
                {
                    ulong key = ReadVarInt(blobBytes, ref offset);
                    int field = (int)(key >> 3);
                    int wireType = (int)(key & 7);

                    if (wireType == 0)
                    {
                        ReadVarInt(blobBytes, ref offset);
                    }
                    else if (wireType == 2)
                    {
                        ulong lengthValue = ReadVarInt(blobBytes, ref offset);
                        if (lengthValue > int.MaxValue)
                        {
                            throw new InvalidDataException($"Unreasonable PBF Blob field length: {lengthValue}.");
                        }

                        int length = (int)lengthValue;
                        if (offset + length > blobBytes.Length)
                        {
                            throw new InvalidDataException("Truncated PBF Blob field.");
                        }

                        if (field == 1)
                        {
                            raw = new byte[length];
                            Array.Copy(blobBytes, offset, raw, 0, length);
                        }
                        else if (field == 3)
                        {
                            zlib = new byte[length];
                            Array.Copy(blobBytes, offset, zlib, 0, length);
                        }

                        offset += length;
                    }
                    else
                    {
                        throw new InvalidDataException($"Unsupported wire type {wireType} in the PBF Blob.");
                    }
                }

                byte[] headerBlockBytes;
                if (raw != null)
                {
                    headerBlockBytes = raw;
                }
                else if (zlib != null)
                {
                    headerBlockBytes = InflateZlib(zlib);
                }
                else
                {
                    throw new InvalidDataException("The PBF header blob carries no raw or zlib-compressed payload (lzma and bzip2 are not supported).");
                }

                long? seconds = null;
                long? sequence = null;
                string? replicationBaseUrl = null;
                string? writingProgram = null;

                offset = 0;
                while (offset < headerBlockBytes.Length)
                {
                    ulong key = ReadVarInt(headerBlockBytes, ref offset);
                    int field = (int)(key >> 3);
                    int wireType = (int)(key & 7);

                    if (wireType == 0)
                    {
                        ulong value = ReadVarInt(headerBlockBytes, ref offset);
                        if (field == 32)
                        {
                            seconds = value <= long.MaxValue ? (long)value : seconds;
                        }
                        else if (field == 33)
                        {
                            sequence = value <= long.MaxValue ? (long)value : sequence;
                        }
                    }
                    else if (wireType == 2)
                    {
                        ulong lengthValue = ReadVarInt(headerBlockBytes, ref offset);
                        if (lengthValue > int.MaxValue)
                        {
                            throw new InvalidDataException($"Unreasonable PBF HeaderBlock field length: {lengthValue}.");
                        }

                        int length = (int)lengthValue;
                        if (offset + length > headerBlockBytes.Length)
                        {
                            throw new InvalidDataException("Truncated PBF HeaderBlock field.");
                        }

                        if (field == 4)
                        {
                            writingProgram = Encoding.UTF8.GetString(headerBlockBytes, offset, length);
                        }
                        else if (field == 34)
                        {
                            replicationBaseUrl = Encoding.UTF8.GetString(headerBlockBytes, offset, length);
                        }

                        offset += length;
                    }
                    else if (wireType == 1)
                    {
                        offset += 8;
                    }
                    else if (wireType == 5)
                    {
                        offset += 4;
                    }
                    else
                    {
                        throw new InvalidDataException($"Unsupported wire type {wireType} in the PBF HeaderBlock.");
                    }
                }

                DateTimeOffset? timestamp = null;
                if (seconds is long seconds_Temp && seconds_Temp >= 0)
                {
                    try
                    {
                        timestamp = DateTimeOffset.FromUnixTimeSeconds(seconds_Temp);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        timestamp = null;
                    }
                }

                return new PbfHeader(timestamp, sequence, replicationBaseUrl, writingProgram);
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = position;
                }
            }
        }
    }
}

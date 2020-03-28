using System;
using System.IO;
using System.Text;

namespace Helion.Core.Util.Bytes
{
    /// <summary>
    /// A convenience class for reading bytes from some source.
    /// </summary>
    /// <remarks>
    /// If constructed from a stream, the byte reader will not close or dispose
    /// the underlying stream. That is the callers job. However if the stream
    /// is closed and then the byte reader is used, an exception will likely
    /// result. Further, reading past the end may not throw depending on the
    /// underlying implementation, as some C# implementations will not throw if
    /// trying to read past the end, but rather only read until the end and
    /// perform an early return with not enough data.
    /// </remarks>
    public abstract class ByteReader
    {
        /// <summary>
        /// How many bytes are in the underlying wrapped byte buffer/stream.
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// Gets or sets the offset of the reader. If the index is set to a
        /// negative number, it may throw an exception.
        /// </summary>
        /// <remarks>
        /// This implementation will perform seeking on its underlying stream.
        /// Because it is using a BinaryReader, this is safe to do as per link:
        /// https://stackoverflow.com/questions/19134172/is-it-safe-to-use-stream-seek-when-a-binaryreader-is-open
        /// </remarks>
        public int Offset
        {
            get => (int)Stream.BaseStream.Position;
            set => Stream.BaseStream.Seek(value, SeekOrigin.Begin);
        }

        /// <summary>
        /// The underlying stream that helps is in reading data.
        /// </summary>
        protected readonly BinaryReader Stream;

        /// <summary>
        /// Creates a new byte reader from a series of bytes.
        /// </summary>
        /// <param name="bytes">A series of bytes to read.</param>
        protected ByteReader(byte[] bytes)
        {
            Length = bytes.Length;
            Stream = new BinaryReader(new MemoryStream(bytes));
        }

        /// <summary>
        /// Creates a new byte reader from an underlying stream.
        /// </summary>
        /// <param name="stream">The stream to make the reader out of.</param>
        protected ByteReader(BinaryReader stream)
        {
            Length = (int)stream.BaseStream.Length;
            Stream = stream;
        }

        /// <summary>
        /// Creates a byte reader from the ordering provided.
        /// </summary>
        /// <param name="order">The byte order.</param>
        /// <param name="data">The data to make the reader from.</param>
        /// <returns>The appropriate byte reader for the architecture of the
        /// computer.</returns>
        public static ByteReader From(ByteOrder order, byte[] data)
        {
            if (BitConverter.IsLittleEndian)
            {
                if (order == ByteOrder.Big)
                    return new NonNativeByteReader(data);
                return new NativeByteReader(data);
            }

            if (order == ByteOrder.Big)
                return new NativeByteReader(data);
            return new NonNativeByteReader(data);
        }

        /// <summary>
        /// Creates a byte reader from the ordering provided.
        /// </summary>
        /// <param name="order">The byte order.</param>
        /// <param name="stream">The stream to make the reader from.</param>
        /// <returns>The appropriate byte reader for the architecture of the
        /// computer.</returns>
        public static ByteReader From(ByteOrder order, BinaryReader stream)
        {
            if (BitConverter.IsLittleEndian)
            {
                if (order == ByteOrder.Big)
                    return new NonNativeByteReader(stream);
                return new NativeByteReader(stream);
            }

            if (order == ByteOrder.Big)
                return new NativeByteReader(stream);
            return new NonNativeByteReader(stream);
        }

        /// <summary>
        /// Reads a byte at the current offset.
        /// </summary>
        /// <returns>A byte at the current offset.</returns>
        public byte Byte() => Stream.ReadByte();

        /// <summary>
        /// Reads a byte at the offset provided. The underlying stream pointer
        /// will not be advanced.
        /// </summary>
        /// <param name="offset">The offset to read at.</param>
        /// <returns>The byte at the offset.</returns>
        public byte Byte(int offset)
        {
            int oldOffset = Offset;

            Offset = offset;
            byte b = Byte();
            Offset = oldOffset;

            return b;
        }

        /// <summary>
        /// Reads a length of bytes from the underlying stream.
        /// </summary>
        /// <remarks>
        /// If not enough data is left, it will either throw or return only the
        /// remaining data left. This is implementation defined.
        /// </remarks>
        /// <param name="length">The number of bytes to read.</param>
        /// <returns>An array of the length provided with all of the bytes
        /// read.</returns>
        public byte[] Bytes(int length) => Stream.ReadBytes(length);

        /// <summary>
        /// Reads a length of bytes from the underlying stream at some offset.
        /// It will not advance the pointer.
        /// </summary>
        /// <remarks>
        /// If not enough data is left, it will either throw or return only the
        /// remaining data left. This is implementation defined.
        /// </remarks>
        /// <param name="length">The number of bytes to read.</param>
        /// <param name="offset">The offset to read at.</param>
        /// <returns>An array of the length provided with all of the bytes
        /// read at the offset provided.</returns>
        public byte[] Bytes(int length, int offset)
        {
            int oldOffset = Offset;

            Offset = offset;
            byte[] bytes = Bytes(length);
            Offset = oldOffset;

            return bytes;
        }

        /// <summary>
        /// Reads the following primitive at the current stream position.
        /// </summary>
        /// <returns>The primitive that was read.</returns>
        public abstract short Short();

        /// <summary>
        /// Reads the following primitive at the stream offset provided.
        /// </summary>
        /// <param name="offset">The offset from the beginning to start reading
        /// at.</param>
        /// <returns>The primitive that was read.</returns>
        public short Short(int offset)
        {
            int oldOffset = Offset;

            Offset = offset;
            short s = Short();
            Offset = oldOffset;

            return s;
        }

        /// <summary>
        /// Reads the following primitive at the current stream position.
        /// </summary>
        /// <returns>The primitive that was read.</returns>
        public ushort UShort() => (ushort)Short();

        /// <summary>
        /// Reads the following primitive at the stream offset provided.
        /// </summary>
        /// <param name="offset">The offset from the beginning to start reading
        /// at.</param>
        /// <returns>The primitive that was read.</returns>
        public ushort UShort(int offset) => (ushort)Short(offset);

        /// <summary>
        /// Reads the following primitive at the current stream position.
        /// </summary>
        /// <returns>The primitive that was read.</returns>
        public abstract int Int();

        /// <summary>
        /// Reads the following primitive at the stream offset provided.
        /// </summary>
        /// <param name="offset">The offset from the beginning to start reading
        /// at.</param>
        /// <returns>The primitive that was read.</returns>
        public int Int(int offset)
        {
            int oldOffset = Offset;

            Offset = offset;
            int i = Int();
            Offset = oldOffset;

            return i;
        }

        /// <summary>
        /// Reads the following primitive at the current stream position.
        /// </summary>
        /// <returns>The primitive that was read.</returns>
        public uint UInt() => (uint)Int();

        /// <summary>
        /// Reads the following primitive at the stream offset provided.
        /// </summary>
        /// <param name="offset">The offset from the beginning to start reading
        /// at.</param>
        /// <returns>The primitive that was read.</returns>
        public uint UInt(int offset) => (uint)Int(offset);

        /// <summary>
        /// Reads the following primitive at the current stream position.
        /// </summary>
        /// <returns>The primitive that was read.</returns>
        public abstract long Long();

        /// <summary>
        /// Reads the following primitive at the stream offset provided.
        /// </summary>
        /// <param name="offset">The offset from the beginning to start reading
        /// at.</param>
        /// <returns>The primitive that was read.</returns>
        public long Long(int offset)
        {
            int oldOffset = Offset;

            Offset = offset;
            long l = Long();
            Offset = oldOffset;

            return l;
        }

        /// <summary>
        /// Reads the following primitive at the current stream position.
        /// </summary>
        /// <returns>The primitive that was read.</returns>
        public ulong ULong() => (ulong)Long();

        /// <summary>
        /// Reads the following primitive at the stream offset provided.
        /// </summary>
        /// <param name="offset">The offset from the beginning to start reading
        /// at.</param>
        /// <returns>The primitive that was read.</returns>
        public ulong ULong(int offset) => (ulong)Long(offset);

        // TODO: This was a .NET Core 3.0+ thing.
        // /// <summary>
        // /// Reads the following primitive at the current stream position.
        // /// </summary>
        // /// <returns>The primitive that was read.</returns>
        // public float Float() => BitConverter.Int32BitsToSingle(Int());
        //
        // /// <summary>
        // /// Reads the following primitive at the stream offset provided.
        // /// </summary>
        // /// <param name="offset">The offset from the beginning to start reading
        // /// at.</param>
        // /// <returns>The primitive that was read.</returns>
        // public float Float(int offset) => BitConverter.Int32BitsToSingle(Int(offset));

        /// <summary>
        /// Reads the following primitive at the current stream position.
        /// </summary>
        /// <returns>The primitive that was read.</returns>
        public double Double() => BitConverter.Int64BitsToDouble(Long());

        /// <summary>
        /// Reads the following primitive at the stream offset provided.
        /// </summary>
        /// <param name="offset">The offset from the beginning to start reading
        /// at.</param>
        /// <returns>The primitive that was read.</returns>
        public double Double(int offset) => BitConverter.Int64BitsToDouble(Long(offset));

        /// <summary>
        /// Reads an ASCII string of the length provided.
        /// </summary>
        /// <param name="length">The number of characters.</param>
        /// <returns>A new ASCII string.</returns>
        public string String(int length)
        {
            StringBuilder builder = new StringBuilder();

            for (int i = 0; i < length; i++)
                builder.Append((char)Byte());

            return builder.ToString();
        }

        /// <summary>
        /// Reads an ASCII string of the length provided. Does not advance the
        /// underlying stream.
        /// </summary>
        /// <param name="length">The number of characters.</param>
        /// <param name="offset">The offset to read at.</param>
        /// <returns>A new ASCII string.</returns>
        public string String(int length, int offset)
        {
            int oldOffset = Offset;

            Offset = offset;
            string str = String(length);
            Offset = oldOffset;

            return str;
        }

        /// <summary>
        /// Reads a null terminated string. The null terminated character is
        /// not included in the string. If there is no null terminator, this
        /// will throw an exception.
        /// </summary>
        /// <returns>The string (without the null terminator).</returns>
        public string NullTerminatedString()
        {
            StringBuilder stringBuilder = new StringBuilder();

            char c = (char)Byte();
            while (c != '\0')
            {
                stringBuilder.Append(c);
                c = (char)Byte();
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Reads a null terminated string until either a null terminator is
        /// reached or the length is reached. The null terminated character is
        /// not included in the string.
        /// </summary>
        /// <param name="maxLength">How many characters to read before ending
        /// the string.</param>
        /// <returns>The string (without the null terminator).</returns>
        public string NullTerminatedString(int maxLength)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < maxLength; i++)
            {
                char c = (char)Byte();
                if (c == '\0')
                    break;

                stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Reads a string of the length provided, but drops null terminators
        /// upon the first encounter and returns the string read thus far.
        /// </summary>
        /// <remarks>
        /// This allows us to read a set of characters and still advance the
        /// internal pointer by the length provided, but without reading any
        /// of the null terminators. As soon as one null terminator is
        /// encountered, it will skip to the end. This way it accommodates
        /// the case where we have 8 characters to read and some corruption
        /// causes valid characters to appear after a null terminator.
        /// </remarks>
        /// <example>
        /// Suppose we had ['a', 'b', 'c', 'd', 0, 0, 0, 0, 'h', 'i']. Then
        /// calling this with a length of 8 would return "abcd", but the pointer
        /// would be at 'h'.
        /// </example>
        /// <param name="length">How many characters to read. The stream will
        /// be advanced by this amount regardless of whether it finds null
        /// terminators or not.</param>
        /// <returns>The string (without the null terminators).</returns>
        public string StringWithoutNulls(int length)
        {
            StringBuilder stringBuilder = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                char c = (char)Byte();
                if (c == '\0')
                {
                    Skip(length - i - 1);
                    break;
                }

                stringBuilder.Append(c);
            }

            return stringBuilder.ToString();
        }

        /// <summary>
        /// Reads a null terminated string until either a null terminator is
        /// reached or the length is reached. The null terminated character is
        /// not included in the string. This will read at the offset provided
        /// and not advance the internal offset pointer.
        /// </summary>
        /// <param name="maxLength">How many characters to read before ending
        /// the string.</param>
        /// <param name="offset">The offset to start reading at.</param>
        /// <returns>The string (without the null terminator).</returns>
        public string NullTerminatedString(int maxLength, int offset)
        {
            int oldOffset = Offset;

            Offset = offset;
            string str = NullTerminatedString(maxLength);
            Offset = oldOffset;

            return str;
        }

        /// <summary>
        /// Checks if there are still a requested amount bytes remaining to be
        /// read.
        /// </summary>
        /// <remarks>
        /// Not intended for negative offsets. Zero can return false if the
        /// stream position is past the end of the array.
        /// </remarks>
        /// <param name="byteAmount">How many bytes to read.</param>
        /// <returns>True if there are at least the provided bytes left to be
        /// safely read, false if not.</returns>
        public bool HasRemaining(int byteAmount) => Stream.BaseStream.Position + byteAmount <= Length;

        /// <summary>
        /// Skips ahead by the amount of bytes provided.
        /// </summary>
        /// <param name="amount">The number of bytes to skip.</param>
        public void Skip(int amount)
        {
            Stream.BaseStream.Seek(amount, SeekOrigin.Current);
        }

        /// <summary>
        /// Moves the stream pointer backwards by some amount of bytes.
        /// </summary>
        /// <param name="amount">The amount to move the stream backwards.
        /// </param>
        public void Rewind(int amount)
        {
            Skip(-amount);
        }
    }
}

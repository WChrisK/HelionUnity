using System.IO;

namespace Helion.Util.Bytes
{
    /// <summary>
    /// A byte reader that uses the native architecture.
    /// </summary>
    public sealed class NativeByteReader : ByteReader
    {
        /// <summary>
        /// Creates a native byte reader from a series of bytes.
        /// </summary>
        /// <param name="bytes">The bytes to wrap around.</param>
        public NativeByteReader(byte[] bytes) : base(bytes)
        {
        }

        /// <summary>
        /// Creates a native byte reader from a stream of bytes.
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        public NativeByteReader(BinaryReader stream) : base(stream)
        {
        }

        public override short Short() => Stream.ReadInt16();

        public override int Int() => Stream.ReadInt32();

        public override long Long() => Stream.ReadInt64();
    }
}

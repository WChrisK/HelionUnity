using System;
using System.IO;

namespace Helion.Core.Util.Bytes
{
    /// <summary>
    /// A byte reader that uses the native architecture.
    /// </summary>
    public sealed class NonNativeByteReader : ByteReader
    {
        /// <summary>
        /// Creates a native byte reader from a series of bytes.
        /// </summary>
        /// <param name="bytes">The bytes to wrap around.</param>
        public NonNativeByteReader(byte[] bytes) : base(bytes)
        {
        }

        /// <summary>
        /// Creates a native byte reader from a stream of bytes.
        /// </summary>
        /// <param name="stream">The stream to use.</param>
        public NonNativeByteReader(BinaryReader stream) : base(stream)
        {
        }

        public override short Short()
        {
            byte[] data = Bytes(2);
            Array.Reverse(data);
            return BitConverter.ToInt16(data, 0);
        }

        public override int Int()
        {
            byte[] data = Bytes(4);
            Array.Reverse(data);
            return BitConverter.ToInt32(data, 0);
        }

        public override long Long()
        {
            byte[] data = Bytes(8);
            Array.Reverse(data);
            return BitConverter.ToInt64(data, 0);
        }
    }
}

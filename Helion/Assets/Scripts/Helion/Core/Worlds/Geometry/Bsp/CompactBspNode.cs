using Helion.Core.Util.Extensions;
using Helion.Core.Util.Geometry.Segments;

namespace Helion.Core.Worlds.Geometry.Bsp
{
    /// <summary>
    /// A compact version of a BSP node for quick lookups and low memory
    /// footprints.
    /// </summary>
    public struct CompactBspNode
    {
        /// <summary>
        /// The bit set when the child is a subsector.
        /// </summary>
        public const uint IsSubsectorBit = 0x80000000;

        /// <summary>
        /// The left child. Use <see cref="LeftChildWithoutBit"/> for getting
        /// the index.
        /// </summary>
        public readonly uint LeftChildBits;

        /// <summary>
        /// The right child. Use <see cref="LeftChildWithoutBit"/> for getting
        /// the index.
        /// </summary>
        public readonly uint RightChildBits;

        /// <summary>
        /// The splitter used to determine the side to branch on when doing a
        /// BSP traversal.
        /// </summary>
        public readonly Line2F Splitter;

        public bool LeftIsSubsector => LeftChildBits.HasBits(IsSubsectorBit);
        public bool RightIsSubsector => RightChildBits.HasBits(IsSubsectorBit);
        public uint LeftChildWithoutBit => LeftChildBits & ~IsSubsectorBit;
        public uint RightChildWithoutBit => RightChildBits & ~IsSubsectorBit;

        public CompactBspNode(uint leftChildBits, uint rightChildBits, Line2F splitter)
        {
            LeftChildBits = leftChildBits;
            RightChildBits = rightChildBits;
            Splitter = splitter;
        }
    }
}

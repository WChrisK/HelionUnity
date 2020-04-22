using Helion.Core.Util.Geometry;
using Helion.Core.Util.Geometry.Segments;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry.Bsp
{
    /// <summary>
    /// A node in a BSP tree.
    /// </summary>
    public struct BspNode
    {
        public const uint ChildBit = 0x80000000U;

        public readonly uint LeftChild;
        public readonly uint RightChild;
        public readonly Line2F Splitter;

        public bool IsLeftSubsector => (LeftChild & ChildBit) == ChildBit;
        public bool IsRightSubsector => (RightChild & ChildBit) == ChildBit;
        public int LeftSubsectorIndex => (int)(LeftChild & ~ChildBit);
        public int RightSubsectorIndex => (int)(RightChild & ~ChildBit);

        public BspNode(uint leftBits, uint rightBits, Line2F splitter)
        {
            LeftChild = leftBits;
            RightChild = rightBits;
            Splitter = splitter;
        }

        /// <summary>
        /// Checks if the point is on the right side of the splitter or not.
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if on the right side, false if not.</returns>
        public bool OnRight(Vector2 point) => Splitter.OnRight(point);
    }
}

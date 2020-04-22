using System.Collections.Generic;
using Helion.Core.Util.Geometry.Graphs;
using Helion.Core.Util.Geometry.Vectors;

namespace Helion.Bsp.Geometry
{
    /// <summary>
    /// A vertex in a BSP map.
    /// </summary>
    public class BspVertex : Vector2D, IGraphVertex
    {
        /// <summary>
        /// The index of this vertex. Can be used for quick comparisons, and
        /// knowing where it is positioned in the vertex allocator.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// All the outbound edges from this vertex.
        /// </summary>
        public readonly List<BspSegment> Edges = new List<BspSegment>();

        /// <summary>
        /// Creates a new BSP vertex.
        /// </summary>
        /// <param name="position">The position of the vertex.</param>
        /// <param name="index">The index of the vertex.</param>
        public BspVertex(Vec2D position, int index) : base(position)
        {
            Index = index;
        }

        public IReadOnlyList<IGraphEdge> GetEdges() => Edges;

        public override string ToString() => $"{base.ToString()} (index = {Index}, edgeCount = {Edges.Count})";
    }
}

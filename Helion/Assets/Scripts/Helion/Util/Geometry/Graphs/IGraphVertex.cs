using System.Collections.Generic;

namespace Helion.Util.Geometry.Graphs
{
    /// <summary>
    /// A vertex in a graph.
    /// </summary>
    public interface IGraphVertex
    {
        /// <summary>
        /// Gets the edges (if any) this is connected to.
        /// </summary>
        /// <returns>The edges that connect to this vertex.</returns>
        IReadOnlyList<IGraphEdge> GetEdges();
    }
}

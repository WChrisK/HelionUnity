namespace Helion.Util.Geometry.Graphs
{
    /// <summary>
    /// An edge in a graph.
    /// </summary>
    public interface IGraphEdge
    {
        /// <summary>
        /// Gets the starting vertex.
        /// </summary>
        /// <returns>The starting vertex.</returns>
        IGraphVertex GetStart();

        /// <summary>
        /// Gets the ending vertex.
        /// </summary>
        /// <returns>The ending vertex.</returns>
        IGraphVertex GetEnd();
    }
}

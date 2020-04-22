using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Util.Geometry.Graphs
{
    /// <summary>
    /// A graph of discrete points and edges.
    /// </summary>
    /// <typeparam name="V">The vertex class.</typeparam>
    /// <typeparam name="E">The edge class.</typeparam>
    public abstract class Graph<V, E> where V : IGraphVertex where E : IGraphEdge
    {
        /// <summary>
        /// Performs a traversal from some starting edge and vertex.
        /// </summary>
        /// <remarks>
        /// The results are undefined if the edge does not contain the vertex.
        /// </remarks>
        /// <param name="start">The start vertex.</param>
        /// <param name="edge">An edge that contains the start vertex.</param>
        /// <param name="func">The traversal function that decides where to
        /// branch for each edge. If it is an N-ary graph, this function is
        /// used to select which edge should be followed.</param>
        public static void Traverse(V start, E edge, Func<V, V, E, (GraphIterationStatus status, V nextVertex, E nextEdge)> func)
        {
            V prev = start;
            V current = (V)(edge.GetStart().Equals(prev) ? edge.GetEnd() : edge.GetStart());
            E currentEdge = edge;

            while (true)
            {
                (GraphIterationStatus status, V nextVertex, E nextEdge) = func(prev, current, currentEdge);

                if (status == GraphIterationStatus.Stop)
                    break;

                prev = current;
                current = nextVertex;
                currentEdge = nextEdge;
            }
        }

        /// <summary>
        /// Starts a traversal from a random vertex.
        /// </summary>
        /// <param name="func">The traversal function that decides where to
        /// branch for each edge. If it is an N-ary graph, this function is
        /// used to select which edge should be followed.</param>
        public void Traverse(Func<V, V, E, (GraphIterationStatus status, V nextVertex, E nextEdge)> func)
        {
            if (GetEdges().Empty())
                return;

            E edge = GetEdges().First();
            V start = (V)edge.GetStart();
            Traverse(start, edge, func);
        }

        /// <summary>
        /// Gets all the vertices for this graph.
        /// </summary>
        /// <returns>The vertices for this graph.</returns>
        protected abstract IEnumerable<V> GetVertices();

        /// <summary>
        /// Gets all the edges for this graph.
        /// </summary>
        /// <returns>The edges for this graph.</returns>
        protected abstract IEnumerable<E> GetEdges();
    }
}

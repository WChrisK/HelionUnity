using System.Collections.Generic;
using System.Linq;
using Helion.Util.Extensions;
using UnityEngine;

namespace Helion.Bsp.Geometry
{
    /// <summary>
    /// A table that allows quick lookups based on two vertex indices.
    /// </summary>
    public class SegmentLookupTable
    {
        private readonly Dictionary<int, VertexSegmentPairList> table = new Dictionary<int, VertexSegmentPairList>();

        /// <summary>
        /// Gets the segment if it exists in the table.
        /// </summary>
        /// <param name="firstIndex">The first vertex index.</param>
        /// <param name="secondIndex">The second vertex index.</param>
        /// <returns>The segment if one exists, or null if not.</returns>
        public BspSegment this[int firstIndex, int secondIndex] => TryGetValue(firstIndex, secondIndex, out BspSegment seg) ? seg : null;

        /// <summary>
        /// Clears the table of all segments.
        /// </summary>
        public void Clear()
        {
            table.Clear();
        }

        /// <summary>
        /// Adds a new segment to be tracked by this table.
        /// </summary>
        /// <param name="segment">The segment to add. This should not already
        /// be in the table.</param>
        public void Add(BspSegment segment)
        {
            (int minIndex, int maxIndex) = segment.StartIndex.MinMax(segment.EndIndex);

            if (table.TryGetValue(minIndex, out VertexSegmentPairList vertexSegPairs))
            {
                vertexSegPairs.Add(maxIndex, segment);
                return;
            }

            VertexSegmentPairList pairList = new VertexSegmentPairList();
            pairList.Add(maxIndex, segment);
            table[minIndex] = pairList;
        }

        /// <summary>
        /// Checks if a segment is in this table with the provided indices. The
        /// order of indices does not matter.
        /// </summary>
        /// <param name="firstVertexIndex">The first index.</param>
        /// <param name="secondVertexIndex">The second index.</param>
        /// <returns>True if it exists, false if not.</returns>
        public bool Contains(int firstVertexIndex, int secondVertexIndex)
        {
            (int minIndex, int maxIndex) = firstVertexIndex.MinMax(secondVertexIndex);

            if (table.TryGetValue(minIndex, out VertexSegmentPairList vertexSegPairs))
                return vertexSegPairs.Contains(maxIndex);
            return false;
        }

        /// <summary>
        /// Tries to get the value provided. Order of the indices does not have
        /// any effect on the result.
        /// </summary>
        /// <param name="firstVertexIndex">The first vertex index.</param>
        /// <param name="secondVertexIndex">The second vertex index.</param>
        /// <param name="segment">The segment to be set with the value if it
        /// does exist.</param>
        /// <returns>True if it does exist, false if not (and is unsafe to
        /// use the out value).</returns>
        public bool TryGetValue(int firstVertexIndex, int secondVertexIndex, out BspSegment segment)
        {
            (int minIndex, int maxIndex) = firstVertexIndex.MinMax(secondVertexIndex);

            if (table.TryGetValue(minIndex, out VertexSegmentPairList vertexSegPairs))
            {
                if (vertexSegPairs.TryGetSegIndex(maxIndex, out BspSegment foundSeg))
                {
                    segment = foundSeg;
                    return true;
                }
            }

            segment = default;
            return false;
        }

        private class VertexSegmentPairList
        {
            private readonly List<(int vertexIndex, BspSegment segment)> pairs = new List<(int, BspSegment)>();

            internal bool Contains(int largerVertexIndex)
            {
                return pairs.Any(pair => pair.vertexIndex == largerVertexIndex);
            }

            internal void Add(int maxIndex, BspSegment segment)
            {
                Debug.Assert(!Contains(maxIndex), "Trying to add the same vertex/seg pair twice");

                pairs.Add((maxIndex, segment));
            }

            internal bool TryGetSegIndex(int maxIndex, out BspSegment segment)
            {
                for (int i = 0; i < pairs.Count; i++)
                {
                    (int vertexIndex, BspSegment seg) = pairs[i];
                    if (maxIndex == vertexIndex)
                    {
                        segment = seg;
                        return true;
                    }
                }

                segment = default;
                return false;
            }
        }
    }
}

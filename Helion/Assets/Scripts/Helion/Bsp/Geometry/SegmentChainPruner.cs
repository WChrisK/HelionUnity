using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Logging;
using MoreLinq;
using UnityEngine;

namespace Helion.Bsp.Geometry
{
    /// <summary>
    /// Identifies and handles chains of segments which should not be eligible
    /// for consideration when doing BSP building.
    /// </summary>
    /// <remarks>
    /// <para>The algorithm works as follows. It will take a series of segments
    /// and create a graph out of all the vertices. It will then remove each
    /// terminal chain it finds until none are left. In a completely degenerate
    /// map, this could include every single vertex.</para>
    /// <para>It starts out by indexing all the terminal nodes in a terminal
    /// chain. After that, it will recursively cut away the chain until it
    /// reaches a node that is no longer terminal (ex: a node that has 3
    /// connections, which then becomes 2 after we cut off the chain) or it
    /// reaches the other terminating end of the chain.</para>
    /// <para>This process keeps getting repeated until the set of terminal
    /// nodes are empty. At the end, every segment that is removed is placed
    /// into the <see cref="PrunedSegments"/> set.</para>
    /// </remarks>
    public class SegmentChainPruner
    {
        private static readonly Log Log = LogManager.Instance();

        /// <summary>
        /// A list of all the pruned segments. These may harmless lines or they
        /// could be lines that would cause problems with BSP building.
        /// </summary>
        public readonly HashSet<BspSegment> PrunedSegments = new HashSet<BspSegment>();

        private readonly SegmentLookupTable segmentTable = new SegmentLookupTable();
        private readonly Dictionary<int, List<int>> vertexAdjacencyList = new Dictionary<int, List<int>>();
        private readonly HashSet<int> terminalChainTails = new HashSet<int>();

        /// <summary>
        /// Goes through all the segments and prunes all the terminal chains.
        /// </summary>
        /// <param name="segments">The segments to prune. This list will be
        /// modified.</param>
        /// <returns>A new list of the non-pruned segments. This may return the
        /// list passed in if it was unchanged, or it may return a completely
        /// new list.</returns>
        public static List<BspSegment> Prune(List<BspSegment> segments)
        {
            SegmentChainPruner segmentChainPruner = new SegmentChainPruner();
            return segmentChainPruner.PerformPrune(segments);
        }

        private List<BspSegment> PerformPrune(List<BspSegment> segments)
        {
            ClearDataStructures();
            AddSegmentsToAdjacencyList(segments);
            DiscoverTerminalChains();
            RemoveAllTerminalChains();

            if (PrunedSegments.Count > 0)
                Log.Debug("BSP builder pruned {0} dangling segments", PrunedSegments.Count);

            return CalculatePrunedSegments(segments);
        }

        private List<BspSegment> CalculatePrunedSegments(List<BspSegment> segments)
        {
            if (PrunedSegments.Empty())
                return segments;
            return segments.Where(seg => !PrunedSegments.Contains(seg)).ToList();
        }

        private void ClearDataStructures()
        {
            PrunedSegments.Clear();
            segmentTable.Clear();
            vertexAdjacencyList.Clear();
            terminalChainTails.Clear();
        }

        private void AddSegmentsToAdjacencyList(List<BspSegment> segments)
        {
            foreach (BspSegment segment in segments)
            {
                segmentTable.Add(segment);
                AddToAdjacencyList(segment.StartIndex, segment.EndIndex);
                AddToAdjacencyList(segment.EndIndex, segment.StartIndex);
            }

            void AddToAdjacencyList(int beginIndex, int endIndex)
            {
                if (vertexAdjacencyList.TryGetValue(beginIndex, out List<int> indices))
                    indices.Add(endIndex);
                else
                    vertexAdjacencyList[beginIndex] = new List<int> { endIndex };
            }
        }

        private void DiscoverTerminalChains()
        {
            // This is a very annoying workaround for the library's resolution
            // issues. Is there not a way to make it 'just work' the normal way
            // at all?
            MoreEnumerable.ForEach(vertexAdjacencyList.Where(intToListPair => intToListPair.Value.Count == 1),
                intToListPair => terminalChainTails.Add(intToListPair.Key));
        }

        private void RemoveAllTerminalChains()
        {
            // Need to clone so we don't mutate while iterating.
            foreach (int index in terminalChainTails.ToArray())
            {
                // It is possible we removed it while trimming a chain. This
                // will occur when removing double-ended terminal chain.
                if (!terminalChainTails.Contains(index))
                    continue;

                (int endingIndex, bool wasDoubleEnded) = RemoveTerminalChain(index);
                terminalChainTails.Remove(index);

                if (wasDoubleEnded)
                    terminalChainTails.Remove(endingIndex);
            }
        }

        private (int endingIndex, bool wasDoubleEnded) RemoveTerminalChain(int index)
        {
            Debug.Assert(vertexAdjacencyList.ContainsKey(index), "Vertex index was somehow not indexed");
            Debug.Assert(vertexAdjacencyList[index].Count == 1, "Trying to remove a non-terminal chain");

            List<int> adjacentIndices = vertexAdjacencyList[index];
            Debug.Assert(adjacentIndices[0] != index, "Terminal chain has a self-referenced index");

            return RemoveTerminalChainIteratively(index, adjacentIndices[0]);
        }

        private (int endingIndex, bool wasDoubleEnded) RemoveTerminalChainIteratively(int currentIndex, int nextIndex)
        {
            while (true)
            {
                PruneSegment(currentIndex, nextIndex);

                if (!vertexAdjacencyList.TryGetValue(nextIndex, out List<int> nextIndexList))
                    return (nextIndex, true);

                if (nextIndexList.Count >= 2)
                    return (nextIndex, false);

                int adjacentNextIndex = (nextIndexList[0] == currentIndex ? nextIndexList[1] : nextIndexList[0]);
                currentIndex = nextIndex;
                nextIndex = adjacentNextIndex;
            }
        }

        private void PruneSegment(int currentIndex, int nextIndex)
        {
            BspSegment segment = segmentTable[currentIndex, nextIndex];

            if (segment == null)
                throw new NullReferenceException("Cannot prune a segment that doesn't exist");
            Debug.Assert(!PrunedSegments.Contains(segment), "Trying to prune a segment we already pruned");

            PrunedSegments.Add(segment);

            RemoveFromAdjacencyList(currentIndex, nextIndex);
        }

        private void RemoveFromAdjacencyList(int currentIndex, int nextIndex)
        {
            RemoveSegment(currentIndex, nextIndex);
            RemoveSegment(nextIndex, currentIndex);

            void RemoveSegment(int firstIndex, int secondIndex)
            {
                List<int> currentAdjList = vertexAdjacencyList[firstIndex];
                if (currentAdjList.Count == 1)
                    vertexAdjacencyList.Remove(firstIndex);
                else
                    currentAdjList.Remove(secondIndex);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Helion.Util.Extensions;
using Helion.Util.Geometry.Vectors;
using UnityEngine;

namespace Helion.Bsp.Geometry
{
    // This may be converted into a class in the future. Right now this is
    // intended to be a mapping of a segments points to a segment index. The
    // first key is supposed to be the smaller of the two indices to make any
    // lookups easier (and enforce a simple ordering to prevent the reversed
    // pair from being added).
    using SegmentTable = Dictionary<int, Dictionary<int, int>>;

    /// <summary>
    /// An allocator of segments. This is required such that any splitting that
    /// occurs also properly creates new vertices through the vertex allocator.
    /// </summary>
    /// <remarks>
    /// This is also intended to be a way of preventing any duplicates from
    /// being created.
    /// </remarks>
    public class SegmentAllocator
    {
        private readonly VertexAllocator vertexAllocator;
        private readonly CollinearTracker collinearTracker;
        private readonly List<BspSegment> segments = new List<BspSegment>();
        private readonly SegmentTable segmentTable = new SegmentTable();

        /// <summary>
        /// How many segments have been allocated.
        /// </summary>
        public int Count => segments.Count;

        /// <summary>
        /// Creates a segment allocator that uses the vertex allocator for
        /// creating new segment endpoints from.
        /// </summary>
        /// <param name="vertexAllocator">The vertex allocator.</param>
        /// <param name="collinearTracker">The collinear index tracker.</param>
        public SegmentAllocator(VertexAllocator vertexAllocator, CollinearTracker collinearTracker)
        {
            this.vertexAllocator = vertexAllocator;
            this.collinearTracker = collinearTracker;
        }

        /// <summary>
        /// Gets the segment for the index provided.
        /// </summary>
        /// <param name="segIndex">The index of the segment. It is an error if
        /// it is not in the range of [0, Count).</param>
        /// <returns>The segment for the index.</returns>
        /// <exception cref="IndexOutOfRangeException">If the index is out of
        /// range.</exception>
        public BspSegment this[int segIndex] => segments[segIndex];

        /// <summary>
        /// Gets the segment at the vertices provided if it exists, or creates
        /// it if not.
        /// </summary>
        /// <param name="start">The start vertex.</param>
        /// <param name="end">The end vertex.</param>
        /// <param name="line">The line (optional).</param>
        /// <returns>Either a newly allocated BSP segment, or the one that
        /// already exists with the information provided.</returns>
        public BspSegment GetOrCreate(BspVertex start, BspVertex end, IBspUsableLine line = null)
        {
            Debug.Assert(start != end, "Cannot create a segment that is a point");

            (int smallerIndex, int largerIndex) = start.Index.MinMax(end.Index);

            if (segmentTable.TryGetValue(smallerIndex, out var largerValues))
            {
                if (largerValues.TryGetValue(largerIndex, out int segIndex))
                    return segments[segIndex];

                largerValues[largerIndex] = segments.Count;
                int newCollinearIndex = GetCollinearIndex(start, end);
                return CreateNewSegment(start, end, newCollinearIndex, line);
            }

            var largerIndexDict = new Dictionary<int, int> { [largerIndex] = segments.Count };
            int collinearIndex = GetCollinearIndex(start, end);
            segmentTable[smallerIndex] = largerIndexDict;
            return CreateNewSegment(start, end, collinearIndex, line);
        }

        /// <summary>
        /// Checks if a segment exists with the start/end index combination.
        /// The order of the indices does not matter.
        /// </summary>
        /// <param name="start">The start index.</param>
        /// <param name="end">The end index.</param>
        /// <returns>True if a segment (start, end) or (end, start) exists in
        /// this allocator, false otherwise.</returns>
        public bool ContainsSegment(BspVertex start, BspVertex end)
        {
            (int smallerIndex, int largerIndex) = start.Index.MinMax(end.Index);

            if (segmentTable.TryGetValue(smallerIndex, out var largerValues))
                return largerValues.ContainsKey(largerIndex);
            return false;
        }

        /// <summary>
        /// Performs a split on the segment at some time t (which should be in
        /// the normal range).
        /// </summary>
        /// <param name="seg">The segment to split.</param>
        /// <param name="t">The time to split at, which should be in (0, 1). It
        /// is an error for it to be equal to 0.0 or 1.0 as that would create
        /// a point for a segment (which is no longer a segment).</param>
        /// <returns>The two segments, where the first element is the segment
        /// from [start, middle], and the second segment element is from the
        /// [middle, end].</returns>
        public (BspSegment first, BspSegment second) Split(BspSegment seg, double t)
        {
            Debug.Assert(t > 0.0 && t < 1.0, "Trying to split BSP out of the (0.0, 1.0) range");

            Vec2D middle = seg.FromTime(t);
            BspVertex middleVertex = vertexAllocator[middle];

            BspSegment firstSeg = GetOrCreate(seg.StartVertex, middleVertex, seg.Line.Value);
            BspSegment secondSeg = GetOrCreate(middleVertex, seg.EndVertex, seg.Line.Value);
            return (firstSeg, secondSeg);
        }

        /// <summary>
        /// Allocates a new list with all the segment references.
        /// </summary>
        /// <returns>A list of all the existing segments.</returns>
        public List<BspSegment> ToList() => new List<BspSegment>(segments);

        private BspSegment CreateNewSegment(BspVertex start, BspVertex end, int collinearIndex,
            IBspUsableLine line = null)
        {
            BspSegment seg = new BspSegment(start, end, collinearIndex, line);
            segments.Add(seg);
            return seg;
        }

        private int GetCollinearIndex(BspVertex start, BspVertex end)
        {
            return collinearTracker.GetOrCreateIndex(start, end);
        }
    }
}

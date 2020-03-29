using Helion.Core.Util;
using Helion.Core.Util.Geometry;

namespace Helion.Core.Resource.Maps.Shared
{
    /// <summary>
    /// A GL segment that makes up a subsector edge.
    /// </summary>
    public class GLSegment
    {
        /// <summary>
        /// The span of the segment.
        /// </summary>
        public Line2 Segment;

        /// <summary>
        /// If this is on the front side of the line.
        /// </summary>
        public bool Front;

        /// <summary>
        /// The partner segment, or opposite line on the back side of this
        /// segment (if any).
        /// </summary>
        public Optional<GLSegment> Partner = new Optional<GLSegment>();

        /// <summary>
        /// Creates a new GL segment.
        /// </summary>
        /// <param name="segment">The segment span.</param>
        /// <param name="front">True if it's on the front of the line, false if
        /// on the back.</param>
        public GLSegment(Line2 segment, bool front)
        {
            Segment = segment;
            Front = front;
        }
    }
}

using Helion.Core.Resource.Maps.Doom;
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
        /// The line ths segment is on. This is not present if it's a miniseg.
        /// </summary>
        public Optional<DoomLinedef> Linedef;

        /// <summary>
        /// The side this segment is on. This is not present if it's a miniseg.
        /// </summary>
        public Optional<DoomSidedef> Sidedef;

        /// <summary>
        /// The partner segment, or opposite line on the back side of this
        /// segment (if any).
        /// </summary>
        public Optional<GLSegment> Partner = new Optional<GLSegment>();

        /// <summary>
        /// Creates a new GL segment.
        /// </summary>
        /// <param name="segment">The segment span.</param>
        /// <param name="linedef">The linedef this references.</param>
        /// <param name="sidedef">The sidedef this references.</param>
        /// <param name="front">True if it's on the front of the line, false if
        /// on the back.</param>
        public GLSegment(Line2 segment, bool front, Optional<DoomLinedef> linedef, Optional<DoomSidedef> sidedef)
        {
            Segment = segment;
            Front = front;
            Linedef = linedef;
            Sidedef = sidedef;
        }
    }
}

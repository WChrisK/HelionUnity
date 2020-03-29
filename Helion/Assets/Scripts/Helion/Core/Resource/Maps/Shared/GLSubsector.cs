using System.Collections.Generic;

namespace Helion.Core.Resource.Maps.Shared
{
    /// <summary>
    /// A convex leaf in a BSP tree.
    /// </summary>
    public class GLSubsector
    {
        /// <summary>
        /// A collection of the clockwise segments.
        /// </summary>
        public readonly IList<GLSegment> Segments;

        /// <summary>
        /// Creates a subsector from a list of clockwise segments.
        /// </summary>
        /// <param name="segments">The subsector segments.</param>
        public GLSubsector(IList<GLSegment> segments)
        {
            Segments = segments;
        }
    }
}

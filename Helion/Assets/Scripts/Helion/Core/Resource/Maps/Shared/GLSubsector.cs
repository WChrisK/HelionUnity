using System.Collections.Generic;
using System.Linq;
using Helion.Core.Resource.Maps.Doom;

namespace Helion.Core.Resource.Maps.Shared
{
    /// <summary>
    /// A convex leaf in a BSP tree.
    /// </summary>
    public class GLSubsector
    {
        /// <summary>
        /// The index of this subsector in the entry.
        /// </summary>
        public readonly int Index;

        /// <summary>
        /// A collection of the clockwise segments.
        /// </summary>
        public readonly IList<GLSegment> Segments;

        /// <summary>
        /// Gets the sector for this subsector.
        /// </summary>
        /// <remarks>
        /// This must exist in a well formed map
        /// </remarks>
        public DoomSector Sector => Segments.First(s => s.Sidedef.HasValue).Sidedef.Value.Sector;

        /// <summary>
        /// Creates a subsector from a list of clockwise segments.
        /// </summary>
        /// <param name="index">The subsector index in the entry.</param>
        /// <param name="segments">The subsector segments.</param>
        public GLSubsector(int index, IList<GLSegment> segments)
        {
            Index = index;
            Segments = segments;
        }
    }
}

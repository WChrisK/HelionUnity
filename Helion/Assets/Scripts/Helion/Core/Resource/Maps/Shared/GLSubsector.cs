using System.Collections.Generic;
using UnityEngine;

namespace Helion.Core.Resource.Maps.Shared
{
    /// <summary>
    /// A convex leaf in a BSP tree.
    /// </summary>
    public class GLSubsector
    {
        /// <summary>
        /// A collection of the clockwise convex vertices.
        /// </summary>
        public readonly IList<Vector2> Vertices;

        /// <summary>
        /// Creates a subsector from a list of clockwise convex vertices.
        /// </summary>
        /// <param name="vertices">The subsector vertices.</param>
        public GLSubsector(IList<Vector2> vertices)
        {
            Vertices = vertices;
        }
    }
}

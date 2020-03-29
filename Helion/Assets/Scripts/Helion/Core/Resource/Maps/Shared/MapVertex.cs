using UnityEngine;

namespace Helion.Core.Resource.Maps.Shared
{
    /// <summary>
    /// A vertex in a map.
    /// </summary>
    public class MapVertex
    {
        /// <summary>
        /// The X coordinate.
        /// </summary>
        public readonly float X;

        /// <summary>
        /// The Y coordinate.
        /// </summary>
        public readonly float Y;

        /// <summary>
        /// Gets the vector representation of this vertex.
        /// </summary>
        public Vector2 Vector => new Vector2(X, Y);

        /// <summary>
        /// Creates a new vertex.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public MapVertex(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{X}, {Y}";
    }
}

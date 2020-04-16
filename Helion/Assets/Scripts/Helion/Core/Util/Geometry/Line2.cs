using System.Diagnostics.Contracts;
using UnityEngine;

namespace Helion.Core.Util.Geometry
{
    /// <summary>
    /// A two dimensional line.
    /// </summary>
    public struct Line2
    {
        /// <summary>
        /// The starting point.
        /// </summary>
        public Vector2 Start;

        /// <summary>
        /// The ending point.
        /// </summary>
        public Vector2 End;

        /// <summary>
        /// The delta between the start and end.
        /// </summary>
        public Vector2 Delta;

        /// <summary>
        /// The length of the line.
        /// </summary>
        public float Length => (End - Start).magnitude;

        /// <summary>
        /// Creates a new line that is the reversed version of this line.
        /// </summary>
        public Line2 Reverse => new Line2(End, Start);

        /// <summary>
        /// Creates a line.
        /// </summary>
        /// <param name="start">The starting point.</param>
        /// <param name="end">The ending point.</param>
        public Line2(Vector2 start, Vector2 end)
        {
            Start = start;
            End = end;
            Delta = end - start;
        }

        /// <summary>
        /// Creates a line.
        /// </summary>
        /// <param name="startX">The starting coordinate.</param>
        /// <param name="startY">The starting coordinate.</param>
        /// <param name="endX">The ending coordinate.</param>
        /// <param name="endY">The ending coordinate.</param>
        public Line2(float startX, float startY, float endX, float endY) :
            this(new Vector2(startX, startY), new Vector2(endX, endY))
        {
        }

        /// <summary>
        /// Calculates the perpendicular dot product. This also may be known as
        /// the wedge product.
        /// </summary>
        /// <param name="point">The point to test against.</param>
        /// <returns>The perpendicular dot product.</returns>
        [Pure]
        public float PerpDot(Vector2 point)
        {
            return (Delta.x * (point.y - Start.y)) - (Delta.y * (point.x - Start.x));
        }

        /// <summary>
        /// Checks if a point is on the right side (or on the line).
        /// </summary>
        /// <param name="point">The point to check.</param>
        /// <returns>True if it's on the right or on the line, false if it's on
        /// the left.</returns>
        [Pure]
        public bool OnRight(Vector2 point) => PerpDot(point) <= 0;

        public override string ToString() => $"({Start}), ({End})";
    }
}

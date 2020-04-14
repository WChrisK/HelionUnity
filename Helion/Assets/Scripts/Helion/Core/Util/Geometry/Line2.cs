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

        public override string ToString() => $"({Start}), ({End})";
    }
}

namespace Helion.Core.Resource.Maps.Doom
{
    /// <summary>
    /// A vertex in a Doom (or Hexen) map.
    /// </summary>
    public class DoomVertex
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
        /// Creates a new vertex.
        /// </summary>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        public DoomVertex(float x, float y)
        {
            X = x;
            Y = y;
        }

        public override string ToString() => $"{X}, {Y}";
    }
}

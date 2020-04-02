namespace Helion.Core.Util.Geometry
{
    /// <summary>
    /// A simple dimension wrapper around a width and height.
    /// </summary>
    public readonly struct Dimension
    {
        /// <summary>
        /// The width of the dimension.
        /// </summary>
        public readonly int Width;

        /// <summary>
        /// The height of the dimension.
        /// </summary>
        public readonly int Height;

        /// <summary>
        /// Creates a new dimension object with the dimensions provided.
        /// </summary>
        /// <param name="width">The width which should be non-negative.</param>
        /// <param name="height">The height which should be non-negative.
        /// </param>
        public Dimension(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Gets the area of the box formed by this dimension.
        /// </summary>
        public int Area => Width * Height;

        /// <summary>
        /// Calculates the aspect ratio of width by height.
        /// </summary>
        public float AspectRatio => (float)Width / Height;

        /// <summary>
        /// Checks for bitwise equality between components.
        /// </summary>
        /// <param name="self">The left side dimension.</param>
        /// <param name="other">The right side dimension.</param>
        /// <returns>True if they are equal, false if not.</returns>
        public static bool operator ==(in Dimension self, in Dimension other)
        {
            return self.Width == other.Width && self.Height == other.Height;
        }

        /// <summary>
        /// Checks for bitwise inequality between components.
        /// </summary>
        /// <param name="self">The left side dimension.</param>
        /// <param name="other">The right side dimension.</param>
        /// <returns>True if they are not equal, false if they are.</returns>
        public static bool operator !=(in Dimension self, in Dimension other) => !(self == other);

        /// <summary>
        /// Checks if two dimensions are equal.
        /// </summary>
        /// <param name="other">The other dimension to check.</param>
        /// <returns>True if they are equal, false if not.</returns>
        public bool Equals(Dimension other) => Width == other.Width && Height == other.Height;

        public override bool Equals(object obj) => obj is Dimension other && Equals(other);

        public override int GetHashCode() => Width ^ Height;

        public override string ToString() => $"{Width}, {Height}";
    }
}

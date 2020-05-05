using Helion.Util.Geometry.Vectors;

namespace Helion.Util.Interpolation
{
    /// <summary>
    /// Handles an interpolatable vector value.
    /// </summary>
    public struct Vec3Interpolation
    {
        /// <summary>
        /// The current position.
        /// </summary>
        public readonly Vec3F Current;

        /// <summary>
        /// The previous position.
        /// </summary>
        public readonly Vec3F Previous;

        /// <summary>
        /// Creates a new interpolatable from the value.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public Vec3Interpolation(Vec3F value)
        {
            Current = value;
            Previous = value;
        }

        /// <summary>
        /// Creates a new value from the current and previous position.
        /// </summary>
        /// <param name="current">The current position.</param>
        /// <param name="previous">The previous position.</param>
        public Vec3Interpolation(Vec3F current, Vec3F previous)
        {
            Current = current;
            Previous = previous;
        }

        /// <summary>
        /// Creates a new interpolation value at the position provided. This
        /// will carry over the previous position to create a new value.
        /// </summary>
        /// <param name="newPosition">The new position to set current to in the
        /// newly created value.</param>
        /// <returns>The new interpolation value.</returns>
        public Vec3Interpolation At(Vec3F newPosition)
        {
            return new Vec3Interpolation(newPosition, Previous);
        }

        /// <summary>
        /// Gets the interpolated value.
        /// </summary>
        /// <param name="fraction">The fraction between 0.0 and 1.0.</param>
        /// <returns>The interpolated value based on the fraction.</returns>
        public Vec3F Value(float fraction) => Previous + ((Current - Previous) * fraction);

        /// <summary>
        /// Acts as if the position was ticked, which moves the current to the
        /// previous.
        /// </summary>
        /// <returns>A new interpolation after ticking the struct.</returns>
        public Vec3Interpolation Tick() => new Vec3Interpolation(Current);

        public override string ToString() => $"{Current} (prev: {Previous})";
    }
}

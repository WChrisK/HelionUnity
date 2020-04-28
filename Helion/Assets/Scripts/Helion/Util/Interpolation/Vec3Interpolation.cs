using Helion.Util.Geometry.Vectors;
using Helion.Worlds;

namespace Helion.Util.Interpolation
{
    /// <summary>
    /// Handles an interpolatable vector value.
    /// </summary>
    public struct Vec3Interpolation : ITickable
    {
        /// <summary>
        /// The current position.
        /// </summary>
        public Vec3F Current;

        /// <summary>
        /// The previous position.
        /// </summary>
        public Vec3F Previous;

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
        /// Adds a value the current interpolation's Current value.
        /// </summary>
        /// <param name="self">The interpolation to add to.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>A new interpolation value with the updated Current field.
        /// </returns>
        public static Vec3Interpolation operator +(Vec3Interpolation self, float value)
        {
            self.Current += value;
            return self;
        }

        /// <summary>
        /// Adds a vector the current interpolation's Current value.
        /// </summary>
        /// <param name="self">The interpolation to add to.</param>
        /// <param name="vec">The vector to add.</param>
        /// <returns>A new interpolation value with the updated Current field.
        /// </returns>
        public static Vec3Interpolation operator +(Vec3Interpolation self, Vec3F vec)
        {
            self.Current += vec;
            return self;
        }

        /// <summary>
        /// Subtracts a value the current interpolation's Current value.
        /// </summary>
        /// <param name="self">The interpolation to add to.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>A new interpolation value with the updated Current field.
        /// </returns>
        public static Vec3Interpolation operator -(Vec3Interpolation self, float value)
        {
            self.Current -= value;
            return self;
        }

        /// <summary>
        /// Subtracts a vector the current interpolation's Current value.
        /// </summary>
        /// <param name="self">The interpolation to add to.</param>
        /// <param name="vec">The vector to add.</param>
        /// <returns>A new interpolation value with the updated Current field.
        /// </returns>
        public static Vec3Interpolation operator -(Vec3Interpolation self, Vec3F vec)
        {
            self.Current -= vec;
            return self;
        }

        /// <summary>
        /// Multiplies a value the current interpolation's Current value.
        /// </summary>
        /// <param name="self">The interpolation to add to.</param>
        /// <param name="value">The value to add.</param>
        /// <returns>A new interpolation value with the updated Current field.
        /// </returns>
        public static Vec3Interpolation operator *(Vec3Interpolation self, float value)
        {
            self.Current *= value;
            return self;
        }

        /// <summary>
        /// Multiplies a vector the current interpolation's Current value.
        /// </summary>
        /// <param name="self">The interpolation to add to.</param>
        /// <param name="vec">The vector to add.</param>
        /// <returns>A new interpolation value with the updated Current field.
        /// </returns>
        public static Vec3Interpolation operator *(Vec3Interpolation self, Vec3F vec)
        {
            self.Current *= vec;
            return self;
        }

        /// <summary>
        /// Gets the interpolated value.
        /// </summary>
        /// <param name="fraction">The fraction between 0.0 and 1.0.</param>
        /// <returns>The interpolated value based on the fraction.</returns>
        public Vec3F Value(float fraction) => Previous + ((Current - Previous) * fraction);

        /// <summary>
        /// Updates the current value to a new one. Does not change the
        /// previous position's value (use <see cref="Reset"/> for that).
        /// </summary>
        /// <param name="value">The new value.</param>
        public void Set(Vec3F value)
        {
            Current = value;
        }

        /// <summary>
        /// Resets the interpolation to the value provided. This will set both
        /// the current and previous position to the provided value.
        /// </summary>
        /// <param name="value">The value to set the current and previous to.
        /// </param>
        public void Reset(Vec3F value)
        {
            Current = value;
            Previous = value;
        }

        /// <summary>
        /// Advances the interpolation so that the previous state is equal to
        /// the current state.
        /// </summary>
        public void Tick()
        {
            Previous = Current;
        }
    }
}

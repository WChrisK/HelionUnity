namespace Helion.Core.Util.Interpolation
{
    /// <summary>
    /// Handles an interpolatable float value.
    /// </summary>
    public struct FloatInterpolation
    {
        /// <summary>
        /// The current position.
        /// </summary>
        public float Current;

        /// <summary>
        /// The previous position.
        /// </summary>
        public float Previous;

        /// <summary>
        /// Creates a new interpolatable from the value.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public FloatInterpolation(float value)
        {
            Current = value;
            Previous = value;
        }

        /// <summary>
        /// Gets the interpolated value.
        /// </summary>
        /// <param name="fraction">The fraction between 0.0 and 1.0.</param>
        /// <returns>The interpolated value based on the fraction.</returns>
        public float Value(float fraction) => Previous + ((Current - Previous) * fraction);

        /// <summary>
        /// Updates the current value to a new one.
        /// </summary>
        /// <param name="value">The new value.</param>
        public void Update(float value)
        {
            Current = value;
        }

        /// <summary>
        /// Resets the interpolation to the value provided.
        /// </summary>
        /// <param name="value">The value to set the current and previous to.
        /// </param>
        public void Reset(float value)
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

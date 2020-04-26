using Helion.Worlds;
using UnityEngine;

namespace Helion.Util.Interpolation
{
    /// <summary>
    /// Handles an interpolatable vector value.
    /// </summary>
    public struct Vector3Interpolation : ITickable
    {
        /// <summary>
        /// The current position.
        /// </summary>
        public Vector3 Current;

        /// <summary>
        /// The previous position.
        /// </summary>
        public Vector3 Previous;

        /// <summary>
        /// Creates a new interpolatable from the value.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public Vector3Interpolation(Vector3 value)
        {
            Current = value;
            Previous = value;
        }

        /// <summary>
        /// Gets the interpolated value.
        /// </summary>
        /// <param name="fraction">The fraction between 0.0 and 1.0.</param>
        /// <returns>The interpolated value based on the fraction.</returns>
        public Vector3 Value(float fraction) => Previous + ((Current - Previous) * fraction);

        /// <summary>
        /// Updates the current value to a new one.
        /// </summary>
        /// <param name="value">The new value.</param>
        public void Update(Vector3 value)
        {
            Current = value;
        }

        /// <summary>
        /// Resets the interpolation to the value provided.
        /// </summary>
        /// <param name="value">The value to set the current and previous to.
        /// </param>
        public void Reset(Vector3 value)
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

using System;

namespace Helion.Core.Worlds.Geometry.Enums
{
    /// <summary>
    /// An enumeration for how lines can be unpegged to do alignment.
    /// </summary>
    [Flags]
    public enum Unpegged
    {
        None = 0b00,
        Upper = 0b01,
        Lower = 0b10,
        UpperAndLower = Upper | Lower
    }

    /// <summary>
    /// Helper functions for the <see cref="Unpegged"/> enum.
    /// </summary>
    public static class UnpeggedExtensions
    {
        /// <summary>
        /// Checks if the lower flag is set on this.
        /// </summary>
        /// <param name="unpegged">The unpegged value.</param>
        /// <returns>True if it has the lower flag set.</returns>
        public static bool HasLower(this Unpegged unpegged)
        {
            return unpegged == Unpegged.Lower || unpegged == Unpegged.UpperAndLower;
        }

        /// <summary>
        /// Checks if the upper flag is set on this.
        /// </summary>
        /// <param name="unpegged">The unpegged value.</param>
        /// <returns>True if it has the upper flag set.</returns>
        public static bool HasUpper(this Unpegged unpegged)
        {
            return unpegged == Unpegged.Upper || unpegged == Unpegged.UpperAndLower;
        }
    }
}

namespace Helion.Core.Util
{
    /// <summary>
    /// Constant values shared globally.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// Each meter in a Unity world is equal to this many map units.
        /// </summary>
        public const int MeterToMapUnits = 32;

        /// <summary>
        /// A conversion factor of meters to map units.
        /// </summary>
        public const float MapUnit = 1.0f / MeterToMapUnits;

        /// <summary>
        /// How many milliseconds each tick takes.
        /// </summary>
        public const float TickRateMillis = 28;

        /// <summary>
        /// A conversion factor of light level units to a normalized range.
        /// </summary>
        public const float InverseLightLevel = 1.0f / 255;

        /// <summary>
        /// The characters for a missing texture in a map.
        /// </summary>
        /// <remarks>
        /// Intended to indicate a texture should not be drawn. For example, if
        /// a middle two-sided texture is set to this, then we should not make
        /// a wall for it.
        /// </remarks>
        public static readonly UpperString NoTexture = "-";

        /// <summary>
        /// The name of the application.
        /// </summary>
        public static readonly string ApplicationName = "Helion";

        /// <summary>
        /// The application version.
        /// </summary>
        public static readonly string ApplicationVersion = "0.1";
    }
}

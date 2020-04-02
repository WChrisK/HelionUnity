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
    }
}

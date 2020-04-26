using Helion.Util;
using Helion.Util.Extensions;

namespace Helion.Worlds
{
    /// <summary>
    /// A bunch of helper classes for a world.
    /// </summary>
    public static class WorldUtil
    {
        /// <summary>
        /// Converts an integral light level to a doom-styled fractional light
        /// level.
        /// </summary>
        /// <param name="lightLevelInt">The integral light level.</param>
        /// <returns>The light level that is more doom-like.</returns>
        public static float ToDoomLightLevel(int lightLevelInt)
        {
            float lightLevel = lightLevelInt.Clamp(0, 255) * Constants.InverseLightLevel;

            if (lightLevel <= 0.75f)
            {
                if (lightLevel > 0.4f)
                {
                    lightLevel = -0.6375f + (1.85f * lightLevel);
                    if (lightLevel < 0.08f)
                        lightLevel = 0.08f + (lightLevel * 0.2f);
                }
                else
                {
                    lightLevel /= 5.0f;
                }
            }

            return lightLevel.Clamp(0, 1);
        }
    }
}

using UnityEngine;

namespace Helion.Core.Graphics
{
    /// <summary>
    /// Helper methods for colors.
    /// </summary>
    public static class ColorHelper
    {
        private const float InvScale = 1.0f / 255.0f;

        /// <summary>
        /// Turns RGB colors into a color with max alpha.
        /// </summary>
        /// <param name="r">The red color.</param>
        /// <param name="g">The green color.</param>
        /// <param name="b">The blue color.</param>
        /// <returns>The color.</returns>
        public static Color FromRGB(byte r, byte g, byte b)
        {
            return new Color(r * InvScale, g * InvScale, b * InvScale);
        }

        /// <summary>
        /// Turns RGB colors into a color with max alpha.
        /// </summary>
        /// <param name="r">The red color.</param>
        /// <param name="g">The green color.</param>
        /// <param name="b">The blue color.</param>
        /// <returns>The color.</returns>
        public static Color FromRGB(int r, int g, int b) => FromRGB((byte)r, (byte)g, (byte)b);

        /// <summary>
        /// Takes a string and converts it to a known color.
        /// </summary>
        /// <param name="text">The color text. This is case insensitive.
        /// </param>
        /// <returns>The color, or null if there is no matching color for the
        /// name provided.</returns>
        public static Color? StringToColor(string text)
        {
            switch (text.ToUpper())
            {
            case "BLACK":
                return Color.black;
            case "BLUE":
                return Color.blue;
            case "BRICK":
                return FromRGB(200, 50, 50);
            case "BROWN":
                return FromRGB(150, 100, 50);
            case "CREAM":
                return FromRGB(255, 200, 150);
            case "CYAN":
                return Color.cyan;
            case "DARKBROWN":
                return FromRGB(64, 16, 16);
            case "DARKGRAY":
            case "DARKGREY":
                return FromRGB(128, 128, 128);
            case "DARKGREEN":
                return FromRGB(0, 140, 0);
            case "DARKRED":
                return FromRGB(128, 0, 0);
            case "GOLD":
                return FromRGB(255, 200, 50);
            case "GRAY":
            case "GREY":
                return Color.gray;
            case "GREEN":
                return Color.green;
            case "LIGHTBLUE":
                return FromRGB(50, 255, 255);
            case "OLIVE":
                return FromRGB(200, 200, 170);
            case "ORANGE":
                return FromRGB(255, 170, 0);
            case "PURPLE":
                return FromRGB(150, 100, 200);
            case "RED":
                return Color.red;
            case "TAN":
                return FromRGB(210, 180, 140);
            case "WHITE":
                return Color.white;
            case "YELLOW":
                return Color.yellow;
            default:
                return null;
            }
        }
    }
}

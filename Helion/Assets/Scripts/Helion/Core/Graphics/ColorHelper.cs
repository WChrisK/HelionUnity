using UnityEngine;

namespace Helion.Core.Graphics
{
    /// <summary>
    /// Helper methods for colors.
    /// </summary>
    public static class ColorHelper
    {
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
                return Color.Firebrick;
            case "BROWN":
                return Color.brown;
            case "CREAM":
                return Color.PeachPuff;
            case "CYAN":
                return Color.cyan;
            case "DARKBROWN":
                return new Color(0.25f, 0.0625f, 0.0625f);
            case "DARKGRAY":
            case "DARKGREY":
                return Color.DarkGray;
            case "DARKGREEN":
                return Color.DarkGreen;
            case "DARKRED":
                return Color.DarkRed;
            case "GOLD":
                return Color.Gold;
            case "GRAY":
            case "GREY":
                return Color.gray;
            case "GREEN":
                return Color.green;
            case "LIGHTBLUE":
                return Color.LightBlue;
            case "OLIVE":
                return Color.Olive;
            case "ORANGE":
                return Color.Orange;
            case "PURPLE":
                return Color.Purple;
            case "RED":
                return Color.red;
            case "TAN":
                return Color.Tan;
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

using System;
using Helion.Core.Util;
using UnityEngine;
using static Helion.Core.Util.OptionalHelper;

namespace Helion.Core.Resource.Colors.Palettes
{
    /// <summary>
    /// A layer in a palette, which is a collection of 256 colors.
    /// </summary>
    public class PaletteLayer
    {
        public const int ColorsPerLayer = 256;
        public const int BytesPerColor = 3;
        public const int BytesPerLayer = ColorsPerLayer * BytesPerColor;

        private readonly Color[] colors;

        private PaletteLayer(Color[] colors)
        {
            this.colors = colors;
        }

        /// <summary>
        /// Reads a palette layer from a chunk of data.
        /// </summary>
        /// <param name="data">The data to use.</param>
        /// <param name="dataOffset">The offset to read the layer from.</param>
        /// <returns>The layer at the offset provided, or an empty value if the
        /// data cannot be read due to insufficient data or invalid offsets.
        /// </returns>
        public static Optional<PaletteLayer> From(byte[] data, int dataOffset)
        {
            if (dataOffset < 0 || dataOffset + BytesPerLayer > data.Length)
                return Empty;

            Color[] colors = new Color[ColorsPerLayer];
            for (int i = 0; i < ColorsPerLayer; i++)
            {
                int offset = dataOffset + (i * BytesPerColor);
                float r = data[offset] / 255.0f;
                float g = data[offset + 1] / 255.0f;
                float b = data[offset + 2] / 255.0f;
                colors[i] = new Color(r, g, b);
            }

            return new PaletteLayer(colors);
        }

        /// <summary>
        /// Gets the color at the index provided.
        /// </summary>
        /// <param name="index">The color index.
        /// </param>
        /// <exception cref="IndexOutOfRangeException">If the index is not in
        /// the palette value range.</exception>
        public Color this[int index] => colors[index];
    }
}

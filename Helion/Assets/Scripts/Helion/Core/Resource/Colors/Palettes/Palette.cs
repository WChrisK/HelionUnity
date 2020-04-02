using System.Collections.Generic;
using System.Linq;
using Helion.Core.Util;

namespace Helion.Core.Resource.Colors.Palettes
{
    /// <summary>
    /// A palette of colors from a PLAYPAL entry.
    /// </summary>
    public class Palette
    {
        /// <summary>
        /// All the layers
        /// </summary>
        public readonly PaletteLayer[] Layers;

        /// <summary>
        /// The standard layer for coloring.
        /// </summary>
        public PaletteLayer Default => Layers[0];

        private Palette(IEnumerable<PaletteLayer> layers)
        {
            Layers = layers.ToArray();
        }

        /// <summary>
        /// Reads the data into a palette object.
        /// </summary>
        /// <param name="data">The data to read.</param>
        /// <returns>A palette object if the data is well formed, or an empty
        /// optional if the data is too little or is corrupt in size.</returns>
        public static Optional<Palette> From(byte[] data)
        {
            if (data.Length < PaletteLayer.BytesPerLayer)
                return Optional<Palette>.Empty();

            List<PaletteLayer> layers = new List<PaletteLayer>();

            for (int offset = 0; offset < data.Length; offset += PaletteLayer.BytesPerLayer)
            {
                Optional<PaletteLayer> processedLayer = PaletteLayer.From(data, offset);
                if (!processedLayer)
                    return Optional<Palette>.Empty();

                layers.Add(processedLayer.Value);
            }

            return new Palette(layers);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helion.Archives;
using Helion.Util;
using Helion.Util.Bytes;
using MoreLinq;
using UnityEngine;
using static Helion.Util.OptionalHelper;

namespace Helion.Resource.Textures.Definitions.Vanilla
{
    /// <summary>
    /// A processed version of a TEXTURE1/2/3 entry.
    /// </summary>
    public class TextureX : IVanillaTextureDefinition, IEnumerable<TextureXImage>
    {
        /// <summary>
        /// The number of the X in the name, as in 1 for TEXTURE1 and 2 for
        /// TEXTURE2.
        /// </summary>
        public readonly int TextureXNumber;

        /// <summary>
        /// The list of texture components.
        /// </summary>
        private readonly Dictionary<UpperString, TextureXImage> images = new Dictionary<UpperString, TextureXImage>();

        /// <summary>
        /// Gets the number of textures.
        /// </summary>
        public int Count => images.Count;

        private TextureX(int number, IEnumerable<TextureXImage> textureXImages)
        {
            TextureXNumber = number;

            // Apparently only the first one is taken. Compatibility sucks...
            textureXImages.Reverse().ForEach(img => { images[img.Name] = img; });
        }

        /// <summary>
        /// Reads a TEXTURE1/2 entry.
        /// </summary>
        /// <param name="entry">The entry to read.</param>
        /// <returns>All the processed texture definitions, or null if the
        /// entry is corrupt.</returns>
        public static Optional<TextureX> From(IEntry entry)
        {
            switch (entry.Name.String.LastOrDefault())
            {
            case '1':
                return From(1, entry.Data);
            case '2':
                return From(2, entry.Data);
            default:
                return Empty;
            }
        }

        /// <summary>
        /// Reads a TEXTURE1/2 entry.
        /// </summary>
        /// <param name="number">The index of the X in the textureX. Should be
        /// either 1 or 2.</param>
        /// <param name="data">The data to read.</param>
        /// <returns>All the processed texture definitions, or null if the
        /// entry is corrupt.</returns>
        public static Optional<TextureX> From(int number, byte[] data)
        {
            Debug.Assert(number == 1 || number == 2, "TEXTUREx should have only 1 or 2 as a number");

            try
            {
                ByteReader reader = ByteReader.From(ByteOrder.Little, data);
                Dictionary<UpperString, TextureXImage> images = new Dictionary<UpperString, TextureXImage>();

                int numTextures = reader.Int();

                List<int> offsets = new List<int>();
                for (int i = 0; i < numTextures; i++)
                    offsets.Add(reader.Int());

                foreach (int offset in offsets)
                {
                    reader.Offset = offset;
                    Optional<TextureXImage> imageOptional = TextureXImage.From(reader);
                    if (!imageOptional)
                        return Empty;

                    // Unfortunately we have to follow the specification as per
                    // the ZDoom wiki:  "...however if a texture is defined
                    // more than once in the same TEXTUREx lump, the later
                    // definitions are skipped".
                    TextureXImage image = imageOptional.Value;
                    if (!images.ContainsKey(image.Name))
                        images[image.Name] = image;
                }

                return new TextureX(number, images.Values);
            }
            catch
            {
                return Empty;
            }
        }

        /// <summary>
        /// Looks up the texture image. The latest texture name in the TextureX
        /// entry is returned if duplicates exist.
        /// </summary>
        /// <param name="name">The name of the texture.</param>
        /// <returns>The most recent texture image if the texture name exists
        /// for some definition, null if it cannot find it.</returns>
        public Optional<TextureXImage> Find(UpperString name)
        {
            if (images.TryGetValue(name, out TextureXImage image))
                return image;
            return Empty;
        }

        public IEnumerator<TextureXImage> GetEnumerator() => images.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

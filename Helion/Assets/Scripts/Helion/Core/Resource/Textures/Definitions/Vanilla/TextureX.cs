using System.Collections;
using System.Collections.Generic;
using Helion.Core.Util;
using Helion.Core.Util.Bytes;
using MoreLinq;

namespace Helion.Core.Resource.Textures.Definitions.Vanilla
{
    /// <summary>
    /// A processed version of a TEXTURE1/2/3 entry.
    /// </summary>
    public class TextureX : IEnumerable<TextureXImage>
    {
        /// <summary>
        /// The list of texture components.
        /// </summary>
        private readonly Dictionary<UpperString, TextureXImage> images = new Dictionary<UpperString, TextureXImage>();

        /// <summary>
        /// Gets the number of textures.
        /// </summary>
        public int Count => images.Count;

        private TextureX(IEnumerable<TextureXImage> textureXImages)
        {
            textureXImages.ForEach(img => images[img.Name] = img);
        }

        /// <summary>
        /// Reads a TEXTURE1/2/3 entry.
        /// </summary>
        /// <param name="data">The data to read.</param>
        /// <returns>All the processed texture definitions, or null if the
        /// entry is corrupt.</returns>
        public static Optional<TextureX> From(byte[] data)
        {
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
                        return Optional<TextureX>.Empty();

                    // Unfortunately we have to follow the specification as per
                    // the ZDoom wiki:  "...however if a texture is defined
                    // more than once in the same TEXTUREx lump, the later
                    // definitions are skipped".
                    TextureXImage image = imageOptional.Value;
                    if (!images.ContainsKey(image.Name))
                        images[image.Name] = image;
                }

                return new TextureX(images.Values);
            }
            catch
            {
                return Optional<TextureX>.Empty();
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
            return images.TryGetValue(name, out TextureXImage image) ?
                image :
                Optional<TextureXImage>.Empty();
        }

        public IEnumerator<TextureXImage> GetEnumerator() => images.Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

using Helion.Core.Resource;
using Helion.Core.Resource.Colors.Palettes;
using Helion.Core.Util;
using Helion.Core.Util.Bytes;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Geometry;
using UnityEngine;

namespace Helion.Core.Graphics
{
    /// <summary>
    /// An image made up of palette indices.
    /// </summary>
    public class PaletteImage : IImage
    {
        /// <summary>
        /// The palette index for a transparent pixel.
        /// </summary>
        /// <remarks>
        /// This number was chosen so if we want to do any lookups of colors,
        /// then all that has to be done is add an extra single value to the
        /// palette in memory, such that the length goes from being 256 bytes
        /// to 257 bytes.
        /// </remarks>
        public const short TransparentIndex = 256;

        public Dimension Dimension { get; }
        public int Width => Dimension.Width;
        public int Height => Dimension.Height;
        public int Area => Dimension.Area;
        public ImageMetadata Metadata { get; }
        public readonly short[] Indices;

        private PaletteImage(int width, int height, short[] indices, ImageMetadata metadata)
        {
            Dimension = new Dimension(width, height);
            Indices = indices;
            Metadata = new ImageMetadata(metadata);
        }

        /// <summary>
        /// Creates an empty palette image from a given width, height, and
        /// possibly metadata. The image contents are undefined.
        /// </summary>
        /// <param name="width">The width of the image. Should not be negative.
        /// </param>
        /// <param name="height">The height of the image. Should not be
        /// negative.</param>
        /// <param name="metadata">The metadata to use, or null if we want a
        /// default one.</param>
        /// <returns>The palette image, or null if the parameters are invalid
        /// and one cannot be made (ex: negative dimension).</returns>
        public static Optional<PaletteImage> From(int width, int height, ImageMetadata metadata = null)
        {
            if (width <= 0 || height <= 0)
                return Optional<PaletteImage>.Empty();

            short[] data = Arrays.Create(width * height, TransparentIndex);
            return new PaletteImage(width, height, data, metadata);
        }

        /// <summary>
        /// Creates a palette image from a column-based image.
        /// </summary>
        /// <param name="data">The data for the image.</param>
        /// <param name="resourceNamespace">The namespace this image belongs
        /// to.</param>
        /// <returns>The palette image for the data, or null if the image is
        /// corrupt and would not make a valid column image.</returns>
        public static Optional<PaletteImage> FromColumn(byte[] data, ResourceNamespace resourceNamespace)
        {
            // OPTIMIZE: Short circuit if it's likely not a column image.
            // OPTIMIZE: Write posts horizontally, then rotate for speed?
            try
            {
                ByteReader reader = ByteReader.From(ByteOrder.Little, data);
                int width = reader.Short();
                int height = reader.Short();
                Vec2I offset = (reader.Short(), reader.Short());

                if (offset.X < 0 || offset.Y < 0)
                    return Optional<PaletteImage>.Empty();

                int[] offsets = new int[width];
                for (int i = 0; i < width; i++)
                    offsets[i] = reader.Int();

                short[] indices = Arrays.Create(width * height, TransparentIndex);

                for (int col = 0; col < width; col++)
                {
                    reader.Offset = offsets[col];

                    while (true)
                    {
                        int rowStart = reader.Byte();
                        if (rowStart == 0xFF)
                            break;

                        int indicesCount = reader.Byte();
                        reader.Skip(1);
                        byte[] paletteIndices = reader.Bytes(indicesCount);
                        reader.Skip(1);

                        int indicesOffset = (rowStart * width) + col;
                        for (int i = 0; i < paletteIndices.Length; i++)
                        {
                            indices[indicesOffset] = paletteIndices[i];
                            indicesOffset += width;
                        }
                    }
                }

                ImageMetadata metadata = new ImageMetadata(offset, resourceNamespace);
                return new PaletteImage(width, height, indices, metadata);
            }
            catch
            {
                return Optional<PaletteImage>.Empty();
            }
        }

        /// <summary>
        /// Creates a palette image from a flat-based image.
        /// </summary>
        /// <param name="data">The data for the image.</param>
        /// <param name="resourceNamespace">The namespace this image belongs
        /// to.</param>
        /// <returns>The palette image for the data, or null if the image is
        /// corrupt and would not make a valid flat image.</returns>
        public static Optional<PaletteImage> FromFlat(byte[] data, ResourceNamespace resourceNamespace)
        {
            int width;
            int height;

            switch (data.Length)
            {
            case 64 * 64:
                width = 64;
                height = 64;
                break;
            case 64 * 65:
                width = 64;
                height = 65;
                break;
            case 128 * 128:
                width = 128;
                height = 128;
                break;
            default:
                return Optional<PaletteImage>.Empty();
            }

            int area = width * height;
            short[] indices = new short[area];
            for (int i = 0; i < area; i++)
                indices[i] = data[i];

            ImageMetadata metadata = new ImageMetadata(Vec2I.Zero, resourceNamespace);
            return new PaletteImage(width, height, indices, metadata);
        }

        /// <summary>
        /// Checks if the data is likely a flat formatted image.
        /// </summary>
        /// <param name="data">The data to check.</param>
        /// <returns>True if it is, false if not.</returns>
        public static bool LikelyFlat(byte[] data)
        {
            switch (data.Length * data.Length)
            {
            case 64 * 64:
            case 64 * 65:
            case 128 * 128:
                return true;
            default:
                return false;
            }
        }

        /// <summary>
        /// Converts this image to an RGBA image with the palette provided.
        /// </summary>
        /// <param name="palette">The palette to convert with.</param>
        /// <returns>The RGBA image.</returns>
        public RgbaImage ToColor(Palette palette)
        {
            PaletteLayer paletteLayer = palette.Default;

            Color[] pixels = new Color[Area];
            for (int i = 0; i < Area; i++)
            {
                // TODO: Could we extend the palette by 1 and set it to be transparent to avoid branching *every* pixel?
                short index = Indices[i];
                Color color = index == TransparentIndex ? RgbaImage.Transparent : paletteLayer[index];
                pixels[i] = color;
            }

            RgbaImage image = RgbaImage.From(Width, Height, pixels, Metadata).Value;
            Debug.Assert(image != null, "Should never fail converting a valid palette image to RGBA");
            return image;
        }
    }
}

using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Geometry;
using UnityEngine;

namespace Helion.Core.Graphics
{
    /// <summary>
    /// An image made up of RGBA colors.
    /// </summary>
    public class RgbaImage : IImage
    {
        public static readonly Color Transparent = new Color(0, 0, 0, 0);

        public Dimension Dimension { get; }
        public int Width => Dimension.Width;
        public int Height => Dimension.Height;
        public int Area => Dimension.Area;
        public ImageMetadata Metadata { get; }
        public readonly Color[] Pixels;

        private RgbaImage(int width, int height, Color[] pixels, ImageMetadata metadata = null)
        {
            Dimension = new Dimension(width, height);
            Pixels = pixels;
            Metadata = new ImageMetadata(metadata);
        }

        /// <summary>
        /// Creates an empty RGBA image from a given width, height, and
        /// possibly metadata. The image contents are undefined.
        /// </summary>
        /// <param name="width">The width of the image. Should not be negative.
        /// </param>
        /// <param name="height">The height of the image. Should not be
        /// negative.</param>
        /// <param name="metadata">The metadata to use, or null if we want a
        /// default one.</param>
        /// <returns>The RGBA image, or empty if the parameters are invalid
        /// and one cannot be made (ex: negative dimension).</returns>
        public static Optional<RgbaImage> From(int width, int height, ImageMetadata metadata = null)
        {
            if (width <= 0 || height <= 0)
                return Optional<RgbaImage>.Empty();

            Color[] pixels = Arrays.Create(width * height, Transparent);
            return new RgbaImage(width, height, pixels, metadata);
        }

        /// <summary>
        /// Creates an RGBA image from the pixels and dimensions provided.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="pixels">The pixels that make up the image.</param>
        /// <param name="metadata">The metadata for the image, which is optional
        /// and may be null.</param>
        /// <returns>The RGBA image, or an empty value if any dimension is not
        /// positive or the pixel size does not match the area provided.
        /// </returns>
        public static Optional<RgbaImage> From(int width, int height, Color[] pixels, ImageMetadata metadata = null)
        {
            if (width <= 0 || height <= 0 || pixels.Length != width * height)
                return Optional<RgbaImage>.Empty();

            return new RgbaImage(width, height, pixels, metadata);
        }

        /// <summary>
        /// Converts this into a 2D texture that can be used by Unity.
        /// </summary>
        /// <remarks>
        /// This loads in an inverted image because we assume the top left
        /// corner is the origin, whereas OpenGL/Unity use the bottom left
        /// as the origin.
        /// </remarks>
        /// <returns>A 2D texture made of the pixels from this image.</returns>
        public Texture2D ToTexture()
        {
            Texture2D texture = new Texture2D(Width, Height, TextureFormat.RGBA32, false);

            // Note: This assumes the bottom left is the origin. We assume the
            // top left is the origin. As such, the UV coordinates will need to
            // take this into account!
            texture.SetPixels(Pixels);
            texture.Apply();

            return texture;
        }
    }
}

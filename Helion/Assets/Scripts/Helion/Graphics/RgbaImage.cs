using System;
using Helion.Resource;
using Helion.Util;
using Helion.Util.Extensions;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;
using Helion.Util.Logging;
using UnityEngine;
using static Helion.Util.OptionalHelper;

namespace Helion.Graphics
{
    /// <summary>
    /// An image made up of RGBA colors.
    /// </summary>
    public class RgbaImage : IImage
    {
        public static readonly Color Transparent = new Color(0, 0, 0, 0);
        private static readonly Log Log = LogManager.Instance();

        public int Width => Dimension.Width;
        public int Height => Dimension.Height;
        public int Area => Dimension.Area;
        public ResourceNamespace Namespace { get; set; } = ResourceNamespace.Global;
        public Vec2I Offset { get; set; } = Vec2I.Zero;
        public Dimension Dimension { get; }
        public readonly Color[] Pixels;

        /// <summary>
        /// Creates a new RGBA image.
        /// </summary>
        /// <remarks>
        /// If the width or height are zero or less, it will be substituted in
        /// with a value of 1 for that axis.
        /// </remarks>
        /// <param name="dimension">The dimension of the image.</param>
        public RgbaImage(Dimension dimension) : this(dimension.Width, dimension.Height)
        {
        }

        /// <summary>
        /// Creates a new RGBA image.
        /// </summary>
        /// <remarks>
        /// If the width or height are zero or less, it will be substituted in
        /// with a value of 1 for that axis.
        /// </remarks>
        /// <param name="width">The width in pixels. Should be positive.
        /// </param>
        /// <param name="height">The height in pixels. Should be positive.
        /// </param>
        public RgbaImage(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                width = Math.Max(1, width);
                height = Math.Max(1, height);
                Log.Error($"Trying to create an RGBA image width a bad dimension ({width} x {height})");
            }

            Dimension = new Dimension(width, height);
            Pixels = new Color[Dimension.Area];
            Pixels.Fill(Transparent);
        }

        private RgbaImage(int width, int height, Color[] pixels)
        {
            Dimension = new Dimension(width, height);
            Pixels = pixels;
        }

        /// <summary>
        /// Creates an RGBA image from the pixels and dimensions provided.
        /// </summary>
        /// <param name="width">The width of the image.</param>
        /// <param name="height">The height of the image.</param>
        /// <param name="pixels">The pixels that make up the image.</param>
        /// <returns>The RGBA image, or an empty value if any dimension is not
        /// positive or the pixel size does not match the area provided.
        /// </returns>
        public static Optional<RgbaImage> From(int width, int height, Color[] pixels)
        {
            if (width <= 0 || height <= 0 || pixels.Length != width * height)
                return Empty;
            return new RgbaImage(width, height, pixels);
        }

        /// <summary>
        /// Takes the image given and draws it on top of the current image.
        /// Does not draw a pixel if the alpha is not fully opaque (as in
        /// alpha must be >= 1.0f).
        /// </summary>
        /// <param name="image">The image to draw on top.</param>
        /// <param name="topLeft">The top left corner to start drawing the
        /// image from.</param>
        /// <returns>True on success, false if it could not be written due to
        /// bounding issues (ex: image would write outside the bounds).
        /// </returns>
        public bool DrawOntoThis(RgbaImage image, Vec2I topLeft)
        {
            // We want to find the area we draw. This is effectively finding
            // the rectangle overlap. If there is none, we exit early.
            Vec2I drawStart = new Vec2I(Math.Max(0, topLeft.X), Math.Max(0, topLeft.Y));
            Vec2I drawEnd = new Vec2I(Math.Min(topLeft.X + image.Width, Width), Math.Min(topLeft.Y + image.Height, Height));
            Vec2I drawRange = drawEnd - drawStart;
            if (drawRange.X <= 0 || drawRange.Y <= 0)
                return false;

            // The starting coordinate from the source image is usually from
            // the top left corner, but if the offset is negative then we are
            // going to have to draw from the insides of the source image.
            Vec2I srcStart = Vec2I.Zero;
            if (topLeft.X < 0)
                srcStart = srcStart.WithX(-topLeft.X);
            if (topLeft.Y < 0)
                srcStart = srcStart.WithY(-topLeft.Y);

            // The destination coordinate is usually at the top left coordinate
            // provided, but if the top left is negative then we will start our
            // drawing at the zero index for that axis. If we have no drawing
            // overlap we'd have exited early before reaching this, so starting
            // at zero is okay.
            Vec2I destStart = topLeft;
            if (destStart.X < 0)
                destStart = destStart.WithX(0);
            if (destStart.Y < 0)
                destStart = destStart.WithY(0);

            // Now we have enough info to draw.
            RgbaImage src = image;
            RgbaImage dest = this;
            int srcOffset = (srcStart.Y * src.Width) + srcStart.X;
            int destOffset = (destStart.Y * dest.Width) + destStart.X;

            try
            {
                for (int y = 0; y < drawRange.Y; y++)
                {
                    int srcIndex = srcOffset;
                    int destIndex = destOffset;

                    for (int x = 0; x < drawRange.X; x++)
                    {
                        // For now, only draw the pixel if it is opaque. In the
                        // future we can do alpha blending.
                        Color srcColor = src.Pixels[srcIndex];
                        if (srcColor.a >= 1.0f)
                            dest.Pixels[destIndex] = srcColor;

                        srcIndex++;
                        destIndex++;
                    }

                    srcOffset += src.Width;
                    destOffset += dest.Width;
                }

                return true;
            }
            catch
            {
                return false;
            }
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

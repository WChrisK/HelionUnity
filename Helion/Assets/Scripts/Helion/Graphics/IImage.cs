using Helion.Resource;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;

namespace Helion.Graphics
{
    /// <summary>
    /// An image object represented by some size and metadata.
    /// </summary>
    public interface IImage
    {
        /// <summary>
        /// The width of the image.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// The height of the image.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// How many pixels are in the image.
        /// </summary>
        /// <remarks>
        /// Is equal to the width times the height.
        /// </remarks>
        int Area { get; }

        /// <summary>
        /// The dimensions of the image.
        /// </summary>
        Dimension Dimension { get; }

        /// <summary>
        /// The namespace this data was loaded from.
        /// </summary>
        ResourceNamespace Namespace { get; }

        /// <summary>
        /// The offset embedded in this texture.
        /// </summary>
        Vec2I Offset { get; }
    }
}
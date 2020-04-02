using Helion.Core.Resource;
using Helion.Core.Util.Geometry;

namespace Helion.Core.Graphics
{
    /// <summary>
    /// Metadata stored in an image.
    /// </summary>
    public class ImageMetadata
    {
        /// <summary>
        /// The offset of this image.
        /// </summary>
        public Vec2I Offset;

        /// <summary>
        /// The namespace this image was created in.
        /// </summary>
        public ResourceNamespace Namespace;

        /// <summary>
        /// Creates an empty metadata object.
        /// </summary>
        public ImageMetadata() : this(Vec2I.Zero, ResourceNamespace.Global)
        {
        }

        /// <summary>
        /// Creates a new copy of the given metadata.
        /// </summary>
        /// <param name="other">The other object to copy from. It may be null,
        /// in which case this object will have default values set.</param>
        public ImageMetadata(ImageMetadata other = null)
        {
            Offset = other?.Offset ?? Vec2I.Zero;
            Namespace = other?.Namespace ?? ResourceNamespace.Global;
        }

        /// <summary>
        /// Creates a metadata object for an image.
        /// </summary>
        /// <param name="offset">The cartesian offsets.</param>
        /// <param name="resourceNamespace">The image namespace.</param>
        public ImageMetadata(Vec2I offset, ResourceNamespace resourceNamespace)
        {
            Offset = offset;
            Namespace = resourceNamespace;
        }
    }
}

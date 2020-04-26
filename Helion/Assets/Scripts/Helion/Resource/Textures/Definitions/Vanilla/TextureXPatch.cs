using Helion.Util.Geometry;

namespace Helion.Resource.Textures.Definitions.Vanilla
{
    /// <summary>
    /// A patch that is in a TextureX image.
    /// </summary>
    public class TextureXPatch
    {
        /// <summary>
        /// The horizontal and vertical offset relative to the upper left
        /// corner of the texture.
        /// </summary>
        public readonly Vec2I Offset;

        /// <summary>
        /// The index in a PNames entry.
        /// </summary>
        public readonly short PatchIndex;

        /// <summary>
        /// An unused value.
        /// </summary>
        public readonly short StepDirection;

        /// <summary>
        /// Another unused value.
        /// </summary>
        public readonly short Colormap;

        /// <summary>
        /// Creates a TextureX patch from the data provided.
        /// </summary>
        /// <param name="offset">The offset from the top left.</param>
        /// <param name="patchIndex">The PNames index.</param>
        /// <param name="stepDirection">An unused value.</param>
        /// <param name="colormap">Another unused value.</param>
        public TextureXPatch(Vec2I offset, short patchIndex, short stepDirection, short colormap)
        {
            Offset = offset;
            PatchIndex = patchIndex;
            StepDirection = stepDirection;
            Colormap = colormap;
        }
    }
}

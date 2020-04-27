using Helion.Util;

namespace Helion.Resource.Textures.Sprites
{
    /// <summary>
    /// A collection of the sprite rotations for some sprite.
    /// </summary>
    public class SpriteRotations
    {
        /// <summary>
        /// The sprite base name (ex: "ABCDE").
        /// </summary>
        public readonly UpperString Name;

        /// <summary>
        /// True if the frames 2/3/4 and 6/7/8 are mirrored. This does not
        /// apply to frames 1 and 8.
        /// </summary>
        public readonly bool Mirrored;

        /// <summary>
        /// Intended for textures that should never be rendered (ex: NULLA or
        /// TNT1A).
        /// </summary>
        public readonly bool DoNotRender;

        private readonly Texture[] textures;

        public SpriteRotations(UpperString name) : this(name, TextureManager.NullTexture)
        {
            DoNotRender = true;
        }

        public SpriteRotations(UpperString name, Texture frame0)
        {
            Name = name;
            textures = new[] { frame0, frame0, frame0, frame0, frame0, frame0, frame0, frame0 };
        }

        public SpriteRotations(UpperString name, Texture frame1, Texture frame2and8,
            Texture frame3and7, Texture frame4and6, Texture frame5)
        {
            Name = name;
            Mirrored = true;
            textures = new[] { frame1, frame2and8, frame3and7, frame4and6, frame5, frame4and6, frame3and7, frame2and8 };
        }

        public SpriteRotations(UpperString name, Texture frame1, Texture frame2,
            Texture frame3, Texture frame4, Texture frame5, Texture frame6, Texture frame7,
            Texture frame8)
        {
            Name = name;
            textures = new[] { frame1, frame2, frame3, frame4, frame5, frame6, frame7, frame8 };
        }

        public Texture this[int index] => textures[index];
    }
}

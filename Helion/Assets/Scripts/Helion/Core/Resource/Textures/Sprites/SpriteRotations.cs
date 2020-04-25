using Helion.Core.Util;

namespace Helion.Core.Resource.Textures.Sprites
{
    /// <summary>
    /// A collection of the sprite rotations for some sprite.
    /// </summary>
    public class SpriteRotations
    {
        public readonly UpperString Name;
        public readonly bool Mirrored;
        private readonly Texture[] textures;

        public SpriteRotations(UpperString name, Texture frame0)
        {
            Name = name;
            Mirrored = false;
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
            Mirrored = false;
            textures = new[] { frame1, frame2, frame3, frame4, frame5, frame6, frame7, frame8 };
        }

        public Texture this[int index] => textures[index];
    }
}

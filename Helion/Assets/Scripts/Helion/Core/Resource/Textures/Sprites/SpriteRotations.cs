using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Resource.Textures.Sprites
{
    /// <summary>
    /// A collection of the sprite rotations for some sprite.
    /// </summary>
    public class SpriteRotations
    {
        public readonly UpperString Name;
        public readonly bool Mirrored;
        private readonly Material[] materials;

        public SpriteRotations(UpperString name, Material frame0)
        {
            Name = name;
            Mirrored = false;
            materials = new[] { frame0, frame0, frame0, frame0, frame0, frame0, frame0, frame0 };
        }

        public SpriteRotations(UpperString name, Material frame1, Material frame2and8,
            Material frame3and7, Material frame4and6, Material frame5)
        {
            Name = name;
            Mirrored = true;
            materials = new[] { frame1, frame2and8, frame3and7, frame4and6, frame5, frame4and6, frame3and7, frame2and8 };
        }

        public SpriteRotations(UpperString name, Material frame1, Material frame2,
            Material frame3, Material frame4, Material frame5, Material frame6, Material frame7,
            Material frame8)
        {
            Name = name;
            Mirrored = false;
            materials = new[] { frame1, frame2, frame3, frame4, frame5, frame6, frame7, frame8 };
        }
    }
}

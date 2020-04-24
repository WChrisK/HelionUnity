using System;
using Helion.Core.Resource.Textures;
using Helion.Core.Util;
using Helion.Core.Util.Unity;
using Helion.Core.Worlds.Geometry.Enums;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    public class Wall : IDisposable
    {
        public readonly int Index;
        public readonly Side Side;
        public readonly WallSection Section;
        public Material Material { get; private set; }
        private readonly GameObject gameObject;
        private UpperString textureName;

        public bool OnFrontSide => ReferenceEquals(Side.Line.Front, Side);
        public bool OnBackSide => !OnFrontSide;
        public UpperString TextureName
        {
            get => textureName;
            set
            {
                if (value != null)
                {
                    textureName = value;
                    Material = TextureManager.Material(value);
                }
            }
        }

        public Wall(int index, Side side, WallSection section)
        {
            Index = index;
            Side = side;
            Section = section;
            gameObject = new GameObject($"Wall {index} ({section}) [Line {side.Line.Index}, Side {side.Index}]");

            side.Walls.Add(this);
        }

        public void Dispose()
        {
            GameObjectHelper.Destroy(gameObject);
        }
    }
}

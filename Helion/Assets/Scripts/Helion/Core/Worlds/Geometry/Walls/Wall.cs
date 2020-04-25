using System;
using Helion.Core.Resource.Textures;
using Helion.Core.Util;
using Helion.Core.Util.Unity;
using UnityEngine;
using Texture = Helion.Core.Resource.Textures.Texture;

namespace Helion.Core.Worlds.Geometry.Walls
{
    public class Wall : IDisposable
    {
        public readonly int Index;
        public readonly Side Side;
        public readonly WallSection Section;
        public Texture Texture { get; private set; }
        private readonly GameObject gameObject;
        private readonly WallMeshComponents meshComponents;
        private UpperString textureName;

        public UpperString TextureName => textureName;
        public bool OnFrontSide => ReferenceEquals(Side.Line.Front, Side);
        public bool OnBackSide => !OnFrontSide;
        public Line Line => Side.Line;

        public Wall(int index, Side side, WallSection section)
        {
            Index = index;
            Side = side;
            Section = section;
            textureName = GetTextureNameFrom(side, section);
            Texture = TextureManager.Texture(textureName);
            gameObject = new GameObject($"Wall {index} ({section}) [Line {side.Line.Index}, Side {side.Index}]");
            meshComponents = new WallMeshComponents(this, gameObject, Texture);

            AttachToSectorPlanes();
            side.Walls.Add(this);

            UpdateWallMesh();
        }

        public void UpdateWallMesh()
        {
            meshComponents.Update();
        }

        public void SetTexture(UpperString newTextureName)
        {
            textureName = newTextureName;
            Texture = TextureManager.Texture(newTextureName);
            meshComponents.SetTexture(Texture);
        }

        public void Dispose()
        {
            meshComponents.Dispose();
            GameObjectHelper.Destroy(gameObject);
        }

        private static UpperString GetTextureNameFrom(Side side, WallSection section)
        {
            switch (section)
            {
            case WallSection.Lower:
                return side.LowerTextureName;
            case WallSection.Middle:
                return side.MiddleTextureName;
            case WallSection.Upper:
                return side.UpperTextureName;
            default:
                throw new Exception($"Unknown wall section to get side texture from: {section}");
            }
        }

        private void AttachToSectorPlanes()
        {
            if (Side.Line.OneSided)
            {
                Side.Sector.Floor.WallListeners.Add(this);
                Side.Sector.Ceiling.WallListeners.Add(this);
                return;
            }

            Sector frontSector = Side.Line.Front.Sector;
            Sector backSector = Side.Line.Back.Value.Sector;

            switch (Section)
            {
            case WallSection.Lower:
                frontSector.Floor.WallListeners.Add(this);
                backSector.Floor.WallListeners.Add(this);
                break;
            case WallSection.Middle:
                frontSector.Floor.WallListeners.Add(this);
                frontSector.Ceiling.WallListeners.Add(this);
                backSector.Floor.WallListeners.Add(this);
                backSector.Ceiling.WallListeners.Add(this);
                break;
            case WallSection.Upper:
                frontSector.Ceiling.WallListeners.Add(this);
                backSector.Ceiling.WallListeners.Add(this);
                break;
            default:
                throw new Exception($"Unexpected section type for wall attachment: {Section}");
            }
        }
    }
}

using System;
using Helion.Resource.Textures;
using Helion.Util;
using Texture = Helion.Resource.Textures.Texture;

namespace Helion.Worlds.Geometry.Walls
{
    public class Wall : IDisposable
    {
        public readonly int Index;
        public readonly Side Side;
        public readonly WallSection Section;
        public UpperString TextureName { get; }
        public Texture Texture { get; }

        private readonly WallMeshComponents meshComponents;

        public bool OnFrontSide => ReferenceEquals(Side.Line.Front, Side);
        public bool OnBackSide => !OnFrontSide;
        public Line Line => Side.Line;

        public bool IsTwoSidedNoMiddle => Section == WallSection.Middle && Line.TwoSided && TextureName == Constants.NoTexture;

        public Wall(int index, Side side, WallSection section)
        {
            Index = index;
            Side = side;
            Section = section;
            TextureName = GetTextureNameFrom(side, section);
            Texture = TextureManager.Texture(TextureName);
            meshComponents = new WallMeshComponents(this, Texture);

            AttachToSectorPlanes();
            side.Walls.Add(this);
        }

        public void Update()
        {
            meshComponents.Update();
        }

        public void Dispose()
        {
            meshComponents.Dispose();
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

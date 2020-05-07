using System;
using Helion.Resource.Textures;
using Helion.Util;
using Helion.Util.Geometry.Boxes;
using Texture = Helion.Resource.Textures.Texture;

namespace Helion.Worlds.Geometry.Walls
{
    public class Wall : IRenderable, IDisposable
    {
        public readonly int Index;
        public readonly Side Side;
        public readonly WallSection Section;
        public UpperString TextureName { get; }
        public Texture Texture { get; }
        public float FloorHeight;
        public float CeilingHeight;

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

            (SectorPlane floor, SectorPlane ceiling) = FindBoundingPlane();
            FloorHeight = floor.Height;
            CeilingHeight = ceiling.Height;

            AttachToSectorPlanes();
            side.Walls.Add(this);
        }

        /// <summary>
        /// Checks if a box intersects this wall. Intersection is considered to
        /// be fully crossing, not just touching (for compatibility reasons).
        /// </summary>
        /// <param name="box">The box to check.</param>
        /// <returns>True if the box intersects the wall, false if not.
        /// </returns>
        public bool IntersectedBy(in Box3F box)
        {
            if (!Line.Segment.Intersects(box.XZ))
                return false;

            return !(box.Max.Y <= FloorHeight || box.Min.Y >= CeilingHeight);
        }

        public void NotifyPlaneUpdate()
        {
            // TODO: Update plane boundings.
        }

        public void Update(float tickFraction)
        {
            meshComponents.Update(tickFraction);
        }

        public void Dispose()
        {
            meshComponents.Dispose();
        }

        internal (SectorPlane floor, SectorPlane ceiling) FindBoundingPlane()
        {
            Sector facingSector = Side.Sector;
            if (Line.OneSided)
                return (facingSector.Floor, facingSector.Ceiling);

            Sector partnerSector = Side.PartnerSide.Value.Sector;
            if (OnBackSide)
                (facingSector, partnerSector) = (partnerSector, facingSector);

            SectorPlane facingFloor = facingSector.Floor;
            SectorPlane facingCeiling = facingSector.Ceiling;
            SectorPlane partnerFloor = partnerSector.Floor;
            SectorPlane partnerCeiling = partnerSector.Ceiling;

            switch (Section)
            {
            case WallSection.Lower:
                return (facingFloor, partnerFloor);
            case WallSection.Middle:
                return (facingFloor.Height >= partnerFloor.Height ? facingFloor : partnerFloor,
                    facingCeiling.Height <= partnerCeiling.Height ? facingCeiling : partnerCeiling);
            case WallSection.Upper:
                return (partnerCeiling, facingCeiling);
            default:
                throw new Exception($"Unexpected section type for wall attachment: {Section}");
            }
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

using System.Collections.Generic;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using Helion.Core.Worlds.Geometry.Lines;
using UnityEngine;
using static Helion.Core.Util.OptionalHelper;

namespace Helion.Core.Worlds.Geometry
{
    public class Side
    {
        public readonly int Index;
        public readonly Line Line;
        public readonly Sector Sector;
        public readonly Optional<Wall> Upper;
        public readonly Optional<Wall> Middle;
        public readonly Optional<Wall> Lower;
        public Vector2 Offset;
        public Optional<Side> PartnerSide = Empty;

        public bool IsFront => ReferenceEquals(this, Line.Front);

        public Side(Line line, bool isFront, DoomLinedef linedef, DoomSidedef sidedef, IList<Sector> sectors,
            GameObject parentGameObject)
        {
            Index = sidedef.Index;
            Line = line;
            Sector = sectors[sidedef.Sector.Index];
            Offset = sidedef.Offset;

            if (linedef.OneSided)
            {
                Lower = Empty;
                Middle = CreateOneSidedMiddle(this, line, linedef, sidedef, sectors, parentGameObject);
                Upper = Empty;
            }
            else
            {
                Lower = CreateTwoSidedLower(this, line, isFront, linedef, sidedef, sectors, parentGameObject);
                Middle = CreateTwoSidedMiddle(this, line, isFront, linedef, sidedef, sectors, parentGameObject);
                Upper = CreateTwoSidedUpper(this, line, isFront, linedef, sidedef, sectors, parentGameObject);
            }
        }

        private static Wall CreateOneSidedMiddle(Side side, Line line, DoomLinedef linedef,
            DoomSidedef sidedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            Sector sector = sectors[sidedef.Sector.Index];
            SectorPlane lowerPlane = sector.FloorPlane;
            SectorPlane upperPlane = sector.CeilingPlane;
            UpperString textureName = sidedef.MiddleTexture;

            return new Wall(side, line, true, textureName, lowerPlane, upperPlane, WallSection.MiddleOneSided, parentGameObject);
        }

        private static Optional<Wall> CreateTwoSidedLower(Side side, Line line, bool isFront,
            DoomLinedef linedef, DoomSidedef sidedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            DoomSidedef partnerSide = linedef.PartnerSideOf(sidedef);
            Sector sector = sectors[sidedef.Sector.Index];
            Sector partnerSector = sectors[partnerSide.Sector.Index];
            SectorPlane lowerPlane = sector.FloorPlane;
            SectorPlane upperPlane = partnerSector.FloorPlane;
            UpperString textureName = sidedef.LowerTexture;

            if (lowerPlane.Height >= upperPlane.Height)
                return Empty;

            return new Wall(side, line, isFront, textureName, lowerPlane, upperPlane, WallSection.Lower, parentGameObject);
        }

        private static Optional<Wall> CreateTwoSidedMiddle(Side side, Line line, bool isFront,
            DoomLinedef linedef, DoomSidedef sidedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            if (sidedef.MiddleTexture == Constants.NoTexture)
                return Empty;

            DoomSidedef partnerSide = linedef.PartnerSideOf(sidedef);
            Sector sector = sectors[sidedef.Sector.Index];
            Sector partnerSector = sectors[partnerSide.Sector.Index];
            SectorPlane frontLowerPlane = sector.FloorPlane;
            SectorPlane frontUpperPlane = sector.CeilingPlane;
            SectorPlane backLowerPlane = partnerSector.FloorPlane;
            SectorPlane backUpperPlane = partnerSector.CeilingPlane;
            SectorPlane lowerPlane = frontLowerPlane.Height >= backLowerPlane.Height ? frontLowerPlane : backLowerPlane;
            SectorPlane upperPlane = frontUpperPlane.Height <= backUpperPlane.Height ? frontUpperPlane : backUpperPlane;
            UpperString textureName = sidedef.MiddleTexture;

            if (lowerPlane.Height >= upperPlane.Height)
                return Empty;

            return new Wall(side, line, isFront, textureName, lowerPlane, upperPlane, WallSection.MiddleTwoSided, parentGameObject);
        }

        private static Optional<Wall> CreateTwoSidedUpper(Side side, Line line, bool isFront,
            DoomLinedef linedef, DoomSidedef sidedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            DoomSidedef partnerSide = linedef.PartnerSideOf(sidedef);
            Sector sector = sectors[sidedef.Sector.Index];
            Sector partnerSector = sectors[partnerSide.Sector.Index];
            SectorPlane lowerPlane = partnerSector.CeilingPlane;
            SectorPlane upperPlane = sector.CeilingPlane;
            UpperString textureName = sidedef.UpperTexture;

            if (lowerPlane.Height >= upperPlane.Height)
                return Empty;

            return new Wall(side, line, isFront, textureName, lowerPlane, upperPlane, WallSection.Upper, parentGameObject);
        }
    }
}

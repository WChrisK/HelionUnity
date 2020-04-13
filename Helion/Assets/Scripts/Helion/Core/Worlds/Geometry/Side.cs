using System.Collections.Generic;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using UnityEngine;

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

        public Side(Line line, DoomLinedef linedef, DoomSidedef sidedef, IList<Sector> sectors,
            GameObject parentGameObject)
        {
            Index = sidedef.Index;
            Line = line;
            Sector = sectors[sidedef.Sector.Index];
            Offset = sidedef.Offset;

            if (linedef.OneSided)
            {
                Upper = Optional<Wall>.Empty();
                Middle = CreateOneSidedMiddle(this, linedef, sidedef, sectors, parentGameObject);
                Lower = Optional<Wall>.Empty();
            }
            else
            {
                Upper = CreateTwoSidedUpper(this, linedef, sidedef, sectors, parentGameObject);
                Middle = CreateTwoSidedMiddle(this, linedef, sidedef, sectors, parentGameObject);
                Lower = CreateTwoSidedLower(this, linedef, sidedef, sectors, parentGameObject);
            }
        }

        private static Wall CreateOneSidedMiddle(Side side, DoomLinedef linedef,
            DoomSidedef sidedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            Sector sector = sectors[sidedef.Sector.Index];
            SectorPlane lowerPlane = sector.FloorPlane;
            SectorPlane upperPlane = sector.CeilingPlane;
            UpperString textureName = sidedef.MiddleTexture;

            return new Wall(side, textureName, lowerPlane, upperPlane, WallSection.Middle, parentGameObject);
        }

        private static Optional<Wall> CreateTwoSidedLower(Side side, DoomLinedef linedef,
            DoomSidedef sidedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            // TODO
            return Optional<Wall>.Empty();
        }

        private static Optional<Wall> CreateTwoSidedMiddle(Side side, DoomLinedef linedef,
            DoomSidedef sidedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            if (sidedef.MiddleTexture == Constants.NoTexture)
                return Optional<Wall>.Empty();

            // TODO
            return Optional<Wall>.Empty();
        }

        private static Optional<Wall> CreateTwoSidedUpper(Side side, DoomLinedef linedef,
            DoomSidedef sidedef, IList<Sector> sectors, GameObject parentGameObject)
        {
            // TODO
            return Optional<Wall>.Empty();
        }
    }
}

using System.Collections.Generic;
using Helion.Core.Resource.MapsNew.Components;
using Helion.Core.Util;
using Helion.Core.Util.Geometry;
using Helion.Core.Worlds.Geometry.Walls;

namespace Helion.Core.Worlds.Geometry
{
    public class Side
    {
        public readonly int Index;
        public readonly Sector Sector;
        public readonly List<Wall> Walls = new List<Wall>();
        public Line Line { get; internal set; }
        public Vec2I Offset;
        internal readonly UpperString LowerTextureName;
        internal readonly UpperString MiddleTextureName;
        internal readonly UpperString UpperTextureName;

        public Optional<Side> PartnerSide => ReferenceEquals(Line.Front, this) ? Line.Back : Line.Front;

        public Side(int index, MapSidedef sidedef, Sector sector)
        {
            Index = index;
            Sector = sector;
            Offset = sidedef.Offset;
            LowerTextureName = sidedef.LowerTexture;
            MiddleTextureName = sidedef.MiddleTexture;
            UpperTextureName = sidedef.UpperTexture;

            sector.Sides.Add(this);
        }

        public bool TryGetWall(WallSection section, out Wall wall)
        {
            foreach (Wall childWall in Walls)
            {
                if (childWall.Section == section)
                {
                    wall = childWall;
                    return true;
                }
            }

            wall = null;
            return false;
        }
    }
}

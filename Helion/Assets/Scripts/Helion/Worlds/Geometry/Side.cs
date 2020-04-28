using System.Collections.Generic;
using Helion.Resource.Maps.Components;
using Helion.Util;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;
using Helion.Worlds.Geometry.Walls;

namespace Helion.Worlds.Geometry
{
    /// <summary>
    /// A side of a Line object.
    /// </summary>
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

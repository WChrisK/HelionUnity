using System;
using System.Collections.Generic;
using Helion.Bsp;
using Helion.Bsp.Node;
using Helion.Resource.Maps;
using Helion.Resource.Maps.Components;
using Helion.Util.Geometry.Vectors;
using Helion.Util.Logging;
using Helion.Worlds.Geometry.Bsp;
using Helion.Worlds.Geometry.Subsectors;
using Helion.Worlds.Geometry.Walls;

namespace Helion.Worlds.Geometry
{
    /// <summary>
    /// Contains all of the map geometry (walls, ceilings, floors).
    /// </summary>
    public class MapGeometry : IDisposable
    {
        private static readonly Log Log = LogManager.Instance();

        public readonly BspTree BspTree;
        public readonly List<Line> Lines = new List<Line>();
        public readonly List<Sector> Sectors = new List<Sector>();
        public readonly List<SectorPlane> SectorPlanes = new List<SectorPlane>();
        public readonly List<Side> Sides = new List<Side>();
        public readonly List<Wall> Walls = new List<Wall>();

        public List<Subsector> Subsectors => BspTree.Subsectors;

        public MapGeometry(MapData map)
        {
            CreateSectorsAndPlanes(map);
            CreateSides(map);
            CreateLines(map);
            CreateWalls();
            BspTree = CreateSubsectorsAndBspTree(map);
        }

        public void Dispose()
        {
            BspTree.Dispose();
            Walls.ForEach(wall => wall.Dispose());
        }

        private BspTree CreateSubsectorsAndBspTree(MapData map)
        {
            try
            {
                BspNode root = new BspBuilder(map).Build();
                if (root == null)
                    throw new Exception("Failed to generate BSP tree");
                if (root.IsDegenerate)
                    throw new Exception("Generated BSP tree is degenerate (malformed map?)");

                return new BspTree(this, root);
            }
            catch (Exception e)
            {
                Log.Error($"Unexpected BSP building error: {e.Message}");
                throw;
            }
        }

        private void CreateSectorsAndPlanes(MapData map)
        {
            foreach (MapSector mapSector in map.Sectors)
            {
                SectorPlane floor = new SectorPlane(SectorPlanes.Count, false,
                    mapSector.FloorHeight, mapSector.FloorTexture);
                SectorPlanes.Add(floor);

                SectorPlane ceiling = new SectorPlane(SectorPlanes.Count, true,
                    mapSector.CeilingHeight, mapSector.CeilingTexture);
                SectorPlanes.Add(ceiling);

                Sector sector = new Sector(Sectors.Count, mapSector, floor, ceiling);
                Sectors.Add(sector);
            }
        }

        private void CreateSides(MapData map)
        {
            foreach (MapSidedef sidedef in map.Sidedefs)
            {
                Sector sector = Sectors[sidedef.SectorID];

                Side side = new Side(Sides.Count, sidedef, sector);
                Sides.Add(side);
            }
        }

        private void CreateLines(MapData map)
        {
            foreach (MapLinedef linedef in map.Linedefs)
            {
                Vec2F start = map.Vertices[linedef.StartVertex].Float();
                Vec2F end = map.Vertices[linedef.EndVertex].Float();
                Side front = Sides[linedef.FrontSide];
                Side back = linedef.BackSide != null ? Sides[linedef.BackSide.Value] : null;

                Line line = new Line(Lines.Count, linedef, start, end, front, back);
                Lines.Add(line);
            }
        }

        private void CreateWalls()
        {
            // TODO: Will break on sidedef compressed maps: sides can be shared, geometry will be missing.
            foreach (Side side in Sides)
            {
                Wall middle = new Wall(Walls.Count, side, WallSection.Middle);
                Walls.Add(middle);

                if (side.Line.TwoSided)
                {
                    Wall upper = new Wall(Walls.Count, side, WallSection.Upper);
                    Walls.Add(upper);

                    Wall lower = new Wall(Walls.Count, side, WallSection.Lower);
                    Walls.Add(lower);
                }
            }
        }
    }
}

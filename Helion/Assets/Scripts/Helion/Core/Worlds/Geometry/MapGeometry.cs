using System.Collections.Generic;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Util.Extensions;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    public class MapGeometry
    {
        public readonly List<Line> Lines = new List<Line>();
        public readonly List<Side> Sides = new List<Side>();
        public readonly List<Wall> Walls = new List<Wall>();
        public readonly List<Sector> Sectors = new List<Sector>();
        public readonly List<SectorPlane> SectorPlanes = new List<SectorPlane>();
        public readonly List<Subsector> Subsectors = new List<Subsector>();
        private GameObject geometryGameObject;
        private GameObject wallsGameObject;
        private GameObject subsectorsGameObject;

        public MapGeometry(GameObject parentObject, IMap map)
        {
            CreateGameObjects(parentObject);
            CreateGeometryFrom(map);
        }

        private void CreateGameObjects(GameObject parentObject)
        {
            geometryGameObject = new GameObject("Geometry");
            parentObject.SetChild(geometryGameObject);

            wallsGameObject = new GameObject("Walls");
            geometryGameObject.SetChild(wallsGameObject);

            subsectorsGameObject = new GameObject("Subsectors");
            geometryGameObject.SetChild(subsectorsGameObject);
        }

        private void CreateGeometryFrom(IMap map)
        {
            // This will go away when we have a universal format!
            DoomMap doomMap = (DoomMap)map;

            CreateSectors(doomMap.Sectors);
            CreateSubsectors(doomMap.Subsectors);
            CreateLines(doomMap.Linedefs);
        }

        private void CreateSectors(IList<DoomSector> mapSectors)
        {
            foreach (DoomSector sec in mapSectors)
            {
                int floorIndex = SectorPlanes.Count;
                SectorPlane floorPlane = new SectorPlane(floorIndex, sec.FloorTexture, sec.FloorHeight, sec.LightLevel);
                SectorPlanes.Add(floorPlane);

                int ceilingIndex = SectorPlanes.Count;
                SectorPlane ceilingPlane = new SectorPlane(ceilingIndex, sec.CeilingTexture, sec.CeilingHeight, sec.LightLevel);
                SectorPlanes.Add(ceilingPlane);

                int sectorIndex = Sectors.Count;
                Sector sector = new Sector(sectorIndex, floorPlane, ceilingPlane, sec.LightLevel);
                Sectors.Add(sector);
            }
        }

        private void CreateSubsectors(IList<GLSubsector> mapSubsectors)
        {
            foreach (GLSubsector glSubsector in mapSubsectors)
            {
                Sector sector = Sectors[glSubsector.Sector.Index];
                Subsector subsector = new Subsector(glSubsector, sector, subsectorsGameObject);
                Subsectors.Add(subsector);
                sector.Subsectors.Add(subsector);
            }
        }

        private void CreateLines(IList<DoomLinedef> linedefs)
        {
            foreach (DoomLinedef linedef in linedefs)
            {
                Line line = new Line(linedef, Sectors, wallsGameObject);
                Lines.Add(line);

                TrackSideAndWalls(line.Front);
                if (line.Back)
                    TrackSideAndWalls(line.Back.Value);
            }

            void TrackSideAndWalls(Side side)
            {
                Sides.Add(side);
                if (side.Upper)
                    Walls.Add(side.Upper.Value);
                if (side.Middle)
                    Walls.Add(side.Middle.Value);
                if (side.Lower)
                    Walls.Add(side.Lower.Value);
            }
        }
    }
}

using System.Collections.Generic;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using Helion.Core.Worlds.Geometry;

namespace Helion.Core.Worlds
{
    public class World
    {
        private readonly List<Wall> walls;
        private readonly List<Subsector> subsectors;

        public World(List<Wall> walls, List<Subsector> subsectors)
        {
            this.walls = walls;
            this.subsectors = subsectors;
        }

        public static Optional<World> From(IMap map)
        {
            switch (map)
            {
            case DoomMap doomMap:
                return ReadDoomMap(doomMap);
            }

            return Optional<World>.Empty();
        }

        private static Optional<World> ReadDoomMap(DoomMap map)
        {
            List<Wall> walls = new List<Wall>();
            List<Subsector> subsectors = new List<Subsector>();

            foreach (DoomLinedef line in map.Linedefs)
            {
                if (line.TwoSided || line.Length.ApproxZero())
                    continue;

                Wall wall = new Wall(line.Front);
                walls.Add(wall);
            }

            foreach (GLSubsector glSubsector in map.Subsectors)
            {
                Subsector subsector = new Subsector(glSubsector);
                subsectors.Add(subsector);
            }

            return new World(walls, subsectors);
        }
    }
}

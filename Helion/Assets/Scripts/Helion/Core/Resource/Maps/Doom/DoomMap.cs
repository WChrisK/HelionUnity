using System.Collections.Generic;
using Helion.Core.Util;

namespace Helion.Core.Resource.Maps.Doom
{
    public class DoomMap : IMap
    {
        public IList<DoomVertex> Vertices;
        // TODO: Sectors
        // TODO: Sidedefs
        // TODO: Linedefs
        // TODO: Nodes
        // TODO: Things

        public MapType Type => MapType.Doom;

        private DoomMap(IList<DoomVertex> vertices)
        {
            Vertices = vertices;
        }

        public static Optional<IMap> From(MapComponents components)
        {
            if (!components.IsValid() || components.MapType != MapType.Doom)
                return Optional<IMap>.Empty();

            try
            {
                IList<DoomVertex> vertices = ReadVertices(components);
                // TODO: Sectors
                // TODO: Sidedefs
                // TODO: Linedefs
                // TODO: Nodes
                // TODO: Things

                IMap map = new DoomMap(vertices);
                return new Optional<IMap>(map);
            }
            catch
            {
                return Optional<IMap>.Empty();
            }
        }

        private static IList<DoomVertex> ReadVertices(MapComponents components)
        {
            IList<DoomVertex> vertices = new List<DoomVertex>();

            // TODO

            return vertices;
        }
    }
}

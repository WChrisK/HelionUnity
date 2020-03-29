using System;
using System.Collections.Generic;
using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Util;
using Helion.Core.Util.Bytes;
using Helion.Core.Util.Geometry;

namespace Helion.Core.Resource.Maps.Doom
{
    public class DoomMap : IMap
    {
        public readonly IList<DoomVertex> Vertices;
        public readonly IList<DoomVertex> GLVertices;
        public readonly IList<DoomSector> Sectors;
        public readonly IList<DoomSidedef> Sidedefs;
        public readonly IList<DoomLinedef> Linedefs;
        public readonly IList<DoomThing> Things;
        public readonly IList<GLNode> Nodes;

        public MapType Type => MapType.Doom;

        private DoomMap(IList<DoomVertex> vertices, IList<DoomVertex> glVertices,
            IList<DoomSector> sectors, IList<DoomSidedef> sidedefs, IList<DoomLinedef> linedefs,
            IList<DoomThing> things, IList<GLNode> nodes)
        {
            Vertices = vertices;
            GLVertices = glVertices;
            Sectors = sectors;
            Sidedefs = sidedefs;
            Linedefs = linedefs;
            Things = things;
            Nodes = nodes;
        }

        public static Optional<IMap> From(MapComponents components)
        {
            if (!components.IsValid() || components.MapType != MapType.Doom)
                return Optional<IMap>.Empty();

            try
            {
                IList<DoomVertex> vertices = ReadVertices(components);
                IList<DoomVertex> glVertices = ReadGLVertices(components);
                IList<DoomSector> sectors = ReadSectors(components);
                IList<DoomSidedef> sidedefs = ReadSidedefs(components, sectors);
                IList<DoomLinedef> linedefs = ReadLinedefs(components, sidedefs);
                IList<DoomThing> things = ReadThings(components);
                IList<GLNode> nodes = ReadGLNodes(components, vertices, glVertices);

                IMap map = new DoomMap(vertices, glVertices, sectors, sidedefs, linedefs, things, nodes);
                return new Optional<IMap>(map);
            }
            catch
            {
                return Optional<IMap>.Empty();
            }
        }

        internal static IList<DoomVertex> ReadVertices(MapComponents components)
        {
            IList<DoomVertex> vertices = new List<DoomVertex>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Vertices.Value.Data);

            int count = reader.Length / 4;
            for (int i = 0; i < count; i++)
            {
                float x = reader.Short();
                float y = reader.Short();
                DoomVertex vertex = new DoomVertex(x, y);
                vertices.Add(vertex);
            }

            return vertices;
        }

        internal static IList<DoomVertex> ReadGLVertices(MapComponents components)
        {
            IList<DoomVertex> glVertices = new List<DoomVertex>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.GLVertices.Value.Data);

            string header = reader.String(4);
            if (header != "gNd2")
                throw new Exception("Currently unsupported (expected gNd2 for GL_VERT)");

            int count = (reader.Length - 4) / 8;
            for (int i = 0; i < count; i++)
            {
                float x = new Fixed(reader.Int()).Float();
                float y = new Fixed(reader.Int()).Float();
                DoomVertex vertex = new DoomVertex(x, y);
                glVertices.Add(vertex);
            }

            return glVertices;
        }

        internal static IList<DoomSector> ReadSectors(MapComponents components)
        {
            // TODO
            return new List<DoomSector>();
        }

        internal static IList<DoomSidedef> ReadSidedefs(MapComponents components, IList<DoomSector> sectors)
        {
            // TODO
            return new List<DoomSidedef>();
        }

        internal static IList<DoomLinedef> ReadLinedefs(MapComponents components, IList<DoomSidedef> sidedefs)
        {
            // TODO
            return new List<DoomLinedef>();
        }

        internal static IList<DoomThing> ReadThings(MapComponents components)
        {
            // TODO
            return new List<DoomThing>();
        }

        internal static IList<GLNode> ReadGLNodes(MapComponents components, IList<DoomVertex> vertices, IList<DoomVertex> glVertices)
        {
            // TODO
            return new List<GLNode>();
        }
    }
}

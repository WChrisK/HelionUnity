using System;
using System.Collections.Generic;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util.Bytes;
using Helion.Core.Util.Geometry;

namespace Helion.Core.Resource.Maps.Shared
{
    /// <summary>
    /// A helper class for reading GL components.
    /// </summary>
    public static class GLReader
    {
        public static IList<DoomVertex> ReadDoomGLVertices(MapComponents components)
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

        public static IList<GLSubsector> ReadDoomGLSubsectors(MapComponents components, IList<DoomVertex> vertices, IList<DoomVertex> glVertices)
        {
            // TODO
            return new List<GLSubsector>();
        }

        public static IList<GLNode> ReadDoomGLNodes(MapComponents components, IList<GLSubsector> subsectors)
        {
            // TODO
            return new List<GLNode>();
        }
    }
}

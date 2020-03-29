using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Util;
using Helion.Core.Util.Bytes;
using Helion.Core.Util.Geometry;
using UnityEngine;

namespace Helion.Core.Resource.Maps.Doom
{
    public class DoomMap : IMap
    {
        private const int BytesPerLine = 14;
        protected const int BytesPerSector = 26;
        protected const int BytesPerSide = 30;
        private const int BytesPerThing = 10;
        protected const int BytesPerVertex = 4;
        public const ushort NoSidedef = (ushort)0xFFFFU;

        public readonly IList<MapVertex> Vertices;
        public readonly IList<MapVertex> GLVertices;
        public readonly IList<DoomSector> Sectors;
        public readonly IList<DoomSidedef> Sidedefs;
        public readonly IList<DoomLinedef> Linedefs;
        public readonly IList<DoomThing> Things;
        public readonly IList<GLSegment> Segments;
        public readonly IList<GLSubsector> Subsectors;
        public readonly IList<GLNode> Nodes;

        public MapType Type => MapType.Doom;

        private DoomMap(IList<MapVertex> vertices, IList<MapVertex> glVertices,
            IList<DoomSector> sectors, IList<DoomSidedef> sidedefs, IList<DoomLinedef> linedefs,
            IList<DoomThing> things, IList<GLSegment> segments, IList<GLSubsector> subsectors,
            IList<GLNode> nodes)
        {
            Vertices = vertices;
            GLVertices = glVertices;
            Sectors = sectors;
            Sidedefs = sidedefs;
            Linedefs = linedefs;
            Things = things;
            Segments = segments;
            Subsectors = subsectors;
            Nodes = nodes;
        }

        public static Optional<IMap> From(MapComponents components)
        {
            if (!components.IsValid() || components.MapType != MapType.Doom)
                return Optional<IMap>.Empty();

            try
            {
                IList<MapVertex> vertices = ReadVertices(components);
                IList<MapVertex> glVertices = GLReader.ReadGLVertices(components);
                IList<DoomSector> sectors = ReadSectors(components);
                IList<DoomSidedef> sidedefs = ReadSidedefs(components, sectors);
                IList<DoomLinedef> linedefs = ReadLinedefs(components, vertices, sidedefs);
                IList<DoomThing> things = ReadThings(components);
                IList<GLSegment> segments = GLReader.ReadGLSegments(components, vertices, glVertices);
                IList<GLSubsector> subsectors = GLReader.ReadGLSubsectors(components, segments);
                IList<GLNode> nodes = GLReader.ReadGLNodes(components, subsectors);
                AssertWellFormedGeometryOrThrow(sidedefs, linedefs, subsectors);

                IMap map = new DoomMap(vertices, glVertices, sectors, sidedefs, linedefs, things, segments, subsectors, nodes);
                return new Optional<IMap>(map);
            }
            catch
            {
                return Optional<IMap>.Empty();
            }
        }

        internal static IList<MapVertex> ReadVertices(MapComponents components)
        {
            List<MapVertex> vertices = new List<MapVertex>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Vertices.Value.Data);

            int count = reader.Length / BytesPerVertex;
            for (int i = 0; i < count; i++)
            {
                float x = reader.Short();
                float y = reader.Short();
                MapVertex vertex = new MapVertex(x, y);
                vertices.Add(vertex);
            }

            return vertices;
        }

        internal static IList<DoomSector> ReadSectors(MapComponents components)
        {
            List<DoomSector> sectors = new List<DoomSector>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Sectors.Value.Data);

            int count = reader.Length / BytesPerSector;
            for (int i = 0; i < count; i++)
            {
                short floorHeight = reader.Short();
                short ceilingHeight = reader.Short();
                string floorTexture = reader.StringWithoutNulls(8).ToUpper();
                string ceilTexture = reader.StringWithoutNulls(8).ToUpper();
                short lightLevel = reader.Short();
                ushort specialBits = reader.UShort();
                ushort tag = reader.UShort();

                DoomSector sector = new DoomSector(floorHeight, ceilingHeight, floorTexture, ceilTexture, lightLevel, specialBits, tag);
                sectors.Add(sector);
            }

            return sectors;
        }

        internal static IList<DoomSidedef> ReadSidedefs(MapComponents components, IList<DoomSector> sectors)
        {
            List<DoomSidedef> sides = new List<DoomSidedef>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Sidedefs.Value.Data);

            int count = reader.Length / BytesPerSide;
            for (int i = 0; i < count; i++)
            {
                Vector2 offset = new Vector2(reader.Short(), reader.Short());
                UpperString upperTexture = reader.StringWithoutNulls(8);
                UpperString lowerTexture = reader.StringWithoutNulls(8);
                UpperString middleTexture = reader.StringWithoutNulls(8);
                DoomSector sector = sectors[reader.UShort()];

                DoomSidedef side = new DoomSidedef(offset, upperTexture, middleTexture, lowerTexture, sector);
                sides.Add(side);
            }

            return sides;
        }

        internal static IList<DoomLinedef> ReadLinedefs(MapComponents components, IList<MapVertex> vertices,
            IList<DoomSidedef> sidedefs)
        {
            List<DoomLinedef> lines = new List<DoomLinedef>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Linedefs.Value.Data);

            int count = reader.Length / BytesPerLine;
            for (int i = 0; i < count; i++)
            {
                MapVertex startVertex = vertices[reader.UShort()];
                MapVertex endVertex = vertices[reader.UShort()];
                ushort flags = reader.UShort();
                ushort type = reader.UShort();
                ushort sectorTag = reader.UShort();
                DoomSidedef front = sidedefs[reader.UShort()];
                ushort leftSidedef = reader.UShort();
                DoomSidedef back = (leftSidedef != NoSidedef ? sidedefs[leftSidedef] : null);

                DoomLinedef line = new DoomLinedef(startVertex, endVertex, front, back, flags, type, sectorTag);
                lines.Add(line);

                front.Line = line;
                if (back != null)
                    back.Line = line;
            }

            return lines;
        }

        internal static IList<DoomThing> ReadThings(MapComponents components)
        {
            List<DoomThing> things = new List<DoomThing>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Things.Value.Data);

            int count = reader.Length / BytesPerThing;
            for (int id = 0; id < count; id++)
            {
                float x = new Fixed(reader.Short(), 0).Float();
                float y = new Fixed(reader.Short(), 0).Float();
                Vector2 position = new Vector2(x, y);
                ushort angle = reader.UShort();
                ushort editorNumber = reader.UShort();
                ushort flags = reader.UShort();

                DoomThing thing = new DoomThing(position, angle, editorNumber, flags);
                things.Add(thing);
            }

            return things;
        }

        private static void AssertWellFormedGeometryOrThrow(IEnumerable<DoomSidedef> sidedefs,
            IEnumerable<DoomLinedef> linedefs, IEnumerable<GLSubsector> subsectors)
        {
            if (sidedefs.Any(side => side.Line == null))
                throw new Exception("Missing parent line for side");
            if (linedefs.Any(line => line.Front == null))
                throw new Exception("Missing front side for line");
            if (subsectors.Any(subsector => subsector.Segments.Count < 3))
                throw new Exception("Subsector is degenerate");
        }
    }
}

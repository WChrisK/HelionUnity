using System;
using Helion.Resource.Maps;
using Helion.Resource.Maps.Components;
using Helion.Util;
using Helion.Util.Bytes;
using Helion.Util.Extensions;
using Helion.Util.Geometry;
using UnityEngine;

namespace Helion.Resource.Maps.Readers
{
    /// <summary>
    /// A reader of doom maps.
    /// </summary>
    public static class DoomMapReader
    {
        private const int BytesPerLine = 14;
        private const int BytesPerSector = 26;
        private const int BytesPerSide = 30;
        private const int BytesPerThing = 10;
        private const int BytesPerVertex = 4;
        private const ushort NoSidedef = (ushort)0xFFFFU;

        /// <summary>
        /// Reads the map components and tries to parse a doom map.
        /// </summary>
        /// <param name="components">The map components.</param>
        /// <param name="map">The map that was generated, or null if this
        /// returns false.</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool TryRead(MapComponents components, out MapData map)
        {
            if (!components.IsValid() || components.MapType != MapType.Doom)
            {
                map = null;
                return false;
            }

            try
            {
                map = new MapData(components.Name);
                ReadVerticesOrThrow(map, components);
                ReadSectorsOrThrow(map, components);
                ReadSidesOrThrow(map, components);
                ReadLinesOrThrow(map, components);
                ReadThingsOrThrow(map, components);

                return true;
            }
            catch
            {
                map = null;
                return false;
            }
        }

        private static void ReadVerticesOrThrow(MapData map, MapComponents components)
        {
            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Vertices.Value.Data);

            int count = reader.Length / BytesPerVertex;
            for (int index = 0; index < count; index++)
            {
                MapVertex vertex = new MapVertex(index, reader.Short(), reader.Short());
                map.Vertices.Add(vertex);
            }
        }

        private static void SetSpecialBits(MapSector sector, ushort bits)
        {
            // TODO
        }

        private static void ReadSectorsOrThrow(MapData map, MapComponents components)
        {
            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Sectors.Value.Data);

            int count = reader.Length / BytesPerSector;
            for (int index = 0; index < count; index++)
            {
                MapSector sector = new MapSector(index)
                {
                    FloorHeight = reader.Short(),
                    CeilingHeight = reader.Short(),
                    FloorTexture = reader.StringWithoutNulls(8).ToUpper(),
                    CeilingTexture = reader.StringWithoutNulls(8).ToUpper(),
                    LightLevel = reader.Short()
                };
                SetSpecialBits(sector, reader.UShort());
                sector.Tag = reader.UShort();

                map.Sectors.Add(sector);
            }
        }

        private static void ReadSidesOrThrow(MapData map, MapComponents components)
        {
            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Sidedefs.Value.Data);

            int count = reader.Length / BytesPerSide;
            for (int index = 0; index < count; index++)
            {
                MapSidedef sidedef = new MapSidedef(index)
                {
                    Offset = new Vec2I(reader.Short(), reader.Short()),
                    UpperTexture = reader.StringWithoutNulls(8),
                    LowerTexture = reader.StringWithoutNulls(8),
                    MiddleTexture = reader.StringWithoutNulls(8),
                    SectorID = reader.UShort()
                };

                if (!sidedef.SectorID.InRangeExclusive(0, map.Sectors.Count))
                    throw new Exception($"Sidedef {index} has out of range sector: {sidedef.SectorID}");

                map.Sidedefs.Add(sidedef);
            }
        }

        private static void SetLineSpecial(MapLinedef linedef, int flags, ushort type, ushort sectorTag)
        {
            if (flags.HasBits(0x0008))
                linedef.UpperUnpegged = true;
            if (flags.HasBits(0x0010))
                linedef.LowerUnpegged = true;

            // TODO
        }

        private static void ReadLinesOrThrow(MapData map, MapComponents components)
        {
            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Linedefs.Value.Data);

            int count = reader.Length / BytesPerLine;
            for (int index = 0; index < count; index++)
            {
                MapLinedef linedef = new MapLinedef(index)
                {
                    StartVertex = reader.UShort(),
                    EndVertex = reader.UShort()
                };

                ushort flags = reader.UShort();
                ushort type = reader.UShort();
                ushort sectorTag = reader.UShort();
                SetLineSpecial(linedef, flags, type, sectorTag);

                linedef.FrontSide = reader.UShort();
                int backID = reader.UShort();
                if (backID != NoSidedef)
                    linedef.BackSide = backID;

                if (!linedef.StartVertex.InRangeExclusive(0, map.Vertices.Count))
                    throw new Exception($"Line {index} has out of range start vertex: {linedef.StartVertex}");
                if (!linedef.EndVertex.InRangeExclusive(0, map.Vertices.Count))
                    throw new Exception($"Line {index} has out of range start vertex: {linedef.EndVertex}");
                if (!linedef.FrontSide.InRangeExclusive(0, map.Sidedefs.Count))
                    throw new Exception($"Line {index} has out of range front sidedef: {linedef.FrontSide}");
                if (linedef.BackSide != null && !linedef.FrontSide.InRangeExclusive(0, map.Sidedefs.Count))
                    throw new Exception($"Line {index} has out of range back sidedef: {linedef.BackSide.Value}");

                map.Linedefs.Add(linedef);
            }
        }

        private static void SetThingFlags(MapThing thing, int flagBits)
        {
            if (flagBits.HasBits(0x0001))
            {
                thing.Skill1 = true;
                thing.Skill2 = true;
            }

            if (flagBits.HasBits(0x0002))
                thing.Skill3 = true;

            if (flagBits.HasBits(0x0004))
            {
                thing.Skill4 = true;
                thing.Skill5 = true;
            }

            if (flagBits.HasBits(0x0008))
                thing.Deaf = true;

            thing.InSinglePlayer = !flagBits.HasBits(0x0010);
            thing.InDeathmatch = !flagBits.HasBits(0x0020);
            thing.InCooperative = !flagBits.HasBits(0x0040);
        }

        private static void ReadThingsOrThrow(MapData map, MapComponents components)
        {
            ByteReader reader = ByteReader.From(ByteOrder.Little, components.Things.Value.Data);

            int count = reader.Length / BytesPerThing;
            for (int index = 0; index < count; index++)
            {
                MapThing thing = new MapThing(index);
                float x = new Fixed(reader.Short(), 0).Float();
                float y = new Fixed(reader.Short(), 0).Float();
                thing.Position = new Vector2(x, y);
                thing.AngleDegrees = (int)(reader.UShort() * 65535.0 / 360.0);
                thing.EditorID = reader.UShort();
                SetThingFlags(thing, reader.UShort());

                map.Things.Add(thing);
            }
        }
    }
}

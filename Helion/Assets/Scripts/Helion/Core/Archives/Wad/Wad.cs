using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Helion.Core.Resource;
using Helion.Core.Util;
using Helion.Core.Util.Bytes;

namespace Helion.Core.Archives.Wad
{
    /// <summary>
    /// The encapsulation of a Wad archive.
    /// </summary>
    public class Wad : IArchive
    {
        private readonly List<WadEntry> entries = new List<WadEntry>();
        private readonly Dictionary<UpperString, List<WadEntry>> nameToEntry = new Dictionary<UpperString, List<WadEntry>>();

        private Wad(List<WadEntry> wadEntries)
        {
            entries.AddRange(wadEntries);

            wadEntries.ForEach(entry =>
            {
                if (nameToEntry.TryGetValue(entry.Name, out List<WadEntry> existingEntries))
                    existingEntries.Add(entry);
                else
                    nameToEntry[entry.Name] = new List<WadEntry> { entry };
            });
        }

        /// <summary>
        /// Reads a wad file from a path provided.
        /// </summary>
        /// <param name="path">The path to read from.</param>
        /// <returns>The wad file, or an empty optional if it cannot be read
        /// from.</returns>
        public static Optional<Wad> From(string path)
        {
            try
            {
                byte[] data = File.ReadAllBytes(path);
                List<WadEntry> entries = ReadEntriesOrThrow(data);
                return new Wad(entries);
            }
            catch
            {
                return Optional<Wad>.Empty();
            }
        }

        public Optional<IEntry> Find(UpperString name)
        {
            return nameToEntry.TryGetValue(name, out List<WadEntry> existingEntries) ?
                existingEntries.FirstOrDefault() :
                Optional<IEntry>.Empty();
        }

        public IEnumerable<IEntry> FindAll(UpperString name)
        {
            // Can't wait to use C# 8 again one day so this is simpler...
            if (nameToEntry.TryGetValue(name, out List<WadEntry> existingEntries))
                return existingEntries;
            return new List<IEntry>();
        }

        public IArchiveMapIterator GetMaps() => new WadArchiveMapIterator(this);

        public IEnumerator<IEntry> GetEnumerator() => entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static List<WadEntry> ReadEntriesOrThrow(byte[] wadData)
        {
            ByteReader reader = ByteReader.From(ByteOrder.Little, wadData);

            // The header must exist fully or else we can't read it.
            if (!reader.HasRemaining(12))
                throw new Exception("Cannot read Wad");

            WadResourceNamespaceTracker namespaceTracker = new WadResourceNamespaceTracker();
            List<WadEntry> entries = new List<WadEntry>();

            WadHeader header = ReadHeader(reader);
            reader.Offset = header.DirectoryTableOffset;

            for (int i = 0; i < header.EntryCount; i++)
            {
                WadDirectoryEntry dirEntry = ReadWadEntry(reader);

                ResourceNamespace resourceNamespace = namespaceTracker.Update(dirEntry);
                byte[] data = reader.Bytes(dirEntry.Size, dirEntry.Offset);

                // Unfortunately binary readers can silently fail and just
                // consume the remaining data. We want to let the caller
                // know the wad is in fact corrupt if it can't read enough.
                if (data.Length != dirEntry.Size)
                    throw new Exception("Malformed wad entry length");

                WadEntry entry = new WadEntry(dirEntry.Name, resourceNamespace, data);
                entries.Add(entry);
            }

            return entries;
        }

        private static WadHeader ReadHeader(ByteReader reader)
        {
            bool iwad = reader.String(4).ToUpper() == "IWAD";
            int entryCount = reader.Int();
            int dirOffset = reader.Int();

            return new WadHeader(iwad, entryCount, dirOffset);
        }

        private static WadDirectoryEntry ReadWadEntry(ByteReader reader)
        {
            int offset = reader.Int();
            int size = reader.Int();
            UpperString name = reader.StringWithoutNulls(8);

            return new WadDirectoryEntry(offset, size, name);
        }
    }
}

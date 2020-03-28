using System;
using System.Collections.Generic;
using Helion.Core.Archive;
using Helion.Core.Archive.Wad;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Resource
{
    /// <summary>
    /// A collection of all the loaded resources for a collection of archives.
    /// </summary>
    public static class GameData
    {
        private static List<IArchive> archives = new List<IArchive>();

        /// <summary>
        /// Loads all of the archives at the file paths provided.
        /// </summary>
        /// <param name="filePaths">The paths to the archives on the hard drive
        /// to load.</param>
        /// <returns>True if they all loaded, false if any failed.</returns>
        public static bool Load(IEnumerable<string> filePaths)
        {
            archives = new List<IArchive>();

            foreach (string filePath in filePaths)
            {
                Optional<Wad> wad = Wad.From(filePath);
                if (!wad)
                {
                    Debug.Log($"Unable to open archive: {filePath}");
                    return false;
                }

                if (!ProcessArchive(wad.Value))
                {
                    Debug.Log($"Error processing archive data for: {filePath}");
                    return false;
                }

                archives.Add(wad.Value);
            }

            return true;
        }

        /// <summary>
        /// Finds the latest entry from all the loaded archives.
        /// </summary>
        /// <param name="name">The entry to find.</param>
        /// <returns>The entry if it exists, or an empty optional if not.
        /// </returns>
        public static Optional<IEntry> Find(UpperString name)
        {
            for (int i = archives.Count - 1; i >= 0; i--)
            {
                Optional<IEntry> entry = archives[i].Find(name);
                if (entry)
                    return entry;
            }

            return Optional<IEntry>.Empty();
        }

        /// <summary>
        /// Gets all of the entries. The latest loaded ones are at the front of
        /// the enumerable.
        /// </summary>
        /// <param name="name">The entry name to search for.</param>
        /// <returns>A collection of entries, which is empty if there are no
        /// matches</returns>
        public static IEnumerable<IEntry> FindAll(UpperString name)
        {
            List<IEntry> entries = new List<IEntry>();
            for (int i = archives.Count - 1; i >= 0; i--)
                entries.AddRange(archives[i].FindAll(name));

            return entries;
        }

        /// <summary>
        /// Finds a map with the name provided, or if it cannot be found then
        /// returns an empty value.
        /// </summary>
        /// <param name="name">The map name.</param>
        /// <returns>The map, or an empty optional if no map name matches.
        /// </returns>
        public static Optional<IMap> FindMap(UpperString name)
        {
            for (int i = archives.Count - 1; i >= 0; i--)
            {
                foreach (MapComponents mapComponents in archives[i].GetMaps())
                {
                    if (mapComponents.Name != name)
                        continue;

                    switch (mapComponents.MapType)
                    {
                    case MapType.Doom:
                        return DoomMap.From(mapComponents);
                    case MapType.Hexen:
                        return Optional<IMap>.Empty();
                    case MapType.UDMF:
                        return Optional<IMap>.Empty();
                    default:
                        throw new ArgumentOutOfRangeException($"Unexpected map type when finding map {name}");
                    }
                }
            }

            return Optional<IMap>.Empty();
        }

        private static bool ProcessArchive(IArchive archive)
        {
            // TODO
            return true;
        }
    }
}

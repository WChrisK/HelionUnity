using System;
using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Resource.Textures;
using Helion.Core.Resource.Textures.Definitions;
using Helion.Core.Util;
using Helion.Core.Util.Logging;

namespace Helion.Core.Resource
{
    public class DataNew
    {
        private static readonly Log Log = LogManager.Instance();

        public static readonly List<IArchive> Archives = new List<IArchive>();
        private static readonly ResourceTracker<IEntry> entries = new ResourceTracker<IEntry>();

        public static bool Load(IEnumerable<string> uris)
        {
            List<IArchive> archiveList = new List<IArchive>();

            foreach (string uri in uris)
            {
                Optional<IArchive> archive = ArchiveReader.ReadFile(uri);
                if (archive)
                    archiveList.Add(archive.Value);
                else
                {
                    Log.Error($"Unable to open or read archive: {uri}");
                    return false;
                }
            }

            Archives.Clear();
            Archives.AddRange(archiveList);

            return ProcessArchives();
        }

        public static bool TryFind(UpperString name, out IEntry entry)
        {
            return entries.TryGetAnyValue(name, out entry, out _);
        }

        public static bool TryFind(UpperString name, ResourceNamespace resourceNamespace, out IEntry entry)
        {
            return entries.TryGetValue(name, resourceNamespace, out entry);
        }

        public static List<IEntry> TryFindAll(UpperString name)
        {
            return entries.TryGetAnyValues(name);
        }

        /// <summary>
        /// Finds a map with the name provided. If the map that was found ends
        /// up being corrupt or it could not be found, this returns false.
        /// </summary>
        /// <param name="name">The map name.</param>
        /// <param name="map">The found map, or null if none was found.</param>
        /// <returns>The map, or an empty optional if no map name matches.
        /// </returns>
        public static bool TryFindMap(UpperString name, out IMap map)
        {
            map = null;

            for (int i = Archives.Count - 1; i >= 0; i--)
            {
                foreach (MapComponents mapComponents in Archives[i].GetMaps())
                {
                    if (mapComponents.Name != name)
                        continue;

                    switch (mapComponents.MapType)
                    {
                    case MapType.Doom:
                        Optional<IMap> doomMap = DoomMap.From(mapComponents);
                        map = doomMap.Value;
                        return doomMap.HasValue;
                    case MapType.Hexen:
                        return false;
                    case MapType.UDMF:
                        return false;
                    default:
                        throw new ArgumentOutOfRangeException($"Unexpected map type when finding map {name}");
                    }
                }
            }

            return false;
        }

        private static bool ProcessArchives()
        {
            try
            {
                entries.Clear();
                TextureManagerNew.Clear();
                TextureDefinitionManager.Clear();

                // TODO

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

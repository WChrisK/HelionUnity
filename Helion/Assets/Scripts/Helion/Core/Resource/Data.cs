using System;
using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Configs;
using Helion.Core.Resource.Decorate;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.MapsNew;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Resource.Textures;
using Helion.Core.Resource.Textures.Definitions;
using Helion.Core.Resource.Textures.Sprites;
using Helion.Core.Util;
using Helion.Core.Util.Logging;

namespace Helion.Core.Resource
{
    /// <summary>
    /// A collection of all of the entries loaded from an archive. All archive
    /// loading should be done through this class, as it will populate the rest
    /// of the classes with data.
    /// </summary>
    public static class Data
    {
        private static readonly Log Log = LogManager.Instance();

        public static Config Config = new Config();
        public static readonly List<IArchive> Archives = new List<IArchive>();
        private static readonly ResourceTracker<IEntry> entries = new ResourceTracker<IEntry>();
        private static readonly Dictionary<UpperString, IEntry> nameToEntry = new Dictionary<UpperString, IEntry>();
        private static readonly Dictionary<UpperString, IEntry> pathToEntry = new Dictionary<UpperString, IEntry>();

        /// <summary>
        /// Attempts to load the data at the URIs provided. This will destroy
        /// any existing data.
        /// </summary>
        /// <param name="uris">The URIs to load.</param>
        /// <returns>True on success, false on failure.</returns>
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

        /// <summary>
        /// Finds the last loaded entry with the name provided.
        /// </summary>
        /// <param name="name">The entry name.</param>
        /// <param name="entry">The entry that was found.</param>
        /// <returns>True if found, false if not (and then entry is set to
        /// null).</returns>
        public static bool TryFindLatest(UpperString name, out IEntry entry)
        {
            return nameToEntry.TryGetValue(name, out entry);
        }

        /// <summary>
        /// Finds the last loaded entry with the name provided. This will
        /// include the extension in searches.
        /// </summary>
        /// <param name="path">The entry path.</param>
        /// <param name="entry">The entry that was found.</param>
        /// <returns>True if found, false if not (and then entry is set to
        /// null).</returns>
        public static bool TryFindPath(UpperString path, out IEntry entry)
        {
            return pathToEntry.TryGetValue(path, out entry);
        }

        /// <summary>
        /// Finds the latest entry for the name and namespace.
        /// </summary>
        /// <param name="name">The entry name.</param>
        /// <param name="resourceNamespace">The namespace.</param>
        /// <param name="entry">The found entry.</param>
        /// <returns>True if found, false if not (and entry is null in this
        /// case).</returns>
        public static bool TryFindExact(UpperString name, ResourceNamespace resourceNamespace, out IEntry entry)
        {
            return entries.TryGetValue(name, resourceNamespace, out entry);
        }

        /// <summary>
        /// Looks up an entry by name. Note that this is not guaranteed to be
        /// the latest value, but rather the latest one from an implementation
        /// defined namespace.
        /// </summary>
        /// <param name="name">The entry name.</param>
        /// <param name="entry">The entry.</param>
        /// <returns>True if found, false if not.</returns>
        public static bool TryFindAny(UpperString name, out IEntry entry)
        {
            return entries.TryGetAnyValue(name, out entry, out _);
        }

        /// <summary>
        /// Gets all of the entries for all namespaces for the name provided.
        /// </summary>
        /// <param name="name">The entries to find.</param>
        /// <returns>The found entries, or an empty list otherwise.</returns>
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
                nameToEntry.Clear();
                pathToEntry.Clear();
                TextureManager.Clear();
                TextureDefinitionManager.Clear();
                SpriteManager.Clear();
                DecorateManager.Clear();

                foreach (IArchive archive in Archives)
                {
                    Log.Info("Loading ", archive.Uri);

                    // We want every entry to be tracked before processing any
                    // definition files.
                    foreach (IEntry entry in archive)
                    {
                        entries.Add(entry.Name, entry.Namespace, entry);
                        nameToEntry[entry.Name] = entry;
                        pathToEntry[entry.Path.ToString()] = entry;
                    }

                    foreach (IEntry entry in archive)
                    {
                        switch (entry.Name.String)
                        {
                        case "DECORATE":
                            DecorateManager.HandleDefinitionsOrThrow(entry, archive);
                            continue;
                        case "PLAYPAL":
                            TextureManager.TrackPalette(entry);
                            continue;
                        case "PNAMES":
                        case "TEXTURE1":
                        case "TEXTURE2":
                            TextureDefinitionManager.TrackVanillaDefinition(entry);
                            continue;
                        }
                    }

                    TextureDefinitionManager.CompileAnyNewVanillaDefinitions();
                }

                DecorateManager.AttachSpriteRotationsToFrames();

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}

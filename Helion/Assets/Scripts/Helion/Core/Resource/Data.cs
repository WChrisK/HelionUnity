using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Core.Archives;
using Helion.Core.Configs;
using Helion.Core.Resource.Decorate;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Resource.Textures;
using Helion.Core.Resource.Textures.Sprites;
using Helion.Core.Util;
using Helion.Core.Util.Logging;
using MoreLinq;

namespace Helion.Core.Resource
{
    /// <summary>
    /// A collection of all of the data. It is a static singleton (RIP).
    /// </summary>
    public static class Data
    {
        private static readonly Log Log = LogManager.Instance();

        // TODO: Break initialize order dependency (use singletons? oh man...)
        public static Config Config = new Config();
        public static TextureManager Textures = new TextureManager();
        public static SpriteManager Sprites = new SpriteManager(Textures);
        public static DecorateManager Decorate = new DecorateManager();
        private static List<IArchive> archives = new List<IArchive>();
        private static ResourceTracker<IEntry> entries = new ResourceTracker<IEntry>();
        private static Dictionary<UpperString, List<IEntry>> nameToEntries = new Dictionary<UpperString, List<IEntry>>();

        /// <summary>
        /// Loads a config from either the path provided, or the default path.
        /// </summary>
        /// <param name="path">The path to use, or null if the default path
        /// should be used.</param>
        /// <returns>True if the load was successful, false if not.</returns>
        public static bool LoadConfig(string path = null)
        {
            path = path ?? Config.DefaultConfigName;

            Optional<Config> config = Config.FromFile(path);
            if (!config)
            {
                Log.Error("Failed to load config from: ", path);
                Log.Info("Creating empty config, will save on exit");
                return false;
            }

            Log.Info("Loaded config from ", path);
            Config = config.Value;
            return true;
        }

        /// <summary>
        /// Loads all of the archives at the file paths provided. If this fails
        /// then the state of the application will be in a malformed state.
        /// </summary>
        /// <param name="filePaths">The paths to the archives on the hard drive
        /// to load.</param>
        /// <returns>True if they all loaded, false if any failed.</returns>
        public static bool Load(params string[] filePaths)
        {
            try
            {
                archives = ReadArchivesOrThrow(filePaths);

                // Any game objects that need disposing must be done first,
                // since we could leak valuable memory/resources if we do not
                // dispose Unity things (as GC won't get them until an unload,
                // or possibly never).
                Textures.Dispose();

                // Then initialize a clean slate, so if we call this multiple
                // times we will load new resources cleanly each time.
                entries = new ResourceTracker<IEntry>();
                nameToEntries = new Dictionary<UpperString, List<IEntry>>();
                Textures = new TextureManager();
                Sprites = new SpriteManager(Textures);
                Decorate = new DecorateManager();

                ProcessArchivesOrThrow();

                return true;
            }
            catch (Exception e)
            {
                Log.Error("Unable to load files: ", e.Message);
                return false;
            }
        }

        /// <summary>
        /// Finds the latest entry from all the loaded archives.
        /// </summary>
        /// <param name="name">The entry to find.</param>
        /// <returns>The entry if it exists, or an empty optional if not.
        /// </returns>
        public static Optional<IEntry> Find(UpperString name)
        {
            if (nameToEntries.TryGetValue(name, out List<IEntry> entryList))
                return new Optional<IEntry>(entryList.Last());
            return Optional<IEntry>.Empty();
        }

        /// <summary>
        /// Finds the latest entry for the exact namespace provided. This will
        /// not return any entry that has another namespace. To find any entry
        /// but with a namespace priority, use <see cref="FindPriority"/>.
        /// </summary>
        /// <param name="name">The entry to find.</param>
        /// <param name="resourceNamespace">The namespace to match.</param>
        /// <returns>The entry if it exists, or an empty optional if not.
        /// </returns>
        public static Optional<IEntry> Find(UpperString name, ResourceNamespace resourceNamespace)
        {
            if (entries.TryGetValue(name, resourceNamespace, out IEntry entry))
                return new Optional<IEntry>(entry);
            return Optional<IEntry>.Empty();
        }

        /// <summary>
        /// Searches for all entries with the namespace provided, and if it
        /// cannot find one with the priority namespace then will return the
        /// most recent entry. If no entry exists with the name at all, then
        /// an empty value is returned.
        /// </summary>
        /// <param name="name">The entry to find.</param>
        /// <param name="priority">The priority namespace to look in, or global
        /// by default.</param>
        /// <returns>The entry if it exists, or an empty optional if not.
        /// </returns>
        public static Optional<IEntry> FindPriority(UpperString name, ResourceNamespace priority = ResourceNamespace.Global)
        {
            Optional<IEntry> entryForNamespace = Find(name, priority);
            return entryForNamespace ? entryForNamespace : Find(name);
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

        private static List<IArchive> ReadArchivesOrThrow(IEnumerable<string> filePaths)
        {
            List<IArchive> archiveList = new List<IArchive>();

            foreach (string filePath in filePaths)
            {
                Optional<IArchive> archive = ArchiveReader.ReadFile(filePath);
                if (archive)
                    archiveList.Add(archive.Value);
                else
                    throw new Exception($"Unable to open or read archive: {filePath}");
            }

            return archiveList;
        }

        private static void TrackEntry(IEntry entry)
        {
            entries.Add(entry.Name, entry.Namespace, entry);

            if (nameToEntries.TryGetValue(entry.Name, out List<IEntry> entryList))
                entryList.Add(entry);
            else
                nameToEntries[entry.Name] = new List<IEntry> { entry };
        }

        private static void ProcessArchivesOrThrow()
        {
            foreach (IArchive archive in archives)
            {
                Log.Info("Loading ", archive.Uri);

                // We want every entry to be tracked before processing any
                // definition files.
                archive.ForEach(TrackEntry);

                // Now that every entry has been indexed, we can process any
                // definition/data entries.
                foreach (IEntry entry in archive)
                {
                    switch (entry.Name.String)
                    {
                    case "DECORATE":
                        Decorate.HandleDefinitionsOrThrow(entry, archive);
                        continue;
                    case "PLAYPAL":
                        Textures.HandlePaletteOrThrow(entry);
                        continue;
                    case "PNAMES":
                        Textures.TrackPNamesOrThrow(entry, archive);
                        continue;
                    case "TEXTURE1":
                    case "TEXTURE2":
                        Textures.TrackTextureXOrThrow(entry, archive);
                        continue;
                    }
                }
            }

            Textures.FinishPostProcessingOrThrow();
            Decorate.AttachSpriteRotationsToFrames();
        }
    }
}

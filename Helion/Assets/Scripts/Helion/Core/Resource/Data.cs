using System;
using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Configs;
using Helion.Core.Resource.Decorate;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Resource.Textures;
using Helion.Core.Util;
using Helion.Core.Util.Logging;

namespace Helion.Core.Resource
{
    /// <summary>
    /// A collection of all of the data
    /// </summary>
    public static class Data
    {
        public static Config Config = new Config();
        public static DecorateManager Decorate = new DecorateManager();
        public static TextureManager Textures = new TextureManager();
        private static readonly Log Log = LogManager.Instance();
        private static List<IArchive> Archives = new List<IArchive>();

        /// <summary>
        /// Loads a config from either the path provided, or the default path.
        /// </summary>
        /// <param name="path">The path to use, or null if the default path
        /// should be used.</param>
        /// <returns>True if the load was successful, false if not.</returns>
        public static bool LoadConfig(string path = null)
        {
            path = path ?? Constants.ConfigName;

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
                Archives = ReadArchivesOrThrow(filePaths);

                // Any game objects that need disposing must be done first,
                // since we could leak valuable memory/resources if we do not
                // dispose Unity things (as GC won't get them until an unload,
                // or possibly never).
                Textures.Dispose();

                // Then initialize a clean slate, so if we call this multiple
                // times we will load new resources cleanly each time.
                Decorate = new DecorateManager();
                Textures = new TextureManager();

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
            for (int i = Archives.Count - 1; i >= 0; i--)
            {
                Optional<IEntry> entry = Archives[i].Find(name);
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
            for (int i = Archives.Count - 1; i >= 0; i--)
                entries.AddRange(Archives[i].FindAll(name));

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
            for (int i = Archives.Count - 1; i >= 0; i--)
            {
                foreach (MapComponents mapComponents in Archives[i].GetMaps())
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
            List<IArchive> archives = new List<IArchive>();

            foreach (string filePath in filePaths)
            {
                Optional<IArchive> archive = ArchiveReader.ReadFile(filePath);
                if (archive)
                    archives.Add(archive.Value);
                else
                    throw new Exception($"Unable to open or read archive: {filePath}");
            }

            return archives;
        }

        private static void ProcessArchivesOrThrow()
        {
            foreach (IArchive archive in Archives)
            {
                Log.Info("Loading ", archive.Uri);

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
        }
    }
}

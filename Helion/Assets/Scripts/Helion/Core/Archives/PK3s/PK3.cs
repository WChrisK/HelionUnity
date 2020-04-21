using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Helion.Core.Resource;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using static Helion.Core.Util.OptionalHelper;

namespace Helion.Core.Archives.PK3s
{
    /// <summary>
    /// A PK3 archive.
    /// </summary>
    public class PK3 : IArchive
    {
        public string Uri { get; }
        private readonly List<PK3Entry> entries;
        private readonly Dictionary<UpperString, PK3Entry> pathToEntry = new Dictionary<UpperString, PK3Entry>();
        private readonly Dictionary<UpperString, List<PK3Entry>> nameToEntry = new Dictionary<UpperString, List<PK3Entry>>();
        private readonly Dictionary<UpperString, List<PK3Entry>> topLevelFolderEntries = new Dictionary<UpperString, List<PK3Entry>>();

        private PK3(string uri, List<PK3Entry> pk3Entries)
        {
            Uri = uri;
            entries = pk3Entries;

            foreach (PK3Entry entry in pk3Entries)
            {
                pathToEntry[entry.Path.ToString()] = entry;
                AddToTopLevelFolderTracker(entry);
                AddToNameTracker(entry);
            }
        }

        /// <summary>
        /// Tries to read a PK3 (or zip file) from the path provided.
        /// </summary>
        /// <param name="path">The path to read from.</param>
        /// <returns>The read PK3, or an empty value if it cannot be read.
        /// </returns>
        public static Optional<PK3> From(string path)
        {
            FileStream fileStream = null;
            ZipArchive zip = null;

            try
            {
                fileStream = File.Open(path, FileMode.Open);
                zip = new ZipArchive(fileStream);

                // Note: Despite returning an IEnumerable, we need to convert
                // it to some actual concrete final implementation instead of
                // returning the enumerable, otherwise it's wrapped around a
                // stream that is closed and will cause exceptions to appear.
                List<PK3Entry> entries = zip.Entries
                    .Where(IsNotDirectory)
                    .Select(ReadZipEntry)
                    .ToList();

                return new PK3(path, entries);
            }
            catch
            {
                return Empty;
            }
            finally
            {
                zip?.Dispose();
                fileStream?.Dispose();
            }
        }
        /// <summary>
        /// A helper method that reads a PK3 file but returns the interface
        /// type.
        /// </summary>
        /// <param name="path">The path to read the PK3 from.</param>
        /// <returns>The PK3 file, or an empty optional if it cannot be read
        /// from.</returns>
        public static Optional<IArchive> FromArchive(string path)
        {
            return From(path).Map(val => (IArchive)val);
        }

        /// <summary>
        /// Gets all of the entries that are under the top level folder name
        /// provided.
        /// </summary>
        /// <param name="folderName">The name of the folder.</param>
        /// <returns>An enumerator that yields entries for the folder name
        /// provided.</returns>
        public IEnumerable<IEntry> TopLevelFolderEntries(UpperString folderName)
        {
            if (topLevelFolderEntries.TryGetValue(folderName, out List<PK3Entry> folderEntries))
                return folderEntries;
            return new List<IEntry>();
        }

        public Optional<IEntry> Find(UpperString name)
        {
            if (nameToEntry.TryGetValue(name, out List<PK3Entry> existingEntries))
                return existingEntries.FirstOrDefault();
            return Empty;
        }

        public Optional<IEntry> Find(UpperString name, ResourceNamespace type)
        {
            if (nameToEntry.TryGetValue(name, out List<PK3Entry> existingEntries))
                foreach (PK3Entry entry in existingEntries.Reversed())
                    if (entry.Namespace == type)
                        return entry;

            return Empty;
        }

        public Optional<IEntry> FindPath(UpperString path)
        {
            if (pathToEntry.TryGetValue(path, out PK3Entry entry))
                return entry;
            return Empty;
        }

        public IEnumerable<IEntry> FindAll(UpperString name)
        {
            if (nameToEntry.TryGetValue(name, out List<PK3Entry> existingEntries))
                return existingEntries;
            return new List<IEntry>();
        }

        public IArchiveMapIterator GetMaps() => new PK3ArchiveMapIterator(this);

        public IEnumerator<IEntry> GetEnumerator() => entries.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private static bool IsNotDirectory(ZipArchiveEntry entry)
        {
            return !(entry.FullName.EndsWith("\\") || entry.FullName.EndsWith("/"));
        }

        private static PK3Entry ReadZipEntry(ZipArchiveEntry zipEntry)
        {
            string pathText = ProcessZipPath(zipEntry);
            EntryPath path = new EntryPath(pathText);
            ResourceNamespace resourceNamespace = ResourceNamespaceHelper.From(path);
            byte[] data = DecompressZipEntry(zipEntry);

            return new PK3Entry(path, resourceNamespace, data);
        }

        private static string ProcessZipPath(ZipArchiveEntry zipEntry)
        {
            string fileName = zipEntry.Name;
            string dirPath = zipEntry.FullName.Substring(0, zipEntry.FullName.Length - fileName.Length);
            return dirPath.Replace('\\', '/') + fileName;
        }

        private static byte[] DecompressZipEntry(ZipArchiveEntry zipEntry)
        {
            using (Stream stream = zipEntry.Open())
            {
                byte[] data = new byte[zipEntry.Length];
                stream.Read(data, 0, (int)zipEntry.Length);
                return data;
            }
        }

        private void AddToTopLevelFolderTracker(PK3Entry entry)
        {
            if (entry.Path.Folders.Count >= 1)
            {
                UpperString topLevelFolder = entry.Path.Folders[0];

                if (topLevelFolderEntries.TryGetValue(topLevelFolder, out List<PK3Entry> existingFolderList))
                    existingFolderList.Add(entry);
                else
                    topLevelFolderEntries[topLevelFolder] = new List<PK3Entry> { entry };
            }
        }

        private void AddToNameTracker(PK3Entry entry)
        {
            if (nameToEntry.TryGetValue(entry.Name, out List<PK3Entry> existingEntries))
                existingEntries.Add(entry);
            else
                nameToEntry[entry.Name] = new List<PK3Entry> { entry };
        }
    }
}

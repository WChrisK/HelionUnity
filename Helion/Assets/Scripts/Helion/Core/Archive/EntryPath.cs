using System.Collections.Generic;
using System.IO;
using System.Linq;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Archive
{
    /// <summary>
    /// A path for an entry.
    /// </summary>
    public class EntryPath
    {
        /// <summary>
        /// The separator for folders in string format.
        /// </summary>
        public const string Separator = "/";

        /// <summary>
        /// The separator for folders in character format.
        /// </summary>
        public const char SeparatorChar = '/';

        /// <summary>
        /// The case sensitive name of the path. This can be empty if this is a
        /// path for a directory.
        /// </summary>
        public readonly string Name;

        /// <summary>
        /// The extension, if any.
        /// </summary>
        public readonly string Extension;

        /// <summary>
        /// All of the folders in the path.
        /// </summary>
        /// <remarks>
        /// If this is a directory entry, then it will have an empty name and
        /// extension, and a non-empty list.
        /// </remarks>
        public readonly IReadOnlyList<string> Folders;

        /// <summary>
        /// A convenience property for getting the name and path. If it is just
        /// the name, it will return the name without the period. Otherwise it
        /// will be in the form of [name].[ext].
        /// </summary>
        public string NameAndExtension => Extension.NotEmpty() ? $"{Name}.{Extension}" : Name;

        /// <summary>
        /// Gets the last folder name. If no folders exist, returns an empty
        /// string.
        /// </summary>
        public string LastFolder => Folders.LastOrDefault() ?? "";

        /// <summary>
        /// Creates an entry path from the path provided.
        /// </summary>
        /// <param name="path">The path for this entry.</param>
        public EntryPath(string path = "")
        {
            Folders = ExtractFoldersFrom(path);
            Name = Path.GetFileNameWithoutExtension(path);
            Extension = Path.GetExtension(path) ?? "";
            if (Extension.Length > 1)
                Extension = Extension.Substring(1);
        }

        /// <summary>
        /// Creates an entry path from the path provided.
        /// </summary>
        /// <param name="path">The path for this entry.</param>
        public EntryPath(UpperString path) : this(path.ToString())
        {
        }

        /// <summary>
        /// Takes the folders of the current directory, uses the name and
        /// extension from the provided string, and returns a new instance.
        /// </summary>
        /// <remarks>
        /// The extension, nor the name, are required for this to return a
        /// valid entry path object. Calling this with an empty string would
        /// result with a similar call to <see cref="FromFolders"/>.
        /// </remarks>
        /// <example>
        /// If we had "/hi/a/yes.txt" and called this with "img.png", then
        /// the new object returned would be "/hi/a/img.png".
        /// </example>
        /// <param name="nameAndExtension">The new name and extension to make a
        /// new entry path from, except with this argument as the name and
        /// extension.</param>
        /// <returns>A new entry path with the name and extension changed.
        /// </returns>
        public EntryPath WithNameAndExtension(string nameAndExtension)
        {
            string path = string.Join(Separator, Folders.ToList()) + Separator + nameAndExtension;
            return new EntryPath(path);
        }

        /// <summary>
        /// Takes the current path and extends it. This means it takes the
        /// folder path and creates a new entry path but with the new folder
        /// name at the end.
        /// </summary>
        /// <remarks>
        /// Suppose you had a path that looked like "my/folder/file.txt". When
        /// doing Extend("hi"), the new result is "my/folder/hi/file.txt". This
        /// is useful when traversing a tree of directories without wanting to
        /// bring the full path of the entries with you.
        /// </remarks>
        /// <param name="folderName">The name to extend this with.</param>
        /// <returns>The new entry path with the folder extended.</returns>
        public EntryPath Extend(string folderName)
        {
            string folders = string.Join(Separator, Folders.ToList().Append(folderName)) + Separator;
            return new EntryPath(folders + NameAndExtension);
        }

        /// <summary>
        /// Creates a new entry path from only the folders.
        /// </summary>
        /// <returns>A new entry path with only the folders.</returns>
        public EntryPath FromFolders()
        {
            string folders = string.Join(Separator, Folders.ToList());
            return new EntryPath(folders + Separator);
        }

        /// <summary>
        /// Takes the name (and extension) and returns it as a path with a
        /// folder that has the extension name. The name and extension are
        /// dropped.
        /// </summary>
        /// <example>
        /// The path "/hi/yes.txt" becomes "/hi/yes.txt".
        /// </example>
        /// <returns>A new path with the name and extension as a folder.
        /// </returns>
        public EntryPath NameExtensionAsFolder()
        {
            string folders = string.Join(Separator, Folders.ToList().Append(NameAndExtension)) + Separator;
            return new EntryPath(folders);
        }

        /// <summary>
        /// Similar to <see cref="NameExtensionAsFolder"/> but uses the name
        /// and extension from the path provided.
        /// </summary>
        /// <param name="entryPath">The path's name/extension to use.</param>
        /// <returns>A new path with the folders from the current entry path
        /// and the name/extension from the provided entry path.</returns>
        public EntryPath WithNameAndExtensionOf(EntryPath entryPath)
        {
            return WithNameAndExtension(entryPath.NameAndExtension);
        }

        public override string ToString()
        {
            if (Folders.Empty())
                return NameAndExtension;
            return string.Join(Separator, Folders) + Separator + NameAndExtension;
        }

        private static List<string> ExtractFoldersFrom(string path)
        {
            if (path.EndsWith(Separator))
                return path.Split(SeparatorChar).Where(s => s.NotEmpty()).ToList();

            List<string> folders = path.Split(SeparatorChar).Where(s => s.NotEmpty()).ToList();;
            folders.RemoveAt(folders.Count - 1);
            return folders;
        }
    }
}

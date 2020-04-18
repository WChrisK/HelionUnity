using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Helion.Core.Util.Extensions;
using MoreLinq;
using UnityEngine;

namespace Helion.Core.Util
{
    /// <summary>
    /// Parses the command line arguments.
    /// </summary>
    public class CommandLineArgs : IEnumerable<string>
    {
        private const string CommandLineArgsDevPath = ".commandlineargs";

        public readonly List<string> Files = new List<string>();
        public string BaseDirectory { get; private set; } = "";

        private readonly List<string> args = new List<string>();

        /// <summary>
        /// A helper function to get all the files with their full path in
        /// array format.
        /// </summary>
        public string[] FullFilePaths => Files.Select(f => $"{BaseDirectory}{f}").ToArray();

        /// <summary>
        /// When constructed, carries out parsing of the command line arguments
        /// for the game.
        /// </summary>
        public CommandLineArgs()
        {
            if (File.Exists(CommandLineArgsDevPath))
                ReadDeveloperCommandLineArgsFile();
            else
                ReadEnvironmentCommandLineArgs();

            args = args.Where(arg => arg.NotEmpty()).ToList();
            ProcessArgs();
        }

        public IEnumerator<string> GetEnumerator() => args.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Tries to read the custom command line arguments file that we have
        /// to do to hackily work around Unity's lack of command line arguments
        /// when in the editor mode.
        /// </summary>
        /// <returns>True if it read the file, false if the file does not exist
        /// or if the file cannot be read.</returns>
        private void ReadDeveloperCommandLineArgsFile()
        {
            try
            {
                string text = File.ReadAllText(CommandLineArgsDevPath);
                string[] tokens = text.Replace("\r", "").Split('\n');
                args.AddRange(tokens);
            }
            catch
            {
                Debug.Log("Error reading developer command line args (bad file permissions?)");
            }
        }

        private void ReadEnvironmentCommandLineArgs()
        {
            // Skip the first argument since it's the executable.
            System.Environment.GetCommandLineArgs().Skip(1).ForEach(args.Add);
        }

        private void ProcessArgs()
        {
            for (int i = 0; i < args.Count; i++)
            {
                string arg = args[i];
                if (!(arg.StartsWith("-") && arg.Length == 2) && !arg.StartsWith("+"))
                    continue;

                switch (char.ToUpper(arg[1]))
                {
                case 'B':
                    ConsumeBasePath(ref i);
                    break;
                case 'F':
                    ConsumeFiles(ref i);
                    break;
                }
            }
        }

        private void ConsumeBasePath(ref int i)
        {
            if (i + 1 >= args.Count)
                return;

            BaseDirectory = args[i + 1];
            i++;

            if (!(BaseDirectory.EndsWith("/") || BaseDirectory.EndsWith("\\") || BaseDirectory.EndsWith(Path.DirectorySeparatorChar.ToString())))
                BaseDirectory += Path.DirectorySeparatorChar;
        }

        private void ConsumeFiles(ref int i)
        {
            while (i + 1 < args.Count)
            {
                string filePath = args[i + 1];
                if (filePath.StartsWith("-"))
                    break;

                Files.Add(filePath);
                i++;
            }
        }
    }
}

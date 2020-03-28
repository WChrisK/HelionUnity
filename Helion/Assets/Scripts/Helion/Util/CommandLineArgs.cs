using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MoreLinq;
using UnityEngine;

namespace Helion.Util
{
    /// <summary>
    /// Parses the command line arguments.
    /// </summary>
    public class CommandLineArgs : IEnumerable<string>
    {
        private const string CommandLineArgsDevPath = ".commandlineargs";

        private readonly List<string> args = new List<string>();

        /// <summary>
        /// When constructed, carries out parsing of the command line arguments
        /// for the game.
        /// </summary>
        public CommandLineArgs()
        {
            if (ReadDeveloperCommandLineArgsFile())
                return;

            ReadEnvironmentCommandLineArgs();
        }

        /// <summary>
        /// Tries to read the custom command line arguments file that we have
        /// to do to hackily work around Unity's lack of command line arguments
        /// when in the editor mode.
        /// </summary>
        /// <returns>True if it read the file, false if the file does not exist
        /// or if the file cannot be read.</returns>
        private bool ReadDeveloperCommandLineArgsFile()
        {
            if (!File.Exists(CommandLineArgsDevPath))
                return false;

            try
            {
                string text = File.ReadAllText(CommandLineArgsDevPath);
                string[] tokens = text.Replace("\r", "").Split('\n');
                args.AddRange(tokens);
                return true;
            }
            catch
            {
                Debug.Log("Error reading developer command line args (bad file permissions?)");
                return false;
            }
        }

        private void ReadEnvironmentCommandLineArgs()
        {
            // Skip the first argument since it's the executable.
            System.Environment.GetCommandLineArgs().Skip(1).ForEach(arg => args.Add(arg));
        }

        public IEnumerator<string> GetEnumerator() => args.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

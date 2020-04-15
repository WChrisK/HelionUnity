using System.IO;
using System.Text;

namespace Helion.Core.Util.Logging.Targets
{
    /// <summary>
    /// Writes logs to a file.
    /// </summary>
    public class FileTarget : ILogTarget
    {
        private const int BufferSize = 64 * 1024;

        private readonly StreamWriter writer;

        /// <summary>
        /// Creates a new target to write to. This file location should be
        /// writeable.
        /// </summary>
        /// <param name="path">The path of the file to write to.</param>
        public FileTarget(string path)
        {
            writer = new StreamWriter(path, true, Encoding.UTF8, BufferSize);
        }

        public void Log(string message)
        {
            writer.WriteLine(message);
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }
}

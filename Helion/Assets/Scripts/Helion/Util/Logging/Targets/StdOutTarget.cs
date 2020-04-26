using System;

namespace Helion.Util.Logging.Targets
{
    /// <summary>
    /// Logs to stdout.
    /// </summary>
    public class StdOutTarget : ILogTarget
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
        }

        public void Dispose()
        {
            // Nothing to dispose of.
        }
    }
}

using UnityEngine;

namespace Helion.Core.Util.Logging.Targets
{
    /// <summary>
    /// Writes to the unity debug console.
    /// </summary>
    public class UnityDebugConsoleTarget : ILogTarget
    {
        public void Log(string message)
        {
            Debug.Log(message);
        }

        public void Dispose()
        {
            // Nothing to dispose of.
        }
    }
}

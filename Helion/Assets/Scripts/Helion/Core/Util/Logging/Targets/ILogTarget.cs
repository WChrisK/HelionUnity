using System;

namespace Helion.Core.Util.Logging.Targets
{
    /// <summary>
    /// A destination for a log message.
    /// </summary>
    public interface ILogTarget : IDisposable
    {
        /// <summary>
        /// Logs the message.
        /// </summary>
        /// <param name="message">The message to logs.</param>
        void Log(string message);
    }
}

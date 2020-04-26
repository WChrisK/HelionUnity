using System.Collections.Generic;
using Helion.Util.Logging.Targets;

namespace Helion.Util.Logging
{
    /// <summary>
    /// The manager for all the logs.
    /// </summary>
    public static class LogManager
    {
        /// <summary>
        /// The log level to filter messages with.
        /// </summary>
        public static LogLevel LogLevel = LogLevel.Info;

        private static readonly List<ILogTarget> targets = new List<ILogTarget>();

        internal static void Log(LogLevel level, string message)
        {
            if (level <= LogLevel)
                targets.ForEach(target => target.Log(message));
        }

        /// <summary>
        /// Registers a new sink to write processed logs to.
        /// </summary>
        /// <param name="target">The target to receive log messages.</param>
        public static void Register(ILogTarget target)
        {
            targets.Add(target);
        }

        /// <summary>
        /// Creates a new logging instance.
        /// </summary>
        /// <returns>A new logging instance for that class.</returns>
        public static Log Instance()
        {
            return new Log();
        }

        /// <summary>
        /// Disposes all the logging capabilities. Logging from this point on
        /// will not be processed unless new targets are registered. It is
        /// safe to log, but it ideally should not be done beyond this call.
        /// </summary>
        public static void Dispose()
        {
            targets.ForEach(target => target.Dispose());
            targets.Clear();
        }
    }
}

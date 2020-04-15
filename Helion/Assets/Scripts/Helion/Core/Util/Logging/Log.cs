using System.Text;
using MoreLinq;

namespace Helion.Core.Util.Logging
{
    /// <summary>
    /// A class that performs logging.
    /// </summary>
    public class Log
    {
        private readonly StringBuilder stringBuilder = new StringBuilder();

        internal Log()
        {
        }

        private string ToMessage(params object[] elements)
        {
            elements.ForEach(e => stringBuilder.Append(e));

            string message = stringBuilder.ToString();
            stringBuilder.Clear();

            return message;
        }

        /// <summary>
        /// Writes a message to the trace channel.
        /// </summary>
        /// <param name="elements"></param>
        public void Trace(params object[] elements)
        {
            LogManager.Log(LogLevel.Trace, ToMessage(elements));
        }

        /// <summary>
        /// Writes a message to the debug channel.
        /// </summary>
        /// <param name="elements"></param>
        public void Debug(params object[] elements)
        {
            LogManager.Log(LogLevel.Debug, ToMessage(elements));
        }

        public void Info(params object[] elements)
        {
            LogManager.Log(LogLevel.Info, ToMessage(elements));
        }

        public void Warn(params object[] elements)
        {
            stringBuilder.Append("WARN: ");
            LogManager.Log(LogLevel.Warn, ToMessage(elements));
        }

        public void Error(params object[] elements)
        {
            stringBuilder.Append("ERROR: ");
            LogManager.Log(LogLevel.Error, ToMessage(elements));
        }
    }
}

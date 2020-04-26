namespace Helion.Util.Logging.Targets
{
    /// <summary>
    /// A target for the GUI console plugin.
    /// </summary>
    public class ConsoleGUITarget : ILogTarget
    {
        public void Log(string message)
        {
            ConsoleLog.Instance.Log(message);
        }

        public void Dispose()
        {
            // Not up to us to control this.
        }
    }
}

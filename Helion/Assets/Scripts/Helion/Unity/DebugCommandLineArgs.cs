using System.Collections.Generic;
using UnityEngine;

namespace Helion.Unity
{
    /// <summary>
    /// A helper class that allows us to set command line args inside of the
    /// editor.
    /// </summary>
    public class DebugCommandLineArgs : MonoBehaviour
    {
        [Tooltip("If checked, will record a demo. This will not be performed if a demo is loaded.")]
        public bool RecordDemo;

        [Tooltip("The folder to treat as the base to search for resources (configs, wads, etc).")]
        public string BaseFolder = "";

        [Tooltip("Files to load up on map startup. An empty value is ignored, we provided excess for you to use.")]
        public List<string> Files = new List<string> { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        [Tooltip("Extra commands to run (ex: 'map MAP01', or 'set x \"say Some funny bind here!\"')")]
        public List<string> Commands = new List<string> { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };

        [Tooltip("If checked, will append to the log file instead of overwriting it.")]
        public bool AppendToLogFile;

        [Tooltip("The file to write logs to. If blank, will not write any logs.")]
        public string LogFile = "";

        [Tooltip("The map to go to on start (ex: MAP01). If blank, you will go to the main menu.")]
        public string StartMap = "";

        [Tooltip("A path to a demo to be run. If blank, will not run any demo.")]
        public string DemoPath = "";

        [Tooltip("If checked, will run the world as a connectable server. This is ignored if a demo is running.")]
        public bool HostServer;

        [Tooltip("The port to host on (only applicable if hosting is checked).")]
        public int Port = 21750;

        /// <summary>
        /// If set, will indicate that the values here should be used instead
        /// of any parsed command line arguments.
        /// </summary>
#if UNITY_EDITOR
        public bool Use => true;
#else
        public bool Use => false;
#endif
    }
}

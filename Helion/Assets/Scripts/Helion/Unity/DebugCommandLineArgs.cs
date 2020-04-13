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
#if UNITY_EDITOR
        public bool Use = true;
#else
        public bool Use = false;
#endif
        public string BaseFolder = "D:/Helion";
        public string StartMap = "";
        public List<string> Files = new List<string> { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
        public List<string> Commands = new List<string> { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "" };
    }
}

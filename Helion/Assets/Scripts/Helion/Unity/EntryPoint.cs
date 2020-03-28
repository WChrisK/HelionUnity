using Helion.Core.Util;
using UnityEngine;

namespace Helion.Unity
{
    public class EntryPoint : MonoBehaviour
    {
        public static readonly CommandLineArgs CommandLineArgs = new CommandLineArgs();

        /// <summary>
        /// Called before anything in the game loads, which can be used to
        /// initialize all the managers and data before anything starts. If
        /// something requires the engine to be fully loaded before doing a
        /// task, you can call it in the <see cref="Start"/> method.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeGame()
        {
            // Wad.From(CommandLineArgs.First());
        }

        private void Start()
        {
        }

        private void Update()
        {
        }

        private void FixedUpdate()
        {
        }

        private void OnGUI()
        {
            // GUI.Label(new Rect(200, 200, 100, 15), "Hello!");
            //
            // if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
            //     print("You clicked the button!");
        }
    }
}

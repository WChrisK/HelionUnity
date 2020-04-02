using Helion.Core.Resource;
using Helion.Core.Resource.Maps;
using Helion.Core.Util;
using UnityEngine;

namespace Helion.Unity
{
    /// <summary>
    /// The single object in the world that is used to act as a game controller
    /// and do required tasks like reading the archive data, spawn everything
    /// into the levels, and manage everything.
    /// </summary>
    public class EntryPoint : MonoBehaviour
    {
        /// <summary>
        /// The runtime command line arguments.
        /// </summary>
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
            if (!GameData.Load(CommandLineArgs))
            {
                Debug.Log("Error loading archive data, aborting!");
                Application.Quit(1);
            }
        }

        private void Start()
        {
            // --------------------------------------------
            // The following is all temporary testing code.
            // --------------------------------------------
            Optional<IMap> map = GameData.FindMap("MAP01");
            if (!map)
            {
                Debug.Log("Error loading MAP01");
                return;
            }

            Debug.Log("Loaded MAP01");

            GameObject quad = GameObject.FindWithTag("Quad");
            if (quad)
            {
                Material brnsmal1 = GameData.Resources.TextureManager.Materials["BRNSMAL1"];
                quad.GetComponent<Renderer>().material = brnsmal1;
            }
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

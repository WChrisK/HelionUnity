using Helion.Core.Resource;
using Helion.Core.Resource.Maps;
using Helion.Core.Util;
using Helion.Core.Worlds;
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

        private float cameraPitch;
        private float cameraYaw;
        private float yawSensitivity = 2.5f;
        private float pitchSensitivity = 1.5f;
        private string fpsText;
        private float deltaTime;
        private World world;

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

        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // --------------------------------------------
            // The following is all temporary testing code.
            // --------------------------------------------
            Optional<IMap> map = GameData.FindMap("MAP01");
            if (!map)
            {
                Debug.Log("Error loading MAP01");
                return;
            }

            Optional<World> worldOpt = World.From(map.Value);
            if (!worldOpt)
            {
                Debug.Log("Could not load world from MAP01");
                return;
            }

            world = worldOpt.Value;

            GameObject player = GameObject.Find("Player");
            player.transform.position = new Vector3(-96, 100, 784) * Constants.MapUnit;
        }

        void Update()
        {
            UpdateCamera();
            UpdateFPS();
        }

        void FixedUpdate()
        {
            UpdatePlayerMovement();
        }

        void OnGUI()
        {
            GUI.Label(new Rect(4, 4, 100, 25), $"FPS: {fpsText}");

            // if (GUI.Button(new Rect(10, 10, 150, 100), "I am a button"))
            //     print("You clicked the button!");
        }

        // TODO: This is temporary code

        private void UpdateCamera()
        {
            Transform cameraTransform = Camera.main.transform;

            // TODO: Multiply by Time.deltaTime?
            cameraYaw += Input.GetAxisRaw("Mouse X") * yawSensitivity;
            cameraPitch += Input.GetAxisRaw("Mouse Y") * pitchSensitivity;

            cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);
            while (cameraYaw < 0)
                cameraYaw += 360;
            while (cameraYaw >= 360)
                cameraYaw -= 360;

            cameraTransform.eulerAngles = new Vector3(-cameraPitch, cameraYaw, 0f);
        }

        private void UpdatePlayerMovement()
        {
            const int MOVE_FACTOR = 6;

            GameObject player = GameObject.Find("Player");
            CharacterController controller = player.GetComponent<CharacterController>();

            if (Input.GetKey(KeyCode.W))
                controller.Move(new Vector3(0, 0, MOVE_FACTOR) * Constants.MapUnit);
            if (Input.GetKey(KeyCode.A))
                controller.Move(new Vector3(-MOVE_FACTOR, 0, 0) * Constants.MapUnit);
            if (Input.GetKey(KeyCode.S))
                controller.Move(new Vector3(0, 0, -MOVE_FACTOR) * Constants.MapUnit);
            if (Input.GetKey(KeyCode.D))
                controller.Move(new Vector3(MOVE_FACTOR, 0, 0) * Constants.MapUnit);
            if (Input.GetKey(KeyCode.Space))
                controller.Move(new Vector3(0, MOVE_FACTOR, 0) * Constants.MapUnit);
            if (Input.GetKey(KeyCode.C))
                controller.Move(new Vector3(0, -MOVE_FACTOR, 0) * Constants.MapUnit);
        }

        private void UpdateFPS()
        {
            deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;
            fpsText = Mathf.Ceil(fps).ToString();
        }
    }
}

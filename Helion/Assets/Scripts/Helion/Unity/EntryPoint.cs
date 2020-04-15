using System.Linq;
using Helion.Core.Resource;
using Helion.Core.Resource.Maps;
using Helion.Core.Util;
using Helion.Core.Util.Logging;
using Helion.Core.Util.Logging.Targets;
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
        private static readonly Log Log = LogManager.Instance();

        private float cameraPitch;
        private float cameraYaw;
        private float yawSensitivity = 2.5f;
        private float pitchSensitivity = 1.5f;
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
            Application.targetFrameRate = int.MaxValue;
            QualitySettings.vSyncCount = 0;
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Start()
        {
            LogManager.Register(new UnityDebugConsoleTarget());
            LogManager.Register(new ConsoleGUITarget());

            Log.Info("Loaded ", Constants.ApplicationName, " v", Constants.ApplicationName);

            if (!Data.Load(CommandLineArgs.ToArray()))
            {
                Log.Error("Failure loading archive data, aborting!");
                Application.Quit(1);
            }

            // --------------------------------------------
            // The following is all temporary testing code.
            // --------------------------------------------
            Optional<IMap> map = Data.FindMap("MAP01");
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
        }

        void FixedUpdate()
        {
            UpdatePlayerMovement();
        }

        void OnApplicationQuit()
        {
            LogManager.Dispose();
        }

        private void UpdateCamera()
        {
            Transform cameraTransform = Camera.main.transform;

            // TODO: Multiply by Time.deltaTime?
            cameraYaw += Input.GetAxisRaw("Mouse X") * yawSensitivity;
            cameraPitch += Input.GetAxisRaw("Mouse Y") * pitchSensitivity;

            cameraPitch = Mathf.Clamp(cameraPitch, -89.9f, 89.9f);
            while (cameraYaw < 0)
                cameraYaw += 360;
            while (cameraYaw >= 360)
                cameraYaw -= 360;

            cameraTransform.eulerAngles = new Vector3(-cameraPitch, cameraYaw, 0f);
        }

        private void UpdatePlayerMovement()
        {
            const float MOVE_FACTOR = 12 * Constants.MapUnit;

            Vector3 direction = new Vector3(Mathf.Sin(Mathf.Deg2Rad * cameraYaw), 0, Mathf.Cos(Mathf.Deg2Rad * cameraYaw));
            Vector3 rightDirection = new Vector3(direction.z, 0, -direction.x);

            GameObject player = GameObject.Find("Player");
            CharacterController controller = player.GetComponent<CharacterController>();

            if (Input.GetKey(KeyCode.W))
                controller.Move(direction * MOVE_FACTOR);
            if (Input.GetKey(KeyCode.A))
                controller.Move(rightDirection * -MOVE_FACTOR);
            if (Input.GetKey(KeyCode.S))
                controller.Move(direction * -MOVE_FACTOR);
            if (Input.GetKey(KeyCode.D))
                controller.Move(rightDirection * MOVE_FACTOR);
            if (Input.GetKey(KeyCode.Space))
                controller.Move(Vector3.up * MOVE_FACTOR);
            if (Input.GetKey(KeyCode.C))
                controller.Move(Vector3.down * MOVE_FACTOR);
        }
    }
}

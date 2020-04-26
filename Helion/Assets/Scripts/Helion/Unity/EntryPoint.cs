using Helion.Configs;
using Helion.Configs.Fields;
using Helion.Resource;
using Helion.Resource.Maps;
using Helion.Util;
using Helion.Util.Logging;
using Helion.Util.Logging.Targets;
using Helion.Util.Unity;
using Helion.Worlds;
using Helion.Worlds.Entities.Players;
using Helion.Worlds.Info;
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

        private World world;
        private Player player;

        /// <summary>
        /// Called before anything in the game loads, which can be used to
        /// initialize all the managers and data before anything starts. If
        /// something requires the engine to be fully loaded before doing a
        /// task, you can call it in the <see cref="Start"/> method.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeGame()
        {
            // We want to set this first before anything so we have access to
            // destruction abilities before we touch anything.
            GameObjectHelper.destroyFunc = Destroy;

            // Apparently Unity can override our settings...
            Application.targetFrameRate = int.MaxValue;
            QualitySettings.vSyncCount = 0;
            Cursor.lockState = CursorLockMode.Locked;

#if UNITY_EDITOR
            LogManager.Register(new UnityDebugConsoleTarget());
#endif
            LogManager.Register(new ConsoleGUITarget());

            Log.Info("Running ", Constants.ApplicationName, " v", Constants.ApplicationVersion);

            LoadAndRegisterConfig();
        }

        void Start()
        {
            RegisterConsoleCommands();

            if (!Data.Load(CommandLineArgs.Files))
            {
                Log.Error("Failure loading archive data, aborting!");
                Application.Quit(1);
            }

            ConsoleCommandsRepository.Instance.ExecuteCommand("map", new[] { "map01" });
        }

        void Update()
        {
            player?.ApplyPlayerCameraInput();
            world?.Update();
        }

        void FixedUpdate()
        {
            world?.Tick();
        }

        void OnApplicationQuit()
        {
            // TODO: This is buggy and saves an empty one, no idea why yet...
            //Data.Config.Save($"{CommandLineArgs.BaseDirectory}{Config.DefaultConfigName}")
            LogManager.Dispose();
        }

        private static void LoadAndRegisterConfig()
        {
            LoadConfig($"{CommandLineArgs.BaseDirectory}{Config.DefaultConfigName}");

            var console = ConsoleCommandsRepository.Instance;
            foreach (IConfigField configField in Data.Config.GetConfigFields())
            {
                console.RegisterCommand(configField.FullName, args =>
                {
                    if (args.Length == 0)
                    {
                        if (configField is StringConfigField)
                            return $"{configField.FullName} is \"{configField.TextValue}\"";
                        return $"{configField.FullName} is {configField.TextValue}";
                    }

                    bool setSuccess = configField.SetValue(args[0]);
                    if (!setSuccess)
                        return "Cannot set value, invalid type (ex: if it's a boolean, use 'true' or 'false')";

                    return $"{configField.FullName} set to '{args[0]}'";
                });
            }
        }

        /// <summary>
        /// Loads a config from either the path provided, or the default path.
        /// </summary>
        /// <param name="path">The path to use, or null if the default path
        /// should be used.</param>
        private static void LoadConfig(string path = null)
        {
            path = path ?? Config.DefaultConfigName;

            Optional<Config> config = Config.FromFile(path);
            if (!config)
            {
                Log.Error("Failed to load config from: ", path);
                Log.Info("Creating empty config, will save on exit");
                return;
            }

            Log.Info("Loaded config from ", path);
            Data.Config = config.Value;
        }

        private void RegisterConsoleCommands()
        {
            var console = ConsoleCommandsRepository.Instance;

            console.RegisterCommand("exit", args =>
            {
                Application.Quit(0);
                return "Exiting application";
            });

            console.RegisterCommand("map", args =>
            {
                if (args.Length == 0)
                    return "Usage: map <NAME>";

                string mapName = args[0];
                if (!Data.TryFindMap(mapName, out MapData map))
                    return $"Cannot find {mapName}";

                WorldInfo info = new WorldInfo(mapName, 0)
                {
                    Skill = Skill.Hard,
                    Mode = GameMode.Cooperative
                };
                if (!World.TryCreateWorld(info, map, out World newWorld, out GameObject _))
                    return $"Unable to load corrupt world data for {mapName}";

                world?.Dispose();
                world = newWorld;
                player = world.Entities.SpawnPlayer(1);

                if (Camera.main != null)
                {
                    Camera.main.enabled = false;
                    GameObject cameraObject = GameObject.FindGameObjectWithTag("MainCamera");
                    if (cameraObject)
                    {
                        AudioListener audioListener = cameraObject.GetComponent<AudioListener>();
                        Destroy(audioListener);
                    }
                }
                player.Camera.enabled = true;
                player.GameObject.AddComponent<AudioListener>();

                return $"Loaded {mapName}";
            });
        }
    }
}

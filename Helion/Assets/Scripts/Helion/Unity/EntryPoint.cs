using Helion.Core.Configs;
using Helion.Core.Configs.Fields;
using Helion.Core.Resource;
using Helion.Core.Resource.Maps;
using Helion.Core.Util;
using Helion.Core.Util.Logging;
using Helion.Core.Util.Logging.Targets;
using Helion.Core.Util.Unity;
using Helion.Core.Worlds;
using Helion.Core.Worlds.Entities;
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
        private Entity player;

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

            if (!Data.Load(CommandLineArgs.FullFilePaths))
            {
                Log.Error("Failure loading archive data, aborting!");
                Application.Quit(1);
            }
        }

        void Update()
        {
            if (world == null || Camera.main == null)
                return;

            Camera.main.transform.position = player.InterpolatedPosition(world.GameTickFraction).MapUnit();
        }

        void FixedUpdate()
        {
            world?.Tick();
        }

        void OnApplicationQuit()
        {
            Data.Config.Save($"{CommandLineArgs.BaseDirectory}{Config.DefaultConfigName}");
            LogManager.Dispose();
        }

        private static void LoadAndRegisterConfig()
        {
            Data.LoadConfig($"{CommandLineArgs.BaseDirectory}{Config.DefaultConfigName}");

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
                Optional<IMap> map = Data.FindMap(mapName);
                if (!map)
                    return $"Cannot find {mapName}";

                Optional<World> worldOpt = World.From(map.Value);
                if (!worldOpt)
                    return $"Unable to load corrupt world data for {mapName}";

                world?.Dispose();

                world = worldOpt.Value;
                player = world.Entities.SpawnPlayer(1).Value;

                return $"Loaded {mapName}";
            });
        }
    }
}

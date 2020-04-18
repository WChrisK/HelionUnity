using System.Collections.Generic;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Resource.Decorate.Definitions.Flags;
using Helion.Core.Resource.Decorate.Definitions.Properties.Types;
using Helion.Core.Resource.Decorate.Definitions.States;
using Helion.Core.Resource.Decorate.Definitions.Types;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate
{
    /// <summary>
    /// A helper class for creating definitions.
    /// </summary>
    public static class DefaultDefinitionFactory
    {
        private static readonly ActorFlowControl stop = new ActorFlowControl(ActorStateBranch.Stop);
        private static readonly ActorFlowControl wait = new ActorFlowControl(ActorStateBranch.Wait);
        private static ActorFrameProperties MakeProperties() => new ActorFrameProperties();
        private static Optional<ActorActionFunction> MakeNoAction() => Optional<ActorActionFunction>.Empty();

        /// <summary>
        /// Creates a list of new default definitions that are critical to the
        /// functioning of the game.
        /// </summary>
        /// <returns>A list of newly created default definitions.</returns>
        public static List<ActorDefinition> CreateAllDefaultDefinitions()
        {
            ActorDefinition actorBase = CreateBaseDefinition();

            return new List<ActorDefinition>
            {
                actorBase,
                CreateBaseSpawnPoint(actorBase),
                CreatePlayerPawn(actorBase),
                CreateTeleportDestination(actorBase)
            };
        }

        private static ActorDefinition CreateBaseDefinition()
        {
            Optional<ActorActionFunction> genericFreezeDeath = new ActorActionFunction("A_GenericFreezeDeath");
            Optional<ActorActionFunction> freezeDeathChunks = new ActorActionFunction("A_FreezeDeathChunks");

            ActorDefinition actor = new ActorDefinition("ACTOR", Optional<ActorDefinition>.Empty());
            actor.States.Labels.Add("SPAWN", 0);
            actor.States.Frames.Add(new ActorFrame(0, "TNT1A", -1, MakeProperties(), MakeNoAction(), stop, 0));
            actor.States.Labels.Add("NULL", 1);
            actor.States.Frames.Add(new ActorFrame(1, "TNT1A", 1, MakeProperties(), MakeNoAction(), stop, 0));
            actor.States.Labels.Add("GENERICFREEZEDEATH", 2);
            actor.States.Frames.Add(new ActorFrame(2, "#####", 5, MakeProperties(), genericFreezeDeath, stop, 1));
            actor.States.Frames.Add(new ActorFrame(3, "----A", 1, MakeProperties(), freezeDeathChunks, wait, 0));
            actor.States.Labels.Add("GENERICCRUSH", 4);
            actor.States.Frames.Add(new ActorFrame(4, "POL5A", -1, MakeProperties(), MakeNoAction(), stop, 0));

            return actor;
        }

        private static ActorDefinition CreateBaseSpawnPoint(ActorDefinition actorBase)
        {
            ActorDefinition spawnpoint = new ActorDefinition("SPAWNPOINT", actorBase);
            spawnpoint.ActorType.Set(ActorType.SpawnPoint);
            spawnpoint.Flags.Set(ActorFlagType.DontSplash, true);
            spawnpoint.Flags.Set(ActorFlagType.Invisible, true);
            spawnpoint.Flags.Set(ActorFlagType.NoBlockmap, true);
            spawnpoint.Flags.Set(ActorFlagType.NoGravity, true);

            return spawnpoint;
        }

        private static ActorDefinition CreatePlayerPawn(ActorDefinition actorBase)
        {
            ActorDefinition playerPawn = new ActorDefinition("PLAYERPAWN", actorBase);
            playerPawn.ActorType.Set(ActorType.Player);
            playerPawn.Flags.Set(ActorFlagType.CanPass, true);
            playerPawn.Flags.Set(ActorFlagType.CanPushWalls, true);
            playerPawn.Flags.Set(ActorFlagType.Dropoff, true);
            playerPawn.Flags.Set(ActorFlagType.FloorClip, true);
            playerPawn.Flags.Set(ActorFlagType.Friendly, true);
            playerPawn.Flags.Set(ActorFlagType.NoBlockMonst, true);
            playerPawn.Flags.Set(ActorFlagType.NotDMatch, true);
            playerPawn.Flags.Set(ActorFlagType.Pickup, true);
            playerPawn.Flags.Set(ActorFlagType.Shootable, true);
            playerPawn.Flags.Set(ActorFlagType.SlidesOnWalls, true);
            playerPawn.Flags.Set(ActorFlagType.Solid, true);
            playerPawn.Flags.Set(ActorFlagType.Telestomp, true);
            playerPawn.Flags.Set(ActorFlagType.WindThrust, true);
            playerPawn.Properties.Health = 100;
            playerPawn.Properties.Height = 56;
            playerPawn.Properties.Mass = 100;
            playerPawn.Properties.PainChance.Value = 255;
            playerPawn.Properties.Radius = 16;
            playerPawn.Properties.Speed = 1;

            return playerPawn;
        }

        private static ActorDefinition CreateTeleportDestination(ActorDefinition actorBase)
        {
            ActorDefinition teleportDest = new ActorDefinition("TELEPORTDEST", actorBase, 14);
            teleportDest.ActorType.Set(ActorType.TeleportDestination);
            teleportDest.Flags.Set(ActorFlagType.DontSplash, true);
            teleportDest.Flags.Set(ActorFlagType.NoBlockmap, true);

            return teleportDest;
        }
    }
}

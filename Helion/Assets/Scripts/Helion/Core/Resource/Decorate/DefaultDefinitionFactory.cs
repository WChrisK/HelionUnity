using System.Collections.Generic;
using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Resource.Decorate.Definitions.Flags;
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

        public static List<ActorDefinition> CreateAllDefaultDefinitions()
        {
            ActorDefinition actorBase = CreateBaseDefinition();

            return new List<ActorDefinition>
            {
                actorBase,
                CreateBaseSpawnPoint(actorBase)
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
    }
}

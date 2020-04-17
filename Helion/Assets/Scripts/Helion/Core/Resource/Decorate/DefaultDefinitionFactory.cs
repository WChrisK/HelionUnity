using Helion.Core.Resource.Decorate.Definitions;
using Helion.Core.Resource.Decorate.Definitions.States;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate
{
    /// <summary>
    /// A helper class for creating definitions.
    /// </summary>
    public static class DefaultDefinitionFactory
    {
        /// <summary>
        /// Creates the default actor class that everything inherits from.
        /// </summary>
        /// <returns>A new base actor definition for all actors.</returns>
        public static ActorDefinition CreateDefinition()
        {
            ActorFrameProperties MakeProperties() => new ActorFrameProperties();
            Optional<ActorActionFunction> MakeNoAction() => Optional<ActorActionFunction>.Empty();

            Optional<ActorActionFunction> genericFreezeDeath = new ActorActionFunction("A_GenericFreezeDeath");
            Optional<ActorActionFunction> freezeDeathChunks = new ActorActionFunction("A_FreezeDeathChunks");
            ActorFlowControl stop = new ActorFlowControl(ActorStateBranch.Stop);
            ActorFlowControl wait = new ActorFlowControl(ActorStateBranch.Wait);

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
    }
}

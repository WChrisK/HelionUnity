using System.Collections.Generic;
using Helion.Core.Resource.Decorate.Definitions.States;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions
{
    /// <summary>
    /// All of the states for when an actor is ticked. Also contains rendering
    /// information for frames.
    /// </summary>
    public class ActorStates
    {
        public readonly ActorStateLabels Labels;
        public readonly List<ActorFrame> Frames;

        public ActorStates()
        {
            Labels = new ActorStateLabels();
            Frames = new List<ActorFrame>();
        }

        public ActorStates(ActorStates other, UpperString parentName)
        {
            other.Frames.ForEach(frame => Frames.Add(new ActorFrame(frame)));
            Labels = new ActorStateLabels(other.Labels, parentName);
        }
    }
}

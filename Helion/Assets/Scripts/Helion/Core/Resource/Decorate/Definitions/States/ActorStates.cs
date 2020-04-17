using System.Collections.Generic;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.States
{
    /// <summary>
    /// All of the states for when an actor is ticked. Also contains rendering
    /// information for frames.
    /// </summary>
    public class ActorStates
    {
        public readonly ActorStateLabels Labels = new ActorStateLabels();
        public readonly List<ActorFrame> Frames = new List<ActorFrame>();

        public ActorStates()
        {
        }

        public ActorStates(ActorStates other, UpperString parentName)
        {
            foreach (ActorFrame frame in other.Frames)
            {
                ActorFrame newFrame = new ActorFrame(frame);
                Frames.Add(newFrame);
            }

            Labels = new ActorStateLabels(other.Labels, parentName);
        }
    }
}

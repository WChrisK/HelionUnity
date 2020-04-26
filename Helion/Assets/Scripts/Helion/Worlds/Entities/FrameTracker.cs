using Helion.Resource.Decorate.Definitions.States;
using Helion.Util;
using Helion.Util.Logging;

namespace Helion.Worlds.Entities
{
    /// <summary>
    /// A tracker around the entity frame state. This is used to determine what
    /// the current actor's frame is, and all advancing over the entity frames
    /// is done through this object.
    /// </summary>
    public class FrameTracker : ITickable
    {
        private static readonly Log Log = LogManager.Instance();
        private static readonly UpperString SpawnLabel = "SPAWN";

        public ActorFrame Frame { get; private set; }
        private readonly Entity entity;
        private int offset;
        private int ticksInFrame;

        public FrameTracker(Entity entity)
        {
            this.entity = entity;

            SetupInitialOffset();
        }

        /// <summary>
        /// Jumps the actor to a label if it exists.
        /// </summary>
        /// <param name="label">The label to go to.</param>
        /// <returns>True if the jump succeeded, false otherwise.</returns>
        public bool GoToLabel(UpperString label)
        {
            int? labelIndex = entity.Definition.States.Labels[label];
            if (labelIndex == null)
                return false;

            GoToFrame(labelIndex.Value);
            return true;
        }

        public void Tick()
        {
            if (Frame.IsInfiniteStopFrame)
                return;

            if (ticksInFrame == 0 && Frame.ActionFunction)
                Frame.ActionFunction.Value.Execute();

            ticksInFrame++;
            if (ticksInFrame < Frame.Ticks)
                return;

            // TODO: Need to loop if the next frames are all zero.
            // TODO: Need to kill actor if it has an infinite loop.

            // TODO: Eventually we will drop the addition when it becomes an index.
            GoToFrame(offset + Frame.NextStateOffset);
        }

        private void SetupInitialOffset()
        {
            ActorStates states = entity.Definition.States;

            int? spawnOffset = states.Labels[SpawnLabel];
            if (spawnOffset == null)
                Log.Error("Unable to find spawn state for actor: ", entity.Definition.Name);
            else
                offset = spawnOffset.Value;

            Frame = states.Frames[offset];
        }

        private void GoToFrame(int index)
        {
            ticksInFrame = 0;
            offset = index;
            Frame = entity.Definition.States.Frames[offset];
        }
    }
}

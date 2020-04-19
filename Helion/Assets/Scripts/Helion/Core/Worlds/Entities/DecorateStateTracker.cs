using Helion.Core.Resource.Decorate.Definitions.States;
using Helion.Core.Util;
using Helion.Core.Util.Logging;

namespace Helion.Core.Worlds.Entities
{
    public class DecorateStateTracker : ITickable
    {
        private static readonly Log Log = LogManager.Instance();
        private static readonly UpperString SpawnLabel = "SPAWN";

        public ActorFrame Frame { get; private set; }
        private int offset;
        private readonly Entity entity;

        public DecorateStateTracker(Entity entity)
        {
            this.entity = entity;

            SetupInitialOffset();
        }

        public void Tick()
        {
            if (Frame.IsInfiniteStopFrame)
                return;

            // TODO: Eventually we will drop the addition when it becomes an index.
            offset += Frame.NextStateOffset;
            Frame = entity.Definition.States.Frames[offset];
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
    }
}

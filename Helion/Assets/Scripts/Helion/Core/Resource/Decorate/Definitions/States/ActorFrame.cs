using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.States
{
    public class ActorFrame
    {
        public readonly UpperString Sprite;
        public readonly char Frame;
        public readonly int Ticks;
        public readonly ActorFrameProperties Properties;
        public readonly Optional<ActorActionFunction> ActionFunction;
        public ActorFlowControl FlowControl;
    }
}

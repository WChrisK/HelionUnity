using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.States
{
    public class ActorFrame
    {
        public readonly UpperString Sprite;
        public readonly int Ticks;
        public readonly ActorFrameProperties Properties;
        public readonly Optional<ActorActionFunction> ActionFunction;
        public ActorFlowControl FlowControl = new ActorFlowControl();

        public ActorFrame(UpperString sprite, int ticks, ActorFrameProperties properties,
            Optional<ActorActionFunction> actionFunction)
        {
            Sprite = sprite;
            Ticks = ticks;
            Properties = properties;
            ActionFunction = actionFunction;
        }

        public ActorFrame(ActorFrame other)
        {
            Sprite = other.Sprite;
            Ticks = other.Ticks;
            Properties = new ActorFrameProperties(other.Properties);
            ActionFunction = other.ActionFunction.Map(af => new ActorActionFunction(af));
            FlowControl = new ActorFlowControl(other.FlowControl);
        }
    }
}

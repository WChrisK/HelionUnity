using Helion.Core.Util;
using Helion.Core.Util.Geometry;

namespace Helion.Core.Resource.Decorate.Definitions.States
{
    public class ActorFrameProperties
    {
        public bool? Bright;
        public bool? CanRaise;
        public bool? Fast;
        public Optional<UpperString> Light;
        public bool? NoDelay;
        public Vec2I? Offset;
        public bool? Slow;

        public ActorFrameProperties()
        {
        }

        public ActorFrameProperties(ActorFrameProperties other)
        {
            Bright = other.Bright;
            CanRaise = other.CanRaise;
            Fast = other.Fast;
            Light = new Optional<UpperString>(other.Light.Value);
            NoDelay = other.NoDelay;
            Offset = other.Offset;
            Slow = other.Slow;
        }
    }
}

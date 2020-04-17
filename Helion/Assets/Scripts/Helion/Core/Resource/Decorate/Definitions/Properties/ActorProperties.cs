using Helion.Core.Resource.Decorate.Definitions.Properties.Types;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.Properties
{
    public class ActorProperties
    {
        public int Height = 16;
        public int Radius = 20;
        public Optional<SpawnInfo> SpawnInfo = Optional<SpawnInfo>.Empty();

        public ActorProperties()
        {
        }

        public ActorProperties(ActorProperties other)
        {
            Height = other.Height;
            Radius = other.Radius;
            SpawnInfo = other.SpawnInfo.Map(si => new SpawnInfo(si));
        }
    }
}

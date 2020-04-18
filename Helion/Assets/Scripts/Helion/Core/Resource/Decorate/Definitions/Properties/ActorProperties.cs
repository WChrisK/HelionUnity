using Helion.Core.Resource.Decorate.Definitions.Properties.Types;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.Properties
{
    public class ActorProperties
    {
        public int Health = 1000;
        public int Height = 16;
        public int Mass = 100;
        public int Radius = 20;
        public int PainChance;
        public Optional<SpawnInfo> SpawnInfo = Optional<SpawnInfo>.Empty();
        public float Speed;

        public ActorProperties()
        {
        }

        public ActorProperties(ActorProperties other)
        {
            Health = other.Health;
            Height = other.Height;
            Mass = other.Mass;
            PainChance = other.PainChance;
            Radius = other.Radius;
            SpawnInfo = other.SpawnInfo.Map(si => new SpawnInfo(si));
            Speed = other.Speed;
        }
    }
}

namespace Helion.Core.Resource.Decorate.Definitions
{
    public class ActorProperties
    {
        public int Height = 16;
        public int Radius = 20;

        public ActorProperties()
        {
        }

        public ActorProperties(ActorProperties other)
        {
            Height = other.Height;
            Radius = other.Radius;
        }
    }
}

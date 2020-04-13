using Helion.Core.Util;

namespace Helion.Core.Worlds.Geometry
{
    public class Line
    {
        public Side Front;
        public Optional<Side> Back;

        public Line(Side front, Optional<Side> back)
        {
            Front = front;
            Back = back;
        }
    }
}

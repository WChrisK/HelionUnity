using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    public class Side
    {
        public readonly Optional<Wall> Upper;
        public readonly Optional<Wall> Middle;
        public readonly Optional<Wall> Lower;
        public Line Line;

        public Side(Wall middle) :
            this(new Optional<Wall>(), middle, new Optional<Wall>())
        {
        }

        public Side(Optional<Wall> upper, Optional<Wall> middle, Optional<Wall> lower)
        {
            Debug.Assert(upper || middle || lower, "Creating a side without any walls");

            Upper = upper;
            Middle = middle;
            Lower = lower;
        }
    }
}

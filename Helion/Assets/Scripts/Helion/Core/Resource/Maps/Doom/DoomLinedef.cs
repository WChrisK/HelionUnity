using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Util;

namespace Helion.Core.Resource.Maps.Doom
{
    /// <summary>
    /// A line in a Doom map.
    /// </summary>
    public class DoomLinedef
    {
        public readonly MapVertex Start;
        public readonly MapVertex End;
        public readonly DoomSidedef Front;
        public readonly Optional<DoomSidedef> Back;

        public DoomLinedef(MapVertex start, MapVertex end, DoomSidedef front, DoomSidedef back,
            ushort type, ushort flags, ushort sectorTag)
        {
            Start = start;
            End = end;
            Front = front;
            Back = back;
        }
    }
}

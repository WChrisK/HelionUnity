using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Util;

namespace Helion.Core.Resource.Maps.Doom
{
    /// <summary>
    /// A line in a Doom map.
    /// </summary>
    public class DoomLinedef
    {
        public readonly int Index;
        public readonly OldMapVertex Start;
        public readonly OldMapVertex End;
        public readonly DoomSidedef Front;
        public readonly Optional<DoomSidedef> Back;
        public readonly ushort Flags;

        public bool OneSided => !TwoSided;
        public bool TwoSided => Back.HasValue;
        public float Length => Start.Distance(End);

        public DoomLinedef(int index, OldMapVertex start, OldMapVertex end, DoomSidedef front, DoomSidedef back,
            ushort type, ushort flags, ushort sectorTag)
        {
            Index = index;
            Start = start;
            End = end;
            Front = front;
            Back = back;
            Flags = flags;
        }

        /// <summary>
        /// Gets the partner side. This should not be called on a one sided
        /// line. For example, if we pass this the front, we get the back.
        /// Likewise, if we pass in the back, we get the front side.
        /// </summary>
        /// <param name="side">The side that we want to get the partner of.
        /// </param>
        /// <returns>The partner side.</returns>
        public DoomSidedef PartnerSideOf(DoomSidedef side) => ReferenceEquals(side, Front) ? Back.Value : Front;
    }
}

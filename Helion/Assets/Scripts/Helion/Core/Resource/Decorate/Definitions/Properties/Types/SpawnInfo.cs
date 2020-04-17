using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public class SpawnInfo
    {
        public int Number;
        public SpawnGameMode Mode;
        public Optional<UpperString> Team = Optional<UpperString>.Empty();

        public SpawnInfo()
        {
        }

        public SpawnInfo(SpawnInfo other)
        {
            Number = other.Number;
            Mode = other.Mode;
            Team = new Optional<UpperString>(other.Team.Value);
        }
    }
}

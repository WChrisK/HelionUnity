using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Core.Util;
using static Helion.Core.Util.OptionalHelper;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public class SpawnInfo
    {
        public int Number;
        public SpawnGameMode Mode;
        public Optional<UpperString> Team = Empty;

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

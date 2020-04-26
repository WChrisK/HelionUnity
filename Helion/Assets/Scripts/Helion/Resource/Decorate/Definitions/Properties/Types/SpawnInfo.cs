using Helion.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Util;
using Helion.Worlds.Info;
using static Helion.Util.OptionalHelper;

namespace Helion.Resource.Decorate.Definitions.Properties.Types
{
    public class SpawnInfo
    {
        public int Number;
        public GameMode Mode;
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

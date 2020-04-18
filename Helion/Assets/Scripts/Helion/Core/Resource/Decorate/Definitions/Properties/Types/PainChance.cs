using System.Collections.Generic;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public class PainChance
    {
        public int? Value;
        public readonly Dictionary<UpperString, int> Types;

        public PainChance()
        {
            Types = new Dictionary<UpperString, int>();
        }

        public PainChance(PainChance other)
        {
            Value = other.Value;
            Types = new Dictionary<UpperString, int>(other.Types);
        }
    }
}

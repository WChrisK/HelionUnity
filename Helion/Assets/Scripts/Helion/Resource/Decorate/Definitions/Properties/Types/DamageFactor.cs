using System.Collections.Generic;
using Helion.Util;

namespace Helion.Resource.Decorate.Definitions.Properties.Types
{
    public class DamageFactor
    {
        public float Value = 1.0f;
        public readonly Dictionary<UpperString, float> Types;

        public DamageFactor()
        {
            Types = new Dictionary<UpperString, float>();
        }

        public DamageFactor(DamageFactor other)
        {
            Value = other.Value;
            Types = new Dictionary<UpperString, float>(other.Types);
        }
    }
}

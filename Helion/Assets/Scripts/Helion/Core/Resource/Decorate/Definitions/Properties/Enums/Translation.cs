using System.Collections.Generic;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Enums
{
    public class Translation
    {
        public int? Standard;
        public bool? Ice;
        public readonly List<UpperString> Translations = new List<UpperString>();

        public Translation()
        {
        }

        public Translation(Translation other)
        {
            Standard = other.Standard;
            Ice = other.Ice;
            Translations = new List<UpperString>(other.Translations);
        }
    }
}

using System.Collections.Generic;
using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Enums
{
    public class DecorateTranslation
    {
        public int? Standard;
        public bool? Ice;
        public readonly List<UpperString> Translations = new List<UpperString>();

        public DecorateTranslation()
        {
        }

        public DecorateTranslation(DecorateTranslation other)
        {
            Standard = other.Standard;
            Ice = other.Ice;
            Translations = new List<UpperString>(other.Translations);
        }
    }
}

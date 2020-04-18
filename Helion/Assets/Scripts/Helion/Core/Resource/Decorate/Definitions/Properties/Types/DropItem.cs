using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public class DropItem
    {
        public readonly UpperString ClassName;
        public readonly int? Probability;
        public readonly int? Amount;

        public DropItem(UpperString className, int? probability, int? amount)
        {
            ClassName = className;
            Probability = probability;
            Amount = amount;
        }

        public DropItem(DropItem other)
        {
            ClassName = other.ClassName;
            Probability = other.Probability;
            Amount = other.Amount;
        }
    }
}

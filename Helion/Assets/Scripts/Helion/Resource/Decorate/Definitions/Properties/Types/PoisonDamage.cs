namespace Helion.Resource.Decorate.Definitions.Properties.Types
{
    public struct PoisonDamage
    {
        public int Value;
        public int? Duration;
        public int? Period;

        public PoisonDamage(int value, int? duration, int? period)
        {
            Value = value;
            Duration = duration;
            Period = period;
        }
    }
}

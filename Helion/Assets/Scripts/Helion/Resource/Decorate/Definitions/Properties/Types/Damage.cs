namespace Helion.Resource.Decorate.Definitions.Properties.Types
{
    public struct Damage
    {
        public readonly int Value;
        public readonly bool MultiplyByEight;

        public Damage(int value, bool multiplyByEight)
        {
            Value = value;
            MultiplyByEight = multiplyByEight;
        }
    }
}

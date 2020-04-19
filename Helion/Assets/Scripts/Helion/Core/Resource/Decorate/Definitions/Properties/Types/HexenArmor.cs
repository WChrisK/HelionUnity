namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public struct HexenArmor
    {
        public readonly int Base;
        public readonly int Armor;
        public readonly int Shield;
        public readonly int Helm;
        public readonly int Amulet;

        public HexenArmor(int baseValue, int armor, int shield, int helm, int amulet)
        {
            Base = baseValue;
            Armor = armor;
            Shield = shield;
            Helm = helm;
            Amulet = amulet;
        }
    }
}

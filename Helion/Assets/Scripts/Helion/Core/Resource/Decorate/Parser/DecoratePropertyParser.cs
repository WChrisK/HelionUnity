using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Parser
{
    public partial class DecorateParser
    {
        private void ConsumeAmmoProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumeArmorProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumeFakeInventoryProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumeHealthProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumeHealthPickupProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumeInventoryProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumeMorphProjectileProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumePlayerProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumePowerupProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumePuzzleItemProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumeWeaponProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumeWeaponPieceProperty()
        {
            MakeException("Not supported currently");
        }

        private void ConsumeTopLevelPropertyOrCombo(UpperString property)
        {
            switch (property.String)
            {
            case "HEIGHT":
                currentDefinition.Properties.Height = ConsumeInteger();
                break;
            case "RADIUS":
                currentDefinition.Properties.Radius = ConsumeInteger();
                break;
            }
        }

        private void ConsumeActorPropertyOrCombo()
        {
            UpperString property = ConsumeString();

            if (ConsumeIf('.'))
            {
                switch (property.String)
                {
                case "AMMO":
                    ConsumeAmmoProperty();
                    break;
                case "ARMOR":
                    ConsumeArmorProperty();
                    break;
                case "FAKEINVENTORY":
                    ConsumeFakeInventoryProperty();
                    break;
                case "HEALTH":
                    ConsumeHealthProperty();
                    break;
                case "HEALTHPICKUP":
                    ConsumeHealthPickupProperty();
                    break;
                case "INVENTORY":
                    ConsumeInventoryProperty();
                    break;
                case "MORPHPROJECTILE":
                    ConsumeMorphProjectileProperty();
                    break;
                case "PLAYER":
                    ConsumePlayerProperty();
                    break;
                case "POWERUP":
                    ConsumePowerupProperty();
                    break;
                case "PUZZLEITEM":
                    ConsumePuzzleItemProperty();
                    break;
                case "WEAPON":
                    ConsumeWeaponProperty();
                    break;
                case "WEAPONPIECE":
                    ConsumeWeaponPieceProperty();
                    break;
                default:
                    throw MakeException($"Unknown prefix property '{property}' on actor '{currentDefinition.Name}'");
                }
            }
            else
                ConsumeTopLevelPropertyOrCombo(property);
        }
    }
}

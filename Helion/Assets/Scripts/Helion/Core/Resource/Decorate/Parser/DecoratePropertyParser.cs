using Helion.Core.Util;

namespace Helion.Core.Resource.Decorate.Parser
{
    public partial class DecorateParser
    {
        private void ConsumeAmmoProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumeArmorProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumeFakeInventoryProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumeHealthProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumeHealthPickupProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumeInventoryProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumeMorphProjectileProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumePlayerProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumePowerupProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumePuzzleItemProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumeWeaponProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ConsumeWeaponPieceProperty()
        {
            throw MakeException("Not supported currently");
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

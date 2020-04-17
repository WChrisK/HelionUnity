using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Core.Resource.Decorate.Definitions.Properties.Types;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Resource.Decorate.Parser
{
    public partial class DecorateParser
    {
        private void ReadAmmoProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadArmorProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadFakeInventoryProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadHealthProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadHealthPickupProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadInventoryProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadMorphProjectileProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadPlayerProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadPowerupProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadPuzzleItemProperty()
        {
            throw MakeException("Not supported currently");
        }

        private SpawnGameMode ReadSpawnType()
        {
            string spawnName = ConsumeString();

            switch (spawnName.ToUpper())
            {
            case "COOPERATIVE":
                return SpawnGameMode.Cooperative;
            case "DEATHMATCH":
                return SpawnGameMode.Deathmatch;
            case "TEAM":
                return SpawnGameMode.Team;
            default:
                throw MakeException($"Unknown spawn type: {spawnName}");
            }
        }

        private void ReadSpawnInfoProperty()
        {
            if (!currentDefinition.Properties.SpawnInfo)
                currentDefinition.Properties.SpawnInfo = new SpawnInfo();

            SpawnInfo spawnInfo = currentDefinition.Properties.SpawnInfo.Value;
            string name = ConsumeIdentifier();

            switch (name.ToUpper())
            {
            case "MODE":
                spawnInfo.Mode = ReadSpawnType();
                break;
            case "NUMBER":
                spawnInfo.Number = ConsumeInteger();
                break;
            case "TEAM":
                spawnInfo.Team = ConsumeString().AsUpper();
                break;
            default:
                throw MakeException($"Unexpected SpawnInfo property type: {name}");
            }
        }

        private void ReadWeaponProperty()
        {
            throw MakeException("Not supported currently");
        }

        private void ReadWeaponPieceProperty()
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
            UpperString property = ConsumeIdentifier();

            if (ConsumeIf('.'))
            {
                switch (property.String)
                {
                case "AMMO":
                    ReadAmmoProperty();
                    break;
                case "ARMOR":
                    ReadArmorProperty();
                    break;
                case "FAKEINVENTORY":
                    ReadFakeInventoryProperty();
                    break;
                case "HEALTH":
                    ReadHealthProperty();
                    break;
                case "HEALTHPICKUP":
                    ReadHealthPickupProperty();
                    break;
                case "INVENTORY":
                    ReadInventoryProperty();
                    break;
                case "MORPHPROJECTILE":
                    ReadMorphProjectileProperty();
                    break;
                case "PLAYER":
                    ReadPlayerProperty();
                    break;
                case "POWERUP":
                    ReadPowerupProperty();
                    break;
                case "PUZZLEITEM":
                    ReadPuzzleItemProperty();
                    break;
                case "SPAWNINFO":
                    ReadSpawnInfoProperty();
                    break;
                case "WEAPON":
                    ReadWeaponProperty();
                    break;
                case "WEAPONPIECE":
                    ReadWeaponPieceProperty();
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

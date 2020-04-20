using System.Collections.Generic;
using System.Globalization;
using Helion.Core.Graphics;
using Helion.Core.Resource.Decorate.Definitions.Flags;
using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Core.Resource.Decorate.Definitions.Properties.Types;
using Helion.Core.Resource.Maps.Actions;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using UnityEngine;

namespace Helion.Core.Resource.Decorate.Parser
{
    public partial class DecorateParser
    {
        #region Helper Functions

        public SpecialArgs ReadArgs()
        {
            List<int> args = new List<int> { ConsumeInteger() };

            for (int i = 0; i < SpecialArgs.NumArgs - 1; i++)
                if (ConsumeIf(','))
                    args.Add(ConsumeInteger());

            return new SpecialArgs(args);
        }

        private Color ReadColor()
        {
            if (PeekInteger())
            {
                int intR = ConsumeInteger();
                Consume(',');
                int intG = ConsumeInteger();
                Consume(',');
                int intB = ConsumeInteger();
                return ColorHelper.FromRGB(intR, intG, intB);
            }

            string colorString = ConsumeString();

            Color? fromString = ColorHelper.StringToColor(colorString);
            if (fromString != null)
                return fromString.Value;

            if (colorString.Length != 8)
                throw MakeException($"Expecting 'rr gg bb' format for a color in actor '{currentDefinition.Name}");

            string redStr = colorString.Substring(0, 2);
            if (!int.TryParse(redStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int r))
                throw MakeException($"Cannot parse red component from 'rr gg bb' format for a color in actor '{currentDefinition.Name}");

            string greenStr = colorString.Substring(3, 2);
            if (!int.TryParse(greenStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int g))
                throw MakeException($"Cannot parse red component from 'rr gg bb' format for a color in actor '{currentDefinition.Name}");

            string blueStr = colorString.Substring(6, 2);
            if (!int.TryParse(blueStr, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int b))
                throw MakeException($"Cannot parse red component from 'rr gg bb' format for a color in actor '{currentDefinition.Name}");

            return ColorHelper.FromRGB(r, g, b);
        }

        private DecorateRange<int> ReadSignedIntRange()
        {
            int min = ConsumeSignedInteger();
            Consume(',');
            int max = ConsumeSignedInteger();

            return new DecorateRange<int>(min, max);
        }

        private List<UpperString> ReadStringList()
        {
            List<UpperString> list = new List<UpperString> { ConsumeString() };
            while (ConsumeIf(','))
                list.Add(ConsumeString());

            return list;
        }

        #endregion

        #region Ammo Property Readers

        private void ReadAmmoProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "BACKPACKAMOUNT":
                currentDefinition.Properties.AmmoBackpackAmount = ConsumeInteger();
                break;
            case "BACKPACKMAXAMOUNT":
                currentDefinition.Properties.AmmoBackpackMaxAmount = ConsumeInteger();
                break;
            case "DROPAMOUNT":
                currentDefinition.Properties.AmmoDropAmount = ConsumeInteger();
                break;
            default:
                throw MakeException($"Unknown Ammo property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region Armor Property Readers

        private void ReadArmorProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "MAXABSORB":
                currentDefinition.Properties.ArmorMaxAbsorb = ConsumeInteger();
                break;
            case "MAXBONUS":
                currentDefinition.Properties.ArmorMaxBonus = ConsumeInteger();
                break;
            case "MAXBONUSMAX":
                currentDefinition.Properties.ArmorMaxBonusMax = ConsumeInteger();
                break;
            case "MAXSAVEAMOUNT":
                currentDefinition.Properties.ArmorMaxSaveAmount = ConsumeInteger();
                break;
            case "MAXFULLABSORB":
                currentDefinition.Properties.ArmorMaxFullAbsorb = ConsumeInteger();
                break;
            case "SAVEAMOUNT":
                currentDefinition.Properties.ArmorSaveAmount = ConsumeInteger();
                break;
            case "SAVEPERCENT":
                currentDefinition.Properties.ArmorSavePercent = ConsumeFloat().Clamp(0, 100);
                break;
            default:
                throw MakeException($"Unknown Armor property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region FakeInventory Property Readers

        private void ReadFakeInventoryProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "RESPAWNS":
                currentDefinition.Properties.FakeInventoryRespawns = true;
                break;
            default:
                throw MakeException($"Unknown FakeInventory property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region Health Property Readers

        private void ReadHealthProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "LOWMESSAGE":
                int health = ConsumeInteger();
                Consume(',');
                string message = ConsumeString();
                currentDefinition.Properties.HealthLowMessage.Set(health, message);
                break;
            default:
                throw MakeException($"Unknown Health property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region HealthPickup Property Readers

        private void ReadHealthPickupProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "AUTOUSE":
                switch (ConsumeInteger())
                {
                case 0:
                    currentDefinition.Properties.HealthPickupAutoUse = AutoUseType.Never;
                    break;
                case 1:
                    currentDefinition.Properties.HealthPickupAutoUse = AutoUseType.WouldDieAndHasAutoUse;
                    break;
                case 2:
                    currentDefinition.Properties.HealthPickupAutoUse = AutoUseType.WouldDieAndHasAutoUseDeathmatch;
                    break;
                case 3:
                    currentDefinition.Properties.HealthPickupAutoUse = AutoUseType.UnderFiftyHealth;
                    break;
                default:
                    throw MakeException($"Out of range HealthPickup property {selector} on actor {currentDefinition.Name} (should be within 0 - 3)");
                }
                break;
            default:
                throw MakeException($"Unknown HealthPickup property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region Inventory Readers

        private void ReadInventoryProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "ALTHUDICON":
                currentDefinition.Properties.InventoryAltHUDIcon = ConsumeString().AsUpper();
                break;
            case "AMOUNT":
                currentDefinition.Properties.InventoryAmount = ConsumeInteger();
                break;
            case "DEFMAXAMOUNT":
                // Note: This is wrong if it's for Heretic (then would be 16).
                currentDefinition.Properties.InventoryDefMaxAmount = 25;
                break;
            case "FORBIDDENTO":
                currentDefinition.Properties.InventoryForbiddenTo = ReadStringList();
                break;
            case "GIVEQUEST":
                currentDefinition.Properties.InventoryGiveQuest = ConsumeInteger();
                if (currentDefinition.Properties.InventoryGiveQuest < 1 || currentDefinition.Properties.InventoryGiveQuest > 31)
                    throw MakeException($"Must have Inventory.GiveQuest for actor {currentDefinition.Name} be in range of [1, 31] inclusive");
                break;
            case "ICON":
                currentDefinition.Properties.InventoryIcon = ConsumeString().AsUpper();
                break;
            case "INTERHUBAMOUNT":
                currentDefinition.Properties.InventoryInterHubAmount = ConsumeInteger();
                break;
            case "MAXAMOUNT":
                currentDefinition.Properties.InventoryMaxAmount = ConsumeInteger();
                break;
            case "PICKUPFLASH":
                currentDefinition.Properties.InventoryPickupFlash = ConsumeString().AsUpper();
                break;
            case "PICKUPMESSAGE":
                currentDefinition.Properties.InventoryPickupMessage = ConsumeString().AsUpper();
                break;
            case "PICKUPSOUND":
                currentDefinition.Properties.InventoryPickupSound = ConsumeString().AsUpper();
                break;
            case "RESPAWNTICS":
                currentDefinition.Properties.InventoryRespawnTics = ConsumeInteger();
                break;
            case "RESTRICTEDTO":
                currentDefinition.Properties.InventoryRestrictedTo = ReadStringList();
                break;
            case "USESOUND":
                currentDefinition.Properties.InventoryUseSound = ConsumeString().AsUpper();
                break;
            default:
                throw MakeException($"Unknown Inventory property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region MorphProjectile Property Readers

        private MorphStyle? ReadMorphStyle()
        {
            string style = ConsumeString();
            switch (style.ToUpper())
            {
            case "MRF_ADDSTAMINA":
                return MorphStyle.AddStamina;
            case "MRF_FAILNOLAUGH":
                return MorphStyle.FailNoLaugh;
            case "MRF_FAILNOTELEFRAG":
                return MorphStyle.FailNotTelefrag;
            case "MRF_FULLHEALTH":
                return MorphStyle.FullHealth;
            case "MRF_LOSEACTUALWEAPON":
                return MorphStyle.LoseActualWeapon;
            case "MRF_NEWTIDBEHAVIOUR":
                return MorphStyle.NewTidBehavior;
            case "MRF_TRANSFERTRANSLATION":
                return MorphStyle.TransferTranslation;
            case "MRF_UNDOALWAYS":
                return MorphStyle.UndoAlways;
            case "MRF_UNDOBYTOMEOFPOWER":
                return MorphStyle.UndoByTomeOfPower;
            case "MRF_UNDOBYCHAOSDEVICE":
                return MorphStyle.UndoByChaosDevice;
            case "MRF_UNDOBYDEATH":
                return MorphStyle.UndoByDeath;
            case "MRF_UNDOBYDEATHFORCED":
                return MorphStyle.UndoByDeathForced;
            case "MRF_UNDOBYDEATHSAVES":
                return MorphStyle.UndoByDeathSaves;
            case "MRF_WHENINVULNERABLE":
                return MorphStyle.WhenInvulnerable;
            default:
                throw MakeException($"Unknown morph style type '{style}' on actor '{currentDefinition.Name}'");
            }
        }

        private void ReadMorphProjectileProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "DURATION":
                currentDefinition.Properties.MorphProjectileDuration = ConsumeInteger();
                break;
            case "MONSTERCLASS":
                currentDefinition.Properties.MorphProjectileMonsterClass = ConsumeString().AsUpper();
                break;
            case "MORPHFLASH":
                currentDefinition.Properties.MorphProjectileMorphFlash = ConsumeString().AsUpper();
                break;
            case "MORPHSTYLE":
                currentDefinition.Properties.MorphProjectileMorphStyle = ReadMorphStyle();
                break;
            case "PLAYERCLASS":
                currentDefinition.Properties.MorphProjectilePlayerClass = ConsumeString().AsUpper();
                break;
            case "UNMORPHFLASH":
                currentDefinition.Properties.MorphProjectileUnMorphFlash = ConsumeString().AsUpper();
                break;
            default:
                throw MakeException($"Unknown MorphProjectile property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region Player Property Readers

        private void ReadDamageScreenColor()
        {
            Color color = ReadColor();
            float? intensity = null;
            UpperString damageTypeOrNull = null;

            if (ConsumeIf(','))
                intensity = ConsumeFloat().Clamp(0, 1);
            if (ConsumeIf(','))
                damageTypeOrNull = ConsumeString();

            if (damageTypeOrNull != null)
            {
                var component = new DamageScreenColor.DamageScreenColorComponent(color, intensity);
                currentDefinition.Properties.PlayerDamageScreenColor.DamageTypes[damageTypeOrNull] = component;
                return;
            }

            currentDefinition.Properties.PlayerDamageScreenColor.Color = color;
            if (intensity != null)
                currentDefinition.Properties.PlayerDamageScreenColor.Intensity = intensity;
        }

        private HexenArmor ReadHexenArmor()
        {
            int baseValue = ConsumeInteger();
            if (baseValue % 5 != 0)
                throw MakeException($"Player.HexenArmor value Base must be divisible by 5 in {currentDefinition.Name}");

            int armor = ConsumeInteger();
            if (armor % 5 != 0)
                throw MakeException($"Player.HexenArmor value Armor must be divisible by 5 in {currentDefinition.Name}");

            int shield = ConsumeInteger();
            if (shield % 5 != 0)
                throw MakeException($"Player.HexenArmor value Shield must be divisible by 5 in {currentDefinition.Name}");

            int helm = ConsumeInteger();
            if (helm % 5 != 0)
                throw MakeException($"Player.HexenArmor value Helm must be divisible by 5 in {currentDefinition.Name}");

            int amulet = ConsumeInteger();
            if (amulet % 5 != 0)
                throw MakeException($"Player.HexenArmor value Amulet must be divisible by 5 in {currentDefinition.Name}");

            return new HexenArmor(baseValue, armor, shield, helm, amulet);
        }

        private RunSpeed ReadRunSpeed()
        {
            float run = ConsumeFloat();
            float walk = 1.0f;

            if (ConsumeIf(','))
                walk = ConsumeFloat();

            return new RunSpeed(run, walk);
        }

        private void ReadStartItem()
        {
            UpperString startItemName = ConsumeString();

            // Note: I have no idea if this is needed, the wiki says it is not
            // needed, yet the definitions elsewhere say it is. I have no idea
            // so I'll just accept both as valid for now.
            if (ConsumeIf(','))
            {
                currentDefinition.Properties.PlayerStartItem[startItemName] = ConsumeInteger();
                return;
            }

            if (PeekInteger())
                currentDefinition.Properties.PlayerStartItem[startItemName] = ConsumeInteger();
            else
                currentDefinition.Properties.PlayerStartItem[startItemName] = 1;
        }

        private void ReadPlayerProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "AIRCAPACITY":
                currentDefinition.Properties.PlayerAirCapacity = ConsumeFloat();
                break;
            case "ATTACKZOFFSET":
                currentDefinition.Properties.PlayerAttackZOffset = ConsumeFloat();
                break;
            case "COLORSET":
            case "COLORSETFILE":
            case "CLEARCOLORSET":
                throw MakeException($"Player.{selector} is not supported");
            case "CROUCHSPRITE":
                currentDefinition.Properties.PlayerCrouchSprite = ConsumeString().AsUpper();
                break;
            case "DAMAGESCREENCOLOR":
                ReadDamageScreenColor();
                break;
            case "DISPLAYNAME":
                currentDefinition.Properties.PlayerDisplayName = ConsumeString().AsUpper();
                break;
            case "FACE":
                currentDefinition.Properties.PlayerFace = ConsumeString().AsUpper();
                break;
            case "FALLINGSCREAMSPEED":
                float fallingScreamMin = ConsumeFloat();
                Consume(',');
                float fallingScreamMax = ConsumeFloat();
                currentDefinition.Properties.PlayerFallingScreamSpeed = new DecorateRange<float>(fallingScreamMin, fallingScreamMax);
                break;
            case "FLECHETTETYPE":
                currentDefinition.Properties.PlayerFlechetteType = ConsumeString().AsUpper();
                break;
            case "FORWARDMOVE":
                currentDefinition.Properties.PlayerForwardMove = ReadRunSpeed();
                break;
            case "GRUNTSPEED":
                currentDefinition.Properties.PlayerGruntSpeed = ConsumeFloat();
                break;
            case "HEALRADIUSTYPE":
                currentDefinition.Properties.PlayerHealRadiusType = ConsumeString().AsUpper();
                break;
            case "HEXENARMOR":
                currentDefinition.Properties.PlayerHexenArmor = ReadHexenArmor();
                break;
            case "JUMPZ":
                currentDefinition.Properties.PlayerJumpZ = ConsumeFloat();
                break;
            case "MAXHEALTH":
                currentDefinition.Properties.PlayerMaxHealth = ConsumeInteger();
                break;
            case "MORPHWEAPON":
                currentDefinition.Properties.PlayerMorphWeapon = ConsumeString().AsUpper();
                break;
            case "MUGSHOTMAXHEALTH":
                currentDefinition.Properties.PlayerMugShotMaxHealth = ConsumeSignedInteger();
                break;
            case "RUNHEALTH":
                currentDefinition.Properties.PlayerRunHealth = ConsumeInteger();
                break;
            case "PORTRAIT":
                currentDefinition.Properties.PlayerPortrait = ConsumeString().AsUpper();
                break;
            case "SCOREICON":
                currentDefinition.Properties.PlayerScoreIcon = ConsumeString().AsUpper();
                break;
            case "SIDEMOVE":
                currentDefinition.Properties.PlayerSideMove = ReadRunSpeed();
                break;
            case "SOUNDCLASS":
                currentDefinition.Properties.PlayerSoundClass = ConsumeString().AsUpper();
                break;
            case "SPAWNCLASS":
                currentDefinition.Properties.PlayerSpawnClass = ConsumeString().AsUpper();
                break;
            case "STARTITEM":
                ReadStartItem();
                break;
            case "TELEPORTFREEZETIME":
                currentDefinition.Properties.PlayerTeleportFreezeTime = ConsumeSignedInteger();
                break;
            case "USERANGE":
                currentDefinition.Properties.PlayerUseRange = ConsumeFloat();
                break;
            case "WEAPONSLOT":
                int weaponSlot = ConsumeInteger();
                Consume(',');
                List<UpperString> weapons = ReadStringList();
                currentDefinition.Properties.PlayerWeaponSlot[weaponSlot] = weapons;
                break;
            case "VIEWBOB":
                currentDefinition.Properties.PlayerViewBob = ConsumeFloat();
                break;
            case "VIEWHEIGHT":
                currentDefinition.Properties.PlayerViewHeight = ConsumeFloat();
                break;
            default:
                throw MakeException($"Unknown Player property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region Powerup Property Readers

        private PowerupColor ReadPowerupColor()
        {
            // Note that this might not be quite identical to what is normally
            // done. We will fix it if we ever run into problems.

            float alpha = 0.333333f;

            if (PeekString())
            {
                PeekCurrentText(out string current);
                PowerupColorType? colorType = null;

                switch (current.ToUpper())
                {
                case "BLUEMAP":
                    colorType = PowerupColorType.Blue;
                    break;
                case "GOLDMAP":
                    colorType = PowerupColorType.Gold;
                    break;
                case "GREENMAP":
                    colorType = PowerupColorType.Green;
                    break;
                case "INVERSEMAP":
                    colorType = PowerupColorType.Inverse;
                    break;
                case "REDMAP":
                    colorType = PowerupColorType.Red;
                    break;
                case "NONE":
                    colorType = PowerupColorType.None;
                    break;
                }

                // If we ran into a known color, then we can safely look for
                // the alpha value. However if we don't, we'll try one last
                // ditch read since we only peaked.
                if (colorType != null)
                {
                    // We only peaked, so consume it now.
                    ConsumeString();

                    if (ConsumeIf(','))
                        alpha = ConsumeFloat().Clamp(0, 1);

                    return new PowerupColor(colorType.Value, alpha);
                }
            }

            Color color = ReadColor();
            if (ConsumeIf(','))
                alpha = ConsumeFloat().Clamp(0, 1);

            return new PowerupColor(color, alpha);
        }

        private PowerupColormap ReadPowerupColormap()
        {
            float r = ConsumeFloat().Clamp(0, 1);
            Consume(',');
            float g = ConsumeFloat().Clamp(0, 1);
            Consume(',');
            float b = ConsumeFloat().Clamp(0, 1);

            if (ConsumeIf(','))
            {
                float destR = ConsumeFloat().Clamp(0, 1);
                Consume(',');
                float destG = ConsumeFloat().Clamp(0, 1);
                Consume(',');
                float destB = ConsumeFloat().Clamp(0, 1);

                Color source = new Color(r, g, b);
                Color dest = new Color(destR, destG, destB);
                return new PowerupColormap(source, dest);
            }

            return new PowerupColormap(null, new Color(r, g, b));
        }

        private void ReadPowerupProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "COLOR":
                currentDefinition.Properties.PowerupColor = ReadPowerupColor();
                break;
            case "COLORMAP":
                currentDefinition.Properties.PowerupColormap = ReadPowerupColormap();
                break;
            case "DURATION":
                currentDefinition.Properties.PowerupDuration = ConsumeSignedInteger();
                break;
            case "MODE":
                currentDefinition.Properties.PowerupMode = ReadRenderStyle();
                break;
            case "STRENGTH":
                currentDefinition.Properties.PowerupStrength = ConsumeInteger();
                break;
            case "TYPE":
                currentDefinition.Properties.PowerupType = ConsumeString().AsUpper();
                break;
            default:
                throw MakeException($"Unknown Powerup property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region PowerSpeed Property Readers

        private void ReadPowerSpeedProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "NOTRAIL":
                currentDefinition.Properties.PowerSpeedNoTrail = ConsumeInteger() != 0;
                break;
            default:
                throw MakeException($"Unknown PowerSpeed property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region PuzzleItem Property Readers

        private void ReadPuzzleItemProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "NUMBER":
                currentDefinition.Properties.PuzzleItemNumber = ConsumeInteger();
                break;
            case "FAILMESSAGE":
                currentDefinition.Properties.PuzzleItemFailMessage = ConsumeString().AsUpper();
                break;
            default:
                throw MakeException($"Unknown PuzzleItem property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region SpawnType Property Readers

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

        #endregion

        #region Weapon Property Readers

        private BobStyle ReadBobStyle()
        {
            string text = ConsumeString();

            switch (text.ToUpper())
            {
            case "ALPHA":
                return BobStyle.Alpha;
            case "INVERSEALPHA":
                return BobStyle.InverseAlpha;
            case "INVERSENORMAL":
                return BobStyle.InverseNormal;
            case "INVERSESMOOTH":
                return BobStyle.InverseSmooth;
            case "NORMAL":
                return BobStyle.Normal;
            case "SMOOTH":
                return BobStyle.Smooth;
            default:
                throw MakeException($"Unknown weapon bob style {text} on actor {currentDefinition.Name}");
            }
        }

        private void ReadWeaponProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "AMMOGIVE":
                currentDefinition.Properties.WeaponAmmoGive = ConsumeSignedInteger();
                break;
            case "AMMOGIVE1":
                currentDefinition.Properties.WeaponAmmoGive1 = ConsumeSignedInteger();
                break;
            case "AMMOGIVE2":
                currentDefinition.Properties.WeaponAmmoGive2 = ConsumeSignedInteger();
                break;
            case "AMMOTYPE":
                currentDefinition.Properties.WeaponAmmoType = ConsumeString().AsUpper();
                break;
            case "AMMOTYPE1":
                currentDefinition.Properties.WeaponAmmoType1 = ConsumeString().AsUpper();
                break;
            case "AMMOTYPE2":
                currentDefinition.Properties.WeaponAmmoType2 = ConsumeString().AsUpper();
                break;
            case "AMMOUSE":
                currentDefinition.Properties.WeaponAmmoUse = ConsumeInteger();
                break;
            case "AMMOUSE1":
                currentDefinition.Properties.WeaponAmmoUse1 = ConsumeInteger();
                break;
            case "AMMOUSE2":
                currentDefinition.Properties.WeaponAmmoUse2 = ConsumeInteger();
                break;
            case "BOBRANGEX":
                currentDefinition.Properties.WeaponBobRangeX = ConsumeFloat();
                break;
            case "BOBRANGEY":
                currentDefinition.Properties.WeaponBobRangeY = ConsumeFloat();
                break;
            case "BOBSPEED":
                currentDefinition.Properties.WeaponBobSpeed = ConsumeFloat();
                break;
            case "BOBSTYLE":
                currentDefinition.Properties.WeaponBobStyle = ReadBobStyle();
                break;
            case "DEFAULTKICKBACK":
                currentDefinition.Properties.WeaponDefaultKickBack = ConsumeBoolean();
                break;
            case "KICKBACK":
                currentDefinition.Properties.WeaponKickBack = ConsumeInteger();
                break;
            case "LOOKSCALE":
                currentDefinition.Properties.WeaponLookScale = ConsumeFloat();
                break;
            case "MINSELECTIONAMMO1":
                currentDefinition.Properties.WeaponMinSelectionAmmo1 = ConsumeInteger();
                break;
            case "MINSELECTIONAMMO2":
                currentDefinition.Properties.WeaponMinSelectionAmmo2 = ConsumeInteger();
                break;
            case "READYSOUND":
                currentDefinition.Properties.WeaponReadySound = ConsumeString().AsUpper();
                break;
            case "SELECTIONORDER":
                currentDefinition.Properties.WeaponSelectionOrder = ConsumeInteger();
                break;
            case "SISTERWEAPON":
                currentDefinition.Properties.WeaponSisterWeapon = ConsumeString().AsUpper();
                break;
            case "SLOTNUMBER":
                currentDefinition.Properties.WeaponSlotNumber = ConsumeInteger();
                break;
            case "SLOTPRIORITY":
                currentDefinition.Properties.WeaponSlotPriority = ConsumeSignedFloat();
                break;
            case "UPSOUND":
                currentDefinition.Properties.WeaponUpSound = ConsumeString().AsUpper();
                break;
            case "YADJUST":
                currentDefinition.Properties.WeaponYAdjust = ConsumeInteger();
                break;
            default:
                throw MakeException($"Unknown Weapon property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region WeaponPiece Property Readers

        private void ReadWeaponPieceProperty()
        {
            UpperString selector = ConsumeIdentifier();

            switch (selector.String)
            {
            case "NUMBER":
                currentDefinition.Properties.WeaponPieceNumber = ConsumeInteger();
                break;
            case "WEAPON":
                currentDefinition.Properties.WeaponPieceWeapon = ConsumeString().AsUpper();
                break;
            default:
                throw MakeException($"Unknown WeaponPiece property {selector} on actor {currentDefinition.Name}");
            }
        }

        #endregion

        #region Top Level Property Readers

        private ThingSpecialActivationType ReadActivation()
        {
            UpperString activation = ConsumeString();
            ThingSpecialActivationType result = TextToType(activation);
            while (ConsumeIf('|'))
                result |= TextToType(ConsumeString());

            return result;

            ThingSpecialActivationType TextToType(UpperString text)
            {
                switch (text.String)
                {
                case "THINGSPEC_DEFAULT":
                    return ThingSpecialActivationType.Default;
                case "THINGSPEC_THINGACTS":
                    return ThingSpecialActivationType.ThingActs;
                case "THINGSPEC_TRIGGERACTS":
                    return ThingSpecialActivationType.TriggerActs;
                case "THINGSPEC_THINGTARGETS":
                    return ThingSpecialActivationType.ThingTargets;
                case "THINGSPEC_TRIGGERTARGETS":
                    return ThingSpecialActivationType.TriggerTargets;
                case "THINGSPEC_MONSTERTRIGGER":
                    return ThingSpecialActivationType.MonsterTrigger;
                case "THINGSPEC_MISSILETRIGGER":
                    return ThingSpecialActivationType.MissileTrigger;
                case "THINGSPEC_CLEARSPECIAL":
                    return ThingSpecialActivationType.ClearSpecial;
                case "THINGSPEC_NODEATHSPECIAL":
                    return ThingSpecialActivationType.NoDeathSpecial;
                case "THINGSPEC_ACTIVATE":
                    return ThingSpecialActivationType.Activate;
                case "THINGSPEC_DEACTIVATE":
                    return ThingSpecialActivationType.Deactivate;
                case "THINGSPEC_SWITCH":
                    return ThingSpecialActivationType.Switch;
                default:
                    throw MakeException($"Unknown activation type '{text}' on actor '{currentDefinition.Name}'");
                }
            }
        }

        private void ReadBloodType()
        {
            currentDefinition.Properties.BloodType.Add(ConsumeString());

            if (ConsumeIf(','))
                currentDefinition.Properties.BloodType.Add(ConsumeString());

            if (ConsumeIf(','))
                currentDefinition.Properties.BloodType.Add(ConsumeString());
        }

        private BounceType ReadBounceType()
        {
            string text = ConsumeString();
            switch (text.ToUpper())
            {
            case "NONE":
                return BounceType.None;
            case "DOOM":
                return BounceType.Doom;
            case "HERETIC":
                return BounceType.Heretic;
            case "HEXEN":
                return BounceType.Hexen;
            case "CLASSIC":
                return BounceType.Classic;
            case "GRENADE":
                return BounceType.Grenade;
            case "DOOMCOMPAT":
                return BounceType.DoomCompat;
            case "HERETICCOMPAT":
                return BounceType.HereticCompat;
            case "HEXENCOMPAT":
                return BounceType.HexenCompat;
            default:
                throw MakeException($"Unknown activation type '{text}' on actor '{currentDefinition.Name}'");
            }
        }

        private Damage ReadDamage()
        {
            if (PeekInteger())
                return new Damage(ConsumeInteger(), false);

            Consume('(');
            int value = ConsumeInteger();
            Consume(')');
            return new Damage(value, true);
        }

        private void ReadDamageFactor()
        {
            if (PeekFloat())
            {
                currentDefinition.Properties.DamageFactor.Value = ConsumeFloat();
                return;
            }

            UpperString name = ConsumeString();
            currentDefinition.Properties.DamageFactor.Types[name] = ConsumeFloat();
        }

        private DropItem ReadDropItem()
        {
            UpperString name = ConsumeString();
            int? probability = null;
            int? amount = null;

            if (ConsumeIf(','))
                probability = ConsumeInteger();
            if (ConsumeIf(','))
                amount = ConsumeInteger();

            return new DropItem(name, probability, amount);
        }

        private void ReadPainChance()
        {
            if (PeekInteger())
            {
                currentDefinition.Properties.PainChance.Value = ConsumeInteger();
                return;
            }

            UpperString name = ConsumeString();
            currentDefinition.Properties.PainChance.Types[name] = ConsumeInteger();
        }

        private void ReadPoisonDamage()
        {
            int value = ConsumeInteger();
            int? duration = null;
            int? period = null;

            if (ConsumeIf(','))
                duration = ConsumeInteger();
            if (ConsumeIf(','))
                period = ConsumeInteger();

            currentDefinition.Properties.PoisonDamage = new PoisonDamage(value, duration, period);
        }

        private RenderStyle ReadRenderStyle()
        {
            string style = ConsumeString();
            switch (style.ToUpper())
            {
            case "NONE":
                return RenderStyle.None;
            case "NORMAL":
                return RenderStyle.Normal;
            case "FUZZY":
                return RenderStyle.Fuzzy;
            case "SOULTRANS":
                return RenderStyle.SoulTrans;
            case "OPTFUZZY":
                return RenderStyle.OptFuzzy;
            case "STENCIL":
                return RenderStyle.Stencil;
            case "ADDSTENCIL":
                return RenderStyle.AddStencil;
            case "TRANSLUCENT":
                return RenderStyle.Translucent;
            case "ADD":
                return RenderStyle.Add;
            case "SUBTRACT":
                return RenderStyle.Subtract;
            case "SHADED":
                return RenderStyle.Shaded;
            case "ADDSHADED":
                return RenderStyle.AddShaded;
            case "SHADOW":
                return RenderStyle.Shadow;
            default:
                throw MakeException($"Unknown heal radius type '{style}' on actor '{currentDefinition.Name}'");
            }
        }

        private void ReadTranslation()
        {
            if (PeekInteger())
            {
                int index = ConsumeInteger();
                if (index < 0 || index > 2)
                    throw MakeException($"Translation index out of range for {currentDefinition.Name}");

                currentDefinition.Properties.DecorateTranslation.Standard = index;
                return;
            }

            UpperString text = ConsumeString();
            if (text == "ICE")
            {
                currentDefinition.Properties.DecorateTranslation.Ice = true;
                return;
            }

            // TODO: Assert it's a proper translation string!
            currentDefinition.Properties.DecorateTranslation.Translations.Add(text);

            while (ConsumeIf(','))
            {
                // TODO: Assert it's a proper translation string!
                currentDefinition.Properties.DecorateTranslation.Translations.Add(ConsumeString());
            }
        }

        private void ConsumeTopLevelProperty(UpperString property)
        {
            switch (property.String)
            {
            case "ACCURACY":
                currentDefinition.Properties.Accuracy = ConsumeInteger();
                break;
            case "ACTIVATION":
                currentDefinition.Properties.Activation = ReadActivation();
                break;
            case "ACTIVESOUND":
                currentDefinition.Properties.ActiveSound = ConsumeString().AsUpper();
                break;
            case "ALPHA":
                currentDefinition.Properties.Alpha = ConsumeFloat();
                break;
            case "ARGS":
                currentDefinition.Properties.Args = ReadArgs();
                break;
            case "ATTACKSOUND":
                currentDefinition.Properties.AttackSound = ConsumeString().AsUpper();
                break;
            case "BLOODCOLOR":
                currentDefinition.Properties.BloodColor = ReadColor();
                break;
            case "BLOODTYPE":
                ReadBloodType();
                break;
            case "BOUNCECOUNT":
                currentDefinition.Properties.BounceCount = ConsumeInteger();
                break;
            case "BOUNCEFACTOR":
                currentDefinition.Properties.BounceFactor = ConsumeFloat();
                break;
            case "BOUNCESOUND":
                currentDefinition.Properties.BounceSound = ConsumeString().AsUpper();
                break;
            case "BOUNCETYPE":
                currentDefinition.Properties.BounceType = ReadBounceType();
                break;
            case "BURNHEIGHT":
                currentDefinition.Properties.BurnHeight = ConsumeInteger();
                break;
            case "CAMERAHEIGHT":
                currentDefinition.Properties.CameraHeight = ConsumeInteger();
                break;
            case "CONVERSATIONID":
                currentDefinition.Properties.ConversationID = ConsumeInteger();
                break;
            case "CRUSHPAINSOUND":
                currentDefinition.Properties.CrushPainSound = ConsumeString().AsUpper();
                break;
            case "DAMAGE":
                currentDefinition.Properties.Damage = ReadDamage();
                break;
            case "DAMAGEFACTOR":
                ReadDamageFactor();
                break;
            case "DAMAGETYPE":
                currentDefinition.Properties.DamageType = ConsumeString().AsUpper();
                break;
            case "DEATHHEIGHT":
                currentDefinition.Properties.DeathHeight = ConsumeInteger();
                break;
            case "DEATHSOUND":
                currentDefinition.Properties.DeathSound = ConsumeString().AsUpper();
                break;
            case "DEATHTYPE":
                currentDefinition.Properties.DeathType = ConsumeString().AsUpper();
                break;
            case "DECAL":
                currentDefinition.Properties.Decal = ConsumeString().AsUpper();
                break;
            case "DEFTHRESHOLD":
                currentDefinition.Properties.DefThreshold = ConsumeInteger();
                break;
            case "DESIGNATEDTEAM":
                currentDefinition.Properties.DesignatedTeam = ConsumeInteger();
                break;
            case "DISTANCECHECK":
                currentDefinition.Properties.DistanceCheck = ConsumeString().AsUpper();
                break;
            case "DROPITEM":
                currentDefinition.Properties.DropItem.Add(ReadDropItem());
                break;
            case "EXPLOSIONDAMAGE":
                currentDefinition.Properties.ExplosionDamage = ConsumeInteger();
                break;
            case "EXPLOSIONRADIUS":
                currentDefinition.Properties.ExplosionRadius = ConsumeInteger();
                break;
            case "FASTSPEED":
                currentDefinition.Properties.FastSpeed = ConsumeFloat();
                break;
            case "FLOATBOBPHASE":
                currentDefinition.Properties.FloatBobPhase = ConsumeInteger();
                break;
            case "FLOATBOBSTRENGTH":
                currentDefinition.Properties.FloatBobStrength = ConsumeFloat();
                break;
            case "FLOATSPEED":
                currentDefinition.Properties.FloatSpeed = ConsumeFloat();
                break;
            case "FRICTION":
                currentDefinition.Properties.Friction = ConsumeFloat();
                break;
            case "FRIENDLYSEEBLOCKS":
                currentDefinition.Properties.FriendlySeeBlocks = ConsumeInteger();
                break;
            case "GAME":
                // Note: We do not support multiple games by only checking one.
                currentDefinition.Properties.Game.Add(ConsumeString());
                break;
            case "GIBHEALTH":
                currentDefinition.Properties.GibHealth = ConsumeInteger();
                break;
            case "GRAVITY":
                currentDefinition.Properties.Gravity = ConsumeFloat();
                break;
            case "HEALTH":
                currentDefinition.Properties.Health = ConsumeInteger();
                break;
            case "HEIGHT":
                currentDefinition.Properties.Height = ConsumeInteger();
                break;
            case "HITOBITUARY":
                currentDefinition.Properties.HitObituary = ConsumeString();
                break;
            case "HOWLSOUND":
                currentDefinition.Properties.HowlSound = ConsumeString().AsUpper();
                break;
            case "MASS":
                currentDefinition.Properties.Mass = ConsumeInteger();
                break;
            case "MAXSTEPHEIGHT":
                currentDefinition.Properties.MaxStepHeight = ConsumeInteger();
                break;
            case "MAXDROPOFFHEIGHT":
                currentDefinition.Properties.MaxDropOffHeight = ConsumeInteger();
                break;
            case "MAXTARGETRANGE":
                currentDefinition.Properties.MaxTargetRange = ConsumeInteger();
                break;
            case "MELEEDAMAGE":
                currentDefinition.Properties.MeleeDamage = ConsumeInteger();
                break;
            case "MELEERANGE":
                currentDefinition.Properties.MeleeRange = ConsumeInteger();
                break;
            case "MELEESOUND":
                currentDefinition.Properties.MeleeSound = ConsumeString().AsUpper();
                break;
            case "MELEETHRESHOLD":
                currentDefinition.Properties.MeleeThreshold = ConsumeInteger();
                break;
            case "MINMISSILECHANCE":
                currentDefinition.Properties.MinMissileChance = ConsumeInteger();
                break;
            case "MISSILEHEIGHT":
                currentDefinition.Properties.MissileHeight = ConsumeInteger();
                break;
            case "MISSILETYPE":
                currentDefinition.Properties.MissileType = ConsumeString().AsUpper();
                break;
            case "OBITUARY":
                currentDefinition.Properties.Obituary = ConsumeString();
                break;
            case "PAINCHANCE":
                ReadPainChance();
                break;
            case "PAINSOUND":
                currentDefinition.Properties.PainSound = ConsumeString().AsUpper();
                break;
            case "PAINTHRESHOLD":
                currentDefinition.Properties.PainThreshold = ConsumeInteger();
                break;
            case "PAINTYPE":
                currentDefinition.Properties.PainType = ConsumeString().AsUpper();
                break;
            case "POISONDAMAGE":
                ReadPoisonDamage();
                break;
            case "POISONDAMAGETYPE":
                currentDefinition.Properties.PoisonDamageType = ConsumeString().AsUpper();
                break;
            case "PROJECTILEKICKBACK":
                currentDefinition.Properties.ProjectileKickBack = ConsumeInteger();
                break;
            case "PROJECTILEPASSHEIGHT":
                currentDefinition.Properties.ProjectilePassHeight = ConsumeSignedInteger();
                break;
            case "PUSHFACTOR":
                currentDefinition.Properties.PushFactor = ConsumeFloat();
                break;
            case "RADIUS":
                currentDefinition.Properties.Radius = ConsumeInteger();
                break;
            case "RADIUSDAMAGEFACTOR":
                currentDefinition.Properties.RadiusDamageFactor = ConsumeFloat();
                break;
            case "REACTIONTIME":
                currentDefinition.Properties.ReactionTime = ConsumeInteger();
                break;
            case "RENDERRADIUS":
                currentDefinition.Properties.RenderRadius = ConsumeSignedFloat();
                break;
            case "RENDERSTYLE":
                currentDefinition.Properties.RenderStyle = ReadRenderStyle();
                break;
            case "RIPLEVELMAX":
                currentDefinition.Properties.RipLevelMax = ConsumeInteger();
                break;
            case "RIPLEVELMIN":
                currentDefinition.Properties.RipLevelMin = ConsumeInteger();
                break;
            case "RIPPERLEVEL":
                currentDefinition.Properties.RipperLevel = ConsumeSignedInteger();
                break;
            case "SCALE":
                currentDefinition.Properties.Scale = ConsumeFloat();
                break;
            case "SEESOUND":
                currentDefinition.Properties.SeeSound = ConsumeString().AsUpper();
                break;
            case "SELFDAMAGEFACTOR":
                currentDefinition.Properties.SelfDamageFactor = ConsumeFloat();
                break;
            case "SPAWNID":
                currentDefinition.Properties.SpawnID = ConsumeInteger();
                break;
            case "SPECIES":
                currentDefinition.Properties.Species = ConsumeString().AsUpper();
                break;
            case "SPEED":
                currentDefinition.Properties.Speed = ConsumeFloat();
                break;
            case "SPRITEANGLE":
                currentDefinition.Properties.SpriteAngle = ConsumeInteger();
                break;
            case "SPRITEROTATION":
                currentDefinition.Properties.SpriteRotation = ConsumeInteger();
                break;
            case "STAMINA":
                currentDefinition.Properties.Stamina = ConsumeInteger();
                break;
            case "STEALTHALPHA":
                currentDefinition.Properties.StealthAlpha = ConsumeFloat();
                break;
            case "STENCILCOLOR":
                currentDefinition.Properties.StencilColor = ReadColor();
                break;
            case "TAG":
                currentDefinition.Properties.Tag = ConsumeString().AsUpper();
                break;
            case "TELEFOGDESTTYPE":
                currentDefinition.Properties.TeleFogDestType = ConsumeString();
                break;
            case "TELEFOGSOURCETYPE":
                currentDefinition.Properties.TeleFogSourceType = ConsumeString();
                break;
            case "THRESHOLD":
                currentDefinition.Properties.Threshold = ConsumeInteger();
                break;
            case "TRANSLATION":
                ReadTranslation();
                break;
            case "VISIBLEANGLES":
                currentDefinition.Properties.VisibleAngles = ReadSignedIntRange();
                break;
            case "VISIBLEPITCH":
                currentDefinition.Properties.VisiblePitch = ReadSignedIntRange();
                break;
            case "VISIBLETOPLAYERCLASS":
                currentDefinition.Properties.VisibleToPlayerClass = ReadStringList();
                break;
            case "VISIBLETOTEAM":
                currentDefinition.Properties.VisibleToTeam = ConsumeInteger();
                break;
            case "VSPEED":
                currentDefinition.Properties.VSpeed = ConsumeFloat();
                break;
            case "WALLBOUNCEFACTOR":
                currentDefinition.Properties.WallBounceFactor = ConsumeFloat();
                break;
            case "WALLBOUNCESOUND":
                currentDefinition.Properties.WallBounceSound = ConsumeString().AsUpper();
                break;
            case "WEAVEINDEXXY":
                currentDefinition.Properties.WeaveIndexXY = ConsumeInteger();
                if (currentDefinition.Properties.WeaveIndexXY < 0 || currentDefinition.Properties.WeaveIndexXY >= 64)
                    throw MakeException($"WeaveIndexXY must be in the range of 0, 64 for {currentDefinition.Name}");
                break;
            case "WEAVEINDEXZ":
                currentDefinition.Properties.WeaveIndexZ = ConsumeInteger();
                if (currentDefinition.Properties.WeaveIndexZ < 0 || currentDefinition.Properties.WeaveIndexZ >= 64)
                    throw MakeException($"WeaveIndexZ must be in the range of 0, 64 for {currentDefinition.Name}");
                break;
            case "WOUNDHEALTH":
                currentDefinition.Properties.WoundHealth = ConsumeInteger();
                break;
            case "XSCALE":
                currentDefinition.Properties.XScale = ConsumeFloat();
                break;
            case "YSCALE":
                currentDefinition.Properties.YScale = ConsumeFloat();
                break;
            }
        }

        #endregion

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
                case "POWERSPEED":
                    ReadPowerSpeedProperty();
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
            {
                switch (property.String)
                {
                case "DEFAULTALPHA":
                    // Note: This is not accurate, it depends on the game, and
                    // it also can be overridden by a definition later on. We
                    // are doing it wrong for Heretic here.
                    currentDefinition.Properties.Alpha = 0.6f;
                    break;
                case "CLEARFLAGS":
                    currentDefinition.Flags.ClearAll();
                    break;
                case "MONSTER":
                    currentDefinition.Flags.Set(ActorFlagType.ActivateMCross, true);
                    currentDefinition.Flags.Set(ActorFlagType.CanPass, true);
                    currentDefinition.Flags.Set(ActorFlagType.CanPushWalls, true);
                    currentDefinition.Flags.Set(ActorFlagType.CanUseWalls, true);
                    currentDefinition.Flags.Set(ActorFlagType.CountKill, true);
                    currentDefinition.Flags.Set(ActorFlagType.IsMonster, true);
                    currentDefinition.Flags.Set(ActorFlagType.Shootable, true);
                    currentDefinition.Flags.Set(ActorFlagType.Solid, true);
                    break;
                case "PROJECTILE":
                    currentDefinition.Flags.Set(ActorFlagType.ActivateImpact, true);
                    currentDefinition.Flags.Set(ActorFlagType.ActivatePCross, true);
                    currentDefinition.Flags.Set(ActorFlagType.Dropoff, true);
                    currentDefinition.Flags.Set(ActorFlagType.NoBlockmap, true);
                    currentDefinition.Flags.Set(ActorFlagType.NoGravity, true);
                    currentDefinition.Flags.Set(ActorFlagType.Missile, true);
                    currentDefinition.Flags.Set(ActorFlagType.NoTeleport, true);
                    break;
                default:
                    ConsumeTopLevelProperty(property);
                    break;
                }
            }
        }
    }
}

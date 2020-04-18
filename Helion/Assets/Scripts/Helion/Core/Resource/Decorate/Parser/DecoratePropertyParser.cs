using System.Globalization;
using Helion.Core.Graphics;
using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Core.Resource.Decorate.Definitions.Properties.Types;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using UnityEngine;

namespace Helion.Core.Resource.Decorate.Parser
{
    public partial class DecorateParser
    {
        #region Helper Functions

        private Color ReadColor()
        {
            if (PeekInteger())
                return new Color(ConsumeInteger() / 255.0f, ConsumeInteger() / 255.0f, ConsumeInteger() / 255.0f, 1.0f);

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

            return new Color(r / 255.0f, g / 255.0f, b / 255.0f);
        }

        #endregion

        #region Ammo Property Readers

        private void ReadAmmoProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region Armor Property Readers

        private void ReadArmorProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region FakeInventory Property Readers

        private void ReadFakeInventoryProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region Health Property Readers

        private void ReadHealthProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region HealthPickup Property Readers

        private void ReadHealthPickupProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region Inventory Readers

        private void ReadInventoryProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region MorphProjectile Property Readers

        private void ReadMorphProjectileProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region Player Property Readers

        private void ReadPlayerProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region Powerup Property Readers

        private void ReadPowerupProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region PuzzleItem Property Readers

        private void ReadPuzzleItemProperty()
        {
            throw MakeException("Not supported currently");
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

        private void ReadWeaponProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region WeaponPiece Property Readers

        private void ReadWeaponPieceProperty()
        {
            throw MakeException("Not supported currently");
        }

        #endregion

        #region Top Level Property Readers

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

                currentDefinition.Properties.Translation.Standard = index;
                return;
            }

            UpperString text = ConsumeString();
            if (text == "ICE")
            {
                currentDefinition.Properties.Translation.Ice = true;
                return;
            }

            // TODO: Assert it's a proper translation string!
            currentDefinition.Properties.Translation.Translations.Add(text);

            while (ConsumeIf(','))
            {
                // TODO: Assert it's a proper translation string!
                currentDefinition.Properties.Translation.Translations.Add(ConsumeString());
            }
        }

        private void ConsumeTopLevelPropertyOrCombo(UpperString property)
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
                currentDefinition.Properties.Damage = ConsumeInteger();
                throw MakeException("HANDLE BRACKETS YOU IDIOT");
            case "DAMAGEFACTOR":
                ReadDamageFactor();
                break;
            case "DEATHHEIGHT":
                currentDefinition.Properties.DeathHeight = ConsumeInteger();
                break;
            case "DEATHSOUND":
                currentDefinition.Properties.DeathSound = ConsumeString().AsUpper();
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
            case "PAINCHANCE":
                ReadPainChance();
                break;
            case "PAINTHRESHOLD":
                currentDefinition.Properties.PainThreshold = ConsumeInteger();
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
                currentDefinition.Properties.ProjectilePassHeight = ConsumeInteger();
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
            case "VSPEED":
                currentDefinition.Properties.VSpeed = ConsumeFloat();
                break;
            case "WALLBOUNCEFACTOR":
                currentDefinition.Properties.WallBounceFactor = ConsumeFloat();
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

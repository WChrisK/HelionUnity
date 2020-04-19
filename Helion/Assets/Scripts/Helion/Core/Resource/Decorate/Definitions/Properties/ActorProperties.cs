using System.Collections.Generic;
using System.Linq;
using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using Helion.Core.Resource.Decorate.Definitions.Properties.Types;
using Helion.Core.Resource.Maps.Actions;
using Helion.Core.Util;
using MoreLinq;
using UnityEngine;

namespace Helion.Core.Resource.Decorate.Definitions.Properties
{
    public class ActorProperties
    {
        public int? Accuracy; // Unimplemented!
        public ThingSpecialActivationType Activation = ThingSpecialActivationType.Default; // Unimplemented!
        public Optional<UpperString> ActiveSound = Optional<UpperString>.Empty(); // Unimplemented!
        public float Alpha = 1.0f; // Unimplemented!
        public int? AmmoBackpackAmount; // Unimplemented!
        public int? AmmoBackpackMaxAmount; // Unimplemented!
        public int? AmmoDropAmount; // Unimplemented!
        public int? ArmorMaxAbsorb; // Unimplemented!
        public int? ArmorMaxBonus; // Unimplemented!
        public int? ArmorMaxBonusMax; // Unimplemented!
        public int? ArmorMaxFullAbsorb; // Unimplemented!
        public int? ArmorMaxSaveAmount; // Unimplemented!
        public int? ArmorSaveAmount; // Unimplemented!
        public float? ArmorSavePercent; // Unimplemented!
        public SpecialArgs Args = new SpecialArgs();
        public Optional<UpperString> AttackSound = Optional<UpperString>.Empty(); // Unimplemented!
        public Color? BloodColor; // Unimplemented!
        public readonly List<UpperString> BloodType = new List<UpperString>(); // Unimplemented!
        public int BounceCount = int.MaxValue; // Unimplemented!
        public float BounceFactor = 0.7f; // Unimplemented!
        public Optional<UpperString> BounceSound = Optional<UpperString>.Empty(); // Unimplemented!
        public BounceType BounceType = BounceType.None; // Unimplemented!
        public int? BurnHeight; // Unimplemented!
        public int? CameraHeight; // Unimplemented!
        public int ConversationID; // Unimplemented!
        public Optional<UpperString> CrushPainSound = Optional<UpperString>.Empty(); // Unimplemented!
        public Damage? Damage; // Unimplemented!
        public readonly DamageFactor DamageFactor = new DamageFactor(); // Unimplemented!
        public Optional<UpperString> DamageType = Optional<UpperString>.Empty(); // Unimplemented!
        public int? DeathHeight; // Unimplemented!
        public Optional<UpperString> DeathSound = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> DeathType = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> Decal = Optional<UpperString>.Empty(); // Unimplemented!
        public int? DefThreshold; // Unimplemented!
        public int? DesignatedTeam; // Unimplemented!
        public Optional<UpperString> DistanceCheck = Optional<UpperString>.Empty(); // Unimplemented!
        public readonly List<DropItem> DropItem = new List<DropItem>(); // Unimplemented!
        public int? ExplosionDamage; // Unimplemented!
        public int? ExplosionRadius; // Unimplemented!
        public bool FakeInventoryRespawns; // Unimplemented!
        public float? FastSpeed; // Unimplemented!
        public int FloatBobPhase; // Unimplemented!
        public float FloatBobStrength = 1.0f; // Unimplemented!
        public float FloatSpeed = 4.0f; // Unimplemented!
        public float Friction = 1.0f; // Unimplemented!
        public int FriendlySeeBlocks = 10; // Unimplemented!
        public readonly List<UpperString> Game = new List<UpperString>(); // Unimplemented!
        public int? GibHealth; // Unimplemented!
        public float Gravity = 1.0f; // Unimplemented!
        public int Health = 1000; // Unimplemented!
        public readonly HealthLowMessage HealthLowMessage = new HealthLowMessage(); // Unimplemented!
        public AutoUseType HealthPickupAutoUse = AutoUseType.Never; // Unimplemented!
        public int Height = 16;
        public Optional<string> HitObituary = Optional<string>.Empty(); // Unimplemented!
        public Optional<UpperString> HowlSound = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> InventoryAltHUDIcon = Optional<UpperString>.Empty(); // Unimplemented!
        public int? InventoryAmount; // Unimplemented!
        public int? InventoryDefMaxAmount; // Unimplemented!
        public List<UpperString> InventoryForbiddenTo = new List<UpperString>(); // Unimplemented!
        public int? InventoryGiveQuest; // Unimplemented!
        public Optional<UpperString> InventoryIcon = Optional<UpperString>.Empty(); // Unimplemented!
        public int InventoryInterHubAmount; // Unimplemented!
        public int InventoryMaxAmount; // Unimplemented!
        public Optional<UpperString> InventoryPickupFlash = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> InventoryPickupMessage = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> InventoryPickupSound = Optional<UpperString>.Empty(); // Unimplemented!
        public int? InventoryRespawnTics; // Unimplemented!
        public List<UpperString> InventoryRestrictedTo = new List<UpperString>(); // Unimplemented!
        public Optional<UpperString> InventoryUseSound = Optional<UpperString>.Empty(); // Unimplemented!
        public int Mass = 100; // Unimplemented!
        public int MaxStepHeight = 24; // Unimplemented!
        public int MaxDropOffHeight = 24; // Unimplemented!
        public int? MaxTargetRange; // Unimplemented!
        public int? MeleeDamage; // Unimplemented!
        public int MeleeRange = 44; // Unimplemented!
        public Optional<UpperString> MeleeSound = Optional<UpperString>.Empty(); // Unimplemented!
        public int? MeleeThreshold; // Unimplemented!
        public int MinMissileChance = 200; // Unimplemented!
        public int? MissileHeight; // Unimplemented!
        public Optional<UpperString> MissileType = Optional<UpperString>.Empty(); // Unimplemented!
        public int? MorphProjectileDuration; // Unimplemented!
        public Optional<UpperString> MorphProjectileMonsterClass = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> MorphProjectileMorphFlash = Optional<UpperString>.Empty(); // Unimplemented!
        public MorphStyle? MorphProjectileMorphStyle; // Unimplemented!
        public Optional<UpperString> MorphProjectilePlayerClass = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> MorphProjectileUnMorphFlash = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<string> Obituary = Optional<string>.Empty(); // Unimplemented!
        public readonly PainChance PainChance = new PainChance(); // Unimplemented!
        public Optional<UpperString> PainSound = Optional<UpperString>.Empty(); // Unimplemented!
        public int PainThreshold; // Unimplemented!
        public Optional<UpperString> PainType = Optional<UpperString>.Empty(); // Unimplemented!
        public float PlayerAirCapacity = 1.0f; // Unimplemented!
        public float PlayerAttackZOffset; // Unimplemented!
        public DecorateRange<int> PlayerColorRange = new DecorateRange<int>(0, 0); // Unimplemented!
        public Optional<UpperString> PlayerCrouchSprite = Optional<UpperString>.Empty(); // Unimplemented!
        public readonly DamageScreenColor PlayerDamageScreenColor = new DamageScreenColor(); // Unimplemented!
        public Optional<UpperString> PlayerDisplayName = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> PlayerFace = Optional<UpperString>.Empty(); // Unimplemented!
        public DecorateRange<float> PlayerFallingScreamSpeed = new DecorateRange<float>(35, 45); // Unimplemented!
        public Optional<UpperString> PlayerFlechetteType = Optional<UpperString>.Empty(); // Unimplemented!
        public RunSpeed PlayerForwardMove = new RunSpeed(1.0f, 1.0f); // Unimplemented!
        public float PlayerGruntSpeed = 12.0f; // Unimplemented!
        public Optional<UpperString> PlayerHealRadiusType = Optional<UpperString>.Empty(); // Unimplemented!
        public HexenArmor? PlayerHexenArmor; // Unimplemented!
        public float PlayerJumpZ = 8.0f; // Unimplemented!
        public int PlayerMaxHealth = 100; // Unimplemented!
        public Optional<UpperString> PlayerMorphWeapon = Optional<UpperString>.Empty(); // Unimplemented!
        public int PlayerMugShotMaxHealth; // Unimplemented!
        public Optional<UpperString> PlayerPortrait = Optional<UpperString>.Empty(); // Unimplemented!
        public int PlayerRunHealth; // Unimplemented!
        public Optional<UpperString> PlayerScoreIcon = Optional<UpperString>.Empty(); // Unimplemented!
        public RunSpeed PlayerSideMove = new RunSpeed(1.0f, 1.0f); // Unimplemented!
        public Optional<UpperString> PlayerSoundClass = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> PlayerSpawnClass = Optional<UpperString>.Empty(); // Unimplemented!
        public readonly Dictionary<UpperString, int> PlayerStartItem = new Dictionary<UpperString, int>(); // Unimplemented!
        public int PlayerTeleportFreezeTime = 18; // Unimplemented!
        public float PlayerUseRange = 64.0f; // Unimplemented!
        public readonly Dictionary<int, List<UpperString>> PlayerWeaponSlot = new Dictionary<int, List<UpperString>>(); // Unimplemented!
        public float PlayerViewBob = 1.0f; // Unimplemented!
        public float PlayerViewHeight = 41.0f;
        public PoisonDamage? PoisonDamage; // Unimplemented!
        public Optional<UpperString> PoisonDamageType = Optional<UpperString>.Empty(); // Unimplemented!
        public PowerupColor? PowerupColor; // Unimplemented!
        public PowerupColormap? PowerupColormap; // Unimplemented!
        public int? PowerupDuration; // Unimplemented!
        public RenderStyle PowerupMode = RenderStyle.Normal; // Unimplemented!
        public int? PowerupStrength; // Unimplemented!
        public Optional<UpperString> PowerupType = Optional<UpperString>.Empty(); // Unimplemented!
        public bool PowerSpeedNoTrail; // Unimplemented!
        public int? ProjectileKickBack; // Unimplemented!
        public int? ProjectilePassHeight; // Unimplemented!
        public float PushFactor = 0.25f; // Unimplemented!
        public int? PuzzleItemNumber; // Unimplemented!
        public Optional<UpperString> PuzzleItemFailMessage = Optional<UpperString>.Empty(); // Unimplemented!
        public int Radius = 20;
        public float RadiusDamageFactor = 1.0f; // Unimplemented!
        public int ReactionTime = 8; // Unimplemented!
        public float RenderRadius; // Unimplemented!
        public RenderStyle RenderStyle = RenderStyle.Normal; // Unimplemented!
        public int RipLevelMax; // Unimplemented!
        public int RipLevelMin; // Unimplemented!
        public int? RipperLevel; // Unimplemented!
        public float Scale = 1.0f; // Unimplemented!
        public Optional<UpperString> SeeSound = Optional<UpperString>.Empty(); // Unimplemented!
        public float SelfDamageFactor = 1.0f; // Unimplemented!
        public int SpawnID; // Unimplemented!
        public Optional<SpawnInfo> SpawnInfo = Optional<SpawnInfo>.Empty(); // Unimplemented!
        public Optional<UpperString> Species = Optional<UpperString>.Empty(); // Unimplemented!
        public float Speed; // Unimplemented!
        public int? SpriteAngle; // Unimplemented!
        public int? SpriteRotation; // Unimplemented!
        public int? Stamina; // Unimplemented!
        public float StealthAlpha; // Unimplemented!
        public Color? StencilColor; // Unimplemented!
        public Optional<UpperString> Tag = Optional<UpperString>.Empty(); // Unimplemented!
        public UpperString TeleFogDestType = "TELEPORTFOG"; // Unimplemented!
        public UpperString TeleFogSourceType = "TELEPORTFOG"; // Unimplemented!
        public int Threshold; // Unimplemented!
        public readonly Translation Translation = new Translation(); // Unimplemented!
        public DecorateRange<int>? VisibleAngles; // Unimplemented!
        public DecorateRange<int>? VisiblePitch; // Unimplemented!
        public List<UpperString> VisibleToPlayerClass = new List<UpperString>(); // Unimplemented!
        public int? VisibleToTeam; // Unimplemented!
        public float? VSpeed; // Unimplemented!
        public float WallBounceFactor = 0.7f; // Unimplemented!
        public Optional<UpperString> WallBounceSound = Optional<UpperString>.Empty(); // Unimplemented!
        public int? WeaponAmmoGive; // Unimplemented!
        public int? WeaponAmmoGive1; // Unimplemented!
        public int? WeaponAmmoGive2; // Unimplemented!
        public Optional<UpperString> WeaponAmmoType = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> WeaponAmmoType1 = Optional<UpperString>.Empty(); // Unimplemented!
        public Optional<UpperString> WeaponAmmoType2 = Optional<UpperString>.Empty(); // Unimplemented!
        public int? WeaponAmmoUse; // Unimplemented!
        public int? WeaponAmmoUse1; // Unimplemented!
        public int? WeaponAmmoUse2; // Unimplemented!
        public float WeaponBobRangeX = 1.0f; // Unimplemented!
        public float WeaponBobRangeY = 1.0f; // Unimplemented!
        public float WeaponBobSpeed = 1.0f; // Unimplemented!
        public BobStyle WeaponBobStyle = BobStyle.Normal; // Unimplemented!
        public bool WeaponDefaultKickBack; // Unimplemented!
        public int? WeaponKickBack; // Unimplemented!
        public float? WeaponLookScale; // Unimplemented!
        public int? WeaponMinSelectionAmmo1; // Unimplemented!
        public int? WeaponMinSelectionAmmo2; // Unimplemented!
        public Optional<UpperString> WeaponReadySound = Optional<UpperString>.Empty(); // Unimplemented!
        public int? WeaponSelectionOrder; // Unimplemented!
        public Optional<UpperString> WeaponSisterWeapon = Optional<UpperString>.Empty(); // Unimplemented!
        public int? WeaponSlotNumber; // Unimplemented!
        public float? WeaponSlotPriority; // Unimplemented!
        public Optional<UpperString> WeaponUpSound = Optional<UpperString>.Empty(); // Unimplemented!
        public int? WeaponYAdjust; // Unimplemented!
        public int? WeaponPieceNumber; // Unimplemented!
        public Optional<UpperString> WeaponPieceWeapon = Optional<UpperString>.Empty(); // Unimplemented!
        public int? WeaveIndexXY; // Unimplemented!
        public int? WeaveIndexZ; // Unimplemented!
        public int WoundHealth; // Unimplemented!
        public float XScale = 1.0f; // Unimplemented!
        public float YScale = 1.0f; // Unimplemented!

        public ActorProperties()
        {
        }

        public ActorProperties(ActorProperties other)
        {
            Accuracy = other.Accuracy;
            Activation = other.Activation;
            ActiveSound = other.ActiveSound.Copy();
            Alpha = other.Alpha;
            AmmoBackpackAmount = other.AmmoBackpackAmount;
            AmmoBackpackMaxAmount = other.AmmoBackpackMaxAmount;
            AmmoDropAmount = other.AmmoDropAmount;
            ArmorMaxAbsorb = other.ArmorMaxAbsorb;
            ArmorMaxBonus = other.ArmorMaxBonus;
            ArmorMaxBonusMax = other.ArmorMaxBonusMax;
            ArmorMaxSaveAmount = other.ArmorMaxSaveAmount;
            ArmorMaxFullAbsorb = other.ArmorMaxFullAbsorb;
            ArmorSaveAmount = other.ArmorSaveAmount;
            ArmorSavePercent = other.ArmorSavePercent;
            Args = new SpecialArgs(other.Args);
            AttackSound = other.AttackSound.Copy();
            BloodColor = other.BloodColor;
            BloodType = new List<UpperString>(other.BloodType);
            BounceCount = other.BounceCount;
            BounceFactor = other.BounceFactor;
            BounceSound = other.BounceSound.Copy();
            BounceType = other.BounceType;
            BurnHeight = other.BurnHeight;
            CameraHeight = other.CameraHeight;
            ConversationID = other.ConversationID;
            CrushPainSound = other.CrushPainSound.Copy();
            Damage = other.Damage;
            DamageFactor = new DamageFactor(other.DamageFactor);
            DamageType = other.DamageType.Copy();
            DeathHeight = other.DeathHeight;
            DeathSound = other.DeathSound.Copy();
            DeathType = other.DeathType.Copy();
            Decal = other.Decal.Copy();
            DefThreshold = other.DefThreshold;
            DesignatedTeam = other.DesignatedTeam;
            DistanceCheck = other.DistanceCheck.Copy();
            DropItem = other.DropItem.Select(val => new DropItem(val)).ToList();
            ExplosionDamage = other.ExplosionDamage;
            ExplosionRadius = other.ExplosionRadius;
            FakeInventoryRespawns = other.FakeInventoryRespawns;
            FastSpeed = other.FastSpeed;
            FloatBobPhase = other.FloatBobPhase;
            FloatBobStrength = other.FloatBobStrength;
            FloatSpeed = other.FloatSpeed;
            Friction = other.Friction;
            FriendlySeeBlocks = other.FriendlySeeBlocks;
            Game = new List<UpperString>(Game);
            GibHealth = other.GibHealth;
            Gravity = other.Gravity;
            Health = other.Health;
            HealthLowMessage = new HealthLowMessage(other.HealthLowMessage);
            HealthPickupAutoUse = other.HealthPickupAutoUse;
            Height = other.Height;
            HitObituary = other.HitObituary.Copy();
            HowlSound = other.HowlSound.Copy();
            InventoryAltHUDIcon = other.InventoryAltHUDIcon.Copy();
            InventoryAmount = other.InventoryAmount;
            InventoryDefMaxAmount = other.InventoryDefMaxAmount;
            InventoryForbiddenTo = new List<UpperString>(other.InventoryForbiddenTo);
            InventoryGiveQuest = other.InventoryGiveQuest;
            InventoryIcon = other.InventoryIcon.Copy();
            InventoryInterHubAmount = other.InventoryInterHubAmount;
            InventoryMaxAmount = other.InventoryMaxAmount;
            InventoryPickupFlash = other.InventoryPickupFlash.Copy();
            InventoryPickupMessage = other.InventoryPickupMessage.Copy();
            InventoryPickupSound = other.InventoryPickupSound.Copy();
            InventoryRespawnTics = other.InventoryRespawnTics;
            InventoryRestrictedTo = new List<UpperString>(other.InventoryRestrictedTo);
            InventoryUseSound = other.InventoryUseSound.Copy();
            Mass = other.Mass;
            MaxStepHeight = other.MaxStepHeight;
            MaxDropOffHeight = other.MaxDropOffHeight;
            MaxTargetRange = other.MaxTargetRange;
            MeleeDamage = other.MeleeDamage;
            MeleeRange = other.MeleeRange;
            MeleeSound = other.MeleeSound.Copy();
            MeleeThreshold = other.MeleeThreshold;
            MinMissileChance = other.MinMissileChance;
            MissileHeight = other.MissileHeight;
            MissileType = other.MissileType.Copy();
            MorphProjectileDuration = other.MorphProjectileDuration;
            MorphProjectileMonsterClass = other.MorphProjectileMonsterClass.Copy();
            MorphProjectileMorphFlash = other.MorphProjectileMorphFlash.Copy();
            MorphProjectileMorphStyle = other.MorphProjectileMorphStyle;
            MorphProjectilePlayerClass = other.MorphProjectilePlayerClass.Copy();
            MorphProjectileUnMorphFlash = other.MorphProjectileUnMorphFlash.Copy();
            Obituary = other.Obituary.Copy();
            PainChance = new PainChance(other.PainChance);
            PainSound = other.PainSound.Copy();
            PainThreshold = other.PainThreshold;
            PainType = other.PainType.Copy();
            PlayerAirCapacity = other.PlayerAirCapacity;
            PlayerAttackZOffset = other.PlayerAttackZOffset;
            PlayerColorRange = other.PlayerColorRange;
            PlayerCrouchSprite = other.PlayerCrouchSprite.Copy();
            PlayerDamageScreenColor = new DamageScreenColor(other.PlayerDamageScreenColor);
            PlayerDisplayName = other.PlayerDisplayName.Copy();
            PlayerFace = other.PlayerFace.Copy();
            PlayerFallingScreamSpeed = other.PlayerFallingScreamSpeed;
            PlayerFlechetteType = other.PlayerFlechetteType.Copy();
            PlayerForwardMove = other.PlayerForwardMove;
            PlayerGruntSpeed = other.PlayerGruntSpeed;
            PlayerHealRadiusType = other.PlayerHealRadiusType.Copy();
            PlayerHexenArmor = other.PlayerHexenArmor;
            PlayerJumpZ = other.PlayerJumpZ;
            PlayerMaxHealth = other.PlayerMaxHealth;
            PlayerMorphWeapon = other.PlayerMorphWeapon.Copy();
            PlayerMugShotMaxHealth = other.PlayerMugShotMaxHealth;
            PlayerPortrait = other.PlayerPortrait.Copy();
            PlayerRunHealth = other.PlayerRunHealth;
            PlayerScoreIcon = other.PlayerScoreIcon.Copy();
            PlayerSideMove = other.PlayerSideMove;
            PlayerSoundClass = other.PlayerSoundClass.Copy();
            PlayerSpawnClass = other.PlayerSpawnClass.Copy();
            PlayerStartItem = new Dictionary<UpperString, int>(other.PlayerStartItem);
            PlayerTeleportFreezeTime = other.PlayerTeleportFreezeTime;
            PlayerUseRange = other.PlayerUseRange;
            PlayerWeaponSlot = CopyWeaponSlot(other.PlayerWeaponSlot);
            PlayerViewBob = other.PlayerViewBob;
            PlayerViewHeight = other.PlayerViewHeight;
            PoisonDamage = other.PoisonDamage;
            PoisonDamageType = other.PoisonDamageType.Copy();
            PowerupColor = other.PowerupColor;
            PowerupColormap = other.PowerupColormap;
            PowerupDuration = other.PowerupDuration;
            PowerupMode = other.PowerupMode;
            PowerupStrength = other.PowerupStrength;
            PowerupType = other.PowerupType.Copy();
            PowerSpeedNoTrail = other.PowerSpeedNoTrail;
            ProjectileKickBack = other.ProjectileKickBack;
            ProjectilePassHeight = other.ProjectilePassHeight;
            PushFactor = other.PushFactor;
            PuzzleItemNumber = other.PuzzleItemNumber;
            PuzzleItemFailMessage = other.PuzzleItemFailMessage.Copy();
            Radius = other.Radius;
            RadiusDamageFactor = other.RadiusDamageFactor;
            ReactionTime = other.ReactionTime;
            RenderRadius = other.RenderRadius;
            RenderStyle = other.RenderStyle;
            RipLevelMax = other.RipLevelMax;
            RipLevelMin = other.RipLevelMin;
            RipperLevel = other.RipperLevel;
            Scale = other.Scale;
            SeeSound = other.SeeSound.Copy();
            SelfDamageFactor = other.SelfDamageFactor;
            SpawnID = other.SpawnID;
            SpawnInfo = other.SpawnInfo.Map(si => new SpawnInfo(si));
            Species = other.Species.Copy();
            Speed = other.Speed;
            SpriteAngle = other.SpriteAngle;
            SpriteRotation = other.SpriteRotation;
            Stamina = other.Stamina;
            StealthAlpha = other.StealthAlpha;
            StencilColor = other.StencilColor;
            Tag = other.Tag.Copy();
            Translation = new Translation(Translation);
            Threshold = other.Threshold;
            TeleFogDestType = other.TeleFogDestType;
            TeleFogSourceType = other.TeleFogSourceType;
            VisibleAngles = other.VisibleAngles;
            VisiblePitch = other.VisiblePitch;
            VisibleToPlayerClass = new List<UpperString>(other.VisibleToPlayerClass);
            VisibleToTeam = other.VisibleToTeam;
            VSpeed = other.VSpeed;
            WallBounceFactor = other.WallBounceFactor;
            WallBounceSound = other.WallBounceSound.Copy();
            WeaponAmmoGive = other.WeaponAmmoGive;
            WeaponAmmoGive1 = other.WeaponAmmoGive1;
            WeaponAmmoGive2 = other.WeaponAmmoGive2;
            WeaponAmmoType = other.WeaponAmmoType.Copy();
            WeaponAmmoType1 = other.WeaponAmmoType1.Copy();
            WeaponAmmoType2 = other.WeaponAmmoType2.Copy();
            WeaponAmmoUse = other.WeaponAmmoUse;
            WeaponAmmoUse1 = other.WeaponAmmoUse1;
            WeaponAmmoUse2 = other.WeaponAmmoUse2;
            WeaponBobRangeX = other.WeaponBobRangeX;
            WeaponBobRangeY = other.WeaponBobRangeY;
            WeaponBobSpeed = other.WeaponBobSpeed;
            WeaponBobStyle = other.WeaponBobStyle;
            WeaponDefaultKickBack = other.WeaponDefaultKickBack;
            WeaponKickBack = other.WeaponKickBack;
            WeaponLookScale = other.WeaponLookScale;
            WeaponMinSelectionAmmo1 = other.WeaponMinSelectionAmmo1;
            WeaponMinSelectionAmmo2 = other.WeaponMinSelectionAmmo2;
            WeaponReadySound = other.WeaponReadySound.Copy();
            WeaponSelectionOrder = other.WeaponSelectionOrder;
            WeaponSisterWeapon = other.WeaponSisterWeapon.Copy();
            WeaponSlotNumber = other.WeaponSlotNumber;
            WeaponSlotPriority = other.WeaponSlotPriority;
            WeaponUpSound = other.WeaponUpSound.Copy();
            WeaponYAdjust = other.WeaponYAdjust;
            WeaponPieceNumber = other.WeaponPieceNumber;
            WeaponPieceWeapon = other.WeaponPieceWeapon.Copy();
            WeaveIndexXY = other.WeaveIndexXY;
            WeaveIndexZ = other.WeaveIndexZ;
            WoundHealth = other.WoundHealth;
            XScale = other.XScale;
            YScale = other.YScale;
        }

        private static Dictionary<int, List<UpperString>> CopyWeaponSlot(Dictionary<int, List<UpperString>> otherWeaponSlot)
        {
            Dictionary<int, List<UpperString>> dict = new Dictionary<int, List<UpperString>>();
            otherWeaponSlot.ForEach(kv => dict[kv.Key] = new List<UpperString>(kv.Value));
            return dict;
        }
    }
}

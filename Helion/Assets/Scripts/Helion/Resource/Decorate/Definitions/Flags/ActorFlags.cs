using System;
using System.Collections;

namespace Helion.Resource.Decorate.Definitions.Flags
{
    public class ActorFlags
    {
        private static readonly int NumFlags = Enum.GetNames(typeof(ActorFlagType)).Length;

        private readonly BitArray bits;

        public bool AbsMaskAngle => bits[(int)ActorFlagType.AbsMaskAngle];
        public bool AbsMaskPitch => bits[(int)ActorFlagType.AbsMaskPitch];
        public bool ActivateImpact => bits[(int)ActorFlagType.ActivateImpact];
        public bool ActivateMCross => bits[(int)ActorFlagType.ActivateMCross];
        public bool ActivatePCross => bits[(int)ActorFlagType.ActivatePCross];
        public bool ActLikeBridge => bits[(int)ActorFlagType.ActLikeBridge];
        public bool AdditivePoisonDamage => bits[(int)ActorFlagType.AdditivePoisonDamage];
        public bool AdditivePoisonDuration => bits[(int)ActorFlagType.AdditivePoisonDuration];
        public bool AimReflect => bits[(int)ActorFlagType.AimReflect];
        public bool AllowBounceOnActors => bits[(int)ActorFlagType.AllowBounceOnActors];
        public bool AllowPain => bits[(int)ActorFlagType.AllowPain];
        public bool AllowParticles => bits[(int)ActorFlagType.AllowParticles];
        public bool AllowThruFlags => bits[(int)ActorFlagType.AllowThruFlags];
        public bool AlwaysFast => bits[(int)ActorFlagType.AlwaysFast];
        public bool AlwaysPuff => bits[(int)ActorFlagType.AlwaysPuff];
        public bool AlwaysRespawn => bits[(int)ActorFlagType.AlwaysRespawn];
        public bool AlwaysTelefrag => bits[(int)ActorFlagType.AlwaysTelefrag];
        public bool Ambush => bits[(int)ActorFlagType.Ambush];
        public bool AvoidMelee => bits[(int)ActorFlagType.AvoidMelee];
        public bool Blasted => bits[(int)ActorFlagType.Blasted];
        public bool BlockAsPlayer => bits[(int)ActorFlagType.BlockAsPlayer];
        public bool BlockedBySolidActors => bits[(int)ActorFlagType.BlockedBySolidActors];
        public bool BloodlessImpact => bits[(int)ActorFlagType.BloodlessImpact];
        public bool BloodSplatter => bits[(int)ActorFlagType.BloodSplatter];
        public bool Boss => bits[(int)ActorFlagType.Boss];
        public bool BossDeath => bits[(int)ActorFlagType.BossDeath];
        public bool BounceAutoOff => bits[(int)ActorFlagType.BounceAutoOff];
        public bool BounceAutoOffFloorOnly => bits[(int)ActorFlagType.BounceAutoOffFloorOnly];
        public bool BounceLikeHeretic => bits[(int)ActorFlagType.BounceLikeHeretic];
        public bool BounceOnActors => bits[(int)ActorFlagType.BounceOnActors];
        public bool BounceOnCeilings => bits[(int)ActorFlagType.BounceOnCeilings];
        public bool BounceOnFloors => bits[(int)ActorFlagType.BounceOnFloors];
        public bool BounceOnUnrippables => bits[(int)ActorFlagType.BounceOnUnrippables];
        public bool BounceOnWalls => bits[(int)ActorFlagType.BounceOnWalls];
        public bool Bright => bits[(int)ActorFlagType.Bright];
        public bool Buddha => bits[(int)ActorFlagType.Buddha];
        public bool BumpSpecial => bits[(int)ActorFlagType.BumpSpecial];
        public bool CanBlast => bits[(int)ActorFlagType.CanBlast];
        public bool CanBounceWater => bits[(int)ActorFlagType.CanBounceWater];
        public bool CannotPush => bits[(int)ActorFlagType.CannotPush];
        public bool CanPass => bits[(int)ActorFlagType.CanPass];
        public bool CanPushWalls => bits[(int)ActorFlagType.CanPushWalls];
        public bool CantLeaveFloorPic => bits[(int)ActorFlagType.CantLeaveFloorPic];
        public bool CantSeek => bits[(int)ActorFlagType.CantSeek];
        public bool CanUseWalls => bits[(int)ActorFlagType.CanUseWalls];
        public bool CausePain => bits[(int)ActorFlagType.CausePain];
        public bool CeilingHugger => bits[(int)ActorFlagType.CeilingHugger];
        public bool Corpse => bits[(int)ActorFlagType.Corpse];
        public bool CountItem => bits[(int)ActorFlagType.CountItem];
        public bool CountKill => bits[(int)ActorFlagType.CountKill];
        public bool CountSecret => bits[(int)ActorFlagType.CountSecret];
        public bool Deflect => bits[(int)ActorFlagType.Deflect];
        public bool DehExplosion => bits[(int)ActorFlagType.DehExplosion];
        public bool DoHarmSpecies => bits[(int)ActorFlagType.DoHarmSpecies];
        public bool DontBlast => bits[(int)ActorFlagType.DontBlast];
        public bool DontBounceOnShootables => bits[(int)ActorFlagType.DontBounceOnShootables];
        public bool DontBounceOnSky => bits[(int)ActorFlagType.DontBounceOnSky];
        public bool DontCorpse => bits[(int)ActorFlagType.DontCorpse];
        public bool DontDrain => bits[(int)ActorFlagType.DontDrain];
        public bool DontFaceTalker => bits[(int)ActorFlagType.DontFaceTalker];
        public bool DontFall => bits[(int)ActorFlagType.DontFall];
        public bool DontGib => bits[(int)ActorFlagType.DontGib];
        public bool DontHarmClass => bits[(int)ActorFlagType.DontHarmClass];
        public bool DontHarmSpecies => bits[(int)ActorFlagType.DontHarmSpecies];
        public bool DontHurtSpecies => bits[(int)ActorFlagType.DontHurtSpecies];
        public bool DontInterpolate => bits[(int)ActorFlagType.DontInterpolate];
        public bool DontMorph => bits[(int)ActorFlagType.DontMorph];
        public bool DontOverlap => bits[(int)ActorFlagType.DontOverlap];
        public bool DontReflect => bits[(int)ActorFlagType.DontReflect];
        public bool DontRip => bits[(int)ActorFlagType.DontRip];
        public bool DontSeekInvisible => bits[(int)ActorFlagType.DontSeekInvisible];
        public bool DontSplash => bits[(int)ActorFlagType.DontSplash];
        public bool DontSquash => bits[(int)ActorFlagType.DontSquash];
        public bool DontThrust => bits[(int)ActorFlagType.DontThrust];
        public bool DontTranslate => bits[(int)ActorFlagType.DontTranslate];
        public bool DoomBounce => bits[(int)ActorFlagType.DoomBounce];
        public bool Dormant => bits[(int)ActorFlagType.Dormant];
        public bool Dropoff => bits[(int)ActorFlagType.Dropoff];
        public bool Dropped => bits[(int)ActorFlagType.Dropped];
        public bool ExploCount => bits[(int)ActorFlagType.ExploCount];
        public bool ExplodeOnWater => bits[(int)ActorFlagType.ExplodeOnWater];
        public bool ExtremeDeath => bits[(int)ActorFlagType.ExtremeDeath];
        public bool Faster => bits[(int)ActorFlagType.Faster];
        public bool FastMelee => bits[(int)ActorFlagType.FastMelee];
        public bool FireDamage => bits[(int)ActorFlagType.FireDamage];
        public bool FireResist => bits[(int)ActorFlagType.FireResist];
        public bool FixMapThingPos => bits[(int)ActorFlagType.FixMapThingPos];
        public bool FlatSprite => bits[(int)ActorFlagType.FlatSprite];
        public bool Float => bits[(int)ActorFlagType.Float];
        public bool FloatBob => bits[(int)ActorFlagType.FloatBob];
        public bool FloorClip => bits[(int)ActorFlagType.FloorClip];
        public bool FloorHugger => bits[(int)ActorFlagType.FloorHugger];
        public bool FoilBuddha => bits[(int)ActorFlagType.FoilBuddha];
        public bool FoilInvul => bits[(int)ActorFlagType.FoilInvul];
        public bool ForceDecal => bits[(int)ActorFlagType.ForceDecal];
        public bool ForceInFighting => bits[(int)ActorFlagType.ForceInFighting];
        public bool ForcePain => bits[(int)ActorFlagType.ForcePain];
        public bool ForceRadiusDmg => bits[(int)ActorFlagType.ForceRadiusDmg];
        public bool ForceXYBillboard => bits[(int)ActorFlagType.ForceXYBillboard];
        public bool ForceYBillboard => bits[(int)ActorFlagType.ForceYBillboard];
        public bool ForceZeroRadiusDmg => bits[(int)ActorFlagType.ForceZeroRadiusDmg];
        public bool Friendly => bits[(int)ActorFlagType.Friendly];
        public bool Frightened => bits[(int)ActorFlagType.Frightened];
        public bool Frightening => bits[(int)ActorFlagType.Frightening];
        public bool FullVolActive => bits[(int)ActorFlagType.FullVolActive];
        public bool FullVolDeath => bits[(int)ActorFlagType.FullVolDeath];
        public bool GetOwner => bits[(int)ActorFlagType.GetOwner];
        public bool Ghost => bits[(int)ActorFlagType.Ghost];
        public bool GrenadeTrail => bits[(int)ActorFlagType.GrenadeTrail];
        public bool HarmFriends => bits[(int)ActorFlagType.HarmFriends];
        public bool HereticBounce => bits[(int)ActorFlagType.HereticBounce];
        public bool HexenBounce => bits[(int)ActorFlagType.HexenBounce];
        public bool HitMaster => bits[(int)ActorFlagType.HitMaster];
        public bool HitOwner => bits[(int)ActorFlagType.HitOwner];
        public bool HitTarget => bits[(int)ActorFlagType.HitTarget];
        public bool HitTracer => bits[(int)ActorFlagType.HitTracer];
        public bool IceCorpse => bits[(int)ActorFlagType.IceCorpse];
        public bool IceDamage => bits[(int)ActorFlagType.IceDamage];
        public bool IceShatter => bits[(int)ActorFlagType.IceShatter];
        public bool InCombat => bits[(int)ActorFlagType.InCombat];
        public bool InterpolateAngles => bits[(int)ActorFlagType.InterpolateAngles];
        public bool InventoryAdditiveTime => bits[(int)ActorFlagType.InventoryAdditiveTime];
        public bool InventoryAlwaysPickup => bits[(int)ActorFlagType.InventoryAlwaysPickup];
        public bool InventoryAlwaysRespawn => bits[(int)ActorFlagType.InventoryAlwaysRespawn];
        public bool InventoryAutoActivate => bits[(int)ActorFlagType.InventoryAutoActivate];
        public bool InventoryBigPowerup => bits[(int)ActorFlagType.InventoryBigPowerup];
        public bool InventoryFancyPickupSound => bits[(int)ActorFlagType.InventoryFancyPickupSound];
        public bool InventoryHubPower => bits[(int)ActorFlagType.InventoryHubPower];
        public bool InventoryIgnoreSkill => bits[(int)ActorFlagType.InventoryIgnoreSkill];
        public bool InventoryInterHubStrip => bits[(int)ActorFlagType.InventoryInterHubStrip];
        public bool InventoryInvbar => bits[(int)ActorFlagType.InventoryInvbar];
        public bool InventoryIsArmor => bits[(int)ActorFlagType.InventoryIsArmor];
        public bool InventoryIsHealth => bits[(int)ActorFlagType.InventoryIsHealth];
        public bool InventoryKeepDepleted => bits[(int)ActorFlagType.InventoryKeepDepleted];
        public bool InventoryNeverRespawn => bits[(int)ActorFlagType.InventoryNeverRespawn];
        public bool InventoryNoAttenPickupSound => bits[(int)ActorFlagType.InventoryNoAttenPickupSound];
        public bool InventoryNoScreenBlink => bits[(int)ActorFlagType.InventoryNoScreenBlink];
        public bool InventoryNoScreenFlash => bits[(int)ActorFlagType.InventoryNoScreenFlash];
        public bool InventoryNoTeleportFreeze => bits[(int)ActorFlagType.InventoryNoTeleportFreeze];
        public bool InventoryPersistentPower => bits[(int)ActorFlagType.InventoryPersistentPower];
        public bool InventoryPickupFlash => bits[(int)ActorFlagType.InventoryPickupFlash];
        public bool InventoryQuiet => bits[(int)ActorFlagType.InventoryQuiet];
        public bool InventoryRestrictAbsolutely => bits[(int)ActorFlagType.InventoryRestrictAbsolutely];
        public bool InventoryTossed => bits[(int)ActorFlagType.InventoryTossed];
        public bool InventoryTransfer => bits[(int)ActorFlagType.InventoryTransfer];
        public bool InventoryUnclearable => bits[(int)ActorFlagType.InventoryUnclearable];
        public bool InventoryUndroppable => bits[(int)ActorFlagType.InventoryUndroppable];
        public bool InventoryUntossable => bits[(int)ActorFlagType.InventoryUntossable];
        public bool Invisible => bits[(int)ActorFlagType.Invisible];
        public bool Invulnerable => bits[(int)ActorFlagType.Invulnerable];
        public bool IsMonster => bits[(int)ActorFlagType.IsMonster];
        public bool IsTeleportSpot => bits[(int)ActorFlagType.IsTeleportSpot];
        public bool JumpDown => bits[(int)ActorFlagType.JumpDown];
        public bool JustAttacked => bits[(int)ActorFlagType.JustAttacked];
        public bool JustHit => bits[(int)ActorFlagType.JustHit];
        public bool LaxTeleFragDmg => bits[(int)ActorFlagType.LaxTeleFragDmg];
        public bool LongMeleeRange => bits[(int)ActorFlagType.LongMeleeRange];
        public bool LookAllAround => bits[(int)ActorFlagType.LookAllAround];
        public bool LowGravity => bits[(int)ActorFlagType.LowGravity];
        public bool MaskRotation => bits[(int)ActorFlagType.MaskRotation];
        public bool MbfBouncer => bits[(int)ActorFlagType.MbfBouncer];
        public bool MirrorReflect => bits[(int)ActorFlagType.MirrorReflect];
        public bool Missile => bits[(int)ActorFlagType.Missile];
        public bool MissileEvenMore => bits[(int)ActorFlagType.MissileEvenMore];
        public bool MissileMore => bits[(int)ActorFlagType.MissileMore];
        public bool Monster => bits[(int)ActorFlagType.Monster];
        public bool MoveWithSector => bits[(int)ActorFlagType.MoveWithSector];
        public bool MThruSpecies => bits[(int)ActorFlagType.MThruSpecies];
        public bool NeverFast => bits[(int)ActorFlagType.NeverFast];
        public bool NeverRespawn => bits[(int)ActorFlagType.NeverRespawn];
        public bool NeverTarget => bits[(int)ActorFlagType.NeverTarget];
        public bool NoBlockmap => bits[(int)ActorFlagType.NoBlockmap];
        public bool NoBlockMonst => bits[(int)ActorFlagType.NoBlockMonst];
        public bool NoBlood => bits[(int)ActorFlagType.NoBlood];
        public bool NoBloodDecals => bits[(int)ActorFlagType.NoBloodDecals];
        public bool NoBossRip => bits[(int)ActorFlagType.NoBossRip];
        public bool NoBounceSound => bits[(int)ActorFlagType.NoBounceSound];
        public bool NoClip => bits[(int)ActorFlagType.NoClip];
        public bool NoDamage => bits[(int)ActorFlagType.NoDamage];
        public bool NoDamageThrust => bits[(int)ActorFlagType.NoDamageThrust];
        public bool NoDecal => bits[(int)ActorFlagType.NoDecal];
        public bool NoDropoff => bits[(int)ActorFlagType.NoDropoff];
        public bool NoExplodeFloor => bits[(int)ActorFlagType.NoExplodeFloor];
        public bool NoExtremeDeath => bits[(int)ActorFlagType.NoExtremeDeath];
        public bool NoFear => bits[(int)ActorFlagType.NoFear];
        public bool NoFriction => bits[(int)ActorFlagType.NoFriction];
        public bool NoFrictionBounce => bits[(int)ActorFlagType.NoFrictionBounce];
        public bool NoForwardFall => bits[(int)ActorFlagType.NoForwardFall];
        public bool NoGravity => bits[(int)ActorFlagType.NoGravity];
        public bool NoIceDeath => bits[(int)ActorFlagType.NoIceDeath];
        public bool NoInfighting => bits[(int)ActorFlagType.NoInfighting];
        public bool NoInfightSpecies => bits[(int)ActorFlagType.NoInfightSpecies];
        public bool NoInteraction => bits[(int)ActorFlagType.NoInteraction];
        public bool NoKillScripts => bits[(int)ActorFlagType.NoKillScripts];
        public bool NoLiftDrop => bits[(int)ActorFlagType.NoLiftDrop];
        public bool NoMenu => bits[(int)ActorFlagType.NoMenu];
        public bool NonShootable => bits[(int)ActorFlagType.NonShootable];
        public bool NoPain => bits[(int)ActorFlagType.NoPain];
        public bool NoRadiusDmg => bits[(int)ActorFlagType.NoRadiusDmg];
        public bool NoSector => bits[(int)ActorFlagType.NoSector];
        public bool NoSkin => bits[(int)ActorFlagType.NoSkin];
        public bool NoSplashAlert => bits[(int)ActorFlagType.NoSplashAlert];
        public bool NoTarget => bits[(int)ActorFlagType.NoTarget];
        public bool NoTargetSwitch => bits[(int)ActorFlagType.NoTargetSwitch];
        public bool NotAutoaimed => bits[(int)ActorFlagType.NotAutoaimed];
        public bool NotDMatch => bits[(int)ActorFlagType.NotDMatch];
        public bool NoTelefrag => bits[(int)ActorFlagType.NoTelefrag];
        public bool NoTeleOther => bits[(int)ActorFlagType.NoTeleOther];
        public bool NoTeleport => bits[(int)ActorFlagType.NoTeleport];
        public bool NoTelestomp => bits[(int)ActorFlagType.NoTelestomp];
        public bool NoTimeFreeze => bits[(int)ActorFlagType.NoTimeFreeze];
        public bool NotOnAutomap => bits[(int)ActorFlagType.NotOnAutomap];
        public bool NoTrigger => bits[(int)ActorFlagType.NoTrigger];
        public bool NoVerticalMeleeRange => bits[(int)ActorFlagType.NoVerticalMeleeRange];
        public bool NoWallBounceSnd => bits[(int)ActorFlagType.NoWallBounceSnd];
        public bool OldRadiusDmg => bits[(int)ActorFlagType.OldRadiusDmg];
        public bool Painless => bits[(int)ActorFlagType.Painless];
        public bool Pickup => bits[(int)ActorFlagType.Pickup];
        public bool PierceArmor => bits[(int)ActorFlagType.PierceArmor];
        public bool PlayerPawnCanSuperMorph => bits[(int)ActorFlagType.PlayerPawnCanSuperMorph];
        public bool PlayerPawnCrouchableMorph => bits[(int)ActorFlagType.PlayerPawnCrouchableMorph];
        public bool PlayerPawnNoThrustWhenInvul => bits[(int)ActorFlagType.PlayerPawnNoThrustWhenInvul];
        public bool PoisonAlways => bits[(int)ActorFlagType.PoisonAlways];
        public bool Projectile => bits[(int)ActorFlagType.Projectile];
        public bool PuffGetsOwner => bits[(int)ActorFlagType.PuffGetsOwner];
        public bool PuffOnActors => bits[(int)ActorFlagType.PuffOnActors];
        public bool Pushable => bits[(int)ActorFlagType.Pushable];
        public bool QuarterGravity => bits[(int)ActorFlagType.QuarterGravity];
        public bool QuickToRetaliate => bits[(int)ActorFlagType.QuickToRetaliate];
        public bool Randomize => bits[(int)ActorFlagType.Randomize];
        public bool Reflective => bits[(int)ActorFlagType.Reflective];
        public bool RelativeToFloor => bits[(int)ActorFlagType.RelativeToFloor];
        public bool Ripper => bits[(int)ActorFlagType.Ripper];
        public bool RocketTrail => bits[(int)ActorFlagType.RocketTrail];
        public bool RollCenter => bits[(int)ActorFlagType.RollCenter];
        public bool RollSprite => bits[(int)ActorFlagType.RollSprite];
        public bool ScreenSeeker => bits[(int)ActorFlagType.ScreenSeeker];
        public bool SeeInvisible => bits[(int)ActorFlagType.SeeInvisible];
        public bool SeekerMissile => bits[(int)ActorFlagType.SeekerMissile];
        public bool SeesDaggers => bits[(int)ActorFlagType.SeesDaggers];
        public bool Shadow => bits[(int)ActorFlagType.Shadow];
        public bool ShieldReflect => bits[(int)ActorFlagType.ShieldReflect];
        public bool Shootable => bits[(int)ActorFlagType.Shootable];
        public bool ShortMissileRange => bits[(int)ActorFlagType.ShortMissileRange];
        public bool Skullfly => bits[(int)ActorFlagType.Skullfly];
        public bool SkyExplode => bits[(int)ActorFlagType.SkyExplode];
        public bool SlidesOnWalls => bits[(int)ActorFlagType.SlidesOnWalls];
        public bool Solid => bits[(int)ActorFlagType.Solid];
        public bool SpawnCeiling => bits[(int)ActorFlagType.SpawnCeiling];
        public bool SpawnFloat => bits[(int)ActorFlagType.SpawnFloat];
        public bool SpawnSoundSource => bits[(int)ActorFlagType.SpawnSoundSource];
        public bool Special => bits[(int)ActorFlagType.Special];
        public bool SpecialFireDamage => bits[(int)ActorFlagType.SpecialFireDamage];
        public bool SpecialFloorClip => bits[(int)ActorFlagType.SpecialFloorClip];
        public bool Spectral => bits[(int)ActorFlagType.Spectral];
        public bool SpriteAngle => bits[(int)ActorFlagType.SpriteAngle];
        public bool SpriteFlip => bits[(int)ActorFlagType.SpriteFlip];
        public bool StandStill => bits[(int)ActorFlagType.StandStill];
        public bool StayMorphed => bits[(int)ActorFlagType.StayMorphed];
        public bool Stealth => bits[(int)ActorFlagType.Stealth];
        public bool StepMissile => bits[(int)ActorFlagType.StepMissile];
        public bool StrifeDamage => bits[(int)ActorFlagType.StrifeDamage];
        public bool SummonedMonster => bits[(int)ActorFlagType.SummonedMonster];
        public bool Synchronized => bits[(int)ActorFlagType.Synchronized];
        public bool Teleport => bits[(int)ActorFlagType.Teleport];
        public bool Telestomp => bits[(int)ActorFlagType.Telestomp];
        public bool ThruActors => bits[(int)ActorFlagType.ThruActors];
        public bool ThruGhost => bits[(int)ActorFlagType.ThruGhost];
        public bool ThruReflect => bits[(int)ActorFlagType.ThruReflect];
        public bool ThruSpecies => bits[(int)ActorFlagType.ThruSpecies];
        public bool Touchy => bits[(int)ActorFlagType.Touchy];
        public bool UseBounceState => bits[(int)ActorFlagType.UseBounceState];
        public bool UseKillScripts => bits[(int)ActorFlagType.UseKillScripts];
        public bool UseSpecial => bits[(int)ActorFlagType.UseSpecial];
        public bool VisibilityPulse => bits[(int)ActorFlagType.VisibilityPulse];
        public bool Vulnerable => bits[(int)ActorFlagType.Vulnerable];
        public bool WallSprite => bits[(int)ActorFlagType.WallSprite];
        public bool WeaponAltAmmoOptional => bits[(int)ActorFlagType.WeaponAltAmmoOptional];
        public bool WeaponAltUsesBoth => bits[(int)ActorFlagType.WeaponAltUsesBoth];
        public bool WeaponAmmoCheckBoth => bits[(int)ActorFlagType.WeaponAmmoCheckBoth];
        public bool WeaponAmmoOptional => bits[(int)ActorFlagType.WeaponAmmoOptional];
        public bool WeaponAxeBlood => bits[(int)ActorFlagType.WeaponAxeBlood];
        public bool WeaponBfg => bits[(int)ActorFlagType.WeaponBfg];
        public bool WeaponCheatNotWeapon => bits[(int)ActorFlagType.WeaponCheatNotWeapon];
        public bool WeaponDontBob => bits[(int)ActorFlagType.WeaponDontBob];
        public bool WeaponExplosive => bits[(int)ActorFlagType.WeaponExplosive];
        public bool WeaponMeleeWeapon => bits[(int)ActorFlagType.WeaponMeleeWeapon];
        public bool WeaponNoAlert => bits[(int)ActorFlagType.WeaponNoAlert];
        public bool WeaponNoAutoaim => bits[(int)ActorFlagType.WeaponNoAutoaim];
        public bool WeaponNoAutofire => bits[(int)ActorFlagType.WeaponNoAutofire];
        public bool WeaponNoDeathDeselect => bits[(int)ActorFlagType.WeaponNoDeathDeselect];
        public bool WeaponNoDeathInput => bits[(int)ActorFlagType.WeaponNoDeathInput];
        public bool WeaponNoAutoSwitch => bits[(int)ActorFlagType.WeaponNoAutoSwitch];
        public bool WeaponPoweredUp => bits[(int)ActorFlagType.WeaponPoweredUp];
        public bool WeaponPrimaryUsesBoth => bits[(int)ActorFlagType.WeaponPrimaryUsesBoth];
        public bool WeaponReadySndHalf => bits[(int)ActorFlagType.WeaponReadySndHalf];
        public bool WeaponStaff2Kickback => bits[(int)ActorFlagType.WeaponStaff2Kickback];
        public bool WeaponSpawn => bits[(int)ActorFlagType.WeaponSpawn];
        public bool WeaponWimpyWeapon => bits[(int)ActorFlagType.WeaponWimpyWeapon];
        public bool WindThrust => bits[(int)ActorFlagType.WindThrust];
        public bool ZdoomTrans => bits[(int)ActorFlagType.ZdoomTrans];

        /// <summary>
        /// Creates an empty flags object where no flags are set.
        /// </summary>
        public ActorFlags()
        {
            bits = new BitArray(NumFlags);
        }

        /// <summary>
        /// Copies the existing flags into a new object.
        /// </summary>
        /// <param name="other">The flags to copy from.</param>
        public ActorFlags(ActorFlags other)
        {
            bits = new BitArray(other.bits);
        }

        /// <summary>
        /// Sets the flag.
        /// </summary>
        /// <param name="flag">The flag to set.</param>
        /// <param name="value">The value to set on the flag.</param>
        public void Set(ActorFlagType flag, bool value)
        {
            bits.Set((int)flag, value);
        }

        /// <summary>
        /// Clears all the flags.
        /// </summary>
        public void ClearAll()
        {
            bits.SetAll(false);
        }
    }
}

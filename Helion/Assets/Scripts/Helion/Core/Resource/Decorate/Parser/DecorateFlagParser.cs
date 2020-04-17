using Helion.Core.Resource.Decorate.Definitions.Flags;

namespace Helion.Core.Resource.Decorate.Parser
{
    /// <summary>
    /// Handles parsing the flag components of an actor.
    /// </summary>
    public partial class DecorateParser
    {
        private void ConsumeActorFlag()
        {
            bool flagValue = false;
            if (ConsumeIf('+'))
                flagValue = true;
            else
                Consume('-');

            string flagName = ConsumeIdentifier();

            if (ConsumeIf('.'))
            {
                switch (flagName.ToUpper())
                {
                case "INVENTORY":
                    SetInventoryFlag(flagValue);
                    break;
                case "PLAYERPAWN":
                    SetPlayerPawnFlag(flagValue);
                    break;
                case "WEAPON":
                    SetWeaponFlag(flagValue);
                    break;
                default:
                    Log.Warn("Unknown flag prefix '{0}' for actor {1}", flagName, currentDefinition.Name);
                    break;
                }
            }
            else
                SetTopLevelFlag(flagName, flagValue);
        }

        private void SetInventoryFlag(bool flagValue)
        {
            string nestedFlag = ConsumeIdentifier();
            switch (nestedFlag.ToUpper())
            {
            case "ADDITIVETIME":
                currentDefinition.Flags.Set(ActorFlagType.InventoryAdditiveTime, flagValue);
                break;
            case "ALWAYSPICKUP":
                currentDefinition.Flags.Set(ActorFlagType.InventoryAlwaysPickup, flagValue);
                break;
            case "ALWAYSRESPAWN":
                currentDefinition.Flags.Set(ActorFlagType.InventoryAlwaysRespawn, flagValue);
                break;
            case "AUTOACTIVATE":
                currentDefinition.Flags.Set(ActorFlagType.InventoryAutoActivate, flagValue);
                break;
            case "BIGPOWERUP":
                currentDefinition.Flags.Set(ActorFlagType.InventoryBigPowerup, flagValue);
                break;
            case "FANCYPICKUPSOUND":
                currentDefinition.Flags.Set(ActorFlagType.InventoryFancyPickupSound, flagValue);
                break;
            case "HUBPOWER":
                currentDefinition.Flags.Set(ActorFlagType.InventoryHubPower, flagValue);
                break;
            case "IGNORESKILL":
                currentDefinition.Flags.Set(ActorFlagType.InventoryIgnoreSkill, flagValue);
                break;
            case "INTERHUBSTRIP":
                currentDefinition.Flags.Set(ActorFlagType.InventoryInterHubStrip, flagValue);
                break;
            case "INVBAR":
                currentDefinition.Flags.Set(ActorFlagType.InventoryInvbar, flagValue);
                break;
            case "ISARMOR":
                currentDefinition.Flags.Set(ActorFlagType.InventoryIsArmor, flagValue);
                break;
            case "ISHEALTH":
                currentDefinition.Flags.Set(ActorFlagType.InventoryIsHealth, flagValue);
                break;
            case "KEEPDEPLETED":
                currentDefinition.Flags.Set(ActorFlagType.InventoryKeepDepleted, flagValue);
                break;
            case "NEVERRESPAWN":
                currentDefinition.Flags.Set(ActorFlagType.InventoryNeverRespawn, flagValue);
                break;
            case "NOATTENPICKUPSOUND":
                currentDefinition.Flags.Set(ActorFlagType.InventoryNoAttenPickupSound, flagValue);
                break;
            case "NOSCREENBLINK":
                currentDefinition.Flags.Set(ActorFlagType.InventoryNoScreenBlink, flagValue);
                break;
            case "NOSCREENFLASH":
                currentDefinition.Flags.Set(ActorFlagType.InventoryNoScreenFlash, flagValue);
                break;
            case "NOTELEPORTFREEZE":
                currentDefinition.Flags.Set(ActorFlagType.InventoryNoTeleportFreeze, flagValue);
                break;
            case "PERSISTENTPOWER":
                currentDefinition.Flags.Set(ActorFlagType.InventoryPersistentPower, flagValue);
                break;
            case "PICKUPFLASH":
                currentDefinition.Flags.Set(ActorFlagType.InventoryPickupFlash, flagValue);
                break;
            case "QUIET":
                currentDefinition.Flags.Set(ActorFlagType.InventoryQuiet, flagValue);
                break;
            case "RESTRICTABSOLUTELY":
                currentDefinition.Flags.Set(ActorFlagType.InventoryRestrictAbsolutely, flagValue);
                break;
            case "TOSSED":
                currentDefinition.Flags.Set(ActorFlagType.InventoryTossed, flagValue);
                break;
            case "TRANSFER":
                currentDefinition.Flags.Set(ActorFlagType.InventoryTransfer, flagValue);
                break;
            case "UNCLEARABLE":
                currentDefinition.Flags.Set(ActorFlagType.InventoryUnclearable, flagValue);
                break;
            case "UNDROPPABLE":
                currentDefinition.Flags.Set(ActorFlagType.InventoryUndroppable, flagValue);
                break;
            case "UNTOSSABLE":
                currentDefinition.Flags.Set(ActorFlagType.InventoryUntossable, flagValue);
                break;
            default:
                Log.Warn("Unknown inventory flag suffix '{0}' for actor {1} (in particular, INVENTORY.{2})", nestedFlag, currentDefinition.Name);
                break;
            }
        }

        private void SetPlayerPawnFlag(bool flagValue)
        {
            string nestedFlag = ConsumeIdentifier();
            switch (nestedFlag.ToUpper())
            {
            case "CANSUPERMORPH":
                currentDefinition.Flags.Set(ActorFlagType.PlayerPawnCanSuperMorph, flagValue);
                break;
            case "CROUCHABLEMORPH":
                currentDefinition.Flags.Set(ActorFlagType.PlayerPawnCrouchableMorph, flagValue);
                break;
            case "NOTHRUSTWHENINVUL":
                currentDefinition.Flags.Set(ActorFlagType.PlayerPawnNoThrustWhenInvul, flagValue);
                break;
            default:
                Log.Warn("Unknown playerpawn flag suffix '{0}' for actor {1} (in particular, PLAYERPAWN.{2})", nestedFlag, currentDefinition.Name);
                break;
            }
        }

        private void SetWeaponFlag(bool flagValue)
        {
            string nestedFlag = ConsumeIdentifier();
            switch (nestedFlag.ToUpper())
            {
            case "ALTAMMOOPTIONAL":
                currentDefinition.Flags.Set(ActorFlagType.WeaponAltAmmoOptional, flagValue);
                break;
            case "ALTUSESBOTH":
                currentDefinition.Flags.Set(ActorFlagType.WeaponAltUsesBoth, flagValue);
                break;
            case "AMMOCHECKBOTH":
                currentDefinition.Flags.Set(ActorFlagType.WeaponAmmoCheckBoth, flagValue);
                break;
            case "AMMOOPTIONAL":
                currentDefinition.Flags.Set(ActorFlagType.WeaponAmmoOptional, flagValue);
                break;
            case "AXEBLOOD":
                currentDefinition.Flags.Set(ActorFlagType.WeaponAxeBlood, flagValue);
                break;
            case "BFG":
                currentDefinition.Flags.Set(ActorFlagType.WeaponBfg, flagValue);
                break;
            case "CHEATNOTWEAPON":
                currentDefinition.Flags.Set(ActorFlagType.WeaponCheatNotWeapon, flagValue);
                break;
            case "DONTBOB":
                currentDefinition.Flags.Set(ActorFlagType.WeaponDontBob, flagValue);
                break;
            case "EXPLOSIVE":
                currentDefinition.Flags.Set(ActorFlagType.WeaponExplosive, flagValue);
                break;
            case "MELEEWEAPON":
                currentDefinition.Flags.Set(ActorFlagType.WeaponMeleeWeapon, flagValue);
                break;
            case "NOALERT":
                currentDefinition.Flags.Set(ActorFlagType.WeaponNoAlert, flagValue);
                break;
            case "NOAUTOAIM":
                currentDefinition.Flags.Set(ActorFlagType.WeaponNoAutoaim, flagValue);
                break;
            case "NOAUTOFIRE":
                currentDefinition.Flags.Set(ActorFlagType.WeaponNoAutofire, flagValue);
                break;
            case "NODEATHDESELECT":
                currentDefinition.Flags.Set(ActorFlagType.WeaponNoDeathDeselect, flagValue);
                break;
            case "NODEATHINPUT":
                currentDefinition.Flags.Set(ActorFlagType.WeaponNoDeathInput, flagValue);
                break;
            case "NOAUTOSWITCH":
                currentDefinition.Flags.Set(ActorFlagType.WeaponNoAutoSwitch, flagValue);
                break;
            case "POWEREDUP":
                currentDefinition.Flags.Set(ActorFlagType.WeaponPoweredUp, flagValue);
                break;
            case "PRIMARYUSESBOTH":
                currentDefinition.Flags.Set(ActorFlagType.WeaponPrimaryUsesBoth, flagValue);
                break;
            case "READYSNDHALF":
                currentDefinition.Flags.Set(ActorFlagType.WeaponReadySndHalf, flagValue);
                break;
            case "STAFF2KICKBACK":
                currentDefinition.Flags.Set(ActorFlagType.WeaponStaff2Kickback, flagValue);
                break;
            case "SPAWN":
                currentDefinition.Flags.Set(ActorFlagType.WeaponSpawn, flagValue);
                break;
            case "WIMPY_WEAPON":
                currentDefinition.Flags.Set(ActorFlagType.WeaponWimpyWeapon, flagValue);
                break;
            default:
                Log.Warn("Unknown weapon flag suffix '{0}' for actor '{1}'", nestedFlag, currentDefinition.Name);
                break;
            }
        }

        private void SetTopLevelFlag(string flagName, bool flagValue)
        {
            switch (flagName.ToUpper())
            {
            case "ABSMASKANGLE":
                currentDefinition.Flags.Set(ActorFlagType.AbsMaskAngle, flagValue);
                break;
            case "ABSMASKPITCH":
                currentDefinition.Flags.Set(ActorFlagType.AbsMaskPitch, flagValue);
                break;
            case "ACTIVATEIMPACT":
                currentDefinition.Flags.Set(ActorFlagType.ActivateImpact, flagValue);
                break;
            case "ACTIVATEMCROSS":
                currentDefinition.Flags.Set(ActorFlagType.ActivateMCross, flagValue);
                break;
            case "ACTIVATEPCROSS":
                currentDefinition.Flags.Set(ActorFlagType.ActivatePCross, flagValue);
                break;
            case "ACTLIKEBRIDGE":
                currentDefinition.Flags.Set(ActorFlagType.ActLikeBridge, flagValue);
                break;
            case "ADDITIVEPOISONDAMAGE":
                currentDefinition.Flags.Set(ActorFlagType.AdditivePoisonDamage, flagValue);
                break;
            case "ADDITIVEPOISONDURATION":
                currentDefinition.Flags.Set(ActorFlagType.AdditivePoisonDuration, flagValue);
                break;
            case "AIMREFLECT":
                currentDefinition.Flags.Set(ActorFlagType.AimReflect, flagValue);
                break;
            case "ALLOWBOUNCEONACTORS":
                currentDefinition.Flags.Set(ActorFlagType.AllowBounceOnActors, flagValue);
                break;
            case "ALLOWPAIN":
                currentDefinition.Flags.Set(ActorFlagType.AllowPain, flagValue);
                break;
            case "ALLOWPARTICLES":
                currentDefinition.Flags.Set(ActorFlagType.AllowParticles, flagValue);
                break;
            case "ALLOWTHRUFLAGS":
                currentDefinition.Flags.Set(ActorFlagType.AllowThruFlags, flagValue);
                break;
            case "ALWAYSFAST":
                currentDefinition.Flags.Set(ActorFlagType.AlwaysFast, flagValue);
                break;
            case "ALWAYSPUFF":
                currentDefinition.Flags.Set(ActorFlagType.AlwaysPuff, flagValue);
                break;
            case "ALWAYSRESPAWN":
                currentDefinition.Flags.Set(ActorFlagType.AlwaysRespawn, flagValue);
                break;
            case "ALWAYSTELEFRAG":
                currentDefinition.Flags.Set(ActorFlagType.AlwaysTelefrag, flagValue);
                break;
            case "AMBUSH":
                currentDefinition.Flags.Set(ActorFlagType.Ambush, flagValue);
                break;
            case "AVOIDMELEE":
                currentDefinition.Flags.Set(ActorFlagType.AvoidMelee, flagValue);
                break;
            case "BLASTED":
                currentDefinition.Flags.Set(ActorFlagType.Blasted, flagValue);
                break;
            case "BLOCKASPLAYER":
                currentDefinition.Flags.Set(ActorFlagType.BlockAsPlayer, flagValue);
                break;
            case "BLOCKEDBYSOLIDACTORS":
                currentDefinition.Flags.Set(ActorFlagType.BlockedBySolidActors, flagValue);
                break;
            case "BLOODLESSIMPACT":
                currentDefinition.Flags.Set(ActorFlagType.BloodlessImpact, flagValue);
                break;
            case "BLOODSPLATTER":
                currentDefinition.Flags.Set(ActorFlagType.BloodSplatter, flagValue);
                break;
            case "BOSS":
                currentDefinition.Flags.Set(ActorFlagType.Boss, flagValue);
                break;
            case "BOSSDEATH":
                currentDefinition.Flags.Set(ActorFlagType.BossDeath, flagValue);
                break;
            case "BOUNCEAUTOOFF":
                currentDefinition.Flags.Set(ActorFlagType.BounceAutoOff, flagValue);
                break;
            case "BOUNCEAUTOOFFFLOORONLY":
                currentDefinition.Flags.Set(ActorFlagType.BounceAutoOffFloorOnly, flagValue);
                break;
            case "BOUNCELIKEHERETIC":
                currentDefinition.Flags.Set(ActorFlagType.BounceLikeHeretic, flagValue);
                break;
            case "BOUNCEONACTORS":
                currentDefinition.Flags.Set(ActorFlagType.BounceOnActors, flagValue);
                break;
            case "BOUNCEONCEILINGS":
                currentDefinition.Flags.Set(ActorFlagType.BounceOnCeilings, flagValue);
                break;
            case "BOUNCEONFLOORS":
                currentDefinition.Flags.Set(ActorFlagType.BounceOnFloors, flagValue);
                break;
            case "BOUNCEONUNRIPPABLES":
                currentDefinition.Flags.Set(ActorFlagType.BounceOnUnrippables, flagValue);
                break;
            case "BOUNCEONWALLS":
                currentDefinition.Flags.Set(ActorFlagType.BounceOnWalls, flagValue);
                break;
            case "BRIGHT":
                currentDefinition.Flags.Set(ActorFlagType.Bright, flagValue);
                break;
            case "BUDDHA":
                currentDefinition.Flags.Set(ActorFlagType.Buddha, flagValue);
                break;
            case "BUMPSPECIAL":
                currentDefinition.Flags.Set(ActorFlagType.BumpSpecial, flagValue);
                break;
            case "CANBLAST":
                currentDefinition.Flags.Set(ActorFlagType.CanBlast, flagValue);
                break;
            case "CANBOUNCEWATER":
                currentDefinition.Flags.Set(ActorFlagType.CanBounceWater, flagValue);
                break;
            case "CANNOTPUSH":
                currentDefinition.Flags.Set(ActorFlagType.CannotPush, flagValue);
                break;
            case "CANPASS":
                currentDefinition.Flags.Set(ActorFlagType.CanPass, flagValue);
                break;
            case "CANPUSHWALLS":
                currentDefinition.Flags.Set(ActorFlagType.CanPushWalls, flagValue);
                break;
            case "CANTLEAVEFLOORPIC":
                currentDefinition.Flags.Set(ActorFlagType.CantLeaveFloorPic, flagValue);
                break;
            case "CANTSEEK":
                currentDefinition.Flags.Set(ActorFlagType.CantSeek, flagValue);
                break;
            case "CANUSEWALLS":
                currentDefinition.Flags.Set(ActorFlagType.CanUseWalls, flagValue);
                break;
            case "CAUSEPAIN":
                currentDefinition.Flags.Set(ActorFlagType.CausePain, flagValue);
                break;
            case "CEILINGHUGGER":
                currentDefinition.Flags.Set(ActorFlagType.CeilingHugger, flagValue);
                break;
            case "CORPSE":
                currentDefinition.Flags.Set(ActorFlagType.Corpse, flagValue);
                break;
            case "COUNTITEM":
                currentDefinition.Flags.Set(ActorFlagType.CountItem, flagValue);
                break;
            case "COUNTKILL":
                currentDefinition.Flags.Set(ActorFlagType.CountKill, flagValue);
                break;
            case "COUNTSECRET":
                currentDefinition.Flags.Set(ActorFlagType.CountSecret, flagValue);
                break;
            case "DEFLECT":
                currentDefinition.Flags.Set(ActorFlagType.Deflect, flagValue);
                break;
            case "DEHEXPLOSION":
                currentDefinition.Flags.Set(ActorFlagType.DehExplosion, flagValue);
                break;
            case "DOHARMSPECIES":
                currentDefinition.Flags.Set(ActorFlagType.DoHarmSpecies, flagValue);
                break;
            case "DONTBLAST":
                currentDefinition.Flags.Set(ActorFlagType.DontBlast, flagValue);
                break;
            case "DONTBOUNCEONSHOOTABLES":
                currentDefinition.Flags.Set(ActorFlagType.DontBounceOnShootables, flagValue);
                break;
            case "DONTBOUNCEONSKY":
                currentDefinition.Flags.Set(ActorFlagType.DontBounceOnSky, flagValue);
                break;
            case "DONTCORPSE":
                currentDefinition.Flags.Set(ActorFlagType.DontCorpse, flagValue);
                break;
            case "DONTDRAIN":
                currentDefinition.Flags.Set(ActorFlagType.DontDrain, flagValue);
                break;
            case "DONTFACETALKER":
                currentDefinition.Flags.Set(ActorFlagType.DontFaceTalker, flagValue);
                break;
            case "DONTFALL":
                currentDefinition.Flags.Set(ActorFlagType.DontFall, flagValue);
                break;
            case "DONTGIB":
                currentDefinition.Flags.Set(ActorFlagType.DontGib, flagValue);
                break;
            case "DONTHARMCLASS":
                currentDefinition.Flags.Set(ActorFlagType.DontHarmClass, flagValue);
                break;
            case "DONTHARMSPECIES":
                currentDefinition.Flags.Set(ActorFlagType.DontHarmSpecies, flagValue);
                break;
            case "DONTHURTSPECIES":
                currentDefinition.Flags.Set(ActorFlagType.DontHurtSpecies, flagValue);
                break;
            case "DONTINTERPOLATE":
                currentDefinition.Flags.Set(ActorFlagType.DontInterpolate, flagValue);
                break;
            case "DONTMORPH":
                currentDefinition.Flags.Set(ActorFlagType.DontMorph, flagValue);
                break;
            case "DONTOVERLAP":
                currentDefinition.Flags.Set(ActorFlagType.DontOverlap, flagValue);
                break;
            case "DONTREFLECT":
                currentDefinition.Flags.Set(ActorFlagType.DontReflect, flagValue);
                break;
            case "DONTRIP":
                currentDefinition.Flags.Set(ActorFlagType.DontRip, flagValue);
                break;
            case "DONTSEEKINVISIBLE":
                currentDefinition.Flags.Set(ActorFlagType.DontSeekInvisible, flagValue);
                break;
            case "DONTSPLASH":
                currentDefinition.Flags.Set(ActorFlagType.DontSplash, flagValue);
                break;
            case "DONTSQUASH":
                currentDefinition.Flags.Set(ActorFlagType.DontSquash, flagValue);
                break;
            case "DONTTHRUST":
                currentDefinition.Flags.Set(ActorFlagType.DontThrust, flagValue);
                break;
            case "DONTTRANSLATE":
                currentDefinition.Flags.Set(ActorFlagType.DontTranslate, flagValue);
                break;
            case "DOOMBOUNCE":
                currentDefinition.Flags.Set(ActorFlagType.DoomBounce, flagValue);
                break;
            case "DORMANT":
                currentDefinition.Flags.Set(ActorFlagType.Dormant, flagValue);
                break;
            case "DROPOFF":
                currentDefinition.Flags.Set(ActorFlagType.Dropoff, flagValue);
                break;
            case "DROPPED":
                currentDefinition.Flags.Set(ActorFlagType.Dropped, flagValue);
                break;
            case "EXPLOCOUNT":
                currentDefinition.Flags.Set(ActorFlagType.ExploCount, flagValue);
                break;
            case "EXPLODEONWATER":
                currentDefinition.Flags.Set(ActorFlagType.ExplodeOnWater, flagValue);
                break;
            case "EXTREMEDEATH":
                currentDefinition.Flags.Set(ActorFlagType.ExtremeDeath, flagValue);
                break;
            case "FASTER":
                currentDefinition.Flags.Set(ActorFlagType.Faster, flagValue);
                break;
            case "FASTMELEE":
                currentDefinition.Flags.Set(ActorFlagType.FastMelee, flagValue);
                break;
            case "FIREDAMAGE":
                currentDefinition.Flags.Set(ActorFlagType.FireDamage, flagValue);
                break;
            case "FIRERESIST":
                currentDefinition.Flags.Set(ActorFlagType.FireResist, flagValue);
                break;
            case "FIXMAPTHINGPOS":
                currentDefinition.Flags.Set(ActorFlagType.FixMapThingPos, flagValue);
                break;
            case "FLATSPRITE":
                currentDefinition.Flags.Set(ActorFlagType.FlatSprite, flagValue);
                break;
            case "FLOAT":
                currentDefinition.Flags.Set(ActorFlagType.Float, flagValue);
                break;
            case "FLOATBOB":
                currentDefinition.Flags.Set(ActorFlagType.FloatBob, flagValue);
                break;
            case "FLOORCLIP":
                currentDefinition.Flags.Set(ActorFlagType.FloorClip, flagValue);
                break;
            case "FLOORHUGGER":
                currentDefinition.Flags.Set(ActorFlagType.FloorHugger, flagValue);
                break;
            case "FOILBUDDHA":
                currentDefinition.Flags.Set(ActorFlagType.FoilBuddha, flagValue);
                break;
            case "FOILINVUL":
                currentDefinition.Flags.Set(ActorFlagType.FoilInvul, flagValue);
                break;
            case "FORCEDECAL":
                currentDefinition.Flags.Set(ActorFlagType.ForceDecal, flagValue);
                break;
            case "FORCEINFIGHTING":
                currentDefinition.Flags.Set(ActorFlagType.ForceInFighting, flagValue);
                break;
            case "FORCEPAIN":
                currentDefinition.Flags.Set(ActorFlagType.ForcePain, flagValue);
                break;
            case "FORCERADIUSDMG":
                currentDefinition.Flags.Set(ActorFlagType.ForceRadiusDmg, flagValue);
                break;
            case "FORCEXYBILLBOARD":
                currentDefinition.Flags.Set(ActorFlagType.ForceXYBillboard, flagValue);
                break;
            case "FORCEYBILLBOARD":
                currentDefinition.Flags.Set(ActorFlagType.ForceYBillboard, flagValue);
                break;
            case "FORCEZERORADIUSDMG":
                currentDefinition.Flags.Set(ActorFlagType.ForceZeroRadiusDmg, flagValue);
                break;
            case "FRIENDLY":
                currentDefinition.Flags.Set(ActorFlagType.Friendly, flagValue);
                break;
            case "FRIGHTENED":
                currentDefinition.Flags.Set(ActorFlagType.Frightened, flagValue);
                break;
            case "FRIGHTENING":
                currentDefinition.Flags.Set(ActorFlagType.Frightening, flagValue);
                break;
            case "FULLVOLACTIVE":
                currentDefinition.Flags.Set(ActorFlagType.FullVolActive, flagValue);
                break;
            case "FULLVOLDEATH":
                currentDefinition.Flags.Set(ActorFlagType.FullVolDeath, flagValue);
                break;
            case "GETOWNER":
                currentDefinition.Flags.Set(ActorFlagType.GetOwner, flagValue);
                break;
            case "GHOST":
                currentDefinition.Flags.Set(ActorFlagType.Ghost, flagValue);
                break;
            case "GRENADETRAIL":
                currentDefinition.Flags.Set(ActorFlagType.GrenadeTrail, flagValue);
                break;
            case "HARMFRIENDS":
                currentDefinition.Flags.Set(ActorFlagType.HarmFriends, flagValue);
                break;
            case "HERETICBOUNCE":
                currentDefinition.Flags.Set(ActorFlagType.HereticBounce, flagValue);
                break;
            case "HEXENBOUNCE":
                currentDefinition.Flags.Set(ActorFlagType.HexenBounce, flagValue);
                break;
            case "HITMASTER":
                currentDefinition.Flags.Set(ActorFlagType.HitMaster, flagValue);
                break;
            case "HITOWNER":
                currentDefinition.Flags.Set(ActorFlagType.HitOwner, flagValue);
                break;
            case "HITTARGET":
                currentDefinition.Flags.Set(ActorFlagType.HitTarget, flagValue);
                break;
            case "HITTRACER":
                currentDefinition.Flags.Set(ActorFlagType.HitTracer, flagValue);
                break;
            case "ICECORPSE":
                currentDefinition.Flags.Set(ActorFlagType.IceCorpse, flagValue);
                break;
            case "ICEDAMAGE":
                currentDefinition.Flags.Set(ActorFlagType.IceDamage, flagValue);
                break;
            case "ICESHATTER":
                currentDefinition.Flags.Set(ActorFlagType.IceShatter, flagValue);
                break;
            case "INCOMBAT":
                currentDefinition.Flags.Set(ActorFlagType.InCombat, flagValue);
                break;
            case "INTERPOLATEANGLES":
                currentDefinition.Flags.Set(ActorFlagType.InterpolateAngles, flagValue);
                break;
            case "INVISIBLE":
                currentDefinition.Flags.Set(ActorFlagType.Invisible, flagValue);
                break;
            case "INVULNERABLE":
                currentDefinition.Flags.Set(ActorFlagType.Invulnerable, flagValue);
                break;
            case "ISMONSTER":
                currentDefinition.Flags.Set(ActorFlagType.IsMonster, flagValue);
                break;
            case "ISTELEPORTSPOT":
                currentDefinition.Flags.Set(ActorFlagType.IsTeleportSpot, flagValue);
                break;
            case "JUMPDOWN":
                currentDefinition.Flags.Set(ActorFlagType.JumpDown, flagValue);
                break;
            case "JUSTATTACKED":
                currentDefinition.Flags.Set(ActorFlagType.JustAttacked, flagValue);
                break;
            case "JUSTHIT":
                currentDefinition.Flags.Set(ActorFlagType.JustHit, flagValue);
                break;
            case "LAXTELEFRAGDMG":
                currentDefinition.Flags.Set(ActorFlagType.LaxTeleFragDmg, flagValue);
                break;
            case "LONGMELEERANGE":
                currentDefinition.Flags.Set(ActorFlagType.LongMeleeRange, flagValue);
                break;
            case "LOOKALLAROUND":
                currentDefinition.Flags.Set(ActorFlagType.LookAllAround, flagValue);
                break;
            case "LOWGRAVITY":
                currentDefinition.Flags.Set(ActorFlagType.LowGravity, flagValue);
                break;
            case "MASKROTATION":
                currentDefinition.Flags.Set(ActorFlagType.MaskRotation, flagValue);
                break;
            case "MBFBOUNCER":
                currentDefinition.Flags.Set(ActorFlagType.MbfBouncer, flagValue);
                break;
            case "MIRRORREFLECT":
                currentDefinition.Flags.Set(ActorFlagType.MirrorReflect, flagValue);
                break;
            case "MISSILE":
                currentDefinition.Flags.Set(ActorFlagType.Missile, flagValue);
                break;
            case "MISSILEEVENMORE":
                currentDefinition.Flags.Set(ActorFlagType.MissileEvenMore, flagValue);
                break;
            case "MISSILEMORE":
                currentDefinition.Flags.Set(ActorFlagType.MissileMore, flagValue);
                break;
            case "MONSTER":
                currentDefinition.Flags.Set(ActorFlagType.Monster, flagValue);
                break;
            case "MOVEWITHSECTOR":
                currentDefinition.Flags.Set(ActorFlagType.MoveWithSector, flagValue);
                break;
            case "MTHRUSPECIES":
                currentDefinition.Flags.Set(ActorFlagType.MThruSpecies, flagValue);
                break;
            case "NEVERFAST":
                currentDefinition.Flags.Set(ActorFlagType.NeverFast, flagValue);
                break;
            case "NEVERRESPAWN":
                currentDefinition.Flags.Set(ActorFlagType.NeverRespawn, flagValue);
                break;
            case "NEVERTARGET":
                currentDefinition.Flags.Set(ActorFlagType.NeverTarget, flagValue);
                break;
            case "NOBLOCKMAP":
                currentDefinition.Flags.Set(ActorFlagType.NoBlockmap, flagValue);
                break;
            case "NOBLOCKMONST":
                currentDefinition.Flags.Set(ActorFlagType.NoBlockMonst, flagValue);
                break;
            case "NOBLOOD":
                currentDefinition.Flags.Set(ActorFlagType.NoBlood, flagValue);
                break;
            case "NOBLOODDECALS":
                currentDefinition.Flags.Set(ActorFlagType.NoBloodDecals, flagValue);
                break;
            case "NOBOSSRIP":
                currentDefinition.Flags.Set(ActorFlagType.NoBossRip, flagValue);
                break;
            case "NOBOUNCESOUND":
                currentDefinition.Flags.Set(ActorFlagType.NoBounceSound, flagValue);
                break;
            case "NOCLIP":
                currentDefinition.Flags.Set(ActorFlagType.NoClip, flagValue);
                break;
            case "NODAMAGE":
                currentDefinition.Flags.Set(ActorFlagType.NoDamage, flagValue);
                break;
            case "NODAMAGETHRUST":
                currentDefinition.Flags.Set(ActorFlagType.NoDamageThrust, flagValue);
                break;
            case "NODECAL":
                currentDefinition.Flags.Set(ActorFlagType.NoDecal, flagValue);
                break;
            case "NODROPOFF":
                currentDefinition.Flags.Set(ActorFlagType.NoDropoff, flagValue);
                break;
            case "NOEXPLODEFLOOR":
                currentDefinition.Flags.Set(ActorFlagType.NoExplodeFloor, flagValue);
                break;
            case "NOEXTREMEDEATH":
                currentDefinition.Flags.Set(ActorFlagType.NoExtremeDeath, flagValue);
                break;
            case "NOFEAR":
                currentDefinition.Flags.Set(ActorFlagType.NoFear, flagValue);
                break;
            case "NOFRICTION":
                currentDefinition.Flags.Set(ActorFlagType.NoFriction, flagValue);
                break;
            case "NOFRICTIONBOUNCE":
                currentDefinition.Flags.Set(ActorFlagType.NoFrictionBounce, flagValue);
                break;
            case "NOFORWARDFALL":
                currentDefinition.Flags.Set(ActorFlagType.NoForwardFall, flagValue);
                break;
            case "NOGRAVITY":
                currentDefinition.Flags.Set(ActorFlagType.NoGravity, flagValue);
                break;
            case "NOICEDEATH":
                currentDefinition.Flags.Set(ActorFlagType.NoIceDeath, flagValue);
                break;
            case "NOINFIGHTING":
                currentDefinition.Flags.Set(ActorFlagType.NoInfighting, flagValue);
                break;
            case "NOINFIGHTSPECIES":
                currentDefinition.Flags.Set(ActorFlagType.NoInfightSpecies, flagValue);
                break;
            case "NOINTERACTION":
                currentDefinition.Flags.Set(ActorFlagType.NoInteraction, flagValue);
                break;
            case "NOKILLSCRIPTS":
                currentDefinition.Flags.Set(ActorFlagType.NoKillScripts, flagValue);
                break;
            case "NOLIFTDROP":
                currentDefinition.Flags.Set(ActorFlagType.NoLiftDrop, flagValue);
                break;
            case "NOMENU":
                currentDefinition.Flags.Set(ActorFlagType.NoMenu, flagValue);
                break;
            case "NONSHOOTABLE":
                currentDefinition.Flags.Set(ActorFlagType.NonShootable, flagValue);
                break;
            case "NOPAIN":
                currentDefinition.Flags.Set(ActorFlagType.NoPain, flagValue);
                break;
            case "NORADIUSDMG":
                currentDefinition.Flags.Set(ActorFlagType.NoRadiusDmg, flagValue);
                break;
            case "NOSECTOR":
                currentDefinition.Flags.Set(ActorFlagType.NoSector, flagValue);
                break;
            case "NOSKIN":
                currentDefinition.Flags.Set(ActorFlagType.NoSkin, flagValue);
                break;
            case "NOSPLASHALERT":
                currentDefinition.Flags.Set(ActorFlagType.NoSplashAlert, flagValue);
                break;
            case "NOTARGET":
                currentDefinition.Flags.Set(ActorFlagType.NoTarget, flagValue);
                break;
            case "NOTARGETSWITCH":
                currentDefinition.Flags.Set(ActorFlagType.NoTargetSwitch, flagValue);
                break;
            case "NOTAUTOAIMED":
                currentDefinition.Flags.Set(ActorFlagType.NotAutoaimed, flagValue);
                break;
            case "NOTDMATCH":
                currentDefinition.Flags.Set(ActorFlagType.NotDMatch, flagValue);
                break;
            case "NOTELEFRAG":
                currentDefinition.Flags.Set(ActorFlagType.NoTelefrag, flagValue);
                break;
            case "NOTELEOTHER":
                currentDefinition.Flags.Set(ActorFlagType.NoTeleOther, flagValue);
                break;
            case "NOTELEPORT":
                currentDefinition.Flags.Set(ActorFlagType.NoTeleport, flagValue);
                break;
            case "NOTELESTOMP":
                currentDefinition.Flags.Set(ActorFlagType.NoTelestomp, flagValue);
                break;
            case "NOTIMEFREEZE":
                currentDefinition.Flags.Set(ActorFlagType.NoTimeFreeze, flagValue);
                break;
            case "NOTONAUTOMAP":
                currentDefinition.Flags.Set(ActorFlagType.NotOnAutomap, flagValue);
                break;
            case "NOTRIGGER":
                currentDefinition.Flags.Set(ActorFlagType.NoTrigger, flagValue);
                break;
            case "NOVERTICALMELEERANGE":
                currentDefinition.Flags.Set(ActorFlagType.NoVerticalMeleeRange, flagValue);
                break;
            case "NOWALLBOUNCESND":
                currentDefinition.Flags.Set(ActorFlagType.NoWallBounceSnd, flagValue);
                break;
            case "OLDRADIUSDMG":
                currentDefinition.Flags.Set(ActorFlagType.OldRadiusDmg, flagValue);
                break;
            case "PAINLESS":
                currentDefinition.Flags.Set(ActorFlagType.Painless, flagValue);
                break;
            case "PICKUP":
                currentDefinition.Flags.Set(ActorFlagType.Pickup, flagValue);
                break;
            case "PIERCEARMOR":
                currentDefinition.Flags.Set(ActorFlagType.PierceArmor, flagValue);
                break;
            case "POISONALWAYS":
                currentDefinition.Flags.Set(ActorFlagType.PoisonAlways, flagValue);
                break;
            case "PROJECTILE":
                currentDefinition.Flags.Set(ActorFlagType.Projectile, flagValue);
                break;
            case "PUFFGETSOWNER":
                currentDefinition.Flags.Set(ActorFlagType.PuffGetsOwner, flagValue);
                break;
            case "PUFFONACTORS":
                currentDefinition.Flags.Set(ActorFlagType.PuffOnActors, flagValue);
                break;
            case "PUSHABLE":
                currentDefinition.Flags.Set(ActorFlagType.Pushable, flagValue);
                break;
            case "QUARTERGRAVITY":
                currentDefinition.Flags.Set(ActorFlagType.QuarterGravity, flagValue);
                break;
            case "QUICKTORETALIATE":
                currentDefinition.Flags.Set(ActorFlagType.QuickToRetaliate, flagValue);
                break;
            case "RANDOMIZE":
                currentDefinition.Flags.Set(ActorFlagType.Randomize, flagValue);
                break;
            case "REFLECTIVE":
                currentDefinition.Flags.Set(ActorFlagType.Reflective, flagValue);
                break;
            case "RELATIVETOFLOOR":
                currentDefinition.Flags.Set(ActorFlagType.RelativeToFloor, flagValue);
                break;
            case "RIPPER":
                currentDefinition.Flags.Set(ActorFlagType.Ripper, flagValue);
                break;
            case "ROCKETTRAIL":
                currentDefinition.Flags.Set(ActorFlagType.RocketTrail, flagValue);
                break;
            case "ROLLCENTER":
                currentDefinition.Flags.Set(ActorFlagType.RollCenter, flagValue);
                break;
            case "ROLLSPRITE":
                currentDefinition.Flags.Set(ActorFlagType.RollSprite, flagValue);
                break;
            case "SCREENSEEKER":
                currentDefinition.Flags.Set(ActorFlagType.ScreenSeeker, flagValue);
                break;
            case "SEEINVISIBLE":
                currentDefinition.Flags.Set(ActorFlagType.SeeInvisible, flagValue);
                break;
            case "SEEKERMISSILE":
                currentDefinition.Flags.Set(ActorFlagType.SeekerMissile, flagValue);
                break;
            case "SEESDAGGERS":
                currentDefinition.Flags.Set(ActorFlagType.SeesDaggers, flagValue);
                break;
            case "SHADOW":
                currentDefinition.Flags.Set(ActorFlagType.Shadow, flagValue);
                break;
            case "SHIELDREFLECT":
                currentDefinition.Flags.Set(ActorFlagType.ShieldReflect, flagValue);
                break;
            case "SHOOTABLE":
                currentDefinition.Flags.Set(ActorFlagType.Shootable, flagValue);
                break;
            case "SHORTMISSILERANGE":
                currentDefinition.Flags.Set(ActorFlagType.ShortMissileRange, flagValue);
                break;
            case "SKULLFLY":
                currentDefinition.Flags.Set(ActorFlagType.Skullfly, flagValue);
                break;
            case "SKYEXPLODE":
                currentDefinition.Flags.Set(ActorFlagType.SkyExplode, flagValue);
                break;
            case "SLIDESONWALLS":
                currentDefinition.Flags.Set(ActorFlagType.SlidesOnWalls, flagValue);
                break;
            case "SOLID":
                currentDefinition.Flags.Set(ActorFlagType.Solid, flagValue);
                break;
            case "SPAWNCEILING":
                currentDefinition.Flags.Set(ActorFlagType.SpawnCeiling, flagValue);
                break;
            case "SPAWNFLOAT":
                currentDefinition.Flags.Set(ActorFlagType.SpawnFloat, flagValue);
                break;
            case "SPAWNSOUNDSOURCE":
                currentDefinition.Flags.Set(ActorFlagType.SpawnSoundSource, flagValue);
                break;
            case "SPECIAL":
                currentDefinition.Flags.Set(ActorFlagType.Special, flagValue);
                break;
            case "SPECIALFIREDAMAGE":
                currentDefinition.Flags.Set(ActorFlagType.SpecialFireDamage, flagValue);
                break;
            case "SPECIALFLOORCLIP":
                currentDefinition.Flags.Set(ActorFlagType.SpecialFloorClip, flagValue);
                break;
            case "SPECTRAL":
                currentDefinition.Flags.Set(ActorFlagType.Spectral, flagValue);
                break;
            case "SPRITEANGLE":
                currentDefinition.Flags.Set(ActorFlagType.SpriteAngle, flagValue);
                break;
            case "SPRITEFLIP":
                currentDefinition.Flags.Set(ActorFlagType.SpriteFlip, flagValue);
                break;
            case "STANDSTILL":
                currentDefinition.Flags.Set(ActorFlagType.StandStill, flagValue);
                break;
            case "STAYMORPHED":
                currentDefinition.Flags.Set(ActorFlagType.StayMorphed, flagValue);
                break;
            case "STEALTH":
                currentDefinition.Flags.Set(ActorFlagType.Stealth, flagValue);
                break;
            case "STEPMISSILE":
                currentDefinition.Flags.Set(ActorFlagType.StepMissile, flagValue);
                break;
            case "STRIFEDAMAGE":
                currentDefinition.Flags.Set(ActorFlagType.StrifeDamage, flagValue);
                break;
            case "SUMMONEDMONSTER":
                currentDefinition.Flags.Set(ActorFlagType.SummonedMonster, flagValue);
                break;
            case "SYNCHRONIZED":
                currentDefinition.Flags.Set(ActorFlagType.Synchronized, flagValue);
                break;
            case "TELEPORT":
                currentDefinition.Flags.Set(ActorFlagType.Teleport, flagValue);
                break;
            case "TELESTOMP":
                currentDefinition.Flags.Set(ActorFlagType.Telestomp, flagValue);
                break;
            case "THRUACTORS":
                currentDefinition.Flags.Set(ActorFlagType.ThruActors, flagValue);
                break;
            case "THRUGHOST":
                currentDefinition.Flags.Set(ActorFlagType.ThruGhost, flagValue);
                break;
            case "THRUREFLECT":
                currentDefinition.Flags.Set(ActorFlagType.ThruReflect, flagValue);
                break;
            case "THRUSPECIES":
                currentDefinition.Flags.Set(ActorFlagType.ThruSpecies, flagValue);
                break;
            case "TOUCHY":
                currentDefinition.Flags.Set(ActorFlagType.Touchy, flagValue);
                break;
            case "USEBOUNCESTATE":
                currentDefinition.Flags.Set(ActorFlagType.UseBounceState, flagValue);
                break;
            case "USEKILLSCRIPTS":
                currentDefinition.Flags.Set(ActorFlagType.UseKillScripts, flagValue);
                break;
            case "USESPECIAL":
                currentDefinition.Flags.Set(ActorFlagType.UseSpecial, flagValue);
                break;
            case "VISIBILITYPULSE":
                currentDefinition.Flags.Set(ActorFlagType.VisibilityPulse, flagValue);
                break;
            case "VULNERABLE":
                currentDefinition.Flags.Set(ActorFlagType.Vulnerable, flagValue);
                break;
            case "WALLSPRITE":
                currentDefinition.Flags.Set(ActorFlagType.WallSprite, flagValue);
                break;
            case "WINDTHRUST":
                currentDefinition.Flags.Set(ActorFlagType.WindThrust, flagValue);
                break;
            case "ZDOOMTRANS":
                currentDefinition.Flags.Set(ActorFlagType.ZdoomTrans, flagValue);
                break;
            default:
                Log.Warn("Unknown flag '{0}' for actor {1}", flagName, currentDefinition.Name);
                break;
            }
        }
    }
}

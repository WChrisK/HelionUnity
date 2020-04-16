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
                currentDefinition.Flags.InventoryAdditiveTime = flagValue;
                break;
            case "ALWAYSPICKUP":
                currentDefinition.Flags.InventoryAlwaysPickup = flagValue;
                break;
            case "ALWAYSRESPAWN":
                currentDefinition.Flags.InventoryAlwaysRespawn = flagValue;
                break;
            case "AUTOACTIVATE":
                currentDefinition.Flags.InventoryAutoActivate = flagValue;
                break;
            case "BIGPOWERUP":
                currentDefinition.Flags.InventoryBigPowerup = flagValue;
                break;
            case "FANCYPICKUPSOUND":
                currentDefinition.Flags.InventoryFancyPickupSound = flagValue;
                break;
            case "HUBPOWER":
                currentDefinition.Flags.InventoryHubPower = flagValue;
                break;
            case "IGNORESKILL":
                currentDefinition.Flags.InventoryIgnoreSkill = flagValue;
                break;
            case "INTERHUBSTRIP":
                currentDefinition.Flags.InventoryInterHubStrip = flagValue;
                break;
            case "INVBAR":
                currentDefinition.Flags.InventoryInvbar = flagValue;
                break;
            case "ISARMOR":
                currentDefinition.Flags.InventoryIsArmor = flagValue;
                break;
            case "ISHEALTH":
                currentDefinition.Flags.InventoryIsHealth = flagValue;
                break;
            case "KEEPDEPLETED":
                currentDefinition.Flags.InventoryKeepDepleted = flagValue;
                break;
            case "NEVERRESPAWN":
                currentDefinition.Flags.InventoryNeverRespawn = flagValue;
                break;
            case "NOATTENPICKUPSOUND":
                currentDefinition.Flags.InventoryNoAttenPickupSound = flagValue;
                break;
            case "NOSCREENBLINK":
                currentDefinition.Flags.InventoryNoScreenBlink = flagValue;
                break;
            case "NOSCREENFLASH":
                currentDefinition.Flags.InventoryNoScreenFlash = flagValue;
                break;
            case "NOTELEPORTFREEZE":
                currentDefinition.Flags.InventoryNoTeleportFreeze = flagValue;
                break;
            case "PERSISTENTPOWER":
                currentDefinition.Flags.InventoryPersistentPower = flagValue;
                break;
            case "PICKUPFLASH":
                currentDefinition.Flags.InventoryPickupFlash = flagValue;
                break;
            case "QUIET":
                currentDefinition.Flags.InventoryQuiet = flagValue;
                break;
            case "RESTRICTABSOLUTELY":
                currentDefinition.Flags.InventoryRestrictAbsolutely = flagValue;
                break;
            case "TOSSED":
                currentDefinition.Flags.InventoryTossed = flagValue;
                break;
            case "TRANSFER":
                currentDefinition.Flags.InventoryTransfer = flagValue;
                break;
            case "UNCLEARABLE":
                currentDefinition.Flags.InventoryUnclearable = flagValue;
                break;
            case "UNDROPPABLE":
                currentDefinition.Flags.InventoryUndroppable = flagValue;
                break;
            case "UNTOSSABLE":
                currentDefinition.Flags.InventoryUntossable = flagValue;
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
                currentDefinition.Flags.PlayerPawnCanSuperMorph = flagValue;
                break;
            case "CROUCHABLEMORPH":
                currentDefinition.Flags.PlayerPawnCrouchableMorph = flagValue;
                break;
            case "NOTHRUSTWHENINVUL":
                currentDefinition.Flags.PlayerPawnNoThrustWhenInvul = flagValue;
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
                currentDefinition.Flags.WeaponAltAmmoOptional = flagValue;
                break;
            case "ALTUSESBOTH":
                currentDefinition.Flags.WeaponAltUsesBoth = flagValue;
                break;
            case "AMMOCHECKBOTH":
                currentDefinition.Flags.WeaponAmmoCheckBoth = flagValue;
                break;
            case "AMMOOPTIONAL":
                currentDefinition.Flags.WeaponAmmoOptional = flagValue;
                break;
            case "AXEBLOOD":
                currentDefinition.Flags.WeaponAxeBlood = flagValue;
                break;
            case "BFG":
                currentDefinition.Flags.WeaponBfg = flagValue;
                break;
            case "CHEATNOTWEAPON":
                currentDefinition.Flags.WeaponCheatNotWeapon = flagValue;
                break;
            case "DONTBOB":
                currentDefinition.Flags.WeaponDontBob = flagValue;
                break;
            case "EXPLOSIVE":
                currentDefinition.Flags.WeaponExplosive = flagValue;
                break;
            case "MELEEWEAPON":
                currentDefinition.Flags.WeaponMeleeWeapon = flagValue;
                break;
            case "NOALERT":
                currentDefinition.Flags.WeaponNoAlert = flagValue;
                break;
            case "NOAUTOAIM":
                currentDefinition.Flags.WeaponNoAutoaim = flagValue;
                break;
            case "NOAUTOFIRE":
                currentDefinition.Flags.WeaponNoAutofire = flagValue;
                break;
            case "NODEATHDESELECT":
                currentDefinition.Flags.WeaponNoDeathDeselect = flagValue;
                break;
            case "NODEATHINPUT":
                currentDefinition.Flags.WeaponNoDeathInput = flagValue;
                break;
            case "NOAUTOSWITCH":
                currentDefinition.Flags.WeaponNoAutoSwitch = flagValue;
                break;
            case "POWEREDUP":
                currentDefinition.Flags.WeaponPoweredUp = flagValue;
                break;
            case "PRIMARYUSESBOTH":
                currentDefinition.Flags.WeaponPrimaryUsesBoth = flagValue;
                break;
            case "READYSNDHALF":
                currentDefinition.Flags.WeaponReadySndHalf = flagValue;
                break;
            case "STAFF2KICKBACK":
                currentDefinition.Flags.WeaponStaff2Kickback = flagValue;
                break;
            case "SPAWN":
                currentDefinition.Flags.WeaponSpawn = flagValue;
                break;
            case "WIMPY_WEAPON":
                currentDefinition.Flags.WeaponWimpyWeapon = flagValue;
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
                currentDefinition.Flags.AbsMaskAngle = flagValue;
                break;
            case "ABSMASKPITCH":
                currentDefinition.Flags.AbsMaskPitch = flagValue;
                break;
            case "ACTIVATEIMPACT":
                currentDefinition.Flags.ActivateImpact = flagValue;
                break;
            case "ACTIVATEMCROSS":
                currentDefinition.Flags.ActivateMCross = flagValue;
                break;
            case "ACTIVATEPCROSS":
                currentDefinition.Flags.ActivatePCross = flagValue;
                break;
            case "ACTLIKEBRIDGE":
                currentDefinition.Flags.ActLikeBridge = flagValue;
                break;
            case "ADDITIVEPOISONDAMAGE":
                currentDefinition.Flags.AdditivePoisonDamage = flagValue;
                break;
            case "ADDITIVEPOISONDURATION":
                currentDefinition.Flags.AdditivePoisonDuration = flagValue;
                break;
            case "AIMREFLECT":
                currentDefinition.Flags.AimReflect = flagValue;
                break;
            case "ALLOWBOUNCEONACTORS":
                currentDefinition.Flags.AllowBounceOnActors = flagValue;
                break;
            case "ALLOWPAIN":
                currentDefinition.Flags.AllowPain = flagValue;
                break;
            case "ALLOWPARTICLES":
                currentDefinition.Flags.AllowParticles = flagValue;
                break;
            case "ALLOWTHRUFLAGS":
                currentDefinition.Flags.AllowThruFlags = flagValue;
                break;
            case "ALWAYSFAST":
                currentDefinition.Flags.AlwaysFast = flagValue;
                break;
            case "ALWAYSPUFF":
                currentDefinition.Flags.AlwaysPuff = flagValue;
                break;
            case "ALWAYSRESPAWN":
                currentDefinition.Flags.AlwaysRespawn = flagValue;
                break;
            case "ALWAYSTELEFRAG":
                currentDefinition.Flags.AlwaysTelefrag = flagValue;
                break;
            case "AMBUSH":
                currentDefinition.Flags.Ambush = flagValue;
                break;
            case "AVOIDMELEE":
                currentDefinition.Flags.AvoidMelee = flagValue;
                break;
            case "BLASTED":
                currentDefinition.Flags.Blasted = flagValue;
                break;
            case "BLOCKASPLAYER":
                currentDefinition.Flags.BlockAsPlayer = flagValue;
                break;
            case "BLOCKEDBYSOLIDACTORS":
                currentDefinition.Flags.BlockedBySolidActors = flagValue;
                break;
            case "BLOODLESSIMPACT":
                currentDefinition.Flags.BloodlessImpact = flagValue;
                break;
            case "BLOODSPLATTER":
                currentDefinition.Flags.BloodSplatter = flagValue;
                break;
            case "BOSS":
                currentDefinition.Flags.Boss = flagValue;
                break;
            case "BOSSDEATH":
                currentDefinition.Flags.BossDeath = flagValue;
                break;
            case "BOUNCEAUTOOFF":
                currentDefinition.Flags.BounceAutoOff = flagValue;
                break;
            case "BOUNCEAUTOOFFFLOORONLY":
                currentDefinition.Flags.BounceAutoOffFloorOnly = flagValue;
                break;
            case "BOUNCELIKEHERETIC":
                currentDefinition.Flags.BounceLikeHeretic = flagValue;
                break;
            case "BOUNCEONACTORS":
                currentDefinition.Flags.BounceOnActors = flagValue;
                break;
            case "BOUNCEONCEILINGS":
                currentDefinition.Flags.BounceOnCeilings = flagValue;
                break;
            case "BOUNCEONFLOORS":
                currentDefinition.Flags.BounceOnFloors = flagValue;
                break;
            case "BOUNCEONUNRIPPABLES":
                currentDefinition.Flags.BounceOnUnrippables = flagValue;
                break;
            case "BOUNCEONWALLS":
                currentDefinition.Flags.BounceOnWalls = flagValue;
                break;
            case "BRIGHT":
                currentDefinition.Flags.Bright = flagValue;
                break;
            case "BUDDHA":
                currentDefinition.Flags.Buddha = flagValue;
                break;
            case "BUMPSPECIAL":
                currentDefinition.Flags.BumpSpecial = flagValue;
                break;
            case "CANBLAST":
                currentDefinition.Flags.CanBlast = flagValue;
                break;
            case "CANBOUNCEWATER":
                currentDefinition.Flags.CanBounceWater = flagValue;
                break;
            case "CANNOTPUSH":
                currentDefinition.Flags.CannotPush = flagValue;
                break;
            case "CANPASS":
                currentDefinition.Flags.CanPass = flagValue;
                break;
            case "CANPUSHWALLS":
                currentDefinition.Flags.CanPushWalls = flagValue;
                break;
            case "CANTLEAVEFLOORPIC":
                currentDefinition.Flags.CantLeaveFloorPic = flagValue;
                break;
            case "CANTSEEK":
                currentDefinition.Flags.CantSeek = flagValue;
                break;
            case "CANUSEWALLS":
                currentDefinition.Flags.CanUseWalls = flagValue;
                break;
            case "CAUSEPAIN":
                currentDefinition.Flags.CausePain = flagValue;
                break;
            case "CEILINGHUGGER":
                currentDefinition.Flags.CeilingHugger = flagValue;
                break;
            case "CORPSE":
                currentDefinition.Flags.Corpse = flagValue;
                break;
            case "COUNTITEM":
                currentDefinition.Flags.CountItem = flagValue;
                break;
            case "COUNTKILL":
                currentDefinition.Flags.CountKill = flagValue;
                break;
            case "COUNTSECRET":
                currentDefinition.Flags.CountSecret = flagValue;
                break;
            case "DEFLECT":
                currentDefinition.Flags.Deflect = flagValue;
                break;
            case "DEHEXPLOSION":
                currentDefinition.Flags.DehExplosion = flagValue;
                break;
            case "DOHARMSPECIES":
                currentDefinition.Flags.DoHarmSpecies = flagValue;
                break;
            case "DONTBLAST":
                currentDefinition.Flags.DontBlast = flagValue;
                break;
            case "DONTBOUNCEONSHOOTABLES":
                currentDefinition.Flags.DontBounceOnShootables = flagValue;
                break;
            case "DONTBOUNCEONSKY":
                currentDefinition.Flags.DontBounceOnSky = flagValue;
                break;
            case "DONTCORPSE":
                currentDefinition.Flags.DontCorpse = flagValue;
                break;
            case "DONTDRAIN":
                currentDefinition.Flags.DontDrain = flagValue;
                break;
            case "DONTFACETALKER":
                currentDefinition.Flags.DontFaceTalker = flagValue;
                break;
            case "DONTFALL":
                currentDefinition.Flags.DontFall = flagValue;
                break;
            case "DONTGIB":
                currentDefinition.Flags.DontGib = flagValue;
                break;
            case "DONTHARMCLASS":
                currentDefinition.Flags.DontHarmClass = flagValue;
                break;
            case "DONTHARMSPECIES":
                currentDefinition.Flags.DontHarmSpecies = flagValue;
                break;
            case "DONTHURTSPECIES":
                currentDefinition.Flags.DontHurtSpecies = flagValue;
                break;
            case "DONTINTERPOLATE":
                currentDefinition.Flags.DontInterpolate = flagValue;
                break;
            case "DONTMORPH":
                currentDefinition.Flags.DontMorph = flagValue;
                break;
            case "DONTOVERLAP":
                currentDefinition.Flags.DontOverlap = flagValue;
                break;
            case "DONTREFLECT":
                currentDefinition.Flags.DontReflect = flagValue;
                break;
            case "DONTRIP":
                currentDefinition.Flags.DontRip = flagValue;
                break;
            case "DONTSEEKINVISIBLE":
                currentDefinition.Flags.DontSeekInvisible = flagValue;
                break;
            case "DONTSPLASH":
                currentDefinition.Flags.DontSplash = flagValue;
                break;
            case "DONTSQUASH":
                currentDefinition.Flags.DontSquash = flagValue;
                break;
            case "DONTTHRUST":
                currentDefinition.Flags.DontThrust = flagValue;
                break;
            case "DONTTRANSLATE":
                currentDefinition.Flags.DontTranslate = flagValue;
                break;
            case "DOOMBOUNCE":
                currentDefinition.Flags.DoomBounce = flagValue;
                break;
            case "DORMANT":
                currentDefinition.Flags.Dormant = flagValue;
                break;
            case "DROPOFF":
                currentDefinition.Flags.Dropoff = flagValue;
                break;
            case "DROPPED":
                currentDefinition.Flags.Dropped = flagValue;
                break;
            case "EXPLOCOUNT":
                currentDefinition.Flags.ExploCount = flagValue;
                break;
            case "EXPLODEONWATER":
                currentDefinition.Flags.ExplodeOnWater = flagValue;
                break;
            case "EXTREMEDEATH":
                currentDefinition.Flags.ExtremeDeath = flagValue;
                break;
            case "FASTER":
                currentDefinition.Flags.Faster = flagValue;
                break;
            case "FASTMELEE":
                currentDefinition.Flags.FastMelee = flagValue;
                break;
            case "FIREDAMAGE":
                currentDefinition.Flags.FireDamage = flagValue;
                break;
            case "FIRERESIST":
                currentDefinition.Flags.FireResist = flagValue;
                break;
            case "FIXMAPTHINGPOS":
                currentDefinition.Flags.FixMapThingPos = flagValue;
                break;
            case "FLATSPRITE":
                currentDefinition.Flags.FlatSprite = flagValue;
                break;
            case "FLOAT":
                currentDefinition.Flags.Float = flagValue;
                break;
            case "FLOATBOB":
                currentDefinition.Flags.FloatBob = flagValue;
                break;
            case "FLOORCLIP":
                currentDefinition.Flags.FloorClip = flagValue;
                break;
            case "FLOORHUGGER":
                currentDefinition.Flags.FloorHugger = flagValue;
                break;
            case "FOILBUDDHA":
                currentDefinition.Flags.FoilBuddha = flagValue;
                break;
            case "FOILINVUL":
                currentDefinition.Flags.FoilInvul = flagValue;
                break;
            case "FORCEDECAL":
                currentDefinition.Flags.ForceDecal = flagValue;
                break;
            case "FORCEINFIGHTING":
                currentDefinition.Flags.ForceInFighting = flagValue;
                break;
            case "FORCEPAIN":
                currentDefinition.Flags.ForcePain = flagValue;
                break;
            case "FORCERADIUSDMG":
                currentDefinition.Flags.ForceRadiusDmg = flagValue;
                break;
            case "FORCEXYBILLBOARD":
                currentDefinition.Flags.ForceXYBillboard = flagValue;
                break;
            case "FORCEYBILLBOARD":
                currentDefinition.Flags.ForceYBillboard = flagValue;
                break;
            case "FORCEZERORADIUSDMG":
                currentDefinition.Flags.ForceZeroRadiusDmg = flagValue;
                break;
            case "FRIENDLY":
                currentDefinition.Flags.Friendly = flagValue;
                break;
            case "FRIGHTENED":
                currentDefinition.Flags.Frightened = flagValue;
                break;
            case "FRIGHTENING":
                currentDefinition.Flags.Frightening = flagValue;
                break;
            case "FULLVOLACTIVE":
                currentDefinition.Flags.FullVolActive = flagValue;
                break;
            case "FULLVOLDEATH":
                currentDefinition.Flags.FullVolDeath = flagValue;
                break;
            case "GETOWNER":
                currentDefinition.Flags.GetOwner = flagValue;
                break;
            case "GHOST":
                currentDefinition.Flags.Ghost = flagValue;
                break;
            case "GRENADETRAIL":
                currentDefinition.Flags.GrenadeTrail = flagValue;
                break;
            case "HARMFRIENDS":
                currentDefinition.Flags.HarmFriends = flagValue;
                break;
            case "HERETICBOUNCE":
                currentDefinition.Flags.HereticBounce = flagValue;
                break;
            case "HEXENBOUNCE":
                currentDefinition.Flags.HexenBounce = flagValue;
                break;
            case "HITMASTER":
                currentDefinition.Flags.HitMaster = flagValue;
                break;
            case "HITOWNER":
                currentDefinition.Flags.HitOwner = flagValue;
                break;
            case "HITTARGET":
                currentDefinition.Flags.HitTarget = flagValue;
                break;
            case "HITTRACER":
                currentDefinition.Flags.HitTracer = flagValue;
                break;
            case "ICECORPSE":
                currentDefinition.Flags.IceCorpse = flagValue;
                break;
            case "ICEDAMAGE":
                currentDefinition.Flags.IceDamage = flagValue;
                break;
            case "ICESHATTER":
                currentDefinition.Flags.IceShatter = flagValue;
                break;
            case "INCOMBAT":
                currentDefinition.Flags.InCombat = flagValue;
                break;
            case "INTERPOLATEANGLES":
                currentDefinition.Flags.InterpolateAngles = flagValue;
                break;
            case "INVISIBLE":
                currentDefinition.Flags.Invisible = flagValue;
                break;
            case "INVULNERABLE":
                currentDefinition.Flags.Invulnerable = flagValue;
                break;
            case "ISMONSTER":
                currentDefinition.Flags.IsMonster = flagValue;
                break;
            case "ISTELEPORTSPOT":
                currentDefinition.Flags.IsTeleportSpot = flagValue;
                break;
            case "JUMPDOWN":
                currentDefinition.Flags.JumpDown = flagValue;
                break;
            case "JUSTATTACKED":
                currentDefinition.Flags.JustAttacked = flagValue;
                break;
            case "JUSTHIT":
                currentDefinition.Flags.JustHit = flagValue;
                break;
            case "LAXTELEFRAGDMG":
                currentDefinition.Flags.LaxTeleFragDmg = flagValue;
                break;
            case "LONGMELEERANGE":
                currentDefinition.Flags.LongMeleeRange = flagValue;
                break;
            case "LOOKALLAROUND":
                currentDefinition.Flags.LookAllAround = flagValue;
                break;
            case "LOWGRAVITY":
                currentDefinition.Flags.LowGravity = flagValue;
                break;
            case "MASKROTATION":
                currentDefinition.Flags.MaskRotation = flagValue;
                break;
            case "MBFBOUNCER":
                currentDefinition.Flags.MbfBouncer = flagValue;
                break;
            case "MIRRORREFLECT":
                currentDefinition.Flags.MirrorReflect = flagValue;
                break;
            case "MISSILE":
                currentDefinition.Flags.Missile = flagValue;
                break;
            case "MISSILEEVENMORE":
                currentDefinition.Flags.MissileEvenMore = flagValue;
                break;
            case "MISSILEMORE":
                currentDefinition.Flags.MissileMore = flagValue;
                break;
            case "MONSTER":
                currentDefinition.Flags.Monster = flagValue;
                break;
            case "MOVEWITHSECTOR":
                currentDefinition.Flags.MoveWithSector = flagValue;
                break;
            case "MTHRUSPECIES":
                currentDefinition.Flags.MThruSpecies = flagValue;
                break;
            case "NEVERFAST":
                currentDefinition.Flags.NeverFast = flagValue;
                break;
            case "NEVERRESPAWN":
                currentDefinition.Flags.NeverRespawn = flagValue;
                break;
            case "NEVERTARGET":
                currentDefinition.Flags.NeverTarget = flagValue;
                break;
            case "NOBLOCKMAP":
                currentDefinition.Flags.NoBlockmap = flagValue;
                break;
            case "NOBLOCKMONST":
                currentDefinition.Flags.NoBlockMonst = flagValue;
                break;
            case "NOBLOOD":
                currentDefinition.Flags.NoBlood = flagValue;
                break;
            case "NOBLOODDECALS":
                currentDefinition.Flags.NoBloodDecals = flagValue;
                break;
            case "NOBOSSRIP":
                currentDefinition.Flags.NoBossRip = flagValue;
                break;
            case "NOBOUNCESOUND":
                currentDefinition.Flags.NoBounceSound = flagValue;
                break;
            case "NOCLIP":
                currentDefinition.Flags.NoClip = flagValue;
                break;
            case "NODAMAGE":
                currentDefinition.Flags.NoDamage = flagValue;
                break;
            case "NODAMAGETHRUST":
                currentDefinition.Flags.NoDamageThrust = flagValue;
                break;
            case "NODECAL":
                currentDefinition.Flags.NoDecal = flagValue;
                break;
            case "NODROPOFF":
                currentDefinition.Flags.NoDropoff = flagValue;
                break;
            case "NOEXPLODEFLOOR":
                currentDefinition.Flags.NoExplodeFloor = flagValue;
                break;
            case "NOEXTREMEDEATH":
                currentDefinition.Flags.NoExtremeDeath = flagValue;
                break;
            case "NOFEAR":
                currentDefinition.Flags.NoFear = flagValue;
                break;
            case "NOFRICTION":
                currentDefinition.Flags.NoFriction = flagValue;
                break;
            case "NOFRICTIONBOUNCE":
                currentDefinition.Flags.NoFrictionBounce = flagValue;
                break;
            case "NOFORWARDFALL":
                currentDefinition.Flags.NoForwardFall = flagValue;
                break;
            case "NOGRAVITY":
                currentDefinition.Flags.NoGravity = flagValue;
                break;
            case "NOICEDEATH":
                currentDefinition.Flags.NoIceDeath = flagValue;
                break;
            case "NOINFIGHTING":
                currentDefinition.Flags.NoInfighting = flagValue;
                break;
            case "NOINFIGHTSPECIES":
                currentDefinition.Flags.NoInfightSpecies = flagValue;
                break;
            case "NOINTERACTION":
                currentDefinition.Flags.NoInteraction = flagValue;
                break;
            case "NOKILLSCRIPTS":
                currentDefinition.Flags.NoKillScripts = flagValue;
                break;
            case "NOLIFTDROP":
                currentDefinition.Flags.NoLiftDrop = flagValue;
                break;
            case "NOMENU":
                currentDefinition.Flags.NoMenu = flagValue;
                break;
            case "NONSHOOTABLE":
                currentDefinition.Flags.NonShootable = flagValue;
                break;
            case "NOPAIN":
                currentDefinition.Flags.NoPain = flagValue;
                break;
            case "NORADIUSDMG":
                currentDefinition.Flags.NoRadiusDmg = flagValue;
                break;
            case "NOSECTOR":
                currentDefinition.Flags.NoSector = flagValue;
                break;
            case "NOSKIN":
                currentDefinition.Flags.NoSkin = flagValue;
                break;
            case "NOSPLASHALERT":
                currentDefinition.Flags.NoSplashAlert = flagValue;
                break;
            case "NOTARGET":
                currentDefinition.Flags.NoTarget = flagValue;
                break;
            case "NOTARGETSWITCH":
                currentDefinition.Flags.NoTargetSwitch = flagValue;
                break;
            case "NOTAUTOAIMED":
                currentDefinition.Flags.NotAutoaimed = flagValue;
                break;
            case "NOTDMATCH":
                currentDefinition.Flags.NotDMatch = flagValue;
                break;
            case "NOTELEFRAG":
                currentDefinition.Flags.NoTelefrag = flagValue;
                break;
            case "NOTELEOTHER":
                currentDefinition.Flags.NoTeleOther = flagValue;
                break;
            case "NOTELEPORT":
                currentDefinition.Flags.NoTeleport = flagValue;
                break;
            case "NOTELESTOMP":
                currentDefinition.Flags.NoTelestomp = flagValue;
                break;
            case "NOTIMEFREEZE":
                currentDefinition.Flags.NoTimeFreeze = flagValue;
                break;
            case "NOTONAUTOMAP":
                currentDefinition.Flags.NotOnAutomap = flagValue;
                break;
            case "NOTRIGGER":
                currentDefinition.Flags.NoTrigger = flagValue;
                break;
            case "NOVERTICALMELEERANGE":
                currentDefinition.Flags.NoVerticalMeleeRange = flagValue;
                break;
            case "NOWALLBOUNCESND":
                currentDefinition.Flags.NoWallBounceSnd = flagValue;
                break;
            case "OLDRADIUSDMG":
                currentDefinition.Flags.OldRadiusDmg = flagValue;
                break;
            case "PAINLESS":
                currentDefinition.Flags.Painless = flagValue;
                break;
            case "PICKUP":
                currentDefinition.Flags.Pickup = flagValue;
                break;
            case "PIERCEARMOR":
                currentDefinition.Flags.PierceArmor = flagValue;
                break;
            case "POISONALWAYS":
                currentDefinition.Flags.PoisonAlways = flagValue;
                break;
            case "PROJECTILE":
                currentDefinition.Flags.Projectile = flagValue;
                break;
            case "PUFFGETSOWNER":
                currentDefinition.Flags.PuffGetsOwner = flagValue;
                break;
            case "PUFFONACTORS":
                currentDefinition.Flags.PuffOnActors = flagValue;
                break;
            case "PUSHABLE":
                currentDefinition.Flags.Pushable = flagValue;
                break;
            case "QUARTERGRAVITY":
                currentDefinition.Flags.QuarterGravity = flagValue;
                break;
            case "QUICKTORETALIATE":
                currentDefinition.Flags.QuickToRetaliate = flagValue;
                break;
            case "RANDOMIZE":
                currentDefinition.Flags.Randomize = flagValue;
                break;
            case "REFLECTIVE":
                currentDefinition.Flags.Reflective = flagValue;
                break;
            case "RELATIVETOFLOOR":
                currentDefinition.Flags.RelativeToFloor = flagValue;
                break;
            case "RIPPER":
                currentDefinition.Flags.Ripper = flagValue;
                break;
            case "ROCKETTRAIL":
                currentDefinition.Flags.RocketTrail = flagValue;
                break;
            case "ROLLCENTER":
                currentDefinition.Flags.RollCenter = flagValue;
                break;
            case "ROLLSPRITE":
                currentDefinition.Flags.RollSprite = flagValue;
                break;
            case "SCREENSEEKER":
                currentDefinition.Flags.ScreenSeeker = flagValue;
                break;
            case "SEEINVISIBLE":
                currentDefinition.Flags.SeeInvisible = flagValue;
                break;
            case "SEEKERMISSILE":
                currentDefinition.Flags.SeekerMissile = flagValue;
                break;
            case "SEESDAGGERS":
                currentDefinition.Flags.SeesDaggers = flagValue;
                break;
            case "SHADOW":
                currentDefinition.Flags.Shadow = flagValue;
                break;
            case "SHIELDREFLECT":
                currentDefinition.Flags.ShieldReflect = flagValue;
                break;
            case "SHOOTABLE":
                currentDefinition.Flags.Shootable = flagValue;
                break;
            case "SHORTMISSILERANGE":
                currentDefinition.Flags.ShortMissileRange = flagValue;
                break;
            case "SKULLFLY":
                currentDefinition.Flags.Skullfly = flagValue;
                break;
            case "SKYEXPLODE":
                currentDefinition.Flags.SkyExplode = flagValue;
                break;
            case "SLIDESONWALLS":
                currentDefinition.Flags.SlidesOnWalls = flagValue;
                break;
            case "SOLID":
                currentDefinition.Flags.Solid = flagValue;
                break;
            case "SPAWNCEILING":
                currentDefinition.Flags.SpawnCeiling = flagValue;
                break;
            case "SPAWNFLOAT":
                currentDefinition.Flags.SpawnFloat = flagValue;
                break;
            case "SPAWNSOUNDSOURCE":
                currentDefinition.Flags.SpawnSoundSource = flagValue;
                break;
            case "SPECIAL":
                currentDefinition.Flags.Special = flagValue;
                break;
            case "SPECIALFIREDAMAGE":
                currentDefinition.Flags.SpecialFireDamage = flagValue;
                break;
            case "SPECIALFLOORCLIP":
                currentDefinition.Flags.SpecialFloorClip = flagValue;
                break;
            case "SPECTRAL":
                currentDefinition.Flags.Spectral = flagValue;
                break;
            case "SPRITEANGLE":
                currentDefinition.Flags.SpriteAngle = flagValue;
                break;
            case "SPRITEFLIP":
                currentDefinition.Flags.SpriteFlip = flagValue;
                break;
            case "STANDSTILL":
                currentDefinition.Flags.StandStill = flagValue;
                break;
            case "STAYMORPHED":
                currentDefinition.Flags.StayMorphed = flagValue;
                break;
            case "STEALTH":
                currentDefinition.Flags.Stealth = flagValue;
                break;
            case "STEPMISSILE":
                currentDefinition.Flags.StepMissile = flagValue;
                break;
            case "STRIFEDAMAGE":
                currentDefinition.Flags.StrifeDamage = flagValue;
                break;
            case "SUMMONEDMONSTER":
                currentDefinition.Flags.SummonedMonster = flagValue;
                break;
            case "SYNCHRONIZED":
                currentDefinition.Flags.Synchronized = flagValue;
                break;
            case "TELEPORT":
                currentDefinition.Flags.Teleport = flagValue;
                break;
            case "TELESTOMP":
                currentDefinition.Flags.Telestomp = flagValue;
                break;
            case "THRUACTORS":
                currentDefinition.Flags.ThruActors = flagValue;
                break;
            case "THRUGHOST":
                currentDefinition.Flags.ThruGhost = flagValue;
                break;
            case "THRUREFLECT":
                currentDefinition.Flags.ThruReflect = flagValue;
                break;
            case "THRUSPECIES":
                currentDefinition.Flags.ThruSpecies = flagValue;
                break;
            case "TOUCHY":
                currentDefinition.Flags.Touchy = flagValue;
                break;
            case "USEBOUNCESTATE":
                currentDefinition.Flags.UseBounceState = flagValue;
                break;
            case "USEKILLSCRIPTS":
                currentDefinition.Flags.UseKillScripts = flagValue;
                break;
            case "USESPECIAL":
                currentDefinition.Flags.UseSpecial = flagValue;
                break;
            case "VISIBILITYPULSE":
                currentDefinition.Flags.VisibilityPulse = flagValue;
                break;
            case "VULNERABLE":
                currentDefinition.Flags.Vulnerable = flagValue;
                break;
            case "WALLSPRITE":
                currentDefinition.Flags.WallSprite = flagValue;
                break;
            case "WINDTHRUST":
                currentDefinition.Flags.WindThrust = flagValue;
                break;
            case "ZDOOMTRANS":
                currentDefinition.Flags.ZdoomTrans = flagValue;
                break;
            default:
                Log.Warn("Unknown flag '{0}' for actor {1}", flagName, currentDefinition.Name);
                break;
            }
        }
    }
}

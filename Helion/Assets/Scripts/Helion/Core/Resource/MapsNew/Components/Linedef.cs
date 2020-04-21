using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;

namespace Helion.Core.Resource.MapsNew.Components
{
    public class Linedef
    {
        public readonly int Index;
        public int LineID;
        public int FrontSideID;
        public int? BackSideID;
        public bool BlockThings;
        public bool BlockMonsters;
        public bool UpperUnpegged;
        public bool LowerUnpegged;
        public bool BlockSound;
        public bool DrawAsOneSidedMinimap;
        public bool AlwaysDrawnMinimap;
        public bool NeverDrawnMinimap;
        public bool PassUse;
        public bool Translucent;
        public bool JumpOver;
        public bool BlockFloaters;
        public bool PlayerCanCross;
        public bool PlayerCanUse;
        public bool MonsterCanCross;
        public bool MonsterCanUse;
        public bool ProjectileActivates;
        public bool PlayerCanPush;
        public bool MonsterCanPush;
        public bool ProjectileCanCross;
        public bool RepeatableSpecial;
        public int SpecialNumber;
        public int arg0;
        public int arg1;
        public int arg2;
        public int arg3;
        public int arg4;
        public string Comment = string.Empty;

        // ZDoom specific.
        public float Alpha;
        public RenderStyle RenderStyle = RenderStyle.Normal;
        public bool PlayerUseBack;
        public bool AnyNonProjectileCrossTriggers;
        public bool MonsterCanActivate;
        public bool BlocksPlayers;
        public bool BlocksEverything;
        public bool OnlyTriggeredFromFront;
        public bool ReverbZoneBoundary;
        public bool ClipMidTexture;
        public bool WrapMidTexture;
        public bool CanWalkOnMidTexture;
        public bool MidTextureBlocksAllButProjectiles;
        public bool OnlyActivateIfVerticallyReachable;
        public bool BlockProjectiles;
        public bool BlockUse;
        public bool BlockSight;
        public bool BlockHitscan;
        public int LockNumber;
        public string Arg0String = string.Empty;
        public int[] LineIDsAdditional = { };
        public bool AlphaQuarter;
        public int AutomapStyle;
        public bool VisibleOnAutomap;
        public bool SkiesNotDrawnAboveOrBelow;
        public bool DrawToMaxHeight;
        public int? Hitpoints;
        public int HealthGroup;
        public bool DamageInvokesSpecial;
        public bool DeathInvokesSpecial;

        public Linedef(int index)
        {
            Index = index;
        }
    }
}

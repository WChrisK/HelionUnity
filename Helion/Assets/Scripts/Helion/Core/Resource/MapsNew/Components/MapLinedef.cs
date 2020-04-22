using Helion.Bsp.Geometry;
using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;

namespace Helion.Core.Resource.MapsNew.Components
{
    public class MapLinedef : IBspUsableLine
    {
        public int Index { get; }
        public int StartVertex;
        public int EndVertex;
        public int FrontSide;
        public int? BackSide;
        public int? LineID;
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
        public int Arg0;
        public int Arg1;
        public int Arg2;
        public int Arg3;
        public int Arg4;
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

        // Helper interface functions.
        public bool OneSided => BackSide != null;

        public MapLinedef(int index)
        {
            Index = index;
        }
    }
}

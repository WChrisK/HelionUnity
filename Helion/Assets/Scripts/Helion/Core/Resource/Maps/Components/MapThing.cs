using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using UnityEngine;

namespace Helion.Core.Resource.Maps.Components
{
    public class MapThing
    {
        public readonly int Index;
        public int ThingID;
        public Vector2 Position;
        public float? AbsoluteHeight; // We added this for Hexen support.
        public float? RelativeHeight;
        public int AngleDegrees; // 0 is East, 90 North, etc.
        public bool Skill1;
        public bool Skill2;
        public bool Skill3;
        public bool Skill4;
        public bool Skill5;
        public bool Deaf;
        public bool InSinglePlayer;
        public bool InDeathmatch;
        public bool InCooperative;
        public bool Friendly;
        public bool Dormant;
        public bool Class1;
        public bool Class2;
        public bool Class3;
        public bool StrifeStanding;
        public bool StrifeAlly;
        public bool StrifeTranslucent;
        public bool StrifeInvisible;
        public int Special;
        public int Arg0;
        public int Arg1;
        public int Arg2;
        public int Arg3;
        public int Arg4;
        public string Comment = string.Empty;

        // ZDoom specific.
        public bool Skill6;
        public bool Skill7;
        public bool Skill8;
        public bool Skill9;
        public bool Skill10;
        public bool Skill11;
        public bool Skill12;
        public bool Skill13;
        public bool Skill14;
        public bool Skill15;
        public bool Skill16;
        public bool Class4;
        public bool Class5;
        public bool Class6;
        public bool Class7;
        public bool Class8;
        public bool Class9;
        public bool Class10;
        public bool Class11;
        public bool Class12;
        public bool Class13;
        public bool Class14;
        public bool Class15;
        public bool Class16;
        public int ConversationID;
        public bool CountSecret;
        public string Arg0String = string.Empty;
        public string Arg1String = string.Empty;
        public float GravityFactor = 1.0f;
        public float HealthFactor = 1.0f;
        public RenderStyle RenderStyle = RenderStyle.None;
        public int FillColor;
        public float Alpha = 1.0f;
        public int Score;
        public int PitchDegrees;
        public int RollDegrees;
        public Vector2 ScaleXY = Vector2.one;
        public float Scale = 1.0f;
        public int FloatBobPhase;

        public MapThing(int index)
        {
            Index = index;
        }
    }
}

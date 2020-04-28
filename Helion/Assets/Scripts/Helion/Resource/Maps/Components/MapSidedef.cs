using Helion.Util;
using Helion.Util.Geometry;
using Helion.Util.Geometry.Vectors;
using UnityEngine;

namespace Helion.Resource.Maps.Components
{
    public class MapSidedef
    {
        public readonly int Index;
        public int SectorID;
        public Vec2I Offset = Vec2I.Zero;
        public UpperString UpperTexture = Constants.NoTexture;
        public UpperString MiddleTexture = Constants.NoTexture;
        public UpperString LowerTexture = Constants.NoTexture;
        public string Comment = string.Empty;

        // ZDoom specific.
        public Vector2 ScaleTop = Vector2.one;
        public Vector2 ScaleMiddle = Vector2.one;
        public Vector2 ScaleBottom = Vector2.one;
        public Vec2I OffsetTop = Vec2I.Zero;
        public Vec2I OffsetMiddle = Vec2I.Zero;
        public Vec2I OffsetBottom = Vec2I.Zero;
        public int LightDelta;
        public bool LightDeltaIsAbsolute;
        public bool LightInFog;
        public bool DisableFakeContrast;
        public bool SmoothLighting;
        public bool ClipMiddleTexture;
        public bool NoDecals;
        public bool NoGradientTop;
        public bool FlipGradientTop;
        public bool ClampGradientTop;
        public bool UseSidedefColorsOverSectorTop;
        public float ColorScaleFactorTop = 1.0f;
        public int UpperColorTop;
        public int LowerColorTop;
        public bool UseOwnColorTop;
        public int ColorAddTop;
        public int ColorizationTop;
        public bool NoGradientMiddle;
        public bool FlipGradientMiddle;
        public bool ClampGradientMiddle;
        public bool UseSidedefColorsOverSectorMiddle;
        public float ColorScaleFactorMiddle = 1.0f;
        public int UpperColorMiddle;
        public int LowerColorMiddle;
        public bool UseOwnColorMiddle;
        public int ColorAddMiddle;
        public int ColorizationMiddle;
        public bool NoGradientBottom;
        public bool FlipGradientBottom;
        public bool ClampGradientBottom;
        public bool UseSidedefColorsOverSectorBottom;
        public int UpperColorBottom;
        public int LowerColorBottom;
        public float ColorScaleFactorBottom = 1.0f;
        public bool UseOwnColorBottom;
        public int ColorAddBottom;
        public int ColorizationBottom;

        public MapSidedef(int index)
        {
            Index = index;
        }
    }
}

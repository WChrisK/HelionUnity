using Helion.Core.Resource.Decorate.Definitions.Properties.Enums;
using UnityEngine;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public struct PowerupColor
    {
        public Color? Color;
        public PowerupColorType? ColorType;
        public float? Alpha;

        public PowerupColor(Color color, float alpha)
        {
            Color = color;
            ColorType = null;
            Alpha = alpha;
        }

        public PowerupColor(PowerupColorType colorType, float alpha)
        {
            Color = null;
            ColorType = colorType;
            Alpha = alpha;
        }
    }
}

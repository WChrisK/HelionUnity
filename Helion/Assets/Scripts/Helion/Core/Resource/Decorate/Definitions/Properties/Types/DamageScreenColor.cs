
using System.Collections.Generic;
using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    public class DamageScreenColor
    {
        public Color? Color;
        public float? Intensity;
        public readonly Dictionary<UpperString, DamageScreenColorComponent> DamageTypes = new Dictionary<UpperString, DamageScreenColorComponent>();

        public DamageScreenColor()
        {
        }

        public DamageScreenColor(DamageScreenColor other)
        {
            Color = other.Color;
            Intensity = other.Intensity;
            DamageTypes = new Dictionary<UpperString, DamageScreenColorComponent>(other.DamageTypes);
        }

        public struct DamageScreenColorComponent
        {
            public Color Color;
            public float? Intensity;

            public DamageScreenColorComponent(Color color, float? intensity)
            {
                Color = color;
                Intensity = intensity;
            }
        }
    }
}

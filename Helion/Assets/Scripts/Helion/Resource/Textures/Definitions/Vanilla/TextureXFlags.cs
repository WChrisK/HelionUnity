using System;

namespace Helion.Resource.Textures.Definitions.Vanilla
{
    /// <summary>
    /// Flags supported by TextureX.
    /// </summary>
    [Flags]
    public enum TextureXFlags : ushort
    {
        None = 0,
        WorldPanning = 0x8000
    }
}

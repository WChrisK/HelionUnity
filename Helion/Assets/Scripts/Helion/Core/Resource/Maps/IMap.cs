using Helion.Core.Util;

namespace Helion.Core.Resource.Maps
{
    /// <summary>
    /// Represents a map from an archive that has all the necessary components.
    /// </summary>
    public interface IMap
    {
        /// <summary>
        /// The name of the map.
        /// </summary>
        UpperString Name { get; }

        /// <summary>
        /// The type of map this is.
        /// </summary>
        MapType Type { get; }
    }
}

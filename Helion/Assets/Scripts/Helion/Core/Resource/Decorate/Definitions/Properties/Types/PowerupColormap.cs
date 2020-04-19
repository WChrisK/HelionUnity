using UnityEngine;

namespace Helion.Core.Resource.Decorate.Definitions.Properties.Types
{
    // Note: This is intentional, the wiki says source is optional yet
    // come first.
    public struct PowerupColormap
    {
        public readonly Color? Source;
        public readonly Color Destination;

        public PowerupColormap(Color? source, Color destination)
        {
            Source = source;
            Destination = destination;
        }
    }
}

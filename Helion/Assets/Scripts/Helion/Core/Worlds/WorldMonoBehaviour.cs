using UnityEngine;

namespace Helion.Core.Worlds
{
    /// <summary>
    /// A component that tells a game object to update the world it is attached
    /// to.
    /// </summary>
    public class WorldMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// The world to update.
        /// </summary>
        public World World;

        void Update()
        {
            if (World == null)
                return;

            // TODO
        }

        private void FixedUpdate()
        {
            World?.Tick();
        }
    }
}

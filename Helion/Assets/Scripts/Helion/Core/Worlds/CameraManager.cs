using System;
using UnityEngine;

namespace Helion.Core.Worlds
{
    /// <summary>
    /// Manages the camera for the world so the renderer knows what to render
    /// from.
    /// </summary>
    public class CameraManager : IDisposable
    {
        private World world;

        public CameraManager(World world)
        {
            this.world = world;
        }

        /// <summary>
        /// Gets the camera that the world should be viewed through for the
        /// main rendering.
        /// </summary>
        /// <returns>The camera to use.</returns>
        public Camera FindCamera()
        {
            return Camera.main;
        }

        public void Dispose()
        {
            // TODO
        }
    }
}

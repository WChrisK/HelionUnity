using System;

namespace Helion.Configs.Components
{
    [Serializable]
    public class ConfigMouse
    {
        /// <summary>
        /// Whether we should use freelook or not.
        /// </summary>
        public bool FreeLook = true;

        /// <summary>
        /// A pitch-specific sensitivity. This is multiplied with
        /// <see cref="Sensitivity"/> to get the final Y movement.
        /// </summary>
        public float Pitch = 1.0f;

        /// <summary>
        /// A multiplier of both the pitch and yaw axes.
        /// </summary>
        public float Sensitivity = 1.0f;

        /// <summary>
        /// True to use raw input, false to use the smoothed version.
        /// </summary>
        public bool UseRawInput = true;

        /// <summary>
        /// A yaw-specific sensitivity. This is multiplied with
        /// <see cref="Sensitivity"/> to get the final X movement.
        /// </summary>
        public float Yaw = 1.0f;
    }
}

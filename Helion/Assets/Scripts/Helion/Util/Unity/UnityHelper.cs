namespace Helion.Util.Unity
{
    /// <summary>
    /// Helper code for generic Unity things.
    /// </summary>
    public static class UnityHelper
    {
        /// <summary>
        /// True if we're in the editor, false otherwise.
        /// </summary>
        /// <remarks>
        /// Intended primarily for only doing debug things when coupled with
        /// config settings.
        /// </remarks>
#if UNITY_EDITOR
        public static readonly bool InEditor = true;
#else
        public static readonly bool InEditor = false;
#endif

        /// <summary>
        /// Converts an angle from either Doom or Unity format to the other
        /// format. This is used to convert between the "East is 0 bits"
        /// angle in the doom format and "+Z axis is 0". This function also
        /// works in both directions, meaning `f(f(x)) = x`.
        /// </summary>
        /// <param name="angle">The angle to convert.</param>
        /// <returns>The converted angle.</returns>
        public static float DoomUnityAngleConverter(float angle) => 90 - angle;
    }
}

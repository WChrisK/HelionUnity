namespace Helion.Core.Util.Unity
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
    }
}

namespace Helion.Core.WorldNew
{
    /// <summary>
    /// Indicates the object can be ticked.
    /// </summary>
    public interface ITickable
    {
        /// <summary>
        /// Runs a world simulation for the tickable instance.
        /// </summary>
        void Tick();
    }
}

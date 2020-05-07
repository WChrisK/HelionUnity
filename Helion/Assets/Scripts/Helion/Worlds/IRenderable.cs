namespace Helion.Worlds
{
    /// <summary>
    /// Indicates an object can be updated for each frame.
    /// </summary>
    public interface IRenderable
    {
        /// <summary>
        /// Updates the object so that it is up to date for being rendered.
        /// </summary>
        /// <param name="tickFraction">The fraction between 0.0 and 1.0 for
        /// interpolation purposes.</param>
        void Update(float tickFraction);
    }
}

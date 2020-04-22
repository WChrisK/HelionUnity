namespace Helion.Bsp.Geometry
{
    /// <summary>
    /// Indicates a line can be used for BSP actions within the BSP builder.
    /// </summary>
    public interface IBspUsableLine
    {
        /// <summary>
        /// A unique identifier for this line.
        /// </summary>
        /// <remarks>
        /// This can also mean the ID of the line in the LINEDEFS entry.
        /// </remarks>
        int Index { get; }

        /// <summary>
        /// If the line is one-sided or not.
        /// </summary>
        bool OneSided { get; }
    }
}

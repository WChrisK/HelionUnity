using UnityEngine;

namespace Helion.Core.Resource.Maps.Doom
{
    /// <summary>
    /// An entity in a Doom map.
    /// </summary>
    public class DoomThing
    {
        /// <summary>
        /// The position in the world.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The bit angle of the thing.
        /// </summary>
        public readonly ushort Angle;

        /// <summary>
        /// The editor number.
        /// </summary>
        public readonly ushort EditorNumber;

        /// <summary>
        /// Creates a Doom map thing.
        /// </summary>
        /// <param name="position">The position in the map.</param>
        /// <param name="angle">The bit angle from 0 - 65535.</param>
        /// <param name="editorNumber">The editor number of the thing.</param>
        /// <param name="flags">The flags for the thing.</param>
        public DoomThing(Vector2 position, ushort angle, ushort editorNumber, ushort flags)
        {
            Position = position;
            Angle = angle;
            EditorNumber = editorNumber;
        }
    }
}

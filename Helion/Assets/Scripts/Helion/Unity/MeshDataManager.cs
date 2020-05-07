using UnityEngine;

namespace Helion.Unity
{
    /// <summary>
    /// Because updating Unity mesh properties causes a performance hit, and
    /// since Unity only updates if the references changes, we need a buffer
    /// strategy.
    /// </summary>
    public class MeshDataManager
    {
        private int currentColorBuffer;

        private readonly Color[][] colorBuffers =
        {
            new[] { Color.white, Color.white, Color.white, Color.white },
            new[] { Color.white, Color.white, Color.white, Color.white }
        };

        /// <summary>
        /// Gets one of the swapped buffers and returns that new buffer with
        /// the colors populated in it.
        /// </summary>
        /// <param name="color">The color to use.</param>
        /// <returns>A buffer full of the color provided.</returns>
        public Color[] ColorBufferSwap(Color color)
        {
            Color[] buffer = colorBuffers[currentColorBuffer];
            for (int i = 0; i < buffer.Length; i++)
                buffer[i] = color;

            AdvanceBufferIndex(ref currentColorBuffer);
            return buffer;
        }

        private void AdvanceBufferIndex(ref int index)
        {
            index = (index + 1) % 2;
        }
    }
}

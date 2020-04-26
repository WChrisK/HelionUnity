using System;
using UnityEngine;

namespace Helion.Util.Geometry
{
    public struct BitAngle
    {
        /// <summary>
        /// The rotation angle in diamond angle format. This is equal to 180
        /// degrees + 22.5 degrees. See <see cref="CalculateSpriteRotation"/>
        /// docs for more information.
        /// </summary>
        private const uint SpriteFrameRotationAngle = 9 * (uint.MaxValue / 16);
        private const uint DiamondScale = uint.MaxValue / 4;
        private const double RadiansToDiamondAngleFactor = uint.MaxValue / (2 * Math.PI);

        public static readonly BitAngle East = new BitAngle(0);
        public static readonly BitAngle NorthEast = new BitAngle(1 * (uint.MaxValue / 8));
        public static readonly BitAngle North = new BitAngle(2 * (uint.MaxValue / 8));
        public static readonly BitAngle NorthWest = new BitAngle(3 * (uint.MaxValue / 8));
        public static readonly BitAngle West = new BitAngle(4 * (uint.MaxValue / 8));
        public static readonly BitAngle SouthWest = new BitAngle(5 * (uint.MaxValue / 8));
        public static readonly BitAngle South = new BitAngle(6 * (uint.MaxValue / 8));
        public static readonly BitAngle SouthEast = new BitAngle(7 * (uint.MaxValue / 8));

        public uint Bits;

        /// <summary>
        /// Gets the angle but in degrees. East in the world (or the positive X
        /// axis) is considered zero and this rotates counter-clockwise if we
        /// were looking down along the X/Z plane in a birds eye view.
        /// </summary>
        public float Degrees => ((float)Bits * 360 / uint.MaxValue);

        /// <summary>
        /// Gets the angle but in radians. East in the world (or the positive X
        /// axis) is considered zero and this rotates counter-clockwise if we
        /// were looking down along the X/Z plane in a birds eye view.
        /// </summary>
        public float Radians => (float)(Bits * Math.PI / uint.MaxValue);

        /// <summary>
        /// Gets the quaternion rotation for this angle.
        /// </summary>
        /// <remarks>
        /// A rotation of zero is straight ahead along the Z axis.
        /// Increasing the angle rotates right. This is the opposite of the
        /// world where East is zero and it rotates left. Therefore we do a
        /// right rotation of 90 degrees, and then go left by subtracting
        /// the degrees.
        /// </remarks>
        public Quaternion Quaternion => Quaternion.Euler(0, 90 - Degrees, 0);

        public BitAngle(uint bits)
        {
            Bits = bits;
        }

        public BitAngle(float radians)
        {
            Bits = DiamondAngleFromRadians(radians);
        }

        public static BitAngle FromDegrees(int degrees)
        {
            return new BitAngle((uint)((ulong)degrees * uint.MaxValue / 360));
        }

        public static BitAngle FromByteAngle(int byteAngle)
        {
            return new BitAngle((uint)((ulong)byteAngle * uint.MaxValue / 255));
        }

        public static uint DiamondAngleFromRadians(double radians)
        {
            return unchecked((uint)(radians * RadiansToDiamondAngleFactor));
        }

        /// <summary>
        /// Takes two positions and finds the diamond angle that exists from
        /// start to end. This is also known as the vector angle, but the
        /// calculations find it with respect to being a diamond. The diamond
        /// angle is an ordered angle that is similar to degrees or radians,
        /// and has absolute ordering.
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/questions/1427422/cheap-algorithm-to-find-measure-of-angle-between-vectors
        /// is where the optimization was learned from.
        /// </remarks>
        /// <param name="start">The origin.</param>
        /// <param name="end">The endpoint from the origin forming a vector.
        /// </param>
        /// <returns>The diamond angle for the vertex. This will be zero if the
        /// start and end vertices are the same.</returns>
        public static uint ToDiamondAngle(Vector2 start, Vector2 end)
        {
            // The code below takes some position and finds the vector from the
            // center to the position.
            //
            // It then is able to take the X and Y components of this vector,
            // and turn them into some ratio between [0.0, 4.0) as follows for
            // this table:
            //
            //      X,  Y    Result
            //     -----------------
            //      1,  0     0.0
            //      0,  1     1.0
            //     -1,  0     2.0
            //      0, -1     3.0
            //
            // As such, we can then multiply it by a big number to turn it into
            // a value between [0, 2^32). The key here is that we get an order
            // out of the values, because this allows us to see what angles are
            // blocked or not by mapping every position onto a unit circle with
            // 2^32 precision.
            Vector2 pos = end - start;
            if (pos == Vector2.zero)
                return 0;

            // TODO: Can we fuse two if statements into one statement somehow?
            if (pos.y >= 0)
            {
                if (pos.x >= 0)
                    return (uint)(DiamondScale * (pos.y / (pos.x + pos.y)));
                return (uint)(DiamondScale * (1 - (pos.x / (-pos.x + pos.y))));
            }

            if (pos.x < 0)
                return (uint)(DiamondScale * (2 - (pos.y / (-pos.x - pos.y))));
            return (uint)(DiamondScale * (3 + (pos.x / (pos.x - pos.y))));
        }

        // TODO
        // public static int CalculateSpriteRotation(Entity viewer, Entity target)
        // {
        //     uint viewAngle = ToDiamondAngle(viewer.Position, target.Position);
        //     uint entityAngle = DiamondAngleFromRadians(viewer.Angle.Radians);
        //     return (int)CalculateSpriteRotation(viewAngle, entityAngle);
        // }

        private static uint CalculateSpriteRotation(uint viewAngle, uint entityAngle)
        {
            // This works as follows:
            //
            // First we find the angle that we have to the entity. Since
            // facing along with the actor (ex: looking at their back) wants to
            // give us the opposite rotation side, we add 180 degrees to our
            // angle delta.
            //
            // Then we add 22.5 degrees to that as well because we don't want
            // a transition when we hit 180 degrees... we'd rather have ranges
            // of [180 - 22.5, 180 + 22.5] be the angle rather than the range
            // [180 - 45, 180].
            //
            // Then we can do a bit shift trick which converts the higher order
            // three bits into the angle rotation between 0 - 7.
            return unchecked((viewAngle - entityAngle + SpriteFrameRotationAngle) >> 29);
        }
    }
}

using UnityEngine;

namespace Helion.Util.Geometry.Vectors
{
    public static class Vec3F
    {
        public static Vector3 UnitFromDegrees(float degrees)
        {
            float radians = degrees * Mathf.Deg2Rad;
            return new Vector3(Mathf.Cos(radians), 0, Mathf.Sin(radians));
        }
    }
}

using Helion.Util;
using Helion.Util.Geometry.Vectors;

namespace Helion.Worlds.Entities.Physics
{
    /// <summary>
    /// Responsible for doing all of the physics calculations.
    /// </summary>
    public class PhysicsSystem
    {
        public const float ColliderThickness = 0.02f;
        private const float Friction = 0.90625f;
        private static readonly Vec3F FrictionXZScale = (Friction, 1, Friction);

        private World world;

        public PhysicsSystem(World world)
        {
            this.world = world;
        }

        public void TryMove(Entity entity)
        {
            entity.Position += entity.Velocity;

            ApplyFriction(entity);
        }

        private void ApplyFriction(Entity entity)
        {
            entity.Velocity *= FrictionXZScale;
        }
    }
}

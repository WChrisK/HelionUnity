using Helion.Worlds.Entities;
using Helion.Worlds.Geometry.Subsectors;
using Helion.Worlds.Geometry.Walls;
using UnityEngine;

namespace Helion.Unity
{
    /// <summary>
    /// Designed to be held on by some collision object. This allows us to do a
    /// quick lookup of what the collision type was.
    /// </summary>
    /// <remarks>
    /// When we have a collision that is found from the physics engine, it will
    /// tell us what the game object is... but that is all. To quickly look up
    /// what type of thing we hit, a call to GetComponent() with CollisionInfo
    /// can be done, and inside it'll share
    /// </remarks>
    public class CollisionInfo : MonoBehaviour
    {
        public CollisionInfoType InfoType;
        public Entity Entity;
        public SubsectorPlane SubsectorPlane;
        public Wall Wall;

        /// <summary>
        /// Creates a collision info component on the game object which is for
        /// an entity.
        /// </summary>
        /// <param name="gameObject">The game object to add a collision info
        /// behavior to.</param>
        /// <param name="entity">The object.</param>
        /// <returns>The created collision info.</returns>
        public static CollisionInfo CreateOn(GameObject gameObject, Entity entity)
        {
            CollisionInfo info = gameObject.AddComponent<CollisionInfo>();
            info.InfoType = CollisionInfoType.Entity;
            info.Entity = entity;
            return info;
        }

        /// <summary>
        /// Creates a collision info component on the game object which is for
        /// a plane.
        /// </summary>
        /// <param name="gameObject">The game object to add a collision info
        /// behavior to.</param>
        /// <param name="plane">The object.</param>
        /// <returns>The created collision info.</returns>
        public static CollisionInfo CreateOn(GameObject gameObject, SubsectorPlane plane)
        {
            CollisionInfo info = gameObject.AddComponent<CollisionInfo>();
            info.InfoType = CollisionInfoType.SubsectorPlane;
            info.SubsectorPlane = plane;
            return info;
        }

        /// <summary>
        /// Creates a collision info component on the game object which is for
        /// a wall.
        /// </summary>
        /// <param name="gameObject">The game object to add a collision info
        /// behavior to.</param>
        /// <param name="wall">The object.</param>
        /// <returns>The created collision info.</returns>
        public static CollisionInfo CreateOn(GameObject gameObject, Wall wall)
        {
            CollisionInfo info = gameObject.AddComponent<CollisionInfo>();
            info.InfoType = CollisionInfoType.Wall;
            info.Wall = wall;
            return info;
        }
    }

    /// <summary>
    /// The type of collision info for being able to switch() on.
    /// </summary>
    public enum CollisionInfoType
    {
        Entity,
        SubsectorPlane,
        Wall
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Unity;
using Helion.Util.Geometry.Boxes;
using Helion.Util.Geometry.Segments;
using Helion.Util.Unity;
using UnityEngine;

namespace Helion.Worlds.Geometry.Subsectors
{
    public class SubsectorPlane : IDisposable
    {
        public readonly int Index;
        public readonly SectorPlane SectorPlane;
        public readonly List<Seg2F> Edges;
        public readonly SubsectorMeshComponents MeshComponents;
        public Subsector Subsector { get; internal set; }
        private readonly GameObject gameObject;
        private readonly CollisionInfo collisionInfo;

        public SubsectorPlane(int index, SectorPlane sectorPlane, List<Seg2F> edges)
        {
            Debug.Assert(edges.Count >= 3, $"Two or less lines is not a polygon (subsector plane {index})");

            string facingText = sectorPlane.IsCeiling ? "Ceiling" : "Floor";

            Index = index;
            SectorPlane = sectorPlane;
            Edges = edges.ToList();
            gameObject = new GameObject($"Subsector {index} (Sector {sectorPlane.Sector.Index} Plane {sectorPlane.Index} {facingText})");
            MeshComponents = new SubsectorMeshComponents(this, sectorPlane, edges, gameObject);
            collisionInfo = CollisionInfo.CreateOn(gameObject, this);

            sectorPlane.SubsectorPlanes.Add(this);
        }

        public void UpdateMeshes()
        {
            // TODO
        }

        /// <summary>
        /// To be called to check if a box intersects this subsector plane.
        /// This is usually the bounding box of an entity being moved to some
        /// position.
        /// </summary>
        /// <param name="box">The box to check.</param>
        /// <returns>True if it's intersected by the box (meaning the subsector
        /// plane slices through it at some point), false otherwise.</returns>
        public bool IntersectedBy(in Box3F box)
        {
            Box2F box2D = box.XZ;
            bool insideSubsector = true;

            for (int i = 0; i < Edges.Count; i++)
            {
                if (Edges[i].Intersects(box2D))
                    return HeightIntersectedBy(box);

                // This is for the case when we are not intersecting any edge.
                // If we have no intersections, then all our box vertices are
                // either inside or outside. What we'll do is check some vertex
                // arbitrarily against every edge, and we know we're inside if
                // the arbitrary vertex is always on the right side of every
                // single edge (because a subsector is all clockwise edges).
                insideSubsector &= Edges[i].OnRight(box.Min);
            }

            if (insideSubsector)
                return HeightIntersectedBy(box);

            return false;
        }

        public void Dispose()
        {
            MeshComponents.Dispose();
            GameObjectHelper.Destroy(collisionInfo);
            GameObjectHelper.Destroy(gameObject);
        }

        private bool HeightIntersectedBy(in Box3F box)
        {
            // We consider intersections to not be touching, it has to fully
            // cut through the height of the box.
            // TODO: Will not play nicely with slopes...
            return box.Min.Y < SectorPlane.Height && SectorPlane.Height < box.Max.Y;
        }
    }
}

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

        public bool IntersectedBy(in Box3F box)
        {
            Box2F box2D = box.XZ;

            // Doing iteration by index to reduce GC pressure.
            for (int i = 0; i < Edges.Count; i++)
                if (Edges[i].Intersects(box2D))
                    return HeightIntersectedBy(box);

            // If we're not intersecting any lines, then we are either fully
            // inside or fully outside. We can check this by taking any point
            // (we'll use the bottom left corner) and seeing if it is on the
            // right side of the first edge. Since the subsectors are clockwise
            // then being on the right means it's inside (by definition).
            if (Edges[0].OnRight(box.Min))
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

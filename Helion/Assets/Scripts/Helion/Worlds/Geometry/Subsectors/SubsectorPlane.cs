using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Unity;
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

        public void Dispose()
        {
            MeshComponents.Dispose();
            GameObjectHelper.Destroy(collisionInfo);
            GameObjectHelper.Destroy(gameObject);
        }
    }
}

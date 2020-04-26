using System;
using System.Collections.Generic;
using Helion.Core.Util.Geometry.Segments;
using Helion.Core.Util.Unity;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry.Subsectors
{
    public class SubsectorPlane : IDisposable
    {
        public readonly int Index;
        public readonly SectorPlane SectorPlane;
        public readonly SubsectorMeshComponents MeshComponents;
        public Subsector Subsector { get; internal set; }
        private readonly GameObject gameObject;

        public SubsectorPlane(int index, SectorPlane sectorPlane, List<Seg2F> edges)
        {
            string facingText = sectorPlane.IsCeiling ? "Ceiling" : "Floor";

            Index = index;
            SectorPlane = sectorPlane;
            gameObject = new GameObject($"Subsector {index} (Sector {sectorPlane.Sector.Index} Plane {sectorPlane.Index} {facingText})");
            MeshComponents = new SubsectorMeshComponents(this, sectorPlane, edges, gameObject);

            sectorPlane.SubsectorPlanes.Add(this);
        }

        public void UpdateMeshes()
        {
            // TODO
        }

        public void Dispose()
        {
            MeshComponents.Dispose();
            GameObjectHelper.Destroy(gameObject);
        }
    }
}

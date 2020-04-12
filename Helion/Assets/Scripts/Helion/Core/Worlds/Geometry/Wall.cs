using Helion.Core.Resource;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    public class Wall
    {
        private GameObject gameObject;

        public Wall(DoomSidedef side)
        {
            DoomLinedef line = side.Line;
            DoomSector sector = side.Sector;

            float lightLevel = sector.LightLevel * Constants.InverseLightLevel;

            //=================================================================
            gameObject = new GameObject($"Wall_L{line.Index}_S{side.Index}_Middle");
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = GameData.Resources.TextureManager.FindMaterial(side.MiddleTexture);
            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            Vector3[] vertices =
            {
                new Vector3(line.Start.X, sector.FloorHeight, line.Start.Y) * Constants.MapUnit,
                new Vector3(line.End.X, sector.FloorHeight, line.End.Y) * Constants.MapUnit,
                new Vector3(line.Start.X, sector.CeilingHeight, line.Start.Y) * Constants.MapUnit,
                new Vector3(line.End.X, sector.CeilingHeight, line.End.Y) * Constants.MapUnit
            };

            // TODO: Understand me...
            Vector3 normal = (vertices[0] - vertices[1]).normalized;
            float z = normal.z;
            float x = normal.x;
            normal.x = -z;
            normal.z = x;

            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = new[]
                {
                    0, 2, 1, 2, 3, 1
                },
                normals = new[]
                {
                    normal,
                    normal,
                    normal,
                    normal
                },
                uv = new[]
                {
                    // TODO: Inverted (for now...)
                    new Vector2(0, 1),
                    new Vector2(1, 1),
                    new Vector2(0, 0),
                    new Vector2(1, 0)
                },
                colors = new[]
                {
                    new Color(lightLevel, lightLevel, lightLevel, 1.0f),
                    new Color(lightLevel, lightLevel, lightLevel, 1.0f),
                    new Color(lightLevel, lightLevel, lightLevel, 1.0f),
                    new Color(lightLevel, lightLevel, lightLevel, 1.0f)
                }
            };

            meshFilter.sharedMesh = mesh;
            mesh.RecalculateBounds();
            // TODO: mesh.RecalculateNormals()? mesh.RecalculateTangents()?

            // TODO: Should only do this if the line is blocking.
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
            //=================================================================
        }
    }
}

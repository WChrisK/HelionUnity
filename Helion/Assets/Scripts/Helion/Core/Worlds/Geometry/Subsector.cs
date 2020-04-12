using System;
using System.Linq;
using Helion.Core.Resource;
using Helion.Core.Resource.Maps.Doom;
using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    // TODO: Create parent sector transform, SetParent, then only move parent? Possible perf optimization!

    public class Subsector
    {
        private readonly GameObject floorGameObject;
        private GameObject ceilingGameObject;

        public Subsector(GLSubsector subsector)
        {
            floorGameObject = CreateFlat(subsector, true);
            ceilingGameObject = CreateFlat(subsector, false);
        }

        private GameObject CreateFlat(GLSubsector subsector, bool isFloor)
        {
            DoomSector sector = subsector.Sector;

            string suffix = isFloor ? "Floor" : "Ceiling";
            GameObject gameObject = new GameObject($"Subsector{subsector.Index}_{suffix}");

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = GameData.Resources.TextureManager.FindMaterial(sector.FloorTexture);

            Mesh mesh = new Mesh();

            int vertexCount = subsector.Segments.Count;

            Vector3[] vertices = new Vector3[vertexCount];
            int[] indices = new int[(vertexCount - 2) * 3];
            Vector2[] uvCoords = new Vector2[vertexCount];
            Vector3[] normals = new Vector3[vertexCount];
            Color[] colors = new Color[vertexCount];

            float y = isFloor ? sector.FloorHeight : sector.CeilingHeight;
            Vector3 normal = isFloor ? Vector3.up : Vector3.down;
            float lightLevel = sector.LightLevel * Constants.InverseLightLevel;
            Color sectorColor = new Color(lightLevel, lightLevel, lightLevel, 1.0f);

            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 vertex = subsector.Segments[i].Segment.Start;
                vertices[i] = new Vector3(vertex.x, y, vertex.y) * Constants.MapUnit;
                uvCoords[i] = new Vector2(vertex.x, vertex.y) / 64.0f; // TODO: Fix the divisor later...
                normals[i] = normal;
                colors[i] = sectorColor;
            }

            // We want the vertices to be: [0, 1, 2, 0, 2, 3, ... 0, n+1, n+2].
            for (int i = 0; i < vertexCount - 2; i++)
            {
                indices[i * 3] = 0;
                indices[(i * 3) + 1] = i + 1;
                indices[(i * 3) + 2] = i + 2;
            }

            // If it's a ceiling, we have to reverse them.
            if (!isFloor)
            {
                Array.Reverse(vertices, 0, vertices.Length);
                Array.Reverse(uvCoords, 0, uvCoords.Length);
            }

            mesh.vertices = vertices.ToArray();
            mesh.triangles = indices;
            mesh.normals = normals;
            mesh.uv = uvCoords;
            mesh.colors = colors;

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            return gameObject;
        }
    }
}

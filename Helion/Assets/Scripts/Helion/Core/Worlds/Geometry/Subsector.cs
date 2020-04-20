using System;
using Helion.Core.Resource;
using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Resource.Textures;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Unity;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    // TODO: Create parent sector transform, SetParent, then only move parent? Possible perf optimization!

    /// <summary>
    /// A clockwise convex subsector at the leaf of a BSP tree.
    /// </summary>
    public class Subsector
    {
        public readonly int Index;
        public readonly Sector Sector;
        private readonly GameObject floorGameObject;
        private readonly GameObject ceilingGameObject;

        public Subsector(GLSubsector subsector, Sector sector, GameObject parentGameObject)
        {
            Index = subsector.Index;
            Sector = sector;

            floorGameObject = CreateFlatObject(subsector, true);
            parentGameObject.SetChild(floorGameObject);
            ceilingGameObject = CreateFlatObject(subsector, false);
            parentGameObject.SetChild(ceilingGameObject);
        }

        private GameObject CreateFlatObject(GLSubsector glSubsector, bool isFloor)
        {
            string suffix = isFloor ? "Floor" : "Ceiling";
            GameObject gameObject = new GameObject($"Subsector{glSubsector.Index}_{suffix}");

            SectorPlane plane = isFloor ? Sector.FloorPlane : Sector.CeilingPlane;

            UpperString textureName = plane.TextureName;
            Material material = TextureManager.Material(textureName, ResourceNamespace.Flats);
            Texture texture = material.mainTexture;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;

            Mesh mesh = CreateMesh(glSubsector, plane, isFloor, texture);

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
            meshFilter.sharedMesh = mesh;

            // TODO: Should only do this if the subsector is blocking.
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;

            return gameObject;
        }

        private Mesh CreateMesh(GLSubsector glSubsector, SectorPlane plane, bool isFloor, Texture texture)
        {
            Mesh mesh = new Mesh();

            Vector3 normal = isFloor ? Vector3.up : Vector3.down;
            float lightLevel = plane.LightLevelNormalized;
            Color sectorColor = new Color(lightLevel, lightLevel, lightLevel, 1.0f);

            int vertexCount = glSubsector.Segments.Count;
            (Vector3[] vertices, Vector2[] uvCoords) = CreateVertices(vertexCount, glSubsector, plane, texture, isFloor);

            mesh.vertices = vertices;
            mesh.uv = uvCoords;
            mesh.triangles = CreateFanIndices(vertexCount);
            mesh.normals = Arrays.Create(vertexCount, normal);
            mesh.colors = Arrays.Create(vertexCount, sectorColor);

            return mesh;
        }

        private static (Vector3[] vertices, Vector2[] uvCoords) CreateVertices(int vertexCount,
            GLSubsector glSubsector, SectorPlane plane, Texture texture, bool isFloor)
        {
            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uvCoords = new Vector2[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 vertex = glSubsector.Segments[i].Segment.Start;
                vertices[i] = new Vector3(vertex.x, plane.Height, vertex.y).MapUnit();
                uvCoords[i] = new Vector2(vertex.x / texture.width, vertex.y / texture.height);
            }

            // If it's a ceiling, we have to reverse the triangle fan.
            if (!isFloor)
            {
                Array.Reverse(vertices, 0, vertices.Length);
                Array.Reverse(uvCoords, 0, uvCoords.Length);
            }

            return (vertices, uvCoords);
        }

        private static int[] CreateFanIndices(int vertexCount)
        {
            int[] indices = new int[(vertexCount - 2) * 3];

            // We want the vertices to be: [0, 1, 2, 0, 2, 3, ... 0, n+1, n+2].
            for (int i = 0; i < vertexCount - 2; i++)
            {
                indices[i * 3] = 0;
                indices[(i * 3) + 1] = i + 1;
                indices[(i * 3) + 2] = i + 2;
            }

            return indices;
        }
    }
}

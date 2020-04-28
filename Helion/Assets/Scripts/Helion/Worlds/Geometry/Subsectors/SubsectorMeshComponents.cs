using System;
using System.Collections.Generic;
using Helion.Resource;
using Helion.Resource.Textures;
using Helion.Util.Extensions;
using Helion.Util.Geometry.Segments;
using Helion.Util.Unity;
using UnityEngine;
using Texture = Helion.Resource.Textures.Texture;

namespace Helion.Worlds.Geometry.Subsectors
{
    public class SubsectorMeshComponents : IDisposable
    {
        public readonly Mesh Mesh;
        public readonly MeshFilter Filter;
        public readonly MeshRenderer Renderer;
        private readonly SubsectorPlane subsectorPlane;
        private Texture texture;

        public SubsectorMeshComponents(SubsectorPlane plane, SectorPlane sectorPlane,
            List<Seg2F> edges, GameObject gameObject)
        {
            subsectorPlane = plane;
            texture = TextureManager.Texture(plane.SectorPlane.TextureName, ResourceNamespace.Flats);
            Renderer = gameObject.AddComponent<MeshRenderer>();
            Filter = gameObject.AddComponent<MeshFilter>();
            Mesh = CreateMesh(sectorPlane, edges, texture);

            Renderer.sharedMaterial = texture.Material;
            Filter.sharedMesh = Mesh;
        }

        public void Dispose()
        {
            GameObjectHelper.Destroy(Mesh);
            GameObjectHelper.Destroy(Filter);
            GameObjectHelper.Destroy(Renderer);
        }

        internal void SetTexture(Texture newTexture)
        {
            texture = newTexture;
            Renderer.sharedMaterial = newTexture.Material;
        }

        private static Mesh CreateMesh(SectorPlane sectorPlane, List<Seg2F> edges, Texture texture)
        {
            (Vector3[] vertices, Vector2[] uvCoords) = CalculateVertices(sectorPlane, edges, texture);
            int[] indices = CalculateIndices(vertices.Length);
            Vector3[] normals = CalculateNormals(sectorPlane, vertices.Length);
            Color[] colors = CalculateColors(sectorPlane, vertices.Length);

            return new Mesh
            {
                vertices = vertices,
                triangles = indices,
                normals = normals,
                uv = uvCoords,
                colors = colors
            };
        }

        private static (Vector3[] vertices, Vector2[] uvCoords) CalculateVertices(SectorPlane plane,
            List<Seg2F> edges, Texture texture)
        {
            int vertexCount = edges.Count;
            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uvCoords = new Vector2[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                Vector2 vertex = edges[i].Start.UnityFloat();
                vertices[i] = new Vector3(vertex.x, plane.Height, vertex.y).MapUnit();
                uvCoords[i] = vertex * texture.InverseDimension;
            }

            // If it's a ceiling, we have to reverse the triangle fan to make
            // the facing side visible.
            if (plane.IsCeiling)
            {
                Array.Reverse(vertices, 0, vertices.Length);
                Array.Reverse(uvCoords, 0, uvCoords.Length);
            }

            return (vertices, uvCoords);
        }

        private static int[] CalculateIndices(int vertexCount)
        {
            int[] indices = new int[(vertexCount - 2) * 3];

            // We want the vertices to be: [0, 1, 2, 0, 2, 3, ... 0, n+1, n+2].
            int offset = 0;
            for (int i = 0; i < vertexCount - 2; i++)
            {
                indices[offset] = 0;
                indices[offset + 1] = i + 1;
                indices[offset + 2] = i + 2;

                offset += 3;
            }

            return indices;
        }

        private static Vector3[] CalculateNormals(SectorPlane sectorPlane, int vertexCount)
        {
            Vector3 normal = sectorPlane.IsCeiling ? Vector3.down : Vector3.up;
            return Arrays.Create(vertexCount, normal);
        }

        private static Color[] CalculateColors(SectorPlane sectorPlane, int vertexCount)
        {
            float lightLevel = sectorPlane.LightLevelNormalized;
            Color color = new Color(lightLevel, lightLevel, lightLevel, 1.0f);

            return Arrays.Create(vertexCount, color);
        }
    }
}

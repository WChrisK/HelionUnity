using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Resource;
using Helion.Resource.Textures;
using Helion.Util;
using Helion.Util.Extensions;
using Helion.Util.Geometry.Boxes;
using Helion.Util.Geometry.Segments;
using Helion.Util.Unity;
using Helion.Worlds.Entities.Physics;
using UnityEngine;
using Texture = Helion.Resource.Textures.Texture;

namespace Helion.Worlds.Geometry.Subsectors
{
    public class SubsectorMeshComponents : IDisposable
    {
        public readonly Mesh Mesh;
        public readonly MeshFilter Filter;
        public readonly MeshRenderer Renderer;
        public readonly BoxCollider Collider;
        private readonly SubsectorPlane subsectorPlane;
        private Texture texture;

        public SubsectorMeshComponents(SubsectorPlane plane, SectorPlane sectorPlane,
            List<Seg2F> edges, GameObject gameObject)
        {
            subsectorPlane = plane;
            texture = TextureManager.Texture(plane.SectorPlane.TextureName, ResourceNamespace.Flats);
            Mesh = CreateMesh(sectorPlane, edges, texture);
            Filter = CreateFilter(gameObject);
            Renderer = CreateRenderer(gameObject);
            Collider = CreateCollider(gameObject, edges);
        }

        public void Dispose()
        {
            GameObjectHelper.Destroy(Mesh);
            GameObjectHelper.Destroy(Filter);
            GameObjectHelper.Destroy(Renderer);
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


        private MeshFilter CreateFilter(GameObject gameObject)
        {
            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = Mesh;

            return filter;
        }

        private MeshRenderer CreateRenderer(GameObject gameObject)
        {
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = texture.Material;

            return renderer;
        }

        private BoxCollider CreateCollider(GameObject gameObject, List<Seg2F> edges)
        {
            Box2F box = Box2F.Combine(edges.Select(edge => edge.Box));
            (float x, float z) = box.Center;
            float y = subsectorPlane.SectorPlane.Height;

            // Because we don't want some thin value being made even thinner,
            // we scale it up by the map unit size. This way we don't risk any
            // objects passing through it.
            float colliderThickness = PhysicsSystem.ColliderThickness * Constants.MapUnitInverse;

            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(x, y, z).MapUnit();
            collider.size = new Vector3(box.Width, colliderThickness, box.Height).MapUnit();

            return collider;
        }
    }
}

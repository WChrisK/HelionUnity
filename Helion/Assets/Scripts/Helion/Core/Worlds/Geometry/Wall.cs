using Helion.Core.Resource;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Geometry;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    /// <summary>
    /// A quad that is part of a line and side. Each side is made up of 1 or 3
    /// walls whether it's one or two sided side. The wall is the atomic piece
    /// that is rendered for a line/side component.
    /// </summary>
    public class Wall
    {
        public readonly Side Side;
        public readonly UpperString TextureName;
        public readonly SectorPlane LowerPlane;
        public readonly SectorPlane UpperPlane;
        private readonly GameObject gameObject;

        public Wall(Side side, UpperString textureName, SectorPlane lowerPlane, SectorPlane upperPlane,
            WallSection section, GameObject parentGameObject)
        {
            Side = side;
            TextureName = textureName;
            LowerPlane = lowerPlane;
            UpperPlane = upperPlane;

            gameObject = new GameObject($"Wall_L{side.Line.Index}_S{side.Index}_{section}");
            parentGameObject.SetChild(gameObject);
            CreateWallMesh(textureName, lowerPlane, upperPlane);
        }

        private void CreateWallMesh(UpperString textureName, SectorPlane lowerPlane, SectorPlane upperPlane)
        {
            Sector sector = Side.Sector;
            Line line = Side.Line;
            Material material = GameData.Resources.TextureManager.FindMaterial(textureName);
            Texture texture = material.mainTexture;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            Vector3[] vertices = CreateVertices(line.Segment, lowerPlane, upperPlane);
            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = new[] { 0, 2, 1, 1, 2, 3 },
                normals = CalculateNormals(vertices[0], vertices[1]),
                uv = CreateUVCoordinates(line, line.Segment, texture),
                colors = CreateColors(sector.LightLevelNormalized)
            };
            meshFilter.sharedMesh = mesh;
            mesh.RecalculateBounds();
            // TODO: mesh.RecalculateNormals()?
            // TODO: mesh.RecalculateTangents()?

            // TODO: Should only do this if the line is blocking.
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        private static Vector3[] CreateVertices(Line2 segment, SectorPlane lowerPlane, SectorPlane upperPlane)
        {
            // Vertices are laid out like this in the array:
            //
            // 2---3
            // |   |
            // |   |
            // 0---1
            //
            // This is so we can reference them as clockwise since Unity wants
            // that rotation apparently.
            return new[]
            {
                new Vector3(segment.Start.x, lowerPlane.Height, segment.Start.y) * Constants.MapUnit,
                new Vector3(segment.End.x, lowerPlane.Height, segment.End.y) * Constants.MapUnit,
                new Vector3(segment.Start.x, upperPlane.Height, segment.Start.y) * Constants.MapUnit,
                new Vector3(segment.End.x, upperPlane.Height, segment.End.y) * Constants.MapUnit
            };
        }

        private static Vector2[] CreateUVCoordinates(Line line, Line2 segment, Texture texture)
        {
            // TODO: Need to use the texture.
            return new[]
            {
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 0)
            };
        }

        private static Vector3[] CalculateNormals(Vector3 bottomLeft, Vector3 bottomRight)
        {
            Vector3 edge = (bottomLeft - bottomRight).normalized;
            Vector3 normal = new Vector3(-edge.z, edge.y, edge.x);
            return new[] { normal, normal, normal, normal };
        }

        private static Color[] CreateColors(float lightLevel)
        {
            return new[]
            {
                new Color(lightLevel, lightLevel, lightLevel, 1.0f),
                new Color(lightLevel, lightLevel, lightLevel, 1.0f),
                new Color(lightLevel, lightLevel, lightLevel, 1.0f),
                new Color(lightLevel, lightLevel, lightLevel, 1.0f)
            };
        }
    }
}

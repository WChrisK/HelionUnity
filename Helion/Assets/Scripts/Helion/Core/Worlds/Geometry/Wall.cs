using System;
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
    /// <remarks>
    /// Vertices are laid out like this in the array:
    /// <code>
    ///    2---3
    ///    |   |
    ///    |   |
    ///    0---1
    /// </code>
    /// This is so we can reference them as clockwise since Unity wants
    /// that rotation apparently.
    /// </remarks>
    public class Wall
    {
        public readonly Side Side;
        public readonly UpperString TextureName;
        public readonly SectorPlane LowerPlane;
        public readonly SectorPlane UpperPlane;
        private readonly GameObject gameObject;

        public Wall(Side side, Line line, bool isFront, UpperString textureName, SectorPlane lowerPlane,
            SectorPlane upperPlane, WallSection section, GameObject parentGameObject)
        {
            Side = side;
            TextureName = textureName;
            LowerPlane = lowerPlane;
            UpperPlane = upperPlane;

            gameObject = new GameObject($"Wall_L{line.Index}_S{side.Index}_{section}");
            parentGameObject.SetChild(gameObject);
            CreateWallMesh(line, isFront, textureName, lowerPlane, upperPlane, section);
        }

        private void CreateWallMesh(Line line, bool isFront, UpperString textureName,
            SectorPlane lowerPlane, SectorPlane upperPlane, WallSection section)
        {
            Sector sector = Side.Sector;
            float height = upperPlane.Height - lowerPlane.Height;
            Line2 segment = isFront ? line.Segment : line.Segment.Reverse;

            Material material = Data.Textures.Material(textureName, ResourceNamespace.Textures);
            Texture texture = material.mainTexture;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            Vector3[] vertices = CreateVertices(segment, lowerPlane, upperPlane, texture, section);
            Vector3[] normals = CalculateNormals(vertices[0], vertices[1]);
            Vector2[] uvCoords = CreateUVCoordinates(line, Side, segment, height, texture, section);
            Color[] colors = CreateColors(sector.LightLevelNormalized);
            Mesh mesh = new Mesh
            {
                vertices = vertices,
                triangles = new[] { 0, 2, 1, 1, 2, 3 },
                normals = normals,
                uv = uvCoords,
                colors = colors
            };
            meshFilter.sharedMesh = mesh;
            mesh.RecalculateBounds();

            // TODO: Bake mesh in unity 2019.3 with: Physics.BakeMesh(mesh.instanceID(), ?)
            // TODO: If eligible to move, mesh.MarkDynamic()?

            // TODO: Should only do this if the line is blocking.
            MeshCollider meshCollider = gameObject.AddComponent<MeshCollider>();
            meshCollider.sharedMesh = mesh;
        }

        private static Vector3[] CreateVertices(Line2 segment, SectorPlane lowerPlane, SectorPlane upperPlane,
            Texture texture, WallSection section)
        {
            if (section == WallSection.MiddleTwoSided)
            {
                float height = Math.Min(upperPlane.Height - lowerPlane.Height, texture.height);
                return new[]
                {
                    new Vector3(segment.Start.x, upperPlane.Height - height, segment.Start.y) * Constants.MapUnit,
                    new Vector3(segment.End.x, upperPlane.Height - height, segment.End.y) * Constants.MapUnit,
                    new Vector3(segment.Start.x, upperPlane.Height, segment.Start.y) * Constants.MapUnit,
                    new Vector3(segment.End.x, upperPlane.Height, segment.End.y) * Constants.MapUnit
                };
            }

            return new[]
            {
                new Vector3(segment.Start.x, lowerPlane.Height, segment.Start.y) * Constants.MapUnit,
                new Vector3(segment.End.x, lowerPlane.Height, segment.End.y) * Constants.MapUnit,
                new Vector3(segment.Start.x, upperPlane.Height, segment.Start.y) * Constants.MapUnit,
                new Vector3(segment.End.x, upperPlane.Height, segment.End.y) * Constants.MapUnit
            };
        }

        private static Vector2[] CreateUVCoordinates(Line line, Side side, Line2 segment, float height,
            Texture texture, WallSection section)
        {
            Vector2 span = new Vector2(segment.Length, height);
            Vector2 invDimensions = new Vector2(1.0f / texture.width, 1.0f / texture.height);

            // This coordinate system assumes 0.0 is the bottom/left and 1.0 is
            // the top right, as per OpenGL convention.
            Vector2 origin = side.Offset * invDimensions; //new Vector2(offset.x * invWidth, offset.y * invHeight);
            Vector2 end = (side.Offset + span) * invDimensions; //new Vector2((offset.x + length) * invWidth, (height + offset.y) * invHeight);

            // We end up flipping the Y component because textures are created
            // from the top left and grow downwards. Since this uses OpenGL
            // coordinates, we need to start from the top and grow downward to
            // reconcile the translation in coordinate systems. We also follow
            // the vertices as described in the class documentation remarks
            // section.
            return new[]
            {
                new Vector2(origin.x, end.y),
                new Vector2(end.x, end.y),
                new Vector2(origin.x, origin.y),
                new Vector2(end.x, origin.y),
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

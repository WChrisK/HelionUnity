using System;
using Helion.Core.Resource;
using Helion.Core.Resource.Textures;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Geometry;
using Helion.Core.Util.Unity;
using Helion.Core.Worlds.Geometry.Lines;
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
            Line2F segment = isFront ? line.Segment : line.Segment.Reverse;

            Material material = TextureManager.Material(textureName, ResourceNamespace.Textures);
            Texture texture = material.mainTexture;

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            Vector3[] vertices = CreateVertices(line, segment, lowerPlane, upperPlane, texture, section);
            Vector3[] normals = CalculateNormals(vertices[0], vertices[1]);
            Vector2[] uvCoords = CreateUVCoordinates(line, Side, segment, height, upperPlane, lowerPlane, texture, section);
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

        private static Vector3[] CreateVertices(Line line, Line2F segment, SectorPlane lowerPlane,
            SectorPlane upperPlane, Texture texture, WallSection section)
        {
            // TODO: Need to clip the geometry to the opening.
            if (section == WallSection.MiddleTwoSided)
            {
                float height = Math.Min(upperPlane.Height - lowerPlane.Height, texture.height);

                if (line.Unpeg == Unpeg.Lower || line.Unpeg == Unpeg.LowerAndUpper)
                {
                    return new[]
                    {
                        new Vector3(segment.Start.x, lowerPlane.Height, segment.Start.y).MapUnit(),
                        new Vector3(segment.End.x, lowerPlane.Height, segment.End.y).MapUnit(),
                        new Vector3(segment.Start.x, lowerPlane.Height + height, segment.Start.y).MapUnit(),
                        new Vector3(segment.End.x, lowerPlane.Height + height, segment.End.y).MapUnit()
                    };
                }

                return new[]
                {
                    new Vector3(segment.Start.x, upperPlane.Height - height, segment.Start.y).MapUnit(),
                    new Vector3(segment.End.x, upperPlane.Height - height, segment.End.y).MapUnit(),
                    new Vector3(segment.Start.x, upperPlane.Height, segment.Start.y).MapUnit(),
                    new Vector3(segment.End.x, upperPlane.Height, segment.End.y).MapUnit()
                };
            }

            return new[]
            {
                new Vector3(segment.Start.x, lowerPlane.Height, segment.Start.y).MapUnit(),
                new Vector3(segment.End.x, lowerPlane.Height, segment.End.y).MapUnit(),
                new Vector3(segment.Start.x, upperPlane.Height, segment.Start.y).MapUnit(),
                new Vector3(segment.End.x, upperPlane.Height, segment.End.y).MapUnit()
            };
        }

        private static Vector2[] CreateUVCoordinates(Line line, Side side, Line2F segment, float height,
            SectorPlane upperPlane, SectorPlane lowerPlane, Texture texture, WallSection section)
        {
            // An important note for all of the following functions:
            // Remember that the texture was uploaded such that the
            // 0.0 -> 1.0 coordinates look like:
            //
            // (0.0, 0.0)      (1.0, 0.0)
            //         o--------o
            //         |  Top   |
            //         |        |
            //         |        |
            //         |        |
            //         | Bottom |
            //         o--------o
            // (0.0, 1.0)      (1.0, 1.0)
            //
            // This means when we are drawing from the bottom up, we want to
            // start out at the bottom UV and subtract the X/Y span and offsets
            // to go upwards. Likewise if we are drawing from the top down, we
            // want to start at the top two coordinates and add the span and
            // offset to go down.
            switch (section)
            {
            case WallSection.Lower:
                return CreateUVCoordinatesTwoSidedLower(line, side, segment, height, lowerPlane, upperPlane, texture);
            case WallSection.MiddleOneSided:
                return CreateUVCoordinatesOneSidedMiddle(line, side, segment, height, texture);
            case WallSection.MiddleTwoSided:
                return CreateUVCoordinatesTwoSidedMiddle(line, side, segment, height, lowerPlane, upperPlane, texture);
            case WallSection.Upper:
                return CreateUVCoordinatesTwoSidedUpper(line, side, segment, height, lowerPlane, upperPlane, texture);
            default:
                throw new Exception($"Unexpected wall section type for UV coordinates: {section}");
            }
        }

        private static Vector2[] CreateUVCoordinatesTwoSidedLower(Line line, Side side, Line2F segment,
            float height, SectorPlane lowerPlane, SectorPlane upperPlane, Texture texture)
        {
            Vector2 invDimensions = new Vector2(1.0f / texture.width, 1.0f / texture.height);
            Vector2 spanUV = new Vector2(segment.Length, height) * invDimensions;
            Vector2 offsetUV = side.Offset * invDimensions;

            // If it's upper, it draws from the top of the ceiling sector down.
            // TODO: This is NOT how it's done but I'll fix it later...
            if (line.Unpeg == Unpeg.Lower || line.Unpeg == Unpeg.LowerAndUpper)
            {
                Vector2 bottomLeftUV = new Vector2(0.0f, 1.0f) + offsetUV;
                Vector2 topRightUV = new Vector2(spanUV.x, 1.0f - spanUV.y) + offsetUV;

                return new[]
                {
                    new Vector2(bottomLeftUV.x, bottomLeftUV.y),
                    new Vector2(topRightUV.x, bottomLeftUV.y),
                    new Vector2(bottomLeftUV.x, topRightUV.y),
                    new Vector2(topRightUV.x, topRightUV.y),
                };
            }

            // Otherwise, we draw from the bottom up.
            Vector2 bottomLeft = offsetUV;
            Vector2 topRight = offsetUV + spanUV;

            return new[]
            {
                new Vector2(bottomLeft.x, topRight.y),
                new Vector2(topRight.x, topRight.y),
                new Vector2(bottomLeft.x, bottomLeft.y),
                new Vector2(topRight.x, bottomLeft.y),
            };
        }

        private static Vector2[] CreateUVCoordinatesOneSidedMiddle(Line line, Side side, Line2F segment,
            float height, Texture texture)
        {
            Vector2 invDimensions = new Vector2(1.0f / texture.width, 1.0f / texture.height);
            Vector2 spanUV = new Vector2(segment.Length, height) * invDimensions;
            Vector2 offsetUV = side.Offset * invDimensions;

            // If it's lower, it draws from the floor up.
            if (line.Unpeg == Unpeg.Lower || line.Unpeg == Unpeg.LowerAndUpper)
            {
                Vector2 bottomLeftUV = new Vector2(0.0f, 1.0f) + offsetUV;
                Vector2 topRightUV = new Vector2(spanUV.x, 1.0f - spanUV.y) + offsetUV;

                return new[]
                {
                    new Vector2(bottomLeftUV.x, bottomLeftUV.y),
                    new Vector2(topRightUV.x, bottomLeftUV.y),
                    new Vector2(bottomLeftUV.x, topRightUV.y),
                    new Vector2(topRightUV.x, topRightUV.y),
                };
            }

            // Otherwise, we draw from the top down.
            Vector2 bottomLeft = offsetUV;
            Vector2 topRight = offsetUV + spanUV;

            return new[]
            {
                new Vector2(bottomLeft.x, topRight.y),
                new Vector2(topRight.x, topRight.y),
                new Vector2(bottomLeft.x, bottomLeft.y),
                new Vector2(topRight.x, bottomLeft.y),
            };
        }

        private static Vector2[] CreateUVCoordinatesTwoSidedMiddle(Line line, Side side, Line2F segment,
            float height, SectorPlane lowerPlane, SectorPlane upperPlane, Texture texture)
        {
            Vector2 invDimensions = new Vector2(1.0f / texture.width, 1.0f / texture.height);
            Vector2 spanUV = new Vector2(segment.Length, height) * invDimensions;
            Vector2 offsetUV = side.Offset * invDimensions;

            // TODO: Handle clipping and all that fun stuff later...
            return new[]
            {
                new Vector2(offsetUV.x, 1),
                new Vector2(spanUV.x + offsetUV.x, 1),
                new Vector2(offsetUV.x, 0),
                new Vector2(spanUV.x + offsetUV.x, 0)
            };
        }

        private static Vector2[] CreateUVCoordinatesTwoSidedUpper(Line line, Side side, Line2F segment,
            float height, SectorPlane lowerPlane, SectorPlane upperPlane, Texture texture)
        {
            Vector2 invDimensions = new Vector2(1.0f / texture.width, 1.0f / texture.height);
            Vector2 spanUV = new Vector2(segment.Length, height) * invDimensions;
            Vector2 offsetUV = side.Offset * invDimensions;

            // If it's upper, it draws from the top down.
            if (line.Unpeg == Unpeg.Upper || line.Unpeg == Unpeg.LowerAndUpper)
            {
                Vector2 bottomLeft = offsetUV;
                Vector2 topRight = offsetUV + spanUV;

                return new[]
                {
                    new Vector2(bottomLeft.x, topRight.y),
                    new Vector2(topRight.x, topRight.y),
                    new Vector2(bottomLeft.x, bottomLeft.y),
                    new Vector2(topRight.x, bottomLeft.y),
                };
            }

            // Otherwise, we draw from the top down.
            Vector2 bottomLeftUV = new Vector2(0.0f, 1.0f) + offsetUV;
            Vector2 topRightUV = new Vector2(spanUV.x, 1.0f - spanUV.y) + offsetUV;

            return new[]
            {
                new Vector2(bottomLeftUV.x, bottomLeftUV.y),
                new Vector2(topRightUV.x, bottomLeftUV.y),
                new Vector2(bottomLeftUV.x, topRightUV.y),
                new Vector2(topRightUV.x, topRightUV.y),
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

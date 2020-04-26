using System;
using Helion.Core.Util;
using Helion.Core.Util.Unity;
using Helion.Core.Worlds.Geometry.Enums;
using UnityEngine;
using Texture = Helion.Core.Resource.Textures.Texture;

namespace Helion.Core.Worlds.Geometry.Walls
{
    /// <summary>
    /// The rendering components for the wall.
    /// </summary>
    /// <remarks>
    /// Vertices are laid out like this in the array:
    /// <code>
    ///    2---3
    ///    |   |
    ///    |   |
    ///    0---1
    /// </code>
    /// Then we reference them clockwise since apparently Unity has CW as the
    /// default instead of CCW.
    /// </remarks>
    public class WallMeshComponents : IDisposable
    {
        public readonly Mesh Mesh;
        public readonly MeshFilter Filter;
        public readonly MeshRenderer Renderer;
        private readonly Wall wall;
        private readonly float lineLength;
        private Texture texture;
        private bool isDisabled;

        public WallMeshComponents(Wall wall, GameObject gameObject, Texture texture)
        {
            this.wall = wall;
            this.texture = texture;
            this.lineLength = wall.Line.Segment.Length();
            this.Renderer = gameObject.AddComponent<MeshRenderer>();
            this.Filter = gameObject.AddComponent<MeshFilter>();
            this.Mesh = CreateMesh();

            Renderer.sharedMaterial = texture.Material;
            Filter.sharedMesh = Mesh;
        }

        public void Update()
        {
            (SectorPlane floor, SectorPlane ceiling) = FindBoundingPlane();

            UpdateEnabledStatus(floor, ceiling);

            // If we're disabled, that means it's a zero-height mesh and it is
            // pointless to do any updates on it. It'll be disabled too so even
            // if it is left in some weird state, it won't render anyways. Plus
            // when we enable it again, we'll set it to a proper state.
            if (isDisabled)
                return;

            // TODO: Adjust mesh Y / UV / color
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
        }

        private void UpdateEnabledStatus(SectorPlane floor, SectorPlane ceiling)
        {
            if (wall.Section == WallSection.Middle &&
                wall.Line.TwoSided &&
                wall.TextureName == Constants.NoTexture)
            {
                Disable();
                isDisabled = true;
                return;
            }

            // We do it this way just in case enabling/disabling ends up being
            // a non-cheap operation.
            if (floor.Height < ceiling.Height)
            {
                if (isDisabled)
                {
                    Enable();
                    isDisabled = false;
                }
            }
            else
            {
                if (!isDisabled)
                {
                    Disable();
                    isDisabled = true;
                }
            }
        }

        private void Enable()
        {
            // TODO: `Collider.enabled = true;` if it has one
            Renderer.enabled = true;
        }

        private void Disable()
        {
            // TODO: `Collider.enabled = false;` if it has one
            Renderer.enabled = false;
        }

        private (SectorPlane floor, SectorPlane ceiling) FindBoundingPlane()
        {
            Line line = wall.Line;
            Sector facingSector = wall.Side.Sector;
            if (line.OneSided)
                return (facingSector.Floor, facingSector.Ceiling);

            Sector partnerSector = wall.Side.PartnerSide.Value.Sector;
            if (wall.OnBackSide)
                (facingSector, partnerSector) = (partnerSector, facingSector);

            SectorPlane facingFloor = facingSector.Floor;
            SectorPlane facingCeiling = facingSector.Ceiling;
            SectorPlane partnerFloor = partnerSector.Floor;
            SectorPlane partnerCeiling = partnerSector.Ceiling;

            switch (wall.Section)
            {
            case WallSection.Lower:
                return (facingFloor, partnerFloor);
            case WallSection.Middle:
                return (facingFloor.Height >= partnerFloor.Height ? facingFloor : partnerFloor,
                        facingCeiling.Height <= partnerCeiling.Height ? facingCeiling : partnerCeiling);
            case WallSection.Upper:
                return (partnerCeiling, facingCeiling);
            default:
                throw new Exception($"Unexpected section type for wall attachment: {wall.Section}");
            }
        }

        private Mesh CreateMesh()
        {
            (SectorPlane floor, SectorPlane ceiling) = FindBoundingPlane();

            Vector3[] vertices = CalculateVertices(floor, ceiling);
            Vector3[] normals = CalculateNormals(vertices[0], vertices[1]);
            Vector2[] uvCoords = CalculateUVCoordinates(floor, ceiling, vertices);
            Color[] colors = CalculateColors();

            return new Mesh
            {
                vertices = vertices,
                triangles = new[] { 0, 2, 1, 1, 2, 3 },
                normals = normals,
                uv = uvCoords,
                colors = colors
            };
        }

        private Vector3[] CalculateVertices(SectorPlane floor, SectorPlane ceiling)
        {
            float top = ceiling.Height;
            float bottom = floor.Height;
            Vector2 start = wall.Line.Segment.Start;
            Vector2 end = wall.Line.Segment.End;

            if (wall.OnBackSide)
                (start, end) = (end, start);

            if (wall.Section == WallSection.Middle && wall.Line.TwoSided)
            {
                float imageHeight = texture.Height;
                float texTop = top;
                float texBottom = bottom;

                // Draws from the top down, unless it's lower unpegged.
                if (wall.Line.Unpegged.HasLower())
                    texTop = texBottom + imageHeight;
                else
                    texBottom = texTop - imageHeight;

                texTop += wall.Side.Offset.Y;
                texBottom += wall.Side.Offset.Y;

                // Now we clip it to the gap that exists.
                top = Math.Min(top, texTop);
                bottom = Math.Max(bottom, texBottom);
            }

            return new[]
            {
                new Vector3(start.x, bottom, start.y).MapUnit(),
                new Vector3(end.x, bottom, end.y).MapUnit(),
                new Vector3(start.x, top, start.y).MapUnit(),
                new Vector3(end.x, top, end.y).MapUnit(),
            };
        }

        private Vector2[] CalculateUVCoordinates(SectorPlane floor, SectorPlane ceiling,
            Vector3[] vertices)
        {
            Vector2 start = new Vector2(0, 0);
            Vector2 end = new Vector2(1, 1);

            // An important note for all of the following functions:
            // Remember that the texture was uploaded such that the
            // 0.0 -> 1.0 coordinates look like:
            //
            // (0.0, 0.0)      (1.0, 0.0)
            //         S--------o
            //         |  Top   |      S = start
            //         |        |      E = end
            //         |        |
            //         |        |
            //         | Bottom |
            //         o--------E
            // (0.0, 1.0)      (1.0, 1.0)
            //
            // This means when we are drawing from the bottom up, we want to
            // start out at the bottom UV and subtract the X/Y span and offsets
            // to go upwards. Likewise if we are drawing from the top down, we
            // want to start at the top two coordinates and add the span and
            // offset to go down.
            // In short, the coordinate system looks exactly like the image
            // origin system.
            switch (wall.Section)
            {
            case WallSection.Lower:
                CalculateTwoSidedLowerUV(floor, ceiling, ref start, ref end);
                break;
            case WallSection.Middle:
                if (wall.Line.OneSided)
                    CalculateOneSidedMiddleUV(floor, ceiling, ref start, ref end);
                else
                    CalculateTwoSidedMiddleUV(floor, ceiling, vertices, ref start, ref end);
                break;
            case WallSection.Upper:
                CalculateTwoSidedUpperUV(floor, ceiling, ref start, ref end);
                break;
            default:
                throw new Exception($"Unexpected wall section for UV calculations: {wall.Section}");
            }

            // This follows easily from the comment/ASCII-art above, along with
            // the vertex locations from the class documentation.
            return new[]
            {
                new Vector2(start.x, end.y),
                new Vector2(end.x, end.y),
                new Vector2(start.x, start.y),
                new Vector2(end.x, start.y)
            };
        }

        private void CalculateTwoSidedLowerUV(SectorPlane floor, SectorPlane ceiling,
            ref Vector2 start, ref Vector2 end)
        {
            int height = ceiling.Height - floor.Height;
            Vector2 invDimension = texture.InverseDimension;
            Vector2 spanUV = new Vector2(lineLength, height) * invDimension;
            Vector2 offsetUV = wall.Side.Offset.Float() * invDimension;

            // If it's upper, draw from the top of the ceiling down.
            if (wall.Line.Unpegged.HasUpper())
            {
                // Remember that this draws from the very top, so we need to
                // know the sector height that will act as the top. Drawing the
                // other pieces
                float maxHeight = wall.Side.Sector.Ceiling.Height;

                // We need to find the V coordinates from the top to do these
                // correctly.
                float topV = (maxHeight - ceiling.Height) * invDimension.y;
                float bottomV = (maxHeight - floor.Height) * invDimension.y;

                start = new Vector2(offsetUV.x, topV + offsetUV.y);
                end = new Vector2(offsetUV.x + spanUV.x, bottomV + offsetUV.y);
            }
            else
            {
                start = offsetUV;
                end = offsetUV + spanUV;
            }
        }

        private void CalculateOneSidedMiddleUV(SectorPlane floor, SectorPlane ceiling,
            ref Vector2 start, ref Vector2 end)
        {
            int height = ceiling.Height - floor.Height;
            Vector2 invDimension = texture.InverseDimension;
            Vector2 spanUV = new Vector2(lineLength, height) * invDimension;
            Vector2 offsetUV = wall.Side.Offset.Float() * invDimension;

            // If it's lower, draw from the floor up.
            if (wall.Line.Unpegged.HasLower())
            {
                end = new Vector2(spanUV.x + offsetUV.x, 1.0f - offsetUV.y);
                start = new Vector2(offsetUV.x, end.y - spanUV.y);
            }
            else
            {
                start = offsetUV;
                end = offsetUV + spanUV;
            }
        }

        private void CalculateTwoSidedMiddleUV(SectorPlane floor, SectorPlane ceiling,
            Vector3[] vertices, ref Vector2 start, ref Vector2 end)
        {
            // TODO: Should probably turn this into a data structure to avoid these cryptic references.
            float height = vertices[2].y - vertices[0].y;

            Vector2 invDimension = texture.InverseDimension;
            Vector2 spanUV = new Vector2(lineLength, height) * invDimension;
            Vector2 offsetUV = wall.Side.Offset.Float() * invDimension;

            // TODO: Draw it to the proper clipped spot.
            start = new Vector2(offsetUV.x, 0);
            end = new Vector2(offsetUV.x + spanUV.x, 1);
        }

        private void CalculateTwoSidedUpperUV(SectorPlane floor, SectorPlane ceiling,
            ref Vector2 start, ref Vector2 end)
        {
            int height = ceiling.Height - floor.Height;
            Vector2 invDimension = texture.InverseDimension;
            Vector2 spanUV = new Vector2(lineLength, height) * invDimension;
            Vector2 offsetUV = wall.Side.Offset.Float() * invDimension;

            // If it's upper, draw it from the top down.
            if (wall.Line.Unpegged.HasUpper())
            {
                start = offsetUV;
                end = offsetUV + spanUV;
            }
            else
            {
                end = new Vector2(spanUV.x + offsetUV.x, 1.0f - offsetUV.y);
                start = new Vector2(offsetUV.x, end.y - spanUV.y);
            }
        }

        private static Vector3[] CalculateNormals(Vector3 bottomLeft, Vector3 bottomRight)
        {
            Vector3 edge = (bottomLeft - bottomRight).normalized;
            Vector3 normal = new Vector3(-edge.z, edge.y, edge.x);
            return new[] { normal, normal, normal, normal };
        }

        private Color[] CalculateColors()
        {
            float lightLevel = wall.Side.Sector.LightLevelNormalized;
            Color color = new Color(lightLevel, lightLevel, lightLevel, 1.0f);
            return new[] { color, color, color, color };
        }
    }
}

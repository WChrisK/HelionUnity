using System;
using Helion.Core.Util;
using Helion.Core.Util.Unity;
using Helion.Core.Worlds.Geometry.Enums;
using UnityEngine;

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
        private Material material;
        private bool isDisabled;

        public WallMeshComponents(Wall wall, GameObject gameObject, Material material)
        {
            this.wall = wall;
            this.material = material;
            this.lineLength = wall.Line.Segment.Length;
            this.Renderer = gameObject.AddComponent<MeshRenderer>();
            this.Filter = gameObject.AddComponent<MeshFilter>();
            this.Mesh = CreateMesh();

            Renderer.sharedMaterial = material;
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

        internal void SetMaterial(Material newMaterial)
        {
            material = newMaterial;
        }

        private void UpdateEnabledStatus(SectorPlane floor, SectorPlane ceiling)
        {
            Debug.Log($"Wall {wall.Index}");

            if (wall.Section == WallSection.Middle &&
                wall.Line.TwoSided &&
                wall.TextureName == Constants.NoTexture)
            {
                Debug.Log($"    No middle");
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
            Sector facingSector = line.Front.Sector;
            if (line.OneSided)
                return (facingSector.Floor, facingSector.Ceiling);

            Sector partnerSector = line.Back.Value.Sector;
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
                return (facingFloor.Height <= partnerFloor.Height ? facingFloor : partnerFloor,
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
            Vector2[] uvCoords = CalculateUVCoordinates(floor, ceiling);
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
                float imageHeight = material.mainTexture.height;
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

        private Vector2[] CalculateUVCoordinates(SectorPlane floor, SectorPlane ceiling)
        {
            Vector2 start = new Vector2(0, 0);
            Vector2 end = new Vector2(1, 1);

            switch (wall.Section)
            {
            case WallSection.Lower:
                break;
            case WallSection.Middle:
                if (wall.Line.OneSided)
                    CalculateOneSidedMiddleUV(floor, ceiling, ref start, ref end);
                break;
            case WallSection.Upper:
                break;
            default:
                throw new Exception($"Unexpected wall section for UV calculations: {wall.Section}");
            }

            // We need to flip the V coordinates because of how textures are
            // loaded in from the top rather than the bottom.
            return new[]
            {
                new Vector2(start.x, end.y),
                new Vector2(end.x, end.y),
                new Vector2(start.x, start.y),
                new Vector2(end.x, start.y)
            };
        }

        private void CalculateOneSidedMiddleUV(SectorPlane floor, SectorPlane ceiling,
            ref Vector2 start, ref Vector2 end)
        {
            int height = ceiling.Height - floor.Height;
            Vector2 invDimension = new Vector2(1.0f / material.mainTexture.width, 1.0f / material.mainTexture.height);
            Vector2 spanUV = new Vector2(lineLength, height) * invDimension;
            Vector2 offsetUV = wall.Side.Offset.Float() * invDimension;

            start.x = offsetUV.x;
            end.x = (lineLength * invDimension.x) + offsetUV.x;

            // If it's lower, draw from the floor up. Otherwise, top down.
            if (wall.Line.Unpegged.HasLower())
            {
                start = new Vector2(0.0f, 1.0f) + offsetUV;
                end = new Vector2(spanUV.x, 1.0f - spanUV.y) + offsetUV;
            }
            else
            {
                start = offsetUV;
                end = offsetUV + spanUV;
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

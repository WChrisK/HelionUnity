using System;
using Helion.Unity;
using Helion.Util.Geometry.Vectors;
using Helion.Util.Unity;
using Helion.Worlds.Entities.Movement;
using Helion.Worlds.Geometry.Enums;
using UnityEngine;
using Texture = Helion.Resource.Textures.Texture;

namespace Helion.Worlds.Geometry.Walls
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
    /// Finally, the vertices are placed at +/-0.5 so that the center of the
    /// wall is at the origin and follows along the X/Y plane (Z depth = 0).
    /// This way when we apply scaling and rotations, the box collider will
    /// go along with it and rotate appropriately. This is to get around the
    /// mesh collider which does not work for planes.
    /// </remarks>
    public class WallMeshComponents : IRenderable, IDisposable
    {
        public readonly Mesh Mesh;
        public readonly MeshFilter Filter;
        public readonly MeshRenderer Renderer;
        public readonly BoxCollider Collider;
        private readonly GameObject gameObject;
        private readonly Wall wall;
        private readonly CollisionInfo collisionInfo;
        private Texture texture;
        private bool isDisabled;

        public WallMeshComponents(Wall wall, Texture texture)
        {
            this.wall = wall;
            this.texture = texture;
            this.gameObject = new GameObject($"Wall {wall.Index} ({wall.Section}: Line {wall.Side.Line.Index}, Side {wall.Side.Index})");
            this.Mesh = CreateMesh();
            this.Filter = CreateFilter();
            this.Renderer = CreateRenderer();
            this.Collider = CreateBoxCollider();
            this.collisionInfo = CollisionInfo.CreateOn(gameObject, wall);

            Update(0);
        }

        public void Update(float tickFraction)
        {
            // OPTIMIZE: We should cache this.
            (SectorPlane floor, SectorPlane ceiling) = wall.FindBoundingPlane();

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
            GameObjectHelper.Destroy(Collider);
            GameObjectHelper.Destroy(collisionInfo);
            GameObjectHelper.Destroy(gameObject);
        }

        private Mesh CreateMesh()
        {
            (SectorPlane floor, SectorPlane ceiling) = wall.FindBoundingPlane();

            Vector3[] vertices = CalculateVertices(floor, ceiling);
            Vector3[] normals = CalculateNormals(vertices[0], vertices[1]);
            Vector2[] uvCoords = CalculateUVCoordinates(floor, ceiling, vertices);
            Color[] colors = CalculateColors();
            SetTranslationScaleRotation(vertices);

            return new Mesh
            {
                vertices = new[]
                {
                    new Vector3(-0.5f, -0.5f, 0),
                    new Vector3(0.5f, -0.5f, 0),
                    new Vector3(-0.5f, 0.5f, 0),
                    new Vector3(0.5f, 0.5f, 0)
                },
                triangles = new[] { 0, 2, 1, 1, 2, 3 },
                normals = normals,
                uv = uvCoords,
                colors = colors
            };
        }

        private MeshFilter CreateFilter()
        {
            MeshFilter filter = gameObject.AddComponent<MeshFilter>();
            filter.sharedMesh = Mesh;

            return filter;
        }

        private MeshRenderer CreateRenderer()
        {
            MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
            renderer.sharedMaterial = texture.Material;

            return renderer;
        }

        private BoxCollider CreateBoxCollider()
        {
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.center = Vector3.zero;
            collider.size = new Vector3(1, 1, PhysicsSystem.ColliderThickness);

            return collider;
        }

        private void UpdateEnabledStatus(SectorPlane floor, SectorPlane ceiling)
        {
            if (wall.IsTwoSidedNoMiddle)
            {
                Disable();
                isDisabled = true;
                return;
            }

            // We do it this way just in case enabling/disabling ends up being
            // a non-cheap operation.
            if (floor.Height <= ceiling.Height)
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
            Collider.enabled = true;
            Renderer.enabled = true;
            isDisabled = false;
        }

        private void Disable()
        {
            Collider.enabled = false;
            Renderer.enabled = false;
            isDisabled = true;
        }

        private Vector3[] CalculateVertices(SectorPlane floor, SectorPlane ceiling)
        {
            float top = ceiling.Height;
            float bottom = floor.Height;
            Vec2F start = wall.Line.Segment.Start;
            Vec2F end = wall.Line.Segment.End;

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
                new Vector3(start.X, bottom, start.Y).MapUnit(),
                new Vector3(end.X, bottom, end.Y).MapUnit(),
                new Vector3(start.X, top, start.Y).MapUnit(),
                new Vector3(end.X, top, end.Y).MapUnit(),
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
            float lineLength = wall.Line.Segment.Length();
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
            float lineLength = wall.Line.Segment.Length();
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
            float lineLength = wall.Line.Segment.Length();

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
            float lineLength = wall.Line.Segment.Length();
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

        private void SetTranslationScaleRotation(Vector3[] vertices)
        {
            SetScale(vertices);
            SetTranslation(vertices);
            SetRotation(vertices);
        }

        private void SetScale(Vector3[] vertices)
        {
            // TODO: This will not play nicely with slopes...
            float y = (vertices[2] - vertices[0]).magnitude;
            float x = (vertices[1] - vertices[0]).magnitude;
            gameObject.transform.localScale = new Vector3(x, y, 1);
        }

        private void SetTranslation(Vector3[] vertices)
        {
            // The center of the wall is the center of the quad.
            Vector3 average = Vector3.zero;
            foreach (Vector3 vertex in vertices)
                average += vertex;
            average /= vertices.Length;

            gameObject.transform.Translate(average);
        }

        private void SetRotation(Vector3[] vertices)
        {
            Vector3 direction = vertices[1] - vertices[0];
            Vector3 angle = Quaternion.LookRotation(direction).eulerAngles;
            angle.y -= 90;
            gameObject.transform.localEulerAngles = angle;
        }
    }
}

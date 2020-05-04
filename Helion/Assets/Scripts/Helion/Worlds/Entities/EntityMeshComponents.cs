using System;
using Helion.Resource.Textures.Sprites;
using Helion.Unity;
using Helion.Util;
using Helion.Util.Extensions;
using Helion.Util.Unity;
using UnityEngine;
using Texture = Helion.Resource.Textures.Texture;

namespace Helion.Worlds.Entities
{
    public class EntityMeshComponents : IDisposable
    {
        // TODO: Should we make this non static? If someone edits these accidentlaly, we're screwed.
        private static readonly Vector2[] nonFlippedUV =
        {
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(0, 0),
            new Vector2(1, 0)
        };
        private static readonly Vector2[] flippedUV =
        {
            new Vector2(1, 1),
            new Vector2(0, 1),
            new Vector2(1, 0),
            new Vector2(0, 0)
        };

        public readonly Mesh Mesh;
        public readonly MeshFilter Filter;
        public readonly MeshRenderer Renderer;
        private readonly Entity entity;
        private readonly GameObject gameObject;
        private bool isDisabled;

        public EntityMeshComponents(Entity entity, GameObject entityGameObject)
        {
            this.entity = entity;

            gameObject = new GameObject("Sprite mesh");
            entityGameObject.SetChild(gameObject, false);

            Renderer = gameObject.AddComponent<MeshRenderer>();
            Filter = gameObject.AddComponent<MeshFilter>();
            Mesh = CreateMesh();

            Filter.sharedMesh = Mesh;
        }

        public void Update(float tickFraction)
        {
            SpriteRotations rotations = entity.Frame.SpriteRotations;

            if (rotations.DoNotRender)
            {
                EnsureDisabled();
                return;
            }

            EnsureEnabled();

            // TODO: If the index or the texture does not change, exit early.

            int index = CameraManager.CalculateRotationIndex(entity, tickFraction);
            Texture texture = entity.Frame.SpriteRotations[index];
            Renderer.sharedMaterial = texture.Material;

            // Since the 0-7 range has us looking for 5, 6, or 7, we can check
            // anything >= 5 to see if we should be mirroring.
            bool shouldFlip = rotations.Mirrored && index >= 5;
            Mesh.uv = shouldFlip ? flippedUV : nonFlippedUV;

            float y = texture.Height.MapUnit() / 2;
            gameObject.transform.localPosition = new Vector3(0, y, 0);
            gameObject.transform.localScale = new Vector3(texture.Width, texture.Height, 1);
        }

        public void Dispose()
        {
            GameObjectHelper.Destroy(Renderer);
            GameObjectHelper.Destroy(Filter);
            GameObjectHelper.Destroy(Mesh);
            GameObjectHelper.Destroy(gameObject);
        }

        private Mesh CreateMesh()
        {
            float lightLevel = entity.Sector.LightLevelNormalized;
            Color color = new Color(lightLevel, lightLevel, lightLevel, 1.0f);

            // We make this so all we have to do is adjust the transform scale
            // by the texture size and it will adjust itself accordingly. Same
            // for the translation and rotation. As such to assign the texture
            // coordinates to the scale, the width has to be equal to one pixel
            // in the world size, so our radius becomes the map unit.
            //
            // This also uses the vertex layout as seen in `WallMeshComponent`.
            // See that class for documentation on vertex position choice.
            const float radius = Constants.MapUnit / 2;
            Vector3[] vertices =
            {
                new Vector3(-radius, -radius, 0),
                new Vector3(radius, -radius, 0),
                new Vector3(-radius, radius, 0),
                new Vector3(radius, radius, 0)
            };
            Vector2[] uvCoords = nonFlippedUV;
            Vector3[] normals = { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
            Color[] colors = { color, color, color, color };

            return new Mesh
            {
                vertices = vertices,
                triangles = new[] { 0, 2, 1, 1, 2, 3 },
                normals = normals,
                uv = uvCoords,
                colors = colors
            };
        }

        private void EnsureEnabled()
        {
            if (!isDisabled)
                return;

            Renderer.enabled = true;
            isDisabled = false;
        }

        private void EnsureDisabled()
        {
            if (isDisabled)
                return;

            Renderer.enabled = false;
            isDisabled = true;
        }
    }
}

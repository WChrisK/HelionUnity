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

            // TODO: Need to flip UV coordinates on the mesh or something if mirrored.
            int index = CameraManager.CalculateRotationIndex(entity, tickFraction);
            Texture texture = entity.Frame.SpriteRotations[index];
            Renderer.sharedMaterial = texture.Material;

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
            Vector2[] uvCoords = { new Vector2(0, 1), Vector2.one, Vector2.zero, new Vector2(1, 0) };
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

using System;
using Helion.Core.Resource.Textures;
using Helion.Core.Util;
using Helion.Core.Util.Unity;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry
{
    public class SectorPlane : IDisposable
    {
        public readonly int Index;
        public readonly bool IsCeiling;
        public int Height;
        public int? LightLevel;
        public Material Material { get; private set; }
        private readonly GameObject gameObject;
        private int sectorLightLevel;
        private UpperString textureName;

        public bool IsFloor => !IsCeiling;
        public UpperString TextureName
        {
            get => textureName;
            set
            {
                if (value != null)
                {
                    textureName = value;
                    Material = TextureManager.Material(value);
                }
            }
        }

        public SectorPlane(int index, bool isCeiling, int height, UpperString texture, int startingLightLevel)
        {
            Index = index;
            IsCeiling = isCeiling;
            Height = height;
            TextureName = texture;
            Material = TextureManager.Material(texture);
            sectorLightLevel = startingLightLevel;
            gameObject = CreateGameObject(index, isCeiling, height);
        }

        public void Dispose()
        {
            GameObjectHelper.Destroy(gameObject);
        }

        private static GameObject CreateGameObject(int index, bool isCeiling, int height)
        {
            string facingText = isCeiling ? "Ceiling" : "Floor";
            GameObject gameObj = new GameObject($"Sector plane {index} ({facingText})");
            gameObj.transform.position = new Vector3(0, height, 0).MapUnit();

            return gameObj;
        }
    }
}

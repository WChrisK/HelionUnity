using System;
using System.Collections.Generic;
using Helion.Core.Archives;
using Helion.Core.Resource.Colors.Palettes;
using Helion.Core.Resource.Textures;
using Helion.Core.Resource.Textures.Definitions.Vanilla;
using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Resource
{
    /// <summary>
    /// Manages all of the resources from a series of archives.
    /// </summary>
    public class ResourceManager
    {
        public Palette Palette { get; private set; } // TODO: Default palette goes here!
        public readonly List<PNames> PNamesDefinitions = new List<PNames>();
        public readonly List<TextureX> TextureXDefinitions = new List<TextureX>();
        public readonly TextureManager TextureManager = new TextureManager();
        private bool alreadyLoaded;

        /// <summary>
        /// Loads the data in. Should only be called once. Using this object
        /// after this function returning false is undefined behavior. This
        /// will be fixed to be more sane when we modularize.
        /// </summary>
        /// <param name="archives">A collection of archives to load.</param>
        /// <returns>True on success, false if any failed.</returns>
        public bool Load(IEnumerable<IArchive> archives)
        {
            // This will go away when we modularize!
            if (alreadyLoaded)
                throw new Exception("Trying to load archives multiple times");

            foreach (IArchive archive in archives)
            {
                foreach (IEntry entry in archive)
                {
                    switch (entry.Name.ToString())
                    {
                    case "PLAYPAL":
                        HandlePalette(entry);
                        break;
                    case "PNAMES":
                        HandlePNames(entry);
                        break;
                    case "TEXTURE1":
                    case "TEXTURE2":
                        HandleTextureX(entry);
                        break;
                    default:
                        break;
                    }
                }
            }

            TextureManager.Update(this);
            alreadyLoaded = true;

            return true;
        }

        private void HandlePalette(IEntry entry)
        {
            Optional<Palette> paletteOptional = Palette.From(entry.Data);
            if (!paletteOptional)
            {
                Debug.Log("Cannot load corrupt PLAYPAL");
                return;
            }

            Palette = paletteOptional.Value;
        }

        private void HandlePNames(IEntry entry)
        {
            Optional<PNames> pnamesOptional = PNames.From(entry.Data);
            if (!pnamesOptional)
            {
                Debug.Log("Cannot load corrupt PNAMES");
                return;
            }

            PNamesDefinitions.Add(pnamesOptional.Value);
        }

        private void HandleTextureX(IEntry entry)
        {
            Optional<TextureX> textureXOptional = TextureX.From(entry.Data);
            if (!textureXOptional)
            {
                Debug.Log("Cannot load corrupt TEXTUREx");
                return;
            }

            TextureXDefinitions.Add(textureXOptional.Value);
        }
    }
}

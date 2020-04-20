using System.Collections.Generic;
using System.Text;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using UnityEngine;

namespace Helion.Core.Resource.Textures.Sprites
{
    /// <summary>
    /// Manages graphical information for existing sprites.
    /// </summary>
    public static class SpriteManager
    {
        /// <summary>
        /// Get a rotational sprite with null textures. Always will exist and
        /// can be supplied to any actor frame safely.
        /// </summary>
        public static readonly SpriteRotations NullRotations = new SpriteRotations("NULL", TextureManager.NullMaterial);
        private static readonly Dictionary<UpperString, SpriteRotations> spriteRotations = new Dictionary<UpperString, SpriteRotations>();

        public static void Clear()
        {
            spriteRotations.Clear();
        }

        /// <summary>
        /// Gets the sprite for the provided sprite/frame, or creates one if it
        /// does not exist.
        /// </summary>
        /// <param name="spriteAndFrame">The name of the sprite to look up (ex:
        /// "PLAYD"). This must contain the frame (or 5th letter).</param>
        /// <returns>The sprite rotations, or a default value of missing images
        /// if the sprite/frame is an empty string.</returns>
        public static SpriteRotations Rotations(UpperString spriteAndFrame)
        {
            if (spriteAndFrame.Empty())
                return NullRotations;

            if (spriteRotations.TryGetValue(spriteAndFrame, out SpriteRotations rotations))
                return rotations;

            SpriteRotations newRotations = CreateSpriteFrom(spriteAndFrame);
            spriteRotations[spriteAndFrame] = newRotations;
            return newRotations;
        }

        private static UpperString MakeRotation(UpperString name, char first, char second)
        {
            StringBuilder builder = new StringBuilder(name.String);
            builder.Append(first);
            builder.Append(name[name.Length - 1]);
            builder.Append(second);
            return builder.ToString();
        }

        private static SpriteRotations CreateSpriteFrom(UpperString name)
        {
            Material none = TextureManager.NullMaterial;
            Material[] frames = { none, none, none, none, none, none, none, none };

            // If we have a default rotation, set it to be the rotations for
            // everything and let other valid matches override it later.
            if (TextureManager.TryGetMaterial(name + '0', ResourceNamespace.Sprites, out Material frame0))
                frames = new[] { frame0, frame0, frame0, frame0, frame0, frame0, frame0, frame0 };

            // Track how many 2,8 / 3,7 / 4,6 rotations we find. Write them if
            // we find any.
            int mirrorsFound = 0;
            AddMirrorFrameIfExists(name, '2', '8', frames, ref mirrorsFound);
            AddMirrorFrameIfExists(name, '3', '7', frames, ref mirrorsFound);
            AddMirrorFrameIfExists(name, '4', '6', frames, ref mirrorsFound);

            // Lastly if we have a specific rotation for some frame, use that.
            // This should overwrite all the other ones.
            for (char index = '1'; index <= '8'; index++)
                AddSingleFrameIfExists(name, index, frames);

            if (mirrorsFound == 3)
                return new SpriteRotations(name, frames[0], frames[1], frames[2], frames[3], frames[4]);
            return new SpriteRotations(name, frames[0], frames[1], frames[2], frames[3], frames[4], frames[5], frames[6], frames[7]);
        }

        private static void AddSingleFrameIfExists(UpperString name, char first, Material[] frames)
        {
            UpperString lookupName = name + first;
            if (TextureManager.TryGetMaterial(lookupName, ResourceNamespace.Sprites, out Material material))
                frames[first - '1'] = material;
        }

        private static void AddMirrorFrameIfExists(UpperString name, char first, char second,
            Material[] frames, ref int mirrorsFound)
        {
            UpperString lookupName = MakeRotation(name, first, second);
            if (TextureManager.TryGetMaterial(lookupName, ResourceNamespace.Sprites, out Material material))
            {
                frames[first - '1'] = material;
                frames[second - '1'] = material;
                mirrorsFound++;
            }
        }
    }
}

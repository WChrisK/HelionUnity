using System.Collections.Generic;
using System.Linq;
using Helion.Core.Util;
using Helion.Core.Util.Bytes;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Geometry;
using UnityEngine;
using static Helion.Core.Util.Extensions.EnumerableExtensions;
using static Helion.Core.Util.OptionalHelper;

namespace Helion.Core.Resource.Textures.Definitions.Vanilla
{
    /// <summary>
    /// An image subcomponent of a TextureX definition, which such a definition
    /// is made up of one or more of these.
    /// </summary>
    public class TextureXImage
    {
        /// <summary>
        /// The name of the image.
        /// </summary>
        public readonly UpperString Name;

        /// <summary>
        /// The flags for the image.
        /// </summary>
        public readonly TextureXFlags Flags;

        /// <summary>
        /// The scale of the image.
        /// </summary>
        /// <remarks>
        /// This may go with its own custom scaling. See wiki documentation for
        /// how this value is used (ex: may need to be divided by 8.0 to get a
        /// unit scaling).
        /// </remarks>
        public readonly Vector2 Scale;

        /// <summary>
        /// The dimension of the image.
        /// </summary>
        public readonly Dimension Dimension;

        /// <summary>
        /// A leftover residual component of the definition.
        /// </summary>
        public readonly int ColumnDirectory;

        /// <summary>
        /// All of the patches that make up this image.
        /// </summary>
        public readonly IReadOnlyList<TextureXPatch> Patches;

        private TextureXImage(UpperString name, TextureXFlags flags, Vector2 scale, Dimension dimension,
            int columnDirectory, IReadOnlyList<TextureXPatch> patches)
        {
            Name = name;
            Flags = flags;
            Scale = scale;
            Dimension = dimension;
            ColumnDirectory = columnDirectory;
            Patches = patches;
        }

        public static Optional<TextureXImage> From(ByteReader reader)
        {
            try
            {
                UpperString name = reader.StringWithoutNulls(8);
                TextureXFlags flags = (TextureXFlags) reader.UShort();
                Vector2 scale = new Vector2(reader.Byte(), reader.Byte());
                Dimension dimension = new Dimension(reader.Short(), reader.Short());
                int columnDirectory = reader.Int();
                int patchCount = reader.Short();

                List<TextureXPatch> patches = Range(patchCount).Map(i =>
                {
                    Vec2I offset = new Vec2I(reader.Short(), reader.Short());
                    short patchIndex = reader.Short();
                    short stepDirection = reader.Short();
                    short colormap = reader.Short();
                    return new TextureXPatch(offset, patchIndex, stepDirection, colormap);
                }).ToList();

                return new TextureXImage(name, flags, scale, dimension, columnDirectory, patches);
            }
            catch
            {
                return Empty;
            }
        }
    }
}

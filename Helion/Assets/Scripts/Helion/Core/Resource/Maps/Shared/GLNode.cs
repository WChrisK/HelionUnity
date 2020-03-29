using Helion.Core.Util;
using Helion.Core.Util.Geometry;
using UnityEngine;

namespace Helion.Core.Resource.Maps.Shared
{
    /// <summary>
    /// A GL node for a map.
    /// </summary>
    public class GLNode
    {
        /// <summary>
        /// The partitioning line.
        /// </summary>
        public Line2 Partition;

        /// <summary>
        /// The right bounding box.
        /// </summary>
        public Rect RightBox;

        /// <summary>
        /// The left bounding box.
        /// </summary>
        public Rect LeftBox;

        /// <summary>
        /// The left node, if it exists.
        /// </summary>
        public Optional<GLNode> LeftNode;

        /// <summary>
        /// The right node, if it exists.
        /// </summary>
        public Optional<GLNode> RightNode;

        /// <summary>
        /// The left subsector, if it exists.
        /// </summary>
        public Optional<GLSubsector> LeftSubsector;

        /// <summary>
        /// The right subsector, if it exists.
        /// </summary>
        public Optional<GLSubsector> RightSubsector;

        public GLNode(Line2 partition, Rect rightBox, Rect leftBox)
        {
            Partition = partition;
            RightBox = rightBox;
            LeftBox = leftBox;
            LeftNode = new Optional<GLNode>();
            RightNode = new Optional<GLNode>();
            LeftSubsector = new Optional<GLSubsector>();
            RightSubsector = new Optional<GLSubsector>();
        }
    }
}

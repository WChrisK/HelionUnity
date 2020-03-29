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

        public GLNode(Line2 partition, Rect rightBox, Rect leftBox, GLNode leftNode, GLNode rightNode) :
            this(partition, rightBox, leftBox, leftNode, rightNode, null, null)
        {
        }

        public GLNode(Line2 partition, Rect rightBox, Rect leftBox, GLNode leftNode, GLSubsector rightSubsector) :
            this(partition, rightBox, leftBox, leftNode, null, null, rightSubsector)
        {
        }

        public GLNode(Line2 partition, Rect rightBox, Rect leftBox, GLSubsector leftSubsector, GLNode rightNode) :
            this(partition, rightBox, leftBox, null, rightNode, leftSubsector, null)
        {
        }

        public GLNode(Line2 partition, Rect rightBox, Rect leftBox, GLSubsector leftSubsector, GLSubsector rightSubsector) :
            this(partition, rightBox, leftBox, null, null, leftSubsector, rightSubsector)
        {
        }

        private GLNode(Line2 partition, Rect rightBox, Rect leftBox, GLNode leftNode, GLNode rightNode,
                       GLSubsector leftSubsector, GLSubsector rightSubsector)
        {
            Partition = partition;
            RightBox = rightBox;
            LeftBox = leftBox;
            LeftNode = leftNode;
            RightNode = rightNode;
            LeftSubsector = leftSubsector;
            RightSubsector = rightSubsector;
        }
    }
}

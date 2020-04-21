using System.Collections.Generic;
using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using UnityEngine;
using static Helion.Core.Util.OptionalHelper;

namespace Helion.Core.Worlds.Geometry.Bsp
{
    /// <summary>
    /// Contains the BSP tree for a map.
    /// </summary>
    public class BspTree
    {
        private readonly BspNode[] nodes;
        private readonly List<Subsector> subsectors;

        private BspNode root => nodes[nodes.Length - 1];

        private BspTree(BspNode[] nodes, List<Subsector> subsectors)
        {
            this.nodes = nodes;
            this.subsectors = subsectors;
        }

        /// <summary>
        /// Reads the BSP tree from the map.
        /// </summary>
        /// <param name="glNodes">The nodes for the map.</param>
        /// <param name="subsectors">The list of subsectors.</param>
        /// <returns>The BSP tree, or an empty value if the tree is corrupt or
        /// there are no nodes/subsectors.</returns>
        public static Optional<BspTree> From(IList<GLNode> glNodes, List<Subsector> subsectors)
        {
            if (glNodes.Empty() || subsectors.Empty())
                return Empty;

            try
            {
                BspNode[] nodes = new BspNode[glNodes.Count];
                RecursivelyBuildNodes(nodes, glNodes, (uint)(nodes.Length - 1));
                return new BspTree(nodes, subsectors);
            }
            catch
            {
                return Empty;
            }
        }

        /// <summary>
        /// Finds the subsector at the point provided.
        /// </summary>
        /// <param name="point">The world location.</param>
        /// <returns>The subsector for the point.</returns>
        public Subsector Subsector(Vector2 point)
        {
            BspNode node = root;

            while (true)
            {
                if (node.OnRight(point))
                {
                    if (node.IsRightSubsector)
                        return subsectors[node.RightSubsectorIndex];

                    node = nodes[node.RightChild];
                }
                else
                {
                    if (node.IsLeftSubsector)
                        return subsectors[node.LeftSubsectorIndex];

                    node = nodes[node.LeftChild];
                }
            }
        }

        /// <summary>
        /// Finds the subsector at the point provided.
        /// </summary>
        /// <param name="point">The world location.</param>
        /// <returns>The subsector for the point.</returns>
        public Subsector Subsector(Vector3 point) => Subsector(new Vector2(point.x, point.z));

        /// <summary>
        /// Finds the sector at the point provided.
        /// </summary>
        /// <param name="point">The world location.</param>
        /// <returns>The sector for the point.</returns>
        public Sector Sector(Vector2 point) => Subsector(point).Sector;

        /// <summary>
        /// Finds the sector at the point provided.
        /// </summary>
        /// <param name="point">The world location.</param>
        /// <returns>The sector for the point.</returns>
        public Sector Sector(Vector3 point) => Sector(new Vector2(point.x, point.z));

        private static uint RecursivelyBuildNodes(BspNode[] nodes, IList<GLNode> glNodes, uint index)
        {
            GLNode glNode = glNodes[(int)index];
            uint leftIndex = RecursivelyBuildNode(nodes, glNodes, glNode.LeftNode, glNode.LeftSubsector);
            uint rightIndex = RecursivelyBuildNode(nodes, glNodes, glNode.RightNode, glNode.RightSubsector);

            BspNode node = new BspNode(leftIndex, rightIndex, glNode.Partition);
            nodes[index] = node;
            return index;
        }

        private static uint RecursivelyBuildNode(BspNode[] nodes, IList<GLNode> glNodes, Optional<GLNode> node,
            Optional<GLSubsector> subsector)
        {
            if (node.HasValue)
                return RecursivelyBuildNodes(nodes, glNodes, (uint)node.Value.Index);
            return (uint)subsector.Value.Index | BspNode.ChildBit;
        }
    }
}

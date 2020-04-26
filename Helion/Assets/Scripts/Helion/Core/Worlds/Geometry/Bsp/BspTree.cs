using System;
using System.Collections.Generic;
using System.Linq;
using Helion.Bsp.Geometry;
using Helion.Bsp.Node;
using Helion.Core.Util.Geometry.Segments;
using Helion.Core.Worlds.Geometry.Subsectors;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry.Bsp
{
    /// <summary>
    /// The BSP tree that contains all of the subsectors and nodes.
    /// </summary>
    public class BspTree : IDisposable
    {
        public readonly List<Subsector> Subsectors = new List<Subsector>();
        public readonly List<SubsectorPlane> SubsectorPlanes = new List<SubsectorPlane>();
        private readonly List<CompactBspNode> nodes = new List<CompactBspNode>();
        private readonly MapGeometry geometry;

        private CompactBspNode root => nodes.LastOrDefault();

        /// <summary>
        /// Creates and builds a BSP tree.
        /// </summary>
        /// <remarks>
        /// Depending on the size of the tree, this may be a slow operation.
        /// </remarks>
        /// <param name="mapGeometry">The owning object.</param>
        /// <param name="root">The tree root to build from.</param>
        public BspTree(MapGeometry mapGeometry, BspNode root)
        {
            geometry = mapGeometry;

            RecursivelyHandleNode(root);
        }

        /// <summary>
        /// Finds the sector at the point provided.
        /// </summary>
        /// <param name="point">The world location.</param>
        /// <returns>The sector for the point.</returns>
        public Sector Sector(in Vector2 point) => Subsector(point).Sector;

        /// <summary>
        /// Finds the sector at the point provided.
        /// </summary>
        /// <param name="point">The world location.</param>
        /// <returns>The sector for the point.</returns>
        public Sector Sector(in Vector3 point) => Sector(new Vector2(point.x, point.y));

        /// <summary>
        /// Finds the subsector at the point provided.
        /// </summary>
        /// <param name="point">The world location.</param>
        /// <returns>The subsector for the point.</returns>
        public Subsector Subsector(in Vector2 point)
        {
            CompactBspNode node = root;

            while (true)
            {
                if (node.OnRight(point))
                {
                    if (node.RightIsSubsector)
                        return Subsectors[(int)node.RightChildWithoutBit];

                    node = nodes[(int)node.RightChildBits];
                }
                else
                {
                    if (node.LeftIsSubsector)
                        return Subsectors[(int)node.LeftChildWithoutBit];

                    node = nodes[(int)node.LeftChildBits];
                }
            }
        }

        public void Dispose()
        {
            SubsectorPlanes.ForEach(subsectorPlane => subsectorPlane.Dispose());
        }

        private static Seg2F MakeFloatSplitter(BspSegment segment)
        {
            return new Seg2F(segment.StartVertex.Float(), segment.EndVertex.Float());
        }

        private static Seg2F EdgeToLine(SubsectorEdge edge)
        {
            Vector2 start = new Vector2((float)edge.Start.X, (float)edge.Start.Y);
            Vector2 end = new Vector2((float)edge.End.X, (float)edge.End.Y);
            return new Seg2F(start, end);
        }

        private uint RecursivelyHandleNode(BspNode node)
        {
            Debug.Assert(!node.IsDegenerate, "Degenerate BSP node encountered when building world tree");

            if (node.IsSubsector)
                return CreateSubsector(node);

            uint leftIndex = RecursivelyHandleNode(node.Left);
            uint rightIndex = RecursivelyHandleNode(node.Right);

            uint nodeIndex = (uint)nodes.Count;
            Seg2F splitter = MakeFloatSplitter(node.Splitter);
            CompactBspNode bspNode = new CompactBspNode(leftIndex, rightIndex, splitter);
            nodes.Add(bspNode);

            return nodeIndex;
        }

        private uint CreateSubsector(BspNode node)
        {
            Sector sector = null;
            List<Seg2F> edges = new List<Seg2F>();

            foreach (SubsectorEdge edge in node.ClockwiseEdges)
            {
                edges.Add(EdgeToLine(edge));

                // If we haven't found the sector yet, we need to keep looking
                // for it. The sector can only be found from non-minisegs.
                if (edge.IsMiniseg || sector != null)
                    continue;

                // TODO: We probably won't always be able to make this assumption of line indices...
                Line line = geometry.Lines[edge.Line.Index];
                sector = edge.IsFront ? line.Front.Sector : line.Back.Value.Sector;
            }

            if (sector == null)
                throw new Exception("Encountered a fully miniseg subsector, this should never happen");

            SubsectorPlane floor = new SubsectorPlane(SubsectorPlanes.Count, sector.Floor, edges);
            SubsectorPlanes.Add(floor);

            SubsectorPlane ceiling = new SubsectorPlane(SubsectorPlanes.Count, sector.Ceiling, edges);
            SubsectorPlanes.Add(ceiling);

            int index = Subsectors.Count;
            Subsector subsector = new Subsector(index, sector, floor, ceiling);
            Subsectors.Add(subsector);

            return (uint)index | CompactBspNode.IsSubsectorBit;
        }
    }
}

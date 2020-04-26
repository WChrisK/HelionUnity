using System;
using System.Collections.Generic;
using Helion.Bsp.Geometry;
using Helion.Bsp.Node;
using Helion.Core.Util.Geometry.Segments;
using UnityEngine;

namespace Helion.Core.Worlds.Geometry.Bsp
{
    public class BspTree : IDisposable
    {
        public readonly List<Subsector> Subsectors = new List<Subsector>();
        private readonly List<CompactBspNode> nodes = new List<CompactBspNode>();
        private readonly MapGeometry geometry;

        public BspTree(MapGeometry mapGeometry, BspNode root)
        {
            geometry = mapGeometry;

            RecursivelyHandleNode(root);
        }

        public void Dispose()
        {
            Subsectors.ForEach(subsector => subsector.Dispose());
        }
        
        private static Line2F MakeFloatSplitter(BspSegment segment)
        {
            return new Line2F(segment.StartVertex.Float(), segment.EndVertex.Float());
        }

        private static Line2F EdgeToLine(SubsectorEdge edge)
        {
            Vector2 start = new Vector2((float)edge.Start.X, (float)edge.Start.Y);
            Vector2 end = new Vector2((float)edge.End.X, (float)edge.End.Y);
            return new Line2F(start, end);
        }

        private uint RecursivelyHandleNode(BspNode node)
        {
            Debug.Assert(!node.IsDegenerate, "Degenerate BSP node encountered when building world tree");

            if (node.IsSubsector)
                return CreateSubsector(node);

            uint leftIndex = RecursivelyHandleNode(node.Left);
            uint rightIndex = RecursivelyHandleNode(node.Right);

            uint nodeIndex = (uint)nodes.Count;
            Line2F splitter = MakeFloatSplitter(node.Splitter);
            CompactBspNode bspNode = new CompactBspNode(leftIndex, rightIndex, splitter);
            nodes.Add(bspNode);

            return nodeIndex;
        }

        private uint CreateSubsector(BspNode node)
        {
            Sector sector = null;
            List<Line2F> edges = new List<Line2F>();

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

            int index = Subsectors.Count;
            Subsector subsector = new Subsector(index, sector, edges);
            Subsectors.Add(subsector);

            return (uint)index | CompactBspNode.IsSubsectorBit;
        }
    }
}

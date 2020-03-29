using System;
using System.Collections.Generic;
using Helion.Core.Util.Bytes;
using Helion.Core.Util.Geometry;
using UnityEngine;

namespace Helion.Core.Resource.Maps.Shared
{
    /// <summary>
    /// A helper class for reading GL components.
    /// </summary>
    public static class GLReader
    {
        public const int BytesPerGLVertexV2 = 8;
        public const int BytesPerGLSegV1 = 10;
        public const int BytesPerGLSubsectorV1 = 4;
        public const int BytePerGLNodeV1 = 28;
        public const ushort GLVertexIsGLV2 = 1 << 15;
        public const ushort GLNodeIsSubsectorV2 = 1 << 15;

        /// <summary>
        /// Reads the list of GL vertices from the map components object.
        /// </summary>
        /// <param name="components">The map components object.</param>
        /// <returns>A list of GL vertices.</returns>
        /// <exception cref="Exception">If it is not compiled to match the V2
        /// specification of GLBSP nodes.</exception>
        public static IList<MapVertex> ReadGLVertices(MapComponents components)
        {
            IList<MapVertex> glVertices = new List<MapVertex>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.GLVertices.Value.Data);

            string header = reader.String(4);
            if (header != "gNd2")
                throw new Exception("Currently unsupported (expected gNd2 for GL_VERT)");

            int count = (reader.Length - 4) / BytesPerGLVertexV2;
            for (int i = 0; i < count; i++)
            {
                float x = new Fixed(reader.Int()).Float();
                float y = new Fixed(reader.Int()).Float();
                MapVertex vertex = new MapVertex(x, y);
                glVertices.Add(vertex);
            }

            return glVertices;
        }

        /// <summary>
        /// Reads the GL segments. Assumes the data is in GLBSP V2.
        /// </summary>
        /// <param name="components">The map components.</param>
        /// <param name="vertices">The regular map vertices.</param>
        /// <param name="glVertices">The extra GL vertices.</param>
        /// <returns>A list of the GL segments.</returns>
        /// <exception cref="Exception">If it is not compiled to match the V2
        /// specification of GLBSP nodes.</exception>
        public static IList<GLSegment> ReadGLSegments(MapComponents components,
            IList<MapVertex> vertices, IList<MapVertex> glVertices)
        {
            IList<GLSegment> segments = new List<GLSegment>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.GLSegments.Value.Data);

            int count = reader.Length / BytesPerGLSegV1;
            for (int i = 0; i < count; i++)
            {
                MapVertex start = GetMapVertexV2(reader.UShort(), vertices, glVertices);
                MapVertex end = GetMapVertexV2(reader.UShort(), vertices, glVertices);
                Line2 line = new Line2(start.Vector, end.Vector);
                ushort linedefIndex = reader.UShort();
                bool onRightSide = reader.UShort() == 0;
                ushort partnerSegIndex = reader.UShort();

                GLSegment segment = new GLSegment(line, onRightSide);
                segments.Add(segment);
            }

            return segments;
        }

        /// <summary>
        /// Reads the GL subsectors.
        /// </summary>
        /// <param name="components">The map components.</param>
        /// <param name="segments">The GL segments for the map.</param>
        /// <returns>A list of the GL subsectors.</returns>
        /// <exception cref="Exception">If it is not compiled to match the V2
        /// specification of GLBSP nodes.</exception>
        public static IList<GLSubsector> ReadGLSubsectors(MapComponents components, IList<GLSegment> segments)
        {
            IList<GLSubsector> subsectors = new List<GLSubsector>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.GLSubsectors.Value.Data);

            int count = reader.Length / BytesPerGLSubsectorV1;
            for (int i = 0; i < count; i++)
            {
                int segCount = reader.UShort();
                int firstSeg = reader.UShort();
                IList<GLSegment> edges = new List<GLSegment>();

                for (int segIndex = firstSeg; segIndex < firstSeg + segCount; segIndex++)
                    edges.Add(segments[segIndex]);

                GLSubsector subsector = new GLSubsector(edges);
                subsectors.Add(subsector);
            }

            return subsectors;
        }

        /// <summary>
        /// Reads the GL nodes.
        /// </summary>
        /// <param name="components">The map components.</param>
        /// <param name="subsectors">The GL subsectors.</param>
        /// <returns>A list of GL nodes.</returns>
        /// <exception cref="Exception">If it is not compiled to match the V2
        /// specification of GLBSP nodes.</exception>
        public static IList<GLNode> ReadGLNodes(MapComponents components, IList<GLSubsector> subsectors)
        {
            IList<GLNode> nodes = new List<GLNode>();
            IList<NodeChildren> children = new List<NodeChildren>();

            ByteReader reader = ByteReader.From(ByteOrder.Little, components.GLNodes.Value.Data);

            int count = reader.Length / BytePerGLNodeV1;
            for (int i = 0; i < count; i++)
            {
                short x = reader.Short();
                short y = reader.Short();
                short dx = reader.Short();
                short dy = reader.Short();
                Line2 partition = new Line2(x, y, x + dx, y + dy);
                Rect rightBox = ReadNodeBox(reader);
                Rect leftBox = ReadNodeBox(reader);

                // NOTE: This only works for V2 nodes for now.
                NodeChildren childInfo = new NodeChildren(reader.UShort(), reader.UShort());
                children.Add(childInfo);

                GLNode node = new GLNode(partition, rightBox, leftBox);
                nodes.Add(node);
            }

            // Since we don't have the classes created while we're going
            // through the list, we have to do this after the fact. It sucks
            // but that is the nature of it unfortunately if we want to have
            // references.
            for (int i = 0; i < count; i++)
            {
                NodeChildren childInfo = children[i];
                GLNode node = nodes[i];

                if (childInfo.RightIsNode && childInfo.LeftIsNode)
                {
                    node.RightNode = nodes[childInfo.RightChildNoGLBit];
                    node.LeftNode = nodes[childInfo.LeftChildNoGLBit];
                }
                else if (childInfo.RightIsNode && !childInfo.LeftIsNode)
                {
                    node.RightNode = nodes[childInfo.RightChildNoGLBit];
                    node.LeftSubsector = subsectors[childInfo.LeftChildNoGLBit];
                }
                else if (!childInfo.RightIsNode && childInfo.LeftIsNode)
                {
                    node.RightSubsector = subsectors[childInfo.RightChildNoGLBit];
                    node.LeftNode = nodes[childInfo.LeftChildNoGLBit];
                }
                else
                {
                    node.RightSubsector = subsectors[childInfo.RightChildNoGLBit];
                    node.LeftSubsector = subsectors[childInfo.LeftChildNoGLBit];
                }
            }

            return nodes;
        }

        private static MapVertex GetMapVertexV2(ushort index, IList<MapVertex> vertices, IList<MapVertex> glVertices)
        {
            return (index & GLVertexIsGLV2) == GLVertexIsGLV2 ?
                glVertices[index & ~GLVertexIsGLV2] :
                vertices[index];
        }

        private static Rect ReadNodeBox(ByteReader reader)
        {
            float top = reader.Short();
            float bottom = reader.Short();
            float left = reader.Short();
            float right = reader.Short();

            return new Rect(bottom, left, right - left, top - bottom);
        }
    }

    /// <summary>
    /// A simple container for node index information.
    /// </summary>
    internal readonly struct NodeChildren
    {
        /// <summary>
        /// The bits for the right child.
        /// </summary>
        public readonly ushort RightChild;

        /// <summary>
        /// The bits for the left child.
        /// </summary>
        public readonly ushort LeftChild;

        /// <summary>
        /// If the right child is a node, false if it's a subsector.
        /// </summary>
        public bool RightIsNode => (RightChild & GLReader.GLNodeIsSubsectorV2) == 0;

        /// <summary>
        /// If the left child is a node, false if it's a subsector.
        /// </summary>
        public bool LeftIsNode => (LeftChild & GLReader.GLNodeIsSubsectorV2) == 0;

        /// <summary>
        /// Gets the right child without the GL bit.
        /// </summary>
        public ushort RightChildNoGLBit => (ushort)(RightChild & ~GLReader.GLNodeIsSubsectorV2);

        /// <summary>
        /// Gets the left child without the GL bit.
        /// </summary>
        public ushort LeftChildNoGLBit => (ushort)(LeftChild & ~GLReader.GLNodeIsSubsectorV2);

        internal NodeChildren(ushort rightChild, ushort leftChild)
        {
            RightChild = rightChild;
            LeftChild = leftChild;
        }
    }
}

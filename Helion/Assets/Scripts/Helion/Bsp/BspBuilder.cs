using System;
using System.Collections.Generic;
using Helion.Bsp.Geometry;
using Helion.Bsp.Node;
using Helion.Bsp.States;
using Helion.Bsp.States.Convex;
using Helion.Bsp.States.Miniseg;
using Helion.Bsp.States.Partition;
using Helion.Bsp.States.Split;
using Helion.Core.Resource.Maps;
using Helion.Core.Resource.Maps.Components;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Geometry.Segments;
using UnityEngine;

namespace Helion.Bsp
{
    /// <summary>
    /// Builds a BSP tree.
    /// </summary>
    /// <remarks>
    /// Currently not thread safe, some state is shared (the non-readonly ones)
    /// which prevents threading from being used recursively on a tree. However
    /// it is possible to apply threading to each stage itself since those are
    /// 'embarrassingly parallel'.
    /// </remarks>
    public class BspBuilder
    {
        /// <summary>
        /// A counter which is used to make sure that we don't enter some
        /// infinite loop due to any bugs. In a properly implemented builder
        /// this will never be reached.
        /// </summary>
        private const int RecursiveOverflowAmount = 10000;

        public BspState State { get; private set; } = BspState.NotStarted;
        public readonly ConvexChecker ConvexChecker;
        public readonly SplitCalculator SplitCalculator;
        public readonly Partitioner Partitioner;
        public readonly MinisegCreator MinisegCreator;
        public readonly VertexAllocator VertexAllocator;
        public readonly SegmentAllocator SegmentAllocator;
        private readonly BspConfig BspConfig;
        private readonly BspNode root = new BspNode();
        private readonly Stack<WorkItem> workItems = new Stack<WorkItem>();
        private bool foundDegenerateNode;

        /// <summary>
        /// If true, then all the work is done and the result is available.
        /// </summary>
        public bool Done => State == BspState.Complete;

        /// <summary>
        /// Gets the current work item, or returns null if the collection is
        /// empty.
        /// </summary>
        public WorkItem CurrentWorkItem => workItems.Empty() ? null : workItems.Peek();

        public BspBuilder(MapData map) : this(new BspConfig(), map)
        {
        }

        public BspBuilder(BspConfig config, MapData map)
        {
            BspConfig = config;
            CollinearTracker collinearTracker = new CollinearTracker(config.VertexWeldingEpsilon);
            JunctionClassifier junctionClassifier = new JunctionClassifier();

            VertexAllocator = new VertexAllocator(config.VertexWeldingEpsilon);
            SegmentAllocator = new SegmentAllocator(VertexAllocator, collinearTracker);
            ConvexChecker = new ConvexChecker();
            SplitCalculator = new SplitCalculator(config, collinearTracker);
            Partitioner = new Partitioner(config, SegmentAllocator, junctionClassifier);
            MinisegCreator = new MinisegCreator(VertexAllocator, SegmentAllocator, junctionClassifier);

            List<BspSegment> segments = ProcessMapLines(map);

            junctionClassifier.Add(segments);
            CreateInitialWorkItem(segments);
        }

        /// <summary>
        /// Moves until either the current work item is the branch provided, or
        /// the building is done.
        /// </summary>
        /// <param name="branch">The branch path to go to.</param>
        public void ExecuteUntilBranch(string branch)
        {
            string upperBranch = branch.ToUpper();
            while (!Done)
            {
                WorkItem item = CurrentWorkItem;
                if (item == null || item.BranchPath == upperBranch)
                    break;
                ExecuteFullCycleStep();
            }
        }

        /// <summary>
        /// Steps through all major states until it reaches the convexity check
        /// or it completes.
        /// </summary>
        public void ExecuteFullCycleStep()
        {
            if (Done)
                return;

            do
                ExecuteMajorStep();
            while (State != BspState.CheckingConvexity && !Done);
        }

        /// <summary>
        /// Advances to the next major state.
        /// </summary>
        public void ExecuteMajorStep()
        {
            if (Done)
                return;

            BspState originalState = State;
            BspState currentState = State;
            while (originalState == currentState && !Done)
            {
                Execute();
                currentState = State;
            }
        }

        /// <summary>
        /// Builds the tree completely. Returns null on failure.
        /// </summary>
        /// <returns>The built tree, or null on failure.</returns>
        public BspNode Build()
        {
            while (!Done)
                ExecuteFullCycleStep();

            if (foundDegenerateNode)
                root.StripDegenerateNodes();

            return root.IsDegenerate ? null : root;
        }

        /// <summary>
        /// Executes an atomic step forward, meaning it moves ahead by the most
        /// indivisible element that allows a debugging session to see every
        /// state change independently.
        /// </summary>
        public void Execute()
        {
            switch (State)
            {
            case BspState.NotStarted:
                LoadNextWorkItem();
                break;

            case BspState.CheckingConvexity:
                ExecuteConvexityCheck();
                break;

            case BspState.CreatingLeafNode:
                ExecuteLeafNodeCreation();
                break;

            case BspState.FindingSplitter:
                ExecuteSplitterFinding();
                break;

            case BspState.PartitioningSegments:
                ExecuteSegmentPartitioning();
                break;

            case BspState.GeneratingMinisegs:
                ExecuteMinisegGeneration();
                break;

            case BspState.FinishingSplit:
                ExecuteSplitFinalization();
                break;
            }
        }

        private List<BspSegment> ReadLinesFrom(MapData map)
        {
            List<BspSegment> segments = new List<BspSegment>();

            foreach (MapLinedef line in map.Linedefs)
            {
                MapVertex startMapVertex = map.Vertices[line.StartVertex];
                MapVertex endMapVertex = map.Vertices[line.EndVertex];

                BspVertex start = VertexAllocator[startMapVertex.Struct()];
                BspVertex end = VertexAllocator[endMapVertex.Struct()];

                BspSegment segment = SegmentAllocator.GetOrCreate(start, end, line);
                segments.Add(segment);
            }

            return segments;
        }

        private List<BspSegment> ProcessMapLines(MapData map)
        {
            List<BspSegment> segments = ReadLinesFrom(map);
            return SegmentChainPruner.Prune(segments);
        }

        private void CreateInitialWorkItem(List<BspSegment> segments)
        {
            WorkItem workItem = new WorkItem(root, segments);
            workItems.Push(workItem);
        }

        /// <summary>
        /// Takes the convex traversal that was done and adds it to the top BSP
        /// node on the stack. This effectively creates the subsector.
        /// </summary>
        private void AddConvexTraversalToTopNode()
        {
            Debug.Assert(!workItems.Empty(), "Cannot add convex traversal to an empty work item stack");

            ConvexTraversal traversal = ConvexChecker.States.ConvexTraversal;
            Rotation rotation = ConvexChecker.States.Rotation;
            List<SubsectorEdge> edges = SubsectorEdge.FromClockwiseTraversal(traversal, rotation);

            workItems.Peek().Node.ClockwiseEdges = edges;
        }

        private void LoadNextWorkItem()
        {
            Debug.Assert(workItems.Count > 0, "Expected a root work item to be present");

            ConvexChecker.Load(workItems.Peek().Segments);
            State = BspState.CheckingConvexity;
        }

        private void ExecuteConvexityCheck()
        {
            Debug.Assert(workItems.Count < RecursiveOverflowAmount, "BSP recursive overflow detected");

            switch (ConvexChecker.States.State)
            {
            case ConvexState.Loaded:
            case ConvexState.Traversing:
                ConvexChecker.Execute();
                break;

            case ConvexState.FinishedIsDegenerate:
                foundDegenerateNode = true;
                goto case ConvexState.FinishedIsConvex;
            case ConvexState.FinishedIsConvex:
                State = BspState.CreatingLeafNode;
                break;

            case ConvexState.FinishedIsSplittable:
                WorkItem workItem = workItems.Peek();
                SplitCalculator.Load(workItem.Segments);
                State = BspState.FindingSplitter;
                break;
            }
        }

        private void ExecuteLeafNodeCreation()
        {
            ConvexState convexState = ConvexChecker.States.State;
            Debug.Assert(convexState == ConvexState.FinishedIsDegenerate || convexState == ConvexState.FinishedIsConvex, "Unexpected BSP leaf building state");

            if (convexState == ConvexState.FinishedIsConvex)
                AddConvexTraversalToTopNode();

            workItems.Pop();

            if (workItems.Empty())
                State = BspState.Complete;
            else
                LoadNextWorkItem();
        }

        private void ExecuteSplitterFinding()
        {
            switch (SplitCalculator.States.State)
            {
            case SplitterState.Loaded:
            case SplitterState.Working:
                SplitCalculator.Execute();
                break;

            case SplitterState.Finished:
                Partitioner.Load(SplitCalculator.States.BestSplitter, workItems.Peek().Segments);
                State = BspState.PartitioningSegments;
                break;
            }
        }

        private void ExecuteSegmentPartitioning()
        {
            switch (Partitioner.States.State)
            {
            case PartitionState.Loaded:
            case PartitionState.Working:
                Partitioner.Execute();
                break;

            case PartitionState.Finished:
                if (Partitioner.States.Splitter == null)
                    throw new NullReferenceException("Unexpected null partition splitter");
                BspSegment splitter = Partitioner.States.Splitter;
                MinisegCreator.Load(splitter, Partitioner.States.CollinearVertices);
                State = BspState.GeneratingMinisegs;
                break;
            }
        }

        private void ExecuteMinisegGeneration()
        {
            switch (MinisegCreator.States.State)
            {
            case MinisegState.Loaded:
            case MinisegState.Working:
                MinisegCreator.Execute();
                break;

            case MinisegState.Finished:
                State = BspState.FinishingSplit;
                break;
            }
        }

        private void ExecuteSplitFinalization()
        {
            WorkItem currentWorkItem = workItems.Pop();

            BspNode parentNode = currentWorkItem.Node;
            BspNode leftChild = new BspNode();
            BspNode rightChild = new BspNode();
            parentNode.SetChildren(leftChild, rightChild);
            parentNode.Splitter = SplitCalculator.States.BestSplitter;

            List<BspSegment> rightSegs = Partitioner.States.RightSegments;
            List<BspSegment> leftSegs = Partitioner.States.LeftSegments;
            rightSegs.AddRange(MinisegCreator.States.Minisegs);
            leftSegs.AddRange(MinisegCreator.States.Minisegs);

            string path = currentWorkItem.BranchPath;

            if (BspConfig.BranchRight)
            {
                workItems.Push(new WorkItem(leftChild, leftSegs, path + "L"));
                workItems.Push(new WorkItem(rightChild, rightSegs, path + "R"));
            }
            else
            {
                workItems.Push(new WorkItem(rightChild, rightSegs, path + "R"));
                workItems.Push(new WorkItem(leftChild, leftSegs, path + "L"));
            }

            LoadNextWorkItem();
        }
    }
}

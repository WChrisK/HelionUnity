using System.Collections.Generic;
using Helion.Util;
using Helion.Util.Logging;
using UnityEngine;

namespace Helion.Resource.Decorate.Definitions.States
{
    /// <summary>
    /// All of the states for when an actor is ticked. Also contains rendering
    /// information for frames.
    /// </summary>
    /// <remarks>
    /// The state's flow overrides are when we want to redirect a label to a
    /// new position. This is used to redirect a label with a goto statement
    /// or to remove a label with the stop statement. Such actions should be
    /// done internally and not exposed to the outside world since it is an
    /// implementation detail.
    /// </remarks>
    public class ActorStates
    {
        private static readonly Log Log = LogManager.Instance();

        public readonly ActorStateLabels Labels = new ActorStateLabels();
        public readonly List<ActorFrame> Frames = new List<ActorFrame>();
        internal readonly Dictionary<UpperString, ActorFlowControl> flowOverrides = new Dictionary<UpperString,ActorFlowControl>();
        private readonly ActorDefinition definition;

        public ActorStates(ActorDefinition owner)
        {
            definition = owner;
        }

        public ActorStates(ActorDefinition owner, ActorStates other, UpperString parentName) :
            this(owner)
        {
            foreach (ActorFrame frame in other.Frames)
            {
                ActorFrame newFrame = new ActorFrame(frame);
                Frames.Add(newFrame);
            }

            Labels = new ActorStateLabels(other.Labels, parentName);

            // Note: We are not copying flow overrides because they should be
            // specific to their object definition.
        }

        internal void ResolveLabelsAndOverrides()
        {
            ApplyLabelOffsets();
            ResolveFlowOverrides();
            // TODO: Make sure all offsets are inside the bounds of [0, Frames.Count)!
        }

        private void ApplyLabelOffsets()
        {
            foreach (ActorFrame frame in Frames)
            {
                if (!frame.NeedsToSetStateOffset)
                    continue;

                switch (frame.FlowControl.FlowType)
                {
                case ActorStateBranch.Loop:
                case ActorStateBranch.Goto:
                    frame.NextStateOffset = LabelToOffset(frame) ?? 0;
                    break;
                case ActorStateBranch.Fail:
                case ActorStateBranch.Stop:
                case ActorStateBranch.Wait:
                    frame.NextStateOffset = 0;
                    break;
                case ActorStateBranch.None:
                    frame.NextStateOffset = 1;
                    break;
                default:
                    Log.Error($"Unknown frame branch type {frame.FlowControl.FlowType} in actor {definition.Name}");
                    break;
                }
            }
        }

        private void ResolveFlowOverrides()
        {
            foreach (var labelFlowPair in flowOverrides)
            {
                UpperString label = labelFlowPair.Key;
                ActorFlowControl flowControl = labelFlowPair.Value;

                if (!Labels.Contains(label))
                {
                    Log.Error($"Unable to find label {label} in actor {definition.Name}");
                    continue;
                }

                switch (flowControl.FlowType)
                {
                case ActorStateBranch.Goto:
                    int newOffset = ResolveOverrideGotoOffset(flowControl) ?? 0;
                    Labels.Add(label, newOffset);
                    break;
                case ActorStateBranch.Stop:
                    Labels.Remove(label);
                    break;
                default:
                    Log.Error($"Cannot apply flow override branch type {flowControl.FlowType} to label {label} in actor {definition.Name}");
                    break;
                }
            }
        }

        private int? LabelToOffset(ActorFrame frame)
        {
            ActorFlowControl flowControl = frame.FlowControl;
            ActorStateBranch type = flowControl.FlowType;
            Debug.Assert(type == ActorStateBranch.Goto || type == ActorStateBranch.Loop, "Expected Goto or Loop label here only");

            int? index;
            UpperString label = flowControl.Label.Value;

            if (type == ActorStateBranch.Goto && flowControl.Parent)
            {
                UpperString parent = flowControl.Parent.Value;
                index = GetSuperOrParent(parent, label);

                if (index == null)
                {
                    Log.Error($"Unable to find label: {parent}::{label}");
                    return null;
                }
            }
            else
            {
                index = Labels[label];
                if (index == null)
                {
                    Log.Error($"Unable to find label: {label}");
                    return null;
                }
            }

            // The offset to the label is the delta from our current position,
            // plus any extra offset that the flow control will have provided.
            return index.Value - frame.FrameIndex + flowControl.Offset;
        }

        private int? ResolveOverrideGotoOffset(ActorFlowControl flowControl)
        {
            if (!flowControl.Label)
                return null;

            int? index;
            UpperString label = flowControl.Label.Value;

            if (flowControl.Parent)
                index = GetSuperOrParent(flowControl.Parent.Value, label);
            else
                index = Labels[label];

            if (index == null)
                return null;
            return index.Value + flowControl.Offset;
        }

        private int? GetSuperOrParent(UpperString parent, UpperString label)
        {
            return parent == "SUPER" ? Labels.Super(label) : Labels.Parent(parent, label);
        }
    }
}

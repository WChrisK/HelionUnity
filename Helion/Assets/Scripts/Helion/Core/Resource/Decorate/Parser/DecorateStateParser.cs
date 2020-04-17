using System.Collections.Generic;
using System.Linq;
using System.Text;
using Helion.Core.Resource.Decorate.Definitions.States;
using Helion.Core.Resource.Maps.Actions;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using Helion.Core.Util.Geometry;

namespace Helion.Core.Resource.Decorate.Parser
{
    public partial class DecorateParser
    {
        private static readonly HashSet<UpperString> FramePropertyKeywords = new HashSet<UpperString>
        {
            "BRIGHT", "CANRAISE", "FAST", "LIGHT", "NODELAY", "OFFSET", "SLOW",
        };

        private int frameIndex;
        private UpperString justSeenLabelOrNull;

        private static bool IsValidFrameLetter(char frame)
        {
            return frame == '#' || frame == '-' || frame == '_' || frame == '\\' ||
                   (frame >= '[' && frame <= ']') ||
                   (frame >= '0' && frame <= '9') ||
                   (frame >= 'A' && frame <= 'Z') ||
                   (frame >= 'a' && frame <= 'z');
        }

        private static bool TryGetStateBranch(UpperString text, out ActorStateBranch branchType)
        {
            switch (text.String)
            {
            case "FAIL":
                branchType = ActorStateBranch.Fail;
                return true;
            case "GOTO":
                branchType = ActorStateBranch.Goto;
                return true;
            case "LOOP":
                branchType = ActorStateBranch.Loop;
                return true;
            case "STOP":
                branchType = ActorStateBranch.Stop;
                return true;
            case "WAIT":
                branchType = ActorStateBranch.Wait;
                return true;
            }

            branchType = ActorStateBranch.None;
            return false;
        }

        private ActorFrame GetLastFrameOrThrow()
        {
            if (currentDefinition.States.Frames.Empty())
                throw MakeException("Cannot have a flow control label when no frames were defined");

            return currentDefinition.States.Frames.Last();
        }

        private ActorFlowControl ReadGotoLabel()
        {
            Optional<UpperString> label = ConsumeIdentifier().AsUpper();
            Optional<UpperString> parent = Optional<UpperString>.Empty();
            int offset = 0;

            if (ConsumeIf(':'))
            {
                Consume(':');
                parent = label;
                label = ConsumeString().AsUpper();
            }

            if (ConsumeIf('+'))
                offset = ConsumeInteger();

            return new ActorFlowControl(ActorStateBranch.Goto, parent, label, offset);
        }

        private void HandleLabelOverride(ActorFlowControl flowControl)
        {
            switch (flowControl.FlowType)
            {
            case ActorStateBranch.Goto:
                throw MakeException("Goto flow control override not supported currently");
            case ActorStateBranch.Stop:
                throw MakeException("Goto flow control override not supported currently");
            default:
                throw MakeException("Flow control override after a label must be either 'Stop' or 'Goto'");
            }
        }

        private void TrackNewLabel(UpperString label)
        {
            currentDefinition.States.Labels[label] = frameIndex;
            justSeenLabelOrNull = label;
        }

        private void ReadDottedLabel(UpperString label)
        {
            // We already read the following, so start off with it.
            StringBuilder labelBuilder = new StringBuilder(label.String);
            labelBuilder.Append('.');

            while (true)
            {
                UpperString labelPiece = ConsumeIdentifier();
                labelBuilder.Append(labelPiece.String);

                // We know we're in a label, so the identifier must be followed
                // by either another separator, or a terminal colon.
                if (!ConsumeIf('.'))
                {
                    Consume(':');
                    break;
                }

                labelBuilder.Append('.');
            }

            TrackNewLabel(labelBuilder.ToString());
        }

        private bool TryReadFlowControl(UpperString text, out ActorFlowControl flowControl)
        {
            flowControl = null;

            if (!TryGetStateBranch(text, out ActorStateBranch branchType))
                return false;

            if (branchType == ActorStateBranch.Goto)
                flowControl = ReadGotoLabel();
            else
                flowControl = new ActorFlowControl(branchType);

            return true;
        }

        private void ApplyStateBranch(ActorFlowControl flowControl)
        {
            if (justSeenLabelOrNull != null)
                HandleLabelOverride(flowControl);
            else
                GetLastFrameOrThrow().FlowControl = flowControl;
        }

        private int ConsumeActorFrameTicks()
        {
            if (ConsumeIf("random"))
            {
                Log.Error("Decorate random() tick duration not properly supported!");

                // We don't check for negative numbers because it's probably
                // not allowed.
                Consume('(');
                int low = ConsumeInteger();
                Consume(',');
                int high = ConsumeInteger();
                Consume(')');

                // Right now we don't support random. I don't know if we ever
                // want to until a lot of stuff is implemented because it would
                // be a pain to do prediction with. Therefore we'll just take
                // the average of it.
                (int min, int max) = low.MinMax(high);
                return (min + max) / 2;
            }

            int tickAmount = ConsumeSignedInteger();
            if (tickAmount < -1)
                throw MakeException($"No negative tick durations allowed (unless it is -1) on actor '{currentDefinition.Name}'");
            return tickAmount;
        }

        private ActorFrameProperties ConsumeActorFrameKeywordsIfAny()
        {
            ActorFrameProperties properties = new ActorFrameProperties();

            // These can probably come in any order, so we'll need a looping
            // dictionary. Apparently we have to watch out for new lines when
            // dealing with `fast` and `slow`.
            while (true)
            {
                if (!PeekCurrentText(out string value))
                    throw MakeException($"Ran out of tokens when reading actor frame properties on actor '{currentDefinition.Name}'");

                UpperString upperKeyword = value;
                if (!FramePropertyKeywords.Contains(upperKeyword))
                    break;

                // Note: In the future we will need to possibly *not* consume
                // if its 'fast' or 'slow' and on another line since that may
                // be a valid sprite frame. This is why we moved the string
                // consumer into each block because some of them might not do
                // consumption (whenever we get around to implementing that).
                switch (upperKeyword.String)
                {
                case "BRIGHT":
                    ConsumeString();
                    properties.Bright = true;
                    break;
                case "CANRAISE":
                    ConsumeString();
                    properties.CanRaise = true;
                    break;
                case "FAST":
                    // TODO: Make sure it's on the same line (ex: if it's a sprite).
                    ConsumeString();
                    properties.Fast = true;
                    break;
                case "LIGHT":
                    ConsumeString();
                    Consume('(');
                    properties.Light = ConsumeString().AsUpper();
                    Consume(')');
                    break;
                case "NODELAY":
                    ConsumeString();
                    properties.NoDelay = true;
                    break;
                case "OFFSET":
                    ConsumeString();
                    Consume('(');
                    int x = ConsumeSignedInteger();
                    Consume(',');
                    int y = ConsumeSignedInteger();
                    Consume(')');
                    properties.Offset = new Vec2I(x, y);
                    break;
                case "SLOW":
                    ConsumeString();
                    // TODO: Make sure it's on the same line (ex: if it's a sprite).
                    properties.Slow = true;
                    break;
                }
            }

            return properties;
        }

        private void ConsumeActionFunctionArgumentsIfAny()
        {
            if (!ConsumeIf('('))
                return;

            // For now we're just going to consume everything, and will do an
            // implementation later when we can make an AST.
            int rightParenToFind = 1;
            while (rightParenToFind > 0)
            {
                if (ConsumeIf(')'))
                    rightParenToFind--;
                else if (ConsumeIf('('))
                    rightParenToFind++;
                else
                    Consume();
            }
        }

        private Optional<ActorActionFunction> ConsumeActionFunctionIfAny()
        {
            if (!PeekCurrentText(out string text))
                throw MakeException($"Ran out of tokens when reading an actor frame action function on actor '{currentDefinition.Name}'");

            // It is possible that no such action function exists and we would
            // be reading a label or frame.
            UpperString upperText = text;
            if (upperText.StartsWith("A_") || ActionSpecialHelper.Exists(upperText))
            {
                UpperString functionName = ConsumeIdentifier();
                ConsumeActionFunctionArgumentsIfAny();

                return new ActorActionFunction(functionName);
            }

            return null;
        }

        private void ConsumeActorStateFrames(UpperString sprite)
        {
            UpperString frames = ConsumeString();
            int ticks = ConsumeActorFrameTicks();
            ActorFrameProperties properties = ConsumeActorFrameKeywordsIfAny();
            Optional<ActorActionFunction> actionFunction = ConsumeActionFunctionIfAny();

            foreach (char frame in frames)
            {
                if (!IsValidFrameLetter(frame))
                    throw MakeException($"Invalid actor frame letter: {frame} (ascii ordinal {(int)frame})");

                ActorFrame actorFrame = new ActorFrame(sprite + frame, ticks, properties, actionFunction);
                currentDefinition.States.Frames.Add(actorFrame);
                frameIndex++;
            }
        }

        private void ConsumeActorStateElement()
        {
            UpperString text = ConsumeString();

            if (ConsumeIf('.'))
            {
                ReadDottedLabel(text);
                return;
            }

            if (TryReadFlowControl(text, out ActorFlowControl flowControl))
            {
                ApplyStateBranch(flowControl);
                justSeenLabelOrNull = null;
                return;
            }

            if (ConsumeIf(':'))
            {
                TrackNewLabel(text);
                return;
            }

            ConsumeActorStateFrames(text);
            justSeenLabelOrNull = null;
        }

        private void ConsumeActorStates()
        {
            frameIndex = currentDefinition.States.Frames.Count;
            justSeenLabelOrNull = null;

            Consume('{');
            InvokeUntilAndConsume('}', ConsumeActorStateElement);
        }
    }
}

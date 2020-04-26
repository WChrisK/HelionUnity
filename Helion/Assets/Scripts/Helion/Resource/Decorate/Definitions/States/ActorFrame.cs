using Helion.Resource.Textures.Sprites;
using Helion.Util;

namespace Helion.Resource.Decorate.Definitions.States
{
    /// <summary>
    /// A discrete frame an actor will be in. Contains game logic actions and
    /// rendering information.
    /// </summary>
    public class ActorFrame
    {
        /// <summary>
        /// An indicator used to tell whether the next state offset has been
        /// set or not.
        /// </summary>
        public const int OffsetNotSet = int.MinValue;

        /// <summary>
        /// A constant value used to indicate an infinite frame when a Stop
        /// label is reached.
        /// </summary>
        public const int InfiniteFrame = -1;

        /// <summary>
        /// The absolute frame index in the list of frames.
        /// </summary>
        /// <remarks>
        /// Primarily used for determining the offset to jump to when dealing
        /// with two frame indices (where one of them is the current one). This
        /// in particular is used when <see cref="NeedsToSetStateOffset"/> is
        /// checked for setting <see cref="NextStateOffset"/>.
        /// </remarks>
        public readonly int FrameIndex;

        /// <summary>
        /// The sprite name to use. This is always five characters (ex: PLAYA).
        /// </summary>
        public readonly UpperString Sprite;

        /// <summary>
        /// How many ticks this frame is.
        /// </summary>
        public readonly int Ticks;

        /// <summary>
        /// The properties of this frame (ex: brightness, offsets, etc).
        /// </summary>
        public readonly ActorFrameProperties Properties;

        /// <summary>
        /// An action function to be called, if any.
        /// </summary>
        public readonly Optional<ActorActionFunction> ActionFunction;

        /// <summary>
        /// The flow control to the next frame.
        /// </summary>
        public ActorFlowControl FlowControl;

        /// <summary>
        /// A direct offset jump we can do from this position. When the state
        /// has been ticked enough and needs to advance, the offset can have
        /// this number added to it to go to the proper next state.
        /// </summary>
        /// <remarks>
        /// This is to be calculated at the end of parsing an actor, since the
        /// states cannot all be known beforehand due to a single forward pass
        /// of the parser. This index a relative offset, so if we want to go
        /// backwards by one frame then we use -1.
        /// </remarks>
        public int NextStateOffset;

        /// <summary>
        /// The sprite rotations for rendering with.
        /// </summary>
        /// <remarks>
        /// This is to be set at the very end once all the data has been
        /// loaded.
        /// </remarks>
        public SpriteRotations SpriteRotations = SpriteManager.NullRotations;

        /// <summary>
        /// True if this has not been set due to it being a new frame, or false
        /// if it has been post-processed and is ready to go.
        /// </summary>
        public bool NeedsToSetStateOffset => NextStateOffset == int.MinValue;

        /// <summary>
        /// True if this is a frame that is to loop forever, false if not.
        /// </summary>
        public bool IsInfiniteStopFrame => FlowControl.FlowType == ActorStateBranch.Stop &&
                                           Ticks == InfiniteFrame;

        public ActorFrame(int frameIndex, UpperString sprite, int ticks, ActorFrameProperties properties,
            Optional<ActorActionFunction> actionFunction, ActorFlowControl flowControl = null,
            int nextStateOffset = OffsetNotSet)
        {
            FrameIndex = frameIndex;
            Sprite = sprite;
            Ticks = ticks;
            Properties = properties;
            ActionFunction = actionFunction;
            FlowControl = flowControl ?? new ActorFlowControl();
            NextStateOffset = nextStateOffset;
        }

        public ActorFrame(ActorFrame other)
        {
            FrameIndex = other.FrameIndex;
            Sprite = other.Sprite;
            Ticks = other.Ticks;
            Properties = new ActorFrameProperties(other.Properties);
            ActionFunction = other.ActionFunction.Map(af => new ActorActionFunction(af));
            FlowControl = new ActorFlowControl(other.FlowControl);
            NextStateOffset = other.NextStateOffset;
        }

        public override string ToString() => $"[{FrameIndex}] {Sprite} {Ticks} {FlowControl} {ActionFunction}";
    }
}

using System.Collections.Generic;
using Helion.Core.Util;
using MoreLinq;

namespace Helion.Core.Resource.Decorate.Definitions.States
{
    /// <summary>
    /// A collector of all the label offsets for an actor.
    /// </summary>
    public class ActorStateLabels
    {
        private readonly Dictionary<UpperString, Dictionary<UpperString, int>> parentOffsets = new Dictionary<UpperString, Dictionary<UpperString, int>>();
        private readonly Dictionary<UpperString, int> superOffsets = new Dictionary<UpperString, int>();
        private readonly Dictionary<UpperString, int> labels = new Dictionary<UpperString, int>();

        /// <summary>
        /// Creates a label collection with no labels.
        /// </summary>
        public ActorStateLabels()
        {
        }

        /// <summary>
        /// Creates a label collection by extending the parent labels. This
        /// will copy all of the parent fields, copy the immediate parent into
        /// the super labels, and copy all of the labels.
        /// </summary>
        /// <param name="other">The labels to copy and change to act as a child
        /// of the parent data.</param>
        /// <param name="parentName">The name of the parent.</param>
        public ActorStateLabels(ActorStateLabels other, UpperString parentName)
        {
            // Reminder that we can do shallow copying because both the key and
            // value pairs are immutable. Also order matters for some below.
            other.parentOffsets.ForEach(pair => parentOffsets[pair.Key] = new Dictionary<UpperString, int>(pair.Value));
            parentOffsets[parentName] = new Dictionary<UpperString, int>(other.superOffsets);
            superOffsets = new Dictionary<UpperString, int>(other.labels);
            labels = new Dictionary<UpperString, int>(other.labels);
        }

        /// <summary>
        /// Gets the offset for a label on this actor.
        /// </summary>
        /// <param name="label">The label name.</param>
        /// <returns>The label index, or null if no label with the name exists.
        /// </returns>
        public int? this[UpperString label]
        {
            get
            {
                if (labels.TryGetValue(label, out int number))
                    return number;
                return null;
            }
        }

        /// <summary>
        /// Gets a label for a parent name.
        /// </summary>
        /// <param name="parent">The parent name.</param>
        /// <param name="label">The label name.</param>
        /// <returns>The label index, or null if no label with the name exists.
        /// </returns>
        public int? Parent(UpperString parent, UpperString label)
        {
            if (parentOffsets.TryGetValue(parent, out Dictionary<UpperString, int> parentLabels))
                if (parentLabels.TryGetValue(label, out int number))
                    return number;
            return null;
        }

        /// <summary>
        /// Gets a label for the immediate parent.
        /// </summary>
        /// <param name="label">The label name.</param>
        /// <returns>The label index, or null if no label with the name exists.
        /// </returns>
        public int? Super(UpperString label)
        {
            if (superOffsets.TryGetValue(label, out int number))
                return number;
            return null;
        }

        /// <summary>
        /// Adds a new label. Should not be used with super or parent labels,
        /// as this is intended only for standard labels. Will overwrite any
        /// old label offset with the new one.
        /// </summary>
        /// <param name="label">The label name.</param>
        /// <param name="offset">The label offset. Should be non-negative.
        /// </param>
        public void Add(UpperString label, int offset)
        {
            labels[label] = offset;
        }

        /// <summary>
        /// Checks if a label with the provided name exists. This does not work
        /// for super or parent labels.
        /// </summary>
        /// <param name="label">The label name.</param>
        /// <returns>True if so, false if not.</returns>
        public bool Contains(UpperString label) => labels.ContainsKey(label);

        /// <summary>
        /// Deletes a label. This does not affect parent or super labels.
        /// </summary>
        /// <param name="label">The label name.</param>
        /// <returns>True if one was deleted, false if no such label existed.
        /// </returns>
        public bool Remove(UpperString label) => labels.Remove(label);
    }
}

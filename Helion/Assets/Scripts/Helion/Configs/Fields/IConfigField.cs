namespace Helion.Configs.Fields
{
    /// <summary>
    /// Represents a field that can be mutated or queried.
    /// </summary>
    public interface IConfigField
    {
        /// <summary>
        /// The full period separated name.
        /// </summary>
        /// <remarks>
        /// Ex: "mouse.yaw" for the mouse object with the yaw field.
        /// </remarks>
        string FullName { get; }

        /// <summary>
        /// Gets the text representation of the value for this field.
        /// </summary>
        string TextValue { get; }

        /// <summary>
        /// Sets the field to the value by casting it, or assuming a default
        /// if the casting cannot be done.
        /// </summary>
        /// <param name="text">The value to set by parsing it.</param>
        /// <returns>True if it successfully set due to the text being a well
        /// formed value of the type, or false if parsing failed and nothing
        /// was set.</returns>
        bool SetValue(string text);
    }
}

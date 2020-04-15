using System.Reflection;

namespace Helion.Core.Configs.Fields
{
    /// <summary>
    /// A path to a field that is a string.
    /// </summary>
    public class StringConfigField : IConfigField
    {
        public string FullName { get; }
        public string TextValue => Value;
        private readonly object obj;
        private readonly FieldInfo fieldInfo;

        public string Value => (string)fieldInfo.GetValue(obj);

        public StringConfigField(object obj, FieldInfo fieldInfo, string path)
        {
            this.FullName = path;
            this.obj = obj;
            this.fieldInfo = fieldInfo;
        }

        public bool SetValue(string text)
        {
            if (text.Length > 2048)
                return false;

            // If it's quoted, toss out the wrapping quotes.
            if (text.Length >= 2 && text.StartsWith("\"") && text.EndsWith("\""))
                fieldInfo.SetValue(obj, text.Substring(1, text.Length - 2));
            else
                fieldInfo.SetValue(obj, text);
            return true;
        }
    }
}

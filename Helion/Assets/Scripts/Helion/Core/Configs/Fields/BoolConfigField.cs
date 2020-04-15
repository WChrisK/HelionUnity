using System.Reflection;

namespace Helion.Core.Configs.Fields
{
    /// <summary>
    /// A path to a field that is a boolean.
    /// </summary>
    public class BoolConfigField : IConfigField
    {
        public string FullName { get; }
        public string TextValue => Value.ToString().ToLower();
        private readonly object obj;
        private readonly FieldInfo fieldInfo;

        public bool Value => (bool)fieldInfo.GetValue(obj);

        public BoolConfigField(object obj, FieldInfo fieldInfo, string path)
        {
            this.FullName = path;
            this.obj = obj;
            this.fieldInfo = fieldInfo;
        }

        public bool SetValue(string text)
        {
            switch (text.ToLower())
            {
            case "true":
                fieldInfo.SetValue(obj, true);
                return true;
            case "false":
                fieldInfo.SetValue(obj, false);
                return true;
            default:
                return false;
            }
        }
    }
}

using System.Reflection;

namespace Helion.Core.Configs.Fields
{
    /// <summary>
    /// A path to a field that is an integer.
    /// </summary>
    public class IntConfigField : IConfigField
    {
        public string FullName { get; }
        public string TextValue => Value.ToString();
        private readonly object obj;
        private readonly FieldInfo fieldInfo;

        public int Value => (int)fieldInfo.GetValue(obj);

        public IntConfigField(object obj, FieldInfo fieldInfo, string path)
        {
            this.FullName = path;
            this.obj = obj;
            this.fieldInfo = fieldInfo;
        }

        public bool SetValue(string text)
        {
            if (int.TryParse(text, out int result))
            {
                fieldInfo.SetValue(obj, result);
                return true;
            }

            return false;
        }
    }
}

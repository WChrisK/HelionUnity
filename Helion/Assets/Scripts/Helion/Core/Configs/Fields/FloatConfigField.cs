using System.Reflection;

namespace Helion.Core.Configs.Fields
{
    /// <summary>
    /// A path to a field that is a floating point number.
    /// </summary>
    public class FloatConfigField : IConfigField
    {
        public string FullName { get; }
        public string TextValue => Value.ToString();
        private readonly object obj;
        private readonly FieldInfo fieldInfo;

        public float Value => (float)fieldInfo.GetValue(obj);

        public FloatConfigField(object obj, FieldInfo fieldInfo, string path)
        {
            this.FullName = path;
            this.obj = obj;
            this.fieldInfo = fieldInfo;
        }

        public bool SetValue(string text)
        {
            if (float.TryParse(text, out float result))
            {
                if (float.IsInfinity(result) || float.IsNaN(result))
                    return false;

                fieldInfo.SetValue(obj, result);
                return true;
            }

            return false;
        }
    }
}

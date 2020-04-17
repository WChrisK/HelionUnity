using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Helion.Core.Configs.Components;
using Helion.Core.Configs.Fields;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;
using UnityEngine;

namespace Helion.Core.Configs
{
    using ConfigFieldFunc = Func<object, FieldInfo, string, IConfigField>;

    /// <summary>
    /// Holds all the data from the config file.
    /// </summary>
    public class Config
    {
        private static readonly Dictionary<Type, ConfigFieldFunc> PrimitiveToActionMap = new Dictionary<Type, ConfigFieldFunc>
        {
            [typeof(bool)] = (obj, fieldInfo, path) => new BoolConfigField(obj, fieldInfo, path),
            [typeof(int)] = (obj, fieldInfo, path) => new IntConfigField(obj, fieldInfo, path),
            [typeof(float)] = (obj, fieldInfo, path) => new FloatConfigField(obj, fieldInfo, path),
            [typeof(string)] = (obj, fieldInfo, path) => new StringConfigField(obj, fieldInfo, path)
        };

        public readonly ConfigDebug Debug = new ConfigDebug();
        public readonly ConfigMouse Mouse = new ConfigMouse();
        public readonly ConfigResources Resources = new ConfigResources();

        /// <summary>
        /// Tries to read a config file at the path provided.
        /// </summary>
        /// <param name="path">The path to the file.</param>
        /// <returns>The config, or null if it cannot be read due to IO errors
        /// or if the text is malformed json.</returns>
        public static Optional<Config> FromFile(string path)
        {
            try
            {
                return FromText(File.ReadAllText(path));
            }
            catch
            {
                return Optional<Config>.Empty();
            }
        }

        /// <summary>
        /// Reads a config file from the text provided.
        /// </summary>
        /// <param name="text">The text to read.</param>
        /// <returns>The config from the text, or null if the text is not valid
        /// json data.</returns>
        public static Optional<Config> FromText(string text)
        {
            try
            {
                Config config = new Config();
                JsonUtility.FromJsonOverwrite(text, config);
                return config;
            }
            catch
            {
                return Optional<Config>.Empty();
            }
        }

        /// <summary>
        /// Saves the json file to the path provided.
        /// </summary>
        /// <param name="path">The path to save to, or the default path if
        /// null.</param>
        /// <returns>True on success, false on failure.</returns>
        public bool Save(string path = null)
        {
            path = path ?? Constants.ConfigName;

            try
            {
                string text = JsonUtility.ToJson(this, true);
                File.WriteAllText(path, text);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<IConfigField> GetConfigFields()
        {
            List<IConfigField> fields = new List<IConfigField>();
            GetConfigFieldsRecursively("", this, fields);
            return fields;
        }

        private void GetConfigFieldsRecursively(string path, object obj, List<IConfigField> fields)
        {
            foreach (FieldInfo fieldInfo in obj.GetType().GetFields())
            {
                if (!fieldInfo.IsPublic)
                    continue;

                string extendedPath = CreateOrExtendPath(path, fieldInfo.Name);

                if (fieldInfo.FieldType.IsEnum)
                {
                    // TODO
                    continue;
                }

                if (PrimitiveToActionMap.TryGetValue(fieldInfo.FieldType, out Func<object, FieldInfo, string, IConfigField> func))
                {
                    IConfigField configField = func(obj, fieldInfo, extendedPath);
                    fields.Add(configField);
                }
                else
                {
                    object fieldObject = fieldInfo.GetValue(obj);
                    GetConfigFieldsRecursively(extendedPath, fieldObject, fields);
                }
            }
        }

        private static string CreateOrExtendPath(string path, string className)
        {
            return path.Empty() ? className.ToLower() : $"{path}.{className.ToLower()}";
        }
    }
}

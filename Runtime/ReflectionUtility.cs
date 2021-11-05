using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace UniSpreadsheets
{
    public static class ReflectionUtility
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static IReadOnlyDictionary<string, FieldInfo> GetFieldsWithOverride(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var result = new Dictionary<string, FieldInfo>();

            var fields = type.GetFields(BINDING_FLAGS);

            foreach (var fieldInfo in fields)
            {
                var spreadsheetAttribute = fieldInfo.GetCustomAttribute<SpreadsheetAttributeAttribute>();
                var attributeKey = spreadsheetAttribute?.Key ?? fieldInfo.Name;

                if (result.ContainsKey(attributeKey))
                {
                    throw new ArgumentException($"[UniSpreadsheets] {type} has duplicate member name: \'{attributeKey}\'.");
                }

                result[attributeKey] = fieldInfo;
            }

            return result;
        }

        public static void InjectData(DataSet source, UnityEngine.Object target)
        {
            if (source == default) throw new ArgumentNullException(nameof(source));
            if (target == default) throw new ArgumentNullException(nameof(target));

            var type = target.GetType();
            var fields = type.GetFields(BINDING_FLAGS);

            foreach (var field in fields)
            {
                var rangeAttribute = field.GetCustomAttribute<SpreadsheetRangeAttribute>();
                if (rangeAttribute == default) continue;

                if (field.FieldType.IsArray == false) continue;

                if (source.Tables.Contains(rangeAttribute.Name))
                {
                    var data = ExcelSpreadsheetUtility.Deserialize(source.Tables[rangeAttribute.Name], field.FieldType.GetElementType());
                    field.SetValue(target, data);
                }
#if UNITY_EDITOR
                else
                {
                    UnityEngine.Debug.LogWarning($"[UniSpreadsheets] Range \'{rangeAttribute.Name}\' doesn't exists in {source.DataSetName}.");
                }
#endif
            }
        }
    }
}

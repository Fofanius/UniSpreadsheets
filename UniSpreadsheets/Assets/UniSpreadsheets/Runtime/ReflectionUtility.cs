using System;
using System.Collections.Generic;
using System.Reflection;

namespace UniSpreadsheets
{
    internal static class ReflectionUtility
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
                    throw new ArgumentException($"{type} has duplicate member name: \'{attributeKey}\'.");
                }

                result[attributeKey] = fieldInfo;
            }

            return result;
        }
    }
}

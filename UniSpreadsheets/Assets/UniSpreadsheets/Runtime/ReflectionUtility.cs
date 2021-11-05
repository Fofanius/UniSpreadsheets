using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace UniSpreadsheets
{
    public static class ReflectionUtility
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        /// <summary>
        /// Находит все поля помеченные атрибутом <see cref="SpreadsheetAttributeAttribute"/>.
        /// </summary>
        /// <returns>Attribute -> FieldInfo</returns>
        public static IReadOnlyDictionary<string, FieldInfo> GetSpreadsheetAttributeFields(Type type)
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

        /// <summary>
        /// Находит все поля объекта помеченные атрибутом <see cref="SpreadsheetRangeAttribute"/>. 
        /// </summary>
        /// <returns>Range (лист) -> FieldInfo</returns>
        public static IReadOnlyDictionary<string, FieldInfo> GetSpreadsheetRangeFields(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var result = new Dictionary<string, FieldInfo>();
            var fields = type.GetFields(BINDING_FLAGS);

            foreach (var fieldInfo in fields)
            {
                var spreadsheetRange = fieldInfo.GetCustomAttribute<SpreadsheetRangeAttribute>();
                if (spreadsheetRange == default) continue;

                if (result.ContainsKey(spreadsheetRange.Name))
                {
                    throw new ArgumentException($"[UniSpreadsheets] {type} has duplicate member name: \'{spreadsheetRange.Name}\'.");
                }

                result[spreadsheetRange.Name] = fieldInfo;
            }

            return result;
        }
    }
}

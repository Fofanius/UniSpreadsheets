using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace UniSpreadsheets
{
    public static class ReflectionUtility
    {
        private const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static IReadOnlyDictionary<string, MemberInfo> GetSpreadsheetAttributeMembers(Type type)
        {
            if (type == null) throw new ArgumentNullException(nameof(type));

            var result = new Dictionary<string, MemberInfo>();

            var fields = type.GetFields(BINDING_FLAGS);
            IterateMembers(fields);

            var properties = type.GetProperties(BINDING_FLAGS);
            IterateMembers(properties);

            void IterateMembers<T>(IEnumerable<T> members) where T : MemberInfo
            {
                foreach (var memberInfo in members)
                {
                    var spreadsheetAttribute = memberInfo.GetCustomAttribute<SpreadsheetAttributeAttribute>();
                    var attributeKey = spreadsheetAttribute?.Key ?? memberInfo.Name;

                    if (result.ContainsKey(attributeKey))
                    {
                        throw new ArgumentException($"[UniSpreadsheets] {type} has duplicate attribute target with name: \'{attributeKey}\'.");
                    }

                    result[attributeKey] = memberInfo;
                }
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

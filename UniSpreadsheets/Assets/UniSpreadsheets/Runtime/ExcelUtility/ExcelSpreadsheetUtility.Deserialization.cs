using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace UniSpreadsheets
{
    public static partial class ExcelSpreadsheetUtility
    {
        public static T[] Deserialize<T>(DataTable table, uint headersRowIndex = 0, Func<string, string> tableAttributeNameProcessor = null)
        {
            return (T[])Deserialize(table, typeof(T), headersRowIndex, tableAttributeNameProcessor);
        }

        public static Array Deserialize(DataTable table, Type targetType, uint headersRowIndex = 0, Func<string, string> tableAttributeNameProcessor = null)
        {
            if (table == default) throw new ArgumentNullException(nameof(table));
            if (targetType == default) throw new ArgumentNullException(nameof(targetType));

            var rows = table.Select();
            if (rows.Length == default) throw new Exception($"Range '{table.TableName}' is empty.");

            var resultArrayLenght = rows.Length - 1 - headersRowIndex;
            if (resultArrayLenght < 0) throw new ArgumentException($"Header row position is invalid for {rows.Length} rows. Headers row index: {headersRowIndex}, rows to read: {resultArrayLenght}.");

            tableAttributeNameProcessor ??= x => x;

            var attributeToColumnIndexMap = new Dictionary<string, int>();

            for (var i = 0; i < rows[headersRowIndex].ItemArray.Length; i++)
            {
                var headerValue = rows[headersRowIndex].ItemArray[i];

                var key = headerValue?.ToString() ?? string.Empty;
                key = tableAttributeNameProcessor(key);
                if (string.IsNullOrWhiteSpace(key)) continue;

                attributeToColumnIndexMap.TryAdd(key, i);
            }

            var resultArray = Array.CreateInstance(targetType, resultArrayLenght);

            var members = ReflectionUtility.GetSpreadsheetAttributeMembers(targetType);

            for (var i = 0; i < resultArrayLenght; i++)
            {
                var instance = Activator.CreateInstance(targetType);
                var row = rows[i + headersRowIndex + 1];

                foreach (var attributeToColumnPair in attributeToColumnIndexMap)
                {
                    if (members.TryGetValue(attributeToColumnPair.Key, out var memberInfo))
                    {
                        SetMemberInfo(memberInfo, instance, row.ItemArray[attributeToColumnPair.Value]);
                    }
                }

                resultArray.SetValue(instance, i);
            }

            return resultArray;
        }

        private static void SetMemberInfo(MemberInfo memberInfo, object instance, object value)
        {
            switch (memberInfo)
            {
                case FieldInfo fieldInfo:
                    SetFieldValue(fieldInfo, instance, value);
                    break;
                case PropertyInfo propertyInfo:
                    SetPropertyValue(propertyInfo, instance, value);
                    break;
                default:
                    throw new NotImplementedException($"There is no support for {memberInfo}!");
            }
        }

        private static void SetFieldValue(FieldInfo field, object instance, object tableValue)
        {
            if (TryParseValue(tableValue, field.FieldType, out var value))
            {
                field.SetValue(instance, value);
            }
        }

        private static void SetPropertyValue(PropertyInfo property, object instance, object tableValue)
        {
            if (!property.CanWrite)
            {
                var backingField = instance.GetType().GetField($"<{property.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                if (backingField != null)
                {
                    SetFieldValue(backingField, instance, tableValue);
                }

                return;
            }

            if (TryParseValue(tableValue, property.PropertyType, out var value))
            {
                property.SetValue(instance, value);
            }
        }

        // TODO: refactor this
        private static bool TryParseValue(object tableValue, Type targetType, out object result)
        {
            result = default;
            var fieldStringValue = tableValue.ToString();

            // строковый тип данных
            if (targetType == typeof(string))
            {
                result = fieldStringValue;
                return true;
            }
            // булевый тип данных (конвертируется из int)
            else if (targetType == typeof(bool))
            {
                if (int.TryParse(fieldStringValue, out var output))
                {
                    result = output != 0;
                }
                else
                {
                    result = string.Equals(bool.TrueString, fieldStringValue, StringComparison.OrdinalIgnoreCase);
                }

                return true;
            }
            // дроброе число
            else if (targetType == typeof(float))
            {
                var input = fieldStringValue.ChangeDecimalSeparator();
                result = float.TryParse(input, out var output) ? output : 0f;
                return true;
            }
            // дроброе число
            else if (targetType == typeof(double))
            {
                var input = fieldStringValue.ChangeDecimalSeparator();
                result = double.TryParse(input, out var output) ? output : 0d;
                return true;
            }
            // целое число
            else if (targetType == typeof(int))
            {
                result = int.TryParse(fieldStringValue, out var output) ? output : 0;
                return true;
            }
            // перечесление
            else if (targetType.IsSubclassOf(typeof(Enum)))
            {
                if (Enum.IsDefined(targetType, fieldStringValue))
                {
                    result = Enum.Parse(targetType, fieldStringValue);
                    return true;
                }
            }

            return false;
        }

        private static string ChangeDecimalSeparator(this string input)
        {
            var separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            return input.Replace(".", separator).Replace(",", separator);
        }
    }
}

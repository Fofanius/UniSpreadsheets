using System;
using System.Data;
using System.Linq;
using System.Reflection;

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

            var attributes = rows[headersRowIndex].ItemArray.Where(x => x is string).Select(x => tableAttributeNameProcessor((string)x)).ToList();
            var resultArray = Array.CreateInstance(targetType, resultArrayLenght);

            var fields = ReflectionUtility.GetSpreadsheetAttributeFields(targetType);

            for (int i = 0; i < resultArrayLenght; i++)
            {
                var instance = Activator.CreateInstance(targetType);

                for (int j = 0; j < attributes.Count; j++)
                {
                    if (fields.TryGetValue(attributes[j], out var fieldInfo))
                    {
                        SetFieldValue(fieldInfo, instance, rows[i + headersRowIndex + 1].ItemArray[j]);
                    }
                }

                resultArray.SetValue(instance, i);
            }

            return resultArray;
        }

        // TODO: refactor this
        private static void SetFieldValue(FieldInfo field, object instance, object value)
        {
            var fieldStringValue = value.ToString();

            // строковый тип данных
            if (field.FieldType == typeof(string))
            {
                field.SetValue(instance, fieldStringValue);
            }
            // булевый тип данных (конвертируется из int)
            else if (field.FieldType == typeof(bool))
            {
                if (int.TryParse(fieldStringValue, out var output))
                {
                    field.SetValue(instance, output != 0);
                }
                else
                {
                    field.SetValue(instance, string.Equals(bool.TrueString, fieldStringValue, StringComparison.OrdinalIgnoreCase));
                }
            }
            // дроброе число
            else if (field.FieldType == typeof(float))
            {
                var input = fieldStringValue.ChangeDecimalSeparator();
                field.SetValue(instance, float.TryParse(input, out var output) ? output : 0f);
            }
            // дроброе число
            else if (field.FieldType == typeof(double))
            {
                var input = fieldStringValue.ChangeDecimalSeparator();
                field.SetValue(instance, double.TryParse(input, out var output) ? output : 0d);
            }
            // целое число
            else if (field.FieldType == typeof(int))
            {
                field.SetValue(instance, int.TryParse(fieldStringValue, out var output) ? output : 0);
            }
            // перечесление
            else if (field.FieldType.IsSubclassOf(typeof(Enum)))
            {
                if (Enum.IsDefined(field.FieldType, fieldStringValue))
                {
                    field.SetValue(instance, Enum.Parse(field.FieldType, fieldStringValue));
                }
            }
        }

        private static string ChangeDecimalSeparator(this string input)
        {
            var separator = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            return input.Replace(".", separator).Replace(",", separator);
        }
    }
}

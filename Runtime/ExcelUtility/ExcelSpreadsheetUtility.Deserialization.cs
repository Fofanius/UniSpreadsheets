using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace UniSpreadsheets
{
    public static partial class ExcelSpreadsheetUtility
    {
        public static Array Deserialize(DataTable table, Type targetType)
        {
            if (table == default) throw new ArgumentNullException(nameof(table));
            if (targetType == default) throw new ArgumentNullException(nameof(targetType));

            var rows = table.Select();
            if (rows.Length == default) throw new Exception($"Range '{table.TableName}' is empty.");

            var attributes = rows[0].ItemArray.Where(x => x is string).Select(x => ((string) x).Split(' ')[0]).ToList();
            var resultArray = Activator.CreateInstance(targetType.MakeArrayType(), rows.Length - 1) as Array;

            var fields = ReflectionUtility.GetFieldsWithOverride(targetType);

            for (int i = 1; i < rows.Length; i++)
            {
                var instance = Activator.CreateInstance(targetType);

                for (int j = 0; j < attributes.Count; j++)
                {
                    if (fields.TryGetValue(attributes[j], out var fieldInfo))
                    {
                        SetFieldValue(fieldInfo, instance, rows[i].ItemArray[j]);
                    }
                }

                resultArray.SetValue(instance, i - 1);
            }

            return resultArray;
        }

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
                if (int.TryParse(fieldStringValue, out int output))
                {
                    field.SetValue(instance, output != 0);
                }
                else
                {
                    field.SetValue(instance, false);
                }
            }
            // дроброе число
            else if (field.FieldType == typeof(float))
            {
                var input = fieldStringValue.ChangeDecimalSeparator();

                if (float.TryParse(input, out float output))
                {
                    field.SetValue(instance, output);
                }
                else
                {
                    field.SetValue(instance, 0);
                }
            }
            // дроброе число
            else if (field.FieldType == typeof(double))
            {
                var input = fieldStringValue.ChangeDecimalSeparator();

                if (double.TryParse(input, out double output))
                {
                    field.SetValue(instance, output);
                }
                else
                {
                    field.SetValue(instance, 0);
                }
            }
            // целое число
            else if (field.FieldType == typeof(int))
            {
                if (int.TryParse(fieldStringValue, out int output))
                {
                    field.SetValue(instance, output);
                }
                else
                {
                    field.SetValue(instance, 0);
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

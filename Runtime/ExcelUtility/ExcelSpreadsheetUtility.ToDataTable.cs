using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace UniSpreadsheets
{
    public static partial class ExcelSpreadsheetUtility
    {
        public static DataTable ConvertToDataTable<T>(T[] array, string tableName)
        {
            return ConvertToDataTable((Array)array, tableName);
        }

        public static DataTable ConvertToDataTable(Array array, string tableName)
        {
            if (tableName == null) throw new ArgumentNullException(nameof(tableName));
            if (array == null) throw new ArgumentNullException(nameof(array));

            var table = new DataTable(tableName);

            var type = array.GetType().GetElementType();
            var spreadsheetMembers = ReflectionUtility.GetSpreadsheetAttributeMembers(type);

            var columns = spreadsheetMembers.Keys.ToArray();

            foreach (var field in columns)
            {
                table.Columns.Add(field);
            }

            for (int i = 0; i < array.Length; i++)
            {
                var element = array.GetValue(i);

                if (element == null)
                {
                    table.Rows.Add();
                }
                else
                {
                    var values = columns.Select(x => spreadsheetMembers[x] switch
                    {
                        FieldInfo fieldInfo => fieldInfo.GetValue(element),
                        PropertyInfo propertyInfo => propertyInfo.GetValue(element),

                        _ => default,
                    }).ToArray();
                    table.Rows.Add(values);
                }
            }

            return table;
        }
    }
}

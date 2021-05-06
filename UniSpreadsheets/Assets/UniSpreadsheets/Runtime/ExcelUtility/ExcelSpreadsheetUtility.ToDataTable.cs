using System;
using System.Data;
using System.Linq;

namespace UniSpreadsheets
{
    public static partial class ExcelSpreadsheetUtility
    {
        public static DataTable ConvertToDataTAble<T>(T[] array, string tableName)
        {
            return ConvertToDataTable(array, tableName);
        }
        
        public static DataTable ConvertToDataTable(Array array, string tableName)
        {
            if (tableName == null) throw new ArgumentNullException(nameof(tableName));
            if (array == null) throw new ArgumentNullException(nameof(array));

            var table = new DataTable(tableName);

            var type = array.GetType().GetElementType();
            var spreadsheetFields = ReflectionUtility.GetFieldsWithOverride(type);

            var fields = spreadsheetFields.Keys.ToArray();

            foreach (var field in fields)
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
                    var values = fields.Select(x => spreadsheetFields[x].GetValue(element)).ToArray();
                    table.Rows.Add(values);
                }
            }

            return table;
        }
    }
}

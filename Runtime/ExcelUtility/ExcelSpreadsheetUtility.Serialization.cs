using System;
using System.Data;
using System.Linq;
using System.Text;

namespace UniSpreadsheets
{
    public static partial class ExcelSpreadsheetUtility
    {
        public static string ToCsv(DataTable table, bool useTableColumnNamesAsAttributes = false)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));

            // https://stackoverflow.com/questions/4959722/how-can-i-turn-a-datatable-to-a-csv

            var sb = new StringBuilder();

            if (useTableColumnNamesAsAttributes)
            {
                var columnNames = table.Columns.Cast<DataColumn>().Select(column => column.ColumnName);
                sb.AppendLine(string.Join(",", columnNames));
            }

            foreach (DataRow row in table.Rows)
            {
                var fields = row.ItemArray.Select(field => string.Concat("\"", field.ToString().Replace("\"", "\"\""), "\""));
                sb.AppendLine(string.Join(",", fields));
            }

            return sb.ToString();
        }
    }
}

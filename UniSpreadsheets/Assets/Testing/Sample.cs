using System.Collections;
using System.IO;
using UnityEngine;

namespace UniSpreadsheets.Testing
{
    public class Sample : MonoBehaviour
    {
        [ContextMenu("Read Spreadsheet")]
        private void ReadSpreadsheet()
        {
            using (var fileStream = new FileStream("Assets/Testing/Sample.xlsx", FileMode.Open, FileAccess.Read))
            {
                var dataSet = ExcelSpreadsheetUtility.XlsxStreamToDataSet(fileStream);

                var localization = ExcelSpreadsheetUtility.Deserialize<LocalizationItem>(dataSet.Tables["Localization"]);
                LogCollection(localization);

                var weapons = ExcelSpreadsheetUtility.Deserialize<Weapon>(dataSet.Tables["Weapon"]);
                LogCollection(weapons);
            }
        }

        private void LogCollection(IEnumerable enumerable)
        {
            if (enumerable == null)
            {
                Debug.Log($"Collection is null.");
            }
            else
            {
                foreach (var obj in enumerable)
                {
                    Debug.Log(obj);
                }
            }
        }
    }
}

using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniSpreadsheets.Testing
{
    public class Sample : MonoBehaviour
    {
        [SerializeField] private TextAsset _spreadsheet;
        [SerializeField] private DefaultAsset _archive;

        [ContextMenu("Read Spreadsheet")]
        private void ReadSpreadsheet()
        {
            using (var fileStream = new FileStream("Assets/Testing/Sample.xlsx", FileMode.Open, FileAccess.Read))
            {
                var dataSet = ExcelSpreadsheetUtility.XlsxStreamToDataSet(fileStream);

                var localization = ExcelSpreadsheetUtility.Deserialize(dataSet.Tables["Localization"], typeof(LocalizationItem));
                LogCollection(localization);

                var weapons = ExcelSpreadsheetUtility.Deserialize(dataSet.Tables["Weapon"], typeof(Weapon));
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

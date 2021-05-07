using System.Collections;
using UnityEngine;

namespace UniSpreadsheets.Testing
{
    public class Sample : MonoBehaviour
    {
        [SerializeField] private XlsxAsset _sampleSpreadsheet;

        [ContextMenu("Read Spreadsheet")]
        private void ReadSpreadsheet()
        {
            var dataSet = _sampleSpreadsheet.ToDataSet();

            var localization = ExcelSpreadsheetUtility.Deserialize<LocalizationItem>(dataSet.Tables["Localization"]);
            LogCollection(localization);

            var weapons = ExcelSpreadsheetUtility.Deserialize<Weapon>(dataSet.Tables["Weapon"]);
            LogCollection(weapons);
        }

        private static void LogCollection(IEnumerable enumerable)
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

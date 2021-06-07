using System.IO;
using System.Text;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace UniSpreadsheets.Editor
{
    [ScriptedImporter(1, "xlsx")]
    public class XlsxAssetImporter : ScriptedImporter
    {
        // 94 - is index of 'utf-8' encoding
        [SerializeField] private int _encodingIndex = 94;

        private string GetEncodingSafe()
        {
            var encodings = Encoding.GetEncodings();

            if (_encodingIndex < 0 || _encodingIndex >= encodings.Length)
            {
                return Encoding.UTF8.HeaderName;
            }

            return encodings[_encodingIndex].Name;
        }

        public override void OnImportAsset(AssetImportContext ctx)
        {
            var bytes = File.ReadAllBytes(ctx.assetPath);
            var xlsxAsset = XlsxAsset.Create(bytes, GetEncodingSafe());

            var originFileName = Path.GetFileName(ctx.assetPath);
            ctx.AddObjectToAsset($"{originFileName} - XLSX", xlsxAsset);
            ctx.SetMainObject(xlsxAsset);

            var dataSet = xlsxAsset.ToDataSet();

            for (var i = 0; i < dataSet.Tables.Count; i++)
            {
                var table = dataSet.Tables[i];

                var csvContent = ExcelSpreadsheetUtility.ToCsv(table);
                var csv = new TextAsset(csvContent)
                {
                    name = table.TableName
                };

                ctx.AddObjectToAsset($"{originFileName} - {table.TableName}", csv);
            }
        }
    }
}

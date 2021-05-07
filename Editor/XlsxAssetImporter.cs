using System.IO;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace UniSpreadsheets.Editor
{
    [ScriptedImporter(1, "xlsx")]
    public class XlsxAssetImporter : ScriptedImporter
    {
        public override void OnImportAsset(AssetImportContext ctx)
        {
            var bytes = File.ReadAllBytes(ctx.assetPath);
            var xlsxAsset = XlsxAsset.Create(bytes);

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

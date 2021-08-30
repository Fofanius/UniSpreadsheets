using System;
using System.IO;
using System.Text;
using UnityEditor;
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
            var fileInfo = new FileInfo(ctx.assetPath);

            if (fileInfo.Name.StartsWith("~$")) return;

            if (FileUtility.IsFileLocked(fileInfo))
            {
                EditorUtility.DisplayProgressBar("UniSpreadsheets", $"Wait for file \'{fileInfo.FullName}\'.", 0f);
                FileUtility.WaitForFile(fileInfo);
            }

            try
            {
                EditorUtility.DisplayProgressBar("UniSpreadsheets", $"Reading file \'{fileInfo.FullName}\'.", 0f);

                var bytes = File.ReadAllBytes(fileInfo.FullName);
                var xlsxAsset = XlsxAsset.Create(bytes, GetEncodingSafe());

                var originFileName = Path.GetFileName(ctx.assetPath);
                ctx.AddObjectToAsset($"{originFileName} - XLSX", xlsxAsset);
                ctx.SetMainObject(xlsxAsset);

                var dataSet = xlsxAsset.ToDataSet();
                var count = dataSet.Tables.Count;

                for (var i = 0; i < count; i++)
                {
                    var table = dataSet.Tables[i];

                    EditorUtility.DisplayProgressBar("UniSpreadsheets", $"Processing \'{table.TableName}\': \'{fileInfo.FullName}\'.", (i + 1) / (float) count);

                    var csvContent = ExcelSpreadsheetUtility.ToCsv(table);
                    var csv = new TextAsset(csvContent)
                    {
                        name = table.TableName
                    };

                    ctx.AddObjectToAsset($"{originFileName} - {table.TableName}", csv);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UniSpreadsheets] Error was handled while processing \'{ctx.assetPath}\'.");
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}

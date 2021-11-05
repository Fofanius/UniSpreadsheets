using System;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UniSpreadsheets.Editor
{
    [ScriptedImporter(1, "xlsx")]
    public class XlsxAssetImporter : ScriptedImporter
    {
        // 94 - is index of 'utf-8' encoding
        [SerializeField] private int _encodingIndex = 94;
        [SerializeField] private Object[] _spreadsheetDataReceivers;

        private string GetEncodingSafe()
        {
            var encodings = Encoding.GetEncodings();

            if (_encodingIndex < 0 || _encodingIndex >= encodings.Length)
            {
                return Encoding.UTF8.HeaderName;
            }

            return encodings[_encodingIndex].Name;
        }

        public override void OnImportAsset(AssetImportContext context)
        {
            var fileInfo = new FileInfo(context.assetPath);

            if (fileInfo.Name.StartsWith("~$")) return;

            if (FileUtility.IsFileLocked(fileInfo))
            {
                EditorUtility.DisplayProgressBar("UniSpreadsheets", $"Wait for file \'{fileInfo.FullName}\'.", 0f);
                FileUtility.WaitForFile(fileInfo);
            }

            try
            {
                EditorUtility.DisplayProgressBar("UniSpreadsheets", $"Reading file \'{context.assetPath}\'.", 0f);

                var bytes = File.ReadAllBytes(context.assetPath);
                var xlsxAsset = XlsxAsset.Create(bytes, GetEncodingSafe());

                var originFileName = Path.GetFileName(context.assetPath);
                context.AddObjectToAsset($"{originFileName} - XLSX", xlsxAsset);
                context.SetMainObject(xlsxAsset);

                var dataSet = xlsxAsset.ToDataSet();
                var count = dataSet.Tables.Count;

                for (var i = 0; i < count; i++)
                {
                    var table = dataSet.Tables[i];

                    EditorUtility.DisplayProgressBar("UniSpreadsheets", $"Processing \'{table.TableName}\': \'{context.assetPath}\'.", (i + 1) / (float)count);

                    var csvContent = ExcelSpreadsheetUtility.ToCsv(table);
                    var csv = new TextAsset(csvContent)
                    {
                        name = table.TableName
                    };

                    context.AddObjectToAsset($"{originFileName} - {table.TableName}", csv);
                }

                for (int i = 0; i < _spreadsheetDataReceivers.Length; i++)
                {
                    var injectionTarget = _spreadsheetDataReceivers[i];
                    if (injectionTarget == false) continue;

                    EditorUtility.DisplayProgressBar("UniSpreadsheets", $"Injection into: {injectionTarget}", (i + 1) / (float)_spreadsheetDataReceivers.Length);
                    (injectionTarget as IDataInjectionCallbackReceiver)?.OnBeforeDataInject();
                    ReflectionUtility.InjectData(dataSet, injectionTarget);
                    (injectionTarget as IDataInjectionCallbackReceiver)?.OnAfterDataInject();
                }

                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UniSpreadsheets] Error was handled while processing \'{context.assetPath}\'.");
                Debug.LogError(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
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

                UpdateDataReceivers(dataSet, _spreadsheetDataReceivers);

                EditorUtility.ClearProgressBar();
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[UniSpreadsheets] Error was handled while processing \'{context.assetPath}\'.");
                Debug.LogException(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        public static void UpdateDataReceivers(XlsxAssetImporter importer, XlsxAsset asset)
        {
            UpdateDataReceivers(asset.ToDataSet(), importer._spreadsheetDataReceivers);
        }

        private static void UpdateDataReceivers(DataSet dataSet, Object[] targets)
        {
            if (targets == default || targets.Length == default) return;

            try
            {
                for (int i = 0; i < targets.Length; i++)
                {
                    var injectionTarget = targets[i];
                    if (injectionTarget == false) continue;

                    EditorUtility.DisplayProgressBar("UniSpreadsheets", $"Injection data into: {injectionTarget}", (i + 1) / (float)targets.Length);

                    var callbackReceiver = injectionTarget as IDataInjectionCallbackReceiver;

                    callbackReceiver?.OnBeforeDataInject();
                    {
                        ExcelSpreadsheetUtility.InjectData(dataSet, injectionTarget);
                    }
                    callbackReceiver?.OnAfterDataInject();

                    EditorUtility.SetDirty(injectionTarget);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}

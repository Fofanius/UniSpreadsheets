using System;
using System.Data;

namespace UniSpreadsheets
{
    public static partial class ExcelSpreadsheetUtility
    {
        public static void InjectData(DataSet source, object target)
        {
            if (source == default) throw new ArgumentNullException(nameof(source));
            if (target == default) throw new ArgumentNullException(nameof(target));

            var fields = ReflectionUtility.GetSpreadsheetRangeFields(target.GetType());

            foreach (var field in fields)
            {
                if (field.Value.FieldType.IsArray == false) continue;

                if (source.Tables.Contains(field.Key))
                {
                    var data = Deserialize(source.Tables[field.Key], field.Value.FieldType.GetElementType());
                    field.Value.SetValue(target, data);
                }
#if UNITY_EDITOR
                else
                {
                    UnityEngine.Debug.LogWarning($"[UniSpreadsheets] Range \'{field.Value}\' doesn't exists in {source.DataSetName}.");
                }
#endif
            }
        }
    }
}

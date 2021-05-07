using System.Data;
using System.IO;
using UnityEngine;

namespace UniSpreadsheets
{
    public sealed class XlsxAsset : ScriptableObject
    {
        [HideInInspector] [SerializeField] private byte[] _bytes;

        public byte[] Bytes
        {
            get => _bytes;
            private set => _bytes = value;
        }

        public DataSet ToDataSet()
        {
            using var stream = new MemoryStream(Bytes);
            return ExcelSpreadsheetUtility.XlsxStreamToDataSet(stream);
        }

        public static XlsxAsset Create(byte[] bytes)
        {
            var instance = ScriptableObject.CreateInstance<XlsxAsset>();
            instance.Bytes = bytes;

            return instance;
        }
    }
}

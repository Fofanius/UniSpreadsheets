using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEngine;

namespace UniSpreadsheets
{
    public sealed class XlsxAsset : ScriptableObject
    {
        [Header("Import settings")]
        [Space]
        [SerializeField] private string _encodingKey;
        [Header("Origin Data")]
        [Space]
        [SerializeField] private byte[] _bytes;

        public byte[] Bytes
        {
            get => _bytes;
            private set => _bytes = value;
        }

        public string EncodingKey
        {
            get => _encodingKey;
            private set => _encodingKey = value;
        }

        public DataSet ToDataSet()
        {
            using var stream = new MemoryStream(Bytes);
            var encoding = System.Text.Encoding.GetEncoding(EncodingKey);
            return ExcelSpreadsheetUtility.XlsxStreamToDataSet(stream, new ExcelReaderConfiguration(encoding));
        }

        public static XlsxAsset Create(byte[] bytes, string encodingKey)
        {
            var instance = ScriptableObject.CreateInstance<XlsxAsset>();

            instance.Bytes = bytes;
            instance.EncodingKey = encodingKey;

            return instance;
        }
    }
}

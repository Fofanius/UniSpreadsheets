using System;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEngine;

namespace UniSpreadsheets
{
    public static partial class ExcelSpreadsheetUtility
    {
        public static DataSet XlsxFileToDataSet(string filePath, ExcelReaderConfiguration configuration = null)
        {
            return FileToDataSet(filePath, ExcelReaderFactory.CreateReader, configuration);
        }

        public static DataSet CsvFileToDataSet(string filePath, ExcelReaderConfiguration configuration = null)
        {
            return FileToDataSet(filePath, ExcelReaderFactory.CreateCsvReader, configuration);
        }

        public static DataSet XlsxStreamToDataSet(Stream stream, ExcelReaderConfiguration configuration = null)
        {
            if (stream == null) throw new ArgumentNullException(nameof(stream));

            using var reader = ExcelReaderFactory.CreateReader(stream, configuration);
            return reader.AsDataSet();
        }

        private static DataSet FileToDataSet(string filePath, Func<Stream, ExcelReaderConfiguration, IExcelDataReader> readerCreator, ExcelReaderConfiguration configuration)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"File doesn't exists: {filePath}", filePath);
            }

            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = readerCreator(stream, configuration))
                    {
                        return reader.AsDataSet();
                    }
                }
            }
            catch (Exception)
            {
                Debug.LogWarning($"Conversion to {nameof(DataSet)} failed. Path: '{filePath}'.");
                throw;
            }
        }
    }
}

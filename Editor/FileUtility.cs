using System;
using System.IO;

namespace UniSpreadsheets.Editor
{
    internal static class FileUtility
    {
        /// <summary>
        /// Является ли файл занятым.
        /// <para/>
        /// <see href="https://stackoverflow.com/questions/876473/is-there-a-way-to-check-if-a-fileInfo-is-in-use">Решение со StackOverflow.</see>
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <returns></returns>
        internal static bool IsFileLocked(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));

            try
            {
                using var stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read);
                return stream.Length == 0;
            }
            catch (IOException)
            {
                return true;
            }
        }

        internal static void WaitForFile(FileInfo fileInfo)
        {
            if (fileInfo == null) throw new ArgumentNullException(nameof(fileInfo));

            while (IsFileLocked(fileInfo)) { }
        }
    }
}

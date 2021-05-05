using System;

namespace UniSpreadsheets.Testing
{
    [Serializable]
    public class LocalizationItem
    {
        [SpreadsheetAttribute("Key")] private string _key;
        [SpreadsheetAttribute("RU")] private string _russian;
        [SpreadsheetAttribute("EN")] private string _english;

        public override string ToString() => $"Key: {_key}";
    }
}

using System;
using UnityEngine;

namespace UniSpreadsheets.Testing
{
    [Serializable]
    public class LocalizationItem
    {
        [SpreadsheetAttribute("Key")]
        [SerializeField] private string _key;
        [SpreadsheetAttribute("RU")]
        [SerializeField] private string _russian;
        [SpreadsheetAttribute("EN")]
        [SerializeField] private string _english;

        public override string ToString() => $"Key: {_key}";
    }
}

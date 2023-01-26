using System;

namespace UniSpreadsheets
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SpreadsheetAttributeAttribute : Attribute
    {
        public string Key { get; }

        public SpreadsheetAttributeAttribute(string key)
        {
            Key = key ?? string.Empty;
        }
    }
}

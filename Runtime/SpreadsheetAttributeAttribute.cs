using System;

namespace UniSpreadsheets
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class SpreadsheetAttributeAttribute : Attribute
    {
        public string Key { get; }

        public SpreadsheetAttributeAttribute(string key)
        {
            Key = key ?? "";
        }
    }
}

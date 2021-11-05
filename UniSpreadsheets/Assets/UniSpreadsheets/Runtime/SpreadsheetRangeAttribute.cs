using System;

namespace UniSpreadsheets
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SpreadsheetRangeAttribute : Attribute
    {
        public string Name { get; }

        public SpreadsheetRangeAttribute(string name)
        {
            Name = name;
        }
    }
}

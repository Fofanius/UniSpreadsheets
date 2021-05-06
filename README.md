# UniSpreadsheets

Simple **Unity package** to read data from spreadsheets based on modified [open source package](https://github.com/ExcelDataReader/ExcelDataReader).

_Spreadsheet example №1:_

FieldName1 | FieldName2 | field_name_3 | field_name_4 (this attributes mean ...)
-----------|------------|--------------|------------------------
1|Some string|3.5|1
2|Some very long string|2.47|0
3|Another string|2|1

_C# type example №1_

``` c#
[System.Serializable]
public class Data
{
    public int FieldName1;
    public string FieldName2;
    public double field_name_3;
    public bool field_name_4;
}
```

_C# type example №2_

``` c#
[System.Serializable]
public class Data
{
    [SerializeField] private int FieldName1;
    // ...

    public int Value => FieldName1;
    // ...
}
```

_C# type example №3 (attribute name override)_

``` c#
[System.Serializable]
public class Data
{
    [SpreadsheetAttribute("FieldName1")]
    [SerializeField] private int _myFieldName;
    // ...

    public int Value => FieldName1;
    // ...
}
```

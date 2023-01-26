using System;

namespace UniSpreadsheets.Testing
{
    [Serializable]
    public class Weapon
    {
        [SpreadsheetAttribute("ID")] private int _id;
        [SpreadsheetAttribute("Damage")] public int DamageAmount { get; }

        [SpreadsheetAttribute("Price")] private int _price;

        public override string ToString() => $"Weapon ID: {_id} | D -> {DamageAmount} | Price: {_price}";
    }
}

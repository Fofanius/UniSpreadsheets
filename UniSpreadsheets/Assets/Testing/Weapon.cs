using System;

namespace UniSpreadsheets.Testing
{
    [Serializable]
    public class Weapon
    {
        [SpreadsheetAttribute("ID")] private int _id;
        public int Damage;
        [SpreadsheetAttribute("Price")] private int _price;

        public override string ToString() => $"Weapon ID: {_id} | D -> {Damage} | Price: {_price}";
    }
}

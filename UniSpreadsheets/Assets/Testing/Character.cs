using System;

namespace UniSpreadsheets.Testing
{
    [Serializable]
    public class Character
    {
        public int ID;
        public int HP;
        public bool Flag;
        public CharacterType Enum;
    }

    public enum CharacterType
    {
        Main = 1,
        Secondary = 2,
        Low_Priority = 50,
    }
}

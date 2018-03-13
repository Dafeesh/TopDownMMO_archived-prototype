using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedComponents.Server
{
    public class AccountInfo
    {
        public readonly string Name;
        public readonly string Password;
        public readonly AccountType Type;

        public CharacterInfo[] Characters;

        public AccountInfo(string name, string password, AccountType type)
        {
            this.Name = name;
            this.Password = password;
            this.Type = type;
        }

        public enum AccountType
        {
            Basic = 1,
            TechMod = 2
        }
    }
}

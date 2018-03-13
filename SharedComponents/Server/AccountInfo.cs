using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedComponents.Server
{
    public class AccountInfo
    {
        public string Username;
        public Int32 PasswordToken;
        public CharacterInfo[] PlayerCharacters;
    }
}

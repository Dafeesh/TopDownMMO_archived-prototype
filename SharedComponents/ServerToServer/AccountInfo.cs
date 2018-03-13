using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedComponents.ServerToServer
{
    public class AccountInfo
    {
        public string Username;
        public Int32 PasswordToken;
        public CharacterInfo[] PlayerCharacters;
    }

    public class CharacterInfo
    {
        public string Name;
        public Int32 ServerId;
    }
}

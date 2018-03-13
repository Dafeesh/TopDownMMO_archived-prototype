using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

using SharedComponents.Server;
using SharedComponents.Global.GameProperties;

namespace MasterServer.Database
{
    public static class DBConnections
    {

        public class Accounts : IDisposable
        {
            //Static
            private static string ConnectionString = @"Data Source=C:\Users\Blake\Code & Source\_Game\Databases\AccountsDB.sqlite; Version=3;";

            private static readonly string Column_Accounts_Name = "name";
            private static readonly string Column_Accounts_Password = "password";
            private static readonly string Column_Accounts_Type = "type";

            private static readonly string Column_Characters_Owner = "owner";
            private static readonly string Column_Characters_Name = "name";
            private static readonly string Column_Characters_Type = "type";
            private static readonly string Column_Characters_Level = "level";
            private static readonly string Column_Characters_LevelProgress = "level_progress";
            private static readonly string Column_Characters_Credits = "credits";
            //~Static

            SQLiteConnection connection;
            public bool IsDisposed
            { get; private set; }

            public Accounts()
            {
                IsDisposed = false;

                connection = new SQLiteConnection(ConnectionString);
                connection.Open();
            }

            public void Dispose()
            {
                if (!IsDisposed)
                {
                    IsDisposed = true;

                    connection.Close();
                }
            }

            public void AddNew_Account(AccountInfo info)
            {
                try
                {
                    string commandString = "INSERT INTO accounts (" +
                        Column_Accounts_Name + ", " +
                        Column_Accounts_Password + ", " +
                        Column_Accounts_Type +
                        ") values('" +
                        info.Name + "', '" +
                        info.Password + "', " +
                        (int)info.Type + " " +
                        ");";

                    SQLiteCommand cmd = new SQLiteCommand(commandString, connection);
                    cmd.ExecuteNonQuery();
                }
                catch (SQLiteException e)
                {
                    throw e;
                }
            }

            public AccountInfo Fetch_AccountInfo(string name, string password)
            {
                try
                {
                    string commandString = "SELECT * FROM accounts WHERE UPPER(" + Column_Accounts_Name + ")=UPPER('" + name + "');";
                    SQLiteCommand cmd = new SQLiteCommand(commandString, connection);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                        return null;

                    reader.Read();
                    if (reader[Column_Accounts_Password].ToString().CompareTo(password) != 0)
                        return null;
                    else
                    {
                        return new AccountInfo(reader[Column_Accounts_Name].ToString(),
                                               reader[Column_Accounts_Password].ToString(),
                                               (AccountInfo.AccountType)(Int32)reader[Column_Accounts_Type])
                        {
                            Characters = Fetch_Characters(name)
                        };
                    }
                }
                catch (SQLiteException e)
                {
                    throw e;
                }
            }

            public PlayerCharacterInfo[] Fetch_Characters(string owner)
            {
                List<PlayerCharacterInfo> chars = new List<PlayerCharacterInfo>();

                try
                {
                    string commandString = "SELECT * FROM characters WHERE UPPER(" + Column_Characters_Owner + ")=UPPER('" + owner + "');";
                    SQLiteCommand cmd = new SQLiteCommand(commandString, connection);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                        return new PlayerCharacterInfo[0];

                    while (reader.Read())
                    {
                        chars.Add(new PlayerCharacterInfo(reader[Column_Characters_Name].ToString())
                        {
                            VisualLayout = new CharacterVisualLayout((CharacterVisualLayout.VisualType)((Int32)reader[Column_Characters_Type])),
                            Level = (Int32)reader[Column_Characters_Level],
                            Level_Progress = (Int32)reader[Column_Characters_LevelProgress],
                            Credits = (Int32)reader[Column_Characters_Credits]
                        });
                    }
                }
                catch (SQLiteException e)
                {
                    throw e;
                }

                return chars.ToArray();
            }
        }
    }
}

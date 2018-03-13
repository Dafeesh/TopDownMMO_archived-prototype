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

            private static readonly string Accounts_Column_Name = "name";
            private static readonly string Accounts_Column_Password = "password";
            private static readonly string Accounts_Column_Type = "type";

            private static readonly string Characters_Column_Owner = "owner";
            private static readonly string Characters_Column_Name = "name";
            private static readonly string Characters_Column_Type = "type";
            private static readonly string Characters_Column_Level = "level";
            private static readonly string Characters_Column_LevelProgress = "level_progress";
            private static readonly string Characters_Column_Credits = "credits";
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
                        Accounts_Column_Name + ", " +
                        Accounts_Column_Password + ", " +
                        Accounts_Column_Type +
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
                    string commandString = "SELECT * FROM accounts WHERE UPPER(" + Accounts_Column_Name + ")=UPPER('" + name + "');";
                    SQLiteCommand cmd = new SQLiteCommand(commandString, connection);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                        return null;

                    reader.Read();
                    if (reader[Accounts_Column_Password].ToString().CompareTo(password) != 0)
                        return null;
                    else
                    {
                        return new AccountInfo(reader[Accounts_Column_Name].ToString(),
                                               reader[Accounts_Column_Password].ToString(),
                                               (AccountInfo.AccountType)(Int32)reader[Accounts_Column_Type])
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

            public CharacterInfo[] Fetch_Characters(string owner)
            {
                List<CharacterInfo> chars = new List<CharacterInfo>();

                try
                {
                    string commandString = "SELECT * FROM characters WHERE UPPER(" + Characters_Column_Owner + ")=UPPER('" + owner + "');";
                    SQLiteCommand cmd = new SQLiteCommand(commandString, connection);
                    SQLiteDataReader reader = cmd.ExecuteReader();

                    if (!reader.HasRows)
                        return new CharacterInfo[0];

                    while (reader.Read())
                    {
                        chars.Add(new CharacterInfo(owner, reader[Characters_Column_Name].ToString())
                        {
                            Layout = new CharacterLayout((CharacterLayout.VisualType)((Int32)reader[Characters_Column_Type])),
                            Level = (Int32)reader[Characters_Column_Level],
                            Level_Progress = (Int32)reader[Characters_Column_LevelProgress],
                            Credits = (Int32)reader[Characters_Column_Credits]
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

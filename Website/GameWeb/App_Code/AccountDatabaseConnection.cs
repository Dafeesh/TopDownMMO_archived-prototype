using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for AccountDatabaseAccessor
/// </summary>
public class AccountDatabaseConnection : IDisposable
{
    private MySqlConnection con;
    private string connectionString = "Server=127.0.0.1;" +
                                      "Port=3306;" +
                                      "Initial Catalog=game;" +
                                      "User Id=root;" +
                                      "Password=159753;";

    public AccountDatabaseConnection()
    {
        con = new MySqlConnection(connectionString);
        con.Open();
    }

    public bool UsernameExists(string un)
    {
        MySqlCommand command = new MySqlCommand("SELECT COUNT(username) FROM accounts WHERE username = '" + un + "';", con);
        return !(Convert.ToInt32(command.ExecuteScalar()) == 0);
    }

    public bool EmailExists(string em)
    {
        MySqlCommand command = new MySqlCommand("SELECT COUNT(email) FROM accounts WHERE email = '" + em + "';", con);
        return !(Convert.ToInt32(command.ExecuteScalar()) == 0);
    }

    public bool LoginAttempt(string un, string pw)
    {
        MySqlCommand command = new MySqlCommand("SELECT password FROM accounts WHERE username = '" + un + "';", con);
        string passCheck = command.ExecuteScalar().ToString();

        return pw.CompareTo(passCheck) == 0;
    }

    // IDisposable
    private bool disposed = false;
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        con.Close();

        disposed = true;
    }
    //~
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void Button1_Click(object sender, EventArgs e)
    {
        if (UsernameTextBox.Text.Length > 0 && PasswordTextBox.Text.Length > 0)
        {
            using(AccountDatabaseConnection con = new AccountDatabaseConnection())
            {
                if (con.LoginAttempt(UsernameTextBox.Text, PasswordTextBox.Text))
                {
                    Response.Redirect("account/home.aspx");
                }
                else
                {
                    Response.Write("<font color=red>Invalid username or password.</font>");
                }
            }
        }
        else
        {
            Response.Write("<font color=red>* Please type a Username and Password.</font>");
        }
    }
}
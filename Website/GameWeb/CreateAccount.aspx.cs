using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class CreateAccount : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (IsPostBack)
        {
            Validate();
            if (IsValid)
            {
                Response.Redirect("CreateAccountSuccess.aspx");
            }
        }
    }

    protected void UsernameValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = UsernameIsValid(args.Value);
    }

    protected void UsernameExistValidate(object source, ServerValidateEventArgs args)
    {
        if (UsernameIsValid(args.Value))
            args.IsValid = !UsernameExists(args.Value);
        else
            args.IsValid = true;
    }

    protected void EmailExistValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = !EmailExists(args.Value);
    }

    protected void PasswordValidate(object source, ServerValidateEventArgs args)
    {
        args.IsValid = PasswordIsValid(args.Value);
    }

    private bool UsernameIsValid(string un)
    {
        return
            un.Length >= 3 &&
            un.Length <= 12 &&
            Regex.IsMatch(un, "^[a-zA-Z0-9]*$");
    }

    private bool PasswordIsValid(string un)
    {
        return
            un.Length >= 6 &&
            un.Length <= 20;
    }

    private bool UsernameExists(string un)
    {
        bool exists;
        using (AccountDatabaseConnection con = new AccountDatabaseConnection())
        {
            exists = con.UsernameExists(un);
        }

        return exists;
    }

    private bool EmailExists(string em)
    {
        bool exists;
        using (AccountDatabaseConnection con = new AccountDatabaseConnection())
        {
            exists = con.EmailExists(em);
        }

        return exists;
    }

    protected void SubmitButton_Click(object sender, EventArgs e)
    {
        Response.Write("Submit");
    }
}
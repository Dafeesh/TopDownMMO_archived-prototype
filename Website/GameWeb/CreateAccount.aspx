<%@ Page Language="C#" AutoEventWireup="true" CodeFile="CreateAccount.aspx.cs" Inherits="CreateAccount" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            width: 100%;
        }
        .auto-style2 {
            width: 141px;
        }
        .auto-style3 {
            width: 214px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <h1>Create an Account</h1>
        <br />
        <br />
        <table class="auto-style1">
            <tr>
                <td class="auto-style2">
                    Username:<br />
                    - English characters<br />
                    - 3-12 characters long<br />
                    - No symbols<br />
                </td>
                <td class="auto-style3">
                    <asp:TextBox ID="UsernameTextBox" runat="server" Width="180px"></asp:TextBox>
                </td>
                <td>

        <asp:CustomValidator ID="UsernameValidator" runat="server" 
            ErrorMessage="* Invalid username."
            ForeColor="Red" 
            ControlToValidate="UsernameTextBox"
            OnServerValidate="UsernameValidate">
        </asp:CustomValidator>
                    <br />
        <asp:CustomValidator ID="UsernameExistValidator" runat="server" 
            ErrorMessage="* Username aready exists. Please choose another." 
            ForeColor="Red"
            ControlToValidate="UsernameTextBox"
            OnServerValidate="UsernameExistValidate">
        </asp:CustomValidator>

                </td>
            </tr>
            <tr>
                <td class="auto-style2">E-mail:</td>
                <td class="auto-style3">
                    <asp:TextBox ID="EmailTextBox" runat="server" Width="180px"></asp:TextBox>
                </td>
                <td>

        <asp:RegularExpressionValidator ID="EmailTextValidator" runat="server" ErrorMessage="* Invalid e-mail." ForeColor="Red" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*" ControlToValidate="EmailTextBox"></asp:RegularExpressionValidator>
                    <br />
        <asp:CustomValidator ID="EmailExistValidator" runat="server" 
            ErrorMessage="* E-mail already in use." 
            ForeColor="Red" 
            ControlToValidate="EmailTextBox"
            OnServerValidate="EmailExistValidate">
        </asp:CustomValidator>
                    <br />

                </td>
            </tr>
            <tr>
                <td class="auto-style2">
                    Password:<br />
                    - English characters<br />
                    - 6-20 characters long<br />
                    <br />
                </td>
                <td class="auto-style3">
                    <asp:TextBox ID="PasswordTextBox" runat="server" Width="180px"></asp:TextBox>
                    <br />
                </td>
                <td>

        <asp:CustomValidator ID="PasswordValidator" runat="server" 
            ErrorMessage="* Invalid password." 
            ForeColor="Red"
            OnServerValidate="PasswordValidate" ControlToValidate="PasswordTextBox"></asp:CustomValidator>
                    <br />

                </td>
            </tr>
            <tr>
                <td class="auto-style2">Password (again):</td>
                <td class="auto-style3">
                    <asp:TextBox ID="PasswordVTextBox" runat="server" Width="180px"></asp:TextBox>
                </td>
                <td>

        <asp:CompareValidator ID="PasswordVValidator" runat="server" ControlToCompare="PasswordVTextBox" ControlToValidate="PasswordTextBox" ErrorMessage="* Passwords must match." ForeColor="Red"></asp:CompareValidator>

                </td>
            </tr>
        </table>
    
        <br />
        <asp:Button ID="SubmitButton" runat="server" Text="Submit" OnClick="SubmitButton_Click" />
&nbsp;<br />
    
    </div>
    </form>
</body>
</html>

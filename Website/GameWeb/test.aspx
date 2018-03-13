<%@ Page Language="C#" AutoEventWireup="true" CodeFile="test.aspx.cs" Inherits="test" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <div>
    
        <asp:Label ID="TestLabel" runat="server" Text="Test Website"></asp:Label>
&nbsp;&nbsp;
        <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="Click me" />
        <br />
        <br />
        <asp:CheckBox ID="ClickedCheckBox" runat="server" Enabled="False" Text="Clicked?" />
    
    </div>
    </form>
</body>
</html>

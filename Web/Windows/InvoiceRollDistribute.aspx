<%@ Page Language="C#" AutoEventWireup="True" CodeBehind="InvoiceRollDistribute.aspx.cs" Inherits="ERP.UI.Web.Windows.InvoiceRollDistribute" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>分发发票卷</title>
    <script src="../JavaScript/telerik.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <asp:HiddenField runat="server" ID="HiddenField_RollId" />
    <table>
        <tr>
            <td>分发给：</td>
            <td><asp:TextBox runat="server" ID="TextBox_DistributeToUsername"></asp:TextBox></td>
        </tr>
        <tr>            
            <td colspan="2" align="center">
            <div><asp:Label runat="server" ID="Label_Alert"></asp:Label></div>
            <asp:Button runat="server" ID="Button_Submit" Text="确定分发" OnClick="Button_Submit_Click" />
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server"></rad:RadAjaxManager>
    </form>
</body>
</html>

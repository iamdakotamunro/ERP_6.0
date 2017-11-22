<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceLost.aspx.cs" Inherits="ERP.UI.Web.Windows.InvoiceLost" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
    <script src="../JavaScript/telerik.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
        <table>
            <tr>
                <td>
                    起始号码：
                </td>
                <td>
                    <asp:TextBox ID="TB_StartNo" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    结束号码：
                </td>
                <td>
                    <asp:TextBox ID="TB_EndNo" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    备注：
                </td>
                <td>
                    <asp:TextBox ID="TB_Memo" runat="server" TextMode="MultiLine"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button ID="BT_Save" runat="server" onclick="BtSaveClick" Text="保存" />
                </td>
            </tr>
        </table>
        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
            </AjaxSettings>
        </rad:RadAjaxManager>
    </form>
</body>
</html>

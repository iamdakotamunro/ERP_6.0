<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddReckoningCheck.aspx.cs"
    Inherits="ERP.UI.Web.Windows.AddReckoningCheck" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript">
        </script>
    </rad:RadScriptBlock>
    <div id="DIV_GoodsBaseInfo" runat="server">
        <table width="100%">
            <tr>
                <td style="width: 80px; padding: 10px 0 10px 0;">
                    合计：
                </td>
                <td colspan="3">
                    <asp:Literal ID="LitNonceTotalled" runat="server"></asp:Literal>&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td style="padding: 10px 0 10px 0;width: 80px;">
                    差额说明：
                </td>
                <td colspan="2">
                    <asp:TextBox runat="server" ID="TB_Memo" TextMode="MultiLine" Width="400px" Height="180px"></asp:TextBox>
                </td>
                <td style="width: 100px;color: gray;">差额说明备注</td>
            </tr>
            <tr>
                <td colspan="4" style="padding-left: 80px;">
                    <asp:Button ID="Btn_Apply" Text="确定" runat="server" OnClick="Btn_Apply_Click" />&nbsp;&nbsp;
                    <asp:Button ID="Btn_Cancel" Text="取消" runat="server" OnClientClick="return CancelWindow()" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

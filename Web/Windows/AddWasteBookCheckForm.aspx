<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddWasteBookCheckForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.AddWasteBookCheckForm" EnableEventValidation="false" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    <script type="text/javascript" src="../JavaScript/jquery.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    </rad:RadScriptBlock>
    <%--基本信息--%>
    <div id="DIV_GoodsBaseInfo" runat="server">
        <table width="100%">
            <tr>
                <td style="width: 60px;padding:10px 0 10px 0;">
                    当前余额：
                </td>
                <td colspan="3">
                    <asp:Literal ID="LitNonceBalance" runat="server"></asp:Literal>&nbsp;&nbsp;
                </td>
            </tr>
            <tr>
                <td style="padding:10px 0 10px 0">
                    银行余额：
                </td>
                <td colspan="3">
                        <asp:TextBox runat="server" ID="TB_CheckMoney"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TB_CheckMoney"
                        ErrorMessage="*" ForeColor="red"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="REVCost" runat="server" ControlToValidate="TB_CheckMoney"
                        ErrorMessage="*"  ForeColor="red" ValidationExpression="^(-)?(([1-9]{1}\d*)|([0]{1}))(\.(\d){1,4})?$"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td style="padding:10px 0 10px 0">
                    差额说明：
                </td>
                <td colspan="2">
                    <asp:TextBox runat="server" ID="TB_Memo" TextMode="MultiLine" Width="150px" Height="60px"></asp:TextBox>
                </td>
                <td style="width: 100px;color: gray;">
                   不一致原因
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <asp:Button ID="Btn_Apply" Text="确定" runat="server" OnClick="Btn_Apply_Click" />&nbsp;&nbsp;
                    <asp:Button ID="Btn_Cancel" CausesValidation="False" Text="取消" runat="server" 
                        onclick="Btn_Cancel_Click" />
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

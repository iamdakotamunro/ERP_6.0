<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddGoodSaleStockForm.aspx.cs" Inherits="ERP.UI.Web.Windows.AddGoodSaleStockForm" EnableEventValidation="false" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    </rad:RadScriptBlock>
    
    <asp:Panel ID="Panel_GoodsSaleStock" runat="server">
        <table width="100%">
            <tr>
                <td style="width: 10px;">&nbsp;</td>
                <td style="width: 80px;padding:10px 0 10px 0;">
                    商品名称：
                </td>
                <td>
                    <asp:Literal ID="LitGoodsName" runat="server"></asp:Literal>&nbsp;&nbsp;
                    商品编号：<asp:Literal ID="LitGoodsCode" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td style="padding:10px 0 10px 0">
                    卖完缺货：
                </td>
                <td>
                    <asp:CheckBox ID="CB_ShortStock" runat="server" AutoPostBack="True" OnCheckedChanged="Cb_ShortStock_CheckedChanged" />
                    <asp:TextBox runat="server" ID="TB_ReplenishmentCycle"></asp:TextBox>
                    <asp:Literal ID="Lit_ReplenishmentCycle" runat="server" Text="天" ></asp:Literal> &nbsp;&nbsp;
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TB_ReplenishmentCycle" ErrorMessage="补货周期不允许为空！"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="REVCost" runat="server" ControlToValidate="TB_ReplenishmentCycle" ErrorMessage="补货周期为非0数字！" ValidationExpression="([1-9][0-9]*(\.\d+)?)|(0\.\d+)"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td style="padding:10px 0 10px 0">
                    卖完断货：
                </td>
                <td>
                    <asp:CheckBox ID="CB_OutOfStock" runat="server" AutoPostBack="True" OnCheckedChanged="Cb_OutOfStock_CheckedChanged" />
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td style="padding:10px 0 10px 0">
                    非卖库存：
                </td>
                <td>
                    <asp:CheckBox ID="CB_NotSellStock" runat="server" AutoPostBack="True" OnCheckedChanged="Cb_NotSellStock_CheckedChanged" />
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td style="padding:10px 0 10px 0">
                    申请理由：
                </td>
                <td>
                    <asp:TextBox ID="TB_ApplyReason" runat="server" Width="600px"></asp:TextBox>
                </td>
            </tr>
            <tr id="TR_AuditReason" runat="server">
                <td>&nbsp;</td>
                <td style="padding:10px 0 10px 0">
                    审核理由：
                </td>
                <td>
                    <asp:TextBox ID="TB_AuditReason" runat="server" Width="600px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td colspan="2">
                    <asp:Button ID="Btn_Apply" Text="申请" runat="server" OnClick="Btn_Apply_Click" />&nbsp;&nbsp;
                    <asp:Button ID="Btn_Cancel" Text="取消" runat="server" OnClientClick="return CancelWindow()" onclick="Btn_Cancel_Click" />&nbsp;&nbsp;
                    <asp:Button ID="Btn_Agree" Text="同意" runat="server" onclick="Btn_Agree_Click" />&nbsp;&nbsp;
                    <asp:Button ID="Btn_UnAgree" Text="不同意" runat="server" onclick="Btn_UnAgree_Click" />
                </td>
            </tr>
        </table>
    </asp:Panel>

    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="Btn_Apply">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_GoodsSaleStock" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_Cancel">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_GoodsSaleStock" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_Agree">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_GoodsSaleStock" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_UnAgree">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_GoodsSaleStock" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" Skin="WebBlue" runat="server"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

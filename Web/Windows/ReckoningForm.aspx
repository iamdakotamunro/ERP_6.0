<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.ReckoningForm" CodeBehind="ReckoningForm.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script src="../JavaScript/power.js"></script>
    </rad:RadScriptBlock>

    <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0" Width="100%">
        <asp:TableRow>
            <asp:TableCell CssClass="ControlTools">
                <asp:LinkButton ID="LB_Inster" runat="server" OnClick="Button_InsterReckoning">
                    <asp:Image ID="Image1" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />确定
                </asp:LinkButton>&nbsp;
                <asp:LinkButton ID="Auditing" runat="server" OnClick="Button_Auditing">
                    <asp:Image ID="Image2" SkinID="AffirmImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />审核
                </asp:LinkButton>&nbsp;
                <asp:Label ID="Label1" runat="server" Text="Label">&nbsp;</asp:Label>
                <asp:LinkButton ID="LB_Delete" runat="server" OnClick="Button_Delete">
                    <asp:Image ID="IB_Delete" SkinID="DeleteImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />删除
                </asp:LinkButton>&nbsp;
                <asp:LinkButton ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()">
                    <asp:Image ID="IB_Cancel" SkinID="CancelImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />取消
                </asp:LinkButton>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <table class="PanelArea">
        <tr>
            <td class="PromptFromRowTitle">
                往来单位：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_CompanyName" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                公司：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_FilialeList" runat="server" DropDownWidth="180px" AutoPostBack="True"
                    AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择公司"
                    CausesValidation="false" Filter="StartsWith" onselectedindexchanged="Rcb_FilialeListSelectedIndexChanged">
                </rad:RadComboBox>
                <asp:RequiredFieldValidator ID="RTF_FilialeList" EnableClientScript="true" ControlToValidate="RCB_FilialeList"
                    runat="server" ErrorMessage="*" Text="*" />
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                资金账户：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_BankAccountsId" runat="server" RadControlsDir="~/Controls/RadControls"
                    UseEmbeddedScripts="false" AccessKey="T" MarkFirstMatch="True" AllowCustomText="true"
                    ShowToggleImage="True" DataTextField="AccountsName" DataValueField="BankAccountsId"
                    Width="250px" Height="120px">
                </rad:RadComboBox>
                <asp:RequiredFieldValidator ID="RFVBankAccounts" runat="server" ErrorMessage="账户名称不能为空！"
                    Text="*" ControlToValidate="RCB_BankAccountsId"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr id="TR_ShopBankAccount" runat="server" Visible="False">
            <td class="PromptFromRowTitle">
                往来资金账户：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_ShopBankAccountsId" runat="server" RadControlsDir="~/Controls/RadControls"
                    UseEmbeddedScripts="false" AccessKey="T" MarkFirstMatch="True" AllowCustomText="true"
                    ShowToggleImage="True" Width="250px" Height="120px">
                </rad:RadComboBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="账户名称不能为空！"
                    Text="*" ControlToValidate="RCB_ShopBankAccountsId"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                资金金额：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_PaymentPrice" runat="server" SkinID="LongInput"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RFVPaymentPrice" runat="server" ControlToValidate="TB_PaymentPrice"
                    Text="*" ErrorMessage="资金金额不能为空！"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="REVPaymentPrice" runat="server" ControlToValidate="TB_PaymentPrice"
                    Text="*" ErrorMessage="请输入正数！" ValidationExpression="[0-9]{0,}\.?[0-9]{0,}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                手 续 费：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_HC" runat="server" SkinID="LongInput"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVTB_HC" runat="server" ControlToValidate="TB_HC"
                    Text="*" ErrorMessage="请输入正数！" ValidationExpression="\s|[0-9]{0,}\.?[0-9]{0,}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                备注信息：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_Description" runat="server" TextMode="MultiLine" SkinID="LongTextarea"></asp:TextBox>
            </td>
        </tr>
    </table>

    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="LB_Inster">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="ControlToolsTab" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Auditing">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="ControlToolsTab" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Delete">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="ControlToolsTab" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_FilialeList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_BankAccountsId" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

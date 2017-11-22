<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.TransferForm" CodeBehind="TransferForm.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    </rad:RadScriptBlock>
    
    <asp:Panel ID="TransferPanel" runat="server">
    <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0" Width="100%">
        <asp:TableRow>
            <asp:TableCell CssClass="ControlTools">
                <asp:LinkButton ID="LB_Inster" runat="server" OnClick="Button_InsterTransfer">
                    <asp:Image ID="IB_Inster" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />确定
                </asp:LinkButton>&nbsp;
                <asp:LinkButton ID="Auditing" runat="server" OnClick="Button_Auditing">
                    <asp:Image ID="IB_Auditing" SkinID="AffirmImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />审核
                </asp:LinkButton>&nbsp;
                <asp:Label ID="Label_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
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
                <td class="PromptFromRowTitle">转出公司：</td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_FilialeList" runat="server" AutoPostBack="true" DropDownWidth="180px" AllowCustomText="true" EnableLoadOnDemand="true" 
                       Height="200" EmptyMessage="未知公司" CausesValidation="false" Filter="StartsWith" Enabled="False">
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
            <td class="PromptFromRowTitle">
                转出账户：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_OutBankAccountsId" runat="server" MarkFirstMatch="True"
                    DataTextField="BankName" DataValueField="BankAccountsId" Width="250px" Height="80px" Enabled="False">
                </rad:RadComboBox>
                <asp:RequiredFieldValidator ID="RFVBankAccountsOut" runat="server" ErrorMessage="账户名称不能为空！"
                    Text="*" ControlToValidate="RCB_OutBankAccountsId"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
                <td class="PromptFromRowTitle">转入公司：</td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_InFilialeList" runat="server" AutoPostBack="true" DropDownWidth="180px" AllowCustomText="true" EnableLoadOnDemand="true" 
                       Height="120" EmptyMessage="请选择转入公司" CausesValidation="false" Filter="StartsWith" onselectedindexchanged="Rcb_InFilialeListSelectedIndexChanged" >
                    </rad:RadComboBox>
                </td>
            </tr>
        <tr>
            <td class="PromptFromRowTitle">
                转入账户：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_InBankAccountsId" runat="server" MarkFirstMatch="True" DataTextField="BankName"
                    DataValueField="BankAccountsId" Width="250px" Height="200px" AppendDataBoundItems="true" 
                    AllowCustomText="true" EnableLoadOnDemand="True" OnItemsRequested="RCB_InBankAccounts_OnItemsRequested">
                </rad:RadComboBox>
                <asp:RequiredFieldValidator ID="RFVBankAccountsIn" runat="server" ErrorMessage="账户名称不能为空！"
                    Text="*" ControlToValidate="RCB_InBankAccountsId"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                资金金额：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_Income" runat="server" SkinID="LongInput"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RFVIncome" runat="server" ControlToValidate="TB_Income"
                    Text="*" ErrorMessage="转账金额必须填写！"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="REVIncome" runat="server" ControlToValidate="TB_Income"
                    Text="*" ErrorMessage="请输入正数！" ValidationExpression="[0-9]{0,}\.?[0-9]{0,}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                手 续 费：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_HC" runat="server" SkinID="LongInput"></asp:TextBox>
                <asp:CompareValidator ID="CVHC" runat="server" Type="Double" ControlToValidate="TB_HC"
                    ControlToCompare="TB_Income" Operator="LessThanEqual" ErrorMessage="*"></asp:CompareValidator>
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

    </asp:Panel>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="LB_Inster">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="TransferPanel" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Auditing">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="TransferPanel" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Delete">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="TransferPanel" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

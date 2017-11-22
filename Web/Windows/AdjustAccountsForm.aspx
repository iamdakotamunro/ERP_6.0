<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.AdjustAccountsForm" CodeBehind="AdjustAccountsForm.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    
    <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0" Width="100%">
        <asp:TableRow>
            <asp:TableCell CssClass="ControlTools">
                <asp:LinkButton ID="LB_Adjust" runat="server" OnClick="AdjustIncome_Click" ValidationGroup="ReduceIncomeGroup">
                    <asp:Image ID="Image1" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />确定
                </asp:LinkButton>&nbsp;
                <asp:LinkButton ID="Auditing" runat="server" OnClick="Button_Auditing">
                    <asp:Image ID="Image2" SkinID="AffirmImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />审核
                </asp:LinkButton>&nbsp;
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
                公司：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_FilialeList" runat="server" AutoPostBack="true" DropDownWidth="180px"
                    AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="未知公司" 
                    CausesValidation="false" Filter="StartsWith" Enabled="False">
                </rad:RadComboBox>
                <asp:RequiredFieldValidator ID="RTF_FilialeList" EnableClientScript="true" ControlToValidate="RCB_FilialeList" runat="server" ErrorMessage="*" Text="*" />
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                银行名称：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:DropDownList ID="ddlBank" runat="server" Width="240px" Enabled="False">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                输入资金：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_Income" runat="server" SkinID="StandardInput"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RFVIncome" runat="server" ValidationGroup="ReduceIncomeGroup"
                    ControlToValidate="TB_Income" ErrorMessage="金额不允许为空！" Text="*"></asp:RequiredFieldValidator>
                <asp:RegularExpressionValidator ID="REVIncome" runat="server" ValidationGroup="ReduceIncomeGroup"
                    ControlToValidate="TB_Income" Text="*" ErrorMessage="请输入正数！" ValidationExpression="[0-9]{0,}\.?[0-9]{0,}$"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                备注说明：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_Description" runat="server" TextMode="MultiLine" SkinID="LongTextarea"></asp:TextBox>
            </td>
        </tr>
    </table>


    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="LB_Adjust">
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
                    <rad:AjaxUpdatedControl ControlID="ddlBank" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

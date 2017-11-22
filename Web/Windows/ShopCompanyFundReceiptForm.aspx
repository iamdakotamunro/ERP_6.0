<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopCompanyFundReceiptForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShopCompanyFundReceiptForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>门店往来单位收付款</title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <div>
    <table width="100%" style="padding: 10px">
        <tr>
            <td width="100px" align="right">往来店铺：</td>
            <td><rad:RadComboBox ID="RcbShopList" runat="server" AutoPostBack="true" DropDownWidth="180px"
                        AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择店铺"
                        CausesValidation="false" Enabled="false" OnSelectedIndexChanged="RcbShopListSelectedIndexChanged">
                    </rad:RadComboBox></td>
        </tr>
         <%--<tr>
            <td align="right">往来余额：</td>
            <td><rad:RadTextBox ID="RtbCompanyBalance" ReadOnly="true" runat="server" Enabled="false" Width="155px">
                        </rad:RadTextBox></td>
        </tr>--%>
        <tr>
            <td align="right">往来账户：</td>
            <td >
                <rad:RadComboBox ID="RcbShopAccounts" runat="server" AutoPostBack="true" DropDownWidth="180px"
                        AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择往来账户"
                        CausesValidation="false" >
                    </rad:RadComboBox>
            </td>
        </tr>
        <tr>
            <td align="right">可得账户：</td>
            <td >
                <rad:RadComboBox ID="RcbKeedeAccounts" runat="server" AutoPostBack="true" DropDownWidth="180px"
                        AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择可得账户"
                        CausesValidation="false" OnSelectedIndexChanged="RcbKeedeAccountsSelectedIndexChanged">
                    </rad:RadComboBox>
            </td>
        </tr>
        <tr>
            <td align="right">账户余额：</td>
            <td >
                <rad:RadTextBox ID="RtbAccountsBalance" ReadOnly="true" runat="server" Enabled="false" Width="155px">
                        </rad:RadTextBox>
            </td>
        </tr>
        <tr>
            <td align="right">资金金额：</td>
            <td >
                <rad:RadTextBox ID="RtbRealityBalance"  runat="server" Width="155px"></rad:RadTextBox>
            </td>
        </tr>
        <tr>
            <td align="right">手续费用：</td>
            <td >
                <rad:RadTextBox ID="RtbPoudage"  runat="server" Width="155px"></rad:RadTextBox>
            </td>
        </tr>
        <tr>
            <td align="right">交易流水：</td>
            <td >
                <rad:RadTextBox ID="RtbDealNo"  runat="server" Width="155px"></rad:RadTextBox>
            </td>
        </tr>
        <tr>
            <td align="right">往来描述：</td>
            <td>
                <rad:RadTextBox runat="server" ID="RtbDescription" Width="155px" Rows="4" TextMode="MultiLine"/>
            </td>
        </tr>
        <tr>
            <td colspan="2">&nbsp;</td>
        </tr>
        <tr>
            <td align="right">&nbsp;</td>
            <td>
                <asp:Button runat="server"  ID="BtnAudit" Text="审核" OnClick="BtnAuditClick"/>
                <asp:Button ID="BtnCance" runat="server" Text="取消" onclick="BtnCanceClick" />
            </td>
        </tr>
    </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RcbShopList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RtbCompanyBalance"  />
                    <rad:AjaxUpdatedControl ControlID="RcbShopAccounts" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbKeedeAccounts">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RtbAccountsBalance"  />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

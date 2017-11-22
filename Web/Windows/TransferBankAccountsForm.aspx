<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TransferBankAccountsForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.TransferBankAccountsForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>转移资金账户</title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server" />
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    </rad:RadScriptBlock>
    <div runat="server" id="DIV_Panel">
        <table>
            <tr>
                <td class="PromptFromRowTitle">
                    当前操作帐号:
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Label runat="server" ID="Lb_BankName"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    销售公司:
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" AutoPostBack="True" EmptyMessage="销售公司"
                        OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged">
                    </rad:RadComboBox>
                    <asp:RequiredFieldValidator ID="RTF_FilialeList" EnableClientScript="true" ControlToValidate="RCB_SaleFiliale"
                        runat="server" ErrorMessage="*" Text="*" />
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    销售平台:
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_SalePlatform" runat="server" EmptyMessage="销售平台" Height="150px">
                    </rad:RadComboBox>
                </td>
            </tr>
             <tr>
                <td colspan="2">
                    <br/>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    <span style="font-weight: bold">温馨提示:</span>
                </td>
                <td class="AreaEditFromRowInfo">
                    <span style="font-weight: bold">一经此操作，该帐号立即将更新为当前选择公司的主帐号，将不允许删除和修改。</span>
                    <td />
            </tr>
            <tr>
                <td colspan="2">
                    <br/>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:Button ID="BT_Save" runat="server" Text="确定" OnClick="BtSaveClick" />&nbsp;&nbsp;
                    <asp:Button ID="BT_Cancl" runat="server" Text="取消" OnClientClick="return CancelWindow()" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BT_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_Panel" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

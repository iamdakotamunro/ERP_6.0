<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberBalanceManageForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.MemberBalanceManageForm" %>

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
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    </rad:RadScriptBlock>
    <div runat="server" id="DIV_AuditingPanel">
        <table width="100%">
            <tr>
                <td class="ShortFromRowTitle">
                    拒绝理由：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TB_Memo" runat="server" TextMode="MultiLine" Width="80%" Height="50px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TB_Memo"
                        ErrorMessage="*" ValidationGroup="NoPass"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:Button ID="BT_Save" runat="server" Text="通过" OnClick="BtSaveClick" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="BT_NoPass" runat="server" Text="不通过" OnClick="BtNoPassClick" ValidationGroup="NoPass" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="BT_Cancl" runat="server" Text="取消" OnClientClick="return CancelWindow()" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BT_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BT_NoPass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

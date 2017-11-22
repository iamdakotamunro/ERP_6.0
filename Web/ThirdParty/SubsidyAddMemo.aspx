<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubsidyAddMemo.aspx.cs" Inherits="ERP.UI.Web.ThirdParty.SubsidyAddMemo" %>

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
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <div runat="server" id="DIV_MemoPanel">
        <table>
            <tr>
                <td class="ShortFromRowTitle">
                    备注：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TB_Memo" runat="server" TextMode="MultiLine" Width="250px" Height="50px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TB_Memo"
                        ErrorMessage="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:Button ID="BT_Save" runat="server" Text="添加" OnClick="BtSaveClick" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="BT_Cancl" runat="server" Text="取消" OnClientClick="return CancelWindow()" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BT_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_MemoPanel" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>


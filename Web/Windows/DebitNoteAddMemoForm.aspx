<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DebitNoteAddMemoForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.DebitNoteAddMemoForm" %>

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
    <div runat="server" id="DIV_MemoPanel">
        <table>
            <tr>
                <td class="ShortFromRowTitle">
                    单据标题：
                </td>
                <td>
                    <asp:Label runat="server" id="Lb_Title" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="ShortFromRowTitle">
                    当前备注：
                </td>
                <td>
                    <asp:Label runat="server" id="lable_Memo" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="ShortFromRowTitle">
                    <span runat="server" id="span_Title">备注：</span>
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TB_Memo" runat="server" TextMode="MultiLine" Width="500px" Height="80px"></asp:TextBox>
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
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

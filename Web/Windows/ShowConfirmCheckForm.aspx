<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowConfirmCheckForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.ShowConfirmCheckForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>对账确认上传</title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script type="text/javascript" src="../JavaScript/common.js"></script>
        <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    </rad:RadScriptBlock>
    <table class="PanelArea">
        <tr>
            <td colspan="2">
                <span runat="server" id="span_Msg" style="color: black; font-weight: bold; font-size: 14px;"></span>
                <span runat="server" id="span_Msg2" style="color: red; font-weight: bold;font-size: 14px;"></span>
            </td>
        </tr>
        <tr>
            <td>
                <br />
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                上传对账：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadUpload ID="RuLoadXLS" runat="server" ControlObjectsVisibility="None" Width="250px"
                    AllowedFileExtensions=".xls,xlsx,.csv" ReadOnlyFileInputs="True" />
            </td>
        </tr>
        <tr>
            <td colspan="2" align="center">
                <asp:LinkButton ID="LbSubmit" runat="server" OnClientClick="showmask()" OnClick="LbSubmitClick">
                    <asp:Image ID="Image1" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                        BorderStyle="None" />确认
                </asp:LinkButton>
                <asp:Label ID="Label_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                <asp:LinkButton ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()">
                    <asp:Image ID="IB_Cancel" SkinID="CancelImageButton" runat="server" ImageAlign="AbsMiddle"
                        BorderStyle="None" />取消
                </asp:LinkButton>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    </form>
</body>
</html>

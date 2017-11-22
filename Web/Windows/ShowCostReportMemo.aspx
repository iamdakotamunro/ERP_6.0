<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowCostReportMemo.aspx.cs" Inherits="ERP.UI.Web.Windows.ShowCostReportMemo" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script src="../JavaScript/telerik.js" type="text/javascript">
        </script>

    </rad:RadScriptBlock>
    <table class="PanelArea">
        <tr>
            <td align="center">
                <asp:textbox id="TB_Clew" runat="server" textmode="MultiLine" width="600" columns="200"
                    rows="20" readonly="true">
                </asp:textbox>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>


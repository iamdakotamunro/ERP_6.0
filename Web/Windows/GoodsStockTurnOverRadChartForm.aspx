<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsStockTurnOverRadChartForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.GoodsStockTurnOverRadChartForm" %>

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
    <rad:RadChart ID="GoodsStockTurnOverRadChart" runat="server" Width="1300px" Height="740px" SkinID="Common" >
    </rad:RadChart>
    <rad:RadAjaxManager ID="RadAjaxManagerGoods" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" Skin="WebBlue" runat="server">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>
 
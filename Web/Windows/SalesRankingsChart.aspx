<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SalesRankingsChart.aspx.cs"
    Inherits="ERP.UI.Web.Windows.SalesRankingsChart" %>

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
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
        <div align="left" style="z-index: 999;margin-left: 25px;margin-top: 2px;width: 100px;height: 100px; position: relative;">
            <asp:Image runat="server" ID="ImGoodsId" />
        </div>
    <div align="center" style="margin-top: -100px;"> 
        <rad:RadChart ID="MemberRadChart" runat="server" Width="1200px" Height="700px" SkinID="Common">
        </rad:RadChart>
    </div>
    <rad:RadAjaxManager runat="server" ID="RAM">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

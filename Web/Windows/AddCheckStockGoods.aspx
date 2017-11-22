<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddCheckStockGoods.aspx.cs" Inherits="ERP.UI.Web.Windows.AddCheckStockGoods" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
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
    <div>
      <table align="center">
      <tr><td height="40px">商品仓库：</td><td><asp:DropDownList ID="ddl_AddWarehouse" runat="server"></asp:DropDownList></td></tr>
      
      <tr><td>商品名称：</td><td>
                <rad:RadComboBox ID="RCB_GoodsName" runat="server" CausesValidation="false"
                        AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="GoodsName" DataValueField="GoodsId" AutoPostBack="true"
                        Height="200px" OnItemsRequested="RcbGoodsItemsRequested">
                </rad:RadComboBox></td></tr>
      <tr>
       <td colspan="2" height="40px"><asp:Label ID="Lbl_errMsg" runat="server"></asp:Label></td>
      </tr>
      
      <tr>
      
      
      <td colspan="2" align="center" height="40px"><asp:Button ID="btn_add" runat="server" Text="添 加" onclick="BtnAddClick" />
      </td></tr>
      
      </table>
    </div>
    
<rad:RadAjaxManager ID="RAM" runat="server">
<AjaxSettings>
    <rad:AjaxSetting AjaxControlID="btn_add">
         <UpdatedControls><rad:AjaxUpdatedControl ControlID="Lbl_errMsg" LoadingPanelID="loading"/></UpdatedControls>
    </rad:AjaxSetting>
</AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
</rad:RadAjaxLoadingPanel>
    </form>
   
</body>
</html>

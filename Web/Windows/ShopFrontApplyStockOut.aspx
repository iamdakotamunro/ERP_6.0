<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopFrontApplyStockOut.aspx.cs"
    Inherits="ERP.UI.Web.Windows.ShopFrontApplyStockOut" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>门店调拨出库</title>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script type="text/javascript" src="../JavaScript/common.js"></script>
        <script type="text/javascript">
            function Check(objQ, quantity) {
                var num = parseInt(objQ.value);
                var xq = parseInt(quantity);
                if (num > xq) {
                    alert("申请数量(" + num + ")不能超过出库量数量(" + xq + ")");
                    objQ.value = "0";
                }
            }
        </script>
    </rad:RadScriptBlock>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360">
    </rad:RadScriptManager>
    <table class="PanelArea">
        <tr>
            <td style="padding: 5px;">
                申请单号：<%= CurrentStockInfo.TradeCode %>
            </td>
            <td style="padding: 5px;">
                调拨至仓库：<%=CurrentStockInfo.WarehouseName %>
            </td>
            <td style="padding: 5px;">
                调拨出库单：<asp:Label runat="server" ID="LB_SemiStockOutCode"></asp:Label>
            </td>
            <td style="padding: 5px;">
                备注说明：<rad:RadTextBox runat="server" ID="RTB_Remark">
                </rad:RadTextBox>
            </td>
            <td>
                <Ibt:ImageButtonControl runat="server" CausesValidation="false" ID="IBC_StockOut"
                    OnClick="AffirmSemiStockGoods" Text="确认调拨出库" SkinType="Affirm"></Ibt:ImageButtonControl>
            </td>
        </tr>
    </table>
    <rad:RadGrid runat="server" ID="RG_ApplyStockDetail" SkinID="Common" OnNeedDataSource="RG_ApplyStockDetail_OnNeedDataSource"
        OnItemDataBound="RG_ApplyStockList_ItemDataBound">
        <MasterTableView DataKeyNames="ApplyId,GoodsId,Specification,Price,GoodsStock,GoodsName" CommandItemSettings-ShowAddNewRecordButton="false"
            AllowPaging="False">
            <CommandItemTemplate>
            </CommandItemTemplate>
            <Columns>
                <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Specification" HeaderText="商品SKU">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Quantity" HeaderText="需求总数">
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="可用库存">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="LB_Stock" Text='<%# Eval("GoodsStock") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="已调拨">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="LB_SemiQuantity" Text='<%#GetWaitSemiStockQuantity((Guid)Eval("GoodsId")) %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="需调拨">
                    <ItemTemplate>
                        <asp:HiddenField ID="hfGoodsStock" Value='<%# Eval("GoodsStock") %>' runat="server"/>
                        <asp:TextBox runat="server" ID="TB_ApplyQuantity" ToolTip='<%# int.Parse(Eval("Quantity").ToString())-GetWaitSemiStockQuantity((Guid)Eval("GoodsId")) %>'
                            Visible='<%#IsNeedSemiStockGoods((Guid)Eval("GoodsId")) %>' onKeyup="this.value=this.value.replace(/-?\D/g,'')"
                            onblur='Check(this,this.title);' Text='<%# int.Parse(Eval("Quantity").ToString())-GetWaitSemiStockQuantity((Guid)Eval("GoodsId")) %>'></asp:TextBox>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ShopFilialeId" Display="False">
                </rad:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <br />
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_ApplyStockList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RG_ApplyStockList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_ApplyStockList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IBC_StockOut">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_ApplyStockList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

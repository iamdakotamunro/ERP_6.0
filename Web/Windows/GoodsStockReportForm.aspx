<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsStockReportForm.aspx.cs" Inherits="ERP.UI.Web.Windows.GoodsStockReportForm" %>
<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl_1" Src="~/UserControl/ImageButtonControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    </rad:RadScriptBlock>
    
    <table width="100%">
        <tr>
            <td style="text-align: right;">
                <Ibt:ImageButtonControl_1 ID="IbExport" runat="server" OnClick="IbExportOnClick" SkinType="ExportExcel"
                                    Text="导出EXCEL"></Ibt:ImageButtonControl_1>
                            &nbsp;
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <rad:RadGrid ID="RgGoodsStock" runat="server" SkinID="Common_Foot" OnNeedDataSource="RgGoodsStockNeedDataSource" 
                 OnItemCommand="RgGoodsStockOnItemCommand" OnItemDataBound="RgGoodsStockOnItemDataBound">
                    <MasterTableView DataKeyNames="GoodsId" ClientDataKeyNames="GoodsId">
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <CommandItemTemplate>
                            <rad:RadComboBox ID="RcbMonth" runat="server" Width="150px" Height="100px" >
                            </rad:RadComboBox>
                            <Ibt:ImageButtonControl_1 ID="LbSearch" runat="server" CommandName="Search" ValidationGroup="Search"
                                SkinType="Search" Text="查询"></Ibt:ImageButtonControl_1> &nbsp;&nbsp;&nbsp;
                        </CommandItemTemplate>
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                <HeaderStyle Width="200px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="SettlePrice" HeaderText="结算价" UniqueName="SettlePrice">
                                <ItemTemplate>
                                    <%# Eval("SettlePrice")%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="TotalAmount" HeaderText="库存金额" UniqueName="TotalAmount">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("TotalAmount"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>    
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RgGoodsStock">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgGoodsStock" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

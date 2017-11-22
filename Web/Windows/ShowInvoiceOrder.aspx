<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.ShowInvoiceOrder"
    CodeBehind="ShowInvoiceOrder.aspx.cs" %>
<%@Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>发票订单表</title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadGrid ID="RGGoodsOrder" runat="server" SkinID="Common" OnNeedDataSource="RGGoodsOrder_NeedDataSource">
        <MasterTableView>
            <CommandItemTemplate>
                <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" CausesValidation="false" SkinType="Refresh">
                </Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridBoundColumn DataField="OrderNo" HeaderText="订单编号" UniqueName="OrderNo">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="Consignee" HeaderText="收货人" UniqueName="Consignee">
                    <ItemTemplate>
                        <asp:Label ID="ConsigneeLabel" runat="server" Text='<%# Eval("Consignee") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="消费额" UniqueName="RealTotalPrice">
                    <ItemTemplate>
                        <asp:Label ID="LabRealTotalPriceSum"  Text='<%# (Convert.ToDecimal(Eval("RealTotalPrice"))+Convert.ToDecimal(Eval("PaymentByBalance"))).ToString("0.##")%>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Carriage" HeaderText="快递费"  DataFormatString="{0:C}" UniqueName="Carriage">
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="可开票金额" UniqueName="InvoicePrice">
                    <ItemTemplate>
                        <asp:Label ID="LabInvoiceSum"  Text='<%# (Convert.ToDecimal(Eval("RealTotalPrice"))+Convert.ToDecimal(Eval("PaymentByBalance"))-Convert.ToDecimal(Eval("Carriage"))).ToString("0.##")%>' runat="server"></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ConsignTime" HeaderText="订单完成时间" UniqueName="ConsignTime"
                    DataFormatString="{0:yy-MM-dd HH:mm}">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="订单状态" UniqueName="OrderState">
                    <ItemTemplate>
                        <asp:Label ID="OrderState" runat="server" Text='<%# GetOrderState((int)Eval("OrderState")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RGGoodsOrder">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGGoodsOrder" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

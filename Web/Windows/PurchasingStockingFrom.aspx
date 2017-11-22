<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PurchasingStockingFrom.aspx.cs"
    Inherits="ERP.UI.Web.Windows.PurchasingStockingFrom" %>

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
        <script>  
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
        
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }

            function ShowPurchasingFrom() {
                window.radopen("./Windows/AddGoodsFrom.aspx?PurchasingID=<%=Request["PurchasingID"]%>",
                    "PurchasingForm");
            }
        </script>
    </rad:RadScriptBlock>
    <div class="StagePanel" style="width: 100%;">
        <table class="StagePanelHead" border="0" style="width: 100%;">
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td align="right" style="height: 25px">
                                <div style="float: right; width: 100%;">
                                    <center>
                                        <asp:Label ID="lab_Purchasing" runat="server"></asp:Label>
                                    </center>
                                </div>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <%--采购表--%>
        <rad:RadGrid ID="Rgd_PurchasingDetail" SkinID="Common_Foot" runat="server" OnNeedDataSource="Rgd_PurchasingDetail_OnNeedDataSource"
            AllowMultiRowSelection="true" Width="100%" AllowPaging="false" OnItemDataBound="Rgd_PurchasingDetail_ItemDataBound">
            <ClientSettings>
                <Selecting EnableDragToSelectRows="false" />
            </ClientSettings>
            <MasterTableView CommandItemDisplay="None" DataKeyNames="GoodsID,GoodsCode,Specification,PlanQuantity,PurchasingGoodsID,Price,DayAvgStocking,PlanStocking,RealityQuantity,State"
                ShowFooter="true" EditMode="InPlace" ClientDataKeyNames="GoodsID,GoodsCode,Specification">
                <CommandItemStyle HorizontalAlign="right" Height="25px" />
                <Columns>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" ReadOnly="true" UniqueName="GoodsName">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" ReadOnly="true" UniqueName="GoodsCode">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" ReadOnly="true" UniqueName="Specification">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="PlanQuantity" Aggregate="Sum" FooterText="合计数量:" HeaderText="采购数量" UniqueName="PlanQuantity">
                        <ItemTemplate>
                            <%# Eval("PlanQuantity")%>
                        </ItemTemplate>
                        <FooterTemplate>
                        </FooterTemplate>
                        <FooterStyle HorizontalAlign="Center" />
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="RealityQuantity" HeaderText="实际来货" ReadOnly="true"
                        UniqueName="RealityQuantity">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="State" HeaderText="采购状态" ReadOnly="true" UniqueName="State">
                        <ItemTemplate>
                            <%# Eval("State").ToString()=="0"?"未完成":"已完成" %>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="PurchasingGoodsType" HeaderText="是否赠品" UniqueName="PurchasingGoodsType" ReadOnly="true">
                        <ItemTemplate>
                            <%# Eval("PurchasingGoodsType").ToString()=="1"?"赠品":"" %>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <%--入库单集合--%>
        <rad:RadGrid ID="Rgd_StockGrid" runat="server" SkinID="Common_Foot" OnNeedDataSource="Rgd_StockGrid_OnNeedDataSource"
            OnDetailTableDataBind="Rgd_StockGrid_DetailTableDataBind" Width="100%" AllowPaging="false"
            ShowFooter="true">
            <ClientSettings AllowDragToGroup="True">
                <Selecting EnableDragToSelectRows="false" />
            </ClientSettings>
            <MasterTableView CommandItemDisplay="None" ExpandCollapseColumn-ButtonType="LinkButton"
                EditMode="InPlace" GridLines="None" DataKeyNames="StockId" ShowFooter="true">
                <Columns>
                    <rad:GridTemplateColumn HeaderText="" ReadOnly="true" UniqueName="StockIng">
                        <ItemTemplate>
                            <table height="25px" class="HeadCenter" width="100%">
                                <tr>
                                    <td width="8%" align="left">
                                        入库单号:
                                    </td>
                                    <td width="10%" align="left">
                                        <%#Eval("TradeCode") %>
                                    </td>
                                    <td width="8%" align="left">
                                        原始单号:
                                    </td>
                                    <td width="10%" align="left">
                                        <%#Eval("LinkTradeCode")%>
                                    </td>
                                    <td width="8%" align="left">
                                        入库仓库:
                                    </td>
                                    <td width="10%" align="left">
                                        <%# GetWareHouseName(new Guid(Eval("WarehouseId").ToString())) %>
                                    </td>
                                    <td width="8%" align="left">
                                        经办人:
                                    </td>
                                    <td align="left">
                                        <%#Eval("Transactor") %>
                                    </td>
                                </tr>
                            </table>
                        </ItemTemplate>
                        <HeaderStyle Width="99%" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Left" />
                    </rad:GridTemplateColumn>
                </Columns>
                <DetailTables>
                    <rad:GridTableView runat="server" Width="100%" NoDetailRecordsText="无记录信息。" ShowFooter="true">
                        <Columns>
                            <rad:GridTemplateColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode">
                                <ItemTemplate>
                                    <asp:Label ID="print_del" runat="server" Text='<%# Bind("GoodsCode") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                <HeaderStyle Width="20%" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                <ItemStyle Width="10%" HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <%--<rad:GridBoundColumn DataField="Units" HeaderText="单位" UniqueName="Units">
                                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                <ItemStyle Width="10%" HorizontalAlign="Center" />
                            </rad:GridBoundColumn>--%>
                            <rad:GridBoundColumn DataField="Quantity" HeaderText="数量" UniqueName="Quantity" Aggregate="Sum" 
                                FooterText="入库总数:">
                                <FooterStyle HorizontalAlign="Center" />
                                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridCalculatedColumn DataFields="UnitPrice,Quantity" HeaderText="小计" UniqueName="UnitPrice"
                                DataType="System.Decimal" Expression="{0}*{1}" FooterText="入库总金额 : " Aggregate="Sum">
                                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridCalculatedColumn>
                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="是否赠品" UniqueName="type">
                                <ItemTemplate>
                                    <%# Convert.ToDecimal(Eval("UnitPrice"))==0?"赠品":"" %>
                                </ItemTemplate>
                                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </rad:GridTableView>
                </DetailTables>
            </MasterTableView>
        </rad:RadGrid>

        <rad:RadAjaxManager runat="server" ID="RAM">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RAM">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="Rgd_StockGrid">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Rgd_StockGrid" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </div>
    </form>
</body>
</html>

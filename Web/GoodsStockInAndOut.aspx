<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.GoodsStockInAndOutAw" CodeBehind="GoodsStockInAndOut.aspx.cs" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script type="text/javascript">
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }

        </script>

    </rad:RadScriptBlock>
    <table class="PanelArea">
        <tr>
            <td>
                <table>
                    <tr>
                        <td>&nbsp;年份：
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="DDL_Years" AutoPostBack="true" OnSelectedIndexChanged="DdlYearsSelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>日期：
                        </td>
                        <td>
                            <rad:RadDatePicker ID="R_StartTime" SkinID="Common" runat="server" Width="145px">
                            </rad:RadDatePicker>
                        </td>
                        <td>-</td>
                        <td>
                            <rad:RadDatePicker ID="R_EndTime" SkinID="Common" runat="server" Width="145px">
                            </rad:RadDatePicker>
                        </td>
                        <td>出入库类型：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_StorageType" runat="server" Width="130px"></rad:RadComboBox>
                        </td>
                        <td>仓库：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_Warehouse" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                                MarkFirstMatch="True" ShowToggleImage="True" DataTextField="Value" DataValueField="Key"
                                Width="120px" Height="200px" AutoPostBack="True" OnSelectedIndexChanged="RCBWarehouse_OnSelectedIndexChanged">
                            </rad:RadComboBox>
                        </td>
                        <td>物流配送公司：
                        </td>
                        <td>
                            <asp:DropDownList ID="DdlSaleFiliale" Width="120px" DataTextField="Value" DataValueField="Key" runat="server" AutoPostBack="True" OnSelectedIndexChanged="RCBSaleFiliale_OnSelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                    </tr>
                </table>
            </td>
            <td>&nbsp;</td>
            <td>
                <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="CreationData" OnClick="IbCreationDataClick" />
                &nbsp;
            <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="IbExportDataClick" />
            </td>
        </tr>
    </table>
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <rad:RadTreeView ID="RTVCompanyCussent" runat="server" SkinID="Common" Height="600px"
                    Width="200px" AutoPostBack="true" CausesValidation="True" OnNodeClick="RtvCompanyCussentNodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="StockGrid" runat="server" SkinID="Common_Foot" OnNeedDataSource="StockGrid_NeedDataSource"
                    AllowSorting="true" ShowFooter="true">
                    <MasterTableView>
                        <CommandItemTemplate>
                            <Ibt:ImageButtonControl ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridTemplateColumn DataField="FilialeId" HeaderText="公司" UniqueName="FilialeId">
                                <ItemTemplate>
                                    <%# GetFilialeName(Eval("FilialeId"),Eval("IsOut"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn HeaderText="商品名" DataField="GoodsName">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="Quantity" HeaderText="数量" UniqueName="Quantity">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%# Eval("Quantity").ToString() %>
                                </ItemTemplate>
                                <FooterStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="Time" HeaderText="次数" UniqueName="Time">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="Price" HeaderText="单价" UniqueName="Price">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("Price").ToString()))%>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="出入库类型">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%# GetStockType(Convert.ToInt32(Eval("StorageType")))%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn UniqueName="ToalAmount" HeaderText="总金额">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="TB_Amount" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(ReturnTotalPrice(Eval("StorageType"),Eval("TotalPrice"))) %>'></asp:Label>
                                </ItemTemplate>
                                <FooterStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="最低价">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%#Eval("LowerPrice")%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager ID="StockWindowManager" runat="server" Height="742px" Width="900px"
        ReloadOnShow="true">
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IB_CreationData">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="StockGrid" />
                    <rad:AjaxUpdatedControl ControlID="TB_TotalAmount" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="StockGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="StockGrid" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RTVCompanyCussent">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="StockGrid" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_Years">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RDP_EndTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RDP_StartTime">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RDP_EndTime">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_EndTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" Skin="WebBlue" runat="server">
    </rad:RadAjaxLoadingPanel>
</asp:Content>


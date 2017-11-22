<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MainMaster.master"
    CodeBehind="GoodsStockTurnOverChart.aspx.cs" Inherits="ERP.UI.Web.GoodsStockTurnOver" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <table class="StagePanel">
        <tr>
            <td style="width: 200px; vertical-align: top;">
                <rad:RadTreeView ID="TVGoodsClass" runat="server" SkinID="Common" Height="500px"
                    Width="180px" OnNodeClick="TvGoodsClass_NodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <%--搜索条件--%>
                <table>
                    <tr>
                        <td>
                            <rad:RadComboBox ID="RCB_Warehouse" runat="server" useembeddedscripts="false" AccessKey="T"
                                MarkFirstMatch="True" ShowToggleImage="True" ErrorMessage="仓库列表" AutoPostBack="True" OnSelectedIndexChanged="RcbWarehouseSelectChanged">
                            </rad:RadComboBox>
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false"  EmptyMessage="物流公司">
                            </rad:RadComboBox>
                        </td>
                        <td>
                            <rad:RadComboBox AutoPostBack="true" ID="RCB_State" runat="server" ErrorMessage="状态">
                                <Items>
                                    <rad:RadComboBoxItem Text="全部状态" Value="0" />
                                    <rad:RadComboBoxItem Text="下架缺货有库存" Value="1" />
                                    <rad:RadComboBoxItem Text="无销售商品" Value="2" />
                                </Items>
                            </rad:RadComboBox>
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_Personnel" runat="server" UseEmbeddedScripts="false" AutoPostBack="True"
                                EmptyMessage="采购人">
                            </rad:RadComboBox>
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Width="150px"
                                Height="250px" AutoPostBack="True" Filter="StartsWith" EmptyMessage="供应商列表" ToolTip="供应商列表">
                            </rad:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <rad:RadTextBox runat="server" EmptyMessage="商品名称/编号" ID="RTB_GoodsNameOrCode" Width="280px" />
                        </td>
                        <td>
                            <asp:CheckBox runat="server" Checked="True" Text="是否统计绩效" ID="CB_IsPerformance" ToolTip="是否统计绩效" />
                        </td>
                        <td>
                            <%--<asp:ImageButton ID="IB_CreationData" runat="server" SkinID="CreationData" OnClick="IbCreationDataClick"
                                ToolTip="生成报表数据" />--%>
                            <asp:ImageButton ID="IB_Search" runat="server" SkinID="SearchButton" OnClick="IbCreationDataClick" />
                        </td>
                        <td>
                            <asp:ImageButton ID="IB_CreationChart" runat="server" SkinID="CreationChart" OnClick="IbCreationChartClick"
                                ToolTip="生成折线图" />
                        </td>
                        <td>
                            <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="Ib_ExportData_Click" />
                        </td>
                    </tr>
                </table>
                <%--数据源--%>
                <rad:RadGrid ID="GridGoodsStock" runat="server" SkinID="CustomPaging" OnNeedDataSource="GridGoodsStock_NeedDataSource"
                    Skin="WebBlue">
                    <MasterTableView DataKeyNames="GoodsId" ClientDataKeyNames="GoodsId">
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsCode">
                                <ItemTemplate>
                                    <a style="text-decoration: underline; cursor: pointer; font-weight: bold" title="点击查看商品库存周转率曲线图"
                                        onclick="GoodsStockDetails('<%# Eval("GoodsId") %>')">
                                        <%# Eval("GoodsName")%>
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle Width="220px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="StockNums" HeaderText="库存数" UniqueName="StockNums">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="最后进价" UniqueName="RecentInPrice">
                                <ItemTemplate>
                                    <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("RecentInPrice").ToString()))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="SaleNums" HeaderText="30天内销售数量" UniqueName="SaleNums">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="TurnOverStr" HeaderText="库存周转" UniqueName="TurnOverStr">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="TurnOverByFiling" HeaderText="报备周转" UniqueName="TurnOverByFiling">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="RecentCDate" HeaderText="进货日期" UniqueName="RecentCDate">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="CompanyName" HeaderText="供应商" UniqueName="CompanyName">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="PersonResponsibleName" HeaderText="责任人" UniqueName="PersonResponsibleName">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="IsStatisticalPerformanceStr" HeaderText="绩效状态" UniqueName="IsStatisticalPerformanceStr">
                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="IsScarcityStr" HeaderText="缺货" UniqueName="IsScarcityStr">
                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="IsStateStr" HeaderText="下架" UniqueName="IsStateStr">
                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="TVGoodsClass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="TVGoodsClass" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="GridGoodsStock" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <%--<rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SaleFiliale"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>--%>
            <rad:AjaxSetting AjaxControlID="RCB_State">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_State"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Personnel">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Personnel"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Company">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Company"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Warehouse" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RCB_SaleFiliale" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GridGoodsStock" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="TVGoodsClass" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="GridGoodsStock">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GridGoodsStock" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadWindowManager ID="RWM" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="ChartForm" runat="server" Title="库存周转率折线图" OnClientShow="clientShow" />
            <rad:RadWindow ID="RWM1" runat="server" Title="商品周转折线图" OnClientShow="clientShow" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script language="javascript" type="text/javascript">
            function clientShow(sender) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }

            function GoodsStockDetails(goodsId) {
                window.radopen("./Windows/GoodsStockTurnOverDetailsChartForm.aspx?GoodsId=" + goodsId, "RWM1");
                return false;
            }

            function GoodsStockTurnOverRadChartClick(goodsClassId, personnelId, companyId, goodsNameOrCode, state, IsPerformance) {
                window.radopen("./Windows/GoodsStockTurnOverRadChartForm.aspx?GoodsClassId=" + goodsClassId + "&PersonnelId=" + personnelId + "&CompanyId=" + companyId + "&GoodsNameOrCode=" + goodsNameOrCode + "&State=" + state + "&IsPerformance=" + IsPerformance, "ChartForm");
                return false;
            }
        </script>
    </rad:RadScriptBlock>
    <rad:RadAjaxLoadingPanel ID="loading" Skin="WebBlue" runat="server">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

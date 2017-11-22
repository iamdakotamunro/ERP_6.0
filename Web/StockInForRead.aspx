<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.StockInForReadAw" CodeBehind="StockInForRead.aspx.cs" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowEditForm(stockId, stockType, linkTradeType) {
                if (stockType == 8) {
                    //借出申请
                    window.radopen("./Windows/ApprovalGoodsLendApplyForm.aspx?IsSel=1&StockId=" + stockId,"LendForm");
                } else if (stockType == 6) {
                    //售后退货出库 
                    window.radopen("./Windows/ApprovalDefectiveReturnOutForm.aspx?IsSel=1&StockId=" + stockId, "ReturnOutForm");
                } else if (stockType == 5) {
                    //采购退货出库
                    window.radopen("./Windows/ApprovalPurchaseReturnOutForm.aspx?IsSel=1&StockId=" + stockId, "PurchaseReturnOutForm");
                } else if (stockType == 7) {
                    if (linkTradeType==4) {
                        //销售出库
                        window.radopen("./Windows/ApprovalGoodsSaleOutForm.aspx?IsSel=1&StockId=" + stockId + "&TradeType=" + linkTradeType, "ShopApplyForm");
                    } else {
                        //销售出库
                        window.radopen("./Windows/ApprovalGoodsSaleOutForm.aspx?IsSel=1&StockId=" + stockId + "&TradeType=" + linkTradeType, "GoodsSaleOutForm");
                    }
                } else if (stockType == 9) {
                    //借入返回
                    window.radopen("./Windows/ApprovalGoodsBorrowApplyReturnForm.aspx?IsSel=1&StockId=" + stockId, "GoodsBorrowApplyReturnForm");
                }
                else if (stockType == 1) {
                    //采购进货入库
                    window.radopen("./Windows/GoodsPurchaseInDetailForm.aspx?StockId=" + stockId, "GoodsPurchaseInForm");
                } else if (stockType == 2) {
                    //销售退货入库
                    window.radopen("./Windows/ApprovalSaleReturnInForm.aspx?IsSel=1&StockId=" + stockId, "SaleReturnInForm");
                } else if (stockType == 3) {
                    //借入申请
                    window.radopen("./Windows/ApprovalGoodsBorrowApplyForm.aspx?IsSel=1&StockId=" + stockId, "BorrowForm");
                } else if (stockType == 4) {
                    //借出返回
                    window.radopen("./Windows/ApprovalGoodsLendApplyReturnForm.aspx?IsSel=1&StockId=" + stockId, "GoodsLendApplyReturnForm");
                }
                return false;
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=StockGrid.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=StockGrid.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
            function RowDblClick(obj, args) {
                ShowEditForm(args.getDataKeyValue("StockId"), args.getDataKeyValue("StockType"), args.getDataKeyValue("LinkTradeType"));
            }
        </script>

    </rad:RadScriptBlock>
    <table class="PanelArea">
        <tr>
            <td style="width: 72px;"></td>
            <td class="AreaEditFromRowInfo">
                <table>
                    <tr>
                        <td>年份：
                        </td>
                        <td>
                            <asp:DropDownList runat="server" ID="DDL_Years" AutoPostBack="true" OnSelectedIndexChanged="DdlYearsSelectedIndexChanged">
                            </asp:DropDownList>
                        </td>
                        <td>时间：
                        </td>
                        <td>
                            <rad:RadDatePicker ID="R_StartTime" SkinID="Common" runat="server" Width="145px">
                            </rad:RadDatePicker>
                        </td>
                        <td>-
                        </td>
                        <td>
                            <rad:RadDatePicker ID="R_EndTime" SkinID="Common" runat="server" Width="145px">
                            </rad:RadDatePicker>
                        </td>
                    </tr>
                </table>
            </td>
            <td>单据编号：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_OrderNo" runat="server" Width="200px"></asp:TextBox>
            </td>
            <td>商品名称：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_GoodsName" runat="server" Width="200px"></asp:TextBox>
            </td>
            <td>出入库类型：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_SalesType" runat="server" Width="100px" Height="240px" DropDownWidth="120px">
                   
                </rad:RadComboBox>
            </td>
        </tr>
        <tr>
            <td class="AreaEditFromRowInfo" colspan="8" align="right">
                <table>
                    <tr>
                        <%--                    <td>
                        公司：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <rad:RadComboBox ID="RCB_Filiale" AutoPostBack="true" runat="server" 
                            MarkFirstMatch="True" Height="160px" DataValueField="ClassId" DataTextField="ClassName"
                            AppendDataBoundItems="true">
                        </rad:RadComboBox>
                    </td>--%>
                        <td>仓库：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <rad:RadComboBox ID="RCB_Warehouse" runat="server" MarkFirstMatch="True"
                                Height="160px" DataValueField="WarehouseId" DataTextField="WarehouseName" AppendDataBoundItems="true">
                            </rad:RadComboBox>
                        </td>
                        <td align="right">
                            <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="CreationData" OnClick="IbCreationDataClick" />
                        </td>
                        <td align="left">
                            <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="IbExportDataClick" />
                        </td>
                        <td>&nbsp;</td>
                    </tr>
                </table>
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
                <rad:RadGrid ID="StockGrid" runat="server" SkinID="CustomPaging" MasterTableView-CommandItemDisplay="None"
                    OnNeedDataSource="StockGrid_NeedDataSource">
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="StockId,StockType,DateCreated,LinkTradeType" ClientDataKeyNames="StockId,StockType,DateCreated,LinkTradeType">
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridTemplateColumn DataField="StockType" HeaderText="单据类型" UniqueName="StockType">
                                <ItemTemplate>
                                    <asp:Label ID="StockTypeLabel" runat="server" Text='<%# GetStockType(Convert.ToInt32(Eval("StockType"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="DateCreated" HeaderText="创建时间" UniqueName="DateCreated"
                                DataFormatString="{0:D}">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="TradeCode" HeaderText="单据编号" UniqueName="TradeCode">
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="LinkTradeCode" HeaderText="原始单据号" UniqueName="LinkTradeCode">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="ThirdCompanyID" HeaderText="供货单位" UniqueName="CompanyId">
                                <ItemTemplate>
                                    <asp:Label ID="LB_CompanyId" runat="server" Text='<%# GetCompany(new Guid(Eval("ThirdCompanyID").ToString()))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="WarehouseId" HeaderText="仓库" UniqueName="WarehouseId">
                                <ItemTemplate>
                                    <asp:Label ID="LB_WarehouseId" runat="server" Text='<%# GetWarehouseName(new Guid(Eval("WarehouseId").ToString())) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="AccountReceivable" HeaderText="金额" UniqueName="AccountReceivable">
                                <ItemTemplate>
                                    <asp:Label ID="AccountReceivableLabel" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Math.Abs(Convert.ToDouble(Eval("AccountReceivable")))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="StockState" HeaderText="单据状态" UniqueName="StockState">
                                <ItemTemplate>
                                    <asp:Label ID="StockStateLabel" runat="server" Text='<%# EnumAttribute.GetKeyName((StorageRecordState)Eval("StockState")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager ID="StockWindowManager" runat="server" Title="购买入库信息" Height="646px"
        Width="900px" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="LendForm" runat="server" Title="借出申请" />
            <rad:RadWindow ID="ReturnOutForm" runat="server" Title="售后退货出库" Height="540" Width="900" />
            <rad:RadWindow ID="PurchaseReturnOutForm" runat="server" Title="采购退货出库" Height="580" Width="900" />
            <rad:RadWindow ID="GoodsBorrowApplyReturnForm" runat="server" Title="借入返回" Height="540" Width="900" />
            <rad:RadWindow ID="GoodsSaleOutForm" runat="server" Title="销售出库" Height="540" Width="900" />
            <rad:RadWindow ID="ShopApplyForm" runat="server" Title="门店要货申请出库" Height="540" Width="900" />

            <rad:RadWindow ID="GoodsPurchaseInForm" runat="server" Title="采购进货" Height="540" Width="900"/>
            <rad:RadWindow ID="SaleReturnInForm" runat="server" Title="销售退货" Height="540" Width="900"/>
            <rad:RadWindow ID="BorrowForm" runat="server" Title="借入申请" Height="540" Width="900"/>
            <rad:RadWindow ID="GoodsLendApplyReturnForm" runat="server" Title="借出返还" Height="540" Width="900"/>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IB_CreationData">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="StockGrid" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RTVCompanyCussent">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="StockGrid" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Filiale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Warehouse" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
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
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

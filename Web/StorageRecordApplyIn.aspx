<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="StorageRecordApplyIn.aspx.cs" Inherits="ERP.UI.Web.StorageRecordApplyIn" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <script src="My97DatePicker/WdatePicker.js"></script>
    <script src="JavaScript/ToolTipMsg.js"></script>
    <script src="JavaScript/GiveTip.js"></script>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowGoodsBorrowApply() {
                //借入申请
                window.radopen("./Windows/GoodsBorrowApplyForm.aspx", "BorrowForm");
                return false;
            }

            function ShowSaleReturnIn() {
                //销售退货入库
                window.radopen("./Windows/SaleReturnInForm.aspx", "SaleReturnInForm");
                return false;
            }

            function ShowGoodsPurchaseIn() {
                //采购进货入库
                window.radopen("./Windows/GoodsPurchaseInForm.aspx", "GoodsPurchaseInForm");
                return false;
            }

            function clientShow(sender, eventArgs) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }

            //编辑
            function ShowEditStorageRecordApplyInForm(stockId, stockType, type) {
                if (stockType == 1) {
                    //采购进货入库(重送)
                    window.radopen("./Windows/EditGoodsPurchaseInForm.aspx?StockId=" + stockId, "GoodsPurchaseInForm");
                } else if (stockType == 2) {
                    //销售退货入库
                    window.radopen("./Windows/EditSaleReturnInForm.aspx?IsAgain=" + type + "&StockId=" + stockId, "SaleReturnInForm");
                } else if (stockType == 3) {
                    //借入申请
                    window.radopen("./Windows/EditGoodsBorrowApplyForm.aspx?IsAgain=" + type + "&StockId=" + stockId, "BorrowForm");
                } else if (stockType == 4) {
                    //借出返回(重送)
                    window.radopen("./Windows/EditGoodsLendApplyReturnForm.aspx?StockId=" + stockId, "EditGoodsLendApplyReturnForm");
                }
                return false;
            }

            //审批
            function ShowAuditingForm(stockId, stockType) {
                if (stockType == 1) {
                    //采购进货入库
                    window.radopen("./Windows/ApprovalGoodsPurchaseInForm.aspx?StockId=" + stockId, "GoodsPurchaseInForm");
                } else if (stockType == 2) {
                    //销售退货入库
                    window.radopen("./Windows/ApprovalSaleReturnInForm.aspx?StockId=" + stockId, "SaleReturnInForm");
                } else if (stockType == 3) {
                    //借入申请
                    window.radopen("./Windows/ApprovalGoodsBorrowApplyForm.aspx?StockId=" + stockId, "BorrowForm");
                } else if (stockType == 4) {
                    //借出返回
                    window.radopen("./Windows/ApprovalGoodsLendApplyReturnForm.aspx?StockId=" + stockId, "GoodsLendApplyReturnForm");
                }
                return false;
            }

            function OnDeleteConfirm() {
                var conf = window.confirm('提示：是否确认作废吗？');
                if (!conf)
                    return false;
                return true;
            }

            //打印
            function PrintSemiStockInDetial(stockId) {
                window.radopen("./Windows/PrintStorageRecordApplyInDetail.aspx?StockId=" + stockId + "&IsPrintPrice=0", "PrintSemiStockIn");
                return false;
            };

            //打印-含价格
            function PrintSemiStockInDetialContainPrice(stockId) {
                window.radopen("./Windows/PrintStorageRecordApplyInDetail.aspx?StockId=" + stockId + "&IsPrintPrice=1", "PrintSemiStockInContainPrice");
                return false;
            };
        </script>
    </rad:RadScriptBlock>
    <style type="text/css">
        .timeclass div {
            float: left;
        }
    </style>
    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">单据类型：</td>
            <td>
                <asp:DropDownList ID="DDL_StorageType" runat="server" Width="130px">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">入库仓：</td>
            <td>
                <asp:DropDownList ID="DDL_Waerhouse" runat="server" AutoPostBack="true" Width="130px" OnSelectedIndexChanged="DDLWaerhouse_OnSelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">储位：</td>
            <td>
                <asp:DropDownList ID="DDL_StorageAuth" runat="server" AutoPostBack="true" Width="130px" OnSelectedIndexChanged="DDLStorageAuth_OnSelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">公司：</td>
            <td>
                <asp:DropDownList ID="DDL_HostingFilialeAuth" runat="server" Width="130px" AutoPostBack="true" OnSelectedIndexChanged="DDLHostingFilialeAuth_OnSelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">采购单位：</td>
            <td>
                <asp:DropDownList ID="DDL_Purchase" runat="server" Width="60px" AutoPostBack="true" OnSelectedIndexChanged="DDLPurchase_OnSelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td>
                <asp:DropDownList ID="DDL_PurchaseFiliale" runat="server" Width="130px" AutoPostBack="true" OnSelectedIndexChanged="DDL_PurchaseFiliale_OnSelectedIndexChanged"></asp:DropDownList>
            </td>
            <td style="text-align: right;">门店：</td>
            <td>
                <asp:DropDownList ID="DDL_SaleFiliale" runat="server" Width="130px">
                </asp:DropDownList>

            </td>
            <td rowspan="2">
                <asp:ImageButton ID="LB_Search" runat="server" ValidationGroup="Search" SkinID="SearchButton" OnClick="LB_Search_Click" />
                <asp:Button runat="server" ID="Lb_Reload" OnClick="Lb_Reload_Click" Style="display: none;" />
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">年份：</td>
            <td>
                <asp:DropDownList runat="server" ID="DDL_Years" AutoPostBack="true" Width="130px" OnSelectedIndexChanged="DdlYears_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">日期：</td>
            <td class="timeclass">
                <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
                <div style="padding-right: 5px; padding-top: 4px;">-</div>
                <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
            </td>
            <td style="text-align: right;">状态：</td>
            <td>
                <asp:DropDownList ID="DD_StorageState" runat="server" Width="130px">
                </asp:DropDownList>
            </td>
            <td colspan="4">
                <rad:RadTextBox runat="server" EmptyMessage="入库单号/原始单号/进货单号" ID="RTB_No" Width="197px" />
            </td>
        </tr>

        <tr>
            <td colspan="13">
                <rad:RadGrid ID="RG_StorageRecord" runat="server" OnNeedDataSource="StockGrid_NeedDataSource" OnItemCommand="SemiStockGrid_OnItemCommand"
                    OnItemDataBound="SemiStockGrid_OnItemDataBound" SkinID="CustomPaging">
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="StockId,StockType,StockState,DateCreated,LinkTradeID,TradeBothPartiesType" ClientDataKeyNames="StockId,StockType,StockState,DateCreated,LinkTradeID,TradeBothPartiesType">
                        <CommandItemTemplate>
                            &nbsp;&nbsp;&nbsp;
                   <Ibt:ImageButtonControl runat="server" ID="LinkButton1" OnClientClick="return ShowGoodsBorrowApply()"
                       SkinType="Insert"
                       Text="借入申请" Visible='<%# GetPowerOperationPoint("BorrowingApplication") %>'></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                   <Ibt:ImageButtonControl runat="server" ID="ImageButtonControl1" OnClientClick="return ShowSaleReturnIn()"
                       SkinType="Insert"
                       Text="销售退货入库" Visible='<%# GetPowerOperationPoint("SaleReturnIn") %>'></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                   <Ibt:ImageButtonControl runat="server" ID="LinkButtonAddRecord" OnClientClick="return ShowGoodsPurchaseIn()"
                       SkinType="Insert"
                       Text="采购进货入库" Visible='<%# GetPowerOperationPoint("PurchaseIn") %>'></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                   <Ibt:ImageButtonControl runat="server" ID="LinkButtonRefresh" CommandName="RebindGrid"
                       SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridTemplateColumn HeaderText="单据类型">
                                <ItemTemplate>
                                    <asp:Label ID="StockTypeLabel" runat="server" Text='<%# GetStockType(Convert.ToInt32(Eval("StockType"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="80px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="DateCreated" HeaderText="创建时间" UniqueName="DateCreated"
                                DataFormatString="{0:D}">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="TradeCode" HeaderText="入库单号" UniqueName="TradeCode">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="LinkTradeCode" HeaderText="原始编号" UniqueName="LinkTradeCode">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="CompanyId" HeaderText="采购单位" UniqueName="CompanyId">
                                <ItemTemplate>
                                    <asp:Label ID="CompanyLabel" runat="server" Text='<%# GetCompany(new Guid(Eval("ThirdCompanyID").ToString()),Convert.ToByte(Eval("StorageType"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="180px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="BillNo" HeaderText="进货单号" UniqueName="BillNo">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="StockState" HeaderText="单据状态" UniqueName="StockState">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="StockStateLabel" runat="server" Text='<%# GetStockState(Convert.ToInt32(Eval("StockState"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="备注" UniqueName="Description">
                                <ItemTemplate>
                                    <asp:Image ID="ImageButton" runat="server" SkinID="MemoImg" Visible='<%#Eval("Description")!=null && !string.IsNullOrEmpty(Eval("Description").ToString()) %>'
                                        ToolTip='<%# Eval("Description") %>' />
                                </ItemTemplate>
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="作废" UniqueName="Cancellation">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CommandName="Cancellation" CausesValidation="false"
                                        OnClientClick="return OnDeleteConfirm()" ID="IB_Delete" SkinID="DeleteImageButton" />
                                </ItemTemplate>
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="操作" UniqueName="Modify">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" ID="IB_ModifyOrder" CausesValidation="false" SkinID="EditImageButton"
                                        OnClientClick='<%# "return ShowEditStorageRecordApplyInForm(\"" + Eval("StockId") + "\","+Eval("StockType") +",0);" %>' />
                                    <asp:Button ID="btn_ModifyOrder" runat="server" Text="重送"
                                        OnClientClick='<%# "return ShowEditStorageRecordApplyInForm(\"" + Eval("StockId") + "\","+Eval("StockType") +",1);" %>' />
                                </ItemTemplate>
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审批" UniqueName="Approved">
                                <ItemTemplate>
                                    <asp:ImageButton ID="IB_Approved" runat="server" SkinID="AffirmImageButton"
                                        OnClientClick='<%# "return ShowAuditingForm(\"" + Eval("StockId") + "\","+Eval("StockType") +");" %>' />
                                </ItemTemplate>
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="打印" UniqueName="Print">
                                <ItemTemplate>
                                    <Ibt:ImageButtonControl ID="LBPrintIn" CausesValidation="false" SkinType="Print"
                                        runat="server" OnClientClick='<%# "return PrintSemiStockInDetial(\""+Eval("StockID")+"\")" %>'></Ibt:ImageButtonControl>
                                    <br />
                                    <Ibt:ImageButtonControl ID="ImageButtonControl2" CausesValidation="false" SkinType="Print"
                                        runat="server" OnClientClick='<%# "return PrintSemiStockInDetialContainPrice(\""+Eval("StockID")+"\")" %>'
                                        Text="打印-含价格"></Ibt:ImageButtonControl>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="100px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="EditGoodsLendApplyReturnForm" runat="server" OnClientShow="clientShow" Title="借出返回" />
            <rad:RadWindow ID="BorrowForm" runat="server" OnClientShow="clientShow" Title="借入申请" />
            <rad:RadWindow ID="GoodsPurchaseInForm" runat="server" Title="采购进货入库" Height="580" Width="900" />
            <rad:RadWindow ID="SaleReturnInForm" runat="server" Title="销售退货入库" Height="540" Width="900" />
            <rad:RadWindow ID="GoodsLendApplyReturnForm" runat="server" Title="借出返回" Height="540" Width="900" />
            <rad:RadWindow ID="PrintSemiStockIn" runat="server" Title="打印入库单据" Height="540" Width="900" />
            <rad:RadWindow ID="PrintSemiStockInContainPrice" runat="server" Title="打印入库单据-含价格" Height="540" Width="900" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="DDL_Years">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RDP_EndTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_StorageRecord" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_Waerhouse">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_StorageRecord" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_StorageAuth" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_HostingFilialeAuth" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="LB_Search" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_SaleFiliale" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_StorageAuth">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DDL_HostingFilialeAuth" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="LB_Search" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_SaleFiliale" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RG_StorageRecord" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_HostingFilialeAuth">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LB_Search" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_SaleFiliale" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RG_StorageRecord" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_Purchase">
                <UpdatedControls>
                     <rad:AjaxUpdatedControl ControlID="DDL_PurchaseFiliale" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_PurchaseFiliale">
                <UpdatedControls>
                     <rad:AjaxUpdatedControl ControlID="DDL_SaleFiliale" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>

    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
        });

        function RowDblClick(obj, args) {
            var stockId = args.getDataKeyValue("StockId");
            var stockType = args.getDataKeyValue("StockType");

            if (stockType == 1) {
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

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='Lb_Reload']").click();
        }

    </script>
</asp:Content>

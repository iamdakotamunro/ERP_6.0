<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="StorageRecordApplyOut.aspx.cs" Inherits="ERP.UI.Web.StorageRecordApplyOut" %>

<%@ Import Namespace="ERP.Enum" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <script src="My97DatePicker/WdatePicker.js"></script>
    <script src="JavaScript/ToolTipMsg.js"></script>
    <script src="JavaScript/GiveTip.js"></script>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowGoodsLendApply() {
                //借出申请 
                window.radopen("./Windows/GoodsLendApplyForm.aspx", "LendForm");
                return false;
            }

            function ShowDefectiveReturnOut() {
                //售后退货出库
                window.radopen("./Windows/DefectiveReturnOutForm.aspx", "DefectiveReturnOutForm");
                return false;
            }

            function ShowPurchaseReturnOut() {
                //采购退货出库
                window.radopen("./Windows/PurchaseReturnOutForm.aspx", "PurchaseReturnOutForm");
                return false;
            }

            //function ShowAllocationOut() {
            //    //调拨出库
            //    //window.radopen("./Windows/AllocationOutForm.aspx", "AllocationOutForm");
            //    return false;
            //}

            function ShowInnerPurchase() {
                //内部采购(仓仓)
                window.radopen("./Windows/InnerPurchaseForm.aspx", "InnerPurchaseForm");
                return false;
            }

            function ShowGoodsSaleOut() {
                //销售出库
                window.radopen("./Windows/GoodsSaleOutForm.aspx", "GoodsSaleOutForm");
                return false;
            }

            function clientShow(sender, eventArgs) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }


            function OnDeleteConfirm() {
                var conf = window.confirm('提示：是否确认作废吗？');
                if (!conf)
                    return false;
                return true;
            }

            //编辑
            function ShowEditStorageRecordApplyOutForm(stockId, stockType, type, linkTradeType) {
                if (stockType == 8) {
                    //借出申请
                    window.radopen("./Windows/EditGoodsLendApplyForm.aspx?IsAgain=" + type + "&StockId=" + stockId, "LendForm");
                } else if (stockType == 6) {
                    //售后退货出库 
                    window.radopen("./Windows/EditDefectiveReturnOutForm.aspx?IsAgain=" + type + "&StockId=" + stockId, "DefectiveReturnOutForm");
                } else if (stockType == 5) {
                    //采购退货出库
                    window.radopen("./Windows/EditPurchaseReturnOutForm.aspx?IsAgain=" + type + "&StockId=" + stockId, "PurchaseReturnOutForm");
                } else if (stockType == 10) {
                    //内部采购(仓仓)
                    window.radopen("./Windows/EditInnerPurchaseForm.aspx?IsAgain=" + type + "&StockId=" + stockId, "EditInnerPurchaseForm");
                } else if (stockType == 7) {
                    if (linkTradeType == 4)
                        //销售出库
                        window.radopen("./Windows/EditGoodsSaleOutForm.aspx?IsAgain=" + type + "&StockId=" + stockId + "&TradeType=" + linkTradeType, "ShopApplyForm");
                    else
                        window.radopen("./Windows/EditGoodsSaleOutForm.aspx?IsAgain=" + type + "&StockId=" + stockId + "&TradeType=" + linkTradeType, "GoodsSaleOutForm");
                } else if (stockType == 9) {
                    //借入返回(重送)
                    window.radopen("./Windows/EditGoodsBorrowApplyReturnForm.aspx?StockId=" + stockId, "EditGoodsBorrowApplyReturnForm");
                }
                return false;
            }

            //审批
            function ShowAuditingForm(stockId, stockType, linkTradeType) {
                if (stockType == 8) {
                    //借出申请
                    window.radopen("./Windows/ApprovalGoodsLendApplyForm.aspx?StockId=" + stockId, "LendForm");
                } else if (stockType == 6) {
                    //售后退货出库 
                    window.radopen("./Windows/ApprovalDefectiveReturnOutForm.aspx?StockId=" + stockId, "DefectiveReturnOutForm");
                } else if (stockType == 5) {
                    //采购退货出库
                    window.radopen("./Windows/ApprovalPurchaseReturnOutForm.aspx?StockId=" + stockId, "ApprovalPurchaseReturnOutForm");
                }
                else if (stockType == 10) {
                    //内部采购(仓仓)
                    window.radopen("./Windows/ApprovalInnerPurchaseForm.aspx?StockId=" + stockId, "ApprovalInnerPurchaseForm");
                }
                else if (stockType == 7) {
                    if (linkTradeType == 4) {
                        window.radopen("./Windows/ApprovalGoodsSaleOutForm.aspx?StockId=" + stockId + "&TradeType=" + linkTradeType, "ShopApplyForm");
                    } else {
                        //销售出库
                        window.radopen("./Windows/ApprovalGoodsSaleOutForm.aspx?StockId=" + stockId, "GoodsSaleOutForm");
                    }
                } else if (stockType == 9) {
                    //借入返回
                    window.radopen("./Windows/ApprovalGoodsBorrowApplyReturnForm.aspx?StockId=" + stockId, "GoodsBorrowApplyReturnForm");
                }
                return false;
            }


            //打印
            function PrintSemiStockOutDetial(stockId) {
                window.radopen("./Windows/PrintStorageRecordApplyOutDetail.aspx?StockId=" + stockId + "&IsPrintPrice=0", "PrintSemiStockOut");
                return false;
            };

            //打印-含价格 
            function PrintSemiStockOutDetialContainPrice(stockId) {
                window.radopen("./Windows/PrintStorageRecordApplyOutDetail.aspx?StockId=" + stockId + "&IsPrintPrice=1", "PrintSemiStockOutContainPrice");
                return false;
            };

            function CheckStockValidation(obj) {
                var cbid = obj.id;
                var btnid = cbid.replace("CB_StockValidation", "Btn_StockValidation");
                if (obj.checked) {
                    if (confirm('提示：是否更改为待确认？')) {
                        document.getElementById(btnid).click();
                        return true;
                    }
                    else
                        obj.checked = false;
                }
                else {
                    if (confirm('提示：是否取消待确认？')) {
                        document.getElementById(btnid).click();
                        return true;
                    }
                    else
                        obj.checked = true;
                }
                return false;
            }
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
            <td style="text-align: right;">出库仓：
            </td>
            <td>
                <asp:DropDownList ID="DDL_Waerhouse" runat="server" AutoPostBack="true" Width="130px" OnSelectedIndexChanged="DDLWaerhouse_OnSelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">储位：
            </td>
            <td>
                <asp:DropDownList ID="DDL_StorageAuth" runat="server" AutoPostBack="true" Width="130px" OnSelectedIndexChanged="DDLStorageAuth_OnSelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">公司：
            </td>
            <td>
                <asp:DropDownList ID="DDL_HostingFilialeAuth" runat="server" Width="130px" AutoPostBack="true" OnSelectedIndexChanged="DDLHostingFilialeAuth_OnSelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">门店：
            </td>
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
            <td style="text-align: right;">年份：
            </td>
            <td>
                <asp:DropDownList runat="server" ID="DDL_Years" AutoPostBack="true" Width="130px" OnSelectedIndexChanged="DdlYears_SelectedIndexChanged">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">日期：
            </td>
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
            <td colspan="2">
                <rad:RadTextBox runat="server" EmptyMessage="出库单号/原始单号/出货单号" ID="RTB_No" Width="197px" />

            </td>
            <td style="text-align: right;">往来单位：
            </td>
            <td>
                <rad:RadComboBox ID="RcbThirdCompany" runat="server" CausesValidation="false" AutoPostBack="true"
                        AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="Value" DataValueField="Key"
                        OnItemsRequested="RcbThirdCompany_ItemsRequested" Width="130px" Height="200px">
                    </rad:RadComboBox>
            </td>
        </tr>

        <tr>
            <td colspan="13">
                <rad:RadGrid ID="RG_StorageRecord" runat="server" OnNeedDataSource="StockGrid_NeedDataSource" OnItemCommand="SemiStockGrid_OnItemCommand" SkinID="CustomPaging"
                    OnItemDataBound="SemiStockGrid_OnItemDataBound">
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="StockId,StockType,StockState,DateCreated,LinkTradeID,LinkTradeType,ThirdCompanyID" ClientDataKeyNames="StockId,StockType,StockState,DateCreated,LinkTradeID,LinkTradeType">
                        <CommandItemTemplate>
                            &nbsp;&nbsp;&nbsp;
                   <Ibt:ImageButtonControl runat="server" ID="LinkButton1" OnClientClick="return ShowGoodsLendApply()"
                       SkinType="Insert"
                       Text="借出申请" Visible='<%# GetPowerOperationPoint("LoanApplication") %>'></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                   <Ibt:ImageButtonControl runat="server" ID="ImageButtonControl1" OnClientClick="return ShowDefectiveReturnOut()"
                       SkinType="Insert"
                       Text="售后退货出库" Visible='<%# GetPowerOperationPoint("AfterSaleOut") %>'></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                   <Ibt:ImageButtonControl runat="server" ID="LinkButtonAddRecord" OnClientClick="return ShowPurchaseReturnOut()"
                       SkinType="Insert"
                       Text="采购退货出库" Visible='<%# GetPowerOperationPoint("PurchaseReturn") %>'></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                   <%--<Ibt:ImageButtonControl runat="server" ID="ImageButtonControl2" OnClientClick="return ShowAllocationOut()"
                       SkinType="Insert"
                       Text="调拨出库" Visible='<%# GetPowerOperationPoint("AllocationOut") %>'></Ibt:ImageButtonControl>--%>
                            <Ibt:ImageButtonControl runat="server" ID="ImageButtonControl2" OnClientClick="return ShowInnerPurchase()"
                                SkinType="Insert"
                                Text="内部采购" Visible='<%# GetPowerOperationPoint("InnerPurchase") %>'></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                   <Ibt:ImageButtonControl runat="server" ID="ImageButtonControl3" OnClientClick="return ShowGoodsSaleOut()"
                       SkinType="Insert"
                       Text="销售出库" Visible='<%# GetPowerOperationPoint("SaleOut") %>'></Ibt:ImageButtonControl>
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
                            <rad:GridBoundColumn DataField="TradeCode" HeaderText="出库单号" UniqueName="TradeCode">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="LinkTradeCode" HeaderText="原始单号" UniqueName="LinkTradeCode">
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

                            <rad:GridBoundColumn DataField="BillNo" HeaderText="出货单号" UniqueName="BillNo">
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
                                        OnClientClick='<%# "return ShowEditStorageRecordApplyOutForm(\"" + Eval("StockId") + "\","+Eval("StockType") +",0,"+Eval("LinkTradeType") +");" %>' />
                                    <asp:Button ID="btn_ModifyOrder" runat="server" Text="重送" CommandName='<%#Eval("BillNo") %>'
                                        OnClientClick='<%# "return ShowEditStorageRecordApplyOutForm(\"" + Eval("StockId") + "\","+Eval("StockType") +",1,"+Eval("LinkTradeType") +");" %>' />
                                </ItemTemplate>
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>

                            <rad:GridTemplateColumn HeaderText="售后确认" UniqueName="StockValidation">
                                <ItemTemplate>
                                    <asp:CheckBox ID="CB_StockValidation" Checked='<%# Convert.ToInt32(Eval("StockValidation"))==1  %>'
                                        onclick="CheckStockValidation(this);" Visible='<%# (Convert.ToInt32(Eval("StockType"))==(int)StorageRecordType.AfterSaleOut && (Convert.ToInt32(Eval("StockState"))==(int)StorageRecordState.WaitAudit))||(Convert.ToInt32(Eval("StockType"))==(int)StorageRecordType.BuyStockOut  && GetPowerOperationPoint("RecentInPrice"))%>'
                                        Enabled='<%# (Convert.ToInt32(Eval("StockType")) ==(int)StorageRecordType.BuyStockOut) || !(Convert.ToInt32(Eval("StockType"))==(int)StorageRecordType.AfterSaleOut && Convert.ToInt32(Eval("StockValidation"))==1) %>' runat="server" />
                                    <asp:Button ID="Btn_StockValidation" CommandName="Validation" CommandArgument='<%# Eval("StockValidation") %>'
                                        Text="待确认" Style="display: none;" runat="server" />
                                </ItemTemplate>
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审批" UniqueName="Approved">
                                <ItemTemplate>
                                    <asp:ImageButton ID="IB_Approved" runat="server" SkinID="AffirmImageButton"
                                        OnClientClick='<%# "return ShowAuditingForm(\"" + Eval("StockId") + "\","+Eval("StockType") +","+Eval("LinkTradeType") +");" %>' />
                                </ItemTemplate>
                                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="打印" UniqueName="Print">
                                <ItemTemplate>
                                    <Ibt:ImageButtonControl ID="LBPrintOut" CausesValidation="false" SkinType="Print"
                                        runat="server" OnClientClick='<%# "return PrintSemiStockOutDetial(\""+Eval("StockID")+"\")" %>'></Ibt:ImageButtonControl>
                                    <br />
                                    <Ibt:ImageButtonControl ID="ImageButtonControl3" CausesValidation="false" SkinType="Print"
                                        runat="server" OnClientClick='<%# "return PrintSemiStockOutDetialContainPrice(\""+Eval("StockID")+"\")" %>'
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
            <rad:RadWindow ID="EditGoodsBorrowApplyReturnForm" runat="server" OnClientShow="clientShow" Title="借入返回" />
            <rad:RadWindow ID="LendForm" runat="server" OnClientShow="clientShow" Title="借出申请" />
            <rad:RadWindow ID="DefectiveReturnOutForm" runat="server" Title="售后退货出库" Height="540" Width="900" />
            <rad:RadWindow ID="ApprovalPurchaseReturnOutForm" runat="server" Title="采购退货出库" Height="580" Width="900" />
            <rad:RadWindow ID="PurchaseReturnOutForm" runat="server" Title="采购退货出库" Height="540" Width="900" />
            <rad:RadWindow ID="AllocationOutForm" runat="server" Title="调拨出库" Height="540" Width="900" />
            <rad:RadWindow ID="InnerPurchaseForm" runat="server" Title="内部采购" Height="540" Width="900" />
            <rad:RadWindow ID="EditInnerPurchaseForm" runat="server" Title="内部采购" Height="540" Width="900" />
            <rad:RadWindow ID="ApprovalInnerPurchaseForm" runat="server" Title="内部采购" Height="540" Width="900" />
            <rad:RadWindow ID="GoodsBorrowApplyReturnForm" runat="server" Title="借入返回" Height="540" Width="900" />
            <rad:RadWindow ID="GoodsSaleOutForm" runat="server" Title="销售出库" Height="540" Width="900" />
            <rad:RadWindow ID="ShopApplyForm" runat="server" Title="门店要货申请出库" Height="540" Width="900" />
            <rad:RadWindow ID="PrintSemiStockOut" runat="server" Title="打印出库单据" Height="540" Width="900" />
            <rad:RadWindow ID="PrintSemiStockOutContainPrice" runat="server" Title="打印出库单据-含价格" Height="540" Width="900" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false" OnAjaxRequest="RamAjaxRequest">
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
                    <rad:AjaxUpdatedControl ControlID="LB_Search" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_StorageAuth" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_HostingFilialeAuth" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_SaleFiliale" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbThirdCompany">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RcbThirdCompany" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_StorageAuth">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_StorageRecord" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="LB_Search" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_HostingFilialeAuth" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="DDL_SaleFiliale" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_HostingFilialeAuth">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_StorageRecord" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="LB_Search" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
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
            var linkTradeType = args.getDataKeyValue("LinkTradeType");
            if (stockType == 8) {
                //借出申请
                window.radopen("./Windows/ApprovalGoodsLendApplyForm.aspx?IsSel=1&StockId=" + stockId, "LendForm");
            } else if (stockType == 6) {
                //售后退货出库 
                window.radopen("./Windows/ApprovalDefectiveReturnOutForm.aspx?IsSel=1&StockId=" + stockId, "DefectiveReturnOutForm");
            } else if (stockType == 5) {
                //采购退货出库
                window.radopen("./Windows/ApprovalPurchaseReturnOutForm.aspx?IsSel=1&StockId=" + stockId, "ApprovalPurchaseReturnOutForm");
            }
            else if (stockType == 10) {
                //内部采购
                window.radopen("./Windows/ApprovalInnerPurchaseForm.aspx?IsSel=1&StockId=" + stockId, "ApprovalInnerPurchaseForm");
            }
            else if (stockType == 7) {
                if (linkTradeType == 4) {
                    window.radopen("./Windows/ApprovalGoodsSaleOutForm.aspx?IsSel=1&StockId=" + stockId + "&TradeType=" + linkTradeType, "ShopApplyForm");
                } else {
                    //销售出库
                    window.radopen("./Windows/ApprovalGoodsSaleOutForm.aspx?IsSel=1&StockId=" + stockId + "&TradeType=" + linkTradeType, "GoodsSaleOutForm");
                }
            } else if (stockType == 9) {
                //借入返回
                window.radopen("./Windows/ApprovalGoodsBorrowApplyReturnForm.aspx?IsSel=1&StockId=" + stockId, "GoodsBorrowApplyReturnForm");
            }
            return false;
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='Lb_Reload']").click();
        }

    </script>
</asp:Content>

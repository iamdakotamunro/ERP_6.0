<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="ShopFrontPurchase.aspx.cs" Inherits="ERP.UI.Web.ShopFrontPurchase"
    Title="无标题页" %>
<%@ Import Namespace="ERP.Enum.ShopFront" %>
<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script type="text/javascript" language="javascript">
            //申请出库
            function ShowApplyForm(applyId) {
                window.radopen("Windows/ShopFrontApplyStockOut.aspx?applyid=" + applyId, "ApplyStockOut");
                return false;
            }

            //需确认发货时 明细窗口
            function  ShopConfirmForm(applyId) {
                window.radopen("Windows/ShopConfirmApplyDetailForm.aspx?ApplyId=" + applyId, "ConfirmForm");
                return false;
            }

            //查看采购明细
            function ShowApplyDetailForm(applyId, isAudit) {
                window.radopen("Windows/ShowApplyStockDetailForm.aspx?ApplyId=" + applyId + "&IsAudit=" + isAudit, "RwApplyDetail");
                return false;
            }

            //查看物流
            function ShowLogisticalForm(applyId,applyNo) {
                window.radopen("Windows/ShowLogisticalDetailForm.aspx?Type=0&ApplyId=" + applyId+"&ApplyNo="+applyNo, "RwApplyDetail");
                return false;
            }

            function clientShow(sender) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }

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
            <td align="right">
                单号：
            </td>
            <td>
                <rad:RadTextBox runat="server" ID="RtbSearchKey" ToolTip="申请单号，出库单据号..." Width="150px" EmptyMessage="申请单号，出库单据号..."></rad:RadTextBox>
            </td>
            <td align="right">
                商品名称：
            </td>
            <td>
                <rad:RadTextBox runat="server" ID="RtbGoodsName" ToolTip="商品名称..." Width="250px"></rad:RadTextBox>
            </td>
            <td align="right">
                下单时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RdpStartTime" runat="server" SkinID="Common" Width="100px" ></rad:RadDatePicker>
                 - <rad:RadDatePicker ID="RdpEndTime" runat="server" SkinID="Common" Width="100px" ></rad:RadDatePicker>
            </td>
            <td align="right">
                要货类型：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbPurchaseType">
                    <Items>
                        <rad:RadComboBoxItem Text="全部状态" Value="0" />
                        <rad:RadComboBoxItem Text="订单类型" Value="1" />
                        <rad:RadComboBoxItem Text="要货类型" Value="2" />
                    </Items>
                </rad:RadComboBox>
            </td>
        </tr>
        <tr>
            <td align="right">
                店铺类型：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbShopType" OnSelectedIndexChanged="RcbShopTypeIndexChanged" AutoPostBack="True">
                    <Items>
                        <rad:RadComboBoxItem Text="-- 请选择店铺类型 -- " Value="0" />
                        <rad:RadComboBoxItem Text="联盟店" Value="3" />
                        <rad:RadComboBoxItem Text="加盟店" Value="2" />
                        <rad:RadComboBoxItem Text="直营店" Value="1" />
                    </Items>
                </rad:RadComboBox>
            </td>
            <td align="right">
                要货门店：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbShopFrontList" DataTextField="Value" DataValueField="Key"
                 CausesValidation="False" AllowCustomText="True" EnableLoadOnDemand="True" 
                 OnItemsRequested="RcbShopFrontListItemsRequested"></rad:RadComboBox>
            </td>
            <td align="right">
                申请状态：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbApplyState" DataTextField="Value" DataValueField="Key">
                </rad:RadComboBox>
            </td>
            <td colspan="2" align="center">
                <asp:Button runat="server" ID="BtnSearch" Text="搜索" OnClick="BtnSearchClick" Width="150px"/>
            </td>
        </tr>
    </table>
    <rad:RadGrid runat="server" ID="RgApplyStockList" OnNeedDataSource="RgApplyStockListNeedDataSource" 
    OnItemDataBound="RgApplyStockListItemDataBound">
        <MasterTableView DataKeyNames="ApplyId,FilialeId,SemiStockCode" CommandItemSettings-ShowAddNewRecordButton="false">
            <Columns>
                <rad:GridBoundColumn DataField="TradeCode" ReadOnly="true" HeaderText="申请单号">
                    <HeaderStyle HorizontalAlign="Center" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="SemiStockCode" ReadOnly="true" HeaderText="出库单据号">
                    <HeaderStyle HorizontalAlign="Center" Width="360" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="SubtotalQuantity" ReadOnly="true" HeaderText="数量">
                    <HeaderStyle HorizontalAlign="Center" Width="60" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="FilialeName" ReadOnly="true" HeaderText="申请门店">
                    <ItemStyle HorizontalAlign="Center" Width="120" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Transactor" ReadOnly="true" HeaderText="申请要货者">
                    <ItemStyle HorizontalAlign="Center" Width="120" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="WarehouseName" ReadOnly="true" HeaderText="申请仓库">
                    <HeaderStyle HorizontalAlign="Center" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="PurchaseType" ReadOnly="true" HeaderText="要货类型">
                    <HeaderStyle HorizontalAlign="Center" Width="80" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%# GetPurchaseTypeName(Eval("PurchaseType"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="StockState" ReadOnly="true" HeaderText="状态">
                    <HeaderStyle HorizontalAlign="Center" Width="80" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%# ReturnApplyState(Eval("StockState"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <HeaderStyle HorizontalAlign="Center" Width="120" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                         <asp:LinkButton runat="server" ID="LinkButton2" Text="要货作废" 
                        Visible='<%# ((int)Eval("StockState")==(int)ApplyStockState.Obligation || 
                        (int)Eval("StockState")==(int)ApplyStockState.Confirming ||
                         (int)Eval("StockState")==(int)ApplyStockState.Delivering) && IsAllowCancel(Eval("SemiStockCode").ToString()) %>'
                        OnClientClick="return confirm('确定要作废该申请?')" OnClick="ApplyStockCacel" 
                        CommandArgument='<%# Eval("ApplyId") %>'></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="LbCommit" Text="要货确认" 
                        Visible='<%# (int)Eval("StockState")==(int)ApplyStockState.Confirming %>'
                        OnClientClick='<%# ShopConfirmFormJs(Eval("ApplyId")) %>' 
                        CommandArgument='<%# Eval("ApplyId") %>'></asp:LinkButton>
                        <Ibt:ImageButtonControl runat="server" CausesValidation="false" ID="IBC_StockOut"
                            OnClientClick='<%# ReturnApplyStockJs(Eval("ApplyId"),Eval("FilialeId")) %>' 
                            Visible='<%# ((int)Eval("StockState")==(int)ApplyStockState.Applying
                            ||(int)Eval("StockState")==(int)ApplyStockState.PartFinish || 
                            (int)Eval("StockState")==(int)ApplyStockState.Delivering) && IsAlliance(Eval("FilialeId"),Eval("SemiStockCode")) %>'
                            Text="申请出库" SkinType="Edit"></Ibt:ImageButtonControl>
                        <asp:LinkButton runat="server"  ID="IbApproved" Text="审核"
                                Visible='<%# (int)Eval("StockState")==(int)ApplyStockState.CheckPending  %>'
                                 OnClientClick='<%# "return ShowApplyDetailForm(\"" + Eval("ApplyId")  + "\",1);" %>'/>
                        <asp:LinkButton runat="server" ID="LinkButton1" Text="要货详情" 
                        Visible='<%# (int)Eval("StockState")>=(int)ApplyStockState.Obligation 
                        && (int)Eval("StockState")>=(int)ApplyStockState.Cancel %>' 
                        OnClientClick='<%# ShowApplyStockJs(Eval("ApplyId"),0) %>'></asp:LinkButton>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="TransferWindowManager" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="ApplyStockOut" runat="server" Title="门店调拨申请处理" OnClientShow="clientShow" />
            <rad:RadWindow ID="RwApplyDetail" runat="server" Title="要货单详情"  Width="720" Height="400"/>
            <rad:RadWindow ID="ConfirmForm" runat="server" Title="要货确认" OnClientShow="clientShow" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgApplyStockList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BtnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgApplyStockList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RgApplyStockList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgApplyStockList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbShopType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RcbShopFrontList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

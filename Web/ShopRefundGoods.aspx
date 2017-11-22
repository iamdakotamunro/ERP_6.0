<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" 
CodeBehind="ShopRefundGoods.aspx.cs" Inherits="ERP.UI.Web.ShopRefundGoods" %>
<%@ Import Namespace="ERP.Enum.ShopFront" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script type="text/javascript" language="javascript">
            //显示退货留言窗口
            function ShowMsgForm() {
                window.radopen("./Windows/ShopRefundMsgForm.aspx", "ShowMsgForm");
                return false;
            }

            //显示退货详情
            function ShowRefundForm(applyId,isCheck) {
                window.radopen("./Windows/ShowExchangedDetailForm.aspx?ApplyId=" + applyId + "&Type=1"+"&IsCheck="+isCheck, "RefundDetailForm");
                return false;
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
                商品搜索：
            </td>
            <td>
                <rad:RadTextBox runat="server" ID="RtbSearchKey" Width="250px" EmptyMessage="商品名/编号"/>
            </td>
            <td align="right">
                退货单号：
            </td>
            <td>
                <rad:RadTextBox runat="server" ID="RtbApplyNo" Width="155px"></rad:RadTextBox>
            </td>
            <td align="right">
                退货时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RdpStartTime" runat="server" SkinID="Common" Width="120px" ></rad:RadDatePicker>
                 - <rad:RadDatePicker ID="RdpEndTime" runat="server" SkinID="Common" Width="120px" ></rad:RadDatePicker>
            </td>
            <td>
                <asp:Button runat="server" ID="BtnReturnMsg" Text="退货留言"  Width="100px" OnClick="BtnReturnMsgOnClick"/>
            </td>
        </tr>
        <tr>
            <td align="right">
                店铺名称：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbShopList" DataTextField="Value" DataValueField="Key" Width="180px"
                CausesValidation="False" AllowCustomText="True" EnableLoadOnDemand="True" OnItemsRequested="RcbShopListItemsRequested"
                 EmptyMessage="店铺模糊搜索" AutoPostBack="True" >
                </rad:RadComboBox>
            </td>
            <td align="right">
                退货状态：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbApplyState" DataTextField="Value" DataValueField="Key">
                </rad:RadComboBox>
            </td>
            <td></td>
            <td colspan="3" align="center">
                <asp:Button runat="server" ID="BtnSearch" Text="搜索"  Width="80px" OnClick="BtnSearchOnClick"/>
            </td>
        </tr>
    </table>
    <rad:RadGrid runat="server" ID="RgRefundApplyList" OnNeedDataSource="RgRefundApplyListNeedDataSource" 
    OnItemDataBound="RgRefundApplyListItemDataBound">
        <MasterTableView DataKeyNames="ApplyID,ShopID" CommandItemSettings-ShowAddNewRecordButton="false" CommandItemDisplay="None">
            <Columns>
                <rad:GridBoundColumn DataField="ApplyNo" ReadOnly="true" HeaderText="退货单号">
                    <HeaderStyle HorizontalAlign="Center" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ShopName" ReadOnly="true" HeaderText="退货店铺">
                    <HeaderStyle HorizontalAlign="Center" Width="160" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ApplyTime" ReadOnly="true" HeaderText="退货时间">
                    <HeaderStyle HorizontalAlign="Center" Width="120" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="SubQuantity" ReadOnly="true" HeaderText="退货数量">
                    <ItemStyle HorizontalAlign="Center" Width="80" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="SubPrice" ReadOnly="true" HeaderText="退货总金额">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("SubPrice"))%>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="退货留言" UniqueName="Clew">
                <ItemTemplate>
                    <asp:ImageButton ID="ClewImageButton" runat="server" SkinID="InsertImageButton"
                        ToolTip='<%# Eval("MsgContent").ToString() %>' />
                </ItemTemplate>
                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="StockState" ReadOnly="true" HeaderText="状态">
                    <HeaderStyle HorizontalAlign="Center" Width="60" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%# ApplyStateList[Convert.ToInt32(Eval("ExchangedState"))]%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <HeaderStyle HorizontalAlign="Center" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="LbAudit" Text="审核"
                        Visible='<%# (int)Eval("ExchangedState")==(int)ExchangedState.CheckPending 
                        %>' OnClientClick='<%# "return ShowRefundForm(\"" + Eval("ApplyID")  + "\",0);" %>'></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="LinkButton1" Text="处理" 
                        OnClientClick='<%# ShowRefundDetailJs(Eval("ApplyID"),1) %>'
                        Visible='<%# false %>'></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="LbShowDetail" Text="退货详情" 
                        Visible='<%# (int)Eval("ExchangedState")!=(int)ExchangedState.Cancel %>'
                        OnClientClick='<%# ShowRefundDetailJs(Eval("ApplyID"),0) %>' ></asp:LinkButton>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="ApplyWindowManager" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="AddRefundForm" runat="server" Title="添加退货申请" Width="700" Height="400" />
            <rad:RadWindow ID="ShowMsgForm" runat="server" Title="退货留言" Width="800" Height="400" />
            <rad:RadWindow runat="server" ID="RefundDetailForm" Title="退货详情" Width="700px" Height="400px"></rad:RadWindow>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server"  OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgRefundApplyList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BtnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgRefundApplyList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="BtnReturnMsg"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbShopList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RcbShopList"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RgRefundApplyList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgRefundApplyList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

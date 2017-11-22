<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" 
CodeBehind="ShopBarterGoods.aspx.cs" Inherits="ERP.UI.Web.ShopBarterGoods" %>
<%@ Import Namespace="ERP.Enum.ShopFront" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script type="text/javascript" language="javascript">
            //显示换货详情
            function ShowBarterForm(applyId,isCheck) {
                window.radopen("./Windows/ShowExchangedDetailForm.aspx?ApplyId=" + applyId + "&Type=0"+"&IsCheck="+isCheck, "BarterDetailForm");
                return false;
            }

            //填写快递信息
            function ShowExpressForm(applyId) {
                window.radopen("./Windows/AddLogisticalForm.aspx?ApplyId=" + applyId, "BarterDetailForm");
                return false;
            }

            //商品检查
            function  ShopCheckForm(applyId,shopId) {
                window.radopen("./Windows/ShopReturnGoodsCheckForm.aspx?ApplyId=" + applyId + "&ShopID=" + shopId, "RwCheckForm");
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
                <rad:RadTextBox runat="server" ID="RtbSearchKey" ToolTip="商品名称/商品编号..." Width="250px" 
                EmptyMessage="商品名称，商品编号..."></rad:RadTextBox>
            </td>
            <td align="right">
                换货单号：
            </td>
            <td>
                <rad:RadTextBox runat="server" ID="RtbApplyNo" ToolTip="换货单号..." Width="155px"></rad:RadTextBox>
            </td>
            <td align="right">
                换货时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RdpStartTime" runat="server" SkinID="Common" Width="100px" ></rad:RadDatePicker>
                 - <rad:RadDatePicker ID="RdpEndTime" runat="server" SkinID="Common" Width="100px" ></rad:RadDatePicker>
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
                换货状态：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbApplyState" DataTextField="Value" DataValueField="Key">
                </rad:RadComboBox>
            </td>
            <td></td>
            <td colspan="2" align="center">
                <asp:Button runat="server" ID="BtnSearch" Text="搜索"  Width="80px" OnClick="BtnSearchOnClick"/>&nbsp;&nbsp;&nbsp;&nbsp;
            </td>
        </tr>
    </table>
    <rad:RadGrid runat="server" ID="RgExchangedApplyList" OnNeedDataSource="RgExchangedApplyListNeedDataSource" 
    OnItemDataBound="RgExchangedApplyListItemDataBound">
        <MasterTableView DataKeyNames="ApplyID,ShopID" CommandItemSettings-ShowAddNewRecordButton="false">
            <Columns>
                <rad:GridBoundColumn DataField="ApplyNo" ReadOnly="true" HeaderText="换货单号">
                    <HeaderStyle HorizontalAlign="Center" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ShopName" ReadOnly="true" HeaderText="换货店铺">
                    <HeaderStyle HorizontalAlign="Center" Width="160" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ApplyTime" ReadOnly="true" HeaderText="换货时间">
                    <HeaderStyle HorizontalAlign="Center" Width="120" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="SubQuantity" ReadOnly="true" HeaderText="换货数量">
                    <ItemStyle HorizontalAlign="Center" Width="80" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="SubPrice" ReadOnly="true" HeaderText="换货总金额">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("SubPrice"))%>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100" />
                    <HeaderStyle HorizontalAlign="Center" />
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
                        OnClientClick='<%# "return ShowBarterForm(\"" + Eval("ApplyID")  + "\",1);" %>'
                        Visible='<%# (int)Eval("ExchangedState")==(int)ExchangedState.CheckPending %>'></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="LinkButton1" Text="处理" 
                        OnClientClick='<%# "return ShowBarterForm(\"" + Eval("ApplyID")  + "\",1);" %>'
                        Visible='<%# false %>'></asp:LinkButton>
                        <asp:LinkButton runat="server" ID="LbShowDetail" Text="换货详情" 
                        Visible='<%# 
                         (int)Eval("ExchangedState")!=(int)ExchangedState.Cancel %>'
                         OnClientClick='<%# "return ShowBarterForm(\"" + Eval("ApplyID")  + "\",0);" %>'></asp:LinkButton>
<%--                         OnClientClick='<%# ShowBarterFormJs(Eval("ApplyID"),0) %>'--%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="ApplyWindowManager" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="AddExchanged" runat="server" Title="添加换货申请" Width="900px" Height="500px" />
            <rad:RadWindow runat="server" ID="BarterDetailForm" Title="换货详情" Width="700px" Height="400px"></rad:RadWindow>
            <rad:RadWindow runat="server" ID="RwCheckForm" Title="换货商品检查" Width="700px" Height="400px"></rad:RadWindow>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgExchangedApplyList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BtnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgExchangedApplyList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbShopList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RcbShopList"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BtnAddApply">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgExchangedApplyList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RgExchangedApplyList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgExchangedApplyList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

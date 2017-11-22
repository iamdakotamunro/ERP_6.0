<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.OrderHalfHourChartAw" Title="" CodeBehind="OrderHalfHourChart.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <div class="StagePanel">
        <table class="StagePanelHead" border="0" style="width: 100%;">
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table align="center">
            <tr>
                <td class="ShortFromRowTitle">
                    公司：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" AllowCustomText="True"
                        Width="150px" Height="100px" CausesValidation="False" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged" AutoPostBack="True">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    销售平台：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false"
                        AllowCustomText="True" Width="150px" Height="100px" OnSelectedIndexChanged="RCB_SalePlatform_OnSelectedIndexChanged"
                        AutoPostBack="True" CausesValidation="False">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    起止日期：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadDatePicker EnableViewState="false" ID="RDP_StartDate" runat="server" Width="95px">
                    </rad:RadDatePicker>
                    <asp:RequiredFieldValidator ID="RFVStartTime" runat="server" ErrorMessage="*" ControlToValidate="RDP_StartDate"></asp:RequiredFieldValidator>
                </td>
                <td class="ShortFromRowTitle">
                    订单状态：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBoxOrderState" runat="server" AutoPostBack="false" Height="180"
                        Width="150px">
                        <Items>
                            <rad:RadComboBoxItem ID="RadComboBoxItem8" runat="server" Text="正常状态订单" Value="-1" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem7" runat="server" Text="未经审核" Value="0" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem6" runat="server" Text="待确认" Value="1" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem5" runat="server" Text="审核通过" Value="2" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem4" runat="server" Text="等待付款" Value="3" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem9" runat="server" Text="配货中" Value="4" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem3" runat="server" Text="打印完成" Value="5" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem2" runat="server" Text="需调拨" Value="6" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem1" runat="server" Text="配货完成" Value="7" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem13" runat="server" Text="等待发货" Value="8" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem12" runat="server" Text="完成发货" Value="9" />
                            <%--<rad:RadComboBoxItem ID="RadComboBoxItem11" runat="server" Text="取消订单" Value="10" />--%>
                            <%--<rad:RadComboBoxItem ID="RadComboBoxItem10" runat="server" Text="退款审核" Value="11" />--%>
                            <rad:RadComboBoxItem ID="RadComboBoxItem17" runat="server" Text="作废订单" Value="12" />
                            <%--<rad:RadComboBoxItem ID="RadComboBoxItem16" runat="server" Text="退货退款审核" Value="13" />--%>
                            <rad:RadComboBoxItem ID="RadComboBoxItem15" runat="server" Text="退货" Value="14" />
                            <%--<rad:RadComboBoxItem ID="RadComboBoxItem14" runat="server" Text="退货单" Value="15" />--%>
                        </Items>
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    <asp:ImageButton ID="IB_CreationChart" runat="server" SkinID="CreationChart" OnClick="IB_CreationChart_Click" />
                </td>
            </tr>
            <tr>
                <td class="ShortFromRowTitle">
                    &nbsp;
                </td>
                <td class="AreaEditFromRowInfo">
                    &nbsp;
                </td>
                <td class="ShortFromRowTitle">
                    &nbsp;
                </td>
                <td class="AreaEditFromRowInfo">
                    &nbsp;
                </td>
                <td class="ShortFromRowTitle">
                    &nbsp;
                </td>
            </tr>
        </table>
    </div>
    <div align="center">
        <rad:RadChart EnableViewState="true" OnItemDataBound="Chart_ItemDataBound" ID="MemberChart"
            runat="server" Width="980px" Height="450px" SkinID="Common">
        </rad:RadChart>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_CreationChart">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="MemberChart" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_ProvinceId">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_CityId" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

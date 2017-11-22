<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.OrderAmountChartAw" Title="" CodeBehind="OrderAmountChart.aspx.cs" %>

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
                        Width="150px" Height="100px" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged"
                        AutoPostBack="True">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    起止日期：
                </td>
                <td class="AreaEditFromRowInfo">
                    <table>
                        <tr>
                            <td>
                                年份：<asp:DropDownList runat="server" ID="DDL_Years" AutoPostBack="true" OnSelectedIndexChanged="DdlYearsSelectedIndexChanged">
                                </asp:DropDownList>
                            </td>
                            <td>
                                <rad:RadDatePicker ID="RDP_StartTime" runat="server" AutoPostBack="true" SkinID="Common"
                                    Width="95px">
                                </rad:RadDatePicker>
                            </td>
                            <td>
                                <rad:RadDatePicker ID="RDP_EndTime" runat="server" AutoPostBack="true" SkinID="Common"
                                    Width="95px">
                                </rad:RadDatePicker>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="ShortFromRowTitle">
                    按地址：
                </td>
                <td class="AreaEditFromRowInfo">
                    <table>
                        <tr>
                            <td>
                                <rad:RadComboBox ID="RCB_CountryId" CausesValidation="false" runat="server" AutoPostBack="true"
                                    Height="180" OnSelectedIndexChanged="RcbCountryIdSelectedIndexChanged" Width="76px">
                                </rad:RadComboBox>
                            </td>
                            <td>
                                <rad:RadComboBox ID="RCB_ProvinceId" CausesValidation="false" runat="server" AutoPostBack="true"
                                    Height="180" OnSelectedIndexChanged="RcbProvinceIdSelectedIndexChanged" Width="76px">
                                </rad:RadComboBox>
                            </td>
                            <td>
                                <rad:RadComboBox ID="RCB_CityId" CausesValidation="false" runat="server" AutoPostBack="false"
                                    Height="180" Width="76px">
                                </rad:RadComboBox>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="ShortFromRowTitle">
                    <asp:CheckBox ID="CheckBox1" runat="server" Text="累积" />
                </td>
            </tr>
            <tr>
                <td class="ShortFromRowTitle">
                    销售平台：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false"
                        AllowCustomText="True" Width="150px" Height="100px" OnSelectedIndexChanged="RCB_SalePlatform_OnSelectedIndexChanged"
                        AutoPostBack="True">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    付款方式：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBoxPayMode" runat="server" AutoPostBack="false" Height="180"
                        Width="206px">
                        <Items>
                            <rad:RadComboBoxItem runat="server" Text="全部" Value="-1" />
                            <rad:RadComboBoxItem runat="server" Text="货到付款" Value="0" />
                            <rad:RadComboBoxItem runat="server" Text="款到发货" Value="1" />
                        </Items>
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    订单状态：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBoxOrderState" runat="server" AutoPostBack="false" Height="180"
                        Width="256px">
                        <Items>
                            <rad:RadComboBoxItem ID="RadComboBoxItem2" runat="server" Text="完成订单" Value="9" />
                            <%--<rad:RadComboBoxItem ID="RadComboBoxItem1" runat="server" Text="取消的订单" Value="10" />--%>
                            <rad:RadComboBoxItem ID="RadComboBoxItem3" runat="server" Text="作废的订单" Value="12" />
                            <%--<rad:RadComboBoxItem ID="RadComboBoxItem4" runat="server" Text="退货的订单" Value="14" />--%>
                            <rad:RadComboBoxItem ID="RadComboBoxItem6" runat="server" Text="全部订单" Value="-1" />
                        </Items>
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    <asp:ImageButton ID="IB_CreationChart" runat="server" CausesValidation="true" SkinID="CreationChart"
                        OnClick="IbCreationChartClick" />
                </td>
            </tr>
        </table>
    </div>
    <div align="center">
        <rad:RadChart ID="MemberChart" EnableViewState="false" OnItemDataBound="Chart_ItemDataBound"
            runat="server" Width="980px" SkinID="Common" Height="450px">
        </rad:RadChart>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
        <AjaxSettings>
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
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

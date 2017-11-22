<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.QueryOrderByFinancialChart" CodeBehind="QueryOrderByFinancialChart.aspx.cs" %>

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
                    年份：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:DropDownList ID="DDL_Year" runat="server">
                    </asp:DropDownList>
                </td>
                <td class="ShortFromRowTitle">
                    按地址：
                </td>
                <td class="AreaEditFromRowInfo">
                    <table>
                        <tr>
                            <td>
                                <rad:RadComboBox ID="RCB_CountryId" runat="server" UseEmbeddedScripts="false" AutoPostBack="true"
                                    ShowToggleImage="True" Height="180" Width="76px">
                                </rad:RadComboBox>
                            </td>
                            <td>
                                <rad:RadComboBox ID="RCB_ProvinceId" runat="server" UseEmbeddedScripts="false" AutoPostBack="true"
                                    ShowToggleImage="True" Height="180" Width="76px">
                                </rad:RadComboBox>
                            </td>
                            <td>
                                <rad:RadComboBox ID="RCB_CityId" runat="server" UseEmbeddedScripts="false" ShowToggleImage="True"
                                    AutoPostBack="false" Height="180" Width="76px">
                                </rad:RadComboBox>
                            </td>
                        </tr>
                    </table>
                </td>
                <td class="ShortFromRowTitle">
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
                    查询种类：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBoxPayMode" runat="server" UseEmbeddedScripts="false"
                        ShowToggleImage="True" AutoPostBack="false" Height="180" Width="206px" DropDownWidth="300px">
                        <Items>
                            <rad:RadComboBoxItem ID="RadComboBoxItem1" runat="server" Text="实收金额+余额支付(含门店)" Value="1" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem8" runat="server" Text="实收金额+余额支付(不含门店)" Value="2" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem2" runat="server" Text="实收金额(含门店)" Value="3" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem4" runat="server" Text="实收金额(不含门店)" Value="4" />
                        </Items>
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    订单状态：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBoxOrderState" runat="server" UseEmbeddedScripts="false"
                        ShowToggleImage="True" AutoPostBack="false" Height="180" Width="256px">
                        <Items>
                            <rad:RadComboBoxItem ID="RadComboBoxItem12" runat="server" Text="完成订单" Value="9" />
                            <%--<rad:RadComboBoxItem ID="RadComboBoxItem8" runat="server" Text="取消的订单" Value="10" />--%>
                            <rad:RadComboBoxItem ID="RadComboBoxItem9" runat="server" Text="作废的订单" Value="12" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem10" runat="server" Text="退货的订单" Value="14" />
                            <rad:RadComboBoxItem ID="RadComboBoxItem11" runat="server" Text="全部订单" Value="-1" />
                        </Items>
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    <asp:ImageButton ID="IB_CreationChart" runat="server" SkinID="CreationChart" OnClick="IbCreationChartClick" />
                </td>
            </tr>
        </table>
    </div>
    <div align="center">
        <rad:RadChart EnableViewState="false" OnItemDataBound="Chart_ItemDataBound" ID="MemberChart"
            runat="server" Width="1200px" Height="450px" SkinID="Common">
        </rad:RadChart>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
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

<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="PurchaseStatisticsPage.aspx.cs" Inherits="ERP.UI.Web.PurchaseStatisticsPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table class="PanelArea" width="100%">
        <tr>
            <td width="50%" align="right">
            </td>
            <td width="362px" align="left">
                <table width="100%">
                    <tr>
                        <td align="left">
                            起止时间:
                            <rad:RadDateTimePicker ID="RDP_StartTime" runat="server" Width="145px">
                            </rad:RadDateTimePicker>
                        </td>
                        <td>
                            -
                        </td>
                        <td align="left">
                            <rad:RadDateTimePicker ID="RDP_EndTime" runat="server" Width="145px">
                            </rad:RadDateTimePicker>
                        </td>
                    </tr>
                </table>
            </td>
            <td class="ShortFromRowTitle" align="right">
                <asp:Button ID="btn_Search" runat="server" Text="搜索" OnClick="Btn_Search_Click" />
            </td>
        </tr>
    </table>

    <rad:RadGrid ID="Rgd_PS" runat="server" OnNeedDataSource="Rgd_PS_OnNeedDataSource">
        <MasterTableView CommandItemDisplay="None">
            <Columns>
                <rad:GridBoundColumn DataField="GoodsName" HeaderText='商品名' ReadOnly="true" UniqueName="GoodsName">
                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Count" HeaderText='采购次数' ReadOnly="true" UniqueName="Count">
                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="TotalCount" HeaderText='商品总数' ReadOnly="true" UniqueName="TotalCount">
                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadAjaxManager runat="server" ID="RAM" OnAjaxRequest="Ram_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PS" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Rgd_PS">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PS" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

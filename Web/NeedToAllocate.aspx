<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="NeedToAllocate.aspx.cs" Inherits="ERP.UI.Web.NeedToAllocate" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
        <script src="JavaScript/common.js" type="text/javascript"></script>
        <script type="text/javascript" language="javascript">
            function ShowOrders(pmid, starttime, endtime, warehouseid) {
                window.radopen("./Windows/ShowNeedOrdersForm.aspx?PersonResponsible=" + pmid + "&starttime=" + starttime + "&endtime=" + endtime + "&WarehouseId=" + warehouseid, "OrderForm");
                return false;
            }
            function ShowGoods(pmid, starttime, endtime, warehouseid) {
                window.radopen("./Windows/ShowNeedGoodsForm.aspx?PersonResponsible=" + pmid + "&starttime=" + starttime + "&endtime=" + endtime + "&WarehouseId=" + warehouseid, "OrderForm");
                return false;
            }
        </script>
    </rad:RadScriptBlock>

    <table class="StagePanel">
        <tr style="height: 25px">

            <td width="40%"></td>
            <td>
                <rad:RadComboBox ID="RCB_Warehouse" runat="server" MarkFirstMatch="True" Width="160px"
                    Height="120px" DataValueField="Key" DataTextField="Value" AppendDataBoundItems="true">
                </rad:RadComboBox>
            </td>
            <td>
                <rad:RadComboBox ID="RCB_Pm" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                    MarkFirstMatch="True" ShowToggleImage="True" Width="120px" Height="200px">
                </rad:RadComboBox>
            </td>
            <td width="100px" align="right">起止日期：
            </td>
            <td width="250px" align="left">
                <table width="100%">
                    <tr>
                        <td align="left">
                            <rad:RadDateTimePicker ID="RDP_StartTime" runat="server" Width="150px">
                            </rad:RadDateTimePicker>
                        </td>
                        <td>-
                        </td>
                        <td align="left">
                            <rad:RadDateTimePicker ID="RDP_EndTime" runat="server" Width="150px">
                            </rad:RadDateTimePicker>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <asp:ImageButton ID="IB_Search" runat="server" SkinID="SearchButton" OnClick="Ib_Search_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="6">
                <rad:RadGrid ID="rgd_need" runat="server" OnNeedDataSource="Rgd_Need_OnNeedDataSource">
                    <MasterTableView CommandItemDisplay="None" DataKeyNames="PersonResponsible,WarehouseId">
                        <Columns>
                            <rad:GridBoundColumn HeaderText="负责人" DataField="PersonnelName" UniqueName="PersonnelName">
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn HeaderText="仓库" DataField="WarehouseName" UniqueName="WarehouseName">
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="缺货订单数" UniqueName="OrdersCount">
                                <ItemTemplate>
                                    <a style="text-decoration: underline; cursor: pointer" onclick='ShowOrders("<%#Eval("PersonResponsible")%>","<%#StartTime%>","<%#EndTime %>","<%#Eval("WarehouseId")%>")'>
                                        <%#Eval("OrderCount").ToString()%>
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="缺货商品数" UniqueName="GoodsCount">
                                <ItemTemplate>
                                    <a style="text-decoration: underline; cursor: pointer" onclick='ShowGoods("<%#Eval("PersonResponsible")%>","<%#StartTime%>","<%#EndTime %>","<%#Eval("WarehouseId")%>")'>
                                        <%#Eval("GoodsQuantities").ToString()%>
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>

                            <%--   <rad:GridTemplateColumn HeaderText="负责人当月缺货率" UniqueName="ShortageRate">
                                <ItemTemplate>
                                    <label><%#Convert.ToDecimal(Eval("ShortageRate")).ToString("0.00")%>%</label>
                                </ItemTemplate>
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>--%>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>

    <rad:RadWindowManager ID="WindowManager" runat="server" Width="900px" Height="500px">
        <Windows>
            <rad:RadWindow ID="OrderForm" runat="server" Width="900px" Height="500px" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgd_need" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="rgd_need">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgd_need" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

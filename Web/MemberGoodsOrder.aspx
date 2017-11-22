<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.MemberGoodsOrderAw" Title="" CodeBehind="MemberGoodsOrder.aspx.cs" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
        TagPrefix="Ibt" %>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" language="javascript">
            function ShowClewForm(orderId, saleFilialeId) {
                window.radopen("./Windows/ShowClewForm.aspx?OrderId=" + orderId + "&SaleFilialeId=" + saleFilialeId, "ShowClew");
                return false;
            }

            function RowDblClick(obj, args) {
                window.radopen("./Windows/ShowGoodsOrder.aspx?OrderId=" + args.getDataKeyValue("OrderId") + "&SaleFilialeId=" + args.getDataKeyValue("SaleFilialeId") + "&OrderTime=" + args.getDataKeyValue("OrderTime"), "ShowGoodsOrder");
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
    <div class="StagePanel">
        <table class="StagePanelHead" border="0" style="width: 100%;">
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td class="ShortFromRowTitle">起止日期：
                </td>
                <td class="AreaEditFromRowInfo">年份：<asp:DropDownList runat="server" ID="DDL_Years" AutoPostBack="true" OnSelectedIndexChanged="DdlYearsSelectedIndexChanged">
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
                <td class="ShortFromRowTitle">订单状态：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_OrderState" runat="server" MarkFirstMatch="True" Width="160px"
                        Height="120px" DataValueField="Key" DataTextField="Value" AppendDataBoundItems="true">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">仓库：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" MarkFirstMatch="True" Width="160px"
                        Height="120px" DataValueField="Key" DataTextField="Value" AppendDataBoundItems="true">
                    </rad:RadComboBox>
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadTextBox ID="RTB_Search" runat="server" SkinID="StandardInput" EmptyMessage="搜索订单号">
                    </rad:RadTextBox>
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td>
                    <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" Width="120px"
                        Height="100px" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged"
                        AutoPostBack="True" EmptyMessage="销售公司">
                    </rad:RadComboBox>
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false"
                        Width="120px" Height="100px" OnSelectedIndexChanged="RCB_SalePlatform_OnSelectedIndexChanged"
                        AutoPostBack="True" EmptyMessage="销售平台">
                    </rad:RadComboBox>
                </td>
                <td>
                    <rad:RadTextBox runat="server" EmptyMessage="搜索收货人" ID="RTB_RealName" Width="120px" />
                </td>
                <td>
                    <rad:RadTextBox runat="server" EmptyMessage="搜索手机号" ID="RTB_Mobil" Width="120px" />
                </td>
                <td>
                    <rad:RadTextBox runat="server" EmptyMessage="搜索会员名" ID="RTB_UserName" Width="125px"
                        Enabled="False" ToolTip="会员名搜索需选择具体的销售平台" />
                </td>
                <td>
                    <rad:RadTextBox runat="server" EmptyMessage="搜索支付号" ID="RTB_PaidNo" Width="120px" />
                </td>
                <td>
                    <rad:RadTextBox runat="server" EmptyMessage="搜索快递号" ID="RTB_ExpressNo" Width="120px" />
                </td>
                <td>
                    <asp:ImageButton ID="IB_Search" runat="server" SkinID="SearchButton" OnClick="Ib_Search_Click"
                        ValidationGroup="Search" />
                </td>
                <%--<td>
                    <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="Ib_ExportData_Click" />
                </td>--%>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="RGMGO" runat="server" SkinID="CustomPaging" OnNeedDataSource="Rgmgo_NeedDataSource"
        OnItemDataBound="Rgmgo_ItemDataBound">
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>
        <MasterTableView ClientDataKeyNames="OrderId,SaleFilialeId,OrderTime" DataKeyNames="OrderId,SaleFilialeId,OrderTime">
            <CommandItemTemplate>
                <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" CausesValidation="false"
                    SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridBoundColumn DataField="OrderNo" HeaderText="订单编号" UniqueName="OrderNo">
                    <HeaderStyle Width="140px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="Consignee" HeaderText="收货人" UniqueName="Consignee">
                    <ItemTemplate>
                        <asp:Label ID="LabConsignee" runat="server" Text='<%# Eval("Consignee") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Direction" HeaderText="收货地址" UniqueName="Direction"
                    AllowSorting="false">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="快递单号" UniqueName="ExpressNo">
                    <ItemTemplate>
                        <asp:Label ID="ExpressNo" runat="server" Text='<%# Eval("ExpressNo") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="支付方式" UniqueName="PayMode">
                    <ItemTemplate>
                        <asp:Label ID="PayMode" runat="server" Text='<%# GetPayMode(Eval("PayMode")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="90px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="实收金额" UniqueName="Price">
                    <ItemTemplate>
                        <asp:Label ID="PriceLabel" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealTotalPrice")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="运费" UniqueName="Price">
                    <ItemTemplate>
                        <asp:Label ID="CarriageLabel" runat="server" Text='<%# ReturnCarriage(int.Parse(Eval("OrderState").ToString()),decimal.Parse(Eval("Carriage").ToString())) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="OrderNo" HeaderText="订单来源" UniqueName="ForWhere">
                    <ItemTemplate>
                        <asp:Label ID="lab_forwhere" runat="server" Text='<%# Eval("OrderNo").ToString().ToLower().Contains("srt")?"艾视":"可得" %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="订单状态" UniqueName="OrderState">
                    <ItemTemplate>
                        <asp:Label ID="OrderState" runat="server" Text='<%# GetOrderState(Eval("OrderState")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发货仓库" UniqueName="WarehouseId">
                    <ItemTemplate>
                        <asp:Label ID="WarehouseId" runat="server" Text='<%# GetWarehouseName((Guid)Eval("DeliverWarehouseId")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Clew">
                    <ItemTemplate>
                        <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                            OnClientClick='<%# "return ShowClewForm(\"" + Eval("OrderId") + "\",\""+Eval("SaleFilialeId")+"\");" %>'
                            ToolTip='<%#GetMisClew(Eval("OrderId"))%>' />
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="标签">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="LBTab" Text=""></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGMGO" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTB_RealName"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTB_Mobil"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTB_UserName"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTB_PaidNo"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTB_ExpressNo"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTB_Search"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGMGO">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGMGO" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_Years">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RDP_EndTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTB_UserName" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SalePlatform">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RTB_UserName" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager ID="ClewWindowManager" runat="server" UseEmbeddedScripts="false"
        ReloadOnShow="true">
        <Windows>
            <rad:RadWindow runat="server" ID="ShowClew" Height="500" Width="700" Title="备注">
            </rad:RadWindow>
            <rad:RadWindow runat="server" ID="ShowRefund" Height="500" Width="700" Title="售后流水">
            </rad:RadWindow>
            <rad:RadWindow runat="server" ID="ShowGoodsOrder" Width="900" Height="560" Title="订单查询">
            </rad:RadWindow>
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

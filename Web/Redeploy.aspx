<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.RedeployAw" Title="" CodeBehind="Redeploy.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <%@ register src="UserControl/ImageButtonControl.ascx" tagname="ImageButtonControl"
        tagprefix="Ibt" %>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowClewForm(orderId, SaleFilialeId) {
                window.radopen("./Windows/ShowClewForm.aspx?OrderId=" + orderId + "&SaleFilialeId=" + SaleFilialeId, "ClewWindow");
                return false;
            }

            function refreshGrid(arg) {
                if (!arg) {
                    window["<%=RAM.ClientID %>"].ajaxRequest('Rebind');
                }
                else {
                    window["<%=RAM.ClientID %>"].ajaxRequest('RebindAndNavigate');
                }
            }

            function RowDblClick(object, args) {
                window.radopen("./Windows/ShowGoodsOrder.aspx?OrderId=" + args.getDataKeyValue("OrderId"), "GoodsDialog");
            }
            

        </script>
    </rad:RadScriptBlock>
    <div style="text-align: right;margin-left:23px;">
        <table>
            <tr>
                <td style="width: 60px; text-align: right;">
                    <rad:RadDatePicker ID="R_StartTime" runat="server" Width="145px" SelectedDate='<%# StartTime==DateTime.MinValue ? (DateTime?)null : StartTime%>'
                        SkinID="Common">
                    </rad:RadDatePicker>
                </td>
                <td>
                    &nbsp;&nbsp;-&nbsp;&nbsp;
                </td>
                <td style="width: 60px; text-align: right;">
                    <rad:RadDatePicker ID="R_EndTime" runat="server" Width="145px" SelectedDate='<%# EndTime==DateTime.MinValue ? (DateTime?)null : EndTime%>'
                        SkinID="Common">
                    </rad:RadDatePicker>
                </td>
                <td style="width: 100px; text-align: right;">
                    商品名称/编号：
                </td>
                <td style="width: 60px; text-align: right;">
                    <rad:RadComboBox ID="RCB_Goods" runat="server" CausesValidation="false"
                        AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="GoodsName" DataValueField="GoodsId"
                        Width="220px" Height="200px" OnItemsRequested="RcbGoodsItemsRequested"
                        EmptyMessage="输入搜索商品">
                    </rad:RadComboBox>
                </td>
                <td style=" text-align: right;">
                    关键字:<asp:TextBox ID="TB_Search" runat="server" Width="200px" SkinID="StandardInput" Text='<%# SearchKey %>'></asp:TextBox>
                </td>
                <td>
                    <asp:ImageButton ID="LB_Search" OnClick="LBSearch_Click" runat="server" SkinID="SearchButton" />
                </td>
                <td style="text-align: right;">
                    <Ibt:ImageButtonControl ID="LB_Redeploy" runat="server" OnClick="LB_Redeploy_Click" CausesValidation="false"
                        Enabled='<%# !string.IsNullOrEmpty(SearchKey)  %>' SkinType="Insert"
                        Text="需采购列表"></Ibt:ImageButtonControl>
                </td>
                <td style="text-align: right;">
                    <Ibt:ImageButtonControl ID="LB_SendSMS" runat="server" CommandName="RebindGrid" OnClick="LB_SendSMS_Click"
                        SkinType="Messages" Text="发短信"></Ibt:ImageButtonControl>
                </td>
                <td style="text-align: right;">
                    <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                        Text="刷新" OnClick="LBRefresh_Refresh"></Ibt:ImageButtonControl>
                </td>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="RGGoodsOrder" runat="server" SkinID="CustomPaging" OnNeedDataSource="RGGoodsOrder_NeedDataSource" >
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>
        <MasterTableView DataKeyNames="OrderId" ClientDataKeyNames="OrderId">
            <CommandItemTemplate>
                        </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="0px" />
            <Columns>
                <rad:GridTemplateColumn DataField="OrderNo" HeaderText="订单编号" UniqueName="OrderNo">
                    <ItemTemplate>
                        <asp:Label ID="OrderNoLabel" runat="server" Text='<%# (Eval("ExpressNo")==null ? "" : string.IsNullOrEmpty(Eval("ExpressNo").ToString()) ? "":"*") + Eval("OrderNo") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="Consignee" HeaderText="收货人" UniqueName="Consignee">
                    <ItemTemplate>
                        <asp:Label ID="ConsigneeLabel" runat="server" Text='<%# Eval("Consignee") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>

                <rad:GridBoundColumn DataField="Direction" HeaderText="收货地址" UniqueName="Direction">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="联系电话" UniqueName="TemplateColumn2">
                    <ItemTemplate>
                        <asp:Label ID="Phone" runat="server" Text='<%# Eval("Phone")==null || string.IsNullOrEmpty(Eval("Phone").ToString()) ? "" : "电话："+Eval("Phone") %>'></asp:Label>
                        <br />
                        <asp:Label ID="Mobile" runat="server" Text='<%# Eval("Mobile")==null || string.IsNullOrEmpty(Eval("Mobile").ToString()) ? "" : "手机："+Eval("Mobile") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="160px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="付言" UniqueName="Memo">
                    <ItemTemplate>
                        <asp:Image ID="ImageButton" runat="server" SkinID="MemoImg" Visible='<%# Eval("Memo")!=null &&  !string.IsNullOrEmpty(Eval("Memo").ToString()) %>'
                            ToolTip='<%# Eval("Memo") %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Clew">
                    <ItemTemplate>
                        <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                            OnClientClick='<%# "return ShowClewForm(\"" + Eval("OrderId") + "\",\"" + Eval("SaleFilialeId") + "\");" %>'
                            ToolTip='<%#GetMisClew(Eval("OrderId"))%>' />
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <asp:TextBox ID="TextBoxDate" runat="server" SkinID="AutoTextarea" TextMode="MultiLine"
        Width="100%"></asp:TextBox>
    <rad:RadWindowManager ID="ClewWindowManager" runat="server" Height="195" Width="420"
        ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="GoodsDialog" runat="server" Title="产品信息" Width="900" Height="576" />
            <rad:RadWindow ID="Clew" runat="server" Title="订单留言" Width="700" Height="500" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGGoodsOrder" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Redeploy">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGGoodsOrder" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGGoodsOrder">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGGoodsOrder" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

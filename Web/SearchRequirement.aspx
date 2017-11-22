<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.SearchRequirementAw" Title="" Codebehind="SearchRequirement.aspx.cs" %>
<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" Runat="Server">
<rad:RadScriptBlock ID="RSB" runat="server">
    <script type="text/javascript" language="javascript">
        function RowDblClick(obj, args) {
            var drop = document.getElementById('<%=RGGoodsOrder.MasterTableView.GetItems(GridItemType.CommandItem)[0].FindControl("RCB_InStockF").ClientID%>');
            window.radopen("./Windows/RequirementOrder.aspx?type=0&GoodsId=" + args.getDataKeyValue("RealGoodsId") + "&warehouseId=" + drop.control._value, "");
        }

        function ShowRequirementOrderFrom(goodsId, type) {
            var drop = document.getElementById('<%=RGGoodsOrder.MasterTableView.GetItems(GridItemType.CommandItem)[0].FindControl("RCB_InStockF").ClientID%>');
            var filialeId = '<%= FilialeId %>';
            window.radopen("./Windows/RequirementOrder.aspx?type="+type+"&GoodsId=" + goodsId + "&warehouseId=" + drop.control._value+"&FilialeId="+filialeId, "");
        }
    </script>
</rad:RadScriptBlock>
<rad:RadGrid ID="RGGoodsOrder" runat="server" SkinID="CustomPaging" OnNeedDataSource="RGGoodsOrder_NeedDataSource" OnItemCommand="RGGoodsOrder_ItemCommand">
    <ClientSettings>
        <ClientEvents OnRowDblClick="RowDblClick" />
    </ClientSettings>
    <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="RealGoodsId">
        <CommandItemTemplate>
            商品名称:<asp:TextBox ID="TB_Search" runat="server" SkinID="StandardInput" Text='<%# SearchKey %>' Width="220px"></asp:TextBox>
            公司：
            <rad:RadComboBox ID="RCB_FilileF" runat="server" CommandName="RebindGrid" useembeddedscripts="false"
                AccessKey="T" DataSource='<%#BindFilile()%>' MarkFirstMatch="True" ShowToggleImage="True"
                DataTextField="Name" DataValueField="ID" SelectedValue='<%#FilialeId %>' AutoPostBack="true">
            </rad:RadComboBox>
            仓库：
            <rad:RadComboBox ID="RCB_InStockF" runat="server" useembeddedscripts="false" AccessKey="T"
                DataSource='<%#BindWareHouse()%>' MarkFirstMatch="True" ShowToggleImage="True"
                DataTextField="WarehouseName" DataValueField="WarehouseId" 
                SelectedValue='<%#WarehouseId %>'>
            </rad:RadComboBox>
            <Ibt:ImageButtonControl ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search" SkinType="Search" Text="搜索"></Ibt:ImageButtonControl>
            &nbsp;&nbsp;&nbsp;
        </CommandItemTemplate>
        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
        <Columns>
            <rad:GridTemplateColumn HeaderText="商品名称" UniqueName="GoodsName">
                <ItemTemplate>
                    <asp:Label runat="server" ID="Lable1" Text='<%# Eval("GoodsName")%>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn  HeaderText="SKU" UniqueName="Specification">
                <ItemTemplate>
                    <asp:Label runat="server" ID="Lable2" Text='<%# Eval("Specification")%>'></asp:Label>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridTemplateColumn>
             <rad:GridTemplateColumn HeaderText="需求数" UniqueName="RequireQuantity" SortExpression="RequireQuantity">
                <ItemTemplate>
                    <%# Eval("RequireQuantity")%>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="占用量" UniqueName="DemandQuantity" SortExpression="DemandQuantity">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnQuantity" OnClientClick='<%# GetShowFrom(Eval("RealGoodsId"),1) %>' runat="server"><%# Eval("DemandQuantity")%></asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="出库量" UniqueName="OutboundQuantity" SortExpression="OutboundQuantity">
                <ItemTemplate>
                    <asp:LinkButton ID="lbtnDemandQuantity" OnClientClick='<%# GetShowFrom(Eval("RealGoodsId"),2) %>' runat="server"><%# Eval("OutboundQuantity")%></asp:LinkButton>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</rad:RadGrid>
<rad:RadWindowManager ID="ClewWindowManager" runat="server" Title="订单查询" Height="600px" Width="1000px" ReloadOnShow="true">
</rad:RadWindowManager>
<rad:RadAjaxManager ID="RAM" runat="server">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="RGGoodsOrder">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="RGGoodsOrder" LoadingPanelID="loading" />
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>


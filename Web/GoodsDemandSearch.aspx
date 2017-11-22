<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="GoodsDemandSearch.aspx.cs" Inherits="ERP.UI.Web.GoodsDemandSearch" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <style type="text/css">
        .linkstyle a {
            text-decoration: underline;
        }
    </style>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" language="javascript">
            function ShowGoodsDemandNotOutQuantityDetailForm(goodsId, warehouseId, filileId) {
                window.radopen("./Windows/GoodsDemandNotOutQuantityDetailForm.aspx?GoodsId=" + goodsId + "&WarehouseId=" + warehouseId + "&FilialeId=" + filileId, "GoodsDemandNotOutQuantityDetailForm");
                return false;
            }
        </script>
    </rad:RadScriptBlock>
    <table width="100%">
        <tr>
            <td style="text-align: right;">商品名称：</td>
            <td>
                <rad:RadComboBox ID="RCB_Goods" runat="server" CausesValidation="false"
                    AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="GoodsName" DataValueField="GoodsId"
                    OnItemsRequested="RcbGoodsItemsRequested" Width="220px" Height="200px">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">仓库：</td>
            <td>
                <rad:RadComboBox ID="RCB_Warehouse" runat="server" ShowToggleImage="True"
                    AutoPostBack="true" Width="120px" OnSelectedIndexChanged="DDLWaerhouse_OnSelectedIndexChanged">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">公司：</td>
            <td>
                <rad:RadComboBox ID="RCB_Filile" runat="server" ShowToggleImage="True"
                    Width="130px">
                </rad:RadComboBox>
            </td>
            <td>
                <asp:ImageButton ID="LB_Search" runat="server" ValidationGroup="Search" SkinID="SearchButton" OnClick="LB_Search_Click" /></td>
        </tr>
    </table>
    <rad:RadGrid ID="RGGoodsOrder" runat="server" ShowFooter="true" SkinID="Common_Foot" 
        OnNeedDataSource="RGGoodsOrder_NeedDataSource" CssClass="linkstyle" OnItemDataBound="RGGoodsOrderDataBound">
        <ClientSettings>
        </ClientSettings>
        <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="RealGoodsId">
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="0px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="商品名称" UniqueName="GoodsName" >
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Lable1" Text='<%# Eval("GoodsName")%>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="300px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="SKU" UniqueName="Specification">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="Lable2" Text='<%# Eval("Specification")%>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="有效需求数" UniqueName="EffictiveRequire" SortExpression="EffictiveRequire">
                    <ItemTemplate>
                        <asp:LinkButton ID="lbtnQuantity" OnClientClick='<%# GetShowFrom(Eval("RealGoodsId")) %>' runat="server"><%# Eval("EffictiveRequire")%></asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="可用库存数" UniqueName="CanUseGoodsStock" SortExpression="CanUseGoodsStock">
                    <ItemTemplate>
                        <%# Eval("CanUseGoodsStock")%>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderStyle-HorizontalAlign="Center">
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
        
    <rad:RadWindowManager runat="server">
        <Windows>
            <rad:RadWindow ID="GoodsDemandNotOutQuantityDetailForm" runat="server" Title="需求单据" Height="540" Width="900" />
        </Windows>
    </rad:RadWindowManager>

    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RGGoodsOrder">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGGoodsOrder" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Filile" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGGoodsOrder" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

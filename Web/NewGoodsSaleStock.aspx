<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="NewGoodsSaleStock.aspx.cs" Inherits="ERP.UI.Web.NewGoodsSaleStock" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript">
            function ShowAuditForm(goodsId) {
                window.radopen("./Windows/AddGoodSaleStockForm.aspx?GoodsId=" + goodsId + "&Type=Audit", "RW1");
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
    
    <table width="100%">
        <tr>
            <td style="text-align: right;">
                商品名关键字：
                <asp:TextBox ID="TB_Search" Text='<%# SearchKey %>' Width="300" SkinID="StandardInput" runat="server"></asp:TextBox>
                &nbsp;<asp:ImageButton ID="LB_Search" OnClick="LbSearchClick" ValidationGroup="Search" SkinID="SearchButton" Style='vertical-align: middle' runat="server" />
                &nbsp;&nbsp;
            </td>
        </tr>
    </table>
                
    <rad:RadGrid ID="GoodsGrid" SkinID="CustomPaging" OnNeedDataSource="GoodsGrid_NeedDataSource" OnItemDataBound="GoodsGrid_ItemDataBound" runat="server">
        <MasterTableView DataKeyNames="GoodsId,GoodsName" ClientDataKeyNames="GoodsId" CommandItemDisplay="None">
            <Columns>
                <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                    <HeaderStyle HorizontalAlign="Center" Width="330px" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="GoodsType" HeaderText="商品类型" UniqueName="GoodsType">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((GoodsKindType)Eval("GoodsType"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="95px" HorizontalAlign="Center" />
                    <ItemStyle Width="95px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="(旧)卖库存状态">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((SaleStockType)Eval("OldSaleStockType"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="(新)卖库存状态">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((SaleStockType)Eval("NewSaleStockType"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <ItemTemplate>
                        <rad:GridTextButton ID="GTBtn_Audit" runat="server" Text="审核" style="cursor: pointer" OnClientClick='<%# "return ShowAuditForm(\"" + Eval("GoodsId") + "\");" %>'></rad:GridTextButton>
                    </ItemTemplate>
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" OnAjaxRequest="RamGoodsAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="GoodsGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GoodsGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GoodsGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GoodsGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

    <rad:RadWindowManager ID="WMGoods" runat="server" useembeddedscripts="false" Height="310px" Width="880px" ReloadOnShow="true" useclassicwindows="false">
        <Windows>
            <rad:RadWindow ID="GoodsDialog" runat="server" Title="卖库存商品信息" />
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

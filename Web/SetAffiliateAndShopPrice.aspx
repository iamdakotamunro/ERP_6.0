<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="SetAffiliateAndShopPrice.aspx.cs" Inherits="ERP.UI.Web.SetAffiliateAndShopPrice" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table class="StagePanel">
        <tr>
            <td style="width: 200px; vertical-align: top;">
                <rad:RadTreeView ID="TVGoodsClass" runat="server" SkinID="Common" Height="550px"
                    Width="200px" CausesValidation="false" OnNodeClick="TvGoodsClassNodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <div style="padding-bottom: 10px;">
                    <rad:RadTextBox ID="txt_GoodsNameOrCode" runat="server" EmptyMessage="商品编号/名称" Width="300px" Style="margin-right: 10px;"></rad:RadTextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="搜索" OnClick="btnSearch_Click" />
                </div>
                <rad:RadGrid ID="RG_GoodsPriceList" SkinID="CustomPaging" runat="server" OnNeedDataSource="RG_GoodsPriceList_NeedDataSource">
                    <MasterTableView DataKeyNames="GoodsID,GoodsCode,GoodsName,JoinPrice,WholesalePrice" ClientDataKeyNames="GoodsID">
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle Height="0px" />
                        <Columns>
                            <rad:GridTemplateColumn HeaderText="商品编号" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%#Eval("GoodsCode") %>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="商品名称" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%#Eval("GoodsName")%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="采购价" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetGoodsPurchasePrice(new Guid(Eval("GoodsID").ToString()))) %>
                                </ItemTemplate>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="可得官网价" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("KeedePrice"))%>
                                </ItemTemplate>
                                <ItemStyle Width="60px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="加盟价" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("JoinPrice"))%>' OnTextChanged="txt_ChangeJoinPrice_OnTextChanged" AutoPostBack="True" style="width: 60px;"></asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle Width="60px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="批发价" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <asp:TextBox runat="server" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("WholesalePrice"))%>' OnTextChanged="txt_ChangeWholesalePrice_OnTextChanged" AutoPostBack="True" style="width: 60px;"></asp:TextBox>
                                </ItemTemplate>
                                <ItemStyle Width="60px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="Loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RG_GoodsPriceList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_GoodsPriceList" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="TVGoodsClass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_GoodsPriceList" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_GoodsPriceList" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

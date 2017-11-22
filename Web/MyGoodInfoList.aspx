<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MyGoodInfoList.aspx.cs" Inherits="ERP.UI.Web.MyGoodInfoList" MasterPageFile="~/MainMaster.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="/JavaScript/telerik.js" type="text/javascript"></script>
        <script type="text/javascript">
            function ClientShowMaxForm(sender) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }
            function RowDblClick(obj, args) {
                window.radopen("./Windows/AddComparePro.aspx?goodsId=" + args.getDataKeyValue("GoodsID"), "Add");
                return false;
            }

            function openSetPrice(id) {
                window.radopen("./Windows/SetPrice.aspx?goodsId=" + id, "SetPrice");
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
    <table class="StagePanel">
        <tr>
            <td valign="top">
                <rad:RadTreeView ID="TVGoodsClass" runat="server" SkinID="Common" Height="550px"
                    Width="200px" autopostback="true" CausesValidation="false" OnNodeClick="TvGoodsClassNodeClick">
                </rad:RadTreeView>
            </td>
            <td valign="top">
                <rad:RadGrid ID="RG_NoCompareList" OnNeedDataSource="NoCompareList_NeedDataSource" SkinID="CustomPaging" runat="server"
                    OnItemCommand="ItemCommand">
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="GoodsID" ClientDataKeyNames="GoodsID">
                        <CommandItemTemplate>
                            <rad:RadTextBox ID="TBox_GoodsName" runat="server" Width="300px" Text='<%# SearchGoodsName %>'>
                            </rad:RadTextBox>
                            <asp:LinkButton ID="LB_Search" runat="server" CommandName="Search">
                                <asp:Image ID="ImgSearch" runat="server" ImageAlign="AbsMiddle" SkinID="searchimagebutton" />搜索
                            </asp:LinkButton>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Center" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                <HeaderStyle Width="230px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="可得价">
                                <ItemTemplate>
                                    <a style="text-decoration: underline; cursor: pointer" onclick="openSetPrice('<%# Eval("GoodsID") %>')">
                                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetPrice(new Guid(Eval("GoodsID").ToString()), 2))%>
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="亿超价">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetFetchPrice(1,Eval("GoodsID")))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="视客价">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetFetchPrice(2,Eval("GoodsID")))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="合亚价">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetFetchPrice(3,Eval("GoodsID")))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="4inlook价">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetFetchPrice(4,Eval("GoodsID")))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="爱戴价">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetFetchPrice(5,Eval("GoodsID")))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="1号店价">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetFetchPrice(6,Eval("GoodsID")))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="亚马逊价">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetFetchPrice(7,Eval("GoodsID")))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="易视">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetFetchPrice(8,Eval("GoodsID")))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="天猫">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetFetchPrice(9,Eval("GoodsID")))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="最后进货价">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetPrice(new Guid(Eval("GoodsID").ToString()),1))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="利润率">
                                <ItemTemplate>
                                    <%# GetInterestRate(new Guid(Eval("GoodsID").ToString()))%>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="与可得价差价">
                                <ItemTemplate>
                                    <%# ToHtmlDifferencePrice(CompareDifferencePrice(new Guid(Eval("GoodsID").ToString())))%>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager ID="StockWindowManager" Height="577px" Width="900px" runat="server"
        ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="Add" runat="server" Title="添加匹配" OnClientShow="ClientShowMaxForm"></rad:RadWindow>
            <rad:RadWindow ID="SetPrice" runat="server" Title="设置价格"></rad:RadWindow>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="Loading" OnAjaxRequest="Ram_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RG_NoCompareList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_NoCompareList" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="TVGoodsClass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_NoCompareList" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_NoCompareList" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.GoodsStockAw" Title="" CodeBehind="GoodsStock.aspx.cs" %>

<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<%@ Register Src="~/UserControl/CommonEnterSearchControl.ascx" TagName="CommonEnterSearch" TagPrefix="ce" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" language="javascript">
            function RowDblClick(obj, args) {
                var warehouseList = $find('<%=RCB_Warehouse.ClientID %>');
                var whId = warehouseList.get_selectedItem() == null ? "" : warehouseList.get_selectedItem().get_value();
                window.radopen("./Windows/ShowGoodsStock.aspx?GoodsId=" + args.getDataKeyValue("GoodsId") + "&warehouseid=" + whId, "");
            }

            function DbClick(warehouseId, goodsId) {
                window.radopen("./Windows/ShowGoodsStock.aspx?GoodsId=" + goodsId + "&warehouseid=" + warehouseId, "");
            }

            function removeClass() {
                $(".rgHeader").css("padding", "0");
                $(".rgHeader td").css("min-width", "52px");
                $(".rgMasterTable>tbody td").css({ "padding": "0", "border-bottom-width": "0" });//移除repeater中所有td的padding值和下边框宽度
                $(".rgMasterTable>tbody table td").css({ "padding-top": "5px", "padding-bottom": "5px", "min-width": "50px" });

                SetTdWidth();//设置td的宽度
                SetTableHeight();//设置table的高度
            }

            //设置table的高度
            function SetTableHeight() {
                $(".rgMasterTable>tbody table").each(function () {
                    var parentTd = $(this).parent();
                    //parentTd.css({ "border-left": "solid 1px #3d556c" });//设置包含table的td的左边框样式
                    $(this).height(parentTd.height());//设置table的高度

                    SetTdBorder(this);//设置分组线(设置table中td的border样式)
                });
            }

            //设置分组线(设置table中td的border样式)
            function SetTdBorder(obj) {
                var index = 0;
                $(".rgMasterTable>thead table td[class='Group']").each(function () {
                    var colspan = $(this).attr("colspan");
                    if (typeof (colspan) != "undefined") {
                        index += parseInt(colspan);
                        //$(obj).find("td").eq(index).css({ "border-left": "solid 1px #3d556c" });
                    }
                });
            }
            //设置td的宽度
            function SetTdWidth() {
                $(".rgMasterTable>thead table td[class='title']").each(function (i) {
                    var width = $(this).width();
                    $(".rgMasterTable>tbody table").each(function () {
                        $(this).find("td").eq(i).css("width", width);
                    });
                });
            }

        </script>
    </rad:RadScriptBlock>
    <table class="StagePanel">
        <tr>
            <td style="width: 200px; vertical-align: top;">
                <rad:RadTreeView ID="TVGoodsClass" runat="server" UseEmbeddedScripts="false" Height="500px"
                    Width="220px" AutoPostBack="true" CausesValidation="True" OnNodeClick="TvGoodsClass_NodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <div align="right">
                    <table>
                        <tr>
                            <td>仓库：
                            </td>
                            <td>
                                <rad:RadComboBox ID="RCB_Warehouse" runat="server"></rad:RadComboBox>
                            </td>
                            <td>
                                <Ibt:ImageButtonControl ID="LBXLS" runat="server" OnClick="Lbxls_Click" SkinType="ExportExcel"
                                    Text="导出EXCEL"></Ibt:ImageButtonControl>
                            </td>
                        </tr>
                    </table>
                </div>
                <rad:RadGrid ID="GridGoodsStock" runat="server" SkinID="CustomPaging" OnNeedDataSource="GridGoodsStock_NeedDataSource"
                    OnItemCommand="GridGoodsStock_ItemCommand" OnItemDataBound="GridGoodsStockItemDataBound" ShowFooter="True" ExportSettings="True">
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="GoodsId" ClientDataKeyNames="GoodsId">
                        <CommandItemTemplate>
                            <table width="100%">
                                <tr>
                                    <td width="65%">&nbsp;</td>
                                    <td style="text-align: right;">供应商：<rad:RadComboBox ID="RCB_Company" DataSource='<%# BindCompanyDataBound() %>' DataTextField="Value" DataValueField="Key" SelectedValue='<%# CompanyId %>' Filter="StartsWith" runat="server" Width="150" Height="200"></rad:RadComboBox></td>
                                    <td style="text-align: right;"><ce:CommonEnterSearch ID="CommonSearch1" SearchLabelText="商品名关键字：" CommandNameX="Search"
                                runat="server" SearchText='<%#SearchText %>' Width="300px" /></td>
                                    <td style="text-align: right;"><Ibt:ImageButtonControl ID="LBRefresh" Text="刷新" CommandName="RebindGrid" SkinType="Refresh"
                                runat="server"></Ibt:ImageButtonControl></td>
                                </tr>
                            </table>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                                <HeaderStyle Width="88px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderStyle-HorizontalAlign="Center">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="CurrentQuantity" HeaderText="w库存数量" UniqueName="CurrentQuantity">
                                <HeaderStyle Width="78px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="RecentInPrice" HeaderText="最后进价" UniqueName="RecentInPrice">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("RecentInPrice").ToString()))%>
                                </ItemTemplate>
                                <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="CurrentSumPrice" HeaderText="库存金额" UniqueName="CurrentSumPrice">
                                <ItemTemplate>
                                    &nbsp;<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("CurrentSumPrice"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="RecentCDate" HeaderText="进货时间" UniqueName="RecentCDate">
                                <ItemTemplate>
                                    <%# Convert.ToDateTime(Eval("RecentCDate"))==DateTime.MinValue?"": Eval("RecentCDate").ToString() %>
                                </ItemTemplate>
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="缺货" UniqueName="IsOnShelf">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%# GetIsTrue(Eval("IsScarcity"))%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="上架">
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%# GetIsTrue(Eval("IsOnShelf"))%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager ID="ClewWindowManager" runat="server" Title="商品库存详情查询" Height="600px"
        Width="900px" ReloadOnShow="true">
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="TVGoodsClass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GridGoodsStock"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="GridGoodsStock">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GridGoodsStock"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

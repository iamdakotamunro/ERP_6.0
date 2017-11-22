<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsPriceSerach.aspx.cs"
    Inherits="ERP.UI.Web.GoodsPriceSerach" MasterPageFile="~/MainMaster.master" %>
<%@ Import Namespace="ERP.Environment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table class="StagePanel">
        <tr>
            <td valign="top">
                <rad:RadTreeView ID="TVGoodsClass" runat="server" SkinID="Common" Height="550px"
                    Width="200px" CausesValidation="false" OnNodeClick="TvGoodsClassNodeClick">
                </rad:RadTreeView>
            </td>
            <td valign="top">
                <div style="padding-bottom: 10px;">
                    <rad:RadComboBox ID="rcb_Brand" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="所有品牌" runat="server" Height="200px" Style="padding-right: 10px;" OnItemsRequested="rcb_Brand_ItemsRequested"></rad:RadComboBox>
                    <rad:RadTextBox ID="txt_GoodsNameOrCode" runat="server" EmptyMessage="商品编号/名称" Width="300px" Style="margin-right: 10px;"></rad:RadTextBox>
                    <asp:Button ID="btnSearch" runat="server" Text="搜索" OnClick="btnSearch_Click" />
                </div>
                <rad:RadGrid ID="RG_GoodsPriceList" SkinID="CustomPaging" runat="server" OnItemDataBound="RG_GoodsPriceList_ItemDataBound" OnNeedDataSource="RG_GoodsPriceList_NeedDataSource">
                    <MasterTableView DataKeyNames="GoodsID" ClientDataKeyNames="GoodsID">
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle Height="0px" />
                        <Columns>
                            <rad:GridTemplateColumn HeaderText="商品编号" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    &nbsp;&nbsp;<%#Eval("GoodsCode") %>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="商品名称" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    &nbsp;&nbsp;<%#Eval("GoodsName")%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="采购价" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(GetGoodsPurchasePrice(new Guid(Eval("GoodsID").ToString()))) %>
                                </ItemTemplate>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="加盟价" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("JoinPrice"))%>
                                </ItemTemplate>
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderStyle-HorizontalAlign="Center">
                                <ItemStyle HorizontalAlign="Center" />
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
            <rad:RadWindow ID="ProductSalesStatistics" runat="server" Title="商品销量分析"></rad:RadWindow>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/jquery.js"></script>
        <script type="text/javascript">
            function ProductSales(goodsId, salefilialeId, salePlatformId) {
                window.radopen("./Windows/SalesRankingsChart.aspx?Id=" + goodsId + "&FilialeId=<%=GlobalConfig.ERPFilialeID%>&WarehouseId=<%=Guid.Empty%>&SeriesId=<%=Guid.Empty%>&SalefilialeId=" + salefilialeId + "&SalePlatformId=" + salePlatformId + "&StartTime=<%=DateTime.Now.AddDays(-30)%>&EndTime=<%=DateTime.Now%>", "ProductSalesStatistics");
                return false;
            }

            $(function () {
                removeClass();
            });

            //移除repeater中的部分样式
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
                    parentTd.css({ "border-left": "solid 1px #3d556c" });//设置包含table的td的左边框样式
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
                        $(obj).find("td").eq(index).css({ "border-left": "solid 1px #3d556c" });
                    }
                });
            }
            //设置td的宽度
            function SetTdWidth() {
                $(".rgMasterTable>thead table td[class='title']").each(function (i) {
                    var width = $(this).width();
                    $(".rgMasterTable>tbody table").each(function () {
                        $(this).find("td").eq(i).css("width",width);
                    });
                });
            }
        </script>
    </rad:RadScriptBlock>
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

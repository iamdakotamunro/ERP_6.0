<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.ShowGoodsStock"
    CodeBehind="ShowGoodsStock.aspx.cs" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>库存明细</title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script type="text/javascript" language="javascript">
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
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="ControlTools">
                    <Ibt:ImageButtonControl ID="LBXLS" runat="server" OnClick="Lbxls_Click" SkinType="ExportExcel"></Ibt:ImageButtonControl>
                </td>
            </tr>
        </table>
        <rad:RadGrid ID="RGGoodsStock" runat="server" AllowPaging="false" MasterTableView-CommandItemDisplay="None" SkinID="Common" OnNeedDataSource="RGGoodsStock_NeedDataSource" OnItemDataBound="GridGoodsStockItemDataBound">
            <ClientSettings>
                <%--<Scrolling AllowScroll="True" UseStaticHeaders="True" ScrollHeight="620px" />--%>
            </ClientSettings>
            <MasterTableView DataKeyNames="GoodsId" ClientDataKeyNames="GoodsId">
                <Columns>
                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderStyle-HorizontalAlign="Center">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="w库存数量">
                        <ItemTemplate>
                            &nbsp;<%# Eval("NonceWarehouseGoodsStock")%>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="UnitPrice" HeaderText="最后进价" UniqueName="UnitPrice"
                        DataFormatString="{0:F2}">
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="RecentInDate" HeaderText="最后进货时间" UniqueName="RecentInDate">
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RGGoodsStock">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGGoodsStock"  LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

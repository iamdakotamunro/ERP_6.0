<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="InternalPriceSet.aspx.cs" Inherits="ERP.UI.Web.InternalPriceSet" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
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

            function EditClick(obj) {
                window.radopen("./Windows/EditInternalPriceSet.aspx?GoodsType=" + obj, "RW1");
                return false;
            }

            function refreshGrid() {
                location.reload();
            }
        </script>
    </rad:RadScriptBlock>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="ControlTools" style="text-align: center">
                <asp:Panel ID="ControlPanel" runat="server">
                    <font size="3">商品内部采购价  =  及时结算价 * 预留利润比例</font>
                </asp:Panel>
            </td>
        </tr>
    </table>

    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RadGrid_InvoiceRollList" runat="server" SkinID="Common_Foot" OnNeedDataSource="RadGrid_InvoiceRollList_NeedDataSource"
                    OnItemDataBound="GridInvoiceRollListItemDataBound">
                    <MasterTableView DataKeyNames="GoodsType" ClientDataKeyNames="GoodsType" CommandItemDisplay="None">
                        <Columns>
                            <rad:GridTemplateColumn HeaderText="商品类型">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%# GetEnumName(Eval("GoodsType"))%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderStyle-HorizontalAlign="Center">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="操作" UniqueName="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Execute" SkinID="EditImageButton"
                                        Text="编辑" OnClientClick='<%# "return EditClick(\"" + Eval("GoodsType")+ "\")" %>' />
                                </ItemTemplate>
                                <HeaderStyle Width="220px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
     <rad:RadWindowManager ID="ClewWindowManager" runat="server" Title="修改内部价格设置" Height="400px"
        Width="600px" ReloadOnShow="true">
         <Windows>
             <rad:RadWindow ID="RW1" Width="400" Height="400" runat="server" Title="修改内部价格设置" />
         </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RTV_GoodsType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGrid_InvoiceRollList" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RTV_GoodsType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GroupGrid" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTV_GoodsType" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

</asp:Content>

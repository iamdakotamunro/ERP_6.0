<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsStockReport.aspx.cs"
    Inherits="ERP.UI.Web.GoodsStockReport" Title="库存金额月报" MasterPageFile="~/MainMaster.master" %>

<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl_1" Src="~/UserControl/ImageButtonControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <script src="JavaScript/jquery.js"></script>
    <script src="My97DatePicker/WdatePicker.js"></script>

    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="/JavaScript/telerik.js" type="text/javascript"></script>
        <script type="text/javascript">
            function GoodsStockDetails(type, year, month) {
                window.radopen("./Windows/GoodsStockReportForm.aspx?GoodsType=" + type + "&Year=" + year + "&Month=" + month, "GoodsStockReportForm");
                return false;
            }
        </script>
    </rad:RadScriptBlock>
    <table>
        <tr>
            <td>年月：</td>
            <td>
                <asp:TextBox ID="txt_YearAndMonth" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy',minDate:'{%y-1}',maxDate:'{%y+1}'})"></asp:TextBox>
            </td>
            <td>仓库：</td>
            <td>
                <asp:DropDownList ID="ddl_Warehouse" runat="server"></asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_GoodsStock" runat="server" SkinID="Common_Foot" OnNeedDataSource="RG_GoodsStock_NeedDataSource" ShowFooter="True">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridTemplateColumn DataField="TypeName" HeaderText="商品类型" UniqueName="MonthName">
                    <ItemTemplate>
                        <%# GetTypeName(Eval("GoodsType"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                    <FooterStyle HorizontalAlign="Right"></FooterStyle>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="January" HeaderText="1月" UniqueName="January">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',1)"
                            title="1月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("January"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="February" HeaderText="2月" UniqueName="February">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',2)"
                            title="2月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("February"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="March" HeaderText="3月" UniqueName="March">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',3)"
                            title="3月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("March"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="April" HeaderText="4月" UniqueName="April">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',4)"
                            title="4月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("April"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="May" HeaderText="5月" UniqueName="May">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',5)"
                            title="5月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("May"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="June" HeaderText="6月" UniqueName="June">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',6)"
                            title="6月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("June"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="July" HeaderText="7月" UniqueName="July">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',7)"
                            title="7月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("July"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="August" HeaderText="8月" UniqueName="August">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',8)"
                            title="8月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("August"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="September" HeaderText="9月" UniqueName="September">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',9)"
                            title="9月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("September"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="October" HeaderText="10月" UniqueName="October">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',10)"
                            title="10月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("October"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="November" HeaderText="11月" UniqueName="November">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',11)"
                            title="11月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("November"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="December" HeaderText="12月" UniqueName="December">
                    <ItemTemplate>
                        <a style="text-decoration: underline; cursor: pointer; color: #000;"
                            onclick="GoodsStockDetails('<%# Eval("GoodsType") %>','<%# txt_YearAndMonth.Text %>',12)"
                            title="12月份库存金额详情">
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("December"))%>
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="RWM" Height="440px" Width="750px" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="GoodsStockReportForm" runat="server" Title="商品库存明细">
            </rad:RadWindow>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RgGoodsStock">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgGoodsStock" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PurchaseInStockStatisticsForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.PurchaseStatisticsPageForm" %>

<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl" Src="~/UserControl/ImageButtonControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>采购统计详情</title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    </rad:RadScriptBlock>
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RgPurchaseStatisticsDetails" runat="server" SkinID="Common_Foot"
                    OnNeedDataSource="RgPurchaseStatisticsDetailsOnNeedDataSource" Skin="WebBlue"
                    OnItemCommand="RgPurchaseStatisticsDetailsOnItemCommand">
                    <MasterTableView>
                        <CommandItemTemplate>
                            <rad:RadTextBox ID="RTB_CompanyName" runat="server" Width="250px" Text='<%# CompanyName %>'
                                EmptyMessage="模糊供应商名称">
                            </rad:RadTextBox>
                            <Ibt:ImageButtonControl ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search"
                                SkinType="Search" Text="查询"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <Columns>
                            <rad:GridBoundColumn DataField="CompanyName" HeaderText="供应商名称" UniqueName="CompanyName">
                                <HeaderStyle Width="220px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                           <rad:GridTemplateColumn DataField="January" HeaderText="1月" UniqueName="January">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Jan" Text='<%# ShowStr(Eval("January"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="February" HeaderText="2月" UniqueName="February">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Feb" Text='<%# ShowStr(Eval("February"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="March" HeaderText="3月" UniqueName="March">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Mar" Text='<%# ShowStr(Eval("March"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="April" HeaderText="4月" UniqueName="April">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Apr" Text='<%# ShowStr(Eval("April"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="May" HeaderText="5月" UniqueName="May">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_May" Text='<%# ShowStr(Eval("May"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="June" HeaderText="6月" UniqueName="June">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Jun" Text='<%# ShowStr(Eval("June"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="July" HeaderText="7月" UniqueName="July">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_July" Text='<%# ShowStr(Eval("July"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="August" HeaderText="8月" UniqueName="August">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Aug" Text='<%# ShowStr(Eval("August"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="September" HeaderText="9月" UniqueName="September">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Sept" Text='<%# ShowStr(Eval("September"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="October" HeaderText="10月" UniqueName="October">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Oct" Text='<%# ShowStr(Eval("October"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="November" HeaderText="11月" UniqueName="November">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Nov" Text='<%# ShowStr(Eval("November"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="December" HeaderText="12月" UniqueName="December">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_December" Text='<%# ShowStr(Eval("December"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="YearTotalAmount" HeaderText="总采购金额" UniqueName="YearTotalAmount">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="LabYearTotalAmount" Text='<%# ShowStr(Eval("YearTotalAmount"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RGCompanyPaymentDaysDetails">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCompanyPaymentDaysDetails" LoadingPanelID="loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCompanyPaymentDaysDetails" LoadingPanelID="loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>


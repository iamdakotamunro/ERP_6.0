<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FundDetailForm.aspx.cs" Inherits="ERP.UI.Web.Windows.FundDetailForm" %>
<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl" Src="~/UserControl/ImageButtonControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
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
                <rad:RadGrid ID="RGFundDetails" runat="server" SkinID="Common_Foot" Skin="WebBlue" ShowFooter="True" OnNeedDataSource="RGFundDetails_OnNeedDataSource"
                OnItemCommand="RGFundDetails_OnItemCommand">
                    <MasterTableView>
                        <CommandItemTemplate>
                            <rad:RadTextBox ID="RTB_CompanyName" runat="server" Width="250px" Text='<%# BankName %>'
                                EmptyMessage="银行名称">
                            </rad:RadTextBox>
                            <Ibt:ImageButtonControl ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search"
                                SkinType="Search" Text="查询"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <Columns>
                            <rad:GridBoundColumn DataField="BankName" HeaderText="银行名称" UniqueName="BankName">
                                <HeaderStyle Width="220px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                           <rad:GridTemplateColumn DataField="Jan" HeaderText="1月" UniqueName="Jan">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_Jan" Text='<%# Convert.ToDecimal(Eval("Jan"))==0?string.Empty:Eval("Jan") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_Jan" Text='<%# Eval("MaxJanStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Feb" HeaderText="2月" UniqueName="Feb">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_Feb" Text='<%# Convert.ToDecimal(Eval("Feb"))==0?string.Empty:Eval("Feb") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_Jan" Text='<%#Eval("MaxFebStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Mar" HeaderText="3月" UniqueName="Mar">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_Mar" Text='<%# Convert.ToDecimal(Eval("Mar"))==0?string.Empty:Eval("Mar") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_Mar" Text='<%# Eval("MaxMarStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Apr" HeaderText="4月" UniqueName="Apr">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_Apr" Text='<%# Convert.ToDecimal(Eval("Apr"))==0?string.Empty:Eval("Apr") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_Apr" Text='<%# Eval("MaxAprStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="May" HeaderText="5月" UniqueName="May">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_May" Text='<%# Convert.ToDecimal(Eval("May"))==0?string.Empty:Eval("May") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_May" Text='<%# Eval("MaxMayStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Jun" HeaderText="6月" UniqueName="Jun">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_Jun" Text='<%# Convert.ToDecimal(Eval("Jun"))==0?string.Empty:Eval("Jun") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_Jun" Text='<%# Eval("MaxJunStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="July" HeaderText="7月" UniqueName="July">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_July" Text='<%# Convert.ToDecimal(Eval("July"))==0?string.Empty:Eval("July") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_July" Text='<%# Eval("MaxJulyStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Aug" HeaderText="8月" UniqueName="Aug">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_Aug" Text='<%# Convert.ToDecimal(Eval("Aug"))==0?string.Empty:Eval("Aug") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_Aug" Text='<%# Eval("MaxAugStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Sept" HeaderText="9月" UniqueName="Sept">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_Sept" Text='<%# Convert.ToDecimal(Eval("Sept"))==0?string.Empty:Eval("Sept") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_Sept" Text='<%# Eval("MaxSeptStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Oct" HeaderText="10月" UniqueName="Oct">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_Oct" Text='<%# Convert.ToDecimal(Eval("Oct"))==0?string.Empty:Eval("Oct") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_Oct" Text='<%# Eval("MaxOctStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Nov" HeaderText="11月" UniqueName="Nov">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_Nov" Text='<%# Convert.ToDecimal(Eval("Nov"))==0?string.Empty:Eval("Nov") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_Nov" Text='<%# Eval("MaxNovStr") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="December" HeaderText="12月" UniqueName="December">
                                <ItemTemplate>
                                    <%--<asp:Label runat="server" ID="Lab_December" Text='<%# Convert.ToDecimal(Eval("December"))==0?string.Empty:Eval("December") %>'></asp:Label>--%>
                                    <asp:Label runat="server" ID="Lab_December" Text='<%# Eval("MaxDecemberStr") %>'></asp:Label>
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
            <rad:AjaxSetting AjaxControlID="RGFundDetails">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGFundDetails" LoadingPanelID="loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGFundDetails" LoadingPanelID="loading">
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

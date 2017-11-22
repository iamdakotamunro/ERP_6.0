<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="FundSearch.aspx.cs" Inherits="ERP.UI.Web.FundSearch" Title="资金查询" MasterPageFile="~/MainMaster.master"%>

<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl_1" Src="~/UserControl/ImageButtonControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="/JavaScript/telerik.js" type="text/javascript"></script>
        <script type="text/javascript">
            function PaymentDaysDetails(filialeId, year) {
                window.radopen("./Windows/FundDetailForm.aspx?FilialeId=" + filialeId + "&Year=" + year, "CompanyPaymentDaysDetails");
                return false;
            }
        </script>
    </rad:RadScriptBlock>
   <table class="StagePanel">
        <tr>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RGFundsPaymentDays" runat="server" SkinID="Common_Foot"  ShowFooter="True" OnNeedDataSource="RGFundsPaymentDays_OnNeedDataSource"
                OnItemDataBound="RGFundsPaymentDays_OnItemDataBound" OnItemCommand="RGFundsPaymentDays_OnItemCommand">
                    <mastertableview>
                        <CommandItemTemplate>
                            <rad:RadComboBox ID="RCB_Year" runat="server" Width="150px" Height="200px">
                            </rad:RadComboBox>
                            <Ibt:ImageButtonControl_1 ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search"
                                SkinType="Search" Text="查询"></Ibt:ImageButtonControl_1>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl_1 ID="LBRefresh" runat="server" CommandName="RebindGrid"
                                SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl_1>
                        </CommandItemTemplate>
                        <Columns>
                            <rad:GridTemplateColumn DataField="SaleFilialeName" HeaderText="公司名称" UniqueName="SaleFilialeName">
                                <ItemTemplate>
                                    <a style="text-decoration: underline; cursor: pointer; font-weight: bold" onclick="PaymentDaysDetails('<%# Eval("SaleFilialeId") %>','<%# Eval("Year") %>')"
                                        title="点击查看应付款详情">
                                        <%# Eval("SaleFilialeName")%>
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle Width="220px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
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
                                    <asp:Label runat="server" ID="Lab_Feb" Text='<%# Eval("MaxFebStr") %>'></asp:Label>
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
                    </mastertableview>
                </rad:RadGrid>
         </td>
        </tr>
    </table>
    <rad:RadWindowManager ID="RWM" Height="640px" Width="1200px" runat="server" ReloadOnShow="true">
        <windows>
            <%--<rad:RadWindow ID="CompanyPaymentDaysDetails" runat="server" Title="资金详情">
            </rad:RadWindow>--%>
        </windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <ajaxsettings>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGFundsPaymentDays" LoadingPanelID="loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGFundsPaymentDays">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGFundsPaymentDays" LoadingPanelID="loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </ajaxsettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyPaymentDays.aspx.cs"
    Inherits="ERP.UI.Web.CompanyPaymentDays" Title="应付款查询" MasterPageFile="~/MainMaster.master" %>

<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl_1" Src="~/UserControl/ImageButtonControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="/JavaScript/telerik.js" type="text/javascript"></script>
        <script type="text/javascript">
            function PaymentDaysDetails(filialeId, year) {
                window.radopen("./Windows/CompanyPaymentDaysByFilialeForm.aspx?FilialeId=" + filialeId + "&Year=" + year, "CompanyPaymentDaysDetails");
                return false;
            }

        </script>
    </rad:RadScriptBlock>
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RGCompanyPaymentDays" runat="server" SkinID="Common_Foot" OnNeedDataSource="RgCompanyPaymentDaysOnNeedDataSource"
                    OnItemCommand="RGCompanyPaymentDays_OnItemCommand" OnItemDataBound="RGCompanyPaymentDays_OnItemDataBound" ShowFooter="True">
                    <mastertableview>
                        <CommandItemTemplate>
                            <rad:RadComboBox ID="RCB_Year" runat="server" Width="150px" Height="100px">
                            </rad:RadComboBox>
                            <Ibt:ImageButtonControl_1 ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search"
                                SkinType="Search" Text="查询"></Ibt:ImageButtonControl_1>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl_1 ID="LBRefresh" runat="server" CommandName="RebindGrid"
                                SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl_1>
                        </CommandItemTemplate>
                        <Columns>
                            <rad:GridTemplateColumn DataField="FilialeName" HeaderText="销售公司" UniqueName="FilialeName">
                                <ItemTemplate>
                                    <a style="text-decoration: underline; cursor: pointer; font-weight: bold" 
                                    onclick="PaymentDaysDetails('<%# Eval("FilialeId") %>','<%# Year %>')"
                                        title="点击查看应付款详情">
                                        <%# GetFilialeName(Eval("FilialeId"))%>
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle Width="180px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="January" HeaderText="1月" UniqueName="January">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Jan" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("January")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("January1"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="February" HeaderText="2月" UniqueName="February">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Feb" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("February")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("February2"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="March" HeaderText="3月" UniqueName="March">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Mar" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("March")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("March3"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="April" HeaderText="4月" UniqueName="April">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Apr" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("April")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("April4"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="May" HeaderText="5月" UniqueName="May">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_May" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("May")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("May5"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="June" HeaderText="6月" UniqueName="June">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Jun" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("June")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("June6"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="July" HeaderText="7月" UniqueName="July">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_July" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("July")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("July7"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="August" HeaderText="8月" UniqueName="August">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Aug" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("August")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("August8"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="September" HeaderText="9月" UniqueName="September">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Sept" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("September")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("September9"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="October" HeaderText="10月" UniqueName="October">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Oct" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("October")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("October10"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="November" HeaderText="11月" UniqueName="November">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_Nov" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("November")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("November11")))  %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="December" HeaderText="12月" UniqueName="December">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="Lab_December" Text='<%# string.Format("{0}<br/>[{1}]", 
                                        ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("December")),
                                         ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("December12"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                        </Columns>
                    </mastertableview>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager ID="RWM" Height="640px" Width="1180px" runat="server" ReloadOnShow="true">
        <windows>
            <rad:RadWindow ID="CompanyPaymentDaysDetails" runat="server" Title="应付款详情">
            </rad:RadWindow>
        </windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <ajaxsettings>
            <rad:AjaxSetting AjaxControlID="BtnAuto">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCompanyPaymentDays" >
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGCompanyPaymentDays">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCompanyPaymentDays" LoadingPanelID="loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </ajaxsettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

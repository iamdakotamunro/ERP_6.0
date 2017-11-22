<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SupplierSaleReport.aspx.cs"
    Inherits="ERP.UI.Web.SupplierSaleReport" Title="供应商销售月报" MasterPageFile="~/MainMaster.master" %>

<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl_1" Src="~/UserControl/ImageButtonControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="/JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RgSupplierSale" runat="server" SkinID="Common_Foot" OnNeedDataSource="RgSupplierSaleOnNeedDataSource"
                    OnItemCommand="RgSupplierSaleOnItemCommand" OnItemDataBound="RgSupplierSaleOnItemDataBound" ShowFooter="True">
                    <mastertableview>
                        <CommandItemTemplate>
                            <rad:RadComboBox ID="RcbYear" runat="server" Width="150px" Height="100px" >
                            </rad:RadComboBox>
                            &nbsp;&nbsp;&nbsp;
                            <rad:RadTextBox ID="RtbCompanyName" runat="server" Width="250px" Text='<%# CompanyName %>'
                                EmptyMessage="模糊供应商名称">
                            </rad:RadTextBox>
                            <Ibt:ImageButtonControl_1 ID="LbSearch" runat="server" CommandName="Search" ValidationGroup="Search"
                                SkinType="Search" Text="查询"></Ibt:ImageButtonControl_1>
                            &nbsp;&nbsp;&nbsp;
                        </CommandItemTemplate>
                        <Columns>
                            <rad:GridTemplateColumn DataField="CompanyName" HeaderText="供应商名称" UniqueName="CompanyName">
                                <ItemTemplate>
                                    <%# Eval("CompanyName") %>
                                </ItemTemplate>
                                <HeaderStyle Width="180px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="January" HeaderText="1月" UniqueName="January">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("January"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="February" HeaderText="2月" UniqueName="February">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("February"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="March" HeaderText="3月" UniqueName="March">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("March"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="April" HeaderText="4月" UniqueName="April">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("April"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="May" HeaderText="5月" UniqueName="May">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("May"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="June" HeaderText="6月" UniqueName="June">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("June"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="July" HeaderText="7月" UniqueName="July">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("July"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="August" HeaderText="8月" UniqueName="August">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("August"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="September" HeaderText="9月" UniqueName="September">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("September"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="October" HeaderText="10月" UniqueName="October">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("October"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="November" HeaderText="11月" UniqueName="November">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("November"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"></FooterStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="December" HeaderText="12月" UniqueName="December">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("December"))%>
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
            <rad:RadWindow ID="SupplierSaleReportForm" runat="server" Title="供应商销量明细">
            </rad:RadWindow>
        </windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <ajaxsettings>
            <rad:AjaxSetting AjaxControlID="RgSupplierSale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgSupplierSale" LoadingPanelID="loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </ajaxsettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

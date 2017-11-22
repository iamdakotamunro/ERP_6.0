<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="CostReportStatistics.aspx.cs" Inherits="ERP.UI.Web.CostReportStatistics" %>

<%@ Register Src="/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<%@ Register TagPrefix="asp" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2010.2.713.0, Culture=neutral, PublicKeyToken=29ac1a93ec063d92" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table class="PanelArea">
        <tr>
            <td>
                搜索起止时间：<rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="100px">
                </rad:RadDatePicker>
                -<rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="100px">
                </rad:RadDatePicker>
            </td>
            <td>
                公司：<asp:RadComboBox ID="RCB_Filliale" runat="server" Height="200px" Width="150px"
                    OnSelectedIndexChanged="RCB_Filliale_OnSelectedIndexChanged" AutoPostBack="True">
                </asp:RadComboBox>
                <asp:HiddenField runat="server" ID="HF_Filiale" />
            </td>
            <td>
                部门：<asp:RadComboBox ID="RCB_Branch" runat="server" Height="300px" Width="150px" OnSelectedIndexChanged="RCB_Branch_OnSelectedIndexChanged"
                    AutoPostBack="True">
                </asp:RadComboBox>
                <asp:HiddenField runat="server" ID="HF_Branch" />
            </td>
            <td>
                分类：<asp:RadComboBox ID="RCB_CompanyClass" runat="server" Height="300px">
                </asp:RadComboBox>
            </td>
            <td>
                <asp:ImageButton Style='vertical-align: middle' ID="IB_Search" runat="server" CommandName="Search"
                    ValidationGroup="Search" SkinID="SearchButton" OnClick="OnClick_Search" />
            </td>
        </tr>
        <tr>
            <td style="vertical-align: top;" colspan="5">
                <rad:RadGrid ID="RG_Report" runat="server" SkinID="Common_Foot" AllowSorting="true"
                    ShowFooter="true" OnNeedDataSource="RG_Report_NeedDataSource">
                    <ClientSettings>
                        <Selecting AllowRowSelect="True" EnableDragToSelectRows="false" />
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="CompanyId" ClientDataKeyNames="CompanyId" CommandItemDisplay="Top"
                        NoMasterRecordsText="无可用记录。">
                        <CommandItemTemplate>
                            <Ibt:ImageButtonControl ID="LB_Refresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <ExpandCollapseColumn Visible="False">
                            <HeaderStyle Width="19px" />
                        </ExpandCollapseColumn>
                        <RowIndicatorColumn Visible="False">
                            <HeaderStyle Width="20px" />
                        </RowIndicatorColumn>
                        <CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>
                        <Columns>
                            <rad:GridTemplateColumn DataField="CompanyId" HeaderText="费用类型" UniqueName="CompanyId">
                                <ItemTemplate>
                                    <asp:Label ID="LB_ExecutePosition" runat="server" Text='<%# GetClass(Eval("CompanyId")) %>'
                                        SkinID="StandardInput1"></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="金额" UniqueName="PayCost">
                                <ItemTemplate>
                                    <asp:Label ID="LB_PayCost" runat="server" Text='<%# Convert.ToDecimal(Eval("RealityCost"))==0?"":Math.Abs(Convert.ToDecimal(Eval("RealityCost"))).ToString("##,###,##0.00") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center" Font-Bold="True" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript">
            function RowDblClick(obj, args) {
                debugger;
                var CompanyId = args.getDataKeyValue("CompanyId");
                var filialeId = document.getElementById("<%=HF_Filiale.ClientID%>").value;
                var BranchId = document.getElementById("<%=HF_Branch.ClientID%>").value;
                var StartDate = $("input[name$='RDP_StartTime_dateInput_text']").val();
                var EndDate = $("input[name$=RDP_EndTime_dateInput_text]").val();
                window.radopen("./Windows/CostReportDetail.aspx?CompanyId=" + CompanyId + "&FilialeId=" + filialeId + "&BranchId=" + BranchId + "&StartDate=" + encodeURI(StartDate) + "&EndDate=" + encodeURI(EndDate), "RW1");
            }
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
        </script>
    </rad:RadScriptBlock>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RAM_AjaxRequest">
        <AjaxSettings>
            <%-- <rad:AjaxSetting AjaxControlID="RCB_Filliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Branch" />
                </UpdatedControls>
            </rad:AjaxSetting>--%>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_Report" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="TB_TotalAmount" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RG_Report">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_Report" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="TB_TotalAmount" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_Report" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Branch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="HF_Branch" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadWindowManager ID="TransferWindowManager" runat="server" Height="240px" Width="400px"
        ReloadOnShow="true" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="RW1" runat="server" Title="明细" Width="900px" Height="400px" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

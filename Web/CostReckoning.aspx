<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.CostReckoningAw" CodeBehind="CostReckoning.aspx.cs" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
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
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <span style="font-weight: bold">公司:</span>
                <rad:RadComboBox ID="RCB_FilialeList" runat="server" DropDownWidth="180px" AutoPostBack="True"
                    AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择公司"
                    CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="Rcb_FilialeListSelectedIndexChanged">
                </rad:RadComboBox>
                <br />
                <br />
                <rad:RadTreeView ID="RTVCompanyCussent" runat="server" UseEmbeddedScripts="false"
                    Height="500px" Width="250px" AutoPostBack="true" CausesValidation="True" OnNodeClick="RadTreeViewCompanyCussent_NodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <div align="left">
                    <table>
                        <tr class="rgCommandRow">
                            <td class="rgCommandCell">
                                时间段:
                            </td>
                            <td>
                                <rad:RadDatePicker ID="RDP_StartDate" runat="server" Width="95px">
                                </rad:RadDatePicker>
                            </td>
                            <td>
                                &nbsp; 至 &nbsp;
                            </td>
                            <td>
                                <rad:RadDatePicker ID="RDP_EndDate" runat="server" Width="95px">
                                </rad:RadDatePicker>
                            </td>
                            <td>
                                &nbsp;&nbsp; 单据类型:
                            </td>
                            <td>
                                <rad:RadComboBox ID="RCB_ReceiptType" Width="100px" runat="server">
                                    <Items>
                                        <rad:RadComboBoxItem ID="RadComboBoxItem1" runat="server" Value="-1" Text="全部" />
                                        <rad:RadComboBoxItem ID="RadComboBoxItem2" runat="server" Value="0" Text="应收增加" />
                                        <rad:RadComboBoxItem ID="RadComboBoxItem3" runat="server" Value="1" Text="应收减少" />
                                    </Items>
                                </rad:RadComboBox>
                            </td>
                        <%--    <td>
                                &nbsp;&nbsp;单据状态:
                            </td>--%>
                           <%-- <td>
                                <rad:RadComboBox ID="RCB_Auditing" Width="100px" runat="server" UseEmbeddedScripts="false"
                                    ShowToggleImage="True" OnSelectedIndexChanged="RCB_Auditing_SelectedIndexChanged"
                                    AutoPostBack="true">
                                    <Items>
                                        <rad:RadComboBoxItem runat="server" Value="0" Text="未审核" />
                                        <rad:RadComboBoxItem runat="server" Value="1" Text="已审核" />
                                    </Items>
                                </rad:RadComboBox>
                            </td>--%>
                            <td>
                                <Ibt:ImageButtonControl ID="LB_Search" runat="server" OnClick="LB_Search_Click" SkinType="Search"
                                    Text="检索"></Ibt:ImageButtonControl>
                            </td>
                            <td>
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <Ibt:ImageButtonControl ID="LBXLS" runat="server" OnClick="LBXLS_Click" SkinType="ExportExcel"
                                    Text="导出EXCEL"></Ibt:ImageButtonControl>
                            </td>
                        </tr>
                    </table>
                </div>
                <rad:RadGrid ID="RGReckoning" runat="server" SkinID="Common_Foot" OnNeedDataSource="RGReckoning_NeedDataSource">
                    <ClientSettings>
                    </ClientSettings>
                    <MasterTableView DataKeyNames="ReckoningId,AuditingState,ReckoningType" ClientDataKeyNames="ReckoningId,AuditingState,ReckoningType"
                        CurrentResetPageIndexAction="SetPageIndexToLast">
                        <CommandItemTemplate>
                            <asp:LinkButton ID="LBRefresh" runat="server" CommandName="RebindGrid">
                                <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />
                                刷新</asp:LinkButton>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="TradeCode" HeaderText="单据编号" UniqueName="TradeCode">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="DateCreated" HeaderText="创建日期" UniqueName="DateCreated">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Description" HeaderText="备注说明" UniqueName="Description">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="AccountReceivable" HeaderText="应收款增加" UniqueName="AccountReceivable">
                                <ItemTemplate>
                                    <asp:Label ID="AccountPaymentLabel" runat="server" Text='<%# (ReckoningType)Eval("ReckoningType")==ReckoningType.Income ? ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("AccountReceivable"))) : " " %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="AccountReceivable" HeaderText="应收款减少" UniqueName="AccountPaymentLabel">
                                <ItemTemplate>
                                    <asp:Label ID="AccountReceivableLabel" runat="server" Text='<%# (ReckoningType)Eval("ReckoningType")==ReckoningType.Defray ? ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(-Convert.ToDecimal(Eval("AccountReceivable"))) : " " %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="NonceTotalled" HeaderText="应收合计" UniqueName="NonceTotalled">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("NonceTotalled"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="AuditingState" HeaderText="单据状态" UniqueName="AuditingState">
                                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:Label ID="Lab_AuditingState" runat="server" Text='<%# Convert.ToInt32(Eval("auditingState").ToString())<1 ? "未审核" : "已审核" %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ExportSettings>
                        <Pdf PageBottomMargin="" PageFooterMargin="" PageHeaderMargin="" PageHeight="11in"
                            PageLeftMargin="" PageRightMargin="" PageTopMargin="" PageWidth="8.5in" />
                    </ExportSettings>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager ID="WMReckoning" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="RW1" runat="server" Height="250px" Width="400px" Title="填写单据" />
            <rad:RadWindow ID="RW2" runat="server" Height="170px" Width="400px" Title="单据详情" />
            <rad:RadWindow ID="RW3" runat="server" Height="300px" Width="400px" Title="单据详情" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RAM_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RTVCompanyCussent">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RCB_ReceiptType"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartDate"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RDP_EndDate"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="LB_CompanyCussentInfo"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGReckoning">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Auditing">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

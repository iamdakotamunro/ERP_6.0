<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReportDetail.aspx.cs"
    Inherits="ERP.UI.Web.Windows.CostReportDetail" %>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<form id="form1" runat="server">
<rad:RadScriptManager ID="RSM" runat="server">
</rad:RadScriptManager>
<rad:RadScriptBlock ID="RSB" runat="server">
    <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    <script type="text/javascript">
        function ShowImg(obj) {
            var object = eval(obj);
            object.style.display = "block";
        }

        function HiddleImg(obj) {
            var object = eval(obj);
            object.style.display = "none";
        }
    </script>
</rad:RadScriptBlock>
<table class="StagePanel">
    <tr>
        <td style="vertical-align: top;">
            <rad:RadGrid ID="RG_Report" runat="server" SkinID="Common_Foot" ShowFooter="true"
                OnNeedDataSource="RG_Report_NeedDataSource">
                <MasterTableView DataKeyNames="ReportId" ClientDataKeyNames="ReportId" NoMasterRecordsText="无可用记录。">
                    <CommandItemTemplate>
                    </CommandItemTemplate>
                    <Columns>
                        <rad:GridBoundColumn DataField="ReportNo" UniqueName="ReportNo" HeaderText="申报编号">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn HeaderText="报销类型" UniqueName="ReportKind">
                            <ItemTemplate>
                                <asp:label id="LB_ReportKind" runat="server" text='<%# GetReportKindName(int.Parse(Eval("ReportKind").ToString())) %>'></asp:label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridBoundColumn DataField="ReportName" UniqueName="ReportName" HeaderText="申报名称">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="ReportDate" UniqueName="ReportDate" HeaderText="申报时间"
                            DataFormatString="{0:D}">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn HeaderText="金额" UniqueName="PayCost">
                            <ItemTemplate>
                                <asp:label id="LB_PayCost" runat="server" text='<%# Math.Abs(Convert.ToDecimal(Eval("PayCost"))).ToString("##,###,##0.00") %>'></asp:label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <FooterStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="票据类型" UniqueName="InvoiceType">
                            <ItemTemplate>
                                <asp:label id="LB_InvoiceType" runat="server" text='<%# GetInvoiceType(Eval("InvoiceType"))%>'></asp:label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="申报人" UniqueName="ReportPersonnelId">
                            <ItemTemplate>
                                <asp:label id="LB_ReportPersonnelId" runat="server" text='<%# GetUserName(Eval("ReportPersonnelId"))%>'></asp:label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="备注" UniqueName="Memo">
                            <ItemTemplate>
                                <asp:imagebutton id="ClewImageButton" runat="server" skinid="InsertImageButton" onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                    onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                                <div style="position: absolute;">
                                    <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative;
                                        display: none; background-color: #CCFFFF; border: solid 1px #666; width: 250px;
                                        font-weight: bold; height: auto; overflow: visible; word-break: break-all;" runat="server">
                                        <%# Eval("Memo").ToString() %>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </rad:RadGrid>
        </td>
    </tr>
</table>
<rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" UseEmbeddedScripts="false">
    <AjaxSettings>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
</rad:RadAjaxLoadingPanel>
</form>
</html>

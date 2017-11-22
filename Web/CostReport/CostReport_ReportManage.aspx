<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="CostReport_ReportManage.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_ReportManage" %>

<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.BLL.Implement.Inventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <script src="../My97DatePicker/WdatePicker.js"></script>
    <script src="../JavaScript/ToolTipMsg.js"></script>
    <script src="../JavaScript/GiveTip.js"></script>
    <table>
        <tr>
            <td style="text-align: right;">申报编号：
            </td>
            <td>
                <asp:TextBox ID="txt_ReportNo" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: right;">申报时间：
            </td>
            <td>
                <asp:TextBox ID="txt_ReportDateStart" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
                至
                <asp:TextBox ID="txt_ReportDateEnd" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
            </td>

            <td style="text-align: right;">费用名称：
            </td>
            <td>
                <asp:TextBox ID="txt_ReportName" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: right;">处理状态：
            </td>
            <td>
                <asp:DropDownList ID="ddl_State" runat="server">
                    <asp:ListItem Text="全部" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="已处理" Value="0"></asp:ListItem>
                    <asp:ListItem Text="未处理" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
                <input type="button" value="添加" onclick="Add()" />
            </td>
        </tr>
    </table>

    <rad:RadGrid ID="RG_Report" runat="server" ShowFooter="true" OnNeedDataSource="RG_Report_NeedDataSource" OnItemDataBound="RG_Report_ItemDataBound">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridBoundColumn DataField="ReportNo" HeaderText="申报编号">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="申报类型">
                    <ItemTemplate>
                        <%#EnumAttribute.GetKeyName((CostReportKind)Convert.ToInt32(Eval("ReportKind")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ReportDate" HeaderText="申报时间" DataFormatString="{0:yyyy-MM-dd}">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ReportName" HeaderText="费用名称">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="费用分类" UniqueName="TotalName">
                    <ItemTemplate>
                        <%#Cost.ReadInstance.GetCompanyName(new Guid(Eval("CompanyClassId").ToString()),new Guid(Eval("CompanyId").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <FooterStyle HorizontalAlign="Right"></FooterStyle>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报金额" UniqueName="ReportCost">
                    <ItemTemplate>
                        <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(Convert.ToDecimal(Eval("ReportCost")))) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="已付款金额" UniqueName="PayCost">
                    <ItemTemplate>
                        <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(Convert.ToDecimal(Eval("PayCost")))) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="付款时间">
                    <ItemTemplate>
                        <%#Convert.ToDateTime(Eval("ExecuteDate")).ToString("yyyy-MM-dd").Equals("1900-01-01")?"":Convert.ToDateTime(Eval("ExecuteDate")).ToString("yyyy-MM-dd") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="票据类型">
                    <ItemTemplate>
                        <%#EnumAttribute.GetKeyName((CostReportInvoiceType)Convert.ToInt32(Eval("InvoiceType")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="处理状态">
                    <ItemTemplate>
                        <%# GetReportState(Eval("State").ToString()) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="票据截止日期">
                    <ItemTemplate>
                        <%# GetFinallySubmitTicketDate(Eval("ReportDate").ToString()) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="75px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="80%" tooltipmsg='<%#Eval("Memo").ToString().Replace("\n","<br/>")%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <ItemTemplate>
                        <asp:Button ID="btn_Control" runat="server" OnClientClick='<%# "return Edit(\"" + Eval("ReportId") + "\",\"" + Eval("State")+ "\",\"" + Eval("ReportKind")+ "\",\"" + Math.Abs(Convert.ToDecimal(Eval("PayCost")))+ "\",\"" + Eval("IsSystem") + "\");" %>' />
                        <asp:Button ID="btn_Del" runat="server" Text="作废" CommandArgument='<%# Eval("ReportId") %>' OnClientClick="return GiveTip(event,'您确定作废吗？')" OnClick="btn_Del_Click" Enabled='<%# (Math.Abs(Convert.ToDecimal(Eval("PayCost"))).Equals(0)&&Eval("State").Equals((int)CostReportState.Auditing))||Eval("State").Equals((int)CostReportState.AuditingNoPass)||(Convert.ToInt32(Eval("ReportKind")).Equals((int)CostReportKind.Later)&&Eval("State").Equals((int)CostReportState.InvoiceNoPass))%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="90px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadWindowManager runat="server">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="费用申报" Width="1010px" Height="650px" />
            <rad:RadWindow ID="rawCK" runat="server" Title="费用申报详情" Width="1000px" Height="650px" />
        </Windows>
    </rad:RadWindowManager>

    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
        });

        //添加
        function Add() {
            window.radopen("../CostReport/CostReport_Control.aspx", "raw");
        }

        //编辑按钮事件
        function Edit(reportId, state, reportKind, payCost, isSystem) {
            if ((reportKind === '1' && payCost === '0.00' && (state === '1' || state === '2' || state === '3')) || (reportKind === '2' && (state === '1' || state === '2' || state === '3')) || (reportKind === '3' && (state === '2' || state === '3')) || (reportKind === '4' && (state === '2' || state === '3'))) {
                window.radopen("../CostReport/CostReport_Control.aspx?ReportId=" + reportId + "&ReportKind=" + reportKind + "&Type=1", "raw");
            } else if ((reportKind === '1' && payCost !== '0.00' && (state === '1' || (isSystem==='False' && state === '2') || state === '3' || state === '21')) || (reportKind === '3' && state === '1')) {
                window.radopen("../CostReport/CostReport_CheckBill.aspx?ReportId=" + reportId, "raw");
            }
            else {
                window.radopen("../CostReport/CostReport_Look.aspx?ReportId=" + reportId, "rawCK");
            }
            return false;
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
    </script>
</asp:Content>

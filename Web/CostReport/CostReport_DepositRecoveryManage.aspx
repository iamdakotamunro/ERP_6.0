<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="CostReport_DepositRecoveryManage.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_DepositRecoveryManage" %>

<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.BLL.Implement.Inventory" %>
<%@ Import Namespace="ERP.Enum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table>
        <tr>
            <td style="text-align: right;">预借款/押金编号：
            </td>
            <td>
                <asp:TextBox ID="txt_ReportNo" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: right;">回收状态：
            </td>
            <td>
                <asp:DropDownList ID="ddl_State" runat="server" onchange="ChangeTimeType();">
                    <asp:ListItem Text="未回收" Value="0" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="已回收" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">
                <b id="TimeTitle" style="color: red;">付款</b>时间：
                <asp:HiddenField ID="Hid_TimeType" runat="server" Value="0" />
            </td>
            <td>
                <asp:TextBox ID="txt_DateTimeStart" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
                至
                <asp:TextBox ID="txt_DateTimeEnd" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
            </td>
            <td style="text-align: right;">申请人：
            </td>
            <td>
                <rad:RadComboBox ID="RCB_ReportPersonnel" runat="server" CausesValidation="false"
                    AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="Value" DataValueField="Key"
                    Width="220px" Height="200px" OnItemsRequested="SearchItemsRequested"
                    EmptyMessage="可按工号，名字搜索">
                </rad:RadComboBox>
            </td>
            <td>
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
            </td>
            <td>
                <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="Ib_ExportData_Click" />
            </td>
        </tr>
    </table>

    <rad:RadGrid ID="RG_ReportDepositRecovery" runat="server" ShowFooter="true" OnNeedDataSource="RG_ReportDepositRecovery_NeedDataSource" OnItemDataBound="RG_ReportDepositRecovery_ItemDataBound">
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>
        <MasterTableView DataKeyNames="ReportId" ClientDataKeyNames="ReportId">
            <CommandItemTemplate>
                <asp:LinkButton ID="LB_UpdateReportPersonnel" runat="server" OnClientClick="return UpdateReportPersonnelClick();" Visible='<%# GetPowerOperationPoint("UpdateReportPersonnel") %>'>
                    <asp:Image ID="Image2" runat="server" ImageAlign="AbsMiddle" SkinID="InsertImageButton" />转移申请人   
                </asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="预借款申报编号">
                    <ItemTemplate>
                        <%# Eval("ReportNo") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="押金编号">
                    <ItemTemplate>
                        <%# Eval("DepositNo") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="结算公司">
                    <ItemTemplate>
                        <%#ERP.UI.Web.Common.CacheCollection.Filiale.GetFilialeNameAndFilialeId(new Guid(Eval("PayBankAccountId").ToString())).Split(',')[0]%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报人">
                    <ItemTemplate>
                        <%#new ERP.BLL.Implement.Organization.PersonnelManager().GetName(new Guid(Eval("ReportPersonnelId").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="PayCompany" HeaderText="付款单位">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ReportDate" HeaderText="申报时间" DataFormatString="{0:yyyy-MM-dd}">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ReportName" HeaderText="费用名称">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="费用分类">
                    <ItemTemplate>
                        <%#Cost.ReadInstance.GetCompanyName(new Guid(Eval("CompanyClassId").ToString()),new Guid(Eval("CompanyId").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="BankName" HeaderText="结算账号">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="BankAccount" HeaderText="收款账号" UniqueName="TotalName">
                    <FooterStyle HorizontalAlign="Right"></FooterStyle>
                </rad:GridBoundColumn>
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
                <rad:GridBoundColumn DataField="AuditingDate" HeaderText="审核时间" DataFormatString="{0:yyyy-MM-dd}">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="付款时间">
                    <ItemTemplate>
                        <%#Convert.ToDateTime(Eval("ExecuteDate")).ToString("yyyy-MM-dd").Equals("1900-01-01")?"":Convert.ToDateTime(Eval("ExecuteDate")).ToString("yyyy-MM-dd") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="票据类型">
                    <ItemTemplate>
                        <%#EnumAttribute.GetKeyName((ERP.Enum.CostReportInvoiceType)Convert.ToInt32(Eval("InvoiceType")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="回收单据号">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="回收金额">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="回收时间">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="回收状态">
                    <ItemTemplate>
                        <%# ddl_State.SelectedItem.ToString() %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注说明">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="80%" tooltipmsg='<%#Eval("Memo").ToString().Replace("\n","<br/>")%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <ItemTemplate>
                        <asp:Button ID="btn_Recovery" runat="server" Text="回收" OnClientClick='<%# "return Recovery(\"" + Eval("ReportId") + "\");" %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="申请人转移" Width="700px" Height="550px" />
            <rad:RadWindow ID="rawR" runat="server" Title="押金回收" Width="600px" Height="300px" />
        </Windows>
    </rad:RadWindowManager>

    <script src="../My97DatePicker/WdatePicker.js"></script>
    <script src="../JavaScript/ToolTipMsg.js"></script>
    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
            ChangeTimeType();//根据状态切换时间类型
        });

        //根据状态切换时间类型
        function ChangeTimeType() {
            var selectedValue = $("select[id$='ddl_State']").val();
            var timeTitle = $("#TimeTitle");
            var hidTimeType = $("input[id$='Hid_TimeType']");
            if (selectedValue === "0") {
                timeTitle.text("付款");
                hidTimeType.val("0");
            } else {
                timeTitle.text("回收");
                hidTimeType.val("1");
            }
        }

        function RowDblClick(obj, args) {
            window.radopen("../CostReport/CostReport_DepositRecoveryDetail.aspx?ReportId=" + args.getDataKeyValue("ReportId"), "rawR");
        }

        //回收按钮事件
        function Recovery(reportId) {
            window.radopen("../CostReport/CostReport_DepositRecoveryAdd.aspx?ReportId=" + reportId + "", "rawR");
            return false;
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }

        //申请人转移
        function UpdateReportPersonnelClick() {
            window.radopen("../CostReport/CostReport_DepositRecoveryUpdateReportPersonnel.aspx", "raw");
            return false;
        }
    </script>
</asp:Content>

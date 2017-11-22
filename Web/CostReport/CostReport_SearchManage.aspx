<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="CostReport_SearchManage.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_SearchManage" %>

<%@ Import Namespace="ERP.UI.Web.Common" %>
<%@ Import Namespace="ERP.BLL.Implement.Organization" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.BLL.Implement.Inventory" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <script src="../My97DatePicker/WdatePicker.js"></script>
    <script src="../JavaScript/ToolTipMsg.js"></script>
    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">申报编号：
            </td>
            <td>
                <asp:TextBox ID="txt_ReportNo" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: right;">申报类型：
            </td>
            <td>
                <asp:DropDownList ID="ddl_ReportKind" runat="server" Width="80px"></asp:DropDownList>
            </td>
            <td style="text-align: right;">结算公司：
            </td>
            <td>
                <asp:DropDownList ID="ddl_AssumeFiliale" runat="server" OnSelectedIndexChanged="ddl_AssumeFiliale_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
            </td>
            <td id="ReportCost" runat="server" visible="False" style="text-align: right;">申报金额：
            </td>
            <td id="ddlReportCost" runat="server" visible="False">
                <asp:DropDownList ID="ddl_ReportCost" runat="server" Width="80px">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    <asp:ListItem Text=">=1,000" Value="1000"></asp:ListItem>
                    <asp:ListItem Text=">=5,000" Value="5000"></asp:ListItem>
                    <asp:ListItem Text=">=10,000" Value="10000"></asp:ListItem>
                    <asp:ListItem Text=">=50,000" Value="50000"></asp:ListItem>
                    <asp:ListItem Text=">=100,000" Value="100000"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">申报人：
            </td>
            <td>
                <rad:RadComboBox ID="rcb_ReportPersonnel" Width="172px" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入申报人" runat="server" Height="200px" OnItemsRequested="rcb_ReportPersonnel_ItemsRequested"></rad:RadComboBox>
            </td>
            <td style="text-align: right;">收/付款状态：
            </td>
            <td>
                <asp:DropDownList ID="ddl_PayState" runat="server" Width="80px" onchange="ChangeBillState()">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    <asp:ListItem Text="未收/付款" Value="0"></asp:ListItem>
                    <asp:ListItem Text="已收/付款" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">结算账号：
            </td>
            <td>
                <rad:RadComboBox ID="rcb_PayBankAccount" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入结算账户" runat="server" Height="200px" OnItemsRequested="rcb_PayBankAccount_ItemsRequested"></rad:RadComboBox>
            </td>
            <td id="ReviewState" runat="server" visible="False" style="text-align: right;">审阅状态：
            </td>
            <td id="ddlReviewState" runat="server" visible="False">
                <asp:DropDownList ID="ddl_ReviewState" runat="server" Width="80px">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    <asp:ListItem Text="已审阅" Value="1"></asp:ListItem>
                    <asp:ListItem Text="未审阅" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">费用名称：
            </td>
            <td>
                <asp:TextBox ID="txt_ReportName" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: right;">票据状态：</td>
            <td>
                <asp:DropDownList ID="ddl_BillState" runat="server" Width="80px">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">处理状态：
            </td>
            <td>
                <rad:RadComboBox ID="Rcb_State" runat="server" Width="172px">
                    <ItemTemplate>
                        <input type="checkbox" name="State" onclick="<%# Eval("Key").ToString()=="-1"? "checkAll();":"checkValue();" %>" value='<%# Eval("Key") %>' /><%# Eval("Value") %>
                    </ItemTemplate>
                </rad:RadComboBox>
                <asp:HiddenField ID="Hid_State" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">费用分类：
            </td>
            <td>
                <asp:DropDownList ID="ddl_CompanyClass" runat="server" OnSelectedIndexChanged="ddl_CompanyClass_SelectedIndexChanged" AutoPostBack="True">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                </asp:DropDownList>
                <asp:DropDownList ID="ddl_FeeType" runat="server">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">票据类型：
            </td>
            <td>
                <asp:DropDownList ID="ddl_InvoiceType" runat="server" Width="80px">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    <asp:ListItem Text="普通发票" Value="1"></asp:ListItem>
                    <asp:ListItem Text="增值税专用发票" Value="5"></asp:ListItem>
                    <asp:ListItem Text="收据" Value="2"></asp:ListItem>
                    <asp:ListItem Text="网页凭证" Value="3"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">
                <b id="TimeTitle" style="color: red;">申报</b>时间：
                <asp:HiddenField ID="Hid_TimeType" runat="server" Value="1" />
            </td>
            <td>
                <asp:TextBox ID="txt_DateTimeStart" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
                至
                <asp:TextBox ID="txt_DateTimeEnd" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="8" style="text-align: center;">
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
                <asp:Button ID="btn_ExportExcel" runat="server" Text="导出Excel" OnClick="btn_ExportExcel_Click" OnClientClick="return GiveTip(event,'您确定要导出Excel吗？')" />
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="Hid_Search" runat="server" Value="1" />

    <rad:RadGrid ID="RG_Report" runat="server" ShowFooter="true" OnNeedDataSource="RG_Report_NeedDataSource" OnItemDataBound="RG_Report_ItemDataBound">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="申报编号" UniqueName="TotalName">
                    <ItemTemplate>
                        &nbsp;<%# Eval("ReportNo")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <FooterStyle HorizontalAlign="Right"></FooterStyle>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="结算公司">
                    <ItemTemplate>
                        <%#CacheCollection.Filiale.GetFilialeNameAndFilialeId(new Guid(Eval("PayBankAccountId").ToString())).Split(',')[0]%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报人">
                    <ItemTemplate>
                        <%#new PersonnelManager().GetName(new Guid(Eval("ReportPersonnelId").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="费用归属部门">
                    <ItemTemplate>
                        <%# GetCostAttributionDepartment(new Guid(Eval("AssumeBranchId").ToString()),new Guid(Eval("AssumeGroupId").ToString()),new Guid(Eval("AssumeShopId").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="PayCompany" HeaderText="收/付款单位">
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
                <rad:GridTemplateColumn HeaderText="票据金额" UniqueName="ApplyForCost">
                    <ItemTemplate>
                        <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(Convert.ToDecimal(Eval("ApplyForCost")))) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="审核时间">
                    <ItemTemplate>
                        <%#Convert.ToDateTime(Eval("AuditingDate")).ToString("yyyy-MM-dd").Equals("1900-01-01")?"":Convert.ToDateTime(Eval("AuditingDate")).ToString("yyyy-MM-dd") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
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
                        <%#GetReportState(Eval("State").ToString(),Convert.ToInt32(Eval("RealityCost")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="审阅状态">
                    <ItemTemplate>
                        <%#int.Parse(Eval("ReviewState").ToString()).Equals(0)?"未审阅":"已审阅"%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
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
                        <input type="button" value="查看" onclick="Look(<%#"'"+Eval("ReportId")+"'"%>)" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="rawCK" runat="server" Title="费用申报详情" Width="1000px" Height="650px" />
        </Windows>
    </rad:RadWindowManager>
    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
        });

        function ChangeTimeType(selectedValue) {
            var timeTitle = $("#TimeTitle");
            var hidTimeType = $("input[id$='Hid_TimeType']");
            if (selectedValue==="7") {
                timeTitle.text("完成");
                hidTimeType.val("0");
            } else {
                timeTitle.text("申报");
                hidTimeType.val("1");
            }
        }

        function ChangeBillState() {
            var ddlPayState = $("select[id$='ddl_PayState']");
            var ddlBillState = $("select[id$='ddl_BillState']");

            if (ddlPayState.val()==="0") {
                ddlBillState.val("0");
                ddlBillState.attr("disabled","disabled");
            } else {
                ddlBillState.val("");
                ddlBillState.removeAttr("disabled");
            }
        }

        //查看
        function Look(reportId) {
            window.radopen("../CostReport/CostReport_Look.aspx?ReportId=" + reportId+"&Review=1", "rawCK");
        }

        //获取单选框的值
        function checkValue() {
            var isCheckAll = true;
            var stateStr = "";
            $("input[type='checkbox'][name='State']:not([value='-1'])").each(function () {
                if ($(this).is(':checked')) {
                    stateStr = "," + $(this).val() + stateStr;
                } else {
                    isCheckAll = false;
                }
            });

            //赋值
            $("input[id$='Hid_State']").val(stateStr.substring(1));

            ChangeTimeType($("input[id$='Hid_State']").val());

            //判断全选按钮是否选中
            if (isCheckAll) {
                $("input[value='-1'][name='State']").prop('checked', true);
            } else {
                $("input[value='-1'][name='State']").prop('checked', false);
            }
        }

        //全选
        function checkAll() {
            var stateStr = "";
            if ($("input[value='-1'][name='State']").is(':checked')) {
                $("input[type='checkbox'][name='State']").prop('checked', true);
                $("input[type='checkbox'][name='State']:not([value='-1'])").each(function () {
                    stateStr = "," + $(this).val() + stateStr;
                });
            } else {
                $("input[type='checkbox'][name='State']").prop('checked', false);
            }

            //赋值
            $("input[id$='Hid_State']").val(stateStr.substring(1));

            ChangeTimeType($("input[id$='Hid_State']").val());
        }

        //显示选中值
        function ShowValue(value) {
            if (value.length === 0) {
                return;
            }
            ChangeTimeType(value);
            var arr = value.split(',');
            $("input[type='checkbox'][name='State']").each(function () {
                if (arr.indexOf($(this).val()) > -1) {
                    $(this).attr('checked', true);
                } else {
                    $(this).attr('checked', false);
                }
            });

            var checkItemLength = $("input[type='checkbox'][name='State']:not([value='-1'])").length;
            var checkedLength = $("input[type='checkbox'][name='State']:checked").length;
            if (checkItemLength === checkedLength) {
                $("input[value='-1'][name='State']").prop('checked', true);
            }
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
    </script>
</asp:Content>


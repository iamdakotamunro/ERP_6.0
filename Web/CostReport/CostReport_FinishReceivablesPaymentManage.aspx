<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="CostReport_FinishReceivablesPaymentManage.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_FinishReceivablesPaymentManage" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.UI.Web.Common" %>
<%@ Import Namespace="ERP.BLL.Implement.Organization" %>
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
            <td style="text-align: right;">费用名称：
            </td>
            <td>
                <asp:TextBox ID="txt_ReportName" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: right;">结算公司：
            </td>
            <td>
                <asp:DropDownList ID="ddl_AssumeFiliale" runat="server"></asp:DropDownList>
            </td>
            <td style="text-align: right;">处理状态：
            </td>
            <td>
                <asp:DropDownList ID="ddl_State" runat="server" onchange="ChangeTimeType();">
                    <asp:ListItem Text="已处理" Value="0"></asp:ListItem>
                    <asp:ListItem Text="未处理" Value="1" Selected="True"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">申报人：
            </td>
            <td>
                <rad:RadComboBox ID="rcb_ReportPersonnel" Width="172px" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入申报人" runat="server" Height="200px" OnItemsRequested="rcb_ReportPersonnel_ItemsRequested"></rad:RadComboBox>
            </td>
            <td style="text-align: right;">收/付款单位：
            </td>
            <td>
                <asp:TextBox ID="txt_PayCompany" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: right;">结算账号：
            </td>
            <td>
                <asp:DropDownList ID="ddl_PayBankAccount" runat="server" Width="172px"></asp:DropDownList>
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
                <asp:Button ID="btn_Finish" runat="server" Text="批量完成" OnClick="btn_Finish_Click" />
                <asp:HiddenField ID="Hid_ReportId" runat="server" />
            </td>
        </tr>
    </table>

    <rad:RadGrid ID="RG_Report" runat="server" ShowFooter="true" OnNeedDataSource="RG_Report_NeedDataSource" OnItemDataBound="RG_Report_ItemDataBound">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox value=-1 onclick=checkAll()&gt;全选">
                    <ItemTemplate>
                        <input title="请选择" type="checkbox" name="ckId" value='<%# Eval("ReportId")+"&"+Eval("State")+"&"+Eval("ReportNo")%>' <%# !Eval("State").Equals((int)CostReportState.Pay)?"disabled":""%> /><span <%# !Eval("State").Equals((int)CostReportState.Pay)?"style='color:red;'":""%>>选择</span>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ReportNo" HeaderText="申报编号">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
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
                <rad:GridTemplateColumn HeaderText="费用分类">
                    <ItemTemplate>
                        <%#Cost.ReadInstance.GetCompanyName(new Guid(Eval("CompanyClassId").ToString()),new Guid(Eval("CompanyId").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="BankName" HeaderText="结算账号">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="BankAccount" HeaderText="收款账号">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="TradeNo" HeaderText="交易流水号" UniqueName="TotalName">
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
                <rad:GridTemplateColumn HeaderText="收/付款时间">
                    <ItemTemplate>
                        <%#Convert.ToDateTime(Eval("ExecuteDate")).ToString("yyyy-MM-dd").Equals("1900-01-01")?"":Convert.ToDateTime(Eval("ExecuteDate")).ToString("yyyy-MM-dd") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="完成时间">
                    <ItemTemplate>
                        <%#Convert.ToDateTime(Eval("FinishDate")).ToString("yyyy-MM-dd").Equals("1900-01-01")?"":Convert.ToDateTime(Eval("FinishDate")).ToString("yyyy-MM-dd") %>
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
                        <%#GetReportState(Eval("State").ToString(),Convert.ToInt32(Eval("ReportCost")))%>
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
                        <%#Eval("State").Equals((int)CostReportState.Pay)?"<input type=\"button\" value=\"完成\" onclick=\"Finish('"+Eval("ReportId")+"')\" />":"<input type=\"button\" value=\"查看\" onclick=\"Look('"+Eval("ReportId")+"')\" />"%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="完成收付款" Width="1000px" Height="650px" />
            <rad:RadWindow ID="rawCK" runat="server" Title="费用申报详情" Width="1000px" Height="650px" />
        </Windows>
    </rad:RadWindowManager>
    
    <div id="divContent" style="display: none; position: absolute; _position: absolute; z-index: 100; border: solid 1px #718CA1; background-color: #F1F1F1; width: 400px; font-size: 13px;">
        <table style="width: 100%;" cellpadding="0" cellspacing="0">
            <tr style="height: 27px; background-image:url(../App_Themes/title.png)">
                <td style="cursor: move; text-align: center; color: white; font-weight: bolder;" valign="middle">批量完成</td>
                <td style="width: 49px; text-align: center;">
                    <img src="../Images/X1.gif" style="background-color: white;" alt="" onmouseover="this.src='../Images/X2.gif'" onmouseout="this.src='../Images/X1.gif'" onclick="moveHide();" />
                </td>
            </tr>
            <tr style="height: 30px;">
                <td colspan="2" style="text-align: center;">
                    <asp:Literal ID="lit_Msg" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr style="height: 35px;">
                <td colspan="2" style="text-align: center; padding-bottom: 5px;">
                    <asp:Button ID="btn_Pass" runat="server" Text="完&nbsp;&nbsp;成" OnClick="btn_Pass_Click" />
                </td>
            </tr>
        </table>
    </div>
    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
            $("#divContent").css({ "top": $(document.body).height() / 3, "left": $(document.body).width() / 3 });
            ChangeTimeType();//根据状态切换时间类型
        });

        //根据状态切换时间类型
        function ChangeTimeType() {
            var selectedValue = $("select[id$='ddl_State']").val();
            var timeTitle = $("#TimeTitle");
            var hidTimeType = $("input[id$='Hid_TimeType']");
            if (selectedValue === "0") {
                timeTitle.text("完成");
                hidTimeType.val("0");
            } else {
                timeTitle.text("申报");
                hidTimeType.val("1");
            }
        }

        //显示提示框
        function moveShow() {
            $("#divContent").show();
        }

        //隐藏提示框
        function moveHide() {
            $("#divContent").hide();
        }

        //完成
        function Finish(reportId) {
            window.radopen("../CostReport/CostReport_ReceivablesPayment.aspx?ReportId=" + reportId + "&Type=Finish", "raw");
        }

        //查看
        function Look(reportId) {
            window.radopen("../CostReport/CostReport_Look.aspx?ReportId=" + reportId, "rawCK");
        }

        //全选
        function checkAll() {
            var a = $("input[type='checkbox'][name='ckId']:not(:disabled)");
            var n = a.length;
            for (var i = 0; i < n; i++) {
                a[i].checked = window.event.srcElement.checked;
            }
        }

        //显示选中值
        function ShowValue(value) {
            if (value.length === 0) {
                $("input[id$='btn_Pass']").hide();
                return;
            } else {
                $("input[id$='btn_Pass']").show();
            }
            var arr = value.split(',');
            $("input[type='checkbox'][name='ckId']").each(function () {
                if (arr.indexOf($(this).val().split('&')[0]) > -1) {
                    $(this).attr('checked', true);
                } else {
                    $(this).attr('checked', false);
                }
            });

            var checkItemLength = $("input[type='checkbox'][name='ckId']:not([value='-1'])").length;
            var checkedLength = $("input[type='checkbox'][name='ckId']:checked").length;
            if (checkItemLength === checkedLength) {
                $("input[value='-1'][name='ckId']").prop('checked', true);
            }
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
    </script>
</asp:Content>


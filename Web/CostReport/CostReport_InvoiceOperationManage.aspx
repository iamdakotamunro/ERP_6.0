<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="CostReport_InvoiceOperationManage.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_InvoiceOperationManage" %>

<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <!--当前页面：费用发票-->
    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">申报人：
            </td>
            <td>
                <rad:RadComboBox ID="rcb_ReportPersonnel" Width="172px" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入申报人" runat="server" Height="200px" OnItemsRequested="rcb_ReportPersonnel_ItemsRequested"></rad:RadComboBox>
            </td>
            <td style="text-align: right;">发票抬头：</td>
            <td>
                <asp:DropDownList ID="ddl_InvoiceTitle" runat="server">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">申报时间：
            </td>
            <td>
                <asp:TextBox ID="txt_ReportDateStart" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
                至
                <asp:TextBox ID="txt_ReportDateEnd" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
            </td>
            <td style="text-align: right;">票据状态：</td>
            <td>
                <asp:DropDownList ID="ddl_BillState" runat="server" onchange="ChangeBillState();">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">申报编号：</td>
            <td>
                <asp:TextBox ID="txt_ReportNo" runat="server"> </asp:TextBox>
            </td>
            <td style="text-align: right;">票据号码：</td>
            <td>
                <asp:TextBox ID="txt_BillNo" runat="server"> </asp:TextBox>
            </td>
            <td style="text-align: right;">票据操作时间：</td>
            <td>
                <asp:TextBox ID="txt_OperatingTimeStart" runat="server" Width="70px" onclick="WdatePicker({skin:'blue',maxDate:$('input[id$=rdp_EndTime]').val()})"></asp:TextBox>
                至
                <asp:TextBox ID="txt_OperatingTimeEnd" runat="server" Width="70px" onclick="WdatePicker({skin:'blue',minDate:$('input[id$=rdp_StartTime]').val()})"></asp:TextBox>
            </td>
            <td style="text-align: right;">票据类型：</td>
            <td>
                <asp:DropDownList ID="ddl_InvoiceType" runat="server">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    <asp:ListItem Text="普通发票" Value="1"></asp:ListItem>
                    <asp:ListItem Text="增值税专用发票" Value="5"></asp:ListItem>
                    <asp:ListItem Text="收据" Value="2"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="8" style="text-align: center;">
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
                <asp:Button ID="btn_Batch" runat="server" Text="批量提交" OnClick="btn_Batch_Click" Style="display: none;" />
                <asp:HiddenField ID="Hid_BillId" runat="server" />
                <asp:Button ID="btn_ExportExcel" runat="server" Text="导出Excel" OnClick="btn_ExportExcel_Click" OnClientClick="return GiveTip(event,'您确定要导出Excel吗？')" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_Invoice" runat="server" ShowFooter="true" SkinID="CustomPaging" OnNeedDataSource="RG_Invoice_NeedDataSource" OnItemDataBound="RG_Invoice_ItemDataBound">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox value=-1 onclick=checkAll('ckId')&gt;全选">
                    <ItemTemplate>
                        <input title="请选择" type="checkbox" name="ckId" value='<%# Eval("BillId")+"&"+Eval("BillState")+"&"+Eval("BillNo")%>' <%# Eval("BillState").Equals((int)CostReportBillState.Verification)?"disabled":""%> /><span <%# Eval("BillState").Equals((int)CostReportBillState.Verification)?"style='color:red;'":""%>>选择</span>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报编号" UniqueName="TotalName">
                    <ItemTemplate>
                        &nbsp;<%# Eval("ReportNo")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <FooterStyle HorizontalAlign="Right"></FooterStyle>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报类型">
                    <ItemTemplate>
                        <%#EnumAttribute.GetKeyName((CostReportKind)Convert.ToInt32(Eval("ReportKind")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发票抬头">
                    <ItemTemplate>
                        <%#GetFilialeName(Eval("InvoiceTitleFilialeId").ToString()) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="票据类型">
                    <ItemTemplate>
                        <%#EnumAttribute.GetKeyName((CostReportInvoiceType)Convert.ToInt32(Eval("InvoiceType")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="实际金额" UniqueName="ActualAmount">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("ActualAmount"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报人">
                    <ItemTemplate>
                        <%#new ERP.BLL.Implement.Organization.PersonnelManager().GetName(new Guid(Eval("ReportPersonnelId").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="BillUnit" HeaderText="开票单位" UniqueName="BillTotal">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="150px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="BillDate" HeaderText="开票日期" DataFormatString="{0:yyyy-MM-dd}">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="票据号码">
                    <ItemTemplate>
                        &nbsp;<%#Eval("BillNo")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="票据代码">
                    <ItemTemplate>
                        &nbsp;<%#Eval("BillCode")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="未税金额" UniqueName="NoTaxAmount">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("NoTaxAmount").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="税额" UniqueName="Tax">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("Tax").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="含税金额" UniqueName="TaxAmount">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("TaxAmount").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="票据状态">
                    <ItemTemplate>
                        <%#EnumAttribute.GetKeyName((CostReportBillState)Convert.ToInt32(Eval("BillState")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="查看科目">
                    <ItemTemplate>
                        <a href="javascript:void(0);" title="查看科目" onclick="SubjectView(<%#"'"+Eval("BillId")+"','"+Eval("InvoiceType")+"'"%>)" 
                            <%# !Eval("BillState").Equals((int)CostReportBillState.Verification)||
                                Eval("BillState").Equals((int)CostReportBillState.Finish)||
                                Eval("BillState").Equals((int)CostReportBillState.Verification)?"":"style=\"display: none;\""%>>
                            <img src="../App_Themes/Default/images/Advice.gif" />
                        </a>

                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="60%" tooltipmsg='<%#Eval("Memo").ToString().Replace("\n","<br/>")%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作日志">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="60%" tooltipmsg='<%#Eval("Remark")==null?"": Eval("Remark").ToString().Replace("\n","<br/>")%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="25px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="票据操作">
                    <ItemTemplate>
                        <a href="javascript:void(0);" title="编辑" onclick="Edit(<%#"'"+Eval("BillId")+"','"+Eval("InvoiceType")+"'"%>)" <%# !Eval("BillState").Equals((int)CostReportBillState.Verification)?"":"style=\"display: none;\""%>>
                            <img src="../App_Themes/Default/images/Edit.gif" />
                        </a>
                        <asp:ImageButton ID="imgbtn_Del" runat="server" SkinID="DeleteImageButton" ToolTip="删除" CommandArgument='<%# Eval("BillId") %>' OnClientClick="return GiveTip(event,'您确定删除吗？')" OnClick="imgbtn_Del_Click" Visible='<%# Eval("BillState").Equals((int)CostReportBillState.UnSubmit)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="票据提交">
                    <ItemTemplate>
                        <asp:ImageButton ID="imgbtn_Submit" runat="server" SkinID="AffirmImageButton" ToolTip="提交" CommandArgument='<%# Eval("BillId") %>' OnClientClick="return GiveTip(event,'您确定提交吗？')" OnClick="imgbtn_Submit_Click" Visible='<%# Eval("BillState").Equals((int)CostReportBillState.UnSubmit)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="25px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                
                <rad:GridTemplateColumn HeaderText="票据接收">
                    <ItemTemplate>
                        <a href="javascript:void(0);" title="票据接收" onclick="SubjectAccept(<%#"'"+Eval("BillId")+"','"+Eval("InvoiceType")+"'"%>)" 
                            <%# !Eval("BillState").Equals((int)CostReportBillState.Verification)||
                                Eval("BillState").Equals((int)CostReportBillState.Finish)||
                                Eval("BillState").Equals((int)CostReportBillState.Verification)?"":"style=\"display: none;\""%>>
                            <img src="../App_Themes/Default/images/Affirm.gif" />
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>

                <%--<rad:GridTemplateColumn HeaderText="票据接收">
                    <ItemTemplate>
                        <asp:ImageButton ID="imgbtn_Receive" runat="server" SkinID="AffirmImageButton" ToolTip="接收" CommandArgument='<%# Eval("BillId") %>' CommandName='<%# Eval("InvoiceType") %>' OnClientClick="return GiveTip(event,'您确定接收吗？')" OnClick="imgbtn_Receive_Click" Visible='<%# Eval("BillState").Equals((int)CostReportBillState.Submit)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="25px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>--%>
                <rad:GridTemplateColumn HeaderText="增票待认证">
                    <ItemTemplate>
                        <a title="待认证" onclick="moveShowAuthenticate('<%# Eval("BillId") %>');" <%# Eval("BillState").Equals((int)CostReportBillState.Receive)?"":"style='display:none;'"%>>
                            <img src="../App_Themes/Default/images/Affirm.gif" />
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="25px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="完成认证">
                    <ItemTemplate>
                        <asp:ImageButton ID="imgbtn_FinishAuthenticate" runat="server" SkinID="AffirmImageButton" ToolTip="完成认证" CommandArgument='<%# Eval("BillId") %>' OnClientClick="return GiveTip(event,'您确定完成认证吗？')" OnClick="imgbtn_FinishAuthenticate_Click" Visible='<%# Eval("BillState").Equals((int)CostReportBillState.Finish)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="25px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="票据退回">
                    <ItemTemplate>
                        <asp:ImageButton ID="imgbtn_Back" runat="server" SkinID="AffirmImageButton" ToolTip="退回" CommandArgument='<%# Eval("BillId") %>' OnClientClick="return GiveTip(event,'您确定退回吗？')" OnClick="imgbtn_Back_Click" Visible='<%# !Eval("BillState").Equals((int)CostReportBillState.UnSubmit)&&!Eval("BillState").Equals((int)CostReportBillState.Verification)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="25px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                
                <rad:GridTemplateColumn HeaderText="科目归类">
                    <ItemTemplate>
                        <a href="javascript:void(0);" title="科目归类" onclick="SubjectEdit(<%#"'"+Eval("BillId")+"','"+Eval("InvoiceType")+"'"%>)" 
                            <%# !Eval("BillState").Equals((int)CostReportBillState.Verification)||
                                Eval("BillState").Equals((int)CostReportBillState.Finish)||
                                Eval("BillState").Equals((int)CostReportBillState.Verification)?"":"style=\"display: none;\""%>>
                            <img src="../App_Themes/Default/images/Edit.gif" />
                        </a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <asp:Literal ID="lit_FooterTable" runat="server"></asp:Literal>
    <asp:HiddenField ID="Hid_IsFirstPage" runat="server" />
    <asp:HiddenField ID="Hid_Type" runat="server" />

    <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="编辑票据" Width="700px" Height="250px" />

            <rad:RadWindow ID="raw_SubjectView" runat="server" Title="查看科目" Width="900px" Height="600px" />
            <rad:RadWindow ID="raw_SubjectAccept" runat="server" Title="票据接收" Width="900px" Height="600px" />
            <rad:RadWindow ID="raw_SubjectEdit" runat="server" Title="编辑科目" Width="900px" Height="600px" />
        </Windows>
    </rad:RadWindowManager>

    <div id="divContent" style="display: none; position: absolute; _position: absolute; z-index: 100; border: solid 1px #718CA1; background-color: #F1F1F1; width: 400px; font-size: 13px;">
        <table style="width: 100%;" cellpadding="0" cellspacing="0">
            <tr style="height: 27px; background-image:url(../App_Themes/title.png)">
                <td style="cursor: move; text-align: center; color: white; font-weight: bolder;" valign="middle">
                    <span id="ContentTitle"></span></td>
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
                    <asp:Button ID="btn_Pass" runat="server" Text="确&nbsp;&nbsp;定" OnClick="btn_Pass_Click" />
                </td>
            </tr>
        </table>
    </div>

    <div id="divContentAuthenticate" style="display: none; position: absolute; _position: absolute; z-index: 100; border: solid 1px #718CA1; background-color: #F1F1F1; width: 400px; font-size: 13px;">
        <table style="width: 100%;" cellpadding="0" cellspacing="0">
            <tr style="height: 27px; background-image:url(../App_Themes/title.png)">
                <td style="cursor: move; text-align: center; color: white; font-weight: bolder;" valign="middle">增票待认证</td>
                <td style="width: 49px; text-align: center;">
                    <img src="../Images/X1.gif" style="background-color: white;" alt="" onmouseover="this.src='../Images/X2.gif'" onmouseout="this.src='../Images/X1.gif'" onclick="moveHideAuthenticate();" />
                </td>
            </tr>
            <tr style="height: 30px;">
                <td colspan="2" style="text-align: center;">
                    <asp:Literal ID="lit_AuthenticateMsg" runat="server"></asp:Literal><br />
                    <span id="Title" style="color: blue;">您确认待认证吗？</span>
                </td>
            </tr>
            <tr id="Authenticate" style="height: 35px;">
                <td colspan="2" style="text-align: center; padding-bottom: 5px;">
                    <asp:Button ID="btn_Authenticate" runat="server" Text="需认证" OnClick="btn_Authenticate_Click" />
                    <asp:Button ID="btn_NoAuthenticate" runat="server" Text="无需认证" OnClick="btn_NoAuthenticate_Click" />
                </td>
            </tr>
        </table>
    </div>
    <script src="../JavaScript/jquery.js"></script>
    <script src="../My97DatePicker/WdatePicker.js"></script>
    <script src="../JavaScript/ToolTipMsg.js"></script>
    <script src="../JavaScript/GiveTip.js"></script>
    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
            ChangeBillState();//切换票据状态

            $("tfoot tr[class='rgFooter']").after($("table[id$='FooterTable'] tbody").html());
        });

        //切换票据状态
        function ChangeBillState() {
            var ddlBillState = $("select[id$='ddl_BillState']");
            var btnBatch = $("input[id$='btn_Batch']");
            var contentTitle = $("span[id$='ContentTitle']");
            btnBatch.css("display", "none");
            $("input[id$='Hid_Type']").val("1");
            if (ddlBillState.val() === "0") {
                btnBatch.css("display", "");
                btnBatch.val("批量提交");
                contentTitle.text("批量提交");
            } else if (ddlBillState.val() === "1") {
                btnBatch.css("display", "");
                btnBatch.val("批量接收");
                contentTitle.text("批量接收");
            } else if (ddlBillState.val() === "2") {
                $("input[id$='Hid_Type']").val("2");
                btnBatch.css("display", "");
                btnBatch.val("批量认证");
                contentTitle.text("批量认证");
            } else if (ddlBillState.val() === "4") {
                btnBatch.css("display", "");
                btnBatch.val("批量完成");
                contentTitle.text("批量完成");
            }

            if ($("input[id$='Hid_Type']").val() === "1") {
                moveHideAuthenticate();
            } else if ($("input[id$='Hid_Type']").val() === "2") {
                moveHide();
            }
        }

        //显示提示框
        function moveShow() {
            $("#divContent").css({ "top": ($(window).height() / 2 + $(window).scrollTop() - $("#divContent").height() / 2), "left": ($(window).width() / 2 + $(window).scrollLeft() - $("#divContent").width() / 2) });
            $("#divContent").show();
        }

        //隐藏提示框
        function moveHide() {
            $("#divContent").hide();
        }

        //显示提示框(增票待认证)
        function moveShowAuthenticate(billId) {
            $("#divContentAuthenticate").css({ "top": ($(window).height() / 2 + $(window).scrollTop() - $("#divContentAuthenticate").height() / 2), "left": ($(window).width() / 2 + $(window).scrollLeft() - $("#divContentAuthenticate").width() / 2) });
            $("input[id$='Hid_BillId']").val("");
            $("#divContentAuthenticate").show();
            if (billId.length > 0) {
                $("input[id$='Hid_BillId']").val(billId);
            }
        }

        //隐藏提示框(增票待认证)
        function moveHideAuthenticate() {
            $("#divContentAuthenticate").hide();
        }
        

        //编辑
        function Edit(billId, invoiceType) {
            window.radopen("../CostReport/CostReport_InvoiceOperationEdit.aspx?BillId=" + billId + "&InvoiceType=" + invoiceType, "raw");
        }
        
        //查看科目
        function SubjectView(billId, invoiceType) {
            window.radopen("/CostReport/CostReport_InvoiceOperationSubjectView.aspx?BillId=" + billId + "&InvoiceType=" + invoiceType, "raw_SubjectView");
        }
        
        //票据接收
        function SubjectAccept(billId, invoiceType) {
            window.radopen("/CostReport/CostReport_InvoiceOperationSubjectEdit.aspx?BillId=" + billId + "&InvoiceType=" + invoiceType, "raw_SubjectAccept");
        }
        //编辑科目
        function SubjectEdit(billId, invoiceType) {
            window.radopen("/CostReport/CostReport_InvoiceOperationSubjectEdit2.aspx?BillId=" + billId + "&InvoiceType=" + invoiceType, "raw_SubjectEdit");
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
                $("tr[id$='Authenticate']").hide();
                $("span[id$='Title']").hide();
                return;
            } else {
                $("input[id$='btn_Pass']").show();
                $("tr[id$='Authenticate']").show();
                $("span[id$='Title']").show();
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
            $("input[id$='Hid_IsFirstPage']").val("0");
            $("input[id$='btn_Search']").click();
        }
    </script>
</asp:Content>

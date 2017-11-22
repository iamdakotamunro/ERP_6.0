<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_FeeIncome.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_FeeIncome" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body style="overflow: hidden;">
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360">
        </rad:RadScriptManager>
        <table style="width: 100%;">
            <tr>
                <td style="width: 84px; text-align: right;">费用归属部门：
                </td>
                <td colspan="3">部门：<asp:DropDownList ID="ddl_AssumeBranch" runat="server" OnSelectedIndexChanged="ddl_AssumeBranch_SelectedIndexChanged" AutoPostBack="True" CssClass="Check" lang='费用归属部门'>
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                </asp:DropDownList>
                    小组：<asp:DropDownList ID="ddl_AssumeGroup" runat="server" OnSelectedIndexChanged="ddl_AssumeGroup_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <span id="AssumeShop" runat="server" visible="False">余额扣除门店：<asp:DropDownList ID="ddl_AssumeShop" runat="server">
                        <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    </span>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">费用分类：
                </td>
                <td colspan="3">
                    <asp:DropDownList ID="ddl_CompanyClass" runat="server" OnSelectedIndexChanged="ddl_CompanyClass_SelectedIndexChanged" AutoPostBack="True" CssClass="Check" lang='费用分类'>
                        <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddl_FeeType" runat="server" OnSelectedIndexChanged="ddl_FeeType_SelectedIndexChanged" AutoPostBack="True" CssClass="Check">
                        <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">紧急程度：
                </td>
                <td style="width: 391px;">
                    <asp:RadioButtonList ID="rbl_UrgentOrDefer" runat="server" RepeatDirection="Horizontal" Style="float: left;" onchange="ChangeUrgentOrDefer();">
                        <asp:ListItem Text="普通" Value="0" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="加急" Value="1"></asp:ListItem>
                    </asp:RadioButtonList>
                    <b id="UrgentOrDeferMsg" style="color: red; float: left; padding: 7px 0 0 5px;">每周四审批一次。</b>
                </td>
                <td style="text-align: right; width: 109px;">加急原因：
                </td>
                <td>
                    <asp:TextBox ID="txt_UrgentReason" runat="server" Width="250px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">费用名称：
                </td>
                <td>
                    <asp:TextBox ID="txt_ReportName" runat="server" Width="250px" CssClass="Check" lang='费用名称'></asp:TextBox>
                </td>
                <td style="text-align: right;">费用实际发生时间：
                </td>
                <td>
                    <asp:TextBox ID="txt_StartTime" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy年MM月'})" CssClass="Check" lang='费用实际发生开始时间'></asp:TextBox>
                    至
                    <asp:TextBox ID="txt_EndTime" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy年MM月',minDate:'#F{$dp.$D(\'txt_StartTime\');}'})" CssClass="Check" lang='费用实际发生开始时间'></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;"><b>付款</b>单位：
                </td>
                <td>
                    <asp:TextBox ID="txt_PayCompany" runat="server" Width="250px" CssClass="Check" lang='付款单位'></asp:TextBox>
                </td>
                <td style="text-align: right;">申报金额：
                </td>
                <td>
                    <asp:TextBox ID="txt_ReportCost" runat="server" AutoPostBack="True" OnTextChanged="txt_ReportCost_TextChanged" onblur="check(this,'Decimal');" Width="250px" CssClass="Check" lang='申报金额'></asp:TextBox><br />
                    <b>
                        <asp:Literal ID="Lit_CapitalAmount" runat="server"></asp:Literal></b>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">结算账号：
                </td>
                <td>
                    <rad:RadComboBox ID="rcb_PayBankAccount" Width="254px" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入结算账户" runat="server" Height="200px" OnItemsRequested="rcb_PayBankAccount_ItemsRequested" OnSelectedIndexChanged="rcb_PayBankAccount_SelectedIndexChanged" AutoPostBack="True" CssClass="Check" lang='结算账号'></rad:RadComboBox>
                </td>
                <td style="text-align: right;">结算公司：
                </td>
                <td>
                    <asp:TextBox ID="txt_AssumeFiliale" runat="server" placeholder="请选择结算账号" ReadOnly="True" Width="250px" CssClass="Check" lang='结算公司'></asp:TextBox>
                </td>
                <asp:HiddenField ID="Hid_AssumeFiliale" runat="server" />
            </tr>
            <tr>
                <td style="text-align: right;">结算方式：
                </td>
                <td>
                    <asp:RadioButtonList ID="rbl_CostType" runat="server" RepeatDirection="Horizontal" onchange="ChangeCostType()">
                        <asp:ListItem Text="转账" Value="2" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="现金" Value="1"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td class="BankName" style="text-align: right;"><b style="color: red;">付款</b>银行：
                </td>
                <td class="BankName" lang="BankName">
                    <asp:TextBox ID="txt_BankName" runat="server" placeholder="上海银行" Width="250px" onchange="CheckSubBankName()"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="BankName" style="text-align: right;"><b style="color: red;">付款</b>支行：
                </td>
                <td class="BankName" lang="BankName">
                    <asp:TextBox ID="txt_SubBankName" runat="server" placeholder="松江支行" Width="250px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">申报说明：</td>
                <td colspan="3">
                    <asp:TextBox ID="txt_ReportMemo" runat="server" TextMode="MultiLine" Width="99%" Height="50px" placeholder="请输入申报说明,最多可输入1000字！" onblur="CheckChinese(this)" lang='申报说明'></asp:TextBox>
                </td>
            </tr>
            <tr id="AuditingMemo" runat="server" visible="False">
                <td style="text-align: right;">操作说明：</td>
                <td colspan="3" style="font-weight: bold; color: blue;">
                    <asp:Literal ID="lit_AuditingMemo" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr id="ReportProcess" runat="server" visible="False">
                <td style="text-align: right;">申报流程：</td>
                <td colspan="3">
                    <asp:TextBox ID="txt_Memo" runat="server" TextMode="MultiLine" ReadOnly="True" Width="99%" Height="100px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: center;">
                    <asp:Button ID="btn_Save" runat="server" Text="保存" OnClientClick="return CheckEmpty();" OnClick="btn_Save_Click" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="Hid_CostsClass" runat="server" />
        <script src="../JavaScript/jquery.js"></script>
        <script src="../My97DatePicker/WdatePicker.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script src="../JavaScript/tool.js"></script>
        <script type="text/javascript">
            $(function () {
                ChangeUrgentOrDefer();//紧急程度
                ChangeCostTypeChild();//切换结算方式子元素
                CheckSubBankName();//如果“收/付款银行”是非银行(如支付宝，微信等)，则“收/付款支行”是非必填项
            });

            //紧急程度
            function ChangeUrgentOrDefer() {
                var selectedUrgentOrDefer = $("input[type='radio'][name='rbl_UrgentOrDefer']:checked").val();
                var txtUrgentReason = $("#txt_UrgentReason");
                var urgentOrDeferMsg = $("b[id='UrgentOrDeferMsg']");
                txtUrgentReason.next("span[class='error']").remove();

                if (selectedUrgentOrDefer === "1") {
                    txtUrgentReason.addClass("Check");
                    txtUrgentReason.attr('lang', '加急原因');
                    txtUrgentReason.removeAttr("readonly");
                    urgentOrDeferMsg.text("请填写加急原因。");
                } else {
                    txtUrgentReason.removeClass("Check");
                    txtUrgentReason.attr("readonly", "readonly");
                    txtUrgentReason.val("");
                    urgentOrDeferMsg.text("每周四审批一次。");
                }
            }

            //切换结算方式
            function ChangeCostType() {
                ClearCostTypeChildData();
                ChangeCostTypeChild();
            }

            //切换结算方式子元素
            function ChangeCostTypeChild() {
                var selectedCostType = $("input[type='radio'][name='rbl_CostType']:checked").val();
                var bankName = $("td[class='BankName']");

                var bankNameChildren = $("td[lang='BankName']").children().not("span[class='error']");

                if (selectedCostType === "1") {//结算方式为“现金”
                    bankName.css("display", "none");
                } else {//结算方式为“转账”
                    bankName.css("display", "");
                }
                bankNameChildren.removeClass("Check");
            }

            //清除结算方式子元素相关数据
            function ClearCostTypeChildData() {
                //清除银行名称数据 
                $("input[id='txt_BankName']").val("");
                $("input[id='txt_SubBankName']").val("");
            }

            //如果“收/付款银行”是非银行(如支付宝，微信等)，则“收/付款支行”是非必填项
            function CheckSubBankName() {
                var txtBankName = $("input[id$='txt_BankName']");
                var txtSubBankName = $("input[id$='txt_SubBankName']");
                if (txtBankName.val().indexOf("银行") === -1) {
                    txtSubBankName.removeClass("Check");
                } else {
                    txtSubBankName.addClass("Check");
                }
            }

            //验证
            function CheckEmpty() {
                $("span[class='error']").remove();//移除所有错误提示

                var errorMsg = '';
                $(".Check").each(function () {
                    var obj = $(this);
                    if (obj.val().length === 0) {
                        if (obj.next("span[class='error']").length === 0) {
                            obj.after("<span class='error' style='color:red;'>*</span>");
                            if (typeof (obj.attr('lang')) != "undefined") {
                                errorMsg += '、“' + obj.attr('lang') + '”';
                            }
                        }
                    } else {
                        obj.next("span[class='error']").remove();
                    }
                });

                //验证申报说明
                var resultReportMemo = CheckReportMemo();
                if (resultReportMemo.length !== 0) {
                    errorMsg += resultReportMemo;
                }

                if ($("span[class='error']").length === 0) {
                    return true;
                } else {
                    if (errorMsg.length !== 0) {
                        alert(errorMsg.substring(1) + '不能为空！');
                    }
                    return false;
                }
            }

            //验证申报说明
            function CheckReportMemo() {
                var txtReportMemo = $("#txt_ReportMemo");
                if (txtReportMemo.val().length === 0 || !$.chineseLengthValid(txtReportMemo.val(), 0, 1000)) {
                    if (txtReportMemo.next("span[class='error']").length === 0) {
                        txtReportMemo.after("<span class='error'></span>");
                        txtReportMemo.css("border-color", "red");
                        if (typeof (txtReportMemo.attr('lang')) != "undefined") {
                            return '、“' + txtReportMemo.attr('lang') + '”';
                        }
                    }
                } else {
                    txtReportMemo.next("span[class='error']").remove();
                    txtReportMemo.css("border-color", "");
                }
                return "";
            }

            //验证类型
            function check(obj, type) {
                if (!$.checkType(type).test($(obj).val())) {
                    $(obj).val("");
                    $(obj).attr("placeholder", castErrorMessage(type));
                }
            }

            //验证字数
            function CheckChinese(obj) {
                if (!$.chineseLengthValid($(obj).val(), 0, 1000)) {
                    $(obj).css("border-color", "red");
                    alert("“申报说明”最多可输入1000字！");
                } else {
                    $(obj).css("border-color", "");
                }
            }
        </script>
    </form>
</body>
</html>


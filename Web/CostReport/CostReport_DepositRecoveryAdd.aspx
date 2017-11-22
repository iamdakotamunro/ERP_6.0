<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_DepositRecoveryAdd.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_DepositRecoveryAdd" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body style="overflow: hidden;">
    <form id="form1" runat="server">
        <table style="width: 100%; line-height: 20px;">
            <tr>
                <td style="width: 64px; text-align: right;">结算公司：
                </td>
                <td>
                    <asp:Label ID="lbl_FilialeName" runat="server" ></asp:Label>
                </td>
                <td style="text-align: right;">结算账号：
                </td>
                <td>
                    <asp:Label ID="lbl_BankAccount" runat="server" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">费用名称：
                </td>
                <td>
                   <asp:Label ID="lbl_ReportName" runat="server" ></asp:Label>
                </td>
                <td style="text-align: right;">费用分类：
                </td>
                <td>
                    <asp:Label ID="lbl_CompanyClass" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">申报单号：
                </td>
                <td> 
                     <asp:Label ID="lbl_ReportNo" runat="server" ></asp:Label>
                </td>
                <td style="text-align: right;">付款时间：
                </td>
                <td>
                    <asp:Label ID="lbl_ExecuteDate" runat="server" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">付款金额：
                </td>
                <td>
                    <asp:Label ID="lbl_PayCost" runat="server" ></asp:Label>
                </td>
                <td style="text-align: right;">申请人：
                </td>
                <td>
                    <asp:Label ID="lbl_ReportPersonnelName" runat="server" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">回收类型：
                </td>
                <td>
                    <asp:CheckBox ID="Chk_Bill" runat="server" Text="票据"/>
                </td>
                <td style="text-align: right;">票据回收金额：
                </td>
                <td>
                    <asp:TextBox ID="txt_BillCost" runat="server"  onblur="check(this,'Decimal');" CssClass="Check"></asp:TextBox>
                </td>
            </tr>
             <tr>
                <td style="text-align: right;">
                </td>
                <td>
                    <asp:CheckBox ID="Chk_Cash" runat="server" Text="现金"/>
                </td>
                <td style="text-align: right;">现金回收金额：
                </td>
                <td>
                    <asp:TextBox ID="txt_CashCost" runat="server"  onblur="check(this,'Decimal');" CssClass="Check"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">备注说明：</td>
                <td colspan="3">
                    <asp:TextBox ID="txt_RecoveryRemarks" runat="server" TextMode="MultiLine" Width="99%" Height="50px" ></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: center;">
                    <asp:Button ID="btn_Save" runat="server" Text="回收" OnClientClick="return CheckEmpty();" OnClick="btn_Save_Click" />
                </td>
            </tr>
        </table>
        <script src="../JavaScript/jquery.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script src="../JavaScript/tool.js"></script>
        <script type="text/javascript">
            function CheckEmpty() {
                if (!$("#Chk_Bill").is(':checked') && !$("#Chk_Cash").is(':checked')) {
                    alert("请选择押金回收类型！");
                    return false;
                }
                if ($("#Chk_Bill").is(':checked') && $("#txt_BillCost").val()==="") {
                    alert("请填写票据回收金额！");
                    return false;
                }
                if ($("#Chk_Cash").is(':checked') && $("#txt_CashCost").val() === "") {
                    alert("请填写现金回收金额！");
                    return false;
                }
                if ($("#txt_RecoveryRemarks").val().trim() === "") {
                    alert("请填写备注说明！");
                    return false;
                }
                return true;
            }

            //验证类型
            function check(obj, type) {
                if ($.checkType(type).test($(obj).val()) && $(obj).val()>0) {
                } else {
                    $(obj).val("");
                    $(obj).attr("placeholder", castErrorMessage(type));
                }
            }
        </script>
    </form>
</body>
</html>

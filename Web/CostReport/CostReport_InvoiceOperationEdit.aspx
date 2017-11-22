<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_InvoiceOperationEdit.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_InvoiceOperationEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <table style="width: 100%;">
            <tr>
                <td style="text-align: right;">开票单位：
                </td>
                <td>
                    <asp:TextBox ID="txt_BillUnit" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
                <td style="text-align: right;">开票日期：
                </td>
                <td>
                    <asp:TextBox ID="txt_BillDate" runat="server" Width="250px" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM-dd'})" CssClass="Check"></asp:TextBox>
                </td>
            </tr>
            <tr id="VatInvoice" runat="server" visible="False">
                <td style="text-align: right;">未税金额：
                </td>
                <td>
                    <asp:TextBox ID="txt_NoTaxAmount" runat="server" Width="250px" onblur="check(this,'Decimal');" CssClass="Check"></asp:TextBox>
                </td>
                <td style="text-align: right;">税额：
                </td>
                <td>
                    <asp:TextBox ID="txt_Tax" runat="server" Width="250px" onblur="check(this,'Decimal');" CssClass="Check"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">
                    <asp:Literal ID="lit_BillNo" runat="server">发票</asp:Literal>号码：
                </td>
                <td>
                    <asp:TextBox ID="txt_BillNo" runat="server" Width="250px" CssClass="Check"></asp:TextBox>
                </td>
                <td style="text-align: right;">
                    <asp:Literal ID="lit_TaxAmount" runat="server">含税</asp:Literal>金额：
                </td>
                <td>
                    <asp:TextBox ID="txt_TaxAmount" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td id="BillCode" runat="server" style="text-align: right;">发票代码：
                </td>
                <td id="txtBillCode" runat="server">
                    <asp:TextBox ID="txt_BillCode" runat="server" Width="250px" CssClass="Check"></asp:TextBox>
                </td>
                <td style="text-align: right;">票据状态：
                </td>
                <td>
                    <asp:TextBox ID="txt_BillState" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">备注：
                </td>
                <td colspan="3">
                    <asp:TextBox ID="txt_Memo" runat="server" TextMode="MultiLine" Width="99%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: center;" colspan="4">
                    <asp:Button ID="btn_Save" runat="server" Text="保存" OnClientClick="return CheckEmpty();" OnClick="btn_Save_Click" />
                </td>
            </tr>
        </table>
        <script src="../JavaScript/jquery.js"></script>
        <script src="../My97DatePicker/WdatePicker.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script src="../JavaScript/tool.js"></script>
        <script type="text/javascript">
            //验证
            function CheckEmpty() {
                $("span[class='error']").remove();//移除所有错误提示

                $(".Check").each(function () {
                    var obj = $(this);
                    if (obj.val().length === 0) {
                        if (obj.next("span[class='error']").length === 0) {
                            obj.after("<span class='error' style='color:red;'>*</span>");
                        }
                    } else {
                        obj.next("span[class='error']").remove();
                    }
                });

                if ($("span[class='error']").length === 0) {
                    return true;
                } else {
                    return false;
                }
            }

            //验证类型
            function check(obj, type) {
                if (!$.checkType(type).test($(obj).val())) {
                    $(obj).val($("#"+$(obj).attr("id").replace("txt", "Hid")).val());
                }
            }
        </script>
    </form>
</body>
</html>

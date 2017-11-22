<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberMentionProcessBatch.aspx.cs" Inherits="ERP.UI.Web.Windows.MemberMentionProcessBatch" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360">
        </rad:RadScriptManager>
        <table style="width: 100%; padding-top: 10px;">
            <tr>
                <td style="text-align: right;">总笔数:</td>
                <td>
                    <asp:Literal ID="Lit_Count" runat="server"></asp:Literal></td>
                <td style="text-align: right;">提现总金额:</td>
                <td>
                    <asp:Literal ID="Lit_AmountTotal" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">资金账户:</td>
                <td colspan="3">
                    <rad:RadComboBox ID="RCB_BankAccountsId" runat="server" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入资金账户" OnItemsRequested="RCB_BankAccountsId_ItemsRequested" Height="80px" CssClass="Check">
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">拒绝理由:</td>
                <td colspan="3">
                    <asp:TextBox ID="txt_RefuseReason" runat="server" TextMode="MultiLine" Style="width: 98%;"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: center;">
                    <asp:Button ID="btn_PayFinish" runat="server" Text="打款完成" OnClientClick="return CheckPayFinish();" OnClick="btn_PayFinish_Click" />
                    <asp:Button ID="btn_ReturnApply" runat="server" Text="退回申请" OnClientClick="return CheckReturnApply();" OnClick="btn_ReturnApply_Click" />
                </td>
            </tr>
        </table>
        <script src="../JavaScript/jquery.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script type="text/javascript">
            //验证打款完成
            function CheckPayFinish() {
                $("span[class='error']").remove(); //移除所有错误提示

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

            //验证退回申请
            function CheckReturnApply() {
                var txtRefuseReason = $("#txt_RefuseReason");
                if (txtRefuseReason.val().length === 0) {
                    txtRefuseReason.css("border-color", "red");
                    return false;
                } else {
                    txtRefuseReason.css("border-color", "");
                    return true;
                }
            }
        </script>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Payment_BatchEdit.aspx.cs" Inherits="ERP.UI.Web.SubsidyPayment.Payment_BatchEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!--当前页面：补贴打款——批量受理-->
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>补贴打款——批量受理</title>
    <script src="/JavaScript/jquery.js" type="text/javascript"></script>
    <link href="../Styles/bootstrap.min.css" rel="stylesheet" />
    <script src="../JavaScript/bootstrap.min.js" type="text/javascript"></script>

    <style type="text/css">
        label {
            font-weight: normal;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360">
        </rad:RadScriptManager>
        <br />
        <div class="panel panel-default">
            <div class="panel-heading">批量受理</div>
            <div class="panel-body">

                <div class="form-group col-sm-6 has-feedback">
                    <label class="col-sm-4 control-label" style="text-align: right;">总笔数：</label>
                    <div class="col-sm-8 control-label" style="text-align: left;">
                        <asp:Literal ID="Lit_Count" runat="server"></asp:Literal>
                    </div>
                </div>
                <div class="form-group col-sm-6 has-feedback">
                    <label class="col-sm-4 control-label" style="text-align: right;">补贴金额：</label>
                    <div class="col-sm-8 control-label" style="text-align: left;">
                        <asp:Literal ID="Lit_AmountTotal" runat="server"></asp:Literal>
                    </div>
                </div>
                <div class="form-group col-sm-6 has-feedback">
                    <label class="col-sm-4 control-label" style="text-align: right;">资金账户：</label>
                    <div class="col-sm-8 control-label" style="text-align: left;">
                        <rad:RadComboBox ID="RCB_BankAccountsId" runat="server" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入资金账户" OnItemsRequested="RCB_BankAccountsId_ItemsRequested" Height="80px" CssClass="Check" Width="520">
                        </rad:RadComboBox>
                    </div>
                </div>

                <div class="form-group col-sm-6 has-feedback">
                    <label class="col-sm-4 control-label" style="text-align: right;">&nbsp;</label>
                    <div class="col-sm-8 control-label" style="text-align: left;">
                    </div>
                </div>

                <div class="form-group col-sm-12 has-feedback">
                    <label class="col-sm-4 control-label" style="text-align: right; width: 16%;">拒绝理由：</label>
                    <div class="col-sm-8 control-label" style="text-align: left; width: 77%;">
                        <asp:TextBox ID="txt_RefuseReason" runat="server" TextMode="MultiLine" Width="100%" Height="80px"></asp:TextBox>
                    </div>
                </div>

            </div>
        </div>

        <div class="popBtns">
            <div class="form-group">
                <div class="col-sm-12">
                    <div class="text-center">

                        <asp:Button ID="btn_ReturnApply" runat="server" Text="退回申请" OnClientClick="return CheckReturnApply();" OnClick="btn_ReturnApply_Click" CssClass="btn btn-danger" Style="margin-right: 20px;" />

                        <asp:Button ID="btn_PayFinish" runat="server" Text="打款完成" OnClientClick="return CheckPayFinish();" OnClick="btn_PayFinish_Click" CssClass="btn btn-success" Style="margin-right: 20px;" />

                        <asp:Button ID="BT_Cancl" runat="server" Text="关闭窗口" OnClientClick="return CancelWindow()" CssClass="btn btn-default" />
                    </div>
                </div>
            </div>
        </div>

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

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_Look.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_Look" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
        .table {
            border-left: 1px solid #A9A9A9;
            border-top: 1px solid #A9A9A9;
        }

            .table td {
                border-bottom: 1px solid #A9A9A9;
                border-right: 1px solid #A9A9A9;
            }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table style="width: 100%;">
            <tr>
                <td style="width: 84px; text-align: right;">申报类型：</td>
                <td>
                    <asp:RadioButtonList ID="rbl_ReportKind" runat="server" RepeatDirection="Horizontal" Enabled="False">
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td style="width: 84px; text-align: right;">费用归属部门：
                </td>
                <td colspan="3">部门：<asp:DropDownList ID="ddl_AssumeBranch" runat="server" Enabled="False">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                </asp:DropDownList>
                    小组：<asp:DropDownList ID="ddl_AssumeGroup" runat="server" Enabled="False">
                        <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <span id="AssumeShop" runat="server" visible="False">余额扣除门店：<asp:DropDownList ID="ddl_AssumeShop" runat="server" Enabled="False">
                        <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    </span>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">费用分类：
                </td>
                <td>
                    <asp:TextBox ID="txt_CompanyClass" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                    <a id="TraveA" href="javascript:void(0);" style="display: none; color: red;" onclick="ShowTravel()">▲</a>
                    <asp:HiddenField ID="Hid_Travel" runat="server" />
                </td>
                <td id="GoodsCode" runat="server" visible="False" style="text-align: right; width: 120px;">人事部物品管理编码：
                </td>
                <td id="txtGoodsCode" runat="server" visible="False">
                    <asp:TextBox ID="txt_GoodsCode" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
                <td id="CompanyClass" runat="server" visible="False" style="text-align: right;">广告使用图片：
                </td>
                <td id="ImgCompanyClass" runat="server" visible="False">
                    <asp:TextBox ID="UploadImgName" runat="server" ReadOnly="True" Width="250px"></asp:TextBox>
                    <a id="PreA" runat="server" visible="False" href="javascript:void(0);" target="_blank"><b style="color: red;">预览</b></a>
                </td>
            </tr>
            <tr id="TraveDetail" style="display: none;">
                <td colspan="4">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width: 81px; text-align: right;">随同人员：
                            </td>
                            <td>
                                <asp:TextBox ID="txt_Entourage" runat="server" TextMode="MultiLine" Width="99%" ReadOnly="True"></asp:TextBox>
                            </td>
                            <td style="width: 138px; text-align: right;">出差地点及历程：
                            </td>
                            <td>
                                <asp:TextBox ID="txt_TravelAddressAndCourse" runat="server" TextMode="MultiLine" Width="99%" ReadOnly="True"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table style="width: 100%; text-align: center; line-height: 28px;" cellpadding="0" cellspacing="0" class="table">
                        <tr>
                            <td style="width: 87px;">项目</td>
                            <td>张数</td>
                            <td>金额</td>
                            <td style="width: 87px;">项目</td>
                            <td>天数</td>
                            <td>金额</td>
                        </tr>
                        <tr>
                            <td>火车费</td>
                            <td>
                                <asp:TextBox ID="txt_TrainChargeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_TrainChargeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>餐费</td>
                            <td>
                                <asp:TextBox ID="txt_MealsDays" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_MealsAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>汽车费</td>
                            <td>
                                <asp:TextBox ID="txt_CarFeeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_CarFeeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>住宿费</td>
                            <td>
                                <asp:TextBox ID="txt_AccommodationDays" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_AccommodationAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>市内交通费</td>
                            <td>
                                <asp:TextBox ID="txt_CityFeeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_CityFeeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>过路费</td>
                            <td>
                                <asp:TextBox ID="txt_TollsNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_TollsAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>飞机费</td>
                            <td>
                                <asp:TextBox ID="txt_AircraftFeeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_AircraftFeeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>其它</td>
                            <td>
                                <asp:TextBox ID="txt_OtherDays" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_OtherAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly="True"></asp:TextBox></td>
                        </tr>
                    </table>
                    <table style="width: 100%; text-align: center; line-height: 28px;" cellpadding="0" cellspacing="0" class="table">
                        <tr>
                            <td style="width: 87px;">起日</td>
                            <td style="width: 87px;">止日</td>
                            <td>起讫地点</td>
                        </tr>
                        <asp:Repeater ID="Repeater_Termini" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txt_StartDay" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("StartDay")==null?null:DateTime.Parse(Eval("StartDay").ToString()).ToString("yyyy-MM-dd") %>' ReadOnly="True"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txt_EndDay" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("EndDay")==null?null:DateTime.Parse(Eval("EndDay").ToString()).ToString("yyyy-MM-dd") %>' ReadOnly="True"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txt_TerminiLocation" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("TerminiLocation") %>' ReadOnly="True"></asp:TextBox>
                                        </td>
                                    </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">紧急程度：
                </td>
                <td style="width: 391px;">
                    <asp:RadioButtonList ID="rbl_UrgentOrDefer" runat="server" RepeatDirection="Horizontal" Enabled="False">
                        <asp:ListItem Text="普通" Value="0"></asp:ListItem>
                        <asp:ListItem Text="加急" Value="1"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td style="text-align: right; width: 109px;">加急原因：
                </td>
                <td>
                    <asp:TextBox ID="txt_UrgentReason" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">
                    <asp:Literal ID="lit_ReportName" runat="server">费用名称</asp:Literal>：
                </td>
                <td>
                    <asp:TextBox ID="txt_ReportName" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
                <td style="text-align: right;">费用实际发生时间：
                </td>
                <td>
                    <asp:TextBox ID="txt_StartTime" runat="server" Width="70px" ReadOnly="True"></asp:TextBox>
                    至
                    <asp:TextBox ID="txt_EndTime" runat="server" Width="70px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;"><b>
                    <asp:Literal ID="lit_PayCompany" runat="server">收款</asp:Literal></b>单位：
                </td>
                <td>
                    <asp:TextBox ID="txt_PayCompany" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
                <td style="text-align: right;"><b style="color: red;">
                    <asp:Literal ID="lit_ReportCost" runat="server" Text="申报"></asp:Literal></b>金额：
                </td>
                <td>
                    <asp:TextBox ID="txt_ReportCost" runat="server" Width="250px" ReadOnly="True"></asp:TextBox><br />
                    <b>
                        <asp:Literal ID="Lit_CapitalAmount" runat="server"></asp:Literal></b>
                </td>
            </tr>
            <tr id="PayBankAccountAndAssumeFiliale" runat="server" visible="False">
                <td style="text-align: right;">结算账号：
                </td>
                <td>
                    <asp:TextBox ID="txt_PayBankAccount" runat="server" Width="250px" ReadOnly="True" Style="white-space: nowrap; overflow: hidden; text-overflow: ellipsis;"></asp:TextBox>
                </td>
                <td style="text-align: right;">结算公司：
                </td>
                <td>
                    <asp:TextBox ID="txt_AssumeFiliale" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr id="PoundageAndTradeNo" runat="server" visible="False">
                <td style="text-align: right;">手续费：
                </td>
                <td>
                    <asp:TextBox ID="txt_Poundage" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
                <td style="text-align: right;">交易流水号：
                </td>
                <td>
                    <asp:TextBox ID="txt_TradeNo" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">结算方式：
                </td>
                <td>
                    <asp:RadioButtonList ID="rbl_CostType" runat="server" RepeatDirection="Horizontal" Enabled="False">
                        <asp:ListItem Text="转账" Value="2"></asp:ListItem>
                        <asp:ListItem Text="现金" Value="1"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td id="BankName" runat="server" visible="False" style="text-align: right;"><b style="color: red;">
                    <asp:Literal ID="lit_BankName" runat="server" Text="收款"></asp:Literal></b>银行：
                </td>
                <td id="txtBankName" runat="server" visible="False">
                    <asp:TextBox ID="txt_BankName" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td id="SubBankName" runat="server" visible="False" style="text-align: right;"><b style="color: red;">
                    <asp:Literal ID="lit_SubBankName" runat="server" Text="收款"></asp:Literal></b>支行：
                </td>
                <td id="txtSubBankName" runat="server" visible="False">
                    <asp:TextBox ID="txt_SubBankName" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
                <td id="BankAccount" runat="server" visible="False" style="text-align: right;">收款账号：
                </td>
                <td id="txtBankAccount" runat="server" visible="False">
                    <asp:TextBox ID="txt_BankAccount" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td id="DepositNo" runat="server" visible="False" style="text-align: right;"><b style="color: blue;">押金编号</b>：
                </td>
                <td id="txtDepositNo" runat="server" visible="False">
                    <asp:TextBox ID="txt_DepositNo" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr id="IsLastTime" runat="server" visible="False">
                <td style="text-align: right;">最后一次：</td>
                <td>
                    <asp:RadioButtonList ID="rb_IsLastTime" runat="server" RepeatDirection="Horizontal" Enabled="False">
                        <asp:ListItem Value="True">是</asp:ListItem>
                        <asp:ListItem Value="False" Selected="True">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td id="rbIsEndTitle" runat="server" Visible="False" style="text-align: right;">是否终结：</td>
                <td id="rbIsEnd" runat="server" Visible="False">
                    <asp:RadioButtonList ID="rb_IsEnd" runat="server" RepeatDirection="Horizontal" Enabled="False">
                        <asp:ListItem Value="True">是(若选择终结，发票多出金额公司不予打款)</asp:ListItem>
                        <asp:ListItem Value="False" Selected="True">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr id="Amount" runat="server" visible="False">
                <td colspan="4" style="padding-left: 10px; padding-right: 10px;">
                    <table style="width: 100%; text-align: center; line-height: 28px;" cellpadding="0" cellspacing="0" class="table">
                        <tr>
                            <td style="width: 78px;">申报次数</td>
                            <td>申请金额</td>
                            <td style="width: 50px;">收/付款</td>
                        </tr>
                        <asp:Repeater ID="Repeater_Amount" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td>第<%# Eval("Num") %>次
                                    </td>
                                    <td>
                                        <%# decimal.Parse(Eval("Amount").ToString()).Equals(0)?"":Eval("Amount")%>
                                    </td>
                                    <td>
                                        <%# bool.Parse(Eval("IsPay").ToString())?"已完成":"<span style='color:red;'>未完成</span>" %>
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr>
                            <td style="color: red;">合计：</td>
                            <td colspan="3" style="text-align: center; padding-right: 50px; font-weight: bold;">
                                <asp:Literal ID="lit_SumAmount" runat="server"></asp:Literal></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr id="BillType" runat="server" visible="False">
                <td style="text-align: right;">票据类型：
                </td>
                <td>
                    <asp:RadioButtonList ID="rbl_InvoiceType" runat="server" RepeatDirection="Horizontal" Enabled="False">
                        <asp:ListItem Text="普通发票" Value="1"></asp:ListItem>
                        <asp:ListItem Text="增值税专用发票" Value="5"></asp:ListItem>
                        <asp:ListItem Text="收据" Value="2"></asp:ListItem>
                    </asp:RadioButtonList>
                    <asp:RadioButton ID="rbl_NoVoucher" runat="server" Text="网页凭证" Visible="False" Style="padding-left: 3px;" />
                    <a id="PreANoVoucher" runat="server" visible="False" href="javascript:void(0);" target="_blank"><b style="color: red;">预览</b></a>
                </td>
                <td id="InvoiceTitle" runat="server" style="width: 106px; text-align: right;">发票抬头：</td>
                <td>
                    <asp:TextBox ID="txt_InvoiceTitle" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr id="Bill" runat="server" visible="False">
                <td colspan="4">
                    <fieldset style="padding: 5px; margin-bottom: 5px;">
                        <legend id="InvoiceType" runat="server" style="font-weight: bold; color: blue; margin-left: 12px;">发票</legend>
                        <table style="width: 100%; text-align: center; line-height: 28px; margin-top: 10px; margin-bottom: 10px;" cellpadding="0" cellspacing="0" class="table">
                            <tr>
                                <td>开票单位</td>
                                <td style="width: 70px;">开票日期</td>
                                <td id="BillNo" runat="server" style="width: 100px;">发票号码</td>
                                <td id="BillCode" runat="server" style="width: 100px;">发票代码</td>
                                <td id="NoTaxAmount" runat="server" visible="False" style="width: 70px;">未税金额</td>
                                <td id="Tax" runat="server" visible="False" style="width: 70px;">税额</td>
                                <td id="TaxAmount" runat="server" style="width: 70px;">含税金额</td>
                                <td>备注</td>
                                <td style="width: 70px;">提交时间</td>
                                <td style="width: 50px;">收/付款</td>
                            </tr>
                            <asp:Repeater ID="Repeater_Bill" runat="server" OnItemDataBound="Repeater_Bill_ItemDataBound">
                                <ItemTemplate>
                                    <tr style="text-align: left;">
                                        <td style="padding-left: 3px;">
                                            <%# Eval("BillUnit") %>
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <%# Eval("BillDate")==null?null:DateTime.Parse(Eval("BillDate").ToString()).ToString("yyyy-MM-dd") %>
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <%# Eval("BillNo") %>
                                        </td>
                                        <td id="txtBillCode" runat="server" style="padding-left: 3px;">
                                            <%# Eval("BillCode") %>
                                        </td>
                                        <td id="txtNoTaxAmount" runat="server" style="padding-left: 3px;">
                                            <%#Eval("NoTaxAmount") ==DBNull.Value ?"": ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("NoTaxAmount").ToString())) %>
                                        </td>
                                        <td id="txtTax" runat="server" style="padding-left: 3px;">
                                            <%#Eval("Tax") ==DBNull.Value ?"": ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("Tax").ToString())) %>
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <%#Eval("TaxAmount") ==DBNull.Value ?"": ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("TaxAmount").ToString())) %>
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <%# Eval("Memo") %>
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <%# Convert.ToDateTime(Eval("OperatingTime")).ToString("yyyy-MM-dd") %>
                                        </td>
                                        <td style="padding-left: 3px;">
                                            <%# bool.Parse(Eval("IsPay").ToString())?"已完成":"<span style='color:red;'>未完成</span>" %>
                                        </td>
                                    </tr>
                                </ItemTemplate>
                            </asp:Repeater>
                            <tr>
                                <td id="SumTitle" runat="server" style="color: red;">合计：</td>
                                <td id="litSumNoTaxAmount" runat="server" style="font-weight: bold;">
                                    <asp:Literal ID="lit_SumNoTaxAmount" runat="server"></asp:Literal></td>
                                <td id="litSumTax" runat="server" style="font-weight: bold;">
                                    <asp:Literal ID="lit_SumTax" runat="server"></asp:Literal></td>
                                <td colspan="5" style="text-align: left; padding-left: 3px; font-weight: bold;">
                                    <asp:Literal ID="lit_SumTaxAmount" runat="server"></asp:Literal></td>
                            </tr>
                        </table>
                    </fieldset>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">申报说明：</td>
                <td colspan="3">
                    <asp:TextBox ID="txt_ReportMemo" runat="server" TextMode="MultiLine" Width="99%" Height="50px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr id="AuditingMemo" runat="server" visible="False">
                <td style="text-align: right;">操作说明：</td>
                <td colspan="3" style="font-weight: bold; color: blue;">
                    <asp:Literal ID="lit_AuditingMemo" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">申报流程：</td>
                <td colspan="3">
                    <asp:TextBox ID="txt_Memo" runat="server" TextMode="MultiLine" ReadOnly="True" Width="99%" Height="100px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: center; padding-top: 10px;">
                    <asp:Button ID="btn_Review" runat="server" Text="审阅" OnClick="btn_Review_Click" Visible="False" />
                </td>
            </tr>
        </table>
        <script src="../JavaScript/jquery.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script type="text/javascript">
            $(function () {
                if ($("input[id$='Hid_Travel']").val() === "0") {
                    $("input[id$='Hid_Travel']").val("1");
                    ShowTravel();
                }
                else if ($("input[id$='Hid_Travel']").val() === "1") {
                    $("input[id$='Hid_Travel']").val("0");
                    ShowTravel();
                }
            });

            //显示差旅费
            function ShowTravel() {
                $("#TraveA").show();
                if ($("input[id$='Hid_Travel']").val() === "1") {
                    $("#TraveDetail").show();
                    $("#TraveDetail").slideDown();
                    $("#TraveA").text("▲");
                    $("input[id$='Hid_Travel']").val("0");
                } else {
                    $("#TraveDetail").hide();
                    $("#TraveDetail").slideUp();
                    $("#TraveA").text("▼");
                    $("input[id$='Hid_Travel']").val("1");
                }
            }
        </script>
    </form>
</body>
</html>


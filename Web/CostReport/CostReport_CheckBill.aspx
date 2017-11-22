<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_CheckBill.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_CheckBill" MaintainScrollPositionOnPostback="true" %>

<%@ Import Namespace="ERP.Enum" %>

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
                    <asp:HiddenField ID="Hid_FeeType" runat="server" />
                    <a id="TraveA" href="javascript:void(0);" style="display: none; color: red;" onclick="ShowTravel()">▲</a>
                    <asp:HiddenField ID="Hid_Travel" runat="server" />
                    <asp:HiddenField ID="Hid_TravelBefore" runat="server" />
                    <br/><b style="color: red;">
                        <asp:Literal ID="Lit_Msg" runat="server"></asp:Literal></b>
                </td>
                <td id="GoodsCode" runat="server" visible="False" style="text-align: right; width: 120px;">人事部物品管理编码：
                </td>
                <td id="txtGoodsCode" runat="server" visible="False">
                    <asp:TextBox ID="txt_GoodsCode" runat="server" Width="250px"></asp:TextBox>
                </td>
                <td id="CompanyClass" runat="server" visible="False" style="text-align: right;">广告使用图片：
                </td>
                <td id="ImgCompanyClass" runat="server" visible="False">
                    <asp:TextBox ID="UploadImgName" runat="server" onfocus="this.blur();" Width="175px"></asp:TextBox>
                    <asp:FileUpload ID="UploadImg" runat="server" Style="display: none;" />
                    <input id="btnUploadImg" type="button" value="选择文件" title="选择图片!" onclick="UploadImg.click()" />
                    <a id="PreA" runat="server" href="javascript:void(0);" target="_blank"><b style="color: red;">预览</b></a>
                    <img id="imgPre" style="display: none;" />
                    <a id="clear" href="javascript:void(0);" onclick="clearImg()"><b style="color: red;">删除</b></a>
                </td>
            </tr>
            <tr id="TraveDetail" style="display: none;">
                <td colspan="4">
                    <table style="width: 100%;">
                        <tr>
                            <td style="width: 81px; text-align: right;">随同人员：
                            </td>
                            <td>
                                <asp:TextBox ID="txt_Entourage" runat="server" TextMode="MultiLine" Width="99%"></asp:TextBox>
                            </td>
                            <td style="width: 138px; text-align: right;">出差地点及历程：
                            </td>
                            <td>
                                <asp:TextBox ID="txt_TravelAddressAndCourse" runat="server" TextMode="MultiLine" Width="99%"></asp:TextBox>
                            </td>
                        </tr>
                    </table>
                    <table id="TraveItem" style="width: 100%; text-align: center; line-height: 28px;" cellpadding="0" cellspacing="0" class="table">
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
                                <asp:TextBox ID="txt_TrainChargeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_TrainChargeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>餐费</td>
                            <td>
                                <asp:TextBox ID="txt_MealsDays" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_MealsAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>汽车费</td>
                            <td>
                                <asp:TextBox ID="txt_CarFeeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_CarFeeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>住宿费</td>
                            <td>
                                <asp:TextBox ID="txt_AccommodationDays" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_AccommodationAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>市内交通费</td>
                            <td>
                                <asp:TextBox ID="txt_CityFeeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_CityFeeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>过路费</td>
                            <td>
                                <asp:TextBox ID="txt_TollsNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_TollsAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>飞机费</td>
                            <td>
                                <asp:TextBox ID="txt_AircraftFeeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_AircraftFeeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>其它</td>
                            <td>
                                <asp:TextBox ID="txt_OtherDays" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_OtherAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;"></asp:TextBox></td>
                        </tr>
                    </table>
                    <div style="margin: 5px 0 5px 0; text-align: right;">
                        <asp:Button ID="btn_AddTermin" runat="server" Text="添加起讫" OnClick="btn_AddTermini_Click" />
                    </div>
                    <table id="Termini" style="width: 100%; text-align: center; line-height: 28px;" cellpadding="0" cellspacing="0" class="table">
                        <tr>
                            <td style="width: 87px;">起日</td>
                            <td style="width: 87px;">止日</td>
                            <td>起讫地点</td>
                            <td id="TerminiControl" style="width: 40px;">操作</td>
                        </tr>
                        <asp:Repeater ID="Repeater_Termini" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txt_StartDay" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("StartDay")==null?null:DateTime.Parse(Eval("StartDay").ToString()).ToString("yyyy-MM-dd") %>' onchange="ReplaceIdAndClick(this.id,'txt_StartDay','btn_TerminiAdd');" lang="CheckTermini"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txt_EndDay" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("EndDay")==null?null:DateTime.Parse(Eval("EndDay").ToString()).ToString("yyyy-MM-dd") %>' onchange="ReplaceIdAndClick(this.id,'txt_EndDay','btn_TerminiAdd');" lang="CheckTermini"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txt_TerminiLocation" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("TerminiLocation") %>' onchange="ReplaceIdAndClick(this.id,'txt_TerminiLocation','btn_TerminiAdd');" lang="CheckTermini"></asp:TextBox>
                                        </td>
                                        <td id="RepeaterTerminiControl">
                                            <a id="TerminiDel" runat="server" href="javascript:void(0);" title="删除" onclick="ReplaceIdAndClick(this.id,'TerminiDel','btn_TerminiDel');">
                                                <img src="../icon/bullet_cross.png" />
                                            </a>
                                            <asp:Button ID="btn_TerminiAdd" runat="server" Text="保存" Style="display: none;" CommandName='<%# Eval("TerminiId") %>' OnClick="btn_TerminiAdd_Click" OnClientClick="return CheckRepeaterData(this.id,'btn_TerminiAdd','CheckTermini');" />
                                            <asp:Button ID="btn_TerminiDel" runat="server" Text="删除" Style="display: none;" CommandName='<%# Eval("TerminiId") %>' OnClick="btn_TerminiDel_Click" />
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
                    <asp:RadioButtonList ID="rb_IsLastTime" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="rb_IsLastTime_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem Value="True">是</asp:ListItem>
                        <asp:ListItem Value="False" Selected="True">否</asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td id="rbIsEndTitle" runat="server" Visible="False" style="text-align: right;">是否终结：</td>
                <td id="rbIsEnd" runat="server" Visible="False">
                    <asp:RadioButtonList ID="rb_IsEnd" runat="server" RepeatDirection="Horizontal">
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
                            <td style="width: 40px;">操作</td>
                        </tr>
                        <asp:Repeater ID="Repeater_Amount" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <td style="display: none;">
                                        <asp:Literal ID="lit_IsPay" runat="server" Text='<%# Eval("IsPay") %>'></asp:Literal>
                                        <asp:Literal ID="lit_IsSystem" runat="server" Text='<%# Eval("IsSystem") %>'></asp:Literal>
                                    </td>
                                    <td>第<asp:Literal ID="lit_Num" runat="server" Text='<%# Eval("Num") %>'></asp:Literal>次
                                    </td>
                                    <td>
                                        <asp:TextBox ID="txt_Amount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none; text-align: center;" ReadOnly='<%# bool.Parse(Eval("IsPay").ToString())%>' Text='<%# decimal.Parse(Eval("Amount").ToString()).Equals(0)?"":Eval("Amount")%>' onchange="ReplaceIdAndClick(this.id,'txt_Amount','btn_AmountAdd');" lang="CheckAmount"></asp:TextBox>
                                    </td>
                                    <td>
                                        <%# bool.Parse(Eval("IsPay").ToString())?"已完成":"<span style='color:red;'>未完成</span>" %>
                                    </td>
                                    <td>
                                        <a id="AddAmount" runat="server" href="javascript:void(0);" title="添加" visible='<%# bool.Parse(Eval("IsPay").ToString())%>' onclick="ReplaceIdAndClick(this.id,'AddAmount','btn_AddAmount');">
                                            <img src="../icon/add.png" /></a>
                                        <a id="AmountDel" runat="server" href="javascript:void(0);" title="删除" visible='<%# !bool.Parse(Eval("IsPay").ToString())%>' onclick="ReplaceIdAndClick(this.id,'AmountDel','btn_AmountDel');">
                                            <img src="../icon/bullet_cross.png" /></a>
                                        <asp:Button ID="btn_AddAmount" runat="server" Text="添加" Style="display: none;" OnClick="btn_AddAmount_Click" />
                                        <asp:Button ID="btn_AmountAdd" runat="server" Text="保存" Style="display: none;" CommandName='<%# Eval("AmountId") %>' OnClick="btn_AmountAdd_Click" OnClientClick="return CheckRepeaterData(this.id,'btn_AmountAdd','CheckAmount');" />
                                        <asp:Button ID="btn_AmountDel" runat="server" Text="删除" Style="display: none;" CommandName='<%# Eval("AmountId") %>' OnClick="btn_AmountDel_Click" />
                                    </td>
                                </tr>
                            </ItemTemplate>
                        </asp:Repeater>
                        <tr>
                            <td style="color: red;">合计：</td>
                            <td colspan="4" style="text-align: center; padding-right: 90px; font-weight: bold;">
                                <asp:Literal ID="lit_SumAmount" runat="server"></asp:Literal></td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">票据类型：
                </td>
                <td>
                    <asp:RadioButtonList ID="rbl_InvoiceType" runat="server" RepeatDirection="Horizontal" Enabled="False" OnSelectedIndexChanged="rbl_InvoiceType_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem Text="普通发票" Value="1"></asp:ListItem>
                        <asp:ListItem Text="增值税专用发票" Value="5"></asp:ListItem>
                        <asp:ListItem Text="收据" Value="2"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td id="InvoiceTitle" runat="server" style="width: 106px; text-align: right;">发票抬头：</td>
                <td>
                    <asp:TextBox ID="txt_InvoiceTitle" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <fieldset style="padding: 5px; margin-bottom: 5px;">
                        <legend id="InvoiceType" runat="server" style="font-weight: bold; color: blue; margin-left: 12px;">发票</legend>
                        <div>
                            <asp:Literal ID="lit_Title" runat="server">发票</asp:Literal>导入：<asp:TextBox ID="UploadExcelName" runat="server" onfocus="this.blur();"></asp:TextBox>
                            <asp:FileUpload ID="UploadExcel" runat="server" Style="display: none;" onchange="CheckFile()" />
                            <input type="button" value="选择文件" title="文件格式(.xls)!" onclick="UploadExcel.click()" />
                            <asp:Button ID="btn_Upload" runat="server" Text="导入" OnClick="btn_Upload_Click" />
                            <a id="Template" runat="server" href="../App_Themes/费用申报票据模板(普通发票).xls" style="color: red; font-weight: bold;">模板下载</a>
                            <asp:Button ID="btn_AddBill" runat="server" Text="添加" Style="float: right; margin-right: 15px;" OnClick="btn_AddBill_Click" />
                        </div>

                        <table id="Bill" style="width: 100%; text-align: center; line-height: 28px; margin-top: 10px; margin-bottom: 10px;" cellpadding="0" cellspacing="0" class="table">
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
                                <td style="width: 40px;">操作</td>
                            </tr>
                            <asp:Repeater ID="Repeater_Bill" runat="server" OnItemDataBound="Repeater_Bill_ItemDataBound">
                                <ItemTemplate>
                                    <tr>
                                        <td style="display: none;">
                                            <asp:Literal ID="lit_Remark" runat="server" Text='<%# Eval("Remark") %>'></asp:Literal>
                                            <asp:Literal ID="lit_BillState" runat="server" Text='<%# Eval("BillState") %>'></asp:Literal>
                                            <asp:Literal ID="lit_IsPay" runat="server" Text='<%# Eval("IsPay") %>'></asp:Literal>
                                            <asp:Literal ID="lit_IsPass" runat="server" Text='<%# Eval("IsPass") %>'></asp:Literal>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_BillUnit" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly='<%# bool.Parse(Eval("IsPay").ToString())%>' Text='<%# Eval("BillUnit") %>' onchange="ReplaceIdAndClick(this.id,'txt_BillUnit','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_BillDate" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly='<%# bool.Parse(Eval("IsPay").ToString())%>' onblur="check(this,'DateTime');" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM-dd'})" Text='<%# Eval("BillDate")==null?null:DateTime.Parse(Eval("BillDate").ToString()).ToString("yyyy-MM-dd") %>' onchange="ReplaceIdAndClick(this.id,'txt_BillDate','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_BillNo" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly='<%# bool.Parse(Eval("IsPay").ToString())%>' Text='<%# Eval("BillNo") %>' onchange="ReplaceIdAndClick(this.id,'txt_BillNo','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td id="txtBillCode" runat="server">
                                            <asp:TextBox ID="txt_BillCode" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly='<%# bool.Parse(Eval("IsPay").ToString())%>' Text='<%# Eval("BillCode") %>' onchange="ReplaceIdAndClick(this.id,'txt_BillCode','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td id="txtNoTaxAmount" runat="server">
                                            <asp:TextBox ID="txt_NoTaxAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly='<%# bool.Parse(Eval("IsPay").ToString())%>' Text='<%#decimal.Parse(Eval("NoTaxAmount").ToString()).Equals(0)?"":Eval("NoTaxAmount")%>' onblur="check(this,'Decimal');" onchange="SetTaxAmount(this.id,'txt_NoTaxAmount');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td id="txtTax" runat="server">
                                            <asp:TextBox ID="txt_Tax" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly='<%# bool.Parse(Eval("IsPay").ToString())%>' Text='<%#decimal.Parse(Eval("Tax").ToString()).Equals(0)?"":Eval("Tax") %>' onblur="check(this,'Decimal');" onchange="SetTaxAmount(this.id,'txt_Tax');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_TaxAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly='<%# bool.Parse(Eval("IsPay").ToString())%>' Text='<%#decimal.Parse(Eval("TaxAmount").ToString()).Equals(0)?"":Eval("TaxAmount")%>' onblur="check(this,'Decimal');" onchange="ReplaceIdAndClick(this.id,'txt_TaxAmount','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_Memo" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" ReadOnly='<%# bool.Parse(Eval("IsPay").ToString())%>' Text='<%# Eval("Memo") %>' onchange="ReplaceIdAndClick(this.id,'txt_Memo','btn_BillAdd');"></asp:TextBox>
                                        </td>
                                        <td>
                                            <%# Convert.ToDateTime(Eval("OperatingTime")).ToString("yyyy-MM-dd") %>
                                        </td>
                                        <td>
                                            <%# bool.Parse(Eval("IsPay").ToString())?"已完成":"<span style='color:red;'>未完成</span>" %>
                                        </td>
                                        <td>
                                            <a id="BillDel" runat="server" href="javascript:void(0);" visible='<%# !bool.Parse(Eval("IsPay").ToString())%>' title="删除" onclick="ReplaceIdAndClick(this.id,'BillDel','btn_BillDel');">
                                                <img src="../icon/bullet_cross.png" />
                                            </a>
                                            <asp:Button ID="btn_BillAdd" runat="server" Text="保存" Style="display: none;" CommandName='<%# Eval("BillId") %>' OnClick="btn_BillAdd_Click" OnClientClick="return CheckRepeaterData(this.id,'btn_BillAdd','CheckBill');" />
                                            <asp:Button ID="btn_BillDel" runat="server" Text="删除" Style="display: none;" CommandName='<%# Eval("BillId") %>' OnClick="btn_BillDel_Click" />
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
                    <asp:TextBox ID="txt_ReportMemo" runat="server" TextMode="MultiLine" ReadOnly="True" Width="99%" Height="50px" placeholder="请输入申报说明,最多可输入1000字！" onblur="CheckChinese(this)" lang='申报说明'></asp:TextBox>
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
                <td colspan="4" style="text-align: center;">
                    <asp:Button ID="btn_Save" runat="server" Text="保存" OnClientClick="return CheckEmpty();" OnClick="btn_Save_Click" />
                </td>
            </tr>
        </table>
        <asp:HiddenField ID="Hid_ReportCost" runat="server" />
        <asp:HiddenField ID="Hid_CostsClass" runat="server" />
        <script src="../JavaScript/jquery.js"></script>
        <script src="../My97DatePicker/WdatePicker.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script src="../JavaScript/tool.js"></script>
        <script src="../JavaScript/PreviewImage.js"></script>
        <script type="text/javascript">
            $(function () {
                $("input[id$='UploadExcelName']").val("");//回发时清空导入文本框中的值
                SetReadOnly();//当申报类型是“预借款”，状态是“票据待选”时差旅费和起讫是允许添加、修改的
                if ($("input[id$='Hid_Travel']").val() === "0") {
                    $("input[id$='Hid_Travel']").val("1");
                    ShowTravel();
                }
                else if ($("input[id$='Hid_Travel']").val() === "1") {
                    $("input[id$='Hid_Travel']").val("0");
                    ShowTravel();
                }

                SetTaxAmountReadOnly();//当票据类型是“增值税专用发票”时，票据含税金额不允许编辑
                PreviewImage();//图片预览
            });

            //当申报类型是“预借款”时允许添加、修改差旅费和起讫以及“人事部物品管理编码”、“广告使用图片”
            function SetReadOnly() {
                var selectedReportKind = $("input[type='radio'][name='rbl_ReportKind']:checked").val();
                if (selectedReportKind === "1") {
                    /*--“人事部物品管理编码”、“广告使用图片”相关控制--*/
                    var uploadImgName = $("input[id$='UploadImgName']");
                    if (uploadImgName.val() !== "" && uploadImgName.val() !== undefined) {
                        if (uploadImgName.val().indexOf('\\') > 0) {
                            uploadImgName.val("");
                            $("td[lang='Img'] a").attr("href", "javascript:void(0);");
                        }
                    }

                    var txtGoodsCode = $("input[id$='txt_GoodsCode']");
                    var hidCostsClass = $("input[id$='Hid_CostsClass']");
                    var hidReportCost = $("input[id$='Hid_ReportCost']");
                    if (hidCostsClass.val() === "0") {
                        uploadImgName.addClass("Check");
                        uploadImgName.attr('lang', '广告使用图片');
                    } else {
                        uploadImgName.removeClass("Check");
                    }
                    if (hidCostsClass.val() === "1" && parseFloat(hidReportCost.val()) >= 500) {
                        txtGoodsCode.addClass("Check");
                        txtGoodsCode.attr('lang', '人事部物品管理编码');
                    } else {
                        txtGoodsCode.removeClass("Check");
                    }
                    /*--“人事部物品管理编码”、“广告使用图片”相关控制--*/

                    /*--差旅费控制--*/
                    $("#TraveItem input[type='text']").on("blur", function () {
                        check(this, 'Decimal');
                    });
                    /*--差旅费控制--*/

                    /*--起讫时间控制--*/
                    $("#TraveDetail input[id$='txt_StartDay']").on("click", function () {
                        WdatePicker({ skin: 'blue', dateFmt: 'yyyy-MM-dd', maxDate: '#F{$dp.$D(\'' + this.id.replace('txt_StartDay', 'txt_EndDay') + '\')}' });
                    });
                    $("#TraveDetail input[id$='txt_EndDay']").on("click", function () {
                        WdatePicker({ skin: 'blue', dateFmt: 'yyyy-MM-dd', minDate: '#F{$dp.$D(\'' + this.id.replace('txt_EndDay', 'txt_StartDay') + '\')}' });
                    });
                    /*--起讫时间控制--*/
                } else {
                    /*--“人事部物品管理编码”、“广告使用图片”相关控制--*/
                    $("input[id$='txt_GoodsCode']").attr("readonly", "true");
                    $("input[id$='UploadImgName']").css("width", "250px");
                    if ($("input[id$='UploadImgName']").val() === "0") {
                        $("#PreA").css("display", "none");
                    }
                    $("input[id$='btnUploadImg']").css("display", "none");
                    $("#clear").css("display", "none");
                    /*--“人事部物品管理编码”、“广告使用图片”相关控制--*/

                    /*--差旅费、起讫控制--*/
                    $("#TraveDetail textarea").attr("readonly", "true");
                    $("#TraveDetail input[type='text']").attr("readonly", "true");
                    $("#TraveDetail div").css("display", "none");
                    $("#TraveDetail td[id$='TerminiControl']").css("display", "none");
                    /*--差旅费、起讫控制--*/
                }
            }

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

            //设置含税金额  含税金额=未税金额+税额
            function SetTaxAmount(id, type) {
                var noTaxAmount = 0, tax = 0;
                if (type === "txt_NoTaxAmount") {
                    noTaxAmount = document.getElementById(id).value;
                    tax = document.getElementById(id.replace("txt_NoTaxAmount", "txt_Tax")).value;
                    document.getElementById(id.replace("txt_NoTaxAmount", "txt_TaxAmount")).value = ((noTaxAmount.length === 0 ? 0 : parseFloat(noTaxAmount)) +(tax.length === 0 ? 0 : parseFloat(tax))).toFixed(3);
                } else if (type === "txt_Tax") {
                    noTaxAmount = document.getElementById(id.replace("txt_Tax", "txt_NoTaxAmount")).value;
                    tax = document.getElementById(id).value; 
                    document.getElementById(id.replace("txt_Tax", "txt_TaxAmount")).value = ((noTaxAmount.length === 0 ? 0 : parseFloat(noTaxAmount)) +(tax.length === 0 ? 0 : parseFloat(tax))).toFixed(3);
                }

                ReplaceIdAndClick(id, type, 'btn_BillAdd');
            }

            //当票据类型是“增值税专用发票”时，票据含税金额不允许编辑
            function SetTaxAmountReadOnly() {
                var selectedInvoiceType = $("input[type='radio'][name='rbl_InvoiceType']:checked").val();
                if (selectedInvoiceType === "5") {
                    $("input[id$='txt_TaxAmount']").bind("focus", function () {
                        this.blur();
                    });
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

                //验证操作说明
                var resultReportMemo = CheckReportMemo();
                if (resultReportMemo.length !== 0) {
                    errorMsg += resultReportMemo;
                }

                var resultTermini = CheckRepeaterData('', '', 'CheckTermini'); //验证起讫数据
                var resultAmount = CheckRepeaterData('', '', 'CheckAmount');//验证申请金额

                if ($("span[class='error']").length === 0) {
                    if (!resultTermini || !resultAmount || !billFlag) {
                        if (errorMsg.length !== 0) {
                            alert(errorMsg.substring(1) + '不能为空！');
                        }
                        return false;
                    } else {
                        return true;
                    }
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

            //图片预览
            function PreviewImage() {
                $("#UploadImg").uploadPreview({
                    Img: "imgPre", Callback: function () {
                        $("#UploadImgName").val($("#UploadImg").val());
                        $("#PreA").attr("href", $("#imgPre").attr("src"));
                    }
                });
            }

            //清除上传的图片信息
            function clearImg() {
                $("input[id$='UploadImgName']").val("");
                $("#UploadImg").val("");
                $("#PreA").attr("href", "javascript:void(0);");
            }

            var billFlag = true;
            //验证Repeater票据、起讫、申请金额数据
            function CheckRepeaterData(id, type, checklang) {
                var result = true;
                $("input" + ((id.length === 0 && type.length === 0) ? "" : "[id^='" + id.replace(type, "") + "']") + "[lang='" + checklang + "']").each(function () {
                    var obj = $(this);

                    var itemId = obj.attr("id");
                    if (checklang === "CheckBill") {
                        if (itemId.indexOf("txt_NoTaxAmount") > -1 || itemId.indexOf("txt_Tax") > -1 || itemId.indexOf("txt_TaxAmount") > -1) {
                            check(this, 'Decimal');
                        }
                        if (itemId.indexOf("txt_BillDate") > -1) {
                            check(this, 'DateTime');
                        }
                    } else if (checklang === "CheckAmount") {
                        var itemName = obj.attr("name");
                        if (itemId.indexOf("txt_Amount") > -1) {
                            var rbIsLastTimeName = itemName.replace("txt_Amount", "LastTime");
                            if ($("input[name='" + rbIsLastTimeName + "']:checked").val() === "0" && $(this).val() === "0") {
                                $(this).val("");
                            }
                            check(this, 'Decimal');
                        } else if (itemName.indexOf("LastTime") > -1) {
                            var txtAmountName = itemName.replace("LastTime", "txt_Amount");
                            if ($("input[name='" + itemName + "']:checked").val() === "0" && $("input[name='" + txtAmountName + "']").val() === "0") {
                                $("input[name='" + txtAmountName + "']").val("");
                            }
                        }
                    }

                    if (obj.val().length === 0) {
                        obj.css("border-bottom", "1px solid red");
                        result = false;
                    } else {
                        obj.css("border-bottom", "0px solid #fff");
                    }
                });

                //当票据信息没有填写完整，并且点击保存按钮时，用此判断
                if (checklang === "CheckBill") {
                    billFlag = result;
                }

                return result;
            }

            //替换id并执行替换后的id的事件
            function ReplaceIdAndClick(id, oldStr, newStr) {
                $("#" + id.replace(oldStr, newStr)).click();
            }

            //验证文件格式
            function CheckFile() {
                var filePath = document.getElementById("UploadExcel").value;
                var ext = filePath.substr(filePath.length - 4, 4).toLowerCase();
                if (ext !== ".xls") {
                    alert("请选择格式为“.xls”文件！");
                    document.getElementById("UploadExcelName").value = "";
                } else {
                    document.getElementById("UploadExcelName").value = filePath;
                }
            }
        </script>
    </form>
</body>
</html>

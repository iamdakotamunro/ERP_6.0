<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_Later.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_Later" MaintainScrollPositionOnPostback="true" %>

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
<body style="overflow-y: auto;">
    <form id="form1" runat="server">
        <div style="color: blue; padding: 5px 0px 5px 27px; font-weight: bold;">
            说明：凭发票报销费用。
        </div>
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
                <td>
                    <asp:DropDownList ID="ddl_CompanyClass" runat="server" OnSelectedIndexChanged="ddl_CompanyClass_SelectedIndexChanged" AutoPostBack="True" CssClass="Check" lang='费用分类'>
                        <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <asp:DropDownList ID="ddl_FeeType" runat="server" OnSelectedIndexChanged="ddl_FeeType_SelectedIndexChanged" AutoPostBack="True" CssClass="Check">
                        <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    </asp:DropDownList>
                    <a id="TraveA" href="javascript:void(0);" style="display: none; color: red;" onclick="ShowTravel()">▲</a>
                    <asp:HiddenField ID="Hid_Travel" runat="server" />
                </td>
                <td lang="GoodsCode" style="text-align: right; width: 120px; display: none;">人事部物品管理编码：
                </td>
                <td lang="GoodsCode" style="display: none;">
                    <asp:TextBox ID="txt_GoodsCode" runat="server" Width="250px"></asp:TextBox>
                </td>
                <td lang="Img" style="text-align: right; display: none;">广告使用图片：
                </td>
                <td lang="Img" style="display: none;">
                    <asp:TextBox ID="UploadImgName" runat="server" onfocus="this.blur();" Width="175px"></asp:TextBox>
                    <asp:FileUpload ID="UploadImg" runat="server" Style="display: none;" />
                    <input id="btnUploadImg" type="button" value="选择文件" title="选择图片!" onclick="UploadImg.click()" />
                    <a id="PreA" runat="server" href="javascript:void(0);" target="_blank"><b style="color: red;">预览</b></a>
                    <img id="imgPre" style="display: none;" />
                    <a href="javascript:void(0);" onclick="clearImg()"><b style="color: red;">删除</b></a>
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
                                <asp:TextBox ID="txt_TrainChargeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_TrainChargeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>餐费</td>
                            <td>
                                <asp:TextBox ID="txt_MealsDays" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_MealsAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>汽车费</td>
                            <td>
                                <asp:TextBox ID="txt_CarFeeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_CarFeeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>住宿费</td>
                            <td>
                                <asp:TextBox ID="txt_AccommodationDays" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_AccommodationAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                        </tr>
                        <tr>
                            <td>市内交通费</td>
                            <td>
                                <asp:TextBox ID="txt_CityFeeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_CityFeeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>过路费</td>
                            <td>
                                <asp:TextBox ID="txt_TollsNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_TollsAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td></td>
                            <td></td>
                            <td></td>
                        </tr>
                        <tr>
                            <td>飞机费</td>
                            <td>
                                <asp:TextBox ID="txt_AircraftFeeNum" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_AircraftFeeAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>其它</td>
                            <td>
                                <asp:TextBox ID="txt_OtherDays" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
                            <td>
                                <asp:TextBox ID="txt_OtherAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'Decimal');"></asp:TextBox></td>
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
                            <td style="width: 40px;">操作</td>
                        </tr>
                        <asp:Repeater ID="Repeater_Termini" runat="server">
                            <ItemTemplate>
                                <tr>
                                    <tr>
                                        <td>
                                            <asp:TextBox ID="txt_StartDay" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM-dd',maxDate:'#F{$dp.$D(\''+this.id.replace('txt_StartDay', 'txt_EndDay')+'\')}'})" Text='<%# Eval("StartDay")==null?null:DateTime.Parse(Eval("StartDay").ToString()).ToString("yyyy-MM-dd") %>' onchange="ReplaceIdAndClick(this.id,'txt_StartDay','btn_TerminiAdd');" lang="CheckTermini"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txt_EndDay" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\''+this.id.replace('txt_EndDay', 'txt_StartDay')+'\')}'})" Text='<%# Eval("EndDay")==null?null:DateTime.Parse(Eval("EndDay").ToString()).ToString("yyyy-MM-dd") %>' onchange="ReplaceIdAndClick(this.id,'txt_EndDay','btn_TerminiAdd');" lang="CheckTermini"></asp:TextBox></td>
                                        <td>
                                            <asp:TextBox ID="txt_TerminiLocation" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("TerminiLocation") %>' onchange="ReplaceIdAndClick(this.id,'txt_TerminiLocation','btn_TerminiAdd');" lang="CheckTermini"></asp:TextBox>
                                        </td>
                                        <td>
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
                <td style="text-align: right;"><b>收款</b>单位：
                </td>
                <td>
                    <asp:TextBox ID="txt_PayCompany" runat="server" Width="250px" CssClass="Check" lang='收款单位'></asp:TextBox>
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
                <td style="text-align: right;">结算方式：
                </td>
                <td>
                    <asp:RadioButtonList ID="rbl_CostType" runat="server" RepeatDirection="Horizontal" onchange="ChangeCostType()">
                        <asp:ListItem Text="转账" Value="2" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="现金" Value="1"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>

                <td class="BankName" style="text-align: right;"><b style="color: red;">收款</b>银行：
                </td>
                <td class="BankName" lang="BankName">
                    <asp:TextBox ID="txt_BankName" runat="server" placeholder="上海银行" CssClass="Check" Width="250px" onchange="CheckSubBankName()"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="BankName" style="text-align: right;"><b style="color: red;">收款</b>支行：
                </td>
                <td class="BankName" lang="BankName">
                    <asp:TextBox ID="txt_SubBankName" runat="server" placeholder="松江支行" Width="250px" CssClass="Check" lang='收款银行'></asp:TextBox>
                </td>
                <td class="ClearNum" style="text-align: right;">收款账号：
                </td>
                <td class="ClearNum" lang="ClearNum">
                    <asp:TextBox ID="txt_BankAccount" runat="server" Width="250px" CssClass="Check" lang='收款支行'></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td id="DepositNo" runat="server" visible="False" style="text-align: right;"><b style="color: blue;">押金编号</b>：
                </td>
                <td id="txtDepositNo" runat="server" visible="False">
                    <asp:TextBox ID="txt_DepositNo" runat="server" Width="250px" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="color: red; font-weight: bold; padding-left: 25px;">审批通过了，请把凭证送至(<span style="color: blue;"><asp:Literal ID="lit_InvoiceAcceptPersonInfo" runat="server" Text="财务部"></asp:Literal></span>)接收后，财务再汇款。</td>
            </tr>
            <tr>
                <td style="text-align: right;">票据类型：
                </td>
                <td>
                    <asp:RadioButtonList ID="rbl_InvoiceType" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="rbl_InvoiceType_SelectedIndexChanged" AutoPostBack="True">
                        <asp:ListItem Text="普通发票" Value="1" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="增值税专用发票" Value="5"></asp:ListItem>
                        <asp:ListItem Text="收据" Value="2"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td id="InvoiceTitle" runat="server" style="text-align: right; vertical-align: top; padding-top: 7px;">发票抬头：</td>
                <td style="vertical-align: top; padding-top: 5px;">
                    <asp:DropDownList ID="ddl_InvoiceTitle" runat="server" CssClass="Check" lang='发票抬头'></asp:DropDownList>
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
                                            <asp:TextBox ID="txt_BillUnit" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("BillUnit") %>' onchange="ReplaceIdAndClick(this.id,'txt_BillUnit','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_BillDate" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" onblur="check(this,'DateTime');" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM-dd'})" Text='<%# Eval("BillDate")==null?null:DateTime.Parse(Eval("BillDate").ToString()).ToString("yyyy-MM-dd") %>' onchange="ReplaceIdAndClick(this.id,'txt_BillDate','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_BillNo" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("BillNo") %>' onchange="ReplaceIdAndClick(this.id,'txt_BillNo','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td id="txtBillCode" runat="server">
                                            <asp:TextBox ID="txt_BillCode" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("BillCode") %>' onchange="ReplaceIdAndClick(this.id,'txt_BillCode','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td id="txtNoTaxAmount" runat="server">
                                            <asp:TextBox ID="txt_NoTaxAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%#decimal.Parse(Eval("NoTaxAmount").ToString()).Equals(0)?"":Eval("NoTaxAmount")%>' onblur="check(this,'Decimal');" onchange="SetTaxAmount(this.id,'txt_NoTaxAmount');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td id="txtTax" runat="server">
                                            <asp:TextBox ID="txt_Tax" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%#decimal.Parse(Eval("Tax").ToString()).Equals(0)?"":Eval("Tax") %>' onblur="check(this,'Decimal');" onchange="SetTaxAmount(this.id,'txt_Tax');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_TaxAmount" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%#decimal.Parse(Eval("TaxAmount").ToString()).Equals(0)?"":Eval("TaxAmount")%>' onblur="check(this,'Decimal');" onchange="ReplaceIdAndClick(this.id,'txt_TaxAmount','btn_BillAdd');" lang="CheckBill"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="txt_Memo" runat="server" Style="width: 98%; height: 99%; border: 0; outline: none;" Text='<%# Eval("Memo") %>' onchange="ReplaceIdAndClick(this.id,'txt_Memo','btn_BillAdd');"></asp:TextBox>
                                        </td>
                                        <td>
                                            <a id="BillDel" runat="server" href="javascript:void(0);" title="删除" onclick="ReplaceIdAndClick(this.id,'BillDel','btn_BillDel');">
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
                                <td colspan="3" style="text-align: left; padding-left: 27px; font-weight: bold;">
                                    <asp:Literal ID="lit_SumTaxAmount" runat="server"></asp:Literal></td>
                            </tr>
                        </table>
                    </fieldset>
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
        <script src="../JavaScript/PreviewImage.js"></script>
        <script type="text/javascript">
            $(function () {
                $("input[id$='UploadExcelName']").val("");//回发时清空导入文本框中的值
                ChangeCostsClass();//费用种类
                if ($("input[id$='Hid_Travel']").val() === "0" || $("input[id$='Hid_Travel']").val() === "2") {
                    $("input[id$='Hid_Travel']").val("1");
                    ShowTravel();
                }
                else if ($("input[id$='Hid_Travel']").val() === "1") {
                    $("input[id$='Hid_Travel']").val("0");
                    ShowTravel();
                }
                ChangeUrgentOrDefer();//紧急程度
                ChangeCostTypeChild();//切换结算方式子元素
                CheckSubBankName();//如果“收/付款银行”是非银行(如支付宝，微信等)，则“收/付款支行”是非必填项
                SetTaxAmountReadOnly();//当票据类型是“增值税专用发票”时，票据含税金额不允许编辑
                PreviewImage();//图片预览
            });

            //费用种类
            function ChangeCostsClass() {
                ClearCostsClassChildData();
                ChangeCostsClassChild();
            }

            //切换费用种类子元素
            function ChangeCostsClassChild() {
                var hidCostsClass = $("input[id$='Hid_CostsClass']");
                var goodsCode = $("td[lang='GoodsCode']");
                var img = $("td[lang='Img']");
                var txtGoodsCode = $("input[id$='txt_GoodsCode']");
                var uploadImgName = $("input[id$='UploadImgName']");
                var txtReportCost = $("input[id$='txt_ReportCost']");

                if (hidCostsClass.val() === "0") {
                    goodsCode.hide();
                    img.show();
                    txtGoodsCode.removeClass("Check");
                    uploadImgName.addClass("Check");
                    uploadImgName.attr('lang', '广告使用图片');
                }
                else if (hidCostsClass.val() === "1") {
                    goodsCode.show();
                    img.hide();
                    if (parseFloat(txtReportCost.val()) >= 500) {
                        txtGoodsCode.addClass("Check");
                        txtGoodsCode.attr('lang', '人事部物品管理编码');
                    }
                    uploadImgName.removeClass("Check");
                }
                else {
                    goodsCode.hide();
                    img.hide();
                    txtGoodsCode.removeClass("Check");
                    uploadImgName.removeClass("Check");
                }
            }

            //清除费用种类子元素相关数据
            function ClearCostsClassChildData() {
                var hidCostsClass = $("input[id$='Hid_CostsClass']");
                if (hidCostsClass.val() !== "1") {
                    $("input[id$='txt_GoodsCode']").val("");
                }

                if ($("input[id$='UploadImgName']").val().indexOf('\\') > 0) {
                    $("input[id$='UploadImgName']").val("");
                    $("td[lang='Img'] a").attr("href", "javascript:void(0);");
                }
            }

            //显示差旅费
            function ShowTravel() {
                $("#TraveA").show();
                if ($("input[id$='Hid_Travel']").val() === "1" || $("input[id$='Hid_Travel']").val() === "2") {
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
                var clearNum = $("td[class='ClearNum']");

                var bankNameChildren = $("td[lang='BankName']").children().not("span[class='error']");
                var clearNumChildren = $("td[lang='ClearNum']").children().not("span[class='error']");

                if (selectedCostType === "1") {//结算方式为“现金”
                    bankName.css("display", "none");
                    clearNum.css("display", "none");
                    bankNameChildren.removeClass("Check");
                    clearNumChildren.removeClass("Check");
                } else {//结算方式为“转账”
                    bankName.css("display", "");
                    clearNum.css("display", "");
                    bankNameChildren.addClass("Check");
                    clearNumChildren.addClass("Check");
                }
            }

            //清除结算方式子元素相关数据
            function ClearCostTypeChildData() {
                //清除银行名称数据 
                $("input[id='txt_BankName']").val("");
                $("input[id='txt_SubBankName']").val("");

                //清除收款账号数据
                $("input[id='txt_BankAccount']").val("");
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

            //设置含税金额  含税金额=未税金额+税额
            function SetTaxAmount(id, type) {
                var noTaxAmount = 0, tax = 0;
                if (type === "txt_NoTaxAmount") {
                    noTaxAmount = document.getElementById(id).value;
                    tax = document.getElementById(id.replace("txt_NoTaxAmount", "txt_Tax")).value;
                    document.getElementById(id.replace("txt_NoTaxAmount", "txt_TaxAmount")).value = ((noTaxAmount.length === 0 ? 0 : parseFloat(noTaxAmount)) + (tax.length === 0 ? 0 : parseFloat(tax))).toFixed(2);
                } else if (type === "txt_Tax") {
                    noTaxAmount = document.getElementById(id.replace("txt_Tax", "txt_NoTaxAmount")).value;
                    tax = document.getElementById(id).value;
                    document.getElementById(id.replace("txt_Tax", "txt_TaxAmount")).value = ((noTaxAmount.length === 0 ? 0 : parseFloat(noTaxAmount)) + (tax.length === 0 ? 0 : parseFloat(tax))).toFixed(2);
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

                //验证申报说明
                var resultReportMemo = CheckReportMemo();
                if (resultReportMemo.length !== 0) {
                    errorMsg += resultReportMemo;
                }

                var resultBill = CheckRepeaterData('', '', 'CheckBill');//验证票据信息
                var resultTermini = CheckRepeaterData('', '', 'CheckTermini'); //验证起讫数据

                if ($("span[class='error']").length === 0) {
                    if (!resultBill || !resultTermini) {
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

            //验证Repeater票据、起讫数据
            function CheckRepeaterData(id, type, checklang) {
                var result = true;
                $("input" + ((id.length === 0 && type.length === 0) ? "" : "[id^='" + id.replace(type, "") + "']") + "[lang='" + checklang + "']").each(function () {
                    var obj = $(this);

                    if (checklang === "CheckBill") {
                        var itemId = obj.attr("id");
                        if (itemId.indexOf("txt_NoTaxAmount") > -1 || itemId.indexOf("txt_Tax") > -1 || itemId.indexOf("txt_TaxAmount") > -1) {
                            check(this, 'Decimal');
                        }
                        if (itemId.indexOf("txt_BillDate") > -1) {
                            check(this, 'DateTime');
                        }
                    }

                    if (obj.val().length === 0) {
                        obj.css("border-bottom", "1px solid red");
                        result = false;
                    } else {
                        obj.css("border-bottom", "0px solid #fff");
                    }
                });

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


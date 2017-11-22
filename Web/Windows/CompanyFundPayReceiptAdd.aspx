<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyFundPayReceiptAdd.aspx.cs" Inherits="ERP.UI.Web.Windows.CompanyFundPayReceiptAdd" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script type="text/javascript" src="../JavaScript/jquery.js"></script>
            <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
            <script type="text/javascript" src="../JavaScript/common.js"></script>
            <script type="text/javascript" src="../JavaScript/telerik.js"></script>
            <script type="text/javascript">
                function onSubmit() {
                    var balance = parseFloat($("#RTB_SettleBalance").val());
                    if (balance < 0) {
                        if (confirm("我们还欠此单位￥：" + -balance + " 元，是否还继续提交付款单？")) {
                            return true;
                        } else {
                            return false;
                        }
                    }
                    return true;
                }

                function numberRecovery() {
                    alert(0);
                }

                //验证文件格式 zal 2016-02-18
                function CheckFile() {
                    var filePath = document.getElementById("UploadExcel").value;
                    document.getElementById("UploadExcelName").value = filePath;
                    var ext = filePath.substr(filePath.length - 4, 4).toLowerCase();
                    if (ext != ".xls") {
                        alert("请选择格式为“.xls”文件！");
                        document.getElementById("UploadExcelName").value = "";
                    }
                }
                //设置含税金额  含税金额=未税金额+税额
                function SetTaxAmount(noTaxAmountId, taxId) {
                    var noTaxAmount = 0, tax = 0;
                    if (noTaxAmountId.length !== 0) {
                        noTaxAmount = document.getElementById(noTaxAmountId).value;
                        tax = document.getElementById(noTaxAmountId.replace("txt_NoTaxAmount", "txt_Tax")).value;
                        document.getElementById(noTaxAmountId.replace("txt_NoTaxAmount", "txt_TaxAmount")).value = ((noTaxAmount.length === 0 ? 0 : parseFloat(noTaxAmount)) + (tax.length === 0 ? 0 : parseFloat(tax))).toFixed(2);
                    } else if (taxId.length !== 0) {
                        noTaxAmount = document.getElementById(taxId.replace("txt_Tax", "txt_NoTaxAmount")).value;
                        tax = document.getElementById(taxId).value;
                        document.getElementById(taxId.replace("txt_Tax", "txt_TaxAmount")).value = ((noTaxAmount.length === 0 ? 0 : parseFloat(noTaxAmount)) + (tax.length === 0 ? 0 : parseFloat(tax))).toFixed(2);
                    }
                }

                //取消验证  
                function CancleVerification(id) {
                    var rfvBillingDate = id.replace("btn_Cancel", "RFV_BillingDate");
                    var rfvInvoiceNo = id.replace("btn_Cancel", "RFV_InvoiceNo");
                    var rfvInvoiceCode = id.replace("btn_Cancel", "RFV_InvoiceCode");
                    var rfvNoTaxAmount = id.replace("btn_Cancel", "RFV_NoTaxAmount");
                    var rfvTax = id.replace("btn_Cancel", "RFV_Tax");
                    var rfvTaxAmount = id.replace("btn_Cancel", "RFV_TaxAmount");

                    rfvBillingDate.remove();
                    rfvInvoiceNo.remove();
                    rfvInvoiceCode.remove();
                    rfvNoTaxAmount.remove();
                    rfvTax.remove();
                    rfvTaxAmount.remove();
                }
            </script>
        </rad:RadScriptBlock>
        <table width="100%">
            <tr>
                <td style="text-align: right;">付款单方式：</td>
                <td colspan="4">
                    <asp:RadioButton ID="RB_Date" runat="server" GroupName="PayType" Text="按日期付款" AutoPostBack="true" OnCheckedChanged="RbDateCheckedChanged" />
                    <asp:RadioButton ID="RB_Invoice" runat="server" GroupName="PayType" Text="按入库单付款" AutoPostBack="true" OnCheckedChanged="RbInvoiceCheckedChanged" />
                    <asp:RadioButton ID="RB_PurchaseOrder" runat="server" GroupName="PayType" Text="按采购单付款" AutoPostBack="true" OnCheckedChanged="RbPurchaseOrderCheckedChanged" />
                    <asp:RadioButton ID="RbAdvance" runat="server" GroupName="PayType" Text="预付款" AutoPostBack="true" OnCheckedChanged="RbAdvanceCheckedChanged" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">往来单位：</td>
                <td colspan="2">
                    <rad:RadComboBox ID="RCB_CompanyList" runat="server" AutoPostBack="true" DropDownWidth="180px" AllowCustomText="true" EnableLoadOnDemand="true"
                        Height="200" EmptyMessage="选择往来单位" CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="RcbCompanyListSelectedIndexChanged">
                    </rad:RadComboBox>
                    <asp:RequiredFieldValidator ID="RFV_CompanyList" EnableClientScript="true" ControlToValidate="RCB_CompanyList" runat="server" ErrorMessage="*" Text="*" />
                </td>
                <td style="text-align: right;">付款公司：</td>
                <td>
                    <rad:RadComboBox ID="RCB_FilialeList" runat="server" AutoPostBack="true" DropDownWidth="180px" EnableLoadOnDemand="true"
                        Height="200" EmptyMessage="选择付款公司" CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="Rcb_FilialeListSelectedIndexChanged">
                    </rad:RadComboBox>
                    <asp:RequiredFieldValidator ID="RTF_FilialeList" EnableClientScript="true" ControlToValidate="RCB_FilialeList" runat="server" ErrorMessage="*" Text="*" />
                </td>
            </tr>
            <tr>
                <div id="DIV_Date" runat="server">
                    <td style="text-align: right;">
                        <asp:Label ID="LB_Date" runat="server" Text="结账日期："></asp:Label></td>
                    <td>
                        <rad:RadDatePicker ID="RDP_StartDate" SkinID="Common" Width="115px" DateInput-ReadOnly="true" AutoPostBack="true" runat="server" OnSelectedDateChanged="RdpStartDateOnSelectedDateChanged"></rad:RadDatePicker>
                    </td>
                    <td>
                        <rad:RadDatePicker ID="RDP_EndDate" SkinID="Common" Width="115px" Enabled="false" AutoPostBack="true" DateInput-ReadOnly="true" OnSelectedDateChanged="RdpEndDateOnSelectedDateChanged" runat="server"></rad:RadDatePicker>
                    </td>
                </div>
                <div id="DIV_Orders" runat="server">
                    <td style="text-align: right;">入库单号：</td>
                    <td colspan="2">
                        <rad:RadTextBox ID="TB_Orders" runat="server" EmptyMessage="填写入库单号，多个入库单用英文逗号分隔" AutoPostBack="true" Width="99%" OnTextChanged="TbOrdersTextChanged"></rad:RadTextBox>
                        <br />
                    </td>
                </div>
                <div id="DIV_Goods" runat="server">
                    <td style="text-align: right;">采购单号：</td>
                    <td colspan="2">
                        <rad:RadTextBox ID="RTB_PurchaseOrderNo" EmptyMessage="填写采购单号(单个)" runat="server" AutoPostBack="true" Width="180" OnTextChanged="RtbPurchaseOrderNoTextChanged"></rad:RadTextBox>
                    </td>
                </div>
                <td style="text-align: right;">付款期：
                </td>
                <td>
                    <asp:TextBox ID="txt_PaymentDate" runat="server" Width="100px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM',minDate:'%y-%M'})"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">紧急程度：</td>
                <td colspan="4" style="text-align:left;">
                    <table>
                        <tr>
                            <td>
                                <asp:RadioButton ID="RbNormal" runat="server" GroupName="Urgent" Text="普通" AutoPostBack="true" OnCheckedChanged="RbNormalCheckedChanged" />
                            </td>
                            <td>
                                <asp:RadioButton ID="RbUrgent" runat="server" GroupName="Urgent" Text="加急" AutoPostBack="true" OnCheckedChanged="RbNormalCheckedChanged" />
                            </td>
                            <td style="padding-top:7px;padding-left:20px;">
                                <asp:Label ID="LbUrgent" runat="server" style="color:red;">请填写加急原因</asp:Label>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;"><asp:Label runat="server" ID="LbUrgentTitle" Text="加急原因："></asp:Label></td>
                <td colspan="4">
                    <rad:RadTextBox Width="500" Height="30" TextMode="MultiLine" ID="RtbUrgent" runat="server"></rad:RadTextBox>
                </td >
                </tr>
            <div id="DIV_Related" runat="server">
                <tr>
                    <td style="text-align: right;">退货单：
                    </td>
                    <td colspan="2">
                        <rad:RadTextBox ID="RTB_ReturnOrder" EmptyMessage="退货单号,多个单据按英文逗号隔开" runat="server" AutoPostBack="true" Width="99%" OnTextChanged="RTB_ReturnOrder_TextChanged"></rad:RadTextBox>
                        <b style="color: red;">
                            <asp:Literal ID="Lit_ReturnOrderMoney" runat="server" Text="0"></asp:Literal></b>
                        <b>
                            <asp:Literal ID="Lit_ReturnOrder" runat="server"></asp:Literal></b>
                    </td>
                    <td style="text-align: right;">预付款单：
                    </td>
                    <td>
                        <rad:RadTextBox ID="RTB_PayOrder" EmptyMessage="预付款单号,多个单据按英文逗号隔开" runat="server" AutoPostBack="true" Width="99%" OnTextChanged="RTB_PayOrder_TextChanged"></rad:RadTextBox>
                        <b style="color: red;">
                            <asp:Literal ID="Lit_PayOrderMoney" runat="server" Text="0"></asp:Literal></b>
                        <b>
                            <asp:Literal ID="Lit_PayOrder" runat="server"></asp:Literal></b>
                    </td>
                </tr>
            </div>
            <div id="DivStockNos" runat="server">
                <tr>
                    <td style="text-align: right;">
                        <asp:Label ID="LbInclude" runat="server" Text="包含单据：" /></td>
                    <td colspan="2">
                        <rad:RadTextBox ID="RtbInclude" EmptyMessage="入库单号,多个单据按英文逗号隔开,日期往来账之外" Width="300px" runat="server"
                            AutoPostBack="true" OnTextChanged="RtbIncludeTextChanged">
                        </rad:RadTextBox>
                    </td>
                    <td style="text-align: right;">
                        <asp:Label ID="LbRemove" runat="server" Text="排除单据："></asp:Label></td>
                    <td>
                        <rad:RadTextBox ID="RtbRemove" EmptyMessage="入库单号,多个单据按英文逗号隔开,日期往来账之内" Width="300px" runat="server"
                            AutoPostBack="true" OnTextChanged="RtbRemoveTextChanged">
                        </rad:RadTextBox>
                    </td>
                </tr>
            </div>
            <tr>
                <div id="DIV_BackBalance" runat="server" visible="False">
                    <td style="text-align: right;">
                        <asp:Label ID="LB_BankAmount" runat="server"></asp:Label></td>
                    <td colspan="2">
                        <rad:RadTextBox ID="RTB_SettleBalance" ReadOnly="true" runat="server"></rad:RadTextBox></td>
                </div>
                <td style="text-align: right;">应付金额：</td>
                <td>
                    <rad:RadTextBox ID="RTB_RealityBalance" runat="server" AutoPostBack="true" OnTextChanged="RtbRealityBalanceTextChanged"></rad:RadTextBox>
                    <asp:RequiredFieldValidator ID="RFV_RealityBalance" EnableClientScript="true" ControlToValidate="RTB_RealityBalance" runat="server" ErrorMessage="*" Text="*" />
                    <asp:RegularExpressionValidator ID="REVRealityBalance" runat="server" ControlToValidate="RTB_RealityBalance" ErrorMessage="金额必须为数字！"
                        ValidationExpression="\d+(,\d{3})*(\.\d*){0,1}"></asp:RegularExpressionValidator>
                    <span>
                        <asp:Literal ID="LB_UpperCaseMoney" runat="server"></asp:Literal></span>
                </td>
            </tr>
            <div id="DIV_DiscountMoney" runat="server">
                <tr>
                    <td style="text-align: right;">今年折扣：</td>
                    <td colspan="2">
                        <rad:RadTextBox ID="RTB_DiscountMoney" Text="0" runat="server" AutoPostBack="true" OnTextChanged="RtbDiscountMoneyTextChanged"></rad:RadTextBox>
                        <asp:RegularExpressionValidator ID="REVDiscountMoney" runat="server" ControlToValidate="RTB_DiscountMoney" ErrorMessage="金额必须为数字！" ValidationExpression="\d*(\.\d*){0,1}"></asp:RegularExpressionValidator>
                    </td>
                    <td style="text-align: right;">去年返利：
                    </td>
                    <td>
                        <rad:RadTextBox ID="RtbLastRebate" Text="0" runat="server" AutoPostBack="true" OnTextChanged="RtbDiscountMoneyTextChanged"></rad:RadTextBox>
                        <asp:RegularExpressionValidator ID="RevdLastRebate" runat="server"
                            ControlToValidate="RtbLastRebate" ErrorMessage="金额必须为数字！" ValidationExpression="\d*(\.\d*){0,1}"></asp:RegularExpressionValidator>
                    </td>
                </tr>
            </div>
            <div id="DIV_DiscountCaption" runat="server" visible="false">
                <tr>
                    <td style="text-align: right;">折扣说明：</td>
                    <td colspan="4">
                        <rad:RadTextBox Width="380" Height="40" TextMode="MultiLine" ID="RTB_DiscountCaption" runat="server"></rad:RadTextBox>
                    </td>
                </tr>
            </div>
            <tr>
                <td style="text-align: right;">备注说明：</td>
                <td colspan="4">
                    <rad:RadTextBox Height="60" Width="99%" TextMode="MultiLine" ID="RTB_OtherDiscountCaption" runat="server"></rad:RadTextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">往来单位收款人：</td>
                <td colspan="4">
                    <rad:RadTextBox ID="TB_Payee" ReadOnly="true" runat="server" Width="180px"></rad:RadTextBox></td>
            </tr>
            <tr>
                <td style="text-align: right;">往来单位开户银行：</td>
                <td colspan="2">
                    <rad:RadTextBox ID="RTB_CompBank" ReadOnly="true" runat="server" Width="150px"></rad:RadTextBox></td>
                <td style="text-align: right;">往来单位银行账号：</td>
                <td>
                    <rad:RadTextBox ID="RTB_CompBankAccount" ReadOnly="true" runat="server" Width="150px"></rad:RadTextBox></td>
            </tr>
            <div id="div_Invoice" runat="server">
                <tr style="background-color: #D7D7D7;">
                    <td style="text-align: right;"><b style="color: blue;">票据类型</b>：
                    </td>
                    <td colspan="4">
                        <asp:RadioButtonList ID="rbl_InvoiceType" runat="server" RepeatDirection="Horizontal" OnSelectedIndexChanged="rbl_InvoiceType_SelectedIndexChanged" AutoPostBack="True">
                            <asp:ListItem Text="普通发票" Value="1" Selected="True"></asp:ListItem>
                            <asp:ListItem Text="增值税专用发票" Value="5"></asp:ListItem>
                        </asp:RadioButtonList>
                    </td>
                </tr>
                <tr style="background-color: #D7D7D7;">
                    <td style="text-align: right;">发票信息
                    </td>
                    <td colspan="4">
                        <asp:TextBox ID="UploadExcelName" runat="server" onfocus="this.blur();"></asp:TextBox>
                        <asp:FileUpload ID="UploadExcel" runat="server" Style="display: none;" onchange="CheckFile()" />
                        <input type="button" value="选择文件" title="文件格式(.xls)!" onclick="UploadExcel.click()" />
                        <asp:Button ID="btn_Upload" runat="server" Text="导入发票信息" OnClick="btn_Upload_Click" />
                         <a id="Template" runat="server" href="../App_Themes/费用申报票据模板(普通发票).xls" style="color: red; font-weight: bold;">发票模板导出</a>
                    </td>
                </tr>
                <tr style="background-color: #D7D7D7;">
                    <td style="text-align: right;"><b style="color: blue;">开票单位</b>：
                    </td>
                    <td colspan="4">
                        <asp:TextBox ID="txt_BillingUnit" runat="server" Width="345px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="5">
                        <rad:RadGrid ID="RG_InvoiceList" runat="server" ShowFooter="true" OnNeedDataSource="RG_InvoiceList_NeedDataSource" OnItemDataBound="RG_InvoiceList_ItemDataBound">
                            <MasterTableView DataKeyNames="InvoiceId" EditMode="InPlace">
                                <CommandItemTemplate>
                                    <asp:LinkButton ID="LB_AddRecord" runat="server" CommandName="InitInsert" Visible='<%# IsVisible %>'>
                                    <asp:Image runat="server" SkinID="InsertImageButton" />添加发票
                                    </asp:LinkButton>
                                </CommandItemTemplate>
                                <CommandItemStyle Height="25px" />
                                <Columns>
                                    <rad:GridTemplateColumn HeaderText="开票日期">
                                        <ItemTemplate>
                                            <rad:RadDatePicker ID="RDP_BillingDate" SkinID="Common" Width="100px" SelectedDate='<%#Eval("BillingDate") ==DBNull.Value ? DateTime.Now : Convert.ToDateTime(Eval("BillingDate")) %>' DateInput-ReadOnly="true" runat="server"></rad:RadDatePicker>
                                            <asp:RequiredFieldValidator ID="RFV_BillingDate" EnableClientScript="true" ControlToValidate="RDP_BillingDate" runat="server" ErrorMessage="*" Text="*" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle VerticalAlign="Top" Width="65px"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="发票号码">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txt_InvoiceNo" runat="server" Width="100px" Text='<%# Eval("InvoiceNo") %>'></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RFV_InvoiceNo" EnableClientScript="true" ControlToValidate="txt_InvoiceNo" runat="server" ErrorMessage="*" Text="*" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle VerticalAlign="Top" Width="100px"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="发票代码" UniqueName="InvoiceCode">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txt_InvoiceCode" runat="server" Width="100px" Text='<%# Eval("InvoiceCode") %>'></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RFV_InvoiceCode" EnableClientScript="true" ControlToValidate="txt_InvoiceCode" runat="server" ErrorMessage="*" Text="*" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle VerticalAlign="Top" Width="100px"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="未税金额" UniqueName="NoTaxAmount">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txt_NoTaxAmount" runat="server" Text='<%#Eval("NoTaxAmount") %>' Width="100px" onchange="SetTaxAmount(this.id,'');"></asp:TextBox>
                                            <%--<asp:RequiredFieldValidator ID="RFV_NoTaxAmount" EnableClientScript="true" ControlToValidate="txt_NoTaxAmount" runat="server" ErrorMessage="*" Text="*" />
                                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txt_NoTaxAmount" ErrorMessage="金额必须为数字！"
                                                ValidationExpression="-?\d+(,\d{3})*(\.\d*){0,1}"></asp:RegularExpressionValidator>--%>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle VerticalAlign="Top" HorizontalAlign="Center" Width="50px"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="税额" UniqueName="Tax">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txt_Tax" runat="server" Text='<%# Eval("Tax")%>' Width="100px" onchange="SetTaxAmount('',this.id);"></asp:TextBox>
                                            <%--<asp:RequiredFieldValidator ID="RFV_Tax" EnableClientScript="true" ControlToValidate="txt_Tax" runat="server" ErrorMessage="*" Text="*" />
                                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txt_Tax" ErrorMessage="金额必须为数字！"
                                                ValidationExpression="-?\d+(,\d{3})*(\.\d*){0,1}"></asp:RegularExpressionValidator>--%>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle VerticalAlign="Top" HorizontalAlign="Center" Width="50px"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="含税金额" UniqueName="TaxAmount">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txt_TaxAmount" runat="server" Text='<%# Eval("TaxAmount")%>'  Width="100px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RFV_TaxAmount" EnableClientScript="true" ControlToValidate="txt_TaxAmount" runat="server" ErrorMessage="*" Text="*" />
                                            <asp:RegularExpressionValidator runat="server" ControlToValidate="txt_TaxAmount" ErrorMessage="金额必须为数字！"
                                                ValidationExpression="-?\d+(,\d{3})*(\.\d*){0,1}"></asp:RegularExpressionValidator>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle VerticalAlign="Top" HorizontalAlign="Center" Width="50px"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="备注">
                                        <ItemTemplate>
                                            <asp:TextBox ID="txt_Memo" runat="server" TextMode="MultiLine" Text='<%# Eval("Memo") %>'></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle VerticalAlign="Top"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="操作">
                                        <ItemTemplate>
                                            <asp:Button ID="btn_Add" runat="server" Text="保存" OnClick="btn_Add_OnClick" />
                                            <asp:Button ID="btn_Cancel" runat="server" Text="取消" OnClick="btn_Cancel_OnClick" OnClientClick="CancleVerification(this.id)" />
                                            <asp:Button ID="btn_Edit" runat="server" Text="修改" OnClick="btn_Edit_OnClick" Visible="False" />
                                            <asp:Button ID="btn_Del" runat="server" Text="删除" OnClick="btn_Del_OnClick" Visible="False" />
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle VerticalAlign="Top"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </rad:RadGrid>
                    </td>
                </tr>
            </div>
            <tr>
                <td colspan="5" style="text-align: center;">
                    <table width="100%">
                        <tr>
                            <td style="text-align: right;">
                                <Ibt:ImageButtonControl ID="LB_Inster" OnClick="LbInsterOncLick" OnClientClick="onSubmit();" runat="server" SkinType="Insert" Text="提交"></Ibt:ImageButtonControl>
                                <Ibt:ImageButtonControl ID="LB_Save" OnClick="LbSaveOncLick" runat="server" SkinType="Insert" Text="提交"></Ibt:ImageButtonControl>
                            </td>
                            <td style="text-align: left;">
                                <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="CancelWindow();return false;" SkinType="Cancel" Text="取消"></Ibt:ImageButtonControl>
                            </td>
                        </tr>
                    </table>
                    <asp:Label ID="Lab_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                </td>
            </tr>
        </table>
        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RSM">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RAM" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RTB_RealityBalance">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="LB_UpperCaseMoney" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_RealityBalance" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RDP_EndDate">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RTB_SettleBalance" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RDP_StartDate">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RTB_SettleBalance" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RDP_EndDate">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RtbRemove" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RDP_StartDate">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RtbRemove" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RTB_DiscountCaption">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="DIV_DiscountCaption" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RtbInclude">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RtbInclude" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RtbInclude">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RTB_SettleBalance" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RtbRemove">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RTB_SettleBalance" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RbNormal">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RbUrgent"/>
                        <rad:AjaxUpdatedControl ControlID="LbUrgentTitle"/>
                        <rad:AjaxUpdatedControl ControlID="RtbUrgent"/>
                        <rad:AjaxUpdatedControl ControlID="LbUrgent"/>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RbUrgent">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RbNormal"/>
                        <rad:AjaxUpdatedControl ControlID="LbUrgentTitle"/>
                        <rad:AjaxUpdatedControl ControlID="RtbUrgent"/>
                        <rad:AjaxUpdatedControl ControlID="LbUrgent"/>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <%--<rad:AjaxSetting AjaxControlID="TB_Orders">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_CompanyList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RCB_FilialeList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_SettleBalance" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="TB_Payee" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBankAccount" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_OwnBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_OwnBankAccount" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="Lable_Money" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RCB_BankAccount" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>--%>
                <%--<rad:AjaxSetting AjaxControlID="RTB_PurchaseOrderNo">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RTB_PurchaseOrderNo" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RCB_CompanyList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RCB_FilialeList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="TB_Payee" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBankAccount" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_OwnBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_OwnBankAccount" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="Lable_Money" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RCB_BankAccount" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>--%>
                <rad:AjaxSetting AjaxControlID="RCB_CompanyList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RDP_StartDate" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RDP_EndDate" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="TB_Payee" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBankAccount" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="Lable_Money" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RCB_FilialeList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_FilialeList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_BankAccount" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="Lable_Money" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_SettleBalance" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RDP_StartDate" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RCB_FilialeList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_SettleBalance" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="TB_Payee" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBankAccount" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="LB_Inster">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="LB_Inster" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="LB_Save">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="LB_Save" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

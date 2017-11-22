<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckReceiptForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.CheckReceiptForm" %>

<html>
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/common.js" type="text/javascript"></script>
            <script type="text/javascript" language="javascript">
                function CheckFinsh() {
                    var receiptNo = document.getElementById('<%= HF_ReceiptNo.ClientID %>').value;
                    var typeName = document.getElementById('<%= HF_TypeName.ClientID %>').value;
                    if (confirm("确认单据" + receiptNo + "已" + typeName)) {
                        document.getElementById("<%=btnFinish2.ClientID %>").click();
                    }
                    return false;
                }
            </script>
        </rad:RadScriptBlock>
        <asp:HiddenField ID="HF_ReceiptNo" runat="server" />
        <asp:HiddenField ID="HF_TypeName" runat="server" />
        <div style="margin: 10px;">
            <table style="width: 100%;">
                <div id="DIV_Type" runat="server">
                    <tr>
                        <td style="text-align: right;">付款方式：
                        </td>
                        <td colspan="4">
                            <asp:RadioButton ID="RB_Date" runat="server" GroupName="PayType" Text="按日期付款" AutoPostBack="true"
                                Enabled="false" />
                            <asp:RadioButton ID="RB_Invoice" runat="server" GroupName="PayType" Text="按入库单付款"
                                AutoPostBack="true" Enabled="false" />
                            <asp:RadioButton ID="RB_PurchaseOrder" runat="server" GroupName="PayType" Text="按采购单付款"
                                AutoPostBack="true" Enabled="false" />
                            <asp:RadioButton ID="RbAdvance" runat="server" GroupName="PayType" Text="预付款" AutoPostBack="true"
                                Enabled="false" />
                        </td>
                    </tr>
                </div>
                <tr>
                    <td style="text-align: right;">往来单位：
                    </td>
                    <td colspan="2">
                        <rad:RadComboBox ID="RCB_CompanyList" runat="server" AutoPostBack="true" DropDownWidth="180px"
                            AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择公司"
                            CausesValidation="false" Enabled="false">
                        </rad:RadComboBox>
                    </td>
                    <td style="text-align: right;">付款公司：
                    </td>
                    <td>
                        <rad:RadComboBox ID="RCB_FilialeList" runat="server" AutoPostBack="true" DropDownWidth="180px"
                            AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择公司"
                            CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="Rcb_FilialeListSelectedIndexChanged">
                        </rad:RadComboBox>
                        <asp:RequiredFieldValidator ID="RTF_FilialeList" EnableClientScript="true" ControlToValidate="RCB_FilialeList"
                            runat="server" ErrorMessage="*" Text="*" />
                    </td>
                </tr>
                <tr>
                    <div id="DIV_Date" runat="server">
                        <td style="text-align: right;">结账日期：</td>
                        <td>
                            <rad:RadDatePicker ID="RDP_StartDate" Enabled="false" DateInput-ReadOnly="true" runat="server">
                            </rad:RadDatePicker>
                        </td>
                        <td>
                            <rad:RadDatePicker ID="RDP_EndDate" Enabled="false" DateInput-ReadOnly="true"
                                runat="server">
                            </rad:RadDatePicker>
                        </td>
                    </div>
                    <div id="DIV_Orders" runat="server">
                        <td style="text-align: right;">入库单号：
                        </td>
                        <td colspan="2">
                            <rad:RadTextBox ID="TB_Orders" runat="server" EmptyMessage="填写入库单号,多单号以逗号分隔" AutoPostBack="true"
                                Width="300" ReadOnly="true">
                            </rad:RadTextBox>
                        </td>
                    </div>
                    <div id="DIV_Goods" runat="server">
                        <td style="text-align: right;">采购单号：
                        </td>
                        <td colspan="2">
                            <rad:RadTextBox ID="RTB_PurchaseOrderNo" EmptyMessage="填写采购单号" runat="server" AutoPostBack="true"
                                ReadOnly="true">
                            </rad:RadTextBox>
                        </td>
                    </div>
                    <div id="div_PaymentDate" runat="server">
                        <td style="text-align: right;">付款期：
                        </td>
                        <td>
                            <asp:TextBox ID="txt_PaymentDate" runat="server" Width="70px" ReadOnly="True"></asp:TextBox>
                        </td>
                    </div>
                </tr>
                <tr>
                <td style="text-align: right;"><asp:Label runat="server" ID="LbUrgentLevel" Text="紧急程度："></asp:Label></td>
                <td colspan="4" style="text-align:left;">
                    <table>
                        <tr>
                            <td>
                                <asp:RadioButton ID="RbNormal" runat="server" GroupName="Urgent" Text="普通" AutoPostBack="true" Enabled="False"/>
                            </td>
                            <td>
                                <asp:RadioButton ID="RbUrgent" runat="server" GroupName="Urgent" Text="加急" AutoPostBack="true" Enabled="False"/>
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
                    <rad:RadTextBox Width="500" Height="30" TextMode="MultiLine" ID="RtbUrgent" runat="server" Enabled="False"></rad:RadTextBox>
                </td >
                </tr>
                <div id="DIV_Related" runat="server">
                    <tr>
                        <td style="text-align: right;">退货单：
                        </td>
                        <td colspan="2">
                            <rad:RadTextBox ID="RTB_ReturnOrder" EmptyMessage="退货单号,多个单据按英文逗号隔开" ReadOnly="True" runat="server" Width="99%"></rad:RadTextBox>
                            <b style="color: red;">
                                <asp:Literal ID="Lit_ReturnOrderMoney" runat="server" Text="0"></asp:Literal></b>
                            <b>
                                <asp:Literal ID="Lit_ReturnOrder" runat="server"></asp:Literal></b>
                        </td>
                        <td style="text-align: right;">付款单：
                        </td>
                        <td>
                            <rad:RadTextBox ID="RTB_PayOrder" EmptyMessage="付款单号,多个单据按英文逗号隔开" ReadOnly="True" runat="server" Width="99%"></rad:RadTextBox>
                            <b style="color: red;">
                                <asp:Literal ID="Lit_PayOrderMoney" runat="server" Text="0"></asp:Literal></b>
                            <b>
                                <asp:Literal ID="Lit_PayOrder" runat="server"></asp:Literal></b>
                        </td>
                    </tr>
                </div>
                <div id="DivStockNos" runat="server">
                    <tr>
                        <td style="text-align: right;">包含单据：</td>
                        <td colspan="2">
                            <rad:RadTextBox ID="RtbIncludeNos" runat="server" ReadOnly="true" EmptyMessage="入库单号,多个单据按英文逗号隔开,日期往来账之外" Width="300px"></rad:RadTextBox>
                        </td>
                        <td style="text-align: right;">排除单据：</td>
                        <td>
                            <rad:RadTextBox ID="RtbRemoveNos" runat="server" ReadOnly="true" EmptyMessage="入库单号,多个单据按英文逗号隔开,日期往来账之内" Width="300px"></rad:RadTextBox>
                        </td>
                    </tr>
                </div>
                <tr>
                    <div id="DIV_BackBalance" runat="server">
                        <td style="text-align: right;">单据总额：
                        </td>
                        <td colspan="2">
                            <rad:RadTextBox ID="RTB_SettleBalance" ReadOnly="true" runat="server" Enabled="false">
                            </rad:RadTextBox>
                        </td>
                    </div>
                    <td style="text-align: right;">
                        <asp:Label ID="LB_RealityBalance" runat="server"></asp:Label>
                    </td>
                    <td>
                        <rad:RadTextBox ID="RTB_RealityBalance" runat="server" AutoPostBack="true" ReadOnly="true">
                        </rad:RadTextBox>
                        <span>
                            <asp:Literal ID="LB_UpperCaseMoney" runat="server"></asp:Literal></span>
                    </td>
                </tr>
                <div id="DIV_DiscountMoney" runat="server">
                    <tr>
                        <td style="text-align: right;">
                            <asp:Label ID="LB_DiscountMoney" runat="server"></asp:Label>
                        </td>
                        <td colspan="2">
                            <rad:RadTextBox ID="RTB_DiscountMoney" Text="0" runat="server" AutoPostBack="true"
                                ReadOnly="true" Enabled="false">
                            </rad:RadTextBox>
                        </td>
                        <td style="text-align: right;">
                            <asp:Label ID="LbRebate" runat="server"></asp:Label>
                        </td>
                        <td colspan="2">
                            <rad:RadTextBox ID="RtbRebate" Text="0" runat="server" AutoPostBack="true"
                                ReadOnly="true" Enabled="false">
                            </rad:RadTextBox>
                        </td>
                    </tr>
                </div>
                <div id="DIV_ExpectBalance" runat="server">
                    <tr>
                        <td style="text-align: right;">对方余额：
                        </td>
                        <td colspan="4">
                            <rad:RadTextBox ID="RTB_ExpectBalance" runat="server" ReadOnly="true" Enabled="false">
                            </rad:RadTextBox>
                            <asp:RequiredFieldValidator ID="RFV_ExpectBalance" EnableClientScript="true" ControlToValidate="RTB_ExpectBalance"
                                runat="server" ErrorMessage="*" Text="*" />
                        </td>
                    </tr>
                </div>
                <div id="DIV_DiscountCaption" runat="server" visible="false">
                    <tr>
                        <td style="text-align: right;">折扣说明：
                        </td>
                        <td colspan="4">
                            <asp:Label runat="server" ID="Lb_DiscountCaption" Width="550"></asp:Label>
                        </td>
                    </tr>
                </div>
                <tr>
                    <td style="text-align: right;">备注说明：
                    </td>
                    <td colspan="4">
                        <asp:Label runat="server" ID="LB_OtherDiscountCaption" Width="550"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">
                        <asp:Literal ID="Lit_Payee" runat="server" Text="往来单位收款人"></asp:Literal>：
                    </td>
                    <td colspan="4">
                        <rad:RadTextBox ID="TB_Payee" ReadOnly="true" runat="server" Width="250">
                        </rad:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">往来单位开户银行：
                    </td>
                    <td colspan="2">
                        <rad:RadTextBox ID="RTB_CompBank" ReadOnly="true" runat="server" Width="210">
                        </rad:RadTextBox>
                    </td>
                    <td style="text-align: right;">往来单位银行账号：
                    </td>
                    <td>
                        <rad:RadTextBox ID="RTB_CompBankAccount" ReadOnly="true" runat="server" Width="210">
                        </rad:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <div id="DIV_Poundage" runat="server">
                        <td style="text-align: right;">手续费：
                        </td>
                        <td colspan="4">
                            <rad:RadTextBox ID="TB_Poundage" runat="server">
                            </rad:RadTextBox>
                            <asp:RegularExpressionValidator ID="REVCost" runat="server" ControlToValidate="TB_Poundage"
                                Text="*" ErrorMessage="金额必须为数字！" ValidationExpression="\d*(\.\d*){0,1}"></asp:RegularExpressionValidator>
                        </td>
                    </div>
                    </tr>
                <tr>
                    <div id="DivFlowNo" runat="server">
                        <td style="text-align: right;">交易流水号：
                        </td>
                        <td colspan="4">
                            <rad:RadTextBox ID="TbFlowNo" runat="server">
                            </rad:RadTextBox>
                        </td>
                    </div>
                </tr>
                <tr>
                    <td style="text-align: right;">公司银行账号：
                    </td>
                    <td colspan="2">
                        <rad:RadComboBox ID="RcbBankAccount" runat="server" AutoPostBack="true" DropDownWidth="260px"
                            Width="260px" Height="100" EmptyMessage="请选择银行账号" CausesValidation="false"
                            Filter="StartsWith">
                        </rad:RadComboBox>
                    </td>
                    <td style="text-align: right;">
                        <span id="span_titile" runat="server">其他银行账号：</span>
                    </td>
                    <td colspan="4">
                        <rad:RadComboBox ID="RcbElseBankAccount" runat="server" AutoPostBack="true" DropDownWidth="260px"
                            Width="260px" Height="100" EmptyMessage="请选择银行账号" CausesValidation="false"
                            Filter="StartsWith">
                        </rad:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <div id="Div1" runat="server">
                        <td style="text-align: right;">发票类型：
                        </td>
                        <td colspan="4">
                           <asp:Label runat="server" ID="InvoiceTypeName"></asp:Label>
                        </td>
                    </div>
                </tr>
                <div id="div_Invoice" runat="server">
                    <tr>
                        <td colspan="5">
                            <rad:RadGrid ID="RG_InvoiceList" runat="server" ShowFooter="true" OnNeedDataSource="RG_InvoiceList_NeedDataSource">
                                <MasterTableView>
                                    <CommandItemTemplate>
                                    </CommandItemTemplate>
                                    <CommandItemStyle Height="0px" />
                                    <Columns>
                                        <rad:GridTemplateColumn HeaderText="开票单位">
                                            <ItemTemplate>
                                                <%# Eval("BillingUnit") %>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        </rad:GridTemplateColumn>
                                        <rad:GridTemplateColumn HeaderText="开票日期">
                                            <ItemTemplate>
                                                <%# Convert.ToDateTime(Eval("BillingDate")).ToString("yyyy-MM-dd") %>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle Width="65px"></ItemStyle>
                                        </rad:GridTemplateColumn>
                                        <rad:GridTemplateColumn HeaderText="发票号码">
                                            <ItemTemplate>
                                                <%# Eval("InvoiceNo") %>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle Width="100px"></ItemStyle>
                                        </rad:GridTemplateColumn>
                                        <rad:GridTemplateColumn HeaderText="发票代码" UniqueName="InvoiceCode">
                                            <ItemTemplate>
                                                <%# Eval("InvoiceCode") %>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle Width="100px"></ItemStyle>
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
                                        <rad:GridTemplateColumn HeaderText="备注">
                                            <ItemTemplate>
                                                <%# Eval("Memo") %>
                                            </ItemTemplate>
                                        </rad:GridTemplateColumn>
                                    </Columns>
                                </MasterTableView>
                            </rad:RadGrid>
                        </td>
                    </tr>
                </div>
                <tr>
                    <td colspan="5" style="text-align: center;">
                        <div id="Div_ShowDo" visible="False" runat="server">
                            <span style="padding-left: 100px;">
                                <asp:Button ID="Btn_Do" runat="server" Text="确定执行" OnClick="Btn_Do_Click" />
                            </span>
                            <span style="padding-left: 50px;">
                                <asp:Button ID="BtnSave" runat="server" Text="保    存" OnClick="BtnSaveClick" />
                            </span>
                            <span style="padding-left: 50px;">
                                <asp:Button ID="Btn_GoBack" runat="server" Text="退    回" OnClick="Btn_GoBack_Click" />
                            </span>
                        </div>
                    </td>
                </tr>
                <div id="Div_ShowtAuditing" runat="server" visible="False">
                    <tr>
                        <td style="text-align: right;">不通过理由：
                        </td>
                        <td colspan="4" style="text-align: center;">
                            <rad:RadTextBox Height="60" Width="99%" TextMode="MultiLine" ID="RTB_BackReason"
                                runat="server">
                            </rad:RadTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="5" style="text-align: center;">
                            <asp:Button ID="btnFinish" runat="server" Width="100px" Text="完成" OnClientClick="return CheckFinsh();" />&nbsp;
                                    <asp:Button ID="btnFinish2" runat="server" Width="100px" Text="完成" OnClick="BtnFinishClick"
                                        Style="display: none;" />
                            <asp:Button ID="btnBack" runat="server" Width="100px" Text="退回" OnClick="BtnBackClick" />
                        </td>
                    </tr>
                </div>
            </table>
        </div>
        <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="Btn_Do">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Div_ShowDo" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="Btn_GoBack">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Div_ShowDo" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="btnFinish2">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Div_ShowtAuditing" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RcbBankAccount">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RcbElseBankAccount" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RcbElseBankAccount">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RcbBankAccount" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

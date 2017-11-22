<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckReceiptDetail.aspx.cs"
    Inherits="ERP.UI.Web.Windows.CheckReceiptDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html>
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <div style="margin: 10px;">
            <table width="100%">
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
                <tr>
                    <td style="text-align: right;">往来单位：
                    </td>
                    <td colspan="2">
                        <rad:RadComboBox ID="RCB_CompanyList" runat="server" AutoPostBack="true" DropDownWidth="180px"
                            AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择公司"
                            OnSelectedIndexChanged="RcbCompanyListSelectedIndexChanged" CausesValidation="false"
                            Enabled="false">
                        </rad:RadComboBox>
                    </td>
                    <td style="text-align: right;">付款公司：
                    </td>
                    <td>
                        <rad:RadComboBox ID="RCB_FilialeList" runat="server" AutoPostBack="true" DropDownWidth="180px"
                            AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择付款公司"
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
                        <td style="text-align: right;">入库单号：</td>
                        <td colspan="2">
                            <rad:RadTextBox ID="TB_Orders" runat="server" EmptyMessage="填写入库单号,多单号以逗号分隔" AutoPostBack="true"
                                Width="300" ReadOnly="true">
                            </rad:RadTextBox>
                        </td>
                    </div>
                    <div id="DIV_Goods" runat="server">
                        <td style="text-align: right;">采购单号：</td>
                        <td colspan="2">
                            <rad:RadTextBox ID="RTB_PurchaseOrderNo" EmptyMessage="填写采购单号" runat="server" ReadOnly="true">
                            </rad:RadTextBox>
                        </td>
                    </div>
                    <td style="text-align: right;">付款期：
                    </td>
                    <td>
                        <asp:TextBox ID="txt_PaymentDate" runat="server" Width="70px" ReadOnly="True"></asp:TextBox>
                    </td>
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
                    <div id="DIV_BackBalance" runat="server" visible="False">
                        <td style="text-align: right;">单据总额：</td>
                        <td colspan="2">
                            <rad:RadTextBox ID="RTB_SettleBalance" ReadOnly="true" runat="server">
                            </rad:RadTextBox>
                    </div>
                    <td style="text-align: right;">
                        <asp:Label ID="LB_RealityBalance" runat="server"></asp:Label></td>
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
                            <asp:Label ID="LB_DiscountMoney" runat="server"></asp:Label></td>
                        <td colspan="2">
                            <rad:RadTextBox ID="RTB_DiscountMoney" Text="0" runat="server" AutoPostBack="true"
                                ReadOnly="true">
                            </rad:RadTextBox>
                        </td>
                        <td style="text-align: right;">去年返利：
                        </td>
                        <td>
                            <rad:RadTextBox ID="RtbRebate" Text="0" runat="server" ReadOnly="true"></rad:RadTextBox>
                        </td>
                    </tr>
                </div>
                <div id="DIV_ExpectBalance" runat="server">
                    <tr>
                        <td style="text-align: right;">对方余额：
                        </td>
                        <td colspan="3">
                            <rad:RadTextBox ID="RTB_ExpectBalance" runat="server" ReadOnly="true">
                            </rad:RadTextBox>
                            <asp:RequiredFieldValidator ID="RFV_ExpectBalance" EnableClientScript="true" ControlToValidate="RTB_ExpectBalance"
                                runat="server" ErrorMessage="*" Text="*" />
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
                        <div id="TB" runat="server" style="width: 380px;">
                        </div>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">不通过理由：
                    </td>
                    <td colspan="4">
                        <rad:RadTextBox Height="40" Width="99%" TextMode="MultiLine" ID="RTB_BackReason"
                            runat="server">
                        </rad:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">往来单位收款人：
                    </td>
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
                <tr>
                    <td style="text-align: right;">发票类型：
                    </td>
                    <td colspan="4">
                        <asp:Label runat="server" ID="InvoiceTypeName"></asp:Label>
                    </td>
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
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" Width="50px"></ItemStyle>
                                        </rad:GridTemplateColumn>
                                        <rad:GridTemplateColumn HeaderText="税额" UniqueName="Tax">
                                            <ItemTemplate>
                                                <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("Tax").ToString())) %>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" Width="50px"></ItemStyle>
                                        </rad:GridTemplateColumn>
                                        <rad:GridTemplateColumn HeaderText="含税金额" UniqueName="TaxAmount">
                                            <ItemTemplate>
                                                <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("TaxAmount").ToString())) %>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                            <ItemStyle HorizontalAlign="Center" Width="50px"></ItemStyle>
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
                        <asp:Button ID="Through" runat="server" Text="通 过" OnClick="Through_Click" />
                        &nbsp;&nbsp;
                    <asp:Button ID="Back" runat="server" Text="不通过" OnClick="Back_Click" />
                    </td>
                </tr>
            </table>
        </div>
        <rad:RadGrid ID="RG_CheckInfo" runat="server" SkinID="Common" ShowFooter="true" OnNeedDataSource="RgCheckInfoNeedDataSource">
            <MasterTableView CommandItemDisplay="None" NoMasterRecordsText="无可用记录。">
                <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                <Columns>
                    <rad:GridBoundColumn DataField="ReceiptNo" HeaderText="单号" UniqueName="ReceipNo">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="往来单位" UniqueName="CompanyName">
                        <ItemTemplate>
                            <asp:Label ID="compName" runat="server" Text='<%# GetCompName(Eval("CompanyID").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="申请人" UniqueName="ApplicantName">
                        <ItemTemplate>
                            <asp:Label ID="personName" runat="server" Text='<%# GetPersonName(Eval("ApplicantID").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="ApplyDateTime" HeaderText="申请日期" UniqueName="ApplyDateTime">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="RealityBalance" DataType="System.Decimal" HeaderText="申请金额"
                        SortExpression="RealityBalance" UniqueName="RealityBalance" Aggregate="Sum" FooterText="合计：">
                        <ItemTemplate>
                            <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("RealityBalance").ToString())) %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                        <ItemStyle HorizontalAlign="Center" Width="50px"></ItemStyle>
                        <FooterStyle Font-Bold="true" HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="单据状态" UniqueName="ReceiptStatus">
                        <ItemTemplate>
                            <asp:Label ID="LB_ReceiptStatus" runat="server" Text='<%# GetReceiptStatus(Eval("ReceiptStatus").ToString()) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Remark">
                        <ItemTemplate>
                            <asp:ImageButton ID="RemarkImageButton" CommandName="Remark" runat="server" SkinID="InsertImageButton"
                                onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                            <div style="position: absolute;">
                                <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative; display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px; font-weight: bold; height: auto; overflow: visible; word-break: break-all;"
                                    runat="server">
                                    <%# Eval("Remark")%>
                                </div>
                            </div>
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="Through">
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="Back">
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_CompanyList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="TB_Payee" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBankAccount" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <%--<rad:AjaxSetting AjaxControlID="RCB_FilialeList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RTB_OwnBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_OwnBankAccount" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RCB_BankAccount" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="TB_Payee" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RTB_CompBankAccount" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>--%>
                <rad:AjaxSetting AjaxControlID="RCB_BankAccount">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_BankAccount" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/common.js" type="text/javascript"></script>
            <script type="text/javascript">

                function ShowImg(obj) {
                    var object = eval(obj);
                    object.style.display = "block";
                }

                function HiddleImg(obj) {
                    var object = eval(obj);
                    object.style.display = "none";
                }
            </script>
        </rad:RadScriptBlock>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.AffirmSum" CodeBehind="AffirmSum.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">

<script language="javascript" type="text/javascript">
    function affirm() {
        var tBoxRealTotalPrice = parseFloat(document.getElementById("<%=TBox_RealTotalPrice.ClientID %>").value);
        var tBoxPaidUp = parseFloat(document.getElementById("<%=TBox_PaidUp.ClientID %>").value);
        if (tBoxRealTotalPrice < tBoxPaidUp) {
            if (confirm("确认多收" + (tBoxPaidUp - tBoxRealTotalPrice).toFixed(2) + "元吗？")) {
                return true;
            }
            return false;
        }
        return true;
    }  
</script>

<head id="Head1" runat="server">
    <title>确认支付</title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    </rad:RadScriptBlock>

    <table class="PanelArea" border="1">
        <tr>
            <th>
                订单号
            </th>
            <th>
                收货人
            </th>
            <th>
                订单金额
            </th>
            <th>
                应付金额
            </th>
            <th>
                支付状态
            </th>
            <th>
                订单状态
            </th>
        </tr>
        <tr>
            <td>
                <asp:Label ID="Lbl_OrderNo" runat="server" Text=""></asp:Label>
            </td>
            <td>
                <asp:Label ID="Lbl_Consignee" runat="server" Text=""></asp:Label>
            </td>
            <td>
                <asp:Label ID="Lbl_TotalPrice" runat="server" Text=""></asp:Label>
            </td>
            <td>
                <asp:Label ID="Lbl_RealTotalPrice" runat="server" Text=""></asp:Label>
            </td>
            <td>
                <asp:Label ID="Lbl_PayMode" runat="server" Text=""></asp:Label>
            </td>
            <td>
                <asp:Label ID="Lbl_OrderState" runat="server" Text=""></asp:Label>
            </td>
        </tr>
    </table>

    <table class="PanelArea">
        <tr>
            <td>
                <table>
                    <tr>
                        <td class="PromptFromRowTitle">
                            应收款组成：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:Literal ID="Lit_TotalPrice" runat="server"></asp:Literal>+<asp:Literal ID="Lit_Carriage"
                                runat="server"></asp:Literal>-<asp:Literal ID="Lit_VoucherValue" runat="server"></asp:Literal>-<asp:Literal
                                    ID="Lit_PaymentByBalance" runat="server"></asp:Literal>
                            (货款+运费-礼券-帐号支付)
                        </td>
                    </tr>
                    <tr>
                        <td class="PromptFromRowTitle">
                            实际应收：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TBox_RealTotalPrice" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="PromptFromRowTitle">
                            实际收入：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TBox_PaidUp" runat="server" SkinID="LongInput"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVPaidUp" runat="server" ErrorMessage="实际收入不能为空！"
                                Display="Dynamic" Text="*" ControlToValidate="TBox_PaidUp" ValidationGroup="save"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="REVPaidUp" runat="server" ControlToValidate="TBox_PaidUp"
                                ValidationExpression="^\d+(\.(\d){1,2})?$" Text="*" ErrorMessage="必须输入数字，小数点后最多只能输入两位数！"
                                ValidationGroup="save"></asp:RegularExpressionValidator>
                            <asp:CompareValidator ID="CVPaidUp" runat="server" ControlToValidate="TBox_RealTotalPrice"
                                ControlToCompare="TBox_PaidUp" Type="Double" Operator="LessThanEqual" Text="*"
                                ErrorMessage="数值不允许小于实际应收！" ValidationGroup="save"></asp:CompareValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="PromptFromRowTitle">
                            资金账户：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <rad:RadComboBox ID="RCB_BankAccountsId" runat="server" MarkFirstMatch="True" Width="250px"
                                Height="100px" DataTextField="BankName" DataValueField="BankAccountsId">
                            </rad:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="PromptFromRowTitle">
                            备注说明：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TBox_Description" runat="server" TextMode="MultiLine" SkinID="LongInput"
                                Height="75"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="AreaEditFromRowInfo" align="center">
                            <asp:Panel ID="Panel_LinkButton" runat="server">
                                <asp:LinkButton ID="IBAffirm" OnClick="Button_Affirm" OnClientClick="return affirm()" runat="server" >
                                    <asp:Image ID="Image1" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />确定
                                </asp:LinkButton>
                                <asp:LinkButton ID="IBCancel" OnClick="Button_Cancel" runat="server">
                                    <asp:Image ID="Image2" SkinID="CancelImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />付款未到
                                </asp:LinkButton>
                                <asp:LinkButton ID="IBDelete" OnClick="Button_Delete" Visible="false" runat="server">
                                    <asp:Image ID="Image3" SkinID="CancelImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />删除
                                </asp:LinkButton>
                            </asp:Panel>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2" class="AreaEditFromRowInfo">
                            <asp:Label ID="LblAlert" runat="server" Text="" Visible="false" ForeColor="Red" Font-Bold="true"></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <table style="line-height: 20px;">
                    <tr>
                        <td>
                            <asp:Label ID="lab_bank" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lab_time" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lab_amount" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lab_name" runat="server" Text=""></asp:Label>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IBAffirm">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_LinkButton" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IBCancel">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_LinkButton" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IBDelete">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_LinkButton" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

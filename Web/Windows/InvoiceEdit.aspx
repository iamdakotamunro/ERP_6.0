<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceEdit.aspx.cs" Inherits="ERP.UI.Web.Windows.InvoiceEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="../JavaScript/telerik.js"></script>
    <script src="../My97DatePicker/WdatePicker.js"></script>
    <script src="../JavaScript/jquery.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <table style="width: 100%;">
            <tr>
                <td style="text-align: right;">开票单位：</td>
                <td>
                    <asp:TextBox ID="txt_BillingUnit" runat="server"></asp:TextBox>
                </td>
                <td style="text-align: right;">开票日期：</td>
                <td>
                    <asp:TextBox ID="txt_BillingDate" runat="server" onfocus="this.blur();" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">发票号码：</td>
                <td>
                    <asp:TextBox ID="txt_InvoiceNo" runat="server"></asp:TextBox>
                </td>
                <td style="text-align: right;">发票代码：</td>
                <td>
                    <asp:TextBox ID="txt_InvoiceCode" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">未税金额：</td>
                <td>
                    <asp:TextBox ID="txt_NoTaxAmount" runat="server" ReadOnly="True"></asp:TextBox>
                </td>
                <td style="text-align: right;">税额：</td>
                <td>
                    <asp:TextBox ID="txt_Tax" runat="server" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">含税金额：</td>
                <td>
                    <asp:TextBox ID="txt_TaxAmount" runat="server" ReadOnly="True"></asp:TextBox>
                </td>
                <td style="text-align: right;">发票状态：</td>
                <td>
                    <asp:TextBox ID="txt_InvoiceState" runat="server" ReadOnly="True"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">备注：</td>
                <td colspan="3">
                    <asp:TextBox ID="txt_Memo" runat="server" TextMode="MultiLine" Width="99%" Height="50px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: center;">
                    <asp:Button ID="btn_Save" runat="server" Text="保存" OnClick="btn_Save_Click" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>

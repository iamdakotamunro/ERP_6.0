<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_DepositRecoveryDetail.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_DepositRecoveryDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body style="overflow: hidden; margin-left: 8%; margin-top: 5px;">
    <form id="form1" runat="server">
        <table style="width: 100%; line-height: 20px;">
            <tr>
                <td style="width: 84px; text-align: right;">费用归属部门：
                </td>
                <td>
                    <asp:Label ID="lbl_ReportBranch" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">预借款申报单号：
                </td>
                <td>
                    <asp:Label ID="lbl_ReportNo" runat="server"></asp:Label>
                </td>
                <td style="text-align: right;">费用分类：
                </td>
                <td>
                    <asp:Label ID="lbl_CompanyClass" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">费用名称：
                </td>
                <td>
                    <asp:Label ID="lbl_ReportName" runat="server"></asp:Label>
                </td>
                <td style="text-align: right;">费用实际发生时间：
                </td>
                <td>
                    <asp:Label ID="lbl_ReportDate" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">申报金额：
                </td>
                <td>
                    <asp:Label ID="lbl_ReportCost" runat="server"></asp:Label>
                </td>
                <td style="text-align: right;">结算公司：
                </td>
                <td>
                    <asp:Label ID="lbl_PayCompany" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">结算账号：
                </td>
                <td>
                    <asp:Label ID="lbl_BankAccount" runat="server"></asp:Label>
                </td>
                <td style="text-align: right;">付款时间：
                </td>
                <td>
                    <asp:Label ID="lbl_ExecuteDate" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>

                <td style="text-align: right;">申请人：
                </td>
                <td>
                    <asp:Label ID="lbl_ReportPersonnelId" runat="server"></asp:Label>
                </td>
                <td style="text-align: right;">回收人：
                </td>
                <td>
                    <asp:Label ID="lbl_RecoveryPersonnelId" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">回收类型：
                </td>
                <td>
                    <asp:Label ID="lbl_RecoveryType" runat="server"></asp:Label>
                </td>
                <td style="text-align: right;">回收金额：
                </td>
                <td>
                    <asp:Label ID="lbl_RecoveryCost" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;"></td>
                <td>
                    <asp:Label ID="lbl_RecoveryType2" runat="server"></asp:Label>
                </td>
                <td style="text-align: right;"></td>
                <td>
                    <asp:Label ID="lbl_RecoveryCost2" runat="server"></asp:Label>
                </td>
            </tr>

        </table>
        <script src="../JavaScript/jquery.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script src="../JavaScript/tool.js"></script>
    </form>
</body>
</html>

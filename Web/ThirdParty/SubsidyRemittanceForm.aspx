<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubsidyRemittanceForm.aspx.cs" Inherits="ERP.UI.Web.ThirdParty.SubsidyRemittanceForm" %>

<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .back {
            background-color: cornflowerblue;
            border: none;
        }

        .pass {
            background-color: orange;
            border: none;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">

            <script type="text/javascript" src="../JavaScript/telerik.js"></script>

        </rad:RadScriptBlock>
        <div runat="server" id="DivAuditingPanel">
            <table>
                <tr>
                    <td colspan="4">费用补贴信息</td>
                </tr>
                <tr>
                    <td class="ShortFromRowTitle">订单编号：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbOrderNo" runat="server" Enabled="False" Width="200"></asp:TextBox>
                    </td>
                    <td class="ShortFromRowTitle">第三方订单编号：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbThirdPartyOrderNo" runat="server" Enabled="False" Width="200"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ShortFromRowTitle">会员名：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbMemberNo" runat="server" Enabled="False" Width="200"></asp:TextBox>
                    </td>
                    <td class="ShortFromRowTitle">会员账户：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbAccountsNo" runat="server" Enabled="False" Width="200"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ShortFromRowTitle">补贴类型：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbSubsidyType" runat="server" Enabled="False" Width="200"></asp:TextBox>
                    </td>
                    <td class="ShortFromRowTitle">补贴金额：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbAmount" runat="server" Enabled="False" Width="200"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <tr>
                        <td colspan="4">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">客户收款账户信息</td>
                    </tr>
                    <tr>
                        <td class="ShortFromRowTitle">结构名称：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TbBankName" runat="server" Width="200" Enabled="False"></asp:TextBox>
                        </td>
                        <td class="ShortFromRowTitle" style="width: 200px;">客户支付宝\银行账户：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TbBankAccountsNo" runat="server" Width="200" Enabled="False"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="ShortFromRowTitle">客户姓名：
                        </td>
                        <td class="AreaEditFromRowInfo" colspan="3">
                            <asp:TextBox ID="TbRealName" runat="server" Width="200" Enabled="False"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">打款信息</td>
                    </tr>
                    <tr>
                        <td class="ShortFromRowTitle">销售公司：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TbSaleFiliale" runat="server" Width="200" Enabled="False"></asp:TextBox>
                        </td>
                        <td class="ShortFromRowTitle" style="width: 200px;">手续费：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TbPoundage" runat="server" Width="200"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="ShortFromRowTitle">交易号：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TbFlowNo" runat="server" Width="200" ></asp:TextBox>
                        </td>
                        <td class="ShortFromRowTitle" style="width: 200px;">公司打款账户：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <rad:RadComboBox ID="RcbBankAccounts" runat="server" UseEmbeddedScripts="false" Width="120px" Height="100px" >
                            </rad:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="ShortFromRowTitle">备注信息：
                        </td>
                        <td class="AreaEditFromRowInfo" colspan="3">
                            <asp:TextBox ID="TbDescription" runat="server" TextMode="MultiLine" Width="600px" Height="30px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="ShortFromRowTitle">拒绝理由：
                        </td>
                        <td class="AreaEditFromRowInfo" colspan="3">
                            <asp:TextBox ID="TB_Memo" runat="server" TextMode="MultiLine" Width="600px" Height="50px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">&nbsp;
                        </td>
                    </tr>
                    <tr>
                        <td align="center" colspan="4">
                            <asp:Button ID="BtBack" runat="server" Text="退回申请" OnClick="BtBackClick" CssClass="back" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="BtRemittance" runat="server" Text="打款完成" OnClick="BtRemittanceClick" CssClass="pass" />
                        </td>
                    </tr>
            </table>
        </div>
    </form>
</body>
</html>


<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReviewRechargeManager.aspx.cs"
    Inherits="ERP.UI.Web.Windows.ReviewRechargeManager" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>充值确认</title>
       <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <style>
        td {
            padding: 5px 0px 0px 0px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <div style="padding: 10px">
        <table width="100%">
            <tr>
                <td>
                    店铺名称：
                </td>
                <td>
                    <asp:Label ID="lblShopName" runat="server" ></asp:Label>
                </td>
                <td>
                    可用余额：
                </td>
                <td>
                    <asp:Label ID="lblAccountTotalled" runat="server" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    银行名称：
                </td>
                <td>
                    <asp:Label ID="lblBankAccountName" runat="server"></asp:Label>
                </td>
                <td>
                    充值金额：
                </td>
                <td>
                     <asp:Label ID="lblMoney" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    交易号：
                </td>
                <td colspan="3">
                    <asp:Label ID="lblBankTradeNo" runat="server" ></asp:Label>
                </td>
            </tr>
            <tr>
                <td>
                    备注：
                </td>
                <td colspan="3">
                        <asp:TextBox ID="TB_Remark" runat="server" TextMode="MultiLine" Width="100%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td>
                    操作：
                </td>
                <td colspan="3">
                    <asp:RadioButtonList ID="rbSelectState" runat="server" Height="23px" 
                        RepeatDirection="Horizontal" Width="126px">
                          <asp:ListItem Value="1" Selected="True">已支付</asp:ListItem>
                            <asp:ListItem Value="2">拒绝</asp:ListItem>          
                    </asp:RadioButtonList>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="text-align: center">
                      <asp:Button runat="server" ID="BtnSelect" Text="确定" Width="80px" onclick="BtnSelect_Click" 
                  />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BtnSelect">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="BtnSelect" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

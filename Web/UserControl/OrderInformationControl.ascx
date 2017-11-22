<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OrderInformationControl.ascx.cs" Inherits="ERP.UI.Web.UserControl.OrderInformationControl" %>
原订单信息：
    <table class="PanelArea">
        <tr>
            <td class="AreaRowTitle">
                积分消费：
            </td>
            <td>
                <asp:label id="LB_Score" runat="server"></asp:label>
            </td>
            <td class="AreaRowTitle">
                商品总价：
            </td>
            <td>
                <asp:literal id="Lit_TotalPrice" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                运费：
            </td>
            <td>
                <asp:literal id="Lit_Carriage" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                总计：
            </td>
            <td>
                <asp:literal id="Lit_SumPrice" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                发票状态：
            </td>
            <td>
                <asp:Label ID="LB_Invoice" runat="server"></asp:Label>
            </td>
        </tr>
    </table>
    <table class="PanelArea">
        <tr>
            <td class="AreaRowTitle">
                帐号余额支付：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_PaymentByBalance" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                实际应收：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_RealTotalPrice" runat="server"></asp:literal>
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                促销优惠：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_VoucherValue" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                实际支付：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_PaidUp" runat="server"></asp:literal>
            </td>
        </tr>
    </table>
    <table class="PanelArea">
        <tr>
            <td class="AreaRowTitle">
                订单编号：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_OrderNo" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                快递单编号：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_ExpressNo" runat="server"></asp:literal>
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                收货人：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_OldCustomer" runat="server"></asp:literal><asp:literal id="Lit_Consignee"
                    runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                下单时间：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_OrderTime" runat="server"></asp:literal>
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                地址：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_Direction" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                邮编：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_PostalCode" runat="server"></asp:literal>
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                手机：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_Mobile" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                电话：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_Phone" runat="server"></asp:literal>
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                付款方式：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_PayMode" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                付款状态：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_PayState" runat="server"></asp:literal>
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                付款银行：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_BankAccounts" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                退款方式：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_RefundmentMode" runat="server"></asp:literal>
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                配送公司：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_Express" runat="server"></asp:literal>
            </td>
            <td class="AreaRowTitle">
                发货时间：
            </td>
            <td class="AreaRowInfo">
                <asp:literal id="Lit_ConsignTime" runat="server"></asp:literal>
            </td>
        </tr>
    </table>
    <table class="PanelArea">
        <tr>
            <td class="AreaRowTitle">
                附言：
            </td>
            <td>
                <asp:literal id="Lit_Memo" runat="server"></asp:literal>
            </td>
        </tr>
    </table>
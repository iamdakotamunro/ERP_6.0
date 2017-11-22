<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.ShowGoodsOrder"
    CodeBehind="ShowGoodsOrder.aspx.cs" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>订单详细</title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">

            <script src="../JavaScript/telerik.js" type="text/javascript"> </script>

        </rad:RadScriptBlock>
        <rad:RadGrid ID="RGGoodsOrderDetail" runat="server" SkinID="Common" OnNeedDataSource="RGGoodsOrderDetail_NeedDataSource" OnItemDataBound="RGGoodsOrderDetail_ItemDataBound">
            <MasterTableView DataKeyNames="RealGoodsId">
                <CommandItemTemplate>
                </CommandItemTemplate>
                <Columns>
                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                        <HeaderStyle Width="180px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="有效日期" HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Literal ID="lit_EffectiveDate" runat="server"></asp:Literal>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="70px" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="批次号" HeaderStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Literal ID="lit_BatchNo" runat="server"></asp:Literal>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" Width="70px" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="SellPrice" HeaderText="购买价格" UniqueName="SellPrice">
                        <ItemTemplate>
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("SellPrice"))%>
                        </ItemTemplate>
                        <HeaderStyle Width="70px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="Quantity" HeaderText="购买数量" UniqueName="Quantity">
                        <ItemTemplate>
                            <%# ShowText(Eval("RealGoodsId"),Eval("Quantity"),Eval("DemandQuantity")) %>
                        </ItemTemplate>
                        <HeaderStyle Width="70px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="TotalPrice" HeaderText="小计" UniqueName="TotalPrice">
                        <ItemTemplate>
                            <asp:Label ID="SubtotalLabel" runat="server" Text='<%# GetSubtotal(Eval("TotalPrice"),Eval("SellType")) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
                <ExpandCollapseColumn Visible="False">
                    <HeaderStyle Width="19px" />
                </ExpandCollapseColumn>
                <RowIndicatorColumn Visible="False">
                    <HeaderStyle Width="20px" />
                </RowIndicatorColumn>
            </MasterTableView>
        </rad:RadGrid>
        <table class="PanelArea">
            <tr>
                <td class="AreaQuaRowTitle">积分消费：
                </td>
                <td class="AreaQuaRowInfo">
                    <asp:Label ID="LB_Score" runat="server"></asp:Label>
                </td>
                <td class="AreaQuaRowTitle">商品总价：
                </td>
                <td class="AreaQuaRowInfo">
                    <asp:Literal ID="Lit_TotalPrice" runat="server"></asp:Literal>
                </td>
                <td class="AreaQuaRowTitle">
                    <asp:Literal ID="Lit_ShowCarriage" Text="运费：" runat="server"></asp:Literal>
                </td>
                <td class="AreaQuaRowInfo">
                    <asp:Literal ID="Lit_Carriage" runat="server"></asp:Literal>
                </td>
                <td class="AreaQuaRowTitle">总计：
                </td>
                <td class="AreaQuaRowInfo">
                    <asp:Literal ID="Lit_SumPrice" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td class="AreaQuaRowTitle">余额支付：
                </td>
                <td class="AreaQuaRowInfo">
                    <asp:Literal ID="Lit_PaymentByBalance" runat="server"></asp:Literal>
                </td>
                <td class="AreaQuaRowTitle">优惠券抵扣：
                </td>
                <td class="AreaQuaRowInfo">
                    <asp:Literal ID="Lit_VoucherValue" runat="server"></asp:Literal>
                </td>
                <td class="AreaQuaRowTitle">
                    <asp:Label ID="Lbl_Get" runat="server" Text="实际应收："></asp:Label>
                </td>
                <td class="AreaQuaRowInfo">
                    <asp:Literal ID="Lit_RealTotalPrice" runat="server"></asp:Literal>
                </td>
                <td class="AreaQuaRowTitle">
                    <asp:Label ID="Lbl_Pay" runat="server" Text="实际支付："></asp:Label>
                </td>
                <td class="AreaRowInfo">
                    <asp:Literal ID="Lit_PaidUp" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
        <div style="width: 100%;" id="showGoodsOrderPay" runat="server" visible="false" />
        <asp:Table ID="VoucherExplainTab" runat="server" CssClass="PanelArea" Visible="false">
            <asp:TableRow>
                <asp:TableCell CssClass="ShowInfo">
                    <asp:Label ID="LB_VoucherExplain" runat="server" Text=""></asp:Label>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>

        
        <table class="PanelArea">
            <tr>
                <td class="AreaRowTitle">订单编号：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="Lit_OrderNo" runat="server"></asp:Label>
                </td>
                <td class="AreaRowTitle">快递单编号：
                </td>
                <td class="AreaRowInfo">
                   <asp:Label ID="lbl_ExpressNo" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">收货人：
                </td>
                <td class="AreaRowInfo">
                    <asp:TextBox ID="Lit_Consignee" runat="server" SkinID="LongTextarea"></asp:TextBox>
                </td>
                <td class="AreaRowTitle">下单时间：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="Lit_OrderTime" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">地址：
                </td>
                <td class="AreaRowInfo">
                    <asp:Literal ID="Lit_Direction" runat="server"></asp:Literal>
                </td>
                <td class="AreaRowTitle">邮编：
                </td>
                <td class="AreaRowInfo">
                    <asp:TextBox ID="Lit_PostalCode" runat="server" SkinID="LongInput"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">手机：
                </td>
                <td class="AreaRowInfo">
                    <asp:TextBox ID="Lit_Mobile" runat="server" SkinID="LongInput"></asp:TextBox>
                </td>
                <td class="AreaRowTitle">电话：
                </td>
                <td class="AreaRowInfo">
                    <asp:TextBox ID="Lit_Phone" runat="server" SkinID="LongInput"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">付款方式：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="Lit_PayMode" runat="server"></asp:Label>
                </td>
                <td class="AreaRowTitle">付款状态：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="Lit_PayState" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">付款银行：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="Lit_BankAccounts" runat="server"></asp:Label>
                </td>
                <td class="AreaRowTitle">退款方式：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="Lit_RefundmentMode" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">发票状态：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="LB_InvoiceState" runat="server"></asp:Label>
                </td>
                <td class="AreaRowTitle">下单来源：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="LB_FromsourceId" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">配送公司：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="Lab_Express" runat="server"></asp:Label>
                </td>
                <td class="AreaRowTitle">发货时间：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="LB_ConsignTime" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">订单状态：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="LB_OrderState" runat="server"></asp:Label>
                </td>
                <td class="AreaRowTitle">&nbsp;
                </td>
                <td class="AreaRowInfo">
                    &nbsp;
                </td>
            </tr>
            <tr>
                
                <td class="AreaRowTitle">发货仓库：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="lblConsigneeWarehouse" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
        <div style="width: 100%;" id="showinvoice" runat="server" visible="false">
            <table border="0" cellpadding="0" cellspacing="0" width="100%">
                <tr>
                    <td class="AreaRowTitle">发票抬头：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label ID="Lit_InvoiceName" runat="server"></asp:Label>
                    </td>
                    <td class="AreaRowTitle">商品SKU：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label ID="Lbl_Standard" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="AreaRowTitle">发票金额：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label ID="Lbl_Money" runat="server"></asp:Label>
                    </td>
                    <td class="AreaRowTitle">发票状态：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label ID="Lbl_InvoiceState" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="AreaRowTitle">申请时间：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label ID="Lbl_InvoiceStartTime" runat="server"></asp:Label>
                    </td>
                    <td class="AreaRowTitle">开票时间：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label ID="Lit_AcceptedTime" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
        </div>
        <table class="PanelArea">
            <tr>
                <td class="AreaRowTitle">附言：
                </td>
                <td>
                    <asp:Literal ID="Lit_Memo" runat="server"></asp:Literal>
                </td>
            </tr>
        </table>
        <table style="width: 100%;">
            <tr>
                <td colspan="2" style="height: 30px; text-align: center;">
                    <asp:Literal ID="Lit_FWebName" runat="server"></asp:Literal>
                    <br />
                    索要发票请登录“我的帐户”中“发票管理”
                </td>
            </tr>
        </table>
    </form>
</body>
</html>

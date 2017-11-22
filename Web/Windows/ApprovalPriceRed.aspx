<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalPriceRed.aspx.cs" Inherits="ERP.UI.Web.Windows.ApprovalPriceRed" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <asp:Panel ID="Panel_SemiStockInForm" runat="server">
            <table class="PanelArea">
                <tr>
                    <td style="text-align: right;">新单编号：
                    </td>
                    <td colspan="3">
                        <rad:RadTextBox ID="RtbNewCode" runat="server" Width="250px" ReadOnly="True">
                        </rad:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">原单编号：
                    </td>
                    <td>
                        <rad:RadTextBox ID="RTB_LinkTradeCode" runat="server" Width="250px">
                        </rad:RadTextBox>
                    </td>
                    <td style="text-align: right;">收货公司：
                    </td>
                    <td>
                        <asp:TextBox ID="txt_HostingFiliale" runat="server" ReadOnly="true" Style="width: 250px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">录单时间：
                    </td>
                    <td>
                        <asp:TextBox ID="txt_DateCreated" runat="server" ReadOnly="true" Style="width: 250px;"></asp:TextBox>
                    </td>
                    <td style="text-align: right;">单位名称：
                    </td>
                    <td>
                        <asp:TextBox ID="txt_ThirdCompany" runat="server" ReadOnly="true" Style="width: 250px;"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">经 办 人：
                    </td>
                    <td>
                        <asp:TextBox ID="txt_Transactor" runat="server" ReadOnly="true" Style="width: 250px;"></asp:TextBox>
                    </td>
                    <td style="text-align: right;">入 库 仓：
                    </td>
                    <td>
                        <asp:TextBox ID="txt_Warehouse" runat="server" Width="105px" ReadOnly="true"></asp:TextBox>
                        储：
                            <asp:TextBox ID="txt_StorageType" runat="server" Width="105px" ReadOnly="true"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">原始备注：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txt_OldDescription" runat="server" Width="99%" Height="30px" TextMode="MultiLine" ReadOnly="True" placeholder="原入库单备注信息"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">备注说明：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="txt_Description" runat="server" Width="99%" Height="30px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <rad:RadGrid ID="RGGoods" AllowPaging="False" Height="300px" runat="server" OnNeedDataSource="RGGoods_NeedDataSource">
                <ClientSettings>
                    <Resizing AllowColumnResize="True"></Resizing>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                    <ClientMessages DragToResize="调整大小" />
                    <Selecting AllowRowSelect="True" />
                </ClientSettings>
                <MasterTableView DataKeyNames="GoodsId,RealGoodsId,GoodsCode">
                    <CommandItemTemplate>
                    </CommandItemTemplate>
                    <CommandItemStyle Height="0px" />
                    <Columns>
                        <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="Units" HeaderText="销售单位" UniqueName="Units">
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="Quantity" HeaderText="出/入库数量" UniqueName="Quantity">
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn HeaderText="原价">
                            <ItemTemplate>
                                <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("OldUnitPrice").ToString())) %>
                            </ItemTemplate>
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="修改价">
                            <ItemTemplate>
                                <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString())) %>
                            </ItemTemplate>
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                            <ItemStyle  HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </rad:RadGrid>
            <div style="text-align: center; padding-top: 10px;">
                <asp:Button ID="btn_Approval" runat="server" Text="核准" OnClick="btnApproval_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Return" runat="server" Text="核退" OnClick="btnReturn_Click" />
            </div>
        </asp:Panel>
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
    </form>
</body>
</html>

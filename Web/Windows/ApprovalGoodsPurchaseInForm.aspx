<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalGoodsPurchaseInForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ApprovalGoodsPurchaseInForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/jquery.js" type="text/javascript"></script>
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/common.js" type="text/javascript"></script>
            <script src="../My97DatePicker/WdatePicker.js"></script>
        </rad:RadScriptBlock>
        <asp:Panel ID="Panel_SemiStockInForm" runat="server">
            <div class="StagePanel">

                <table class="PanelArea" style="line-height: 25px;">
                    <tr>
                        <td style="text-align: right;">单据编号：
                        </td>
                        <td>
                            <asp:Label ID="lbl_TradeCode" runat="server"></asp:Label>
                        </td>
                        <td style="text-align: right;">原始单号：
                        </td>
                        <td>
                            <asp:Label ID="lbl_OriginalCode" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="width: 80px; text-align: right;">申请时间：
                        </td>
                        <td>
                            <asp:Label ID="lbl_DateCreated" runat="server"></asp:Label>
                        </td>
                        <td style="width: 100px; text-align: right;">操 作 人：
                        </td>
                        <td>
                            <asp:Label ID="lbl_Transactor" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">供 应 商：
                        </td>
                        <td>
                            <asp:Label ID="lbl_CompanyId" runat="server"></asp:Label>
                        </td>
                        <td style="text-align: right;">收货公司：
                        </td>
                        <td>
                            <asp:Label ID="lbl_Filiale" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">入库仓储：
                        </td>
                        <td>
                            <asp:Label ID="lbl_Warehouse" runat="server"></asp:Label>
                        </td>
                        <td style="text-align: right;">入库公司：
                        </td>
                        <td>
                            <asp:Label ID="lbl_HostingFilialeAuth" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">采购负责人：
                        </td>
                        <td>
                            <asp:Label ID="lbl_Personnel" runat="server"></asp:Label>
                        </td>
                        <td style="text-align: right;">物流单号：
                        </td>
                        <td>
                            <asp:Label ID="lbl_LogisticsCode" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">备注说明：
                        </td>
                        <td colspan="3">
                            <asp:Label ID="lbl_Description" runat="server"></asp:Label>
                        </td>
                    </tr>
                </table>
                <rad:RadGrid ID="RGGoods" AllowPaging="False" Height="280px" runat="server" SkinID="Common"
                    OnNeedDataSource="RgGoodsNeedDataSource">
                    <ClientSettings>
                        <Resizing AllowColumnResize="True"></Resizing>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                        <ClientMessages DragToResize="调整大小" />
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="GoodsId,RealGoodsId,GoodsCode,GoodsName,Specification,Units,Quantity,UnitPrice,ApprovalNO">
                        <CommandItemTemplate>
                            <asp:LinkButton ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid">
                                <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />刷新
                            </asp:LinkButton>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="24px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                                Visible="False">
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode" Visible="False">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>

                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="Quantity" HeaderText="入库数" UniqueName="Quantity">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_Quantity" runat="server" Text='<%# Bind("Quantity") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                                <ItemTemplate>
                                    <asp:Label ID="lbUnitPrice" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString())) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="是否赠品" UniqueName="type">
                                <ItemTemplate>
                                    <%# Convert.ToDecimal(Eval("UnitPrice"))==0?"赠品":"" %>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="ApprovalNO" HeaderText="批文号" UniqueName="ApprovalNO">
                                <ItemTemplate>
                                    <asp:Label ID="lbApprovalNO" runat="server" Text='<%# Bind("ApprovalNO") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Description" HeaderText="备注" Visible="false" UniqueName="Description">
                                <ItemTemplate>
                                    <asp:TextBox ID="TB_Description" runat="server" Text='<%# Bind("Description") %>'>
                                    </asp:TextBox>
                                </ItemTemplate>
                                <HeaderStyle Width="140px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
                <table class="PanelArea">
                    <tr>
                        <td class="AreaRowTitle">合计数量：
                        </td>
                        <td class="AreaRowInfo">
                            <asp:Label ID="Lab_TotalNumber" runat="server" Text="0"></asp:Label>
                        </td>
                        <td class="AreaRowTitle">合计金额：
                        </td>
                        <td class="AreaRowInfo">
                            <asp:Label ID="Lab_TotalAmount" runat="server" Text="0"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
             <div style="text-align: center; padding-top: 10px;">
                <asp:Button ID="btn_Approval" runat="server" Text="  核准  " OnClick="btnApproval_Click" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Return" runat="server" Text="  核退  " OnClick="btnReturn_Click" />
            </div>
        </asp:Panel>
        <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
            <AjaxSettings>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

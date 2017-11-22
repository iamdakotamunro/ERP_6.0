<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalDefectiveReturnOutForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ApprovalDefectiveReturnOutForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
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
        <asp:Panel runat="server">
            <div class="StagePanel">
                <table class="PanelArea" style="line-height: 25px;">
                    <tr>
                        <td style="width: 90px">申请时间：
                        </td>
                        <td>
                            <asp:Label ID="lbl_DateCreated" runat="server"></asp:Label>
                        </td>
                        <td style="width: 90px">操 作 人：
                        </td>
                        <td>
                            <asp:Label ID="lbl_Transactor" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>出库仓储：
                        </td>
                        <td>
                            <asp:Label ID="lbl_Warehouse" runat="server"></asp:Label>
                        </td>
                        <td>物流配送公司：
                        </td>
                        <td>
                            <asp:Label ID="lbl_HostingFilialeAuth" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td >供 应 商：
                        </td>
                        <td colspan="3">
                            <asp:Label ID="lbl_CompanyName" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td>备注说明：
                        </td>
                        <td colspan="3">
                            <asp:Label ID="lbl_Description" runat="server"></asp:Label>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td style="padding-left: 5px;">当前商品清单：</td>
                        <td colspan="3" style="text-align: right; padding-right: 10px;"></td>
                    </tr>

                </table>
                <rad:RadGrid ID="RGGoods" AllowPaging="false" runat="server" SkinID="Common" Height="266px"
                    OnNeedDataSource="RgGoods_NeedDataSource">
                    <ClientSettings>
                        <Resizing AllowColumnResize="True"></Resizing>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                        <ClientMessages DragToResize="调整大小" />
                    </ClientSettings>
                    <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="RealGoodsId,UnitPrice,GoodsId">
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="24px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle Width="80px" HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                <HeaderStyle Width="205px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>

                            <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>

                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_UnitPrice" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString())) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>


                            <rad:GridTemplateColumn DataField="Quantity" HeaderText="退货数" UniqueName="Quantity">
                                <ItemTemplate>
                                    <asp:Label ID="lbl_Quantity" runat="server" Text='<%# Math.Abs(Convert.ToDouble(Eval("Quantity"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>

                            <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                                Visible="False">
                            </rad:GridBoundColumn>
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
                <rad:AjaxSetting AjaxControlID="btn_Refresh">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGGoods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

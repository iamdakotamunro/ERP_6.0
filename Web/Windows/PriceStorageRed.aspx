<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PriceStorageRed.aspx.cs" Inherits="ERP.UI.Web.Windows.PriceStorageRed" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
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
            <div class="StagePanel">
                <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0"
                    Width="100%">
                    <asp:TableRow>
                        <asp:TableCell CssClass="ControlTools">
                            <asp:LinkButton ID="LB_Inster" runat="server" OnClick="btn_InsterStock">
                                <asp:Image ID="IB_Inster" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                                    BorderStyle="None" />提交
                            </asp:LinkButton>
                            <asp:Label ID="Lab_InsterSpace" runat="server">&nbsp;</asp:Label>
                            <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()"
                                SkinType="Cancel" Text="取消"></Ibt:ImageButtonControl>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <table class="PanelArea">
                    <tr >
                        <td style="text-align: right;">
                            <asp:Label runat="server" ID="LbNewCode"></asp:Label>
                        </td>
                        <td colspan="3">
                            <rad:RadTextBox ID="RtbNewTradeCode" runat="server" ReadOnly="True" Width="250px"></rad:RadTextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">
                            <asp:Label runat="server" ID="LbOriginalCode"></asp:Label>
                        </td>
                        <td>
                            <rad:RadTextBox ID="RTB_LinkTradeCode" runat="server" EmptyMessage="搜索单据号"
                                OnTextChanged="RTB_LinkTradeCode_OnTextChanged" AutoPostBack="True" onkeydown="if(event.keyCode==13)return false;" Width="250px">
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
                    <MasterTableView DataKeyNames="GoodsId,RealGoodsId,GoodsCode" CommandItemDisplay="None">
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                <HeaderStyle  HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Units" HeaderText="销售单位" UniqueName="Units">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Quantity" HeaderText="出/入库数量" UniqueName="Quantity">
                                <HeaderStyle Width="90"  HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="原价">
                                <ItemTemplate>
                                    <asp:Label runat="server" ID="lbl_UnitPrice" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("OldUnitPrice").ToString())) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="90"  HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="修改价">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt_ModifyPrice" runat="server" Font-Bold="true" onblur="check(this,'Decimal');" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString())) %>' Width="99%"></asp:TextBox>
                                </ItemTemplate>
                                <HeaderStyle Width="90" HorizontalAlign="Center" />
                                <ItemStyle  HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </div>
        </asp:Panel>
        
       <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RTB_LinkTradeCode">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_SemiStockInForm" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Inster">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_SemiStockInForm" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/tool.js"></script>
        <script type="text/javascript">
            //验证类型
            function check(obj, type) {
                if (!$.checkType(type).test($(obj).val())) {
                    $(obj).val("");
                    $(obj).attr("placeholder", castErrorMessage(type));
                }
            }
        </script>
    </form>
</body>
</html>

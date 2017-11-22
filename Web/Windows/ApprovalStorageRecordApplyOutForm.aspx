<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalStorageRecordApplyOutForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ApprovalStorageRecordApplyOutForm" %>
<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
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
                <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0"
                    Width="100%">
                    <asp:TableRow>
                        <asp:TableCell CssClass="ControlTools">
                            <asp:LinkButton ID="LB_ModifyStock" runat="server" >
                                <asp:Image ID="IB_ModifyStock" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                                    BorderStyle="None" />审核
                            </asp:LinkButton>
                            <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()"
                                SkinType="Cancel" Text="取消"></Ibt:ImageButtonControl>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <table class="PanelArea" style="line-height: 20px;">
                    <tr>
                        <td>单据编号：
                        </td>
                        <td>
                            <asp:Label ID="Label2" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            单位名称：
                        </td>
                        <td>
                            <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                     <tr>
                        <td>申请时间：
                        </td>
                        <td>
                            <asp:Label ID="Label4" runat="server" Text="Label"></asp:Label>
                        </td>
                        <td>
                            操 作 人：
                        </td>
                        <td>
                            <asp:Label ID="Label5" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                     <tr>
                          <td>出库仓储：
                        </td>
                        <td>
                          <asp:Label ID="Label6" runat="server" Text="Label"></asp:Label>
                          <asp:Label ID="Label7" runat="server" Text="Label"></asp:Label>
                        </td>
                         <td>物流配送公司：
                        </td>
                        <td>
                              <asp:Label ID="Label8" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                    

                  
                    <tr>
                        <td>备注说明：
                        </td>
                        <td colspan="3">
                            <asp:Label ID="Label9" runat="server" Text="Label"></asp:Label>
                        </td>
                    </tr>
                     <tr>
                <td  colspan="4">
                    <hr />
                </td>
            </tr>
            <tr>
                <td>
                    复制单据类型：
                </td>
                <td>
                    <asp:Label ID="Label10" runat="server" Text="Label"></asp:Label>
                </td>
                <td >
                    复制单据号：
                </td>
                <td>
                    <asp:Label ID="Label11" runat="server" Text="Label"></asp:Label>
                </td>
            </tr>
            <tr>
                <td >
                   [ 注 解：    
                </td>
                <td  colspan="3">
                    <asp:Label ID="Label1" runat="server" Text="用于复制生成单据的商品信息。选择单据类型和单据号，则会自动加载所选择单据的商品信息。]"></asp:Label>
                </td>
            </tr>
                </table>
                <rad:RadGrid ID="RGGoods" AllowPaging="False" Height="300px" runat="server" SkinID="Common"
                    OnNeedDataSource="RgGoodsNeedDataSource">
                    <ClientSettings>
                        <Resizing AllowColumnResize="True"></Resizing>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                        <ClientMessages DragToResize="调整大小" />
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="GoodsId,RealGoodsId,UnitPrice,GoodsType,EffectiveDate,RegistrationNumber,ProductionPermitNo,ProductionUnit">
                        <CommandItemTemplate>
                        <asp:LinkButton ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid">
                            <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />刷新
                        </asp:LinkButton>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="24px" />
                        <Columns>
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
                                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Units" HeaderText="可出库数" UniqueName="Units">
                                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridTemplateColumn DataField="Quantity" HeaderText="数量" UniqueName="Quantity">
                                <ItemTemplate>
                                    
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                                <ItemTemplate>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                           
                         
                            
                            <rad:GridTemplateColumn HeaderText="金额">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt_EffectiveDate" runat="server" Text='<%#Convert.ToDateTime(Eval("EffectiveDate")).ToString("yyyy-MM-dd").Equals("1900-01-01")?"":Convert.ToDateTime(Eval("EffectiveDate")).ToString("yyyy-MM-dd")%>' Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="85px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" Width="85px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="是否赠品" UniqueName="type">
                                <ItemTemplate>
                                    <%# Convert.ToDecimal(Eval("UnitPrice"))==0?"赠品":"" %>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            
                            <rad:GridTemplateColumn UniqueName="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButton1" SkinID="DeleteImageButton" runat="server" CommandName="Delete" />
                                </ItemTemplate>
                                <HeaderStyle Width="35px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                                Visible="False">
                            </rad:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
                
            </div>
        </asp:Panel>
        <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
            
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

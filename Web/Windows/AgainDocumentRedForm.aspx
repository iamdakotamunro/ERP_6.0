<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AgainDocumentRedForm.aspx.cs" Inherits="ERP.UI.Web.Windows.AgainDocumentRedForm" %>
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
                            <asp:LinkButton ID="LB_Inster" runat="server" >
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
                    <tr>
                        <td>新单编号：
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="TextBox10" runat="server" ReadOnly="true"  SkinID="LongInput"></asp:TextBox>
                        </td>
                        
                    </tr>
                    <tr>
                        <td>单据编号：
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox7" runat="server" ReadOnly="true"  SkinID="LongInput"></asp:TextBox>
                        </td>
                        <td>
                            收货公司：
                        </td>
                        <td>
                              <asp:TextBox ID="TextBox8" runat="server" ReadOnly="true"  SkinID="LongInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>录单时间：
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox1" runat="server" ReadOnly="true" SkinID="LongInput"></asp:TextBox>
                        </td>
                        <td>单位名称：
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox2" runat="server" ReadOnly="true"  SkinID="LongInput" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>经 办 人：
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox3" runat="server" ReadOnly="true" SkinID="LongInput"></asp:TextBox>
                        </td>
                        <td>出 库 仓：
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox9" runat="server" Width="104px" ReadOnly="true" ></asp:TextBox>
                            储：
                            <asp:TextBox ID="TextBox4" runat="server" Width="104px" ReadOnly="true" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>原始备注：
                        </td>
                        <td colspan="3">
                             <asp:TextBox ID="TextBox5" runat="server" Width="87%" MaxLength="100" Height="30px"  TextMode="MultiLine" ReadOnly="true" ></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>备注说明：
                        </td>
                        <td colspan="3">
                             <asp:TextBox ID="TextBox6" runat="server" Width="87%" MaxLength="100" Height="30px"  TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <rad:RadGrid ID="RGGoods" AllowPaging="False" Height="300px" runat="server" 
                    OnNeedDataSource="RgGoodsNeedDataSource" >
                    <ClientSettings>
                        <Resizing AllowColumnResize="True"></Resizing>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                        <ClientMessages DragToResize="调整大小" />
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="GoodsId,RealGoodsId,UnitPrice,GoodsType,EffectiveDate,RegistrationNumber,ProductionPermitNo,ProductionUnit">
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="0px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="" HeaderText="商品名称" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="" HeaderText="SKU" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="销售单位" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="出库数量" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="原价" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="修改价" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
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

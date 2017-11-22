<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalDocumentRedForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ApprovalDocumentRedForm" %>

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
                <table class="PanelArea">
                    <tr>
                        <td>新单编号：
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="TextBox10" runat="server" ReadOnly="true"  SkinID="LongInput"></asp:TextBox>
                        </td>
                        
                    </tr>
                    <tr>
                        <td>原单编号：
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
                        <td>入 库 仓：
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
                             <asp:TextBox ID="TextBox6" runat="server" Width="87%" MaxLength="100" Height="30px"  TextMode="MultiLine" ReadOnly="true" ></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <rad:RadGrid ID="RGGoods" AllowPaging="False" Height="270px" runat="server" 
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
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="" HeaderText="SKU" UniqueName="">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="销售单位" UniqueName="">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="入库数量" UniqueName="">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="原价" UniqueName="">
                                <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="修改价" UniqueName="">
                                <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </div>
            <div style="text-align: center; padding-top: 10px;">
                <asp:Button ID="Button1" runat="server" Text="核准" />
                <asp:Button ID="Button2" runat="server" Text="核退" />
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

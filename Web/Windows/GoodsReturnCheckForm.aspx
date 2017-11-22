<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsReturnCheckForm.aspx.cs" Inherits="ERP.UI.Web.Windows.GoodsReturnCheckForm" %>

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
            <div >
                <rad:RadGrid ID="RGGoods"   runat="server" 
                    OnNeedDataSource="RgGoodsNeedDataSource" >
                    <ClientSettings>
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="GoodsId,RealGoodsId,UnitPrice,GoodsType,EffectiveDate,RegistrationNumber,ProductionPermitNo,ProductionUnit">
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="0px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="" HeaderText="商品编号" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="商品名称" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="" HeaderText="SKU" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="购买数量" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="退回数量" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                             <rad:GridBoundColumn DataField="" HeaderText="处理结果" UniqueName="">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
                <br/>
                <br/>
                <table style="width: 100%">
                    <tr>
                        <td align="right">检查结果：
                        </td>
                        <td >
                            <asp:RadioButton ID="RadioButton2" runat="server" Text="通过"/>
                            <asp:RadioButton ID="RadioButton1" runat="server" Text="退回"/>
                        </td>
                        
                    </tr>
                    <tr>
                        <td align="right">退货仓储：
                        </td>
                        <td>
                           <rad:RadComboBox ID="RadComboBox1" runat="server" Width="120px" Height="200px">
                    </rad:RadComboBox>
                            <rad:RadComboBox ID="RadComboBox2" runat="server" Width="120px" Height="200px">
                    </rad:RadComboBox>
                        </td>
                        
                    </tr>
                    <tr>
                        <td align="right">拒绝原因：
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox6" runat="server" Width="60%" MaxLength="100" Height="40px"  TextMode="MultiLine" ></asp:TextBox>
                        </td>
                       
                    </tr>
                    
                </table>
            </div>
            <div style="text-align: center; padding-top: 10px;">
                <asp:Button ID="Button1" runat="server" Text="提交" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="Button2" runat="server" Text="取消" />
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

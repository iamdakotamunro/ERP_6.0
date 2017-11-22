<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AllocationOutForm.aspx.cs" Inherits="ERP.UI.Web.Windows.AllocationOutForm" %>
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
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script type="text/javascript" src="../JavaScript/common.js"></script>
    </rad:RadScriptBlock>
    <asp:Button ID="BtnIsDelete" runat="server" Text="Button" Style="display: none;" />
    <asp:Panel class="StagePanel" runat="server">
        <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0"
            Width="100%">
            <asp:TableRow>
                <asp:TableCell CssClass="ControlTools">
                    <asp:LinkButton ID="LB_Inster" runat="server" >
                        <asp:Image ID="IB_Inster" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />保存
                    </asp:LinkButton>
                    <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()"
                        SkinType="Cancel" Text="取消"></Ibt:ImageButtonControl>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <table class="PanelArea">
            <tr>
                <td class="AreaRowTitle">
                    物流配送公司：
                </td>
                <td class="AreaRowInfo">
                    <rad:RadComboBox ID="RCB_Filiale" runat="server" AutoPostBack="True" Width="250px" Height="150px" >
                        </rad:RadComboBox>
                </td>
                <td class="AreaRowTitle">
                    单据编号：
                </td>
                <td class="AreaRowInfo">
                    <asp:TextBox ID="TB_TradeCode" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">
                    原单据类型：
                </td>
                <td class="AreaRowInfo">
                    <rad:RadComboBox ID="RadComboBox1" runat="server"  Width="250px" Height="150px" >
                        </rad:RadComboBox>
                </td>
                <td class="AreaRowTitle">
                    原单据号：
                </td>
                <td class="AreaRowInfo">
                    <rad:RadComboBox ID="RadComboBox2" runat="server"  Width="250px" Height="150px" >
                        </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">
                    录单时间：
                </td>
                <td class="AreaRowInfo">
                    <asp:TextBox ID="TB_DateCreated" runat="server" ReadOnly="true" SkinID="LongInput"></asp:TextBox>
                </td>
                
            </tr>
            <tr>
                <td class="AreaRowTitle">
                    经 办 人：
                </td>
                <td class="AreaRowInfo" >
                    <asp:TextBox ID="TB_Transactor" runat="server" ReadOnly="true" SkinID="LongInput"></asp:TextBox>
                </td>
            <td class="AreaRowTitle">
                    入库公司：
                </td>
                <td class="AreaRowInfo" >
                     <rad:RadComboBox ID="RadComboBox4" runat="server"  Width="250px" Height="150px" >
                        </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">
                    出库仓库：
                </td>
                <td class="AreaRowInfo" >
                    <rad:RadComboBox ID="RCB_InStock" CausesValidation="false" 
                        runat="server" AccessKey="T" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="CompanyName"
                        DataValueField="CompanyId" Width="123px" Height="200px">
                    </rad:RadComboBox>
                     <rad:RadComboBox ID="RadComboBox3" CausesValidation="false" 
                        runat="server" AccessKey="T" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="CompanyName"
                        DataValueField="CompanyId" Width="122px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td class="AreaRowTitle">
                    入库仓库：
                </td>
                <td class="AreaRowInfo" >
                    <rad:RadComboBox ID="RadComboBox5" CausesValidation="false" 
                        runat="server" AccessKey="T" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="CompanyName"
                        DataValueField="CompanyId" Width="123px" Height="200px">
                    </rad:RadComboBox>
                     <rad:RadComboBox ID="RadComboBox6" CausesValidation="false" 
                        runat="server" AccessKey="T" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="CompanyName"
                        DataValueField="CompanyId" Width="122px" Height="200px">
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">
                    备注说明：
                </td>
                <td class="AreaRowInfo" colspan="3">
                    <asp:TextBox ID="TB_Description" runat="server" SkinID="AutoInput" TextMode="MultiLine" Height="30px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowInfo" colspan="4">
                    <hr />
                </td>
            </tr>
            
            
        </table>
        
        <rad:RadGrid ID="RGGoods" AllowPaging="false" runat="server" SkinID="Common" Height="266px"
            OnNeedDataSource="RGGoods_NeedDataSource" >
            <ClientSettings>
                <Resizing AllowColumnResize="True"></Resizing>
                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                <ClientMessages DragToResize="调整大小" />
            </ClientSettings>
            <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="RealGoodsId,UnitPrice,GoodsId">
                <CommandItemTemplate>
                    <asp:LinkButton ID="lbtnSelectGoods" OnClientClick="return ShowObject('GoodsPanel');" runat="server">
                        <asp:Image ID="AddRecord" runat="server" ImageAlign="AbsMiddle" SkinID="AddImageButton" />
                        选择商品</asp:LinkButton>&nbsp;&nbsp;&nbsp;
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
                        <HeaderStyle Width="205px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                 
                    <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="NonceWarehouseGoodsStock" HeaderText="可出库数" UniqueName="NonceWarehouseGoodsStock">
                        <ItemTemplate>
                            <asp:Literal ID="Lab_NonceWarehouseGoodsStock" runat="server" Text='<%#Eval("NonceWarehouseGoodsStock")  %>'></asp:Literal>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="Quantity" HeaderText="数量" UniqueName="Quantity">
                        <ItemTemplate>
                            <asp:TextBox ID="TB_Quantity" runat="server" Font-Bold="true" Text='<%# Math.Abs(Convert.ToDouble(Eval("Quantity"))) %>'
                                SkinID="ShortInput" onblur='<%# "SumPrice(\"" + Container.FindControl("TB_Quantity").ClientID + "\",\"" + Container.FindControl("TB_UnitPrice").ClientID + "\",\"" + Container.FindControl("TB_SumPrice").ClientID + "\")" %>'></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVQuantity" runat="server" ControlToValidate="TB_Quantity"
                                ErrorMessage="数量必须填写" Text="*"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="REVnums" runat="server" ControlToValidate="TB_Quantity"
                                ValidationExpression="^\d+$" ErrorMessage="*"></asp:RegularExpressionValidator>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                        <ItemTemplate>
                            <asp:TextBox ID="TB_UnitPrice" runat="server" Font-Bold="true" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString())) %>'
                                SkinID="ShortInput" onblur='<%# "SumPrice(\"" + Container.FindControl("TB_Quantity").ClientID + "\",\"" + Container.FindControl("TB_UnitPrice").ClientID + "\",\"" + Container.FindControl("TB_SumPrice").ClientID + "\")" %>'></asp:TextBox>
                            <asp:Label ID="Lab_p" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString())) %>' Height="0px"
                                Font-Size="XX-Small" Visible="false"></asp:Label>
                            <asp:RequiredFieldValidator ID="RFVUnitPrice" runat="server" ControlToValidate="TB_UnitPrice"
                                ErrorMessage="单价必须填写" Text="*" Visible="False"></asp:RequiredFieldValidator>
                            <asp:RegularExpressionValidator ID="REVprince" runat="server" Visible="False"
                                ControlToValidate="TB_UnitPrice" ValidationExpression="^(([0-9]+\.[0-9]*[1-9][0-9]*)|([0-9]*[1-9][0-9]*\.[0-9]+)|([0-9]*[1-9][0-9]*))$"
                                ErrorMessage="*"></asp:RegularExpressionValidator>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="金额" UniqueName="SumPrice">
                        <ItemTemplate>
                            <asp:TextBox ID="TB_SumPrice" runat="server" Font-Bold="true" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Math.Abs(Convert.ToDouble(Eval("Quantity")))*Convert.ToDouble(Eval("UnitPrice"))) %>'
                                SkinID="ShortInput" ReadOnly="true"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    
                    <rad:GridTemplateColumn DataField="Description" HeaderText="是否赠品" UniqueName="Description">
                        <ItemTemplate>
                          
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridButtonColumn HeaderText="操作" CommandName="Delete" Text="删除" ConfirmText="确实要删除吗？"
                        UniqueName="Delete" ButtonType="ImageButton">
                        <HeaderStyle Width="35px" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridButtonColumn>
                    <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                        Visible="False">
                    </rad:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <table class="PanelArea">
            <tr>
                <td class="AreaRowTitle">
                    合计金额：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="Lab_TotalPrice" runat="server"></asp:Label>
                </td>
                <td class="AreaRowTitle">
                    合计数量：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="Lab_TotalNumber" runat="server"></asp:Label>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div id="GoodsPanel" style="background-color: #FFFFFF; width: 100%; height: 200px;
        left: 0px; position: absolute; top: 0px; z-index: -1; visibility: hidden">
        <table width="100%">
            <tr>
                <td style="width: 90px; height: 20px; text-align: right;">
                    选择分类：
                </td>
                <td style="width: 230px;">
                    <rad:RadComboBox ID="RCB_GoodsClass" runat="server" CausesValidation="false" 
                        DataValueField="ClassId" DataTextField="ClassName"
                        Width="220px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td style="width: 90px; text-align: right;">
                    编号搜索：
                </td>
                <td style="width: 230px;">
                    <rad:RadComboBox ID="RCB_Goods" runat="server" CausesValidation="false" 
                        AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="GoodsName" DataValueField="GoodsId"
                         Width="220px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td>
                    <asp:CheckBox ID="cbIncludeUnsale" runat="server" Text="含下架" />
                </td>
                <td style="text-align: right;">
                    <asp:Button ID="Button_SelectGoods" runat="server" Text="添加商品" CssClass="Button"
                        CausesValidation="false" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <input id="CloseGoodsPanel" type="button" value="关闭添加" class="Button" onclick="return HiddenObject('GoodsPanel');" />
                </td>
            </tr>
        </table>
        <rad:RadGrid runat="server" ID="RGSelectGoods" AutoGenerateColumns="true" MasterTableView-CommandItemDisplay="None"
            Height="175px" SkinID="Common" OnNeedDataSource="RGSelectGoods_NeedDataSource"
            AllowPaging="false">
            <ClientSettings>
                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
            </ClientSettings>
            <MasterTableView ClientDataKeyNames="GoodsId" DataKeyNames="GoodsId,IsRealGoods">
                <CommandItemStyle HorizontalAlign="Right" Height="0px" />
                <Columns>
                    <rad:GridTemplateColumn UniqueName="CheckBoxColumn">
                        <ItemTemplate>
                            <asp:CheckBox ID="CheckGoods" runat="server" />
                            <asp:HiddenField ID="ItemFieldValue" runat="server" />
                        </ItemTemplate>
                        <HeaderStyle Width="30px" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
       
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

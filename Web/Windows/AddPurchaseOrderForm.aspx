<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddPurchaseOrderForm.aspx.cs" Inherits="ERP.UI.Web.Windows.AddPurchaseOrderForm" %>
<%@ Import Namespace="ERP.Enum" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
        
    </rad:RadScriptBlock>

    <div id="GoodsPanel" style="background-color: #FFFFFF; width: 100%; height: 205px;left: 0px; position: absolute; top: 0px;">
        <table width="100%">
            <tr>
                <td style="width: 90px; height: 25px; text-align: left;">
                    选择分类:
                </td>
                <td style="width: 230px;">
                    <rad:RadComboBox ID="RCB_GoodsClass" runat="server"  Width="220px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td style="width: 90px; text-align: left;">
                    编号搜索:
                </td>
                <td style="width: 230px;">
                    <rad:RadComboBox ID="RCB_Goods" runat="server" Width="220px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td width="80px" align="left">
                    是否赠品:
                </td>
                <td width="150px" align="left">
                    <asp:CheckBox ID='Cbx_GoodsType' runat='server' />
                </td>
                <td style="text-align: right;">
                    <asp:Button ID="Button_SelectGoods" Text="添加商品" CssClass="Button"
                        CausesValidation="false" runat="server" />
                </td>
            </tr>
        </table>
        <rad:RadGrid runat="server" ID="RGSelectGoods" AutoGenerateColumns="true" MasterTableView-CommandItemDisplay="None"
            Height="175px" SkinID="Common" OnNeedDataSource="RGSelectGoods_NeedDataSource"
            AllowPaging="false" >
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

        <asp:HiddenField ID="HFSonGoods" runat="server" />
        <table border="0" width="100%">
            <tr>
                <td align="right">
                    仓 储:<rad:RadComboBox ID="RCB_Warehouse" runat="server" Width="120px" Height="200px" >
                    </rad:RadComboBox>
                    <rad:RadComboBox ID="RCB_Filiale" runat="server"  Width="120px" Height="200px">
                    </rad:RadComboBox>
                    &nbsp;到货日期:<rad:RadDateTimePicker ID="RDP_ArrivalTime" runat="server" Width="150px" EnableTyping="False" ></rad:RadDateTimePicker>
                    &nbsp;创建人:<asp:Label runat="server" ID="LB_Director"></asp:Label>
                    &nbsp;<asp:Button ID="btnSubmit" runat="server" Text="生成采购单" CssClass="Button"  />
                </td>
                <td width="55" align="left">
                    <asp:Button ID="btnDelete" runat="server" Text="删  除" CssClass="Button"/>
                </td>
            </tr>
        </table>
        <rad:RadGrid ID="Rgd_PurchasingDetail" OnNeedDataSource="Rgd_PurchasingDetail_OnNeedDataSource"
            AllowMultiRowSelection="true" Width="100%" AllowPaging="false" runat="server">
            <MasterTableView DataKeyNames="GoodsID,GoodsName,Units,GoodsCode,Specification,Price,PurchasingGoodsType"
                ClientDataKeyNames="GoodsID,GoodsCode,Specification">
                <CommandItemTemplate>
                     <asp:LinkButton ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid">
                        <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />刷新
                    </asp:LinkButton>
                </CommandItemTemplate>
                <CommandItemStyle HorizontalAlign="right" Height="25px" />
                <Columns>
                    <rad:GridClientSelectColumn UniqueName="column">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridClientSelectColumn>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名" UniqueName="GoodsName">
                        <HeaderStyle Width="280px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="SixtyDaySales" HeaderText="前第2月销量" UniqueName="SixtyDaySales" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="ThirtyDaySales" HeaderText="前第1月销量" UniqueName="ThirtyDaySales" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="ElevenDaySales" HeaderText="日均销量<br/>(11天)" UniqueName="ElevenDaySales" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="PlanQuantity" HeaderText="采购数量" UniqueName="PlanQuantity">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="是否赠品" DataField="PurchasingGoodsType" UniqueName="PurchasingGoodsType">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>

    <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
        
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

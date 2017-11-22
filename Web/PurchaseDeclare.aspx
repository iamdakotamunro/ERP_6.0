<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="PurchaseDeclare.aspx.cs" Inherits="ERP.UI.Web.PurchaseDeclare" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <div class="StagePanel">
        <table class="StagePanelHead" border="0" style="width: 100%;">
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

        <table class="PanelArea">
            <tr>
                <td>&nbsp;</td>
                <td class="ShortFromRowTitle">
                    仓库：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        MarkFirstMatch="True" ShowToggleImage="True" DataTextField="CompanyName" DataValueField="CompanyId"
                        Height="200px">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    供应商：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Height="200px"
                        DataValueField="CompanyId" DataTextField="CompanyName" AppendDataBoundItems="true">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    关键字：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TextBoxKeys" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="Footer" align="right" colspan="10">
                    <asp:Button ID="Button1" runat="server" Text="查询缺货量" />
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td width="40%">
                </td>
                <td>
                    到货日期:
                </td>
                <td>
                    <rad:RadDateTimePicker ID="RDP_ArrivalTime" runat="server" Width="150px" EnableTyping="False"></rad:RadDateTimePicker>
                </td>
                <td class="Footer" align="right">
                    <asp:ImageButton ID="IB_ExportData" SkinID="PurchaseData" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="RGGoodsDemand" runat="server" SkinID="Common" OnNeedDataSource="GridRGGoodsDemand_NeedDataSource"
        AllowSorting="true" SortingSettings-SortToolTip="点击排序" AutoGenerateColumns="False"
        GroupingEnabled="False">
        <ClientSettings>
        </ClientSettings>
        <MasterTableView DataKeyNames="GoodsId,RealGoodsId,GoodsName,GoodsCode,Specification,Units,CompanyId,CompanyName" 
            ClientDataKeyNames="GoodsId,RealGoodsId,GoodsName,GoodsCode,Specification,Units,CompanyId,CompanyName">
            <CommandItemTemplate>
                <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh">
                </Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>
            <Columns>
                <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName" SortExpression="GoodsName">
                    <HeaderStyle Width="200px" HorizontalAlign="Center" VerticalAlign="Top" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification" SortExpression="Specification">
                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="NeedWarehouseId" HeaderText="缺货仓" UniqueName="NeedWarehouseId">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="NonceWarehouseGoodsStock" HeaderText="库存量" UniqueName="NonceWarehouseGoodsStock" SortExpression="NonceWarehouseGoodsStock">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Demand" HeaderText="缺货量" UniqueName="Demand" SortExpression="Demand">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="采购量">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="CompanyName" HeaderText="供应商" UniqueName="CompanyName" SortExpression="CompanyName">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager ID="StockWindowManager" runat="server" Height="746px" Width="900px" ReloadOnShow="true">
        <Windows>
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

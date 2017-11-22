<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PurchaseOrderEditForm.aspx.cs" Inherits="ERP.UI.Web.Windows.PurchaseOrderEditForm" %>
<%@ Import Namespace="ERP.Enum" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
     <style type="text/css">
        .red {
            color: red;
        }
        
        .normal {
            color: black;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <div style="padding-top: 5px;text-align:center; width: 100%; height: 40px;">
            <asp:Label ID="lab_Purchasing" runat="server"></asp:Label>
            仓储：
            <asp:Label ID="Label2" runat="server"></asp:Label>
            <br/>
            <asp:Label ID="lab_bhdate" runat="server"></asp:Label>
            <asp:Label ID="lab_dhdate" runat="server"></asp:Label>
    </div>
    <%--采购表--%>
    
    <rad:RadGrid ID="Rgd_PurchasingDetail" runat="server" OnNeedDataSource="Rgd_PurchasingDetail_OnNeedDataSource"
        AllowMultiRowSelection="true"
        Width="100%" AllowPaging="true"
        PageSize="40">
        <ClientSettings>
            <Selecting AllowRowSelect="True" />
        </ClientSettings>
        <MasterTableView DataKeyNames="GoodsID,GoodsCode,Specification,PlanQuantity,PurchasingGoodsID,Price,DayAvgStocking,PlanStocking,PurchasingGoodsType,CPrice,CompanyID"
            ShowFooter="true" EditMode="InPlace" ClientDataKeyNames="GoodsID,GoodsCode,Specification"
            ShowGroupFooter="true">
            <CommandItemTemplate>
                <asp:Label ID="Label1" Text="主商品数量:" runat="server"></asp:Label>
                <asp:TextBox ID="tbx_GoodsNum" SkinID="ShortInput"  runat="server"></asp:TextBox>
                <Ibt:ImageButtonControl ID="ibt_GoodsNum" Text="分配" runat="server" SkinType="Insert" CausesValidation="false"  CommandName="Distribution"></Ibt:ImageButtonControl>
                <rad:RadComboBox ID="RCB_AllCommanyList" runat="server"  Height="180px" Width="120px" >
                </rad:RadComboBox>
                <Ibt:ImageButtonControl runat="server" SkinType="Delete" CommandName="UpdateCompany"
                    CausesValidation="false" 
                    ID="IB_UpdateCompanyName" Text="更改供应商">
                </Ibt:ImageButtonControl>
                <Ibt:ImageButtonControl runat="server" SkinType="Delete" CausesValidation="false"
                     ID="IB_AddGoods" OnClientClick="return ShowPurchasingFrom();"
                    Text="添加商品">
                </Ibt:ImageButtonControl>
                <Ibt:ImageButtonControl runat="server" SkinType="Delete" 
                    CausesValidation="false" ID="IB_DeletOrder"  Text="删除">
                </Ibt:ImageButtonControl>
                到货日期：<rad:RadDateTimePicker ID="RDP_ArrivalTime" runat="server" Width="150px" EnableTyping="False" ></rad:RadDateTimePicker>
                <Ibt:ImageButtonControl runat="server" SkinType="Insert" CausesValidation="false"
                     ID="IB_SaveGoods" 
                    Text="保存">
                </Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <Columns>
                <rad:GridClientSelectColumn UniqueName="column">
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridClientSelectColumn>
                <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" ReadOnly="true" UniqueName="GoodsCode">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" ReadOnly="true" UniqueName="Specification">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                
                <rad:GridTemplateColumn DataField="GoodsId" HeaderText="库存数量" UniqueName="GoodsId">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="SixtyDaySales" HeaderText="前第2月销量" ReadOnly="true"
                    UniqueName="SixtyDaySales">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ThirtyDaySales" HeaderText="前第1月销量" ReadOnly="true"
                    UniqueName="ThirtyDaySales">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ElevenDaySales" HeaderText="日均销量<br>(11天)" ReadOnly="true"
                    UniqueName="ElevenDaySales">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="PlanStocking" HeaderText="建议采购数量" ReadOnly="true"
                    UniqueName="PlanStocking">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="PlanQuantity" Aggregate="Sum" FooterText="小计数量:"
                    HeaderText="采购数量" UniqueName="PlanQuantity">
                    <ItemTemplate>
                    </ItemTemplate>
                  
                    <FooterStyle HorizontalAlign="Center" />
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="Price" HeaderText="采购价格" UniqueName="Price">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="CPrice" HeaderText="前价格" ReadOnly="true" UniqueName="CPrice" >
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
               
                <rad:GridBoundColumn DataField="RealityQuantity" HeaderText="实际来货" ReadOnly="true"
                    UniqueName="RealityQuantity">
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="State" HeaderText="采购状态" ReadOnly="true" UniqueName="State">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="PurchasingGoodsType" HeaderText="赠品" ReadOnly="true"
                    UniqueName="PurchasingGoodsType">
                    <ItemTemplate>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadWindowManager ID="StockWindowManager" runat="server" Width="800px" Height="500px">
        <Windows>
        </Windows>
    </rad:RadWindowManager>

    <rad:RadAjaxManager runat="server" ID="RAM">
        
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

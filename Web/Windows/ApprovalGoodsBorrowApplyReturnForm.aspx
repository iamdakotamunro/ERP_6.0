<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalGoodsBorrowApplyReturnForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ApprovalGoodsBorrowApplyReturnForm" %>

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
    <asp:Panel runat="server">
         <div class="StagePanel">
        <table class="PanelArea" style="line-height: 25px">
             <tr>
               <td class="AreaRowTitle">
                    单据编号：
                </td>
               <td class="AreaRowInfo" >
                   <asp:Label ID="lbl_TradeCode" runat="server"></asp:Label>
                </td>
                <td class="AreaRowTitle">
                    供 应 商：
                </td>
               <td class="AreaRowInfo" >
                    <asp:Label ID="lbl_CompanyId" runat="server"></asp:Label>
                </td>
               
            </tr>
           <tr>
                <td class="AreaRowTitle">
                    申请时间：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="lbl_DateCreated" runat="server"></asp:Label>
                </td>
               <td class="AreaRowTitle">
                    操 作 人：
                </td>
                <td class="AreaRowInfo" colspan="3">
                    <asp:Label ID="lbl_Transactor" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">
                    出库仓储：
                </td>
                <td class="AreaRowInfo">
                    <asp:Label ID="lbl_Warehouse" runat="server"></asp:Label>
                </td>
                <td class="AreaRowTitle">
                    物流配送公司：
                </td>
                <td class="AreaRowInfo">
                      <asp:Label ID="lbl_HostingFilialeAuth" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">
                    备注说明：
                </td>
                <td class="AreaRowInfo" colspan="3">
                    <asp:Label ID="lbl_Description" runat="server"></asp:Label>
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
                    <rad:GridTemplateColumn DataField="Quantity" HeaderText="出库数" UniqueName="Quantity">
                        <ItemTemplate>
                            <asp:Label ID="lbl_Quantity" runat="server" Text='<%# Math.Abs(Convert.ToDouble(Eval("Quantity"))) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                        <ItemTemplate>
                             <asp:Label ID="lbl_UnitPrice" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString())) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="金额" UniqueName="SumPrice">
                        <ItemTemplate>
                             <asp:Label ID="lbl_SumPrice" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Math.Abs(Convert.ToDouble(Eval("Quantity")))*Convert.ToDouble(Eval("UnitPrice"))) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    
                    <rad:GridTemplateColumn DataField="Description" HeaderText="是否赠品" UniqueName="Description">
                        <ItemTemplate>
                           <%# Convert.ToDecimal(Eval("UnitPrice"))==0?"赠品":"" %>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
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
                <asp:Button ID="btn_Approval" runat="server" Text="  核准  " OnClick="btnApproval_Click"/>&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Return" runat="server" Text="  核退  " OnClick="btnReturn_Click"/>
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

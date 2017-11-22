<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalGoodsLendApplyForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ApprovalGoodsLendApplyForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .warehousecss div {
            float: left;
            padding-right: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/common.js" type="text/javascript"></script>
        </rad:RadScriptBlock>
        <div runat="server" id="div_Refresh">
            <table class="PanelArea" style="line-height: 25px">
                  <tr>
                    <td style="text-align: right;">单据编号：
                    </td>
                    <td>
                        <asp:Label ID="lbl_TradeCode" runat="server"></asp:Label>
                    </td>
                    <td style="text-align: right;">供 应 商：
                    </td>
                    <td>
                        <asp:Label ID="lbl_CompanyId" runat="server"></asp:Label>
                    </td>
                </tr>
                 <tr>
                    <td style="text-align: right;">申请时间：
                    </td>
                    <td >
                        <asp:Label ID="lbl_DateCreated" runat="server"></asp:Label>
                    </td>
                    <td style="text-align: right;">操 作 人：
                    </td>
                    <td >
                        <asp:Label ID="lbl_Transactor" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td width="15%" style="text-align: right;">出库仓储：
                    </td>
                    <td width="30%" class="warehousecss">
                        <asp:Label ID="lbl_Warehouse" runat="server"></asp:Label>
                    </td>
                    <td width="20%" style="text-align: right;">物流配送公司：
                    </td>
                    <td>
                        <asp:Label ID="lbl_HostingFilialeAuth" runat="server"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">备注说明：
                    </td>
                    <td colspan="3">
                         <asp:Label ID="lbl_Description" runat="server"></asp:Label>
                    </td>
                </tr>
            </table>
            <table class="PanelArea">
                <tr>
                    <td>
                        <table width="100%">
                            <tr>
                                <td width="88px;" style="height: 30px;">当前商品清单：
                                </td>
                                <td>
                                    <asp:Label ID="lbTitle" Text="借出单" Style="font-weight: bold; font-size: 16px;" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right;">
                                    <asp:CheckBox ID="cbStockBack" Text="借出返还单"  AutoPostBack="True" OnCheckedChanged="CbStockBack_CheckedChanged"
                                        runat="server" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:HiddenField ID="HF_Warehouse" runat="server" />
                        <asp:HiddenField ID="HF_StorageType" runat="server" />
                        <asp:HiddenField ID="HF_Filiale" runat="server" />
                        <rad:RadGrid ID="RG_Goods" OnNeedDataSource="RgGoods_NeedDataSource" AllowPaging="false"
                           runat="server">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                                <Selecting AllowRowSelect="True" />
                            </ClientSettings>
                            <MasterTableView DataKeyNames="GoodsId,RealGoodsId,UnitPrice,ShelfType" CommandItemDisplay="None">
                                <Columns>
                                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridTemplateColumn DataField="ShelfType" HeaderText="货架类型" UniqueName="ShelfType">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <%# ShowShelfType(Eval("ShelfType")) %>
                                        </ItemTemplate>
                                    </rad:GridTemplateColumn>
                                    <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                                        <ItemTemplate>
                                            <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString()))%>
                                        </ItemTemplate>
                                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn  HeaderText="库存数" >
                                        <ItemTemplate>
                                             <asp:Label runat="server" ID="lbl_NonceWarehouseGoodsStock" Text='<%#Eval("NonceWarehouseGoodsStock")  %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn DataField="Quantity" HeaderText="借出数" UniqueName="Quantity">
                                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lbl_Quantity" Text='<%# Math.Abs(Convert.ToDouble(Eval("Quantity"))) %>'></asp:Label>
                                           <%-- <asp:TextBox ID="TB_Quantity" runat="server" Font-Bold="true" Text='<%# Eval("Quantity") %>'
                                                Style="width: 55px;" MaxLength="10"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RFVQuantity" runat="server" ControlToValidate="TB_Quantity"
                                                ErrorMessage="数量必须填写" Text="*"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="REVnums" runat="server" ControlToValidate="TB_Quantity"
                                                ValidationExpression="^\d+$" ErrorMessage="*"></asp:RegularExpressionValidator>--%>

                                        </ItemTemplate>
                                    </rad:GridTemplateColumn>
                                   
                                    <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                                        Visible="False">
                                    </rad:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </rad:RadGrid>
                        <rad:RadGrid ID="RG_GoodsBack" OnNeedDataSource="RgGoodsBack_NeedDataSource" OnItemDataBound="RgGoodsBack_OnItemDataBound"
                             AllowPaging="False" Visible="False"
                            runat="server">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                                <Selecting AllowRowSelect="True" />
                            </ClientSettings>
                            <MasterTableView DataKeyNames="GoodsId,RealGoodsId,UnitPrice" CommandItemDisplay="None">
                                <GroupByExpressions>
                                    <rad:GridGroupByExpression>
                                        <GroupByFields>
                                            <rad:GridGroupByField FieldName="GoodsName" HeaderText=" " HeaderValueSeparator=" " />
                                        </GroupByFields>
                                        <SelectFields>
                                            <rad:GridGroupByField FieldName="GoodsName" HeaderText=" " HeaderValueSeparator=" " />
                                        </SelectFields>
                                    </rad:GridGroupByExpression>
                                </GroupByExpressions>
                                <Columns>
                                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridTemplateColumn HeaderText="SKU">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                          <%--  <rad:RadComboBox ID="rcbSpecification" OnSelectedIndexChanged="RcbSpecification_OnSelectedIndexChanged"
                                                AutoPostBack="true" MaxHeight="250px" Visible="False" runat="server">
                                            </rad:RadComboBox>--%>
                                            <asp:Label ID="lbl_Specification" runat="server"></asp:Label>
                                        </ItemTemplate>
                                    </rad:GridTemplateColumn>
                                    <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                     <rad:GridTemplateColumn  HeaderText="库存数" DataField="NonceWarehouseGoodsStock"  UniqueName="NonceWarehouseGoodsStock">
                                        <ItemTemplate>
                                            <asp:Label runat="server" ID="lbl_NonceWarehouseGoodsStock" Text='<%#Eval("NonceWarehouseGoodsStock")  %>'></asp:Label>
                                        </ItemTemplate>
                                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="返还数">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                           <%-- <asp:TextBox ID="TB_Quantity" Text='<%# Eval("Quantity") %>' onblur="HiddenIbtnQuantityOut(this);"
                                                Style="width: 55px;" MaxLength="10" runat="server"></asp:TextBox>
                                            <asp:ImageButton ID="ibtnQuantityOut" Style="visibility: hidden;" CommandName="QuantityOut"
                                                runat="server" SkinID="AffirmImageButton" />--%>
                                            <asp:Label ID="lbl_BackQuantity" runat="server" Text='<%# Math.Abs(Convert.ToDouble(Eval("Quantity"))) %>'></asp:Label>
                                        </ItemTemplate>
                                    </rad:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </rad:RadGrid>
                    </td>
                </tr>
            </table>
            <table class="PanelArea">
                <tr>
                    <td class="AreaRowTitle">合计数量：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label ID="Lab_TotalNumber" runat="server" Text="0"></asp:Label>
                    </td>
                    <td class="AreaRowTitle">合计金额：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label ID="Lab_TotalAmount" runat="server" Text="0"></asp:Label>
                    </td>
                </tr>
            </table>
               <div style="text-align: center; padding-top: 10px;">
                <asp:Button ID="btn_Approval" runat="server" Text="  核准  " OnClick="btnApproval_Click"/>&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Return" runat="server" Text="  核退  " OnClick="btnReturn_Click"/>
            </div>
        </div>
        
        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_StorageAuth"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RCB_HostingFilialeAuth"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                 <rad:AjaxSetting AjaxControlID="RCB_StorageAuth">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_HostingFilialeAuth"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                  <rad:AjaxSetting AjaxControlID="cbStockBack">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="cbStockBack"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="lbTitle" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="btnShowAddGoods"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RG_Goods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RG_GoodsBack" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                 <rad:AjaxSetting AjaxControlID="RCB_GoodsClass">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGSelectGoods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                 <rad:AjaxSetting AjaxControlID="RCB_Goods">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGSelectGoods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="Btn_SelectGoods">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RG_Goods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                 <rad:AjaxSetting AjaxControlID="btnSave">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="div_Refresh" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

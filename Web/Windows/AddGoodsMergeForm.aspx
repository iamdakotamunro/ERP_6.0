<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddGoodsMergeForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.AddGoodsMergeForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <div style="padding: 10px; padding-top: 20px;">
        仓库：
        <rad:RadComboBox ID="RCB_Warehouse" Width="150" runat="server" AutoPostBack="True"
            OnSelectedIndexChanged="OnSelectedIndexChanged_Warehouse">
            <Items>
                <rad:RadComboBoxItem Text="请选择仓库" Value="00000000-0000-0000-0000-000000000000" />
            </Items>
        </rad:RadComboBox>
        <br />
        <br />
        <fieldset style="margin-left: 0px;">
            <legend style="font-style: oblique; font-weight: bold; font-size: 14px;">组合子商品列表</legend>
            <br />
            <table width="650px;" style="background-color: #f7f7f7; border: solid 1px #cccccc;
                border-spacing: 0px; margin-left: 10px;">
                <tr>
                    <td style="width: 40px;">
                        商品：
                    </td>
                    <td>
                        <rad:RadComboBox ID="RCB_NewGoods" runat="server" CausesValidation="false" AllowCustomText="True"
                            EnableLoadOnDemand="True" DataTextField="GoodsName" DataValueField="GoodsId"
                            OnSelectedIndexChanged="Rcb_NewGoods_SelectedIndexChanged" AutoPostBack="True"
                            Width="220px" Height="100px">
                        </rad:RadComboBox>
                    </td>
                    <td style="width: 40px;">
                        SKU：
                    </td>
                    <td>
                        <rad:RadComboBox runat="server" ID="RcbIuminosity" DataTextField="Specification"
                            DataValueField="RealGoodsId" Width="150">
                        </rad:RadComboBox>
                    </td>
                    <td>
                        <asp:Button ID="btnAddGoods" runat="server" Text="添加" OnClick="BtnAddGoods_Click"
                            Width="50" />
                    </td>
                </tr>
            </table>
            <br />
        </fieldset>
        <rad:RadGrid ID="Rgd_SelectGoods" OnNeedDataSource="RGSelectGoods_NeedDataSource"
            OnItemCommand="RGSelectGoods_ItemCommand" AllowPaging="False" runat="server">
            <MasterTableView DataKeyNames="RealGoodsId" ClientDataKeyNames="RealGoodsId" CommandItemDisplay="None">
                <Columns>
                    <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                        Display="False">
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="RealGoodsId" HeaderText="RealGoodsId" UniqueName="RealGoodsId"
                        Display="False">
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="UnitPrice" HeaderText="UnitPrice" UniqueName="UnitPrice"
                        Display="False">
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsName" ReadOnly="true" HeaderText="商品" UniqueName="GoodsName">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Left" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Specification" ReadOnly="true" HeaderText="SKU" UniqueName="Specification">
                        <HeaderStyle HorizontalAlign="Center" Width="150px" />
                        <ItemStyle HorizontalAlign="Left" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsStock" HeaderText="可用库存" UniqueName="GoodsStock">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="数量" UniqueName="Quantity">
                        <ItemTemplate>
                            <asp:Button ID="btnSubtract" runat="server" Text="-" CommandName="Subtract" />
                            <asp:TextBox ID="tbQuantity" Text='<%# Eval("Quantity")%>' ToolTip="数量" MaxLength="10"
                                ReadOnly="True" Width="70" runat="server"></asp:TextBox>
                            <asp:Button ID="btnPlus" runat="server" Text="+" CommandName="Plus" />
                        </ItemTemplate>
                        <HeaderStyle Width="150" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="操作">
                        <ItemTemplate>
                            <asp:LinkButton ID="LB_Delete" runat="server" CommandName="Delete">
                                <asp:Image ID="IB_Delete" SkinID="CancelImageButton" runat="server" ImageAlign="AbsMiddle"
                                    BorderStyle="None" />
                            </asp:LinkButton>
                        </ItemTemplate>
                        <HeaderStyle Width="50" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <img src="../App_Themes/default/Images/next.png" alt="目标组合商品" style="margin-left: 40px;"/>
        <br />
        <fieldset>
            <legend style="font-style: oblique; font-weight: bold; font-size: 14px;">目标组合商品</legend>
            <br />
            <table width="650px;" style="background-color: #f7f7f7; border: solid 1px #cccccc;
                border-spacing: 0px; margin-left: 10px;">
                <tr>
                    <td style="width: 60px;">
                        组合商品：
                    </td>
                    <td>
                        <rad:RadComboBox ID="RCB_OldGoods" runat="server" CausesValidation="false" AllowCustomText="True"
                            EnableLoadOnDemand="True" DataTextField="GoodsName" DataValueField="GoodsId"
                            Width="220px" Height="100px" AutoPostBack="True" OnSelectedIndexChanged="Rcb_OldGoods_SelectedIndexChanged">
                        </rad:RadComboBox>
                    </td>
                    <td style="width: 40px;">
                        SKU：
                    </td>
                    <td>
                        <rad:RadComboBox runat="server" ID="RCB_OldGoodsSpecification" DataTextField="Specification"
                            DataValueField="RealGoodsId" AutoPostBack="True" OnSelectedIndexChanged="RcbOldGoodsSpecification_SelectedIndexChanged"
                            Width="150">
                        </rad:RadComboBox>
                    </td>
                    <td style="width: 40px;">
                        数量：
                    </td>
                    <td>
                        <asp:HiddenField ID="hfOldGoodsQuantity" runat="server" />
                        <asp:TextBox ID="tbOldGoodsQuantity" MaxLength="8" ToolTip="数量" AutoPostBack="True"
                            OnTextChanged="TbOldGoodsQuantity_OnTextChanged" onkeyup="this.value=this.value.replace(/-?\D/g,'')"
                            Width="50" runat="server"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="width: 60px;">
                        可用库存：
                    </td>
                    <td colspan="5">
                        <asp:Label ID="lbQuantity" runat="server" Text="0"></asp:Label>
                    </td>
                </tr>
            </table>
            <br />
        </fieldset>
        <asp:Panel ID="Panel_MergeSplit" runat="server">
            <table width="100%">
                <tr>
                    <td style="text-align: right;">
                        <asp:Button ID="btnMerge" runat="server" Text="组合" OnClick="BtnMerge_Click" Width="80" />
                    </td>
                    <td style="text-align: left; padding-left: 20px;">
                        <input id="btnCancel" value="取消" onclick="return CancelWindow()" style="width: 80px;
                            height: 24px;" type="button" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
    </div>
    <rad:RadAjaxManager runat="server" ID="RAM">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="tbOldGoodsQuantity">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="hfOldGoodsQuantity" />
                    <rad:AjaxUpdatedControl ControlID="tbOldGoodsQuantity" />
                    <rad:AjaxUpdatedControl ControlID="Rgd_SelectGoods" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnAddGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_SelectGoods" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Rgd_SelectGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_SelectGoods" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnSplit">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_MergeSplit" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnMerge">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_MergeSplit" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_OldGoodsSpecification" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="lbQuantity" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RCB_NewGoods" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RcbIuminosity" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_OldGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_OldGoods" />
                    <rad:AjaxUpdatedControl ControlID="lbQuantity" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RCB_OldGoodsSpecification" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_OldGoodsSpecification">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="lbQuantity" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_NewGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LbRealQuantity" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RcbIuminosity" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbIuminosity">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LbRealQuantity" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

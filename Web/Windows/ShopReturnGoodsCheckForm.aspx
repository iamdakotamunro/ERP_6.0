<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopReturnGoodsCheckForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShopReturnGoodsCheckForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
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
        <asp:Panel ID="PanelStagePanel" runat="server">
            <table class="StagePanel">
                <tr>
                    <td colspan="2" style="vertical-align: top;">
                        <rad:RadGrid ID="RgReturnGoodsList" runat="server" SkinID="Common"
                            OnNeedDataSource="RgReturnGoodsListNeedDataSource" OnDetailTableDataBind="RgReturnGoodsListDetailTableDataBind"
                            OnItemDataBound="RgReturnGoodsListItemDataBound">
                            <MasterTableView CommandItemDisplay="None" DataKeyNames="GoodsId,RealGoodsId,Quantity,ShelfType">
                                <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                                <Columns>
                                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="Quantity" HeaderText="购买数量" UniqueName="Quantity">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridTemplateColumn HeaderText="退回数量" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="TbpReturnCount" Text='<%# Eval("ReturnCount") %>'
                                                Enabled="False" Width="50px"
                                                AutoPostBack="True" OnTextChanged="TbReturnCountTextChanged"></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="处理结果" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <%# GetState(Eval("ReturnType").ToString()) %>
                                        </ItemTemplate>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="货架类型" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                        <ItemTemplate>
                                            <asp:DropDownList ID="DdlShelfType" runat="server" SkinID="LongDropDown" AutoPostBack="True" Width="80px" OnSelectedIndexChanged="DdlShelfTypeSelectedChanged"></asp:DropDownList>
                                        </ItemTemplate>
                                    </rad:GridTemplateColumn>
                                    <rad:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="Id" Visible="False"></rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId" Visible="False" />
                                    <rad:GridBoundColumn DataField="ReturnType" HeaderText="ReturnType" UniqueName="ReturnType" Visible="False" />
                                    <rad:GridBoundColumn DataField="DamageCount" HeaderText="DamageCount" UniqueName="DamageCount" Visible="False" />
                                </Columns>
                                <DetailTables>
                                    <rad:GridTableView runat="server" DataKeyNames="GoodsId,RealGoodsId,Quantity,ShelfType" DataMember="Details"
                                        CommandItemDisplay="None" Width="100%" NoDetailRecordsText="">
                                        <Columns>
                                            <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </rad:GridBoundColumn>
                                            <rad:GridBoundColumn DataField="Quantity" HeaderText="购买数量" UniqueName="Quantity">
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </rad:GridBoundColumn>
                                            <rad:GridTemplateColumn HeaderText="退回数量" UniqueName="ReturnCount">
                                                <ItemTemplate>
                                                    <asp:TextBox runat="server" ID="TbReturnCount" Text='<%# Eval("ReturnCount") %>' MaxLength="3"
                                                        ToolTip='<%# Eval("Quantity") %>' onKeyup="this.value=this.value.replace(/-?\D/g,'')"
                                                        Width="30" AutoPostBack="True" Enabled="False"
                                                        OnTextChanged="TbReturnCountTextChanged"></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </rad:GridTemplateColumn>
                                            <rad:GridTemplateColumn HeaderText="货架类型" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="DdlcShelfType" runat="server" SkinID="LongDropDown" AutoPostBack="True" Width="80px" OnSelectedIndexChanged="DdlcShelfTypeSelectedChanged"></asp:DropDownList>
                                                </ItemTemplate>
                                            </rad:GridTemplateColumn>
                                            <rad:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="Id" Visible="False"></rad:GridBoundColumn>
                                            <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId" Visible="False" />
                                            <rad:GridBoundColumn DataField="ReturnType" HeaderText="ReturnType" UniqueName="ReturnType" Visible="False" />
                                            <rad:GridBoundColumn DataField="DamageCount" HeaderText="DamageCount" UniqueName="DamageCount" Visible="False" />
                                        </Columns>
                                    </rad:GridTableView>
                                </DetailTables>
                            </MasterTableView>
                        </rad:RadGrid>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>检查结果：</td>
                    <td>
                        <asp:RadioButton ID="RbPass" Text="通过" Checked="True" AutoPostBack="True"
                            OnCheckedChanged="RbtnCheckedChanged" runat="server" GroupName="CheckState" />
                        <asp:RadioButton ID="RbBack" Text="退回" AutoPostBack="True" OnCheckedChanged="RbtnCheckedChanged"
                            runat="server" GroupName="CheckState" />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>退货仓库：</td>
                    <td>
                        <rad:RadComboBox ID="DdlWarehouse" AllowCustomText="True" runat="server" Width="220px"
                            OnSelectedIndexChanged="DdlWarehouseSelectedChanged" AutoPostBack="True">
                        </rad:RadComboBox>
                        <rad:RadComboBox ID="rcb_StorageType" runat="server" Width="55px">
                            <Items>
                                <rad:RadComboBoxItem runat="server" Value="3" Text="售后区" />
                            </Items>
                        </rad:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td>拒绝原因：</td>
                    <td style="vertical-align: text-top; padding-top: 0;">
                        <rad:RadTextBox ID="RtxReturnReason" runat="server" TextMode="MultiLine" Rows="4" Columns="100" />
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="DivReStart" runat="server">
                            重启原因：<rad:RadTextBox ID="RtxReStart" runat="server" TextMode="MultiLine" Rows="4" Columns="100" />
                        </div>
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="2" style="text-align: center;">
                        <asp:Button ID="BtnSubmit" runat="server" Text="提交" OnClick="BtnSubmitClick" />&nbsp;&nbsp;
                <asp:Button ID="BtnCance" runat="server" Text="取消" OnClick="BtnCancelOnClick" />
                    </td>
                </tr>
            </table>
        </asp:Panel>
        <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="BtnSubmit">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="PanelStagePanel" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RgReturnGoodsList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RgReturnGoodsList" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="StockStatementForm.aspx.cs" Inherits="ERP.UI.Web.Windows.StockStatementForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="radscr" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>

    <div>
        <table class="PanelArea">
            <tr>
                <td class="AreaEditFromRowInfo" align="left">
                    <asp:Label ID="lab_Company" runat="server" Text=""></asp:Label>
                </td>
                <td class="AreaEditFromRowInfo" align="right">
                    到货说明:
                    <asp:TextBox ID="tbx_AllStatement" SkinID="LongInput" runat="server"></asp:TextBox>
                </td>
                <td align="right">
                    <asp:Button ID="Bt_StockStatement" runat="server" Text="保存" CssClass="Button" CausesValidation="false"
                        OnClick="Bt_StockStatement_Click" />
                </td>
            </tr>
        </table>

        <rad:RadGrid ID="Rgd_StockStatement" OnNeedDataSource="Rgd_StockStatement_NeedDataSource" PageSize="10" runat="server">
            <MasterTableView CommandItemDisplay="None" DataKeyNames="GoodsId">
                <Columns>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名" UniqueName="GoodsName">
                        <HeaderStyle Width="140px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Specification" HeaderText="商品SKU" UniqueName="Specification">
                        <HeaderStyle Width="140px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="GoodsStatement" HeaderText="到货说明" UniqueName="GoodsStatement">
                        <ItemTemplate>
                            <asp:TextBox ID="tbx_Statement" SkinID="LongInput" Text='' runat="server"></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="GoodsStatement" HeaderText="到货说明" UniqueName="GoodsStatement">
                        <ItemTemplate>
                            <asp:Label ID="lab_Statement" SkinID="LongInput" Text='<%#Eval("GoodsStatement") %>' runat="server"></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="left" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>

        <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="Bt_StockStatement">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Rgd_StockStatement" LoadingPanelID="loading">
                        </rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </div>
    </form>
</body>
</html>

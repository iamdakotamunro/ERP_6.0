<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowNeedGoodsForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShowNeedGoodsForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>

    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="ControlTools">
                <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData"  OnClick="NeedGoods_ExportOnClick" />
            </td>
        </tr>
    </table>

    <rad:RadGrid ID="rgd_needGoods" runat="server" OnNeedDataSource="Rgd_NeedGoods_NeedDataSource">
        <MasterTableView CommandItemDisplay="None">
            <Columns>
                <rad:GridBoundColumn HeaderText="商品编号" DataField="GoodsCode" UniqueName="GoodsCode">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn HeaderText="商品名" DataField="GoodsName" UniqueName="GoodsName">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn HeaderText="SKU" DataField="Specification" UniqueName="Specification">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="缺货数量" DataField="Quantity" UniqueName="Quantity">
                    <ItemTemplate>
                        <%# Math.Abs(Convert.ToDouble(Eval("Quantity")))%>
                    </ItemTemplate>
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="rgd_needGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgd_needGoods" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

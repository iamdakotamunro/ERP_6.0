<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowNeedOrdersForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShowNeedOrdersForm" %>

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
                <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="NeedGoods_ExportOnClick" />
            </td>
        </tr>
    </table>

    <rad:RadGrid ID="rgd_needGoods" runat="server" OnNeedDataSource="Rgd_NeedOrders_NeedDataSource">
        <MasterTableView CommandItemDisplay="None">
            <Columns>
                <rad:GridBoundColumn HeaderText="订单号" DataField="OrderNo" UniqueName="OrderNo">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn HeaderText="收货人" DataField="Consignee" UniqueName="Consignee">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn HeaderText="有效时间" DataField="EffectiveTime" UniqueName="EffectiveTime">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn HeaderText="订单来源" DataField="Memo" UniqueName="Memo">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <%--<rad:GridTemplateColumn HeaderText="订单来源" UniqueName="SaleFilialeId">
                    <ItemTemplate>
                            <%# GetSaleFilialeName(new Guid(Eval("SaleFilialeId").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>--%>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    </form>
</body>
</html>

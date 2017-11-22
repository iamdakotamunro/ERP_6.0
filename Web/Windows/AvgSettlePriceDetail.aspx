<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AvgSettlePriceDetail.aspx.cs" Inherits="ERP.UI.Web.Windows.AvgSettlePriceDetail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding: 10px 0 10px 0;">
        商品名称：<b><asp:Literal ID="Lit_GoodsName" runat="server"></asp:Literal></b>
        </div>
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadGrid ID="RG_AvgSettlePriceDetail" runat="server" PageSize="10" OnNeedDataSource="RG_AvgSettlePriceDetail_NeedDataSource">
            <MasterTableView>
                <CommandItemTemplate>
                </CommandItemTemplate>
                <CommandItemStyle Height="0px" />
                <Columns>
                    <rad:GridBoundColumn DataField="StatisticMonth" HeaderText="年月" DataFormatString="{0:yyyy-MM}">
                        <HeaderStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="UnitPrice" HeaderText="结算价" DataFormatString="{0:F2}">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle Width="60px" HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
         <rad:RadAjaxManager ID="RAM" runat="server"></rad:RadAjaxManager>
    </form>
</body>
</html>

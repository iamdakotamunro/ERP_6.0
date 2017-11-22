<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintGoodsStockOutDetail.aspx.cs"
    Inherits="ERP.UI.Web.Windows.PrintGoodsStockOutDetail" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl" Src="~/UserControl/ImageButtonControl.ascx" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        th
        {
            font-size: 10px !important;
        }
        td
        {
            font-size: 10px !important;
        }
        .tdTitle
        {
            text-align: right;
            width: 20%;
            height: 12px;
        }
        .tdContent
        {
            width: 30%;
        }
    </style>
    <style media="print" type="text/css">
        .noprint
        {
            visibility: hidden;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RadScriptBlock1" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script language="javascript" type="text/javascript">
            function PrintOut() {
                window.print();
                return false;
            }
        </script>
    </rad:RadScriptBlock>
    <table width="100%" class="noprint">
        <tr>
            <td class="ControlTools">
                <Ibt:ImageButtonControl ID="LBPrintOut" Text="打印" OnClientClick="return PrintOut()"
                    SkinType="Print" runat="server"></Ibt:ImageButtonControl>
            </td>
        </tr>
    </table>
    <table width="100%">
        <tr>
            <td colspan="4" style="font-size: 14px; text-align: center; height: 22px;">
                <%= "出库单 ： " + GetFilialeName(StorageRecordInfo.FilialeId) + "-" + EnumAttribute.GetKeyName((StorageType)StorageRecordInfo.StockType)%>
            </td>
        </tr>
        <tr>
            <td class="tdTitle" runat="server" id="tb_Filiale">
                收货：
            </td>
            <td class="tdContent" runat="server" id="tb_FilialeName">
                <%=GetFilialeNameA(StorageRecordInfo.RelevanceFilialeId)%>
            </td>
            <td class="tdTitle" runat="server" id="tb_Company">
                供应商：
            </td>
            <td class="tdContent" runat="server" id="tb_CompanyName">
                <%=GetCompanyName(StorageRecordInfo.ThirdCompanyID)%>
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                出库仓库：
            </td>
            <td class="tdContent">
                <%=StorageRecordInfo.WarehouseName%>
            </td>
            <td class="tdTitle">
                出库：
            </td>
            <td class="tdContent">
                <%=GetFilialeName(StorageRecordInfo.FilialeId)%>
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                录单时间：
            </td>
            <td class="tdContent">
                <%=StorageRecordInfo.DateCreated.ToString()%>
            </td>
            <td class="tdTitle">
                单据编号：
            </td>
            <td class="tdContent">
                <%=StorageRecordInfo.TradeCode%>
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                经办人：
            </td>
            <td class="tdContent">
                <%=StorageRecordInfo.Transactor%>
            </td>
            <td class="tdTitle">
                原始编号：
            </td>
            <td class="tdContent">
                <%=StorageRecordInfo.LinkTradeCode%>
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                打印时间：
            </td>
            <td class="tdContent">
                <%=GetPrintTime()%>
            </td>
            <td class="tdTitle">
                <asp:Label runat="server" ID="LB_Titie">单据总额：</asp:Label>
            </td>
            <td class="tdContent">
                <asp:Label runat="server" ID="LB_SumPrice"><%=Math.Abs(StorageRecordInfo.AccountReceivable)%></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                备注说明：
            </td>
            <td colspan="3">
                <%=StorageRecordInfo.Description%>
            </td>
        </tr>
        <tr>
            <td colspan="4" style="height: 5px;">
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RGGoods" runat="server" SkinID="Common" AllowPaging="False" OnNeedDataSource="RgGoodsNeedDataSource"
        ShowFooter="true">
        <MasterTableView CommandItemDisplay="None">
            <Columns>
                <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode">
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn HeaderText="批号" DataField="BatchNo">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Units" HeaderText="单位" UniqueName="Units">
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Quantity" HeaderText="数量" UniqueName="Quantity" Aggregate="Sum"
                    FooterText="总计：">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="金额" UniqueName="TotalPrice">
                    <ItemTemplate>
                        <%#  Convert.ToInt32(Eval("Quantity"))*Convert.ToDecimal(Eval("UnitPrice")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ShelfNo" HeaderText="货位号" UniqueName="ShelfNo">
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="赠品" UniqueName="type">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("UnitPrice"))==0?"是":" " %>
                    </ItemTemplate>
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Description" HeaderText="备注" UniqueName="Description"
                    Visible="False">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <asp:Label ID="lbHtml" runat="server"></asp:Label>
    <br />
    </form>
</body>
</html>

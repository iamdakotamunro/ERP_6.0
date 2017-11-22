<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintStorageRecordApplyInDetail.aspx.cs" Inherits="ERP.UI.Web.Windows.PrintStorageRecordApplyInDetail" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl" Src="~/UserControl/ImageButtonControl.ascx" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <style type="text/css">
        th
        {
            font-size: 10px;
        }
        td
        {
            font-size: 10px;
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
            function PrintIn() {
                window.print();
                return false;
            }
        </script>
    </rad:RadScriptBlock>
    <table width="100%" class="noprint">
        <tr>
            <td class="ControlTools">
                <Ibt:ImageButtonControl ID="LBPrintIn" Text="打印" OnClientClick="return PrintIn()"
                    SkinType="Print" runat="server"></Ibt:ImageButtonControl>
            </td>
        </tr>
    </table>
    <table width="100%" style="line-height: 25px;">
        <tr>
            <td colspan="4" style="font-size: 14px; text-align: center; height: 22px;">
                <%= "入库单 ： "+GetFilialeName(StorageRecordInfo.WarehouseId,StorageRecordInfo.StorageType,StorageRecordInfo.FilialeId)+"-"+EnumAttribute.GetKeyName((StorageRecordType)StorageRecordInfo.StockType)%>
            </td>
        </tr>
        <tr>
             <td class="tdTitle">
                单据编号：
            </td>
            <td class="tdContent">
                <%=StorageRecordInfo.TradeCode%>
            </td>
            <td class="tdTitle">
                供应商：
            </td>
            <td class="tdContent">
                <%=GetCompanyName(StorageRecordInfo.ThirdCompanyID)%>
            </td>
        </tr>
         <tr>
            <td class="tdTitle">
                申请时间：
            </td>
            <td class="tdContent">
                <%=StorageRecordInfo.DateCreated.ToString()%>
            </td>
            <td class="tdTitle">
                操作人：
            </td>
            <td class="tdContent">
                <%=StorageRecordInfo.Transactor%>
            </td>
           
        </tr>
        <tr>
            <td class="tdTitle">
                入库仓储：
            </td>
            <td class="tdContent">
                <%=GetWarehouse(StorageRecordInfo.WarehouseId,StorageRecordInfo.StorageType)%>
            </td>
            <td class="tdTitle">
                物流配送公司：
            </td>
            <td class="tdContent">
                <%=GetFilialeName(StorageRecordInfo.WarehouseId,StorageRecordInfo.StorageType,StorageRecordInfo.FilialeId)%>
            </td>
        </tr>
        <tr>
             <td class="tdTitle">
                原始编号：
            </td>
            <td class="tdContent">
                <%=StorageRecordInfo.LinkTradeCode%>
            </td>
            <td class="tdTitle">
                打印时间：
            </td>
            <td class="tdContent">
                <%=GetPrintTime()%>
            </td>
        </tr>
        <tr>
             <td class="tdTitle" runat="server" id="td_TotalTitel">
                单据总额：
            </td>
            <td colspan="3" class="tdContent" runat="server" id="td_TotalPrice">
                <%=Math.Abs(StorageRecordInfo.AccountReceivable)%>
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
                <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                    <ItemStyle HorizontalAlign="Center" />
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
               
                <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
               
                <rad:GridBoundColumn DataField="Quantity" HeaderText="入库数" UniqueName="Quantity" Aggregate="Sum"
                    FooterText="总计：">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                 <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="是否赠品" UniqueName="type">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("UnitPrice"))==0?"赠品":"" %>
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <br />
    </form>
</body>
</html>

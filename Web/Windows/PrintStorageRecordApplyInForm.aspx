<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PrintStorageRecordApplyInForm.aspx.cs" Inherits="ERP.UI.Web.Windows.PrintStorageRecordApplyInForm" %>
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
    <table width="100%" style="line-height: 20px;">
        <tr>
            <td colspan="4" style="font-size: 14px; text-align: center; height: 22px;">
             入库单：可得光学-采购进货
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                单据编号：
            </td>
            <td class="tdContent">
               单号1
            </td>
            <td class="tdTitle">
                供应商：
            </td>
            <td class="tdContent">
                单位111
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                申请时间：
            </td>
            <td class="tdContent">
                2015/9/16 10:11:00
            </td>
            <td class="tdTitle">
                操作人：
            </td>
            <td class="tdContent">
               张三
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                入库仓储：
            </td>
            <td class="tdContent">
                松江仓整件区
            </td>
            <td class="tdTitle">
                物流配送公司：
            </td>
            <td class="tdContent">
                可得
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                备注说明：
            </td>
            <td class="tdContent">
                ××××××××××××××××
            </td>
            <td class="tdTitle">
                原始单号：
            </td>
            <td class="tdContent">
                pj63696552
            </td>
        </tr>
        <tr>
            <td class="tdTitle">
                打印时间：
            </td>
            <td class="tdContent" colspan="3">
                2015/9/16 10:11:00
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
                <rad:GridBoundColumn HeaderText="批号" DataField="BatchNo">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Units" HeaderText="单位" UniqueName="Units">
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Quantity" HeaderText="单据数量" UniqueName="Quantity" Aggregate="Sum"
                    FooterText="总计：">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FooterStyle HorizontalAlign="Center"></FooterStyle>
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="UnitPrice" HeaderText="计量单位" UniqueName="UnitPrice">
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                
                <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="是否赠品" UniqueName="type">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("UnitPrice"))==0?"赠品":"" %>
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Description" HeaderText="备注" UniqueName="Description">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
              
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <br />
    </form>
</body>
</html>

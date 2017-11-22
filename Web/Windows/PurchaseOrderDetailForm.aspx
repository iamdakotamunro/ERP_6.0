<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PurchaseOrderDetailForm.aspx.cs" Inherits="ERP.UI.Web.Windows.PurchaseOrderDetailForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title>采购单</title>
     <style media="print" type="text/css">
        .noprint
        {
            visibility: hidden;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script>
            function Print() {
                window.print();
            }
        </script>
    </rad:RadScriptBlock>

    <table class="noprint" width="100%">
        <tr class="noprint">
            <td class="ShortFromRowTitle" width="25%" class="noprint">
                <asp:DropDownList ID="RCB_ExcelTemp" runat="server" DataTextField="TemplateName"
                    DataValueField="TempId" SkinID="ShortDropDown" AppendDataBoundItems="true" 
                    Width="150px" >
                    <asp:ListItem Selected="00000000-0000-0000-0000-000000000000" Value="00000000-0000-0000-0000-000000000000">选择你预先设好的模板</asp:ListItem>
                </asp:DropDownList>
            </td>
            <td class="shortfromrowtitle" width="20%" class="noprint">
                <asp:CheckBox ID="Cbx_PriceShow" runat="server" 
                    Text="价格显示" />
                <asp:ImageButton ID="Btn_Print" runat="server" OnClientClick="return Print()" ImageUrl="~/App_Themes/print2.gif" />
            </td>
            <td class="shortfromrowtitle" width="25%" class="noprint">
                <asp:ImageButton ID="ImageButton1" runat="server" />
            </td>
        </tr>
    </table>
    <div id="print" style="float: none;">
        <table cellpadding="0" cellspacing="0" border="0" width="100%">
            <tr>
                <td width="12%">
                    <b>To</b>
                </td>
                <td width="15%">
                    <asp:Label ID="lab_company" runat="server" Text=" "></asp:Label>
                </td>
                <td width="12%">
                </td>
                <td width="10%">
                </td>
                <td rowspan="6" align="center">
                    <asp:Image ID="img_keede" Width="200px" Height="120px" ImageUrl="" runat="server" />
                </td>
            </tr>
            <tr>
                <td>
                    <b>订货单位</b>
                </td>
                <td>
                    <asp:Label ID="lab_Customer" runat="server" Text=" "></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <b>收货人</b>
                </td>
                <td>
                    <asp:Label ID="lab_Shipper" runat="server" Text=" "></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <b>收货地址</b>
                </td>
                <td>
                    <asp:Label ID="lab_address" runat="server" Text=" "></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <b>订货联系人</b>
                </td>
                <td>
                    <asp:Label ID="lab_ContactPerson" runat="server" Text=" "></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <b>订货时间</b>
                </td>
                <td>
                    <asp:Label ID="lab_Time" runat="server" Text=" "></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <b>到货日期</b>
                </td>
                <td>
                    <asp:Label ID="lab_ArrivalTime" runat="server" ></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <b>区域厂商联系人</b>
                </td>
                <td>
                    <asp:Label ID="Label1" runat="server" ></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <b>区域厂商联系电话</b>
                </td>
                <td>
                    <asp:Label ID="Label2" runat="server" ></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <b>负责人</b>
                </td>
                <td>
                    <asp:Label ID="lab_Director" runat="server" ></asp:Label>
                </td>
                <td>
                </td>
                <td>
                </td>
            </tr>
            <tr>
                <td>
                    <b>备注</b>
                </td>
                <td>
                    <asp:Label ID="lab_des" runat="server" Text=" "></asp:Label>
                </td>
                <td>
                    <b>采购单</b>
                </td>
                <td>
                    <asp:Label ID="lab_purNo" runat="server" Text=" "></asp:Label>
                </td>
            </tr>
        </table>
    </div>
    <p>
        <div style="float: left; width: 99%">
            <rad:RadGrid ID="Rgd_GoodsPrint" runat="server" Width="99%" GridLines="None" OnNeedDataSource="Rgd_GoodsPrint_OnNeedDataSource"
                 AllowPaging="false" SkinID="Common">
                <ClientSettings AllowDragToGroup="True" />
                <MasterTableView CommandItemDisplay="None" DataKeyNames="PlanQuantity" ShowGroupFooter="true"
                    GridLines="None" ShowFooter="true">
                    <Columns>
                        <rad:GridTemplateColumn DataField="" UniqueName="GoodsName" HeaderText=" ">
                            <HeaderStyle Width="15%" HorizontalAlign="left" />
                            <ItemStyle HorizontalAlign="left" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn DataField="Specification" HeaderText="商品SKU" UniqueName="Specification">
                            <HeaderStyle Width="35%" HorizontalAlign="left" />
                            <ItemStyle HorizontalAlign="left" />
                            <ItemTemplate>
                                </ItemTemplate>
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn DataField="Price" HeaderText="商品价格" UniqueName="Price">
                            <HeaderStyle Width="10%" HorizontalAlign="left" />
                            <ItemStyle HorizontalAlign="left" />
                            <ItemTemplate>
                                </ItemTemplate>
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn DataField="PlanQuantity" UniqueName="PlanQuantity" HeaderText="数量"
                            Aggregate="Sum" DataType="System.Double" FooterText="小计数量:">
                            <ItemTemplate>
                               </ItemTemplate>
                            <FooterStyle HorizontalAlign="Right" />
                            <HeaderStyle Width="15%" HorizontalAlign="left" />
                            <ItemStyle HorizontalAlign="left" />
                            <FooterTemplate>
                            </FooterTemplate>
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn DataField="PurchasingGoodsType" HeaderText="赠品" UniqueName="PurchasingGoodsType">
                            <HeaderStyle HorizontalAlign="left" />
                            <ItemStyle HorizontalAlign="left" />
                            <ItemTemplate>
                                </ItemTemplate>
                            <FooterTemplate>
                            </FooterTemplate>
                        </rad:GridTemplateColumn>
                    </Columns>
                    <GroupByExpressions>
                        <rad:GridGroupByExpression>
                            <GroupByFields>
                                <rad:GridGroupByField FieldName="GoodsName" HeaderText="商品名" />
                            </GroupByFields>
                            <SelectFields>
                                <rad:GridGroupByField FieldName="GoodsName" HeaderText=" " HeaderValueSeparator=" " />
                            </SelectFields>
                        </rad:GridGroupByExpression>
                    </GroupByExpressions>
                </MasterTableView>
            </rad:RadGrid>
        </div>

        <rad:RadWindowManager ID="WindowManager" runat="server" Width="1200px" Height="600px">
            <Windows>
               
            </Windows>
        </rad:RadWindowManager>

        <rad:RadAjaxManager runat="server" ID="RAM">
            
        </rad:RadAjaxManager>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowExchangedDetailForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShowExchangedDetailForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退换货详情</title>
    <style type="text/css">
        .gray {
            background-color: gainsboro;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <table width="100%" style="padding-top: 5px; padding-bottom: 5px">
        <tr>
            <td style="width: 70px;" align="right">
                <asp:Label runat="server" ID="LbApplyNoText" ></asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="LbApplyNoValue" ></asp:Label>
            </td>
            <td style="width: 70px;" align="right">
                <asp:Label runat="server" ID="LbApplyTimeText" ></asp:Label>
            </td>
            <td>
                <asp:Label runat="server" ID="LbApplyTimeValue" ></asp:Label>
            </td>
            <td>
                &nbsp;
            </td>
        </tr>
    </table>
    <rad:RadGrid runat="server" ID="RgRefundGoodsList" OnNeedDataSource="RgRefundGoodsListNeedDataSource"
        OnDetailTableDataBind="RgRefundGoodsListDetailTableDataBind" SkinID="Common" >
        <MasterTableView DataKeyNames="GoodsID,RealGoodsID,Quantity" CommandItemDisplay="None">
            <Columns>
                <rad:GridTemplateColumn ReadOnly="true" HeaderText="退货商品">
                    <ItemTemplate>
                        <%# string.Format("{0}", Eval("GoodsName"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn ReadOnly="True" HeaderText="数量" DataField="Quantity">
                    <HeaderStyle HorizontalAlign="Center" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
            </Columns>
            <DetailTables>
                <rad:GridTableView runat="server" DataKeyNames="GoodsID,RealGoodsID,Quantity" DataMember="Details"
                    CommandItemDisplay="None" Width="100%" NoDetailRecordsText="">
                        <Columns>
                            <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                <HeaderStyle HorizontalAlign="Center"/>
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Quantity" ReadOnly="true" HeaderText="退货数" UniqueName="Quantity" >
                                <ItemStyle HorizontalAlign="Center" Width="80" /> 
                                <HeaderStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"/>
                            </rad:GridBoundColumn>
                        </Columns>
                </rad:GridTableView>
            </DetailTables>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadGrid runat="server" ID="RgBarterGoodsList" OnNeedDataSource="RgBarterGoodsListNeedDataSource" SkinID="Common">
        <MasterTableView DataKeyNames="GoodsId" CommandItemDisplay="None" >
            <Columns>
                <rad:GridTemplateColumn DataField="GoodsName" ReadOnly="true" HeaderText="原商品">
                    <ItemTemplate>
                        <%# string.Format("{0}<br/>{1}", Eval("GoodsName"), Eval("Specification"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="Quantity" ReadOnly="true" HeaderText="换货商品">
                    <ItemTemplate>
                        <%# string.Format("{0}<br/>{1}", Eval("BarterGoodsName"), Eval("BarterSpecification"))%>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <!-- 是否显示审核 -->
    <div runat="server" ID="ShowAudit">
        <table width="100%" style="padding-top: 10px;" cellpadding="4px;">
            <tr>
                <td align="right" style="width: 80px;">
                    处理状态：
                </td>
                <td align="left" style="width: 80px;" >
                    <asp:RadioButton runat="server" ID="RbPass" Text="审核" GroupName="Audit"/>
                </td>
                <td align="left" style="width: 100px;">
                    <asp:RadioButton runat="server" ID="RbNoPass" Text="审核不通过" GroupName="Audit"/>
                </td>
                <td align="left" colspan="2">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="3" align="right">
                    <asp:ImageButton runat="server" ID="LbAudit" SkinID="AffirmImageButton" ToolTip="确认" AlternateText="确认" 
                     OnClick="BtnCheckedOnClick"/>
                </td>
                <td align="left">
                    <asp:ImageButton runat="server" ID="IbCancel" SkinID="CancelImageButton" AlternateText="返回" ToolTip="返回"
                    OnClick="BtnBackOnClick" />
                </td>
            </tr>
        </table>
    </div>
    <!-- 是否显示商品检查 -->
    <div runat="server" ID="ShowCheckRefund">
        <table width="100%" style="padding-top: 10px;" cellpadding="4px;">
            <tr>
                <td align="right">
                    退回原因：
                </td>
                <td colspan="4">
                    <asp:TextBox runat="server" ID="TbReason" Width="500" Enabled="False"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    重启检查：
                </td>
                <td colspan="4" align="left">
                    <asp:CheckBox runat="server" ID="CkReStart" Text="待检查"/>
                </td>
            </tr>
            <tr>
                <td colspan="3" align="right">
                    <asp:ImageButton runat="server" ID="BtnChecked" SkinID="AffirmImageButton" ToolTip="确认" AlternateText="确认" 
                     OnClick="BtnCheckedOnClick"/>
                </td>
                <td align="left">
                    <asp:ImageButton runat="server" ID="BtnBack" SkinID="CancelImageButton" AlternateText="返回" ToolTip="返回"
                    OnClick="BtnBackOnClick" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server">
        
    </rad:RadAjaxManager>
    </form>
</body>
</html>

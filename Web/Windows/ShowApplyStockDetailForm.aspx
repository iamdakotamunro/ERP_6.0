<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowApplyStockDetailForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShowApplyStockDetailForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>要货单详情</title>
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
    <table width="98%">
        <tr>
            <td align="right">
                订单号：
            </td>
            <td align="left" style="width: 120px;">
                <asp:Label runat="server" ID="LbApplyNo" ></asp:Label>
            </td>
            <td align="right">
                下单时间：
            </td>
            <td colspan="2" align="left">
                <asp:Label runat="server" ID="LbCreateTime" ></asp:Label>
            </td>
        </tr>
        <tr>
            <td colspan="5">
                <rad:RadGrid runat="server" ID="RgApplyGoodsList" SkinID="Common"
                OnNeedDataSource="RgApplyGoodsListNeedDataSource" 
                OnItemDataBound="RgApplyGoodsListItemDataBound" OnDetailTableDataBind="RgApplyGoodsListDetailTableDataBind">
                    <MasterTableView DataKeyNames="GoodsId,CompGoodsID,Quantity,SendQuantity" 
                    CommandItemSettings-ShowAddNewRecordButton="false"
                     CommandItemDisplay="None" ShowFooter="True">
                        <Columns>
                            <rad:GridTemplateColumn ReadOnly="true" HeaderText="要货商品">
                                <ItemTemplate>
                                    <%# string.Format("{0}<br/>{1}",Eval("GoodsName"),Eval("Specification")) %>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center"/>
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn ReadOnly="true" HeaderText="单价" FooterText="数据汇总：" UniqueName="Price">
                                <ItemTemplate>
                                    <%# string.Format("￥{0}", Convert.ToDecimal(Eval("Price")).ToString("f2"))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Right"/>
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="Quantity" ReadOnly="true" HeaderText="要货数" UniqueName="Quantity" >
                                <ItemStyle HorizontalAlign="Center" Width="80" /> 
                                <HeaderStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"/>
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="SendQuantity" ReadOnly="true" HeaderText="发货数" UniqueName="SendQuantity" >
                                <ItemStyle HorizontalAlign="Center" Width="80" />
                                <HeaderStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"/>
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="收货数" UniqueName="ReceiveQuantity">
                                <ItemTemplate>  
                                    <asp:TextBox runat="server" ID="TbReceive" Text='<%# Eval("ReceiveQuantity") %>' 
                                    Enabled='<%# IsAudit && Eval("GoodsId").ToString()!=Guid.Empty.ToString() %>'
                                    Width="60" AutoPostBack="True" onKeyup="this.value=this.value.replace(/-?\D/g,'')" 
                                                    OnTextChanged="TbReceiveCountChanged"></asp:TextBox>
                                </ItemTemplate>  
                                <FooterTemplate>
                                    <asp:Label runat="server" ID="LbReceive"></asp:Label>
                                </FooterTemplate>
                                <HeaderStyle HorizontalAlign="Center"  Width="80"/>
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Center"/>
                            </rad:GridTemplateColumn>
                        </Columns>
                        <DetailTables>
                            <rad:GridTableView runat="server" DataKeyNames="GoodsId,CompGoodsID,Quantity,SendQuantity" DataMember="Details"
                                CommandItemDisplay="None" Width="100%" NoDetailRecordsText="">
                                    <Columns>
                                        <rad:GridBoundColumn DataField="Specification" HeaderText="规格" UniqueName="Specification">
                                            <HeaderStyle HorizontalAlign="Center"/>
                                            <ItemStyle HorizontalAlign="Center" />
                                        </rad:GridBoundColumn>
                                        <rad:GridBoundColumn DataField="Quantity" ReadOnly="true" HeaderText="要货数" UniqueName="Quantity" >
                                            <ItemStyle HorizontalAlign="Center" Width="80" /> 
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <FooterStyle HorizontalAlign="Center"/>
                                        </rad:GridBoundColumn>
                                        <rad:GridBoundColumn DataField="SendQuantity" HeaderText="发货数" UniqueName="SendQuantity">
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </rad:GridBoundColumn>
                                        <rad:GridTemplateColumn  HeaderText="收货数" UniqueName="ReceiveQuantity">
                                            <ItemTemplate>
                                                <asp:TextBox runat="server" ID="TbReceiveCount" Text='<%# Eval("ReceiveQuantity") %>' MaxLength="3" 
                                                    ToolTip='<%# Eval("Quantity") %>' onKeyup="this.value=this.value.replace(/-?\D/g,'')" 
                                                     Width="30" AutoPostBack="True" Enabled='<%# IsAudit %>' 
                                                    OnTextChanged="TbReceiveCountChanged"></asp:TextBox>
                                            </ItemTemplate>
                                            <HeaderStyle HorizontalAlign="Center" />
                                            <ItemStyle HorizontalAlign="Center" />
                                        </rad:GridTemplateColumn>
                                    </Columns>
                            </rad:GridTableView>
                        </DetailTables>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <div runat="server" ID="ShowAudit">
        <table width="100%">
            <tr>
                <td align="right">
                    <asp:ImageButton runat="server" SkinID="AffirmImageButton" ID="IbAffirm" OnClick="IbAffirmOnClick"/>
                </td>
                <td>
                    <asp:ImageButton runat="server" SkinID="CancelImageButton" ID="IbCancel" OnClientClick="return CancelWindow();"/>
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IbAffirm">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgApplyGoodsList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="IbAffirm" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="IbCancel" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RgApplyGoodsList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgApplyGoodsList"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="GoodsPriceChange.aspx.cs" Inherits="ERP.UI.Web.GoodsPriceChange" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table>
        <tr>
            <%--<td>销售公司：
            </td>
            <td>
                <rad:RadComboBox ID="rcb_SaleFiliale" runat="server" AutoPostBack="True" OnSelectedIndexChanged="rcb_SaleFiliale_SelectedIndexChanged">
                </rad:RadComboBox>
            </td>
            <td>销售平台：
            </td>
            <td>
                <rad:RadComboBox ID="rcb_SalePlatform" runat="server">
                    <Items>
                        <rad:RadComboBoxItem Value="<%# Guid.Empty %>" Text="全部" />
                    </Items>
                </rad:RadComboBox>
            </td>--%>
            <td>改价类型：
                <rad:RadComboBox ID="rcb_Type" runat="server" Width="60px">
                    <Items>
                        <rad:RadComboBoxItem Value="-1" Text="全部" />
                        <rad:RadComboBoxItem Selected="True" Value="0" Text="销售价" />
                        <rad:RadComboBoxItem Value="1" Text="加盟价" />
                        <rad:RadComboBoxItem Value="2" Text="批发价" />
                    </Items>
                </rad:RadComboBox>
            </td>
            <td>
                <rad:RadTextBox ID="txt_GoodsName" runat="server" EmptyMessage="商品名称" Width="300px" Style="margin-left: 10px; margin-right: 10px;"></rad:RadTextBox>
            </td>
            <td>
                <rad:RadTextBox ID="txt_GoodsCode" runat="server" EmptyMessage="商品编号" Style="margin-right: 10px;"></rad:RadTextBox>
            </td>
            <td>
                <asp:Button ID="btnSearch" runat="server" Text="搜索" OnClick="btnSearch_Click" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_GoodsPriceChange" SkinID="CustomPaging" runat="server" ShowFooter="True" OnNeedDataSource="RG_GoodsPriceChange_NeedDataSource">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridBoundColumn DataField="Name" HeaderText="修改人姓名" HeaderStyle-HorizontalAlign="Center">
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Datetime" DataFormatString="{0:yyyy-MM-dd HH:mm:ss}" HeaderText="修改时间" HeaderStyle-HorizontalAlign="Center">
                    <ItemStyle Width="110px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" HeaderStyle-HorizontalAlign="Center">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" HeaderStyle-HorizontalAlign="Center">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="SaleFilialeName" HeaderText="销售公司" HeaderStyle-HorizontalAlign="Center">
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="SalePlatformName" HeaderText="销售平台" HeaderStyle-HorizontalAlign="Center">
                    <ItemStyle Width="150px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="OldPrice" HeaderText="修改前价格" HeaderStyle-HorizontalAlign="Center">
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="NewPrice" HeaderText="修改后价格" HeaderStyle-HorizontalAlign="Center">
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="Quota" HeaderText="修改额度" HeaderStyle-HorizontalAlign="Center">
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%# Math.Abs(Convert.ToDecimal(Eval("Quota").ToString()))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="改价类型" HeaderStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# GetPriceType(Eval("Type").ToString())%>
                    </ItemTemplate>
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="Loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RG_GoodsPriceChange">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_GoodsPriceChange" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="rcb_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rcb_SalePlatform" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_GoodsPriceChange" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="AvgSettlePrice.aspx.cs" Inherits="ERP.UI.Web.AvgSettlePrice" %>
<%@ Register TagPrefix="rad" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI, Version=2010.2.713.0, Culture=neutral, PublicKeyToken=29ac1a93ec063d92" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table style="width: 100%;">
        <tr>
            <td valign="top" style="width: 200px;">
                <rad:RadTreeView ID="TVGoodsClass" runat="server" SkinID="Common" Height="550px"
                    Width="200px" CausesValidation="false" OnNodeClick="TvGoodsClassNodeClick">
                </rad:RadTreeView>
            </td>
            <td valign="top">
                <div style="padding-bottom: 10px;">
                    公司: <asp:DropDownList ID="DDL_HostingFilialeAuth" runat="server" Width="130px"></asp:DropDownList>
                    商品编号/名称：<asp:TextBox ID="txt_GoodsNameOrCode" runat="server" Width="300px"></asp:TextBox>
                    年月：<asp:TextBox ID="txt_YearMonth" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM',isShowClear:false})"></asp:TextBox>
                    <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
                    <asp:HiddenField ID="Hid_GoodsClassId" runat="server" Value="00000000-0000-0000-0000-000000000000" />
                </div>
                <rad:RadGrid ID="RG_AvgSettlePrice" runat="server" OnNeedDataSource="RG_AvgSettlePrice_NeedDataSource">
                    <MasterTableView>
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle Height="0px" />
                        <Columns>
                            <rad:GridTemplateColumn HeaderText="商品编号" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <a href="JavaScript:void(0);" style="text-decoration: underline; color: #0099CC" onclick="AvgSettlePriceDetail('<%#Eval("GoodsId") %>','<%#Eval("GoodsName") %>')"><%#Eval("GoodsCode") %></a>
                                </ItemTemplate>
                                <ItemStyle Width="80px"/>
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称">
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="DayTime" HeaderText="年月" DataFormatString="{0:yyyy-MM}">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="60px" HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="结算价" HeaderStyle-HorizontalAlign="Center">
                                <ItemTemplate>
                                    <%#Eval("AvgSettlePrice").ToString()=="0"?"-": Eval("AvgSettlePrice")%>
                                </ItemTemplate>
                                <ItemStyle Width="60px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="结算价明细" Width="700px" Height="400px" />
        </Windows>
    </rad:RadWindowManager>
     <rad:RadAjaxManager ID="RAM" runat="server"></rad:RadAjaxManager>
    <script src="My97DatePicker/WdatePicker.js"></script>
    <script type="text/javascript">
        
        //结算价明细
        function AvgSettlePriceDetail(goodsId, goodsName) {
            var select1 = document.all.<%= DDL_HostingFilialeAuth.ClientID %>;
            var hostingFilialeId = select1.options[select1.selectedIndex].value;
            window.radopen("../Windows/AvgSettlePriceDetail.aspx?GoodsId=" + goodsId + "&GoodsName=" + goodsName + "&HostingFilialeId=" + hostingFilialeId, "raw");
        }
    </script>
</asp:Content>

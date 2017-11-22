<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="ReportStatistics.aspx.cs" Inherits="ERP.UI.Web.ReportStatistics" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <style type="text/css">
        td.ShortFromRowTitle {
            /* min-width: 10px;*/
            white-space: nowrap;
            height: 24px;
            text-align: right;
        }

        td.AreaEditFromRowInfo {
            height: 24px;
            padding-left: 2px;
            white-space: nowrap;
        }

        .button {
            width: 25px;
            border: 1px solid #D5D5D5;
            border-bottom: 1px solid #C2C2C2;
            border-right: 1px solid #C2C2C2;
            font-size: 11px;
            color: #666666;
            background-position: top;
            background-color: white;
            height: 20px;
            vertical-align: middle;
        }
    </style>

    <div class="StagePanel" style="height: 240px;">
        <table class="StagePanelHead" style="width: 100%;">
            <tr>
                <td>
                    <table style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

        <table class="PanelArea">
            <tr>
                 <td class="ShortFromRowTitle">物流配送公司：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBox1" runat="server" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">&nbsp;备货天数：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TB_StockDay" runat="server" Width="40px" Text="10"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFVStockDay" runat="server" ErrorMessage="*" ControlToValidate="TB_StockDay"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="REVStockDay" runat="server" ControlToValidate="TB_StockDay"
                        Text="*" ErrorMessage="备货天数必须为正整数！" ValidationExpression="^[1-9]\d*$"></asp:RegularExpressionValidator>
                </td>
                <td class="ShortFromRowTitle">到货日期：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadDateTimePicker ID="RDP_ArrivalTime" runat="server" Width="150px" EnableTyping="False">
                    </rad:RadDateTimePicker>
                </td>
                
                <td class="ShortFromRowTitle">仓库：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_InStock" runat="server" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">责任人：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBox2" runat="server" Height="200px">
                    </rad:RadComboBox>
                </td>
            </tr>
        </table>
      <table class="PanelArea">
            <tr>
                <td class="ShortFromRowTitle">查询商品：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox runat="server" ID="GoodsName" Width="333px"></asp:TextBox>
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td class="ShortFromRowTitle" style="padding-left: -40px;">
                    <asp:Button ID="btn_add" runat="server" Text="添加" />
                    <br />
                    <br />
                </td>
                <td class="ShortFromRowTitle">
                    <rad:RadListBox ID="lbx_goodslist1" runat="server" Width="260px" Height="120px">
                    </rad:RadListBox>
                </td>
                <td class="ShortFromRowTitle" style="padding-right: 55px;">
                    <asp:Button ID="btn_Right" runat="server" Text=">" CssClass="button"
                        ToolTip="添加到右边" />
                    <br />
                    <asp:Button ID="btn_Left" runat="server" Text="<"   CssClass="button"
                        ToolTip="移除到左边" />
                    <br />
                    <br />
                    <asp:Button ID="btn_AllRight" runat="server" Text=">>" CssClass="button"
                        ToolTip="全部添加到右边" />
                    <br />
                    <asp:Button ID="btn_AllLeft" runat="server" Text="<<" CssClass="button"
                        ToolTip="全部移除到左边" />
                    <br />
                </td>
                <td>
                    <rad:RadListBox ID="lbx_goodslist2" runat="server" Width="260px" Height="120px">
                    </rad:RadListBox>
                </td>
            </tr>
        </table>
        <table class="PanelArea" style="border: 0px;">
            <tr>
                <td style="text-align: right">
                    <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="CreationData"  />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="IB_Purchasing" runat="server" SkinID="PurchaseData"  />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="RGSS" OnDetailTableDataBind="Rgss_NeedDataSource" OnNeedDataSource="Rgss_GoodsNeedDataSource" ShowFooter="true" PageSize="40" runat="server">
        <MasterTableView DataKeyNames="GoodsId">
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="left" Height="26px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="商品名" UniqueName="GoodsName">
                    <ItemTemplate>
                        <asp:Label ID="LB_GoodsName" runat="server" Text='<%# Eval("GoodsName") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="98%" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="left" />
                </rad:GridTemplateColumn>
            </Columns>
            <DetailTables>
                <rad:GridTableView runat="server" DataKeyNames="GoodsId,RealityNeedPurchasingQuantity,Specification"
                    ShowFooter="true" ShowGroupFooter="true" CommandItemDisplay="Top" Width="100%"
                    NoDetailRecordsText="无子记录信息。">
                    <CommandItemTemplate>
                    </CommandItemTemplate>
                    <Columns>
                        <rad:GridBoundColumn DataField="Specification" HeaderText="商品SKU" UniqueName="Specification">
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                            <FooterStyle Width="90px" HorizontalAlign="Center" />
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="现有库存" Aggregate="Sum" FooterText="总库存:" DataField="NonceWarehouseGoodsStock"
                            UniqueName="StockNumber">
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="出库量" Aggregate="Sum" FooterText="小计：" DataField="NonceRequest"
                            UniqueName="NonceRequest">
                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                            <FooterStyle Width="70px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="占用数" Aggregate="Sum" FooterText="小计：" DataField="DemandQuantity"
                            UniqueName="DemandQuantity">
                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                            <FooterStyle Width="70px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="需求数" Aggregate="Sum" FooterText="小计：" DataField="RequireQuantity"
                            UniqueName="RequireQuantity">
                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                            <FooterStyle Width="70px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="PurchasingQuantity" HeaderText="采购中" UniqueName="PurchasingQuantity" 
                            Aggregate="Sum" FooterText="小计：">
                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                            <FooterStyle Width="70px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="FirstNumberThreeStockUpSale" HeaderText="前第3月销量"
                            Aggregate="Sum" FooterText="小计：" UniqueName="FirstNumberThreeStockUpSale">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="FirstNumberTwoStockUpSale" HeaderText="前第2月销量" UniqueName="FirstNumberTwoStockUpSale"
                            Aggregate="Sum" FooterText="小计：">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="FirstNumberOneStockUpSale" HeaderText="前第1月销量" UniqueName="FirstNumberOneStockUpSale"
                            Aggregate="Sum" FooterText="小计：">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="SaleAvgCrease" HeaderText="均增长率" UniqueName="SaleAvgCrease"
                            Aggregate="Avg" FooterText="均增长率:" DataFormatString="{0:P}">
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn HeaderText="日平均销量" SortExpression="WeightedAverageSaleQuantity" UniqueName="WeightedAverageSaleQuantity">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                            <FooterStyle Width="90px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="建议备货量" SortExpression="RealityNeedPurchasingQuantity" UniqueName="RealityNeedPurchasingQuantity">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                            <FooterStyle Width="90px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                       
                        <rad:GridTemplateColumn HeaderText="缺货-状态">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                            <FooterStyle Width="90px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                </rad:GridTableView>
            </DetailTables>
        </MasterTableView>
        <SortingSettings SortedAscToolTip="递增排序" SortedDescToolTip="递减排序" SortToolTip="单击排序" />
    </rad:RadGrid>

    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

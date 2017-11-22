<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.StockStatistics" Title="" CodeBehind="StockStatistics.aspx.cs" %>

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
                <td class="ShortFromRowTitle">仓库：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_InStock" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        AutoPostBack="true" MarkFirstMatch="True" ShowToggleImage="True" Height="200px" OnSelectedIndexChanged="RCB_InStock_OnSelectedIndexChanged">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">物流配送公司：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Filile" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        MarkFirstMatch="True" ShowToggleImage="True" Width="120px" Height="200px"
                        AutoPostBack="true">
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
                <td width="30%">&nbsp;</td>
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
                    <asp:Button ID="btn_add" runat="server" Text="添加" OnClick="Btn_Add_Click" />
                    <br />
                    <br />
                </td>
                <td class="ShortFromRowTitle">
                    <rad:RadListBox ID="lbx_goodslist1" runat="server" Width="260px" Height="120px">
                    </rad:RadListBox>
                </td>
                <td class="ShortFromRowTitle" style="padding-right: 55px;">
                    <asp:Button ID="btn_Right" runat="server" Text=">" OnClick="AddToRight" CssClass="button"
                        ToolTip="添加到右边" />
                    <br />
                    <asp:Button ID="btn_Left" runat="server" Text="<" OnClick="RemoveToLeft" CssClass="button"
                        ToolTip="移除到左边" />
                    <br />
                    <br />
                    <asp:Button ID="btn_AllRight" runat="server" Text=">>" OnClick="AllAddToRight" CssClass="button"
                        ToolTip="全部添加到右边" />
                    <br />
                    <asp:Button ID="btn_AllLeft" runat="server" Text="<<" OnClick="AllReMoveToLeft" CssClass="button"
                        ToolTip="全部移除到左边" />
                    <br />
                </td>
                <td style="text-align: left;">
                    <rad:RadListBox ID="lbx_goodslist2" runat="server" Width="260px" Height="120px">
                    </rad:RadListBox>
                </td>
                <td class="ShortFromRowTitle" style="text-align: left;">
                    <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="CreationData" OnClick="Ib_CreationData_Click" />
                    <br />
                    <br />
                    <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="Ib_ExportData_Click" />
                    <br />
                </td>
                <td width="45%">&nbsp;
                </td>
            </tr>
        </table>
        <table class="PanelArea" style="border: 0px;">
            <tr>
                <td>&nbsp;</td>
                <td class="ShortFromRowTitle" style="text-align: center; width: 100px;">到货日期:
                </td>
                <td class="AreaEditFromRowInfo" style="text-align: left; width: 160px;">
                    <rad:RadDateTimePicker ID="RDP_ArrivalTime" runat="server" Width="150px" EnableTyping="False">
                    </rad:RadDateTimePicker>
                </td>
                <td style="text-align: right; width: 100px;">
                    <asp:ImageButton ID="IB_Purchasing" runat="server" SkinID="PurchaseData" OnClick='Ib_Purching_Click' />
                </td>
            </tr>
        </table>
    </div>
    <a href="javascript:void(0);" onclick='javascript:$("#title").slideToggle();' style="display: block; padding-top: 5px; padding-bottom: 5px; padding-left: 10px; color: red; background-color: #cccccc;font-size:large;">名词解释</a>
    <div id="title" style="display: none; border: solid 1px #cccccc; margin-bottom: 10px;">
        <span style="color: blue;">商品规则：</span>此商品下眼镜的度数，取值来源于商品设置中的子商品取值<br />
        <span style="color: blue;">可用库存：</span>（整件区+零配区）-（待下货+已打印+下货中）<br />
        <span style="color: blue;">上货数：</span>上货单状态为待上货+上货中 之和<br />
        <span style="color: blue;">有效需求数：</span>订单状态=待导单、需采购、需调拨<br />
        <span style="color: blue;">采购中：</span>采购状态为：部分完成、入库中的采购单中待审核、核准、核退 入库单和没有生成入库单的采购数<br />
        <span style="color: blue;">前3个月销量：</span>：从这个月开始往前推三个月的第三个月销量和（包含订单、门店申请要货）前2个月销量、前1个月销量,：参考前3个月销量说明<br /><br/>
        <span style="color: blue;">平均增长率：</span>：= （NUM1+NUM2）/2 <br/>&nbsp;&nbsp;当计算得数≥1.3时，则默认均增长率为1.1；若＜1.3 ，则取计算值<br />
        NUM1取值的来源<br/>
        1：前3个月销量=0 ，前2个月销量 ＞1  取值NUM1=1<br/>
        2：前3个月销量=0 ，前2个月销量 ≤1  取值NUM1=0<br/>
        3：前3个月销量≠0   取值NuM1=（前2个月销量 - 前3个月销量）/前3个月销量<br/>
        NUM2取值的来源<br/>
        1：前2个月销量=0 ，前1个月销量 ＞1  取值NUM2=1<br/>
        2：前2个月销量=0 ，前1个月销量 ≤1  取值NUM2=0<br/>
        3：前2个月销量≠0   取值NIM2=（前1个月销量 - 前2个月销量）/前2个月销量<br /><br/>
        
        <span style="color: blue;">日平均销量：</span><br />
        默认取的是今天往前的110天，10天为一个单位 （10*11=110）<br />
        商品的销售天数<br />
        1：销售天数 < 90天时（排除销量最高一天的其中之一和销量最低的一天其中之一）<br />
           &nbsp;每日销量 = 总销量 / （销售的天数-2）<br />
           &nbsp;（若是销量只有两天，每日销量=两日的销量/2）<br />
        2：90天 <= 销售天数  < 100天：这种只取今天往前90天  （不排除里面所有的值）<br />
           &nbsp;100天 <= 销售天数  < 110天：这种只取今天往前100天（排除最高销售的一个十天的其中之一）<br />
           &nbsp;110 天  =  销售天数   （排除最高销售的十天的其中之一 和 最低销售的一个十天的其中之一）<br />
           &nbsp;剩下都是9个10天，按时间排序 离今天最近的一个10天为N1，直到N9<br />
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;3*（N1+N2+N3）+2*(N4+N5+N6)+(N7+N8+N9)<br/>
        每日销量= ------------------------------------------------<br />
           &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;10*9*6<br/><br/>
        
        <span style="color: blue;">建议备货量</span>= （有效需求数+待出库数）-（实际库存-出货数+上货数）-采购中数<br />
        采购数量：采购中，部分完成中未完成的数量，入库中<br />
        注：若建议备货量是负数，则无需备货<br />
        <span style="color: blue;">缺货：</span>“-”显示的是不缺货。“√”显示的是缺货<br />
        <span style="color: blue;">上架：</span>“-”显示的是不上架。“√”显示的是上架<br />
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
                <rad:GridTableView runat="server" DataKeyNames="GoodsId,RealityNeedPurchasingQuantity,Specification" DataMember="Details"
                    ShowFooter="true" ShowGroupFooter="true" CommandItemDisplay="None" Width="100%"
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
                        <rad:GridBoundColumn HeaderText="可用库存" DataField="NonceWarehouseGoodsStock" UniqueName="NonceWarehouseGoodsStock">
                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                            <FooterStyle Width="70px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="待出库数" DataField="SubtotalQuantity"
                            UniqueName="SubtotalQuantity">
                            <HeaderStyle Width="60px" HorizontalAlign="Center" />
                            <FooterStyle Width="60px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="上货数" DataField="UppingQuantity" UniqueName="UppingQuantity">
                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                            <FooterStyle Width="70px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="有效需求数" DataField="RequireQuantity"
                            UniqueName="RequireQuantity">
                            <HeaderStyle Width="60px" HorizontalAlign="Center" />
                            <FooterStyle Width="60px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="SubtractPurchasingQuantity" HeaderText="采购中数" UniqueName="SubtractPurchasingQuantity">
                            <HeaderStyle Width="60px" HorizontalAlign="Center" />
                            <FooterStyle Width="60px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="FirstNumberThreeStockUpSale" HeaderText="前第3月销量</br>(前90天至前60天)" UniqueName="FirstNumberThreeStockUpSale">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <FooterStyle Width="100px" HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="FirstNumberTwoStockUpSale" HeaderText="前第2月销量</br>(前60天至前30天)" UniqueName="FirstNumberTwoStockUpSale">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <FooterStyle Width="100px" HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="FirstNumberOneStockUpSale" HeaderText="前第1月销量</br>(前30天至昨天)" UniqueName="FirstNumberOneStockUpSale">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <FooterStyle Width="100px" HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="SaleAvgCrease" HeaderText="均增长率" UniqueName="SaleAvgCrease" DataFormatString="{0:P}">
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <FooterStyle Width="80px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="日平均销量" DataField="WeightedAverageSaleQuantity" SortExpression="WeightedAverageSaleQuantity" UniqueName="WeightedAverageSaleQuantity" DataFormatString="{0:F2}">
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                            <FooterStyle Width="90px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="建议备货量" DataField="RealityNeedPurchasingQuantity" SortExpression="RealityNeedPurchasingQuantity" UniqueName="RealityNeedPurchasingQuantity">
                            <HeaderStyle Width="90px" HorizontalAlign="Center" />
                            <FooterStyle Width="90px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn HeaderText="缺货">
                            <ItemTemplate>
                                <asp:Label ID="LabIS" runat="server" Text='<%# GetIsScarcity(Eval("IsScarcity")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="40px" HorizontalAlign="Center" />
                            <FooterStyle Width="40px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="上架">
                            <ItemTemplate>
                                <asp:Label ID="LabOnShelf" runat="server" Text='<%# GetIsOnShelf(Eval("IsOnShelf")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="40px" HorizontalAlign="Center" />
                            <FooterStyle Width="40px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                </rad:GridTableView>
            </DetailTables>
        </MasterTableView>
        <SortingSettings SortedAscToolTip="递增排序" SortedDescToolTip="递减排序" SortToolTip="单击排序" />
    </rad:RadGrid>

    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IB_CreationData">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGSS" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGSS">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGSS" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_InStock">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Filile" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_add">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist1" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_delete">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist1" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_InStock">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rcb_Pm" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_Right">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist1" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl />
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist2" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_Left">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist1" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl />
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist2" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_AllRight">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist1" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl />
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist2" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_AllLeft">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist1" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl />
                    <rad:AjaxUpdatedControl ControlID="lbx_goodslist2" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_Purchasing">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGSS" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

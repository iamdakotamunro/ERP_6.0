<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.SalesRankingsAw" Title="" CodeBehind="SalesRankings.aspx.cs" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/jquery.js"></script>
        <script language="javascript" type="text/javascript">
            function doCondFocusGoodsName(cond) {
                if (cond.value == "支持模糊搜索") {
                    cond.value = "";
                }
            }
            function doCondBlurGoodsName(cond) {
                if (cond.value == "") {
                    cond.value = "支持模糊搜索";
                }
            }
            function doCondFocusGoodsCode(cond) {
                if (cond.value == "仅支持精确搜索") {
                    cond.value = "";
                }
            }
            function doCondBlurGoodsCode(cond) {
                if (cond.value == "") {
                    cond.value = "仅支持精确搜索";
                }
            }

            function RowDblClick(obj, args) {
                var temp = document.getElementById("<%=hfParamter.ClientID %>").value;
                var type = document.getElementById("<%=hfType.ClientID %>").value;
                if (type == "") type = 0;
                if (type == 0 || type == 3) {
                    var strs = temp.split("$"); //字符分割
                    var saleFilialeId = strs[0];
                    var salePlatformId = strs[1];
                    var startTime = strs[2];
                    var endTime = strs[3];
                    window.radopen("./Windows/SalesRankingsChart.aspx?type=" + type + "&Id=" + args.getDataKeyValue("Id") + "&SeriesId=" + args.getDataKeyValue("SeriesId") + "&SalefilialeId=" + saleFilialeId + "&SalePlatformId=" + salePlatformId + "&StartTime=" + startTime + "&EndTime=" + endTime, "ChartForm");
                }
                return false;
            }

            function clientShow(sender, eventArgs) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }

            $(function () {
                TreeToggle();
            });

            function TreeToggle() {
                if ($("div[id$='tree_GoodsClass']").attr("lang") === "0") {
                    $("div[id$='tree_GoodsClass']").css("display", "none");
                    $("div[id$='tree_GoodsClass']").attr("lang", "1");
                    $("#treeshow").attr("src", "Images/move2.gif");
                } else {
                    $("div[id$='tree_GoodsClass']").css("display", "");
                    $("div[id$='tree_GoodsClass']").attr("lang", "0");
                    $("div[id$='tree_GoodsClass']").width("200px");
                    $("#treeshow").attr("src", "Images/move1.gif");
                }
            }
        </script>
    </rad:RadScriptBlock>
    <table style="width: 100%;">
        <tr>
            <td style="vertical-align: top;">
                <div id="tree_GoodsClass" runat="server" lang="0">
                    <rad:RadTreeView ID="TVGoodsClass" runat="server" UseEmbeddedScripts="false" Height="500px"
                        Width="200px" CausesValidation="false">
                    </rad:RadTreeView>
                </div>
            </td>
            <td style="border: 1px solid #cccccc; width: 10px;">
                <div style="height: 45%;"></div>
                <img id="treeshow" alt="隐藏菜单" src="Images/move1.gif" width="10" height="56" onclick="TreeToggle();" style="cursor: pointer;"></img>
            </td>
            <td style="vertical-align: top;">
                <div class="StagePanel">
                    <table class="StagePanelHead" border="0" style="width: 100%;">
                        <tr>
                            <td>
                                <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                                    <tr>
                                        <td></td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                    <asp:HiddenField ID="hfParamter" runat="server" />
                    <asp:HiddenField runat="server" ID="hfType" />
                    <table class="PanelArea">
                        <tr>
                            <td style="width: 65px;">起始日期：
                            </td>
                            <td>
                                <rad:RadDatePicker ID="RDP_StartTime" runat="server" Width="120px"></rad:RadDatePicker>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="RFVStartTime" runat="server" ErrorMessage="*" ControlToValidate="RDP_StartTime"></asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 65px;">截止日期：
                            </td>
                            <td>
                                <rad:RadDatePicker ID="RDP_EndTime" runat="server" Width="120px"></rad:RadDatePicker>
                            </td>
                            <td>
                                <asp:RequiredFieldValidator ID="RFVEndTime" runat="server" ErrorMessage="*" ControlToValidate="RDP_EndTime"></asp:RequiredFieldValidator>
                            </td>
                            <td style="width: 65px;">显示方式：
                            </td>
                            <td colspan="2">
                                <asp:RadioButtonList runat="server" ID="CblType" AutoPostBack="True" OnSelectedIndexChanged="CblTypeSelectIndexChanged"
                                    RepeatDirection="Horizontal">
                                    <Items>
                                        <asp:ListItem Text="销量排行" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="平台分组" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="品牌分组" Value="2"></asp:ListItem>
                                    </Items>
                                </asp:RadioButtonList>
                            </td>
                            <td colspan="2">
                                <table width="100%">
                                    <tr>
                                        <td style="width: 65px;">数据条数：
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TB_TopNum" runat="server" Width="40px" Text="100"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RFVTopNum" runat="server" ErrorMessage="*" ControlToValidate="TB_TopNum"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="REVTopNum" runat="server" ControlToValidate="TB_TopNum"
                                                Text="*" ErrorMessage="生成条数必须为正整数！" ValidationExpression="^[1-9]\d*$"></asp:RegularExpressionValidator>
                                        </td>
                                    </tr>
                                </table>
                            </td>

                        </tr>
                        <tr>
                            <td>商品品牌：
                            </td>
                            <td colspan="2">
                                <rad:RadComboBox ID="RCB_Brand" runat="server" UseEmbeddedScripts="false" Width="140px" Filter="StartsWith"
                                    Height="160px" DataValueField="BrandId" DataTextField="Brand" AppendDataBoundItems="true">
                                    <Items>
                                        <rad:RadComboBoxItem Value="<%# Guid.Empty %>" Text="所有品牌" />
                                    </Items>
                                </rad:RadComboBox>
                            </td>
                            <td>商品编号：
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="TB_GoodsCode" Text="仅支持精确搜索" onfocus="doCondFocusGoodsCode(this);"
                                    onblur="doCondBlurGoodsCode(this);" MaxLength="20" runat="server" Width="135px"></asp:TextBox>
                            </td>
                            <td>商品名称：
                            </td>
                            <td colspan="2">
                                <asp:TextBox ID="TB_GoodsName" Text="支持模糊搜索" onfocus="doCondFocusGoodsName(this);"
                                    onblur="doCondBlurGoodsName(this);" MaxLength="50" runat="server" Width="250px"></asp:TextBox>
                            </td>
                            <td colspan="3">
                                <asp:CheckBox runat="server" Checked="True" Text="包含系列" ID="CkSeries" />
                            </td>
                        </tr>
                        <tr>
                            <td>销售公司：
                            </td>
                            <td>
                                <rad:RadComboBox ID="RCB_SaleFiliale" AutoPostBack="true" OnSelectedIndexChanged="Rcb_SalePlatform_SelectedIndexChanged"
                                    DataValueField="ProxyFilialeId" DataTextField="ProxyFilialeName" runat="server" AppendDataBoundItems="true"
                                    Width="140px">
                                </rad:RadComboBox>
                            </td>
                            <td align="right">销售平台：
                            </td>
                            <td>
                                <rad:RadComboBox ID="RCB_SalePlatform" DataValueField="ID" DataTextField="Name" runat="server"
                                    AppendDataBoundItems="true" Width="140px">
                                </rad:RadComboBox>
                            </td>
                            <td>
                                <asp:CheckBox runat="server" Checked="False" Text="含禁用平台" ID="CB_ContainDisableSalePlatform"
                                    OnCheckedChanged="OnCheckedChanged_ContainDisableSalePlatform" AutoPostBack="True" />
                            </td>

                        </tr>
                    </table>
                    <table class="PanelArea">
                        <tr>
                            <td class="Footer" align="center" colspan="4">
                                <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="CreationData" OnClick="Ib_CreationData_Click" />
                                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="Ib_ExportData_Click" />
                            </td>
                        </tr>
                    </table>
                </div>

                <a href="javascript:void(0);" onclick='javascript:$("#title").slideToggle();' style="display: block; padding-top: 5px; padding-bottom: 5px; padding-left: 10px; color: red; background-color: #cccccc;">名词解释</a>
                <div id="title" style="display: none; border: solid 1px #cccccc; margin-bottom: 10px;">
                    <span style="color: blue;">商品名/平台/品牌</span>：按“销量排行”查询显示为商品名称；按“平台分组”查询显示为平台名称；按“品牌分组”查询显示为品牌名称；<br />
                    <span style="color: blue;"><b>本期</b>销量</span>：选定时间范围内的销售数量(排除销售价格为0的销售数量)；<br />
                    <span style="color: blue;"><b>上期</b>销量</span>：以选定的“起始日期”为截止日期的前一个相同天数的销售数量(排除销售价格为0的销售数量)；<br />
                    <span style="color: blue;"><b>本期</b>销售额</span>：选定时间范围内的销售金额；<br />
                    <span style="color: blue;"><b>上期</b>销售额</span>：以选定的“起始日期”为截止日期的前一个相同天数的销售金额；<br />
                    <span style="color: blue;">0元销量</span>：销售价格为0的销售数量；<br />
                    <span style="color: blue;">销售额增长率</span>：(本期销售额 — 上一期销售额)/上一期销售额×100%；<br />
                    <span style="color: blue;">销售额毛利率</span>：(本期销售额 — 进货成本总价)/本期销售额×100%；<br />
                    <span style="color: blue;">进货成本总价</span>：当月结算价×本期销量。结算价:每个自然月商品的平均进价；<br />
                    <%--<span style="color: blue;">实际库存/被占用库存</span>：实际库存:商品的库存数量；被占用库存:待出库的商品的数量 + 被占用的商品的数量；<br />--%>
                </div>

                <rad:RadGrid ID="RGSGR" runat="server" SkinID="Common" ShowFooter="True" OnNeedDataSource="Rgsgr_NeedDataSource">
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="Id,SeriesId" ClientDataKeyNames="Id,SeriesId">
                        <CommandItemTemplate>
                            <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode">
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Name" HeaderText="商品名/平台/品牌" UniqueName="Name">
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="PreSalesNumber" HeaderText="上期销量" UniqueName="PreSalesNumber">
                                <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="SalesNumber" HeaderText="本期销量" UniqueName="SalesNumber">
                                <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="SalesNumIncrease" Visible="false" HeaderText="销量增长率"
                                UniqueName="SalesNumIncrease">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="PreSalesPrice" HeaderText="上期销售额" UniqueName="PreGoodsPrice">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("PreGoodsPrice"))%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="SalesPrice" HeaderText="本期销售额" UniqueName="GoodsPrice">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%#ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("GoodsPrice"))%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="ZeroNumber" HeaderText="0元销量" UniqueName="ZeroNumber">
                                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="SalesPriceIncrease" HeaderText="销售额增长率" UniqueName="SalesPriceIncrease">
                                <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="GrossMargin" HeaderText="销售额毛利率" UniqueName="GrossMargin">
                                <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="PurchasePrice" HeaderText="进货成本总价" UniqueName="PurchasePrice">
                                <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%#ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("PurchasePrice"))%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                        </Columns>
                        <ExpandCollapseColumn Visible="False">
                            <HeaderStyle Width="19px" />
                        </ExpandCollapseColumn>
                        <RowIndicatorColumn Visible="False">
                            <HeaderStyle Width="20px" />
                        </RowIndicatorColumn>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager ID="StockWindowManager" runat="server" Height="577px" Width="900px"
        ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="ChartForm" runat="server" OnClientShow="clientShow" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAMSalesRankings" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IB_CreationData">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGSGR" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="hfParamter"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="hfType"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGSGR">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGSGR" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Filiale" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="Rcb_HostingFiliale" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RCB_SaleFiliale" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Rcb_HostingFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RCB_SaleFiliale" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="CB_ContainDisableSalePlatform">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="CblType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="CkSeries"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="CblType"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

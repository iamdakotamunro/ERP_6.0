<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.GoodsSaleRunningAw" Title="无标题页" CodeBehind="GoodsSaleRunning.aspx.cs" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <script src="JavaScript/jquery.js"></script>
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
        <table class="PanelArea">
            <tr>
                <td>年份：
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="DDL_Years" AutoPostBack="true" OnSelectedIndexChanged="DdlYearsSelectedIndexChanged">
                    </asp:DropDownList>
                </td>
                <td>日期：
                </td>
                <td>
                    <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="95px">
                    </rad:RadDatePicker>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="RDP_StartTime"
                        ErrorMessage="*"></asp:RequiredFieldValidator>
                </td>
                <td>-
                </td>
                <td>
                    <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="95px">
                    </rad:RadDatePicker>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="RDP_EndTime"
                        ErrorMessage="*"></asp:RequiredFieldValidator>
                </td>
                <td>
                    <asp:CheckBox ID="CheckBoxOrderby" runat="server" Checked="true" Text="按SKU排序" />
                </td>
                <td>查询商品：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Goods" runat="server" AllowCustomText="True" EnableLoadOnDemand="True"
                        DataTextField="GoodsName" DataValueField="GoodsId" Width="250px" Height="200px">
                    </rad:RadComboBox>
                    <asp:RequiredFieldValidator ID="RFVGoods" runat="server" ControlToValidate="RCB_Goods"
                        ErrorMessage="*"></asp:RequiredFieldValidator>
                </td>
                <%--<td>
                    分公司：
                </td>--%>
                <td>
                    <rad:RadComboBox ID="RCB_Filiale" runat="server" AutoPostBack="true" AllowCustomText="False" Height="160px" DataValueField="ClassId" DataTextField="ClassName" CausesValidation="False" Visible="False">
                    </rad:RadComboBox>
                </td>
                <td>出入库类型：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_SalesType" runat="server" Width="100px" Height="240px" DropDownWidth="120px">
                        <ItemTemplate>
                            <input type="checkbox" name="SalesType" onclick="<%# Eval("Key").ToString()=="0"? "checkAll('SalesType');":"checkValue('SalesType');" %>" value='<%# Eval("Key") %>' /><%# Eval("Value") %>
                        </ItemTemplate>
                    </rad:RadComboBox>
                    <asp:HiddenField ID="Hid_SalesType" runat="server" />
                </td>
                <td>仓库：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" Width="150px"
                        Height="160px" DataValueField="WarehouseId" DataTextField="WarehouseName">
                    </rad:RadComboBox>
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td class="Footer" align="center" colspan="4">
                    <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="CreationData" OnClick="IbCreationDataClick" />
                    &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="IbExportDataClick" />
                </td>
            </tr>
        </table>
    </div>
    <table class="PanelArea">
        <tr style="width: 0">
            <td class="ShortFromRowTitle" style="width: 0"></td>
            <td class="ShortFromRowTitle">
                <asp:Label ID="tb_FirstGoodsStockNumber" runat="server" Width="0"></asp:Label>
            </td>
            <td class="ShortFromRowTitle"></td>
            <td class="ShortFromRowTitle" style="width: 0">
                <asp:Label ID="tb_LastGoodsStockNumber" runat="server" Width="0"> </asp:Label>
            </td>
            <td class="ShortFromRowTitle"></td>
            <td class="ShortFromRowTitle" style="width: 0">
                <asp:Label ID="tb_CurrentGoodsStockNumber" runat="server" Width="0"></asp:Label>
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RGSS" OnNeedDataSource="Rgss_NeedDataSource" SkinID="Common_Foot"
        runat="server">
        <MasterTableView>
            <CommandItemTemplate>
                <%--期初库存：<asp:Label ID="tb_FirstGoodsStockNumber" runat="server" Text='<%#First %>'>' ></asp:Label>
                期末库存：<asp:Label ID="tb_LastGoodsStockNumber" runat="server" Text='<%#Last %>'>' ></asp:Label>
                本期发生：<asp:Label ID="tb_CurrentGoodsStockNumber" runat="server" Text='<%#Current %>'>' ></asp:Label>--%>
                <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                    Text="刷新"></Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification"
                    AllowSorting="false">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="TradeCode" HeaderText="订单号" UniqueName="TradeCode"
                    AllowSorting="false">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="出入库类型">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%#GetStockType(Convert.ToInt32(Eval("StorageType")))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Companyname" HeaderText="供应商" UniqueName="Companyname"
                    AllowSorting="false">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Quantity" HeaderText="数量" UniqueName="Quantity">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="NonceWarehouseGoodsStock" HeaderText="此SKU库存" UniqueName="NonceWarehouseGoodsStock"
                    AllowSorting="false">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="DateCreated" HeaderText="更新时间" UniqueName="DateCreated"
                    AllowSorting="false">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
            </Columns>
        </MasterTableView>
        <SortingSettings SortedAscToolTip="递增排序" SortedDescToolTip="递减排序" SortToolTip="单击排序" />
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IB_CreationData">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGSS" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="tb_FirstGoodsStockNumber"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="tb_LastGoodsStockNumber"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="tb_CurrentGoodsStockNumber"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGSS">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGSS" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_Years">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RDP_EndTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Filiale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Filiale"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RDP_StartTime">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RDP_EndTime">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_EndTime" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <script type="text/javascript">
        //获取单选框的值
        function checkValue(type) {
            if (type === "SalesType") {
                $("input[id$='Hid_SalesType']").val("");

                $("input[type='checkbox'][name='SalesType']:checked:not([value='0'])").each(function () {
                    $("input[id$='Hid_SalesType']").val("," + $(this).val() + $("input[id$='Hid_SalesType']").val());
                });

                if ($("input[id$='Hid_SalesType']").val() !== "") {
                    $("input[id$='Hid_SalesType']").val($("input[id$='Hid_SalesType']").val().substring(1));
                }

                var salePlatformresult = 0;
                $("input[type='checkbox'][name='SalesType']:not([value='0'])").each(function () {
                    if ($(this).is(':checked') === false) {
                        salePlatformresult = 1;
                        return false;
                    }
                    return true;
                });

                if (salePlatformresult === 0) {
                    $("input[value='0'][name='SalesType']").prop('checked', true);
                } else {
                    $("input[value='0'][name='SalesType']").prop('checked', false);
                }
            }
        }

        function checkAll(type) {
            if (type === "SalesType") {
                $("input[id$='Hid_SalesType']").val("");
                if ($("input[value='0'][name='SalesType']").is(':checked')) {
                    $("input[type='checkbox'][name='SalesType']").prop('checked', true);
                    $("input[type='checkbox'][name='SalesType']:not([value='0'])").each(function () {
                        $("input[id$='Hid_SalesType']").val("," + $(this).val() + $("input[id$='Hid_SalesType']").val());
                    });
                } else {
                    $("input[type='checkbox'][name='SalesType']").prop('checked', false);
                }
                if ($("input[id$='Hid_SalesType']").val() !== "") {
                    $("input[id$='Hid_SalesType']").val($("input[id$='Hid_SalesType']").val().substring(1));
                }
            }
        }
    </script>
</asp:Content>

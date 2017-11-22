<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="CompanyGrossProfit.aspx.cs" Inherits="ERP.UI.Web.CompanyGrossProfit" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <script src="JavaScript/jquery.js"></script>
    <script src="My97DatePicker/WdatePicker.js"></script>

    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">日期类型：</td>
            <td>
                <asp:DropDownList ID="ddl_TimeType" runat="server" AutoPostBack="True" OnSelectedIndexChanged="ddl_TimeType_OnSelectedIndexChanged">
                    <asp:ListItem Text="按当月查询" Value="1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="按年月查询" Value="0"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">销售公司：</td>
            <td>
                <rad:RadComboBox ID="Rcb_SaleFiliale" runat="server" AutoPostBack="true" OnSelectedIndexChanged="Rcb_SaleFiliale_SelectedIndexChanged">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">订单类型：</td>
            <td>
                <rad:RadComboBox ID="Rcb_OrderType" runat="server">
                    <ItemTemplate>
                        <input type="checkbox" name="OrderType" onclick="<%# Eval("Key").ToString()=="-1"? "checkAll('OrderType');":"checkValue('OrderType');" %>" checked="checked" value='<%# Eval("Key") %>' /><%# Eval("Value") %>
                    </ItemTemplate>
                </rad:RadComboBox>
                <asp:HiddenField ID="Hid_OrderType" runat="server" />
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">
                <asp:Literal ID="lit_TimeTitle" runat="server" Text="当月查询："></asp:Literal>
            </td>
            <td>
                <asp:TextBox ID="txt_YearAndMonth" runat="server" Width="70px" Visible="False" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM',maxDate:'%y-{%M-1}'})"></asp:TextBox>

                <div id="currentMonth" runat="server">
                    <asp:TextBox ID="txt_StartTime" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',minDate:'%y-%M-01',maxDate:'#F{$dp.$D(\'ctl00_CPHStage_txt_EndTime\')}'})"></asp:TextBox>
                    至
                    <asp:TextBox ID="txt_EndTime" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',minDate:'#F{$dp.$D(\'ctl00_CPHStage_txt_StartTime\')}',maxDate:'%y-%M-%ld'})"></asp:TextBox>
                </div>
            </td>
            <td style="text-align: right;">销售平台：</td>
            <td>
                <rad:RadComboBox ID="Rcb_SalePlatform" runat="server" Height="200px">
                    <ItemTemplate>
                        <input type="checkbox" name="SalePlatform" onclick="<%# Eval("Key").ToString()=="00000000-0000-0000-0000-000000000000"? "checkAll('SalePlatform');":"checkValue('SalePlatform');" %>" value='<%# Eval("Key") %>' /><%# Eval("Value") %>
                    </ItemTemplate>
                </rad:RadComboBox>
                <asp:HiddenField ID="Hid_SalePlatform" runat="server" />
            </td>
            <td style="text-align: right;">门店汇总：</td>
            <td>
                <asp:RadioButtonList ID="rbl_OrderType" runat="server" RepeatDirection="Horizontal">
                    <asp:ListItem Text="是" Value="1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="否" Value="0"></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
        <tr>
            <td colspan="6" style="text-align: center;">
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
                <asp:Button ID="btn_Export" runat="server" Text="导出" OnClick="btn_Export_Click" />
            </td>
        </tr>
    </table>
    <div style="padding-left: 5%; padding-bottom: 5px; font-weight: bold; color: red;">门店毛利请去门店管理系统查询</div>

<rad:RadGrid ID="RG_GrossProfit" runat="server" ShowFooter="True" OnNeedDataSource="RG_GrossProfit_NeedDataSource" OnItemDataBound="RG_GrossProfit_ItemDataBound">
    <MasterTableView>
        <CommandItemTemplate>
        </CommandItemTemplate>
        <CommandItemStyle Height="0px" />
        <Columns>
            <rad:GridTemplateColumn HeaderText="销售公司">
                <ItemTemplate>
                    <%# (new Guid(Eval("SalePlatformId").ToString()).Equals(Guid.Empty)&&int.Parse(Eval("OrderType").ToString()).Equals(-1))?"":(SaleFiliales.ContainsKey(new Guid(Eval("SaleFilialeId").ToString()))?SaleFiliales[new Guid(Eval("SaleFilialeId").ToString())]:"-")  %>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="销售平台" UniqueName="TotalName">
                <ItemTemplate>
                    <%# SalePlatforms.ContainsKey(new Guid(Eval("SalePlatformId").ToString()))?SalePlatforms[new Guid(Eval("SalePlatformId").ToString())]:(int.Parse(Eval("OrderType").ToString()).Equals(-1)?"平台汇总：":"门店汇总")  %>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="订单类型">
                <ItemTemplate>
                    <%# OrderType.ContainsKey(int.Parse(Eval("OrderType").ToString()))?OrderType[int.Parse(Eval("OrderType").ToString())]:"-"  %>
                </ItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle Width="90px" HorizontalAlign="Center" />
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="运费成本" UniqueName="ShipmentCost">
                <ItemTemplate>
                    <span style="color: blue;">-&nbsp;</span><%#ERP.UI.Web.Common.WebControl.NumberSeparator(decimal.Parse(Eval("ShipmentCost").ToString()))%>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="运费收入" UniqueName="ShipmentIncome">
                <ItemTemplate>
                    <span style="color: blue;">+&nbsp;</span><%#ERP.UI.Web.Common.WebControl.NumberSeparator(decimal.Parse(Eval("ShipmentIncome").ToString()))%>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="销售金额" UniqueName="SalesAmount">
                <ItemTemplate>
                    <%#ERP.UI.Web.Common.WebControl.NumberSeparator(decimal.Parse(Eval("SalesAmount").ToString())-decimal.Parse(Eval("ShipmentIncome").ToString()))%>
                </ItemTemplate>
                <HeaderStyle ForeColor="red" />
                <ItemStyle ForeColor="red" />
                <FooterStyle ForeColor="red"></FooterStyle>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="( 商品金额" UniqueName="GoodsAmount">
                <ItemTemplate>
                    <span style="color: blue;">(&nbsp;</span><%#ERP.UI.Web.Common.WebControl.NumberSeparator(decimal.Parse(Eval("GoodsAmount").ToString()))%>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="- 促销抵扣" UniqueName="PromotionsDeductible">
                <ItemTemplate>
                    <span style="color: blue;">-&nbsp;</span><%#ERP.UI.Web.Common.WebControl.NumberSeparator(decimal.Parse(Eval("PromotionsDeductible").ToString()))%>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="- 积分抵扣 )" UniqueName="PointsDeduction">
                <ItemTemplate>
                    <span style="color: blue;">-&nbsp;</span><%#ERP.UI.Web.Common.WebControl.NumberSeparator(decimal.Parse(Eval("PointsDeduction").ToString()))%><span style="color: blue;">&nbsp;)</span>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="- 进货成本" UniqueName="PurchaseCosts">
                <ItemTemplate>
                    <span style="color: blue;">-&nbsp;</span><%#ERP.UI.Web.Common.WebControl.NumberSeparator(decimal.Parse(Eval("PurchaseCosts").ToString()))%>
                </ItemTemplate>
                <HeaderStyle ForeColor="red" />
                <ItemStyle ForeColor="red" />
                <FooterStyle ForeColor="red"></FooterStyle>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="&nbsp;= 毛利" UniqueName="Profit">
                <ItemTemplate>
                    <%#ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Profit"))%>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="毛利率" UniqueName="ProfitMargin">
                <ItemTemplate>
                    <%#string.Format("{0}{1}", (decimal.Parse(Eval("ProfitMargin").ToString()) * 100).ToString("F"), "%")%>
                </ItemTemplate>
            </rad:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</rad:RadGrid>
    
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_GrossProfit" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>

    <script type="text/javascript">

        //获取单选框的值
        function checkValue(type) {
            $("input[id$='Hid_SalePlatform']").val("");
            $("input[id$='Hid_OrderType']").val("");

            if (type === "SalePlatform") {
                $("input[id$='Hid_SalePlatform']").val("");

                $("input[type='checkbox'][name='SalePlatform']:checked:not([value='00000000-0000-0000-0000-000000000000'])").each(function () {
                    $("input[id$='Hid_SalePlatform']").val("," + $(this).val() + $("input[id$='Hid_SalePlatform']").val());
                });

                if ($("input[id$='Hid_SalePlatform']").val() !== "") {
                    $("input[id$='Hid_SalePlatform']").val($("input[id$='Hid_SalePlatform']").val().substring(1));
                }

                var salePlatformresult = 0;
                $("input[type='checkbox'][name='SalePlatform']:not([value='00000000-0000-0000-0000-000000000000'])").each(function () {
                    if ($(this).is(':checked') === false) {
                        salePlatformresult = 1;
                        return false;
                    }
                    return true;
                });

                if (salePlatformresult === 0) {
                    $("input[value='00000000-0000-0000-0000-000000000000'][name='SalePlatform']").prop('checked', true);
                } else {
                    $("input[value='00000000-0000-0000-0000-000000000000'][name='SalePlatform']").prop('checked', false);
                }
            } else if (type === "OrderType") {
                $("input[id$='Hid_OrderType']").val("");

                $("input[type='checkbox'][name='OrderType']:checked:not([value='-1'])").each(function () {
                    $("input[id$='Hid_OrderType']").val("," + $(this).val() + $("input[id$='Hid_OrderType']").val());
                });

                if ($("input[id$='Hid_OrderType']").val() !== "") {
                    $("input[id$='Hid_OrderType']").val($("input[id$='Hid_OrderType']").val().substring(1));
                }

                var orderTyperesult = 0;
                $("input[type='checkbox'][name='OrderType']:not([value='-1'])").each(function () {
                    if ($(this).is(':checked') === false) {
                        orderTyperesult = 1;
                        return false;
                    }
                    return true;
                });

                if (orderTyperesult === 0) {
                    $("input[value='-1'][name='OrderType']").prop('checked', true);
                } else {
                    $("input[value='-1'][name='OrderType']").prop('checked', false);
                }
            }
        }

        function checkAll(type) {
            if (type === "SalePlatform") {
                $("input[id$='Hid_SalePlatform']").val("");
                if ($("input[value='00000000-0000-0000-0000-000000000000'][name='SalePlatform']").is(':checked')) {
                    $("input[type='checkbox'][name='SalePlatform']").prop('checked', true);
                    $("input[type='checkbox'][name='SalePlatform']:not([value='00000000-0000-0000-0000-000000000000'])").each(function () {
                        $("input[id$='Hid_SalePlatform']").val("," + $(this).val() + $("input[id$='Hid_SalePlatform']").val());
                    });
                } else {
                    $("input[type='checkbox'][name='SalePlatform']").prop('checked', false);
                }
                if ($("input[id$='Hid_SalePlatform']").val() !== "") {
                    $("input[id$='Hid_SalePlatform']").val($("input[id$='Hid_SalePlatform']").val().substring(1));
                }
            }
            else if (type === "OrderType") {
                $("input[id$='Hid_OrderType']").val("");
                if ($("input[value='-1'][name='OrderType']").is(':checked')) {
                    $("input[type='checkbox'][name='OrderType']").prop('checked', true);
                    $("input[type='checkbox'][name='OrderType']:not([value='-1'])").each(function () {
                        $("input[id$='Hid_OrderType']").val("," + $(this).val() + $("input[id$='Hid_OrderType']").val());
                    });
                } else {
                    $("input[type='checkbox'][name='OrderType']").prop('checked', false);
                }
                if ($("input[id$='Hid_OrderType']").val() !== "") {
                    $("input[id$='Hid_OrderType']").val($("input[id$='Hid_OrderType']").val().substring(1));
                }
            }
        }

        function ShowValue(value, type) {
            if (value.length === 0) {
                return;
            }
            var arr = value.split(',');
            if (type === "SalePlatform") {
                $("input[type='checkbox'][name='SalePlatform']").each(function () {
                    if (arr.indexOf($(this).val()) > -1) {
                        $(this).attr('checked', true);
                    } else {
                        $(this).attr('checked', false);
                    }
                });

                var checkSalePlatformLength1 = $("input[type='checkbox'][name='SalePlatform']:not([value='00000000-0000-0000-0000-000000000000'])").length;
                var checkSalePlatformLength2 = $("input[type='checkbox'][name='SalePlatform']:checked").length;
                if (checkSalePlatformLength1 === checkSalePlatformLength2) {
                    $("input[value='00000000-0000-0000-0000-000000000000'][name='SalePlatform']").prop('checked', true);
                }
            }
            else if (type === "OrderType") {
                $("input[type='checkbox'][name='OrderType']").each(function () {
                    if (arr.indexOf($(this).val()) > -1) {
                        $(this).attr('checked', true);
                    } else {
                        $(this).attr('checked', false);
                    }
                });

                var checkOrderTypeLength1 = $("input[type='checkbox'][name='OrderType']:not([value='-1'])").length;
                var checkOrderTypeLength2 = $("input[type='checkbox'][name='OrderType']:checked").length;
                if (checkOrderTypeLength1 === checkOrderTypeLength2) {
                    $("input[value='-1'][name='OrderType']").prop('checked', true);
                }
            }
        }
    </script>
</asp:Content>

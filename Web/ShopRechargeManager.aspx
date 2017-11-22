<%@ Page Title="充值管理" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="ShopRechargeManager.aspx.cs" Inherits="ERP.UI.Web.ShopRechargeManager" %>
<%@ Import Namespace="AllianceShop.Enum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" language="javascript">
            function ShowRefundForm(rechargeId, saleFilialeId) {
                window.radopen("Windows/ReviewRechargeManager.aspx?RechargeId=" + rechargeId + "&SaleFilialeId=" + saleFilialeId, "AddExchanged");
                return false;
            }

            function clientShow(sender) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
        </script>
    </rad:RadScriptBlock>
    <table class="PanelArea">
        <tr>
            <td align="right">
                店铺类型：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbShopType" AutoPostBack="true" CausesValidation="False"
                    AllowCustomText="True" DataTextField="Value" DataValueField="Key"
                    Width="100px" Height="100px"  EnableLoadOnDemand="True"
                    OnSelectedIndexChanged="RcbShopTypeSelectedIndexChanged" >
                </rad:RadComboBox>
            </td>
             <td align="right">
                公司：
            </td>
            <td>
                <rad:RadComboBox ID="RcbSaleFiliale" runat="server" AutoPostBack="true" CausesValidation="False"
                    AllowCustomText="True" DataTextField="Value" DataValueField="Key"
                    Width="220px" Height="200px"  EnableLoadOnDemand="True"
                    OnItemsRequested="RcbSaleFilialeItemsRequested" OnSelectedIndexChanged="SearchIndexChanged" EmptyMessage="模糊搜索">
                </rad:RadComboBox>
            </td>
            <td align="right">
                状态：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbRechargeState">
                    <Items>
                        <rad:RadComboBoxItem Text="全部" Value="-1" />
                        <rad:RadComboBoxItem Text="待确认" Value="0" />
                        <rad:RadComboBoxItem Text="已支付" Value="1" />
                        <rad:RadComboBoxItem Text="拒绝" Value="2" />
                    </Items>
                </rad:RadComboBox>
            </td>
            <td align="right">
                充值时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RdpStartTime" runat="server" SkinID="Common" Width="100px">
                </rad:RadDatePicker>
                -
                <rad:RadDatePicker ID="RdpEndTime" runat="server" SkinID="Common" Width="100px">
                </rad:RadDatePicker>
            </td>
            <td>
            </td>
            <td colspan="2" align="center">
                <asp:Button runat="server" ID="BtnSearch" Text="搜索" Width="80px" 
                    onclick="BtnSearch_Click" />
            </td>
        </tr>
    </table>
    <rad:RadGrid runat="server" ID="RgRechargeList" OnNeedDataSource="RgExchangedApplyListNeedDataSource" SkinID="Common_Foot">
        <MasterTableView DataKeyNames="RechargeID" CommandItemSettings-ShowAddNewRecordButton="false">
            <Columns>
                <rad:GridBoundColumn DataField="No" ReadOnly="true" HeaderText="单据编号">
                    <HeaderStyle HorizontalAlign="Center" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="CreateTime" ReadOnly="true" HeaderText="时间">
                    <HeaderStyle HorizontalAlign="Center" Width="120" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ShopName" ReadOnly="true" HeaderText="充值门店">
                    <ItemStyle HorizontalAlign="Center" Width="80" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                 <rad:GridTemplateColumn DataField="Type" ReadOnly="true" HeaderText="充值方式">
                    <HeaderStyle HorizontalAlign="Center" Width="60" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ReturnType(Convert.ToInt32(Eval("Type")))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="Money" ReadOnly="true" HeaderText="充值金额">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Money"))%>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center" Width="100" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="State" ReadOnly="true" HeaderText="状态">
                    <HeaderStyle HorizontalAlign="Center" Width="60" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ReturnRName(Convert.ToInt32(Eval("State")))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Remark" ReadOnly="true" HeaderText="备注">
                    <ItemStyle HorizontalAlign="Center" Width="100" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <HeaderStyle HorizontalAlign="Center" Width="100" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:LinkButton runat="server" ID="LbExpressNo" Text="操作" Visible='<%# (int)Eval("State")==(int)RechargeState.WaitConfirm %>'
                             OnClientClick='<%# ShowShopRechargeManager(Eval("RechargeID")) %>' ></asp:LinkButton>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="ApplyWindowManager" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="AddExchanged" runat="server" Title="充值确认" Width="500px" Height="250px" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgRechargeList" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BtnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgRechargeList" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BtnAddApply">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgRechargeList" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RgRechargeList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgRechargeList" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbShopType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RcbSaleFiliale" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbSaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RcbSaleFiliale">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

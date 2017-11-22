<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostInvoiceMoney.aspx.cs"
    Inherits="ERP.UI.Web.CostInvoiceMoney" MasterPageFile="~/MainMaster.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <div class="StagePanel">
        <table>
            <tr>
                <td class="ShortFromRowTitle">
                    年份：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Year" runat="server" UseEmbeddedScripts="false" Width="150px"
                        Height="100px" AutoPostBack="True" ToolTip="下拉年份只往前推一年" ErrorMessage="请先选择年份">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">
                    月份：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Month" runat="server" UseEmbeddedScripts="false" Width="150px"
                        Height="240px" AutoPostBack="True" ToolTip="所有月份" ErrorMessage="请先选择月份">
                    </rad:RadComboBox>
                </td>
                <td style="width: 50px;">
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Button runat="server" ID="Btn_Search" OnClick="OnClick_Btn_Search" Text="搜  索"
                        Font-Bold="True"  ToolTip="无条件不搜索" />
                </td>
                <td style="width: 50px;">
                </td>
                <td style="width: 300px;">
                    <span style="font-size: 12px; font-weight: bold; color: gray">注：发票额度输入后即刻保存。</span>
                </td>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="RadGridCostInvoiceMoney" runat="server" OnNeedDataSource="RadCostInvoiceMoney_NeedDataSource">
        <MasterTableView DataKeyNames="FilialeId" ClientDataKeyNames="FilialeId">
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <CommandItemTemplate>
            </CommandItemTemplate>
            <Columns>
                <rad:GridBoundColumn DataField="FilialeName" HeaderText="公司" UniqueName="FilialeName">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="发票额度" UniqueName="CostInvoiceLimit">
                    <ItemTemplate>
                        <asp:TextBox ID="TB_CostInvoiceLimit" Width="150px" SkinID="ShortInput" runat="server"
                            Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Limit")) %>' OnTextChanged="OnTextChanged_CostInvoiceLimit" AutoPostBack="True"
                            onkeydown="if(event.keyCode==13)return false;" BorderStyle="Groove" ToolTip="费用申报发票额度" Enabled='<%#CheckIsCanSave()%>' >
                            
                        </asp:TextBox>
                    </ItemTemplate>
                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="已开金额" UniqueName="Else">
                    <ItemTemplate>
                        待收取: <asp:Label ID="LB_WaitCollect" runat="server" Text='<%# Eval("WaitCollect") %>'  ToolTip="待收取" />;
                        待核销: <asp:Label ID="LB_WaitChargeOff" runat="server" Text='<%# Eval("WaitChargeOff") %>'  ToolTip="待核销" />;
                        已核销: <asp:Label ID="LB_AlreadyChargeOff" runat="server" Text='<%# Eval("AlreadyChargeOff") %>'  ToolTip="已核销" />
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridCostInvoiceMoney" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RadGridCostInvoiceMoney">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridCostInvoiceMoney" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Year">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Year"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Month">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Month"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridCostInvoiceMoney" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridCostInvoiceMoney" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="TB_CostInvoiceLimit">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridCostInvoiceMoney" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

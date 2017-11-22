<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcurementCompactQuantity.aspx.cs"
    Inherits="ERP.UI.Web.ProcurementCompactQuantity" MasterPageFile="~/MainMaster.master" %>

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
                    供应商：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Width="150px"
                        Height="250px" AutoPostBack="True" Filter="StartsWith" EmptyMessage="供应商" ToolTip="供应商列表" >
                    </rad:RadComboBox>
                </td>
                <td style="width: 50px;">
                </td>
                <td class="AreaEditFromRowInfo">
                    <%--<asp:Button runat="server" ID="Btn_Search" OnClick="OnClick_Btn_Search" Text="搜  索"
                        BackColor="#808080" BorderStyle="Double" ToolTip="无条件不搜索" Font-Bold="True" />--%>
                        <asp:Button runat="server" ID="Btn_Search" OnClick="OnClick_Btn_Search" Text="搜  索"
                         ToolTip="无条件不搜索" Font-Bold="True" />
                </td>
                <td style="width: 20px;">
                </td>
                <td style="width: 300px;">
                    <span style="font-size: 12px; font-weight: bold; color: gray">注：合同签约金额输入后即刻保存。</span>
                </td>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="RadGridProcurementCompactQuantity" runat="server" OnNeedDataSource="RadGridProcurementCompactQuantity_NeedDataSource">
        <MasterTableView DataKeyNames="CompanyId" ClientDataKeyNames="CompanyId">
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <CommandItemTemplate>
            </CommandItemTemplate>
            <Columns>
                <rad:GridBoundColumn DataField="CompanyName" HeaderText="供应商" UniqueName="CompanyName">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="CompactMoney" HeaderText="合同签约金额" UniqueName="CompactMoney">
                    <ItemTemplate>
                        <asp:TextBox ID="TB_CompactMoney" Width="150px" SkinID="ShortInput" runat="server"
                            Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("CompactMoney")) %>' OnTextChanged="OnTextChanged_CompactMoney"
                            AutoPostBack="True" onkeydown="if(event.keyCode==13)return false;" BorderStyle="Groove"  ></asp:TextBox>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="FinishedCompactMoney" HeaderText="已完成签约金额" UniqueName="FinishedCompactMoney">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("FinishedCompactMoney"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="TheMonthProcurementMoney" HeaderText="当月采购金额" UniqueName="TheMonthProcurementMoney">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("TheMonthProcurementMoney"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" Font-Bold="True" ForeColor="Black"/>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="SurplusCompactMoney" HeaderText="剩余签约金额" UniqueName="SurplusCompactMoney">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("SurplusCompactMoney"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Percent" HeaderText="完成百分比" UniqueName="Percent">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementCompactQuantity" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RadGridProcurementCompactQuantity">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementCompactQuantity" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Company">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Company"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Year">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Year"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementCompactQuantity" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementCompactQuantity" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="TB_CompactMoney">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementCompactQuantity" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

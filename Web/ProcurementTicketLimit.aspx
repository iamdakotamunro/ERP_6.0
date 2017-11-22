<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ProcurementTicketLimit.aspx.cs"
    Inherits="ERP.UI.Web.ProcurementTicketLimit" MasterPageFile="~/MainMaster.master" %>

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
                <td style="width: 20px;">
                </td>
                <td style="width: 300px;">
                    <span style="font-size: 12px; font-weight: bold; color: gray">注：收票额度输入后即刻保存。</span>
                </td>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="RadGridProcurementTicketLimit" runat="server" OnNeedDataSource="RadProcurementTicketLimit_NeedDataSource">
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
                <rad:GridTemplateColumn  HeaderText="收票额度" UniqueName="TakerTicketLimit">
                    <ItemTemplate>
                        <asp:Label ID="TB_TakerTicketLimit" Width="150px" SkinID="ShortInput" runat="server"
                            Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("TotalTakerTicketLimit")) %>' Enabled="False"></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="供应商设置" UniqueName="DistributorSet">
                    <ItemTemplate>
                        <asp:ImageButton ID="LB_Auditing" runat="server" Text="供应商设置" SkinID="EditImageButton"
                            OnClientClick='<%# "return DistributorSetClick(\"" + Eval("FilialeId")+ "\",\"" + Eval("DateYear")+ "\",\"" + Eval("DateMonth")+ "\")" %>'>
                        </asp:ImageButton>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamOnAjaxRequest" >
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementTicketLimit" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RadGridProcurementTicketLimit">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementTicketLimit" LoadingPanelID="Loading">
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
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementTicketLimit" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementTicketLimit" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="TB_TakerTicketLimit">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridProcurementTicketLimit" LoadingPanelID="Loading">
                    </rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadWindowManager ID="RWM" runat="server">
        <Windows>
            <rad:RadWindow ID="RW1" Width="1200" Height="600" runat="server" Title="供应商收票额度设置" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript">

            function refreshGrid(arg) {
                if (!arg) {
                    window.$find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    window.$find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
            
            function DistributorSetClick(FilialeId, DateYear, DateMonth) {
                window.radopen("./Windows/CompanyProcurementTicketLimitForm.aspx?FilialeId=" + FilialeId + "&DateYear=" + DateYear + "&DateMonth=" + DateMonth, "RW1");
            }
            
        </script>
    </rad:RadScriptBlock>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

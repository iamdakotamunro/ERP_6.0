<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="CostCussentPage.aspx.cs" Inherits="ERP.UI.Web.CostCussentPage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <style type="text/css">
        .displaydiv div div
        {
            display: none;
        }
    </style>
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script language="javascript" type="text/javascript">
            var branchCombo;
            var positionCombo;
            function pageLoad() {
                branchCombo = $find("<%= RCB_BranchId.ClientID %>");
            }

            function LoadBranch(combo, eventArqs) {
                var item = eventArqs.get_item();
                branchCombo.clearSelection();
                branchCombo.set_text("Loading...");

                if (item.get_index() >= 0) {
                    branchCombo.requestItems(item.get_value(), false);
                }
            }

            function LoadPosition(combo, eventArqs) {
                var item = eventArqs.get_item();
                positionCombo.clearSelection();
                positionCombo.set_text("Loading...");
                positionCombo.requestItems(item.get_value(), false);
            }

            function ItemsLoaded(combo, eventArqs) {
                if (combo.get_items().get_count() > 0) {
                    combo.set_text(combo.get_items().getItem(0).get_text());
                }
                combo.showDropDown();
            }

            function OnSelectChanged(combo, eventArqs) {

                var item = eventArqs.get_item();

                var rgdGrid = $find("<%= RAM.ClientID %>");
                rgdGrid.ajaxRequest("OnSelectChanged?" + item.get_value());

            }
        </script>

    </rad:RadScriptBlock>
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <rad:RadTreeView ID="RTVCostCussent" runat="server" SkinID="Common" Width="280px"
                    OnNodeClick="RTVBankAccounts_NodeClick" OnInit="RTVCostCussent_Init">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <div style="float: right;">
                    公司：<rad:RadComboBox ID="RCB_FilialeId" runat="server" ShowToggleImage="True" OnClientSelectedIndexChanging="LoadBranch"
                        Height="180">
                    </rad:RadComboBox>
                    部门：<rad:RadComboBox ID="RCB_BranchId" runat="server" ShowToggleImage="True" OnClientItemsRequested="ItemsLoaded" Height="180">
                    </rad:RadComboBox>
                    &nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="LBUpdate" runat="server" OnClick="LB_Save_Click">
                        <asp:Image ID="IBUpdate" SkinID="updateImageButton" runat="server" ImageAlign="AbsMiddle" />
                        增加权限
                    </asp:LinkButton>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                </div>
                <rad:RadGrid ID="rgdGrid" runat="server" Width="99%" SkinID="Common_Foot" OnNeedDataSource="RgdGrid_NeedDataSource"
                    OnDeleteCommand="RgdGrid_DeleteCommand">
                    <MasterTableView DataKeyNames="CompanyId,FilialeId,BranchId" EditMode="InPlace">
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="CompanyName" HeaderText="费用单位" UniqueName="CussentName">
                                <HeaderStyle HorizontalAlign="Center" Width="40%" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="FilialeName" HeaderText="公司" UniqueName="BankName">
                                <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="BranchName" HeaderText="部门" UniqueName="BankName">
                                <HeaderStyle HorizontalAlign="Center" Width="25%" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" HeaderText="删除" ConfirmText="确实要删除吗？"
                                UniqueName="Delete">
                                <HeaderStyle />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridButtonColumn>
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
    <rad:RadAjaxManager ID="RAM" runat="server" EnableViewState="false" DefaultLoadingPanelID="Loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="rgdGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgdGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LBUpdate">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgdGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RTVCostCussent">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgdGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgdGrid"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

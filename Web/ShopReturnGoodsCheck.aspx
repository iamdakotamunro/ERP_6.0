<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="ShopReturnGoodsCheck.aspx.cs" Inherits="ERP.UI.Web.ShopReturnGoodsCheck" %>
<%@ Register TagPrefix="ibt" TagName="imagebuttoncontrol_1" Src="~/UserControl/ImageButtonControl.ascx" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
        <script src="JavaScript/common.js" type="text/javascript"></script>
        <script type="text/javascript" language="javascript">
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }

            function ShowClewForm(orderId, saleFilialeId) {
                window.radopen("Windows/ShowClewForm.aspx?OrderId=" + orderId + "&SaleFilialeId=" + saleFilialeId, "ClewWin");
                return false;
            }

            function ShowShopCheckForm(refundId) {
                window.radopen("Windows/ShopReturnGoodsCheckForm.aspx?RefundId=" + refundId, "CheckWin");
                return false;
            }
        </script>
    </rad:RadScriptBlock>
    <rad:RadGrid ID="RgShopRefund" runat="server" SkinID="Common_Foot" OnNeedDataSource="RgShopRefundNeedDataSource"
        OnItemCommand="RgShopRefundItemCommand">
        <MasterTableView DataKeyNames="RefundId" ClientDataKeyNames="RefundId">
            <CommandItemTemplate>
                退回时间：<rad:RadDatePicker ID="RdpStartTime" runat="server" SkinID="Common" Width="90px"
                    SelectedDate='<%# StartTime %>'>
                </rad:RadDatePicker>
                -&nbsp;<rad:RadDatePicker ID="RdpEndTime" runat="server" SkinID="Common" Width="90px"
                    SelectedDate='<%# EndTime %>'>
                </rad:RadDatePicker>
                &nbsp;&nbsp;检查状态：
                <asp:DropDownList ID="DdlCheckState" runat="server" SelectedValue='<%# SearchCheckState %>'>
                    <asp:ListItem Text="全部" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="待检查" Value="0"></asp:ListItem>
                    <asp:ListItem Text="检查通过" Value="1"></asp:ListItem>
                    <asp:ListItem Text="退回" Value="2"></asp:ListItem>
                </asp:DropDownList>
                <asp:TextBox ID="TbSearch" runat="server" SkinID="StandardInput" Text='<%# SearchKey %>'></asp:TextBox>
                &nbsp;&nbsp;
                <ibt:imagebuttoncontrol_1 id="LbSearch" runat="server" commandname="Search" validationgroup="Search"
                    skintype="Search" text="搜索"></ibt:imagebuttoncontrol_1>
                &nbsp;&nbsp;
                <ibt:imagebuttoncontrol_1 id="LbRefresh" runat="server" commandname="RebindGrid" causesvalidation="false"
                    skintype="Refresh" text="刷新"></ibt:imagebuttoncontrol_1>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridBoundColumn DataField="RefundNo" HeaderText="退换货号" UniqueName="RefundNo"
                    HeaderStyle-Width="180px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="售后店铺" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# string.Format("{0}",Eval("Consignee"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="售后时间" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# string.Format("{0}", Eval("CreateTime"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="检查状态" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <%# GetCheckState(Eval("CheckState"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="退回检查" UniqueName="Clew" HeaderStyle-HorizontalAlign="Center"
                    ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:ImageButton ID="IbtCheck" CommandName="Check" runat="server" SkinID="AffirmImageButton"
                            OnClientClick='<%# "return ShowShopCheckForm(\"" + Eval("RefundId")  + "\");" %>'
                            ToolTip="退回检查" />
                    </ItemTemplate>
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="Ram_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RgShopRefund">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgShopRefund" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgShopRefund" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager ID="ClewWindowManager" runat="server" Title="联盟店商品检查" Height="630px"
        Width="800px" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="ClewWin" runat="server" Title="备注" Height="500" Width="700" />
            <rad:RadWindow ID="CheckWin" runat="server" Title="退回检查" Height="500" Width="800" />
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

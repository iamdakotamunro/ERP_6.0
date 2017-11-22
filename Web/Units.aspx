<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.UnitsAw" Title="" Codebehind="Units.aspx.cs" %>
<%@Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" Runat="Server">
    <rad:RadGrid ID="UnitsGrid" runat="server"  SkinID="Common_Foot"
    OnNeedDataSource="UnitsGrid_NeedDataSource" OnInsertCommand="UnitsGrid_InsertCommand"
    OnUpdateCommand="UnitsGrid_UpdateCommand" OnDeleteCommand="UnitsGrid_DeleteCommand">
   <MasterTableView DataKeyNames="UnitsId"  EditMode="InPlace">
        <CommandItemTemplate>
            <Ibt:ImageButtonControl ID="LBAddRecord" runat="server" CommandName="InitInsert"
                Visible='<%# !UnitsGrid.MasterTableView.IsItemInserted %>' SkinType="Insert" Text="添加单位">
            </Ibt:ImageButtonControl>
            &nbsp;&nbsp;&nbsp;
            <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh">
            </Ibt:ImageButtonControl>
        </CommandItemTemplate>
        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
        <Columns>
            <rad:GridTemplateColumn HeaderText="数量单位" UniqueName="Units">
                <ItemTemplate>
                    <asp:Label ID="Lab_Units" runat="server" Text='<%# Eval("Units") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TB_Units" runat="server" SkinID="StandardInput" Text='<%# Bind("Units") %>'></asp:TextBox>
                </EditItemTemplate>
                <HeaderStyle Width="90%" />
            </rad:GridTemplateColumn>
            <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" EditText="编辑"
                InsertText="添加">
                <HeaderStyle Width="50px" />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridEditCommandColumn>
            <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="删除" ConfirmText="确实要删除吗？"
                UniqueName="Delete">
                <HeaderStyle Width="35px" />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridButtonColumn>
        </Columns>
    </MasterTableView>
</rad:RadGrid>
<rad:RadAjaxManager ID="RAM" runat="server">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="UnitsGrid">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="UnitsGrid" LoadingPanelID="loading" />
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
</rad:RadAjaxLoadingPanel>
</asp:Content>


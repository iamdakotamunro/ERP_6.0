<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="GoodsSeriesPage.aspx.cs" Inherits="ERP.UI.Web.GoodsSeriesPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">        
        <script type="text/javascript">
            function RowDblClick(obj, args) {
                var seriesId = args.getDataKeyValue("SeriesID");
                window.radopen("./Windows/EditGoodsSeriesForm.aspx?SeriedId=" + seriesId, "EditFrom");
                return false;
            }

            function ShowEditForm(seriesId) {
                if (seriesId && seriesId != "") {
                    window.radopen("./Windows/EditGoodsSeriesForm.aspx?SeriedId=" + seriesId, "EditFrom");
                } else {
                    alert("请先保存该商品系列后在进行属性绑定!");
                    return false;
                }
                return false;
            }
        </script>        
    </rad:RadScriptBlock>

<rad:RadGrid id="Grid_GoodsSeries" runat="server" SkinID="Common" ShowStatusBar="true" ShowGroupPanel="false"
    enableajaxloadingtemplate="True" OnNeedDataSource="GridGoodsSeries_OnNeedDataSource" OnItemCommand="GridGoodsSeries_OnItemCommand" OnUpdateCommand="GridGoodsSeries_OnUpdateCommand"
    OnDeleteCommand="GridGoodsSeries_OnDeleteCommand" OnInsertCommand="GridGoodsSeries_OnInsertCommand">
    <ClientSettings>
        <Selecting AllowRowSelect="true" EnableDragToSelectRows="false" />
        <ClientEvents OnRowDblClick="RowDblClick" />
    </ClientSettings>
    <MasterTableView EditMode="InPlace" DataKeyNames="SeriesID" ClientDataKeyNames="SeriesID">
    <CommandItemTemplate>
        按照系列名称搜索：
        <asp:TextBox ID="TB_Search" runat="server" SkinID="StandardInput" Text='<%# SeriesName %>'></asp:TextBox>
        <asp:ImageButton ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search" SkinID="SearchButton" Style='vertical-align: middle' />
        <asp:LinkButton ID="LB_Insert" runat="server" CommandName="InitInsert">
            <asp:Image ID="I_Insert" runat="server" SkinID="InsertImageButton" ImageAlign="AbsMiddle"></asp:Image>添加
        </asp:LinkButton>
        <asp:LinkButton ID="LB_Refresh" runat="server" CommandName="RebindGrid">
            <asp:Image ID="I_Refresh" runat="server" SkinID="RefreshImageButton" ImageAlign="AbsMiddle"></asp:Image>刷新
        </asp:LinkButton>
    </CommandItemTemplate>
    <CommandItemStyle HorizontalAlign="right" Height="26px" />
    <Columns>
            <rad:GridTemplateColumn DataField="SeriesName" HeaderText="系列名称" UniqueName="SeriesName">
                <ItemTemplate>
                    <asp:Label ID="LB_SeriesName" runat="server" Text='<%# Eval("SeriesName") %>'></asp:Label>
                </ItemTemplate>
                <EditItemTemplate>
                    <asp:TextBox ID="TB_SeriesName" runat="server" Text='<%# Bind("SeriesName") %>' Width="600px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFV_SeriesName" ErrorMessage="类型名称不能为空！" Text="*" ControlToValidate="TB_SeriesName" runat="server"></asp:RequiredFieldValidator>
                </EditItemTemplate>
                <HeaderStyle HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridTemplateColumn>            
            <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" EditText="编辑" InsertText="添加">
                <HeaderStyle />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridEditCommandColumn>
             
            <rad:GridTemplateColumn HeaderText="商品绑定" UniqueName="Modify">
            <ItemTemplate>
                <asp:ImageButton runat="server" CausesValidation="false" ID="IB_ModifyOrder" SkinID="InsertImageButton" OnClientClick='<%# "return ShowEditForm(\""+Eval("SeriesID")+"\")" %>'  />
            </ItemTemplate>
            <HeaderStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
        </rad:GridTemplateColumn>
            <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="删除" ConfirmText="确实删除吗？" UniqueName="Delete">
            <HeaderStyle />
            <ItemStyle HorizontalAlign="Center" />
        </rad:GridButtonColumn>
    </Columns>
</MasterTableView>
</rad:RadGrid>

<rad:RadWindowManager ID="StockWindowManager" runat="server" Height="577px" Width="900px" ReloadOnShow="true">
    <Windows>
        <rad:RadWindow ID="EditFrom" runat="server" Title="" />
    </Windows>
</rad:RadWindowManager>

<rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="Grid_GoodsSeries">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="Grid_GoodsSeries" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

</asp:Content>

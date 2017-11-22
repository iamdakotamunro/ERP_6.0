<%@ Page Title="加盟店广告管理" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="ShopActivityNoticeManager.aspx.cs" Inherits="ERP.UI.Web.ShopActivityNoticeManager" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<%@ Register Src="~/UserControl/ConfirmCheckBox.ascx" TagName="ConfirmCheckBox" TagPrefix="kd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript">
            function ShowInsertForm() {
                window.radopen("Windows/AddActivityNotice.aspx", "");
                return false;
            }

            function RowDblClick(obj, args) {
                var noteId = args.getDataKeyValue("NoticeID");
                window.radopen("Windows/AddActivityNotice.aspx?NoteId=" + noteId, "");
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
            function OnDeleteConfirm() {
                var conf = window.confirm('提示：是否确认删除吗？');
                if (!conf)
                    return false;
                return true;
            }

        </script>
    </rad:RadScriptBlock>
    <rad:RadGrid ID="NoteGrid" runat="server" SkinID="Common_Foot" OnNeedDataSource="NoteGrid_NeedDataSource"
        OnDeleteCommand="NoteGrid_DeleteCommand">
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>
        <MasterTableView DataKeyNames="NoticeID,OrderIndex" ClientDataKeyNames="NoticeID,OrderIndex">
            <CommandItemTemplate>
                <Ibt:ImageButtonControl ID="LBAddRecord" Text="添加公告" OnClientClick="return ShowInsertForm()"
                    SkinType="Insert" runat="server"></Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="序号">
                    <ItemTemplate>
                        <%#(int)DataBinder.Eval(Container, "DataSetIndex")+1%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="NoticeTitle" HeaderText="文字标题" UniqueName="NoticeTitle">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="CreateTime" HeaderText="创建时间" UniqueName="CreateTime">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Left" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="IsNotice" HeaderText="公告" UniqueName="IsNotice">
                    <ItemTemplate>
                        <kd:ConfirmCheckBox ID="ccbIsNotice" OnCheckedChanged="IsNotice_CheckedChanged" Checked='<%# Convert.ToBoolean(Eval("IsNotice")) %>'
                            runat="server" />
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="IsShow" HeaderText="显示" UniqueName="IsShow">
                    <ItemTemplate>
                        <kd:ConfirmCheckBox ID="ccbIsShow" OnCheckedChanged="IsShow_CheckedChanged" Checked='<%# Convert.ToBoolean(Eval("IsShow")) %>'
                            runat="server" />
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="排序">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_OrderIndex" runat="server" Text='<%#Eval("OrderIndex")%>' OnTextChanged="txt_OrderIndex_OnTextChanged" AutoPostBack="True"></asp:TextBox>
                    </ItemTemplate>
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" HeaderText="删除" Text="删除" ConfirmText="确实要删除吗？"
                    UniqueName="Delete">
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridButtonColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" OnAjaxRequest="RadGridAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="NoteGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="NoteGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="NoteGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadWindowManager ID="WMGoods" runat="server" useembeddedscripts="false" Height="640px"
        Width="1055px" ReloadOnShow="true" useclassicwindows="false">
        <Windows>
            <rad:RadWindow ID="GoodsDialog" runat="server" Title="产品信息" />
            <rad:RadWindow ID="RwInfomation" runat="server" Width="700" Height="400" Title="资料上传" />
            <rad:RadWindow ID="EditGoodsAttributeForm" runat="server" Width="350" Height="400"
                Title="属性编辑" />
            <rad:RadWindow ID="ApplySaleStockForm" runat="server" Width="500" Height="300" Title="卖库存商品信息" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

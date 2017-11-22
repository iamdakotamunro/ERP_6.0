<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.BankAccountsAw" Title="Untitled Page" CodeBehind="BankAccounts.aspx.cs" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowInsertForm() {
                window.radopen("./Windows/EditBankAccountsForm.aspx", "InsertBA");
                return false;
            }

            function RowDblClick(obj, args) {
                window.radopen("./Windows/EditBankAccountsForm.aspx?BankAccountsId=" + args.getDataKeyValue("BankAccountsId"), "EditBA");
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
    <rad:RadGrid ID="BankAccountsGrid" runat="server" SkinID="Common_Foot" OnNeedDataSource="BankAccountsGrid_NeedDataSource"
        OnDeleteCommand="BankAccountsGrid_DeleteCommand">
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>
        <GroupPanel Visible="True">
        </GroupPanel>
        <MasterTableView DataKeyNames="BankAccountsId" ClientDataKeyNames="BankAccountsId">
            <CommandItemTemplate>
                <Ibt:ImageButtonControl ID="LBAddRecord" runat="server" CommandName="InitInsert"
                    OnClientClick="return ShowInsertForm();" Visible='<%# !BankAccountsGrid.MasterTableView.IsItemInserted %>'
                    SkinType="Insert" Text="添加账号"></Ibt:ImageButtonControl>
                &nbsp;&nbsp;&nbsp;
                <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                    Text="刷新"></Ibt:ImageButtonControl>
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
                <rad:GridBoundColumn DataField="BankName" HeaderText="机构名称" UniqueName="BankName">
                    <HeaderStyle Width="250px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="AccountsName" HeaderText="账户名称" UniqueName="AccountsName">
                    <ItemTemplate>
                        <asp:Label ID="AccountsNameLabel" runat="server" Text='<%# (Eval("AccountsName").ToString()).Replace(Eval("BankName")+"(","").Replace(")","") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Accounts" HeaderText="账号" UniqueName="Accounts">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="PaymentType" HeaderText="支付类型" UniqueName="PaymentType">
                    <ItemTemplate>
                        <asp:Label ID="PaymentTypeLabel" runat="server" Text='<%# GetPaymentTypeName(Eval("PaymentType")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="IsUse" UniqueName="IsUse" HeaderText="是否启用">
                    <ItemTemplate>
                        <asp:CheckBox ID="CB_IsUse" runat="server" AutoPostBack="true" Enabled="false" Checked='<%# Boolean.Parse(Eval("IsUse").ToString()) %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="65px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="IsBacktrack" UniqueName="IsBacktrack" HeaderText="原路返回">
                    <ItemTemplate>
                        <asp:CheckBox ID="CB_IsBacktrack" runat="server" AutoPostBack="true" Enabled="false"
                            Checked='<%# Boolean.Parse(Eval("IsBacktrack").ToString()) %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="65px" HorizontalAlign="Center" />
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
                    <HeaderStyle Width="40px" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridButtonColumn>
            </Columns>
            <ExpandCollapseColumn Visible="False" Resizable="False">
                <HeaderStyle Width="20px" />
            </ExpandCollapseColumn>
            <RowIndicatorColumn Visible="False">
                <HeaderStyle Width="20px" />
            </RowIndicatorColumn>
        </MasterTableView>
        <ExportSettings>
            <Pdf PageBottomMargin="" PageFooterMargin="" PageHeaderMargin="" PageHeight="11in"
                PageLeftMargin="" PageRightMargin="" PageTopMargin="" PageWidth="8.5in" />
        </ExportSettings>
    </rad:RadGrid>
    <rad:RadWindowManager ID="RWM" runat="server" Height="520px" Width="920px" VisibleStatusbar="false"
        ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="InsertBA" runat="server" Title="增加资金账户" />
            <rad:RadWindow ID="EditBA" runat="server" Title="编辑资金账户" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="BankAccountsGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BankAccountsGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="BankAccountsGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

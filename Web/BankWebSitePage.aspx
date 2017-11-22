<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MainMaster.master"
    CodeBehind="BankWebSitePage.aspx.cs" Inherits="ERP.UI.Web.BankWebSitePage" %>

<%@ Register TagPrefix="wx" TagName="OverwriteCheckBox" Src="~/UserControl/OverwriteCheckBox.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <rad:RadTreeView ID="RTVWebSite" runat="server" useembeddedscripts="false" Height="500px"
                    Width="200px" Autopostback="true" CausesValidation="True" OnNodeClick="RtvWebSite_NodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RG_BankWebSite" runat="server" SkinID="Common_Foot" OnNeedDataSource="Rg_BankWebSite_NeedDataSource" OnInsertCommand="Rg_BankWebSite_InsertCommand">
                    <MasterTableView DataKeyNames="BankAccountsId,TargetId">
                        <CommandItemTemplate>
                              <asp:LinkButton ID="LinkButtonAddRecord" runat="server" CommandName="InitInsert"
                                Visible='<%# !RG_BankWebSite.MasterTableView.IsItemInserted %>'>
                                <asp:Image ID="AddRecord" runat="server" ImageAlign="AbsMiddle" SkinID="AddImageButton" />添加
                            </asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid">
                                <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />刷新
                            </asp:LinkButton>
                        </CommandItemTemplate>
                        <Columns>
                            <rad:GridTemplateColumn HeaderText="所属公司" UniqueName="filialeName">
                                <ItemTemplate>
                                    <asp:Label ID="lbFilialeName" runat="server" Text='<%# GetFilialeName() %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lbFilialeNameE" runat="server" Text='<%# GetFilialeName() %>'></asp:Label>
                                </EditItemTemplate>
                                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="销售平台" UniqueName="FromSourceName">
                                <ItemTemplate>
                                    <asp:Label ID="lbFromSourceName" runat="server" Text='<%# GetFromSourceName(Eval("BankAccountsId").ToString()) %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lbFromSourceNameE" runat="server" Text='<%# GetFromSourceName(Eval("BankAccountsId").ToString() ) %>'></asp:Label>
                                </EditItemTemplate>
                                <HeaderStyle Width="10%" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="账户">
                                <ItemTemplate>
                                    <asp:Label ID="Label3" runat="server" Text='<%# Eval("Accounts") %>'></asp:Label>
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="lbOldBankAccountsId" runat="server" Text='<%# Eval("BankAccountsId") %>'
                                        Style="display: none;"></asp:Label>
                                    <asp:DropDownList ID="DDL_Bank" Width="100%" runat="server" DataSource="<%#GetBankAccountsList()%>"
                                        DataTextField="Accounts" DataValueField="BankAccountsId" SkinID="LongDropDown"
                                        AppendDataBoundItems="true">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                    </asp:DropDownList>
                                    <asp:RequiredFieldValidator ID="RFV_Bank" runat="server" ErrorMessage="必须选择资金银行"
                                        Text="*" ControlToValidate="DDL_Bank"></asp:RequiredFieldValidator>
                                </EditItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn ReadOnly="true" DataField="AccountsName" HeaderText="用户名">
                                <HeaderStyle Width="20%" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="20%" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn ReadOnly="true" DataField="BankName" HeaderText="银行">
                                <HeaderStyle HorizontalAlign="Center" Width="180px" />
                                <ItemStyle HorizontalAlign="Center" Width="20%" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="IsMain" UniqueName="IsMain" HeaderText="主账号">
                                <ItemTemplate>
                                    <wx:OverwriteCheckBox ID="ccb" runat="server" Checked='<%# Boolean.Parse(Eval("IsMain").ToString()) %>'
                                        OnCheckedChanged="CB_IsMain_OnCheckedChanged" Enabled='<%# Boolean.Parse(Eval("IsMain").ToString()) %>' />
                                </ItemTemplate>
                                <EditItemTemplate>
                                    <asp:Label ID="Lable_IsMain" runat="server"></asp:Label>
                                </EditItemTemplate>
                                <HeaderStyle Width="65px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridEditCommandColumn ButtonType="ImageButton" HeaderText="操作" CancelText="取消"
                                EditText="编辑" InsertText="添加" UpdateText="更新" UniqueName="Operation" Visible="False">
                                <HeaderStyle Width="50px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridEditCommandColumn>
                            <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="删除" ConfirmText="确实要删除吗？"
                                UniqueName="Delete" Visible="False">
                                <HeaderStyle Width="35px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridButtonColumn>
                            <rad:GridTemplateColumn HeaderText="转移帐号" UniqueName="Transfer">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Transfer" SkinID="AffirmImageButton"
                                        Text="转移帐号" OnClientClick='<%# "return TransferClick(\"" + Eval("BankAccountsId")+ "\")" %>' />
                                </ItemTemplate>
                                <HeaderStyle Width="220px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript">

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }

            function TransferClick(BankAccountsId) {
                window.radopen("./Windows/TransferBankAccountsForm.aspx?BankAccountsId=" + BankAccountsId, "TransferWindow");
                return false;
            }

        </script>
    </rad:RadScriptBlock>
    <rad:RadWindowManager ID="RW1" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="TransferWindow" Width="380" Height="300" runat="server" Title="转移资金帐号" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server"  OnAjaxRequest="RamAjaxRequest" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RTVWebSite">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_BankWebSite" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
             <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_BankWebSite" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RG_BankWebSite">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_BankWebSite" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="CB_IsMain">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_BankWebSite" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

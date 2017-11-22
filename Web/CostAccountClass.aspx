<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.CostAccountClassAw" Title="Untitled Page" CodeBehind="CostAccountClass.aspx.cs" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <rad:RadTreeView ID="RT_CompanyClass" runat="server" SkinID="Common" OnNodeClick="RT_CompanyClass_NodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RGCussent" runat="server" SkinID="Common_Foot" OnNeedDataSource="RGCussent_NeedDataSource"
                    OnDeleteCommand="RGCussent_DeleteCommand">
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="CompanyId" ClientDataKeyNames="CompanyId">
                        <CommandItemTemplate>
                            <Ibt:ImageButtonControl ID="LB_AddRecord" runat="server" Visible='<%# !RGCussent.MasterTableView.IsItemInserted %>'
                                SkinType="Insert" OnClientClick="AddClick()" Text="添加费用类型"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LB_Refresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="CompanyName" HeaderText="公司名称" UniqueName="CompanyName">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Linkman" HeaderText="联系人" UniqueName="Linkman">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Phone" HeaderText="联系电话" UniqueName="Phone">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Mobile" HeaderText="手机" UniqueName="Mobile" MaxLength="16">
                                <HeaderStyle Width="130px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="Email" HeaderText="电子邮件" UniqueName="Email">
                                <ItemTemplate>
                                    <asp:HyperLink ID="HL_Email" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Email").ToString()) %>'
                                        NavigateUrl='<%# "mailto:"+Eval("Email") %>'>
                                        <asp:Image ID="Email" runat="server" SkinID="EmailImg" /></asp:HyperLink>
                                </ItemTemplate>
                                <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn  HeaderText="编辑" >
                                <ItemTemplate>
                                    <asp:ImageButton ID="LB_Auditing" runat="server" Text="编辑" SkinID="EditImageButton"
                                        OnClientClick='<%# "return EditClick(\"" + Eval("CompanyId")+ "\")" %>'></asp:ImageButton>
                                </ItemTemplate>
                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="删除" ConfirmText="确实要删除吗？"
                                UniqueName="Delete" HeaderText="删除">
                                <HeaderStyle Width="40px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridButtonColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript">
            //添加费用分类
            function AddClick() {
                window.radopen("./Windows/CostCompanyForm.aspx", "RW1");
            }
            //编辑费用分类
            function EditClick(companyId) {
                window.radopen("./Windows/CostCompanyForm.aspx?CompanyId=" + companyId, "RW1");
            }
            //双击编辑费用分类
            function RowDblClick(obj, args) {
                var companyId = args.getDataKeyValue("CompanyId");
                window.radopen("./Windows/CostCompanyForm.aspx?CompanyId=" + companyId, "RW1");
            }
            //刷新
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
    <rad:RadWindowManager ID="RWM" runat="server" Height="240px" Width="400px">
        <Windows>
            <rad:RadWindow ID="RW1" runat="server" Width="980" Height="660" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCussent" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RT_CompanyClass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCussent" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGCussent">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCussent" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

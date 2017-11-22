<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.CompanyCussentAw" Title="Untitled Page" CodeBehind="CompanyCussent.aspx.cs" %>

<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <rad:RadTreeView ID="RT_CompanyClass" runat="server" SkinID="Common" AutoPostBack="true"
                    CausesValidation="True" OnNodeClick="RtCompanyClassNodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RGCussent" runat="server" SkinID="Common_Foot" OnNeedDataSource="RgCussentNeedDataSource"
                    OnDeleteCommand="RgCussentDeleteCommand" OnItemCommand="OnItemRgCussent">
                    <ClientSettings>
                        <Selecting AllowRowSelect="True" EnableDragToSelectRows="false" />
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="CompanyId" ClientDataKeyNames="CompanyId">
                        <CommandItemTemplate>
                            <asp:TextBox ID="TB_Search" Width="250px" runat="server"  Text='<%# SearchKey %>' ></asp:TextBox>
                            <Ibt:ImageButtonControl ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search"
                                SkinType="Search" Text="搜索"></Ibt:ImageButtonControl>
                            <Ibt:ImageButtonControl ID="LB_AddRecord" runat="server" OnClientClick="AddClick()"
                                Visible='<%# !RGCussent.MasterTableView.IsItemInserted %>' SkinType="Insert"
                                Text="添加往来单位"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="CompanyName" HeaderText="公司名称" UniqueName="CompanyName">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="SalesScope" HeaderText="销售范围" UniqueName="SalesScope">
                                <ItemTemplate>
                                    <asp:Label ID="CompanyLabel" runat="server" Text='<%# Eval("SalesScope").ToString() == "0"?"境内":"境外"%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
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
                                    <asp:HyperLink ID="HL_Email" runat="server" Visible='<%# string.IsNullOrEmpty(Eval("Email").ToString()) ? false : true %>'
                                        NavigateUrl='<%# "mailto:"+Eval("Email") %>'>
                                        <asp:Image ID="Email" runat="server" SkinID="EmailImg" /></asp:HyperLink>
                                </ItemTemplate>
                                <HeaderStyle Width="140px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="差额说明" UniqueName="Subject">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CausesValidation="false" ID="IbSubject" SkinID="InsertImageButton"
                                        Text="差额说明" OnClientClick='<%# "return ShowSubjectDiscount(\"" + Eval("CompanyId")+ "\",1)" %>' />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="50px" />
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="折扣说明" UniqueName="DiscountMemo">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CausesValidation="false" ID="IbDiscount" SkinID="InsertImageButton"
                                        Text="折扣说明" OnClientClick='<%# "return ShowSubjectDiscount(\"" + Eval("CompanyId")+ "\",2)" %>' />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="50px" />
                                <ItemStyle HorizontalAlign="Center" Width="50px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="编辑" UniqueName="EditClick">
                                <ItemTemplate>
                                    <asp:ImageButton ID="LB_Auditing" runat="server" Text="编辑" SkinID="EditImageButton"
                                        OnClientClick='<%# "return EditClick(\"" + Eval("CompanyId")+ "\")" %>'></asp:ImageButton>
                                    <asp:ImageButton ID="IB_Approved" runat="server" SkinID="Auth"
                                        OnClientClick='<%# "return ShowAuditingForm(\"" + Eval("CompanyId") + "\");" %>' Visible='<%# Convert.ToInt32(Eval("SalesScope"))==(int)ERP.Enum.Overseas.SalesScopeType.Overseas %>'/>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="删除" ConfirmText="确实要删除吗？"
                                UniqueName="Delete">
                                <HeaderStyle Width="35px" />
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
            function AddClick() {
                window.radopen("./Windows/CompanyCussentControl.aspx", "RW1");
            }

            function ShowInformationForm(companyId) {
                window.radopen("./Windows/CompanyInfomation.aspx?CompanyId=" + companyId, "RW2");
            }

            function EditClick(companyId) {
                    window.radopen("./Windows/CompanyCussentControl.aspx?CompanyId=" + companyId, "RW1");
            }

            function RowDblClick(obj, args) {
                var companyId = args.getDataKeyValue("CompanyId");
                window.radopen("./Windows/CompanyCussentControl.aspx?CompanyId=" + companyId, "RW1");
            }

            function ShowSubjectDiscount(companyId,type) {
                window.radopen("./Windows/ShowReckoning.aspx?CompanyId=" + companyId + "&Type=" + type , "ShowReckoningInfoFormPage");
            }

            function ShowAuditingForm(companyId) {
                window.radopen("./Windows/CompanyCussentAuthorityPage.aspx?CompanyId=" + companyId, "RW3");
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
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCussent" LoadingPanelID="loading" />
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
    <rad:RadWindowManager ID="TransferWindowManager" runat="server" Height="240px" Width="400px"
        ReloadOnShow="true" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="RW1" runat="server" Title="" Width="980" Height="500" />
            <rad:RadWindow ID="RW2" runat="server" Title="" Width="800" Height="500" />
            <rad:RadWindow ID="RW3" runat="server" Title="权限设置" Width="1024" Height="600"/>
            <rad:RadWindow ID="ShowReckoningInfoFormPage" Width="700" Height="500" runat="server"
                Title="往来信息" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

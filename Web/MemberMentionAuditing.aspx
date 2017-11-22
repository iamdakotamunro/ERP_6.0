<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="MemberMentionAuditing.aspx.cs" Inherits="ERP.UI.Web.MemberMentionAuditing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table class="PanelArea">
        <tr>
            <td>
                销售公司:
            </td>
            <td>
                <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" Width="120px"
                    Height="100px" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged"
                    AutoPostBack="True" EmptyMessage="请选择销售公司">
                </rad:RadComboBox>
            </td>
            <td>
                销售平台:
            </td>
            <td>
                <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false"
                    Width="120px" Height="100px" OnSelectedIndexChanged="RCB_SalePlatform_OnSelectedIndexChanged"
                    AutoPostBack="True" EmptyMessage="请选择销售平台">
                </rad:RadComboBox>
            </td>
            <td>
                会员名称：
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="会员名称" ID="RTB_Member" Width="250px"
                    Enabled="False" ToolTip="会员名搜索需选择具体的销售平台"  />
            </td>
            <td>
                时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
                <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
            </td>
            <td>
                状态：
            </td>
            <td>
                <asp:DropDownList ID="DDL_Select" runat="server">
                    <asp:ListItem Text="待审核" Value="1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="待打款" Value="0"></asp:ListItem>
                    <asp:ListItem Text="完成提现" Value="2"></asp:ListItem>
                    <asp:ListItem Text="提现退回" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td>
                <asp:ImageButton Style='vertical-align: middle' ID="LB_Search" runat="server" ValidationGroup="Search"
                    SkinID="SearchButton" OnClick="LbSearchClick" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_MemberMentionApply" runat="server" SkinID="CustomPaging" OnNeedDataSource="RgMemberMentionApplyNeedDataSource">
        <ClientSettings>
            <Selecting AllowRowSelect="True" EnableDragToSelectRows="false" />
        </ClientSettings>
        <MasterTableView DataKeyNames="Id,MemberId" ClientDataKeyNames="Id,MemberId" NoMasterRecordsText="无可用记录。"
            CommandItemDisplay="None">
            <Columns>
                <rad:GridTemplateColumn UniqueName="ApplyTime" HeaderText="时间">
                    <ItemTemplate>
                        <asp:Label ID="LB_ApplyTime" runat="server" Text='<%# DateTime.Parse(Eval("ApplyTime").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("ApplyTime").ToString()).ToString() %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="UserName" UniqueName="UserName" HeaderText="会员名">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn UniqueName="Amount" HeaderText="金额（元）">
                    <ItemTemplate>
                        <asp:Label ID="LB_Amount" runat="server" Text='<%# Convert.ToDecimal(Eval("Amount"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn UniqueName="Balance" HeaderText="账户余额（元）">
                    <ItemTemplate>
                        <asp:Label ID="LB_Balance" runat="server" Text='<%# Convert.ToDecimal(Eval("Balance"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Balance")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="状态" UniqueName="State">
                    <ItemTemplate>
                        <asp:Label ID="LB_State" runat="server" Text='<%# GetState(Eval("State")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注" UniqueName="Description">
                    <ItemTemplate>
                        <asp:Label ID="LB_Memo" runat="server" Text='<%# Eval("Description").ToString() %>'></asp:Label>
                        <asp:ImageButton ID="ImageButton1" SkinID="EditImageButton" OnClientClick='<%# "return AddMemoClick(\"" + Eval("Id")+ "\",\""+Eval("SalePlatformId")+"\")" %>'
                            runat="server" ToolTip="添加备注"/>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="440" />
                    <ItemStyle HorizontalAlign="Center" Width="440" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="站点" UniqueName="FilialeName">
                    <ItemTemplate>
                        <%#GetFromSourceNameById(Eval("SaleFilialeId"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Execute" SkinID="AffirmImageButton"
                            Text="审核" OnClientClick='<%# "return DeclineClick(\"" + Eval("Id")+ "\",\"" + Eval("MemberId")+ "\",\""+Eval("SalePlatformId")+"\")" %>'
                            Visible='<%# int.Parse(Eval("State").ToString())==1 %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript">

            function DeclineClick(applyId, memberId, salePlatformId) {
                window.radopen("./Windows/MemberMentionAuditingDetail.aspx?Type=1&ApplyId=" + applyId + "&MemberId=" + memberId + "&SalePlatformId=" + salePlatformId, "RW2");
            }

            function AddMemoClick(applyId, salePlatformId) {
                window.radopen("./Windows/MemberMentionAddMemo.aspx?Type=2&ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW1");
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
             <rad:AjaxSetting AjaxControlID="RG_MemberMentionApply">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_MemberMentionApply" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_MemberMentionApply" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_MemberMentionApply" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
               <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTB_Member" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
              <rad:AjaxSetting AjaxControlID="RCB_SalePlatform">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RTB_Member" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadWindowManager ID="TransferWindowManager" runat="server" Height="180px" Width="400px"
        ReloadOnShow="true" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="RW1" runat="server" Title="添加备注" Width="400" Height="180" />
            <rad:RadWindow ID="RW2" runat="server" Title="审核操作" Width="985" Height="580" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

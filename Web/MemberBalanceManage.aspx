<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberBalanceManage.aspx.cs"
    Inherits="ERP.UI.Web.MemberBalanceManage" MasterPageFile="~/MainMaster.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <div class="StagePanel">
        <table class="PanelArea">
            <tr>
                <td class="ShortFromRowTitle">销售公司:
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" Width="150px"
                        Height="100px" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged"
                        AutoPostBack="True" EmptyMessage="请选择销售公司">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">销售平台:
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false"
                        Width="150px" Height="100px" OnSelectedIndexChanged="RCB_SalePlatform_OnSelectedIndexChanged"
                        AutoPostBack="True" EmptyMessage="请选择销售平台">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">会员名称:
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Member" runat="server" AutoPostBack="true" CausesValidation="false"
                        AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="UserName" DataValueField="MemberId"
                        Width="150px" Height="200px" OnItemsRequested="RCB_MemberItemsRequested" EmptyMessage="输入会员名搜索"
                        Enabled="False" ToolTip="会员名搜索需选择具体的销售平台">
                    </rad:RadComboBox>
                    <%--<rad:RadTextBox runat="server" EmptyMessage="会员名称" ID="RadTextBox1" Width="145px"/>--%>
                </td>
                <td class="ShortFromRowTitle">单据编号:
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadTextBox runat="server" EmptyMessage="单据编号" ID="RTB_ReceiptNo" Width="150px" />
                </td>
                <td class="ShortFromRowTitle">是否官方:
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:RadioButtonList ID="rbl_IsOfficial" runat="server" RepeatDirection="Horizontal">
                        <asp:ListItem Text="是" Value="True" Selected="true"></asp:ListItem>
                        <asp:ListItem Text="否" Value="False"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
                <td>
                    <asp:ImageButton ID="IB_Search" runat="server" SkinID="SearchButton" OnClick="IB_Search_Click"
                        ValidationGroup="Search" />
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td class="ShortFromRowTitle">时间条件:
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="91px">
                    </rad:RadDatePicker>
                    <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="91px">
                    </rad:RadDatePicker>
                </td>
                <td class="ShortFromRowTitle">操作类型:
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Type" runat="server" Width="150px" Height="120px" DataValueField="Key"
                        DataTextField="Value" EmptyMessage="操作类型">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">操作状态:
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_State" runat="server" Width="150px" Height="120px" DataValueField="Key"
                        DataTextField="Value" EmptyMessage="状态">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">问题类型：
                </td>
                <td>
                    <rad:RadComboBox ID="RcbProblemId" runat="server" Width="150px" Height="120px" DataValueField="Key"
                        DataTextField="Value" EmptyMessage="问题类型">
                    </rad:RadComboBox>
                </td>
                <td>
                    <asp:Button ID="btn_ExportExcel" runat="server" Text="导出Excel" OnClick="btn_ExportExcel_Click" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="RadGridMemberBalanceManage" runat="server" SkinID="CustomPaging"
        OnNeedDataSource="RadGridMemberBalanceManage_NeedDataSource" OnPageIndexChanged="RadGridMemberBalanceManage_OnPageIndexChanged">
        <MasterTableView DataKeyNames="" ClientDataKeyNames="">
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <CommandItemTemplate>
            </CommandItemTemplate>
            <Columns>
                <rad:GridBoundColumn DataField="TradeCode" HeaderText="单据编号" UniqueName="TradeCode">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ApplyTime" HeaderText="申请时间" UniqueName="ApplyTime">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="UserName" HeaderText="会员名称" UniqueName="UserName">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ApplicantName" HeaderText="提交人" UniqueName="ApplicantName">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="BankAccountId" HeaderText="银行名称" UniqueName="BankAccountId">
                    <ItemTemplate>
                        <span style="font-weight: bold;">
                            <%# GetBankAccountName(Eval("BankAccountId"), Eval("PayBankName"))%></span>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="PayTreasureAccount" HeaderText="支付宝\银行帐号" UniqueName="PayTreasureAccount">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="TypeOfProblemName" HeaderText="问题类型" UniqueName="TypeOfProblemName">
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="Increase" HeaderText="存入（元）" UniqueName="Increase">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Increase"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="Subtract" HeaderText="支出（元）" UniqueName="Subtract">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Subtract"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="CurrentBalance" HeaderText="当前余额（元）" UniqueName="CurrentBalance">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("CurrentBalance"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="State" HeaderText="状态" UniqueName="State">
                    <ItemTemplate>
                        <asp:Label ID="LB_State" runat="server" Text='<%# GetState(Eval("State")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Remark" HeaderText="备注" UniqueName="Remark">
                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="操作" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Execute" SkinID="AffirmImageButton"
                            Text="审核" OnClientClick='<%# "return AffirmClick(\"" + Eval("ApplyID")+ "\",\""+Eval("SalePlatformId")+"\")" %>'
                            Visible='<%# int.Parse(Eval("State").ToString())==2 && GetPowerOperationPoint() %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <div style="display: none;">
        <asp:GridView ID="ExportExcel" runat="server" AutoGenerateColumns="False">
            <Columns>
                <asp:BoundField DataField="TradeCode" HeaderText="单据编号" />
                <asp:BoundField DataField="ApplyTime" HeaderText="申请时间" DataFormatString="{0:yyyy-MM-dd}" />
                <asp:BoundField DataField="UserName" HeaderText="会员名称" />
                <asp:BoundField DataField="ApplicantName" HeaderText="提交人" />
                <asp:TemplateField HeaderText="银行名称">
                    <ItemTemplate>
                        <%# GetBankAccountName(Eval("BankAccountId"), Eval("PayBankName"))%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="PayTreasureAccount" HeaderText="支付宝\银行帐号" />
                <asp:BoundField DataField="TypeOfProblemName" HeaderText="问题类型" />
                <asp:TemplateField HeaderText="存入（元）">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Increase"))%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="支出（元）">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Subtract"))%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="当前余额（元）">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("CurrentBalance"))%>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="状态">
                    <ItemTemplate>
                        <%# GetState(Eval("State")) %>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="Remark" HeaderText="备注" />
            </Columns>
        </asp:GridView>
    </div>

    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript">

            function AffirmClick(applyId, salePlatformId) {
                window.radopen("./Windows/MemberBalanceManageForm.aspx?Type=1&ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW1");
            }

            function refreshGrid(arg) {
                if (!arg) {
                    window.$find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    window.$find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
        </script>
    </rad:RadScriptBlock>
    <rad:RadWindowManager ID="TransferWindowManager" runat="server" ReloadOnShow="true"
        onajaxrequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="RW1" Width="500" Height="200" runat="server" Title="会员余额管理审核" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridMemberBalanceManage" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RadGridMemberBalanceManage">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridMemberBalanceManage" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridMemberBalanceManage" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RCB_Member" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SalePlatform">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Member" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RcbProblemId" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

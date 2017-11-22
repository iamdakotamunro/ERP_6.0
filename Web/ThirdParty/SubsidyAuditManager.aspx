<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="SubsidyAuditManager.aspx.cs" Inherits="ERP.UI.Web.ThirdParty.SubsidyAuditManager" %>

<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum.ThirdParty" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">销售公司:
            </td>
            <td>
                <rad:RadComboBox ID="RcbSaleFiliale" runat="server" UseEmbeddedScripts="false" Width="120px"
                    Height="100px" OnSelectedIndexChanged="RcbSaleFilialeOnSelectedIndexChanged"
                    AutoPostBack="True" EmptyMessage="请选择销售公司">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">销售平台:
            </td>
            <td>
                <rad:RadComboBox ID="RcbSalePlatform" runat="server" UseEmbeddedScripts="false" Width="120px" Height="100px" EmptyMessage="请选择销售平台">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">会员名称：
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="会员名称" ID="RtbMember" Width="120px"
                    Enabled="False" ToolTip="会员名搜索需选择具体的销售平台" />
            </td>

            <td style="text-align: right;">时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RdpStartTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
                <rad:RadDatePicker ID="RdpEndTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
            </td>
            <td style="text-align: right;">状态:
            </td>
            <td>
                <rad:RadComboBox ID="RcbState" runat="server" UseEmbeddedScripts="false" Width="120px" Height="100px">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">补贴类型:
            </td>
            <td>
                <rad:RadComboBox ID="RcbType" runat="server" UseEmbeddedScripts="false" Width="120px" Height="100px" >
                </rad:RadComboBox>
            </td>
            <td style="text-align: center;">
                <asp:Button ID="BtnSearch" runat="server" Text="查询" Style="margin-left: 40px;" OnClick="BtnSearchClick" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RgSubsidy" runat="server" SkinID="CustomPaging" OnNeedDataSource="RgSubsidyNeedDataSource">
        <%--<ClientSettings>
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>--%>
        <MasterTableView DataKeyNames="Id,MemberId" ClientDataKeyNames="Id,MemberId" NoMasterRecordsText="无可用记录。"
            CommandItemDisplay="None">
            <Columns>
                <rad:GridTemplateColumn HeaderText="时间">
                    <ItemTemplate>
                        &nbsp;<%# DateTime.Parse(Eval("ApplyDate").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("ApplyDate").ToString()).ToString("yyyy-MM-dd HH:mm:ss") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="MemberNo" HeaderText="会员名">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ThirdPartyOrderNo" HeaderText="订单号">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="金额(元)">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("Amount"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="70px" />
                    <ItemStyle HorizontalAlign="Center" Width="70px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="状态">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((SubsidyApplyState)Eval("ApplyState"))  %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="RealName" HeaderText="提交人">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="BankName" HeaderText="银行名称">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="补贴类型">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((SubsidyType)Eval("SubsidyType"))  %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注" UniqueName="Description">
                    <ItemTemplate>
                        <%# Eval("Description").ToString() %>
                        <asp:ImageButton ID="ImageButton1" SkinID="EditImageButton" OnClientClick='<%# "return AddMemoClick(\"" + Eval("Id")+ "\")" %>'
                            runat="server" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="200px" />
                    <ItemStyle HorizontalAlign="Center" Width="200px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="销售公司">
                    <ItemTemplate>
                        <%#GetSaleFilialeName(Eval("SaleFilialeId"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="90px" />
                    <ItemStyle HorizontalAlign="Center" Width="90px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton ID="LB_Process" runat="server" Text="审核" SkinID="AffirmImageButton"
                            OnClientClick='<%# "return AuditClick(\"" + Eval("Id")+ "\")" %>'
                            Visible='<%# Convert.ToInt32(Eval("State"))==(int)SubsidyApplyState.WaitConfirm %>'></asp:ImageButton>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <asp:HiddenField ID="Hid_SelectedValue" runat="server" />
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BtnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgSubsidy" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbSaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RcbSalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RtbMember" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager runat="server">
        <Windows>
            <rad:RadWindow ID="RwAudit" runat="server" Title="审核操作" Width="700" Height="540" />
            <rad:RadWindow ID="RwShow" runat="server" Title="补贴详情" Width="700" Height="540" />
            <rad:RadWindow ID="RwDescription" runat="server" Title="添加备注" Width="420" Height="110" />
        </Windows>
    </rad:RadWindowManager>

    <script src="JavaScript/jquery.js"></script>
    <script type="text/javascript">
        function RowDblClick(obj, args) {
            var id = args.getDataKeyValue("Id");
            window.radopen("./ThirdParty/SubsidyAuditForm.aspx?Type=0&Id=" + id, "RwShow");
            return false;
        }

        function AuditClick(id) {
            window.radopen("./ThirdParty/SubsidyAuditForm.aspx?Type=1&Id=" + id, "RwAudit");
        }

        function AddMemoClick(id) {
            window.radopen("./ThirdParty/SubsidyAddMemo.aspx?Id=" + id, "RwDescription");
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='BtnSearch']").click();
        }
    </script>
</asp:Content>



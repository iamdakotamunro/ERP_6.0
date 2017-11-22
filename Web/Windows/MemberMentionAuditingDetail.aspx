<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberMentionAuditingDetail.aspx.cs"
    Inherits="ERP.UI.Web.Windows.MemberMentionAuditingDetail" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    </rad:RadScriptBlock>
    <div runat="server" id="DIV_AuditingPanel">
        <table width="100%">
            <tr>
                <td class="ShortFromRowTitle">
                    拒绝理由：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TB_Memo" runat="server" TextMode="MultiLine" Width="80%" Height="50px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TB_Memo"
                        ErrorMessage="*" ValidationGroup="NoPass"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="2">
                    <asp:Button ID="BT_Save" runat="server" Text="通过" OnClick="BtSaveClick" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="BT_NoPass" runat="server" Text="不通过" OnClick="BtNoPassClick" ValidationGroup="NoPass" />
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:Button ID="BT_Cancl" runat="server" Text="取消" OnClientClick="return CancelWindow()" />
                </td>
            </tr>
        </table>
    </div>
    <table width="100%">
        <tr>
            <td align="right">
                资金流向：
                <asp:DropDownList ID="DDL_CostDirection" runat="server">
                    <asp:ListItem Text="全部" Value="0"></asp:ListItem>
                    <asp:ListItem Text="增加" Value="1"></asp:ListItem>
                    <asp:ListItem Text="减少" Value="-1"></asp:ListItem>
                </asp:DropDownList>
                &nbsp;&nbsp; 时间：
                <rad:RadDatePicker ID="rdpStartDate" runat="server" Width="100px">
                </rad:RadDatePicker>
                -
                <rad:RadDatePicker ID="rdpEndDate" runat="server" Width="100px">
                </rad:RadDatePicker>
                &nbsp;&nbsp;
                <asp:LinkButton ID="lbSearch" runat="server" OnClick="LbSearchClick">
                    <asp:Image ID="SearchImage" runat="server" SkinID="SearchImageButton" ImageAlign="AbsMiddle" />
                    搜索
                </asp:LinkButton>
            </td>
        </tr>
        <tr>
            <td width='100%'>
                <rad:RadGrid ID="rgReckoning" runat="server" SkinID="CustomPaging" OnNeedDataSource="RadGridReckoning_NeedDataSource">
                    <MasterTableView DataKeyNames="" ClientDataKeyNames="" CommandItemDisplay="None">
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="TradeCode" HeaderText="单据编号" UniqueName="TradeCode">
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="CreateTime" HeaderText="时间" UniqueName="CreateTime">
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="IncreaseAmount" HeaderText="存入(元)" UniqueName="IncreaseAmount">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("IncreaseAmount"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="SubtractAmount" HeaderText="支出(元)" UniqueName="SubtractAmount">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("SubtractAmount"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="CurrentBalance" HeaderText="当前余额(元)" UniqueName="CurrentBalance">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("CurrentBalance"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="Memo" HeaderText="说明" UniqueName="Memo">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="100px" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="ManageDescription" HeaderText="备注" UniqueName="ManageDescription">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgReckoning" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="lbSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgReckoning" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BT_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BT_NoPass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

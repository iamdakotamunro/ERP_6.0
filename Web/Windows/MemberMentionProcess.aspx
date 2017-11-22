<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberMentionProcess.aspx.cs"
    Inherits="ERP.UI.Web.Windows.MemberMentionProcess" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

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
    <table>
        <tr>
            <td class="ShortFromRowTitle">
                会员名：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:Label ID="LB_Member" runat="server"></asp:Label>
            </td>
            <td class="ShortFromRowTitle">
                账户余额(元)：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:Label ID="LB_Balance" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="ShortFromRowTitle">
                银行名：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:Label ID="LB_BankName" runat="server"></asp:Label>
            </td>
            <td class="ShortFromRowTitle">
                账户名：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:Label ID="LB_AccountName" runat="server"></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="ShortFromRowTitle">
                帐号：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:Label ID="LB_BankNo" runat="server"></asp:Label>
            </td>
            <td class="ShortFromRowTitle">
                资金账户：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_BankAccountsId" runat="server" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入资金账户" OnItemsRequested="RCB_BankAccountsId_ItemsRequested" Height="80px" CssClass="Check">
                </rad:RadComboBox>
                <asp:RequiredFieldValidator ID="RFVBankAccounts" runat="server" ErrorMessage="账户名称不能为空！"
                    Text="*" ControlToValidate="RCB_BankAccountsId" ValidationGroup="save"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td class="ShortFromRowTitle">
                提现金额(元)：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="LB_Amount" runat="server"></asp:TextBox>
            </td>
            <td class="ShortFromRowTitle">
                手续费(元)：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_Poundage" runat="server"></asp:TextBox>
                <asp:RegularExpressionValidator ID="REVCost" runat="server" ControlToValidate="TB_Poundage"
                    Text="*" ErrorMessage="金额必须为数字！" ValidationExpression="\d*(\.\d*){0,1}"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="ShortFromRowTitle">
                站点名称:
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:Label ID="Lbl_Website" runat="server" Text=""></asp:Label>
            </td>
        </tr>
        <tr>
            <td class="ShortFromRowTitle">
                交易号：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_TransCode" runat="server" Width="250"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td class="ShortFromRowTitle">
                拒绝理由：
            </td>
            <td class="AreaEditFromRowInfo" colspan="3">
                <asp:TextBox ID="TB_Memo" runat="server" TextMode="MultiLine" Width="500px" Height="30px"></asp:TextBox>
                <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TB_Memo"
                    ErrorMessage="*"></asp:RequiredFieldValidator>
            </td>
        </tr>
        <tr>
            <td align="center" colspan="4">
                <asp:Button ID="BT_Save" runat="server" Text="打款完成" OnClick="BtSaveClick" ValidationGroup="save" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="BT_Back" runat="server" Text="退回申请" OnClick="BtBackClick" />
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:Button ID="BT_Cancl" runat="server" Text="关闭窗口" OnClientClick="return CancelWindow()" />
            </td>
        </tr>
    </table>
    </div>
    <table>
        <tr>
            <td>
                <rad:RadGrid ID="RG_Order" runat="server" SkinID="Common" OnNeedDataSource="RgOrderNeedDataSource">
                    <MasterTableView DataKeyNames="OrderId,MemberId,OrderNo" ClientDataKeyNames="OrderId,MemberId"
                        CommandItemDisplay="None">
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridClientSelectColumn UniqueName="column">
                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridClientSelectColumn>
                            <rad:GridBoundColumn DataField="OrderNo" HeaderText="订单号" UniqueName="OrderNo">
                                <HeaderStyle Width="160px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="OrderTime" HeaderText="下单时间" UniqueName="OrderTime">
                                <HeaderStyle Width="160px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Consignee" HeaderText="收货人" UniqueName="Consignee">
                                <HeaderStyle HorizontalAlign="Center" Width="170px" />
                                <ItemStyle HorizontalAlign="Center" Width="170px" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="RealTotalPrice" HeaderText="支付金额" UniqueName="RealTotalPrice">
                                <ItemTemplate>
                                    <asp:Label ID="RealTotalPrice" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Convert.ToDecimal(Eval("RealTotalPrice"))) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="订单状态" UniqueName="OrderState">
                                <ItemTemplate>
                                    <asp:Label ID="OrderState" runat="server" Text='<%# EnumAttribute.GetKeyName((OrderState)Eval("OrderState")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" UseEmbeddedScripts="false">
        <AjaxSettings>
              <rad:AjaxSetting AjaxControlID="BT_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BT_Back">
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

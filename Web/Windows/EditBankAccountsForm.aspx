<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.EditBankAccountsForm"
    CodeBehind="EditBankAccountsForm.aspx.cs" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
        <div class="StagePanel">
            <rad:RadScriptManager ID="RSM" runat="server">
            </rad:RadScriptManager>
            <rad:RadScriptBlock ID="RSB" runat="server">
                <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            </rad:RadScriptBlock>
            <asp:Panel ID="EditBankAccounts" runat="server">
                <table border="0" cellpadding="0" cellspacing="0" width="100%">
                    <tr>
                        <td class="ControlTools">
                            <asp:LinkButton ID="LB_Inster" runat="server" OnClick="Button_InsterGoods">
                                <asp:Image ID="IB_Inster" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                                    BorderStyle="None" />添加
                            </asp:LinkButton>
                            <asp:Label ID="Lab_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                            <asp:LinkButton ID="LB_Update" OnClick="Button_UpdateGoods" runat="server">
                                <asp:Image ID="IB_Update" runat="server" SkinID="InsertImageButton" ImageAlign="AbsMiddle"
                                    BorderStyle="None" />更新
                            </asp:LinkButton>
                            <asp:Label ID="Lab_UpdateSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                            <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()"
                                SkinType="Cancel" Text="取消"></Ibt:ImageButtonControl>
                        </td>
                    </tr>
                </table>
                <table class="PanelArea">
                    <tr>
                        <td class="AreaEditFromTitle">机构名称：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TB_BankName" runat="server" SkinID="LongInput"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVBankName" runat="server" ErrorMessage="机构名称不能为空！"
                                Text="*" ControlToValidate="TB_BankName"></asp:RequiredFieldValidator>
                        </td>
                        <td>银行机构名称，用作区分标识。
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">网络接口：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:DropDownList ID="DDL_PaymentInterfaceId" runat="server" SkinID="LongDropDown"
                                DataValueField="PaymentInterfaceId" DataTextField="PaymentInterfaceName">
                            </asp:DropDownList>
                        </td>
                        <td>如该资金账户支持网上支付请填写此项。
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">账户名称：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TB_AccountsName" runat="server" SkinID="LongInput"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="账户名称不能为空！"
                                Text="*" ControlToValidate="TB_AccountsName"></asp:RequiredFieldValidator>
                        </td>
                        <td>资金账户名称，与系统有关的账户均应填写。
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">账 户 号：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TB_Accounts" runat="server" SkinID="LongInput"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVAccounts" runat="server" ErrorMessage="账户号不能为空！"
                                Text="*" ControlToValidate="TB_Accounts"></asp:RequiredFieldValidator>
                        </td>
                        <td>
                            <span style="color: #FF0000;">【用于网络支付，谨慎更改】</span>账户号码，网上银行为帐号或客户号。
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">私钥：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:TextBox ID="TB_AccountsKey" runat="server" SkinID="LongInput"></asp:TextBox>
                        </td>
                        <td>私钥是提供给带有网络接口的账户使用的。私钥也称为“验证码”。
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">支付类型：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:DropDownList ID="DDL_PaymentType" runat="server" SkinID="LongDropDown">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFVPaymentType" runat="server" ErrorMessage="支付类新不允许为空！"
                                Text="*" ControlToValidate="DDL_PaymentType" InitialValue="-1"></asp:RequiredFieldValidator>
                        </td>
                        <td>选择支付该账户的字符类型方式。
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">账户类型：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:DropDownList runat="server" ID="ddl_AccountType" />
                        </td>
                        </tr>
                <tr>
                    <td class="AreaEditFromTitle">图标：
                    </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:HiddenField ID="HF_BankIcon" runat="server" />
                            <rad:RadUpload ID="RU_BankIcon" runat="server" ControlObjectsVisibility="None" Width="250px"
                                AllowedFileExtensions=".gif,.jpg,.png" ReadOnlyFileInputs="True" />
                        </td>
                        <td>
                            <asp:Image ID="BankImg" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">启用/停用：
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:CheckBox ID="CB_IsUse" runat="server" />
                        </td>
                        <td>资金账户状态,可以停用/启用。
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">是/否需要完成
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:CheckBox ID="CB_IsFinish" runat="server" Checked="true" />
                        </td>
                        <td>资金帐户是否需要完成，是/否
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">是/否原路返回
                        </td>
                        <td class="AreaEditFromRowInfo">
                            <asp:CheckBox ID="CB_IsBacktrack" runat="server" Checked="true" />
                        </td>
                        <td>用于废单时订单金额的退回方式选择
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaEditFromTitle">前台是否显示
                        </td>
                        <td class="AreaEditFromRowInfo" colspan="2">
                            <asp:CheckBox ID="CbIsDisplay" runat="server" Checked="False" />
                        </td>
                    </tr>
                </table>
                <rad:RadEditor ID="RE_Description" runat="server" SkinID="Bank" EnableHtmlIndentation="True">
                </rad:RadEditor>
            </asp:Panel>
            <rad:RadAjaxManager ID="RAM" runat="server">
                <AjaxSettings>
                    <rad:AjaxSetting AjaxControlID="LB_Inster">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="EditBankAccounts" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        </UpdatedControls>
                    </rad:AjaxSetting>
                    <rad:AjaxSetting AjaxControlID="LB_Update">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="EditBankAccounts" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        </UpdatedControls>
                    </rad:AjaxSetting>
                </AjaxSettings>
            </rad:RadAjaxManager>
            <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
            </rad:RadAjaxLoadingPanel>
        </div>
    </form>
</body>
</html>

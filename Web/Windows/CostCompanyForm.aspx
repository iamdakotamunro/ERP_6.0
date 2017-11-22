<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostCompanyForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.CostCompanyForm" %>

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
    <table class="PanelArea">
        <tr>
            <td class="AreaRowTitle">
                单位名称：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_CompanyName" runat="server" EmptyMessage="费用名称" SkinID="LongInput"
                    Width="180px" />
                <asp:RequiredFieldValidator ID="RFVCompanyName" runat="server" ErrorMessage="添加单位名称"
                    Text="*" ControlToValidate="RTB_CompanyName"></asp:RequiredFieldValidator>
            </td>
            <td class="AreaRowTitle">
                所属分类：
            </td>
            <td>
                <rad:RadComboBox ID="RCB_CompanyClassId" runat="server" ErrorMessage="所属费用分类" Width="183px" />
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                联系人：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_Linkman" runat="server" EmptyMessage="联系人" SkinID="LongInput"
                    Width="180px" />
            </td>
            <td class="AreaRowTitle">
                手机号码：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_Mobile" runat="server" EmptyMessage="手机号码" SkinID="LongInput"
                    Width="180px" />
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                联系地址：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_Address" runat="server" EmptyMessage="联系地址" SkinID="LongInput"
                    Width="180px" />
            </td>
            <td class="AreaRowTitle">
                邮政编码：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_PostalCode" runat="server" EmptyMessage="邮政编码" SkinID="LongInput"
                    Width="180px" />
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                联系电话：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_Phone" runat="server" EmptyMessage="联系电话" SkinID="LongInput"
                    Width="180px" />
            </td>
            <td class="AreaRowTitle">
                传真号码：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_Fax" runat="server" EmptyMessage="传真号码" SkinID="LongInput"
                    Width="180px" />
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                企业站点：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_WebSite" runat="server" EmptyMessage="企业站点" SkinID="LongInput"
                    Width="180px" />
                <asp:RegularExpressionValidator ID="REVWebSite" runat="server" ControlToValidate="RTB_WebSite"
                    ErrorMessage="网站地址不正确！" Text="*" ValidationExpression="([\w-]+\.)+[\w-]+(/[\w- ./?%&=]*)?"></asp:RegularExpressionValidator>
            </td>
            <td class="AreaRowTitle">
                电子邮箱：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_Email" runat="server" EmptyMessage="电子邮箱" SkinID="LongInput"
                    Width="180px" />
                <asp:RegularExpressionValidator ID="REVEmail" runat="server" ControlToValidate="RTB_Email"
                    ErrorMessage="邮件地址不正确！" Text="*" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                开户银行：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_BankAccounts" runat="server" EmptyMessage="开户银行" SkinID="LongInput"
                    Width="180px" />
            </td>
            <td class="AreaRowTitle">
                银行帐号：
            </td>
            <td>
                <rad:RadTextBox ID="RTB_AccountNumber" runat="server" EmptyMessage="银行帐号" SkinID="LongInput"
                    Width="180px" />
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                公司类型：
            </td>
            <td>
                <asp:DropDownList ID="DDL_CompanyType" runat="server">
                    <asp:ListItem Text="" Value=""></asp:ListItem>
                    <asp:ListItem Text="供应商" Value="1"></asp:ListItem>
                    <asp:ListItem Text="销售商" Value="2"></asp:ListItem>
                    <asp:ListItem Text="物流公司" Value="3"></asp:ListItem>
                    <asp:ListItem Text="会员总帐" Value="4"></asp:ListItem>
                    <asp:ListItem Text="其它" Value="0"></asp:ListItem>
                </asp:DropDownList>
                <asp:RequiredFieldValidator ID="RFVCompanyType" runat="server" ControlToValidate="DDL_CompanyType"
                    Text="*" ErrorMessage="公司类型必须选择！"></asp:RequiredFieldValidator>
            </td>
            <td class="AreaRowTitle">
                当前状态：
            </td>
            <td>
                <asp:RadioButtonList ID="RBL_State" runat="server" RepeatDirection="Horizontal" >
                    <asp:ListItem Text="使用" Value="1" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="搁置" Value="0" ></asp:ListItem>
                </asp:RadioButtonList>
            </td>
        </tr>
 <%--       <tr>
            <td colspan="4">
                <asp:Repeater ID="RepFilialeList" runat="server" OnItemDataBound="RepFilialeList_ItemDataBound">
                    <ItemTemplate>
                        <fieldset > 
                            <legend ><span style="font-weight: bold">
                                <%#Eval("Name") %>：</span></legend>
                            <asp:HiddenField ID="hfFilialeId" Value='<%#Eval("ID") %>' runat="server"></asp:HiddenField>
                            <table width="100%">
                                <tr>
                                    <td class="AreaRowTitle">
                                        发票打款账号：
                                    </td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_InvoiceAccounts" runat="server" ErrorMessage="发票打款账号" />(公司主账号)
                                    </td>
                                    <td class="AreaRowTitle">
                                        凭证打款账号：
                                    </td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_VoucherAccounts" runat="server" ErrorMessage="凭证打款账号" />
                                    </td>
                                </tr>
                                <tr>
                                    <td class="AreaRowTitle">
                                        现金打款账号：
                                    </td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_CashAccounts" runat="server" ErrorMessage="现金打款账号" />
                                    </td>
                                    <td class="AreaRowTitle">
                                        无凭证打款账号：
                                    </td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_NoVoucherAccounts" runat="server" ErrorMessage="无凭证打款账号" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </ItemTemplate>
                </asp:Repeater>
            </td>
        </tr>--%>
        <tr>
            <td class="AreaRowTitle">
                备注说明：
            </td>
            <td colspan="3">
                <rad:RadTextBox ID="RTB_Description" runat="server" EmptyMessage="备注说明" SkinID="AutoTextarea"
                    TextMode="MultiLine" Width="600px" Height="33px" />
            </td>
        </tr>
        <tr>
            <td class="AreaRowTitle">
                详细说明：
            </td>
            <td colspan="3">
                <rad:RadTextBox ID="RTB_SubjectInfo" runat="server" EmptyMessage="详细说明" SkinID="AutoTextarea"
                    TextMode="MultiLine" Width="600px" Height="33px" />
            </td>
        </tr>
        <tr>
            <td align="center" colspan="4">
                <asp:ImageButton ID="BtnSave" runat="server" SkinID="InsertImageButton" AlternateText="保存"
                    OnClick="BtnSave_Click" />&nbsp;
                <asp:ImageButton ID="btnCancel" runat="server" SkinID="CancelImageButton" AlternateText="取消"
                    OnClientClick="return CancelWindow()" CausesValidation="false" />
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BT_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BatchAddInvoice.aspx.cs"
    Inherits="ERP.UI.Web.Windows.BatchAddInvoice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script type="text/javascript" src="../JavaScript/common.js"></script>
        <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    </rad:RadScriptBlock>
    <asp:Panel ID="Panel_AddInvoice" runat="server">
        <table>
            <tr>
                <td>
                    发票抬头：
                </td>
                <td>
                    <asp:TextBox ID="TB_Name" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="TB_Name"
                        ErrorMessage="发票抬头不允许为空！" Text="*"></asp:RequiredFieldValidator>
                </td>
                <td>
                    品名SKU：
                </td>
                <td>
                    <asp:TextBox ID="TB_Content" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="TB_Content"
                        ErrorMessage="品名SKU地址不允许为空！" Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    发票寄送地址：
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TB_Address" runat="server" Width="375px" Text="无"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="TB_Address"
                        ErrorMessage="发票寄送地址不允许为空！" Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    发票代码：
                </td>
                <td>
                    <asp:TextBox ID="TB_Code" runat="server"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TB_Code"
                        Text="*" ErrorMessage="发票代码必须为数字！" ValidationExpression="\d*"></asp:RegularExpressionValidator>
                </td>
                <td>
                    发票号码段：
                </td>
                <td>
                    <asp:TextBox ID="TB_StartNo" runat="server"></asp:TextBox>-
                    <asp:TextBox ID="TB_EndNo" runat="server"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TB_StartNo"
                        Text="*" ErrorMessage="发票号码必须为数字！" ValidationExpression="\d*"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="TB_StartNo"
                        ErrorMessage="发票号码不允许为空！" Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    发票类型：
                </td>
                <td>
                    <asp:DropDownList runat="server" ID="DDR_Type">
                        <asp:ListItem Value="0" Text="正票"></asp:ListItem>
                        <asp:ListItem Value="1" Text="废票" Selected="True"></asp:ListItem>
                    </asp:DropDownList>
                </td>
                <td>
                    发票金额段：
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TB_StartSum" runat="server"></asp:TextBox>-
                    <asp:TextBox ID="TB_EndSum" runat="server"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="REVCost" runat="server" ControlToValidate="TB_StartSum"
                        Text="*" ErrorMessage="发票总额必须为数字！" ValidationExpression="\d*(\.\d*){0,1}"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator6" runat="server" ControlToValidate="TB_StartSum"
                        ErrorMessage="发票总额不允许为空！" Text="*"></asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td>
                    申请人：
                </td>
                <td>
                    <asp:TextBox ID="TB_Receiver" runat="server"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator7" runat="server" ControlToValidate="TB_Receiver"
                        ErrorMessage="发票代码不允许为空！" Text="*"></asp:RequiredFieldValidator>
                </td>
                <td>
                    开票日期段：
                </td>
                <td colspan="3">
                    <rad:RadDateTimePicker ID="RDP_AddInvoiceStartTime" runat="server" SkinID="Common">
                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" CellVAlign="Top"
                            ViewSelectorText="x">
                        </Calendar>
                        <TimeView CellSpacing="-1" Culture="Chinese (People's Republic of China)">
                        </TimeView>
                        <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>
                        <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                        <DateInput DisplayDateFormat="yyyy/M/d" DateFormat="yyyy/M/d">
                        </DateInput>
                    </rad:RadDateTimePicker>-
                    <rad:RadDateTimePicker ID="RDP_AddInvoiceEndTime" runat="server" SkinID="Common">
                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" CellVAlign="Top"
                            ViewSelectorText="x">
                        </Calendar>
                        <TimeView CellSpacing="-1" Culture="Chinese (People's Republic of China)">
                        </TimeView>
                        <TimePopupButton ImageUrl="" HoverImageUrl=""></TimePopupButton>
                        <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                        <DateInput DisplayDateFormat="yyyy/M/d" DateFormat="yyyy/M/d">
                        </DateInput>
                    </rad:RadDateTimePicker>
                </td>
            </tr>
            <tr>
                <td>
                    发票公司：
                </td>
                <td colspan="5">
                    <rad:RadComboBox ID="RcbSaleFiliale" Width="160" runat="server" AutoPostBack="false"
                        CausesValidation="false" OnLoad="RcbFromSourceLoad">
                        <Items>
                            <rad:RadComboBoxItem Text=" " Value="00000000-0000-0000-0000-000000000000" />
                        </Items>
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td colspan="6">
                    &nbsp;
                </td>
            </tr>
            <tr>
                <td colspan="2" align="center">
                    <asp:Button ID="BT_Save" runat="server" OnClick="BtSaveClick" Text="批量保存" />
                </td>
                <td colspan="2" align="center">
                    <asp:Button ID="BT_Cancel" runat="server" Text="取消" OnClientClick="return CancelWindow()" />
                </td>
            </tr>
        </table>
    </asp:Panel>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BT_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_AddInvoice" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

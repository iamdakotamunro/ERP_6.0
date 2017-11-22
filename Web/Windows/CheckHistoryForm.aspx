<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CheckHistoryForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.CheckHistoryForm" %>

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
            <td align="center">
                <span style="font-weight: bold">注：此收款账号必须填写！</span>
            </td>
        </tr>
        <tr>
            <td>
                <br />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Repeater ID="RepReceiptNoList" runat="server" OnItemDataBound="RepReceiptNo_ItemDataBound">
                    <ItemTemplate>
                        <fieldset>
                            <legend><span style="font-weight: bold">对账单位：<%#Eval("CompanyName")%></span> <span
                                style="font-weight: bold">单据号：<%#Eval("ReceiptNo")%></span> </legend>
                            <asp:HiddenField ID="hfReceiptID" Value='<%#Eval("ReceiptID") %>' runat="server">
                            </asp:HiddenField>
                            <asp:HiddenField ID="hfFilialeId" Value='<%#Eval("FilialeId") %>' runat="server">
                            </asp:HiddenField>
                            <table width="100%">
                                <tr>
                                    <td class="AreaRowTitle">
                                        收款公司:
                                    </td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_FilialeList" runat="server" ErrorMessage="收款公司" />
                                    </td>
                                    <td class="AreaRowTitle">
                                        收款账号：
                                    </td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_BankAccounts" runat="server" ErrorMessage="收款账号" Height="150px" />
                                    </td>
                                </tr>
                            </table>
                        </fieldset>
                    </ItemTemplate>
                </asp:Repeater>
            </td>
        </tr>
        <tr>
            <td align="center">
                <asp:ImageButton ID="BtnSave" runat="server" SkinID="InsertImageButton" AlternateText="确定"
                    OnClick="BtnSave_Click" />
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BtnSave">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="BtnSave" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

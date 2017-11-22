<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DebitNoteAddForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.DebitNoteAddForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    <script type="text/javascript" src="../My97DatePicker/WdatePicker.js"></script>
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
                <td class="AreaRowTitle">单据标题：
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TB_Title" runat="server" TextMode="MultiLine" SkinID="AutoTextarea"
                        Width="380px" Height="30px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">供应商：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Width="150px"
                        DataValueField="CompanyId" DataTextField="CompanyName" Height="100px" AutoPostBack="True"
                        Filter="StartsWith" EmptyMessage="供应商">
                    </rad:RadComboBox>
                </td>
                <td class="AreaRowTitle">责任人：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Persion" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        AllowCustomText="True" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="RealName"
                        DataValueField="PersonnelId" Width="150px" Height="100px">
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">仓库：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" Width="150px" Height="100px" DataTextField="Value"
                        DataValueField="Key">
                    </rad:RadComboBox>
                </td>
                <td class="AreaRowTitle">金额：
                </td>
                <td colspan="3">
                    <rad:RadTextBox ID="RTB_PresentAmount" runat="server" EmptyMessage="无可不填写">
                    </rad:RadTextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">活动周期：
                </td>
                <td>
                    <asp:TextBox ID="txt_ActivityTimeStart" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',maxDate:$('input[id$=txt_ActivityTimeEnd]').val()})"></asp:TextBox>
                    至
                    <asp:TextBox ID="txt_ActivityTimeEnd" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',minDate:$('input[id$=txt_ActivityTimeStart]').val()})"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="AreaRowTitle">赠品说明：
                </td>
                <td colspan="3">
                    <asp:TextBox ID="TB_Memo" runat="server" TextMode="MultiLine" SkinID="AutoTextarea"
                        Width="380px" Height="60px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="center" colspan="4">
                    <asp:ImageButton ID="BtnSave" runat="server" SkinID="InsertImageButton" AlternateText="保存"
                        OnClick="BtnSave_OnClick" />&nbsp;
                <asp:ImageButton ID="BtnCancel" runat="server" SkinID="CancelImageButton" AlternateText="取消"
                    OnClientClick="return CancelWindow()" CausesValidation="false" />
                </td>
            </tr>
        </table>
        <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RCB_Company">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_PurchaseGroup" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

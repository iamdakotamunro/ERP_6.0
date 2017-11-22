<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyFundReceiptCancel.aspx.cs" Inherits="ERP.UI.Web.Windows.CompanyFundReceiptCancel" %>
<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server" />
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <asp:HiddenField ID="HF_ReceiptID" runat="server" />
    <div style="text-align: center;">
        作废单据号：<asp:Label ID="lbReceiptNo" runat="server" Text="Label"></asp:Label><br/>
        <b style="color: red;"><asp:Literal ID="Lit_Msg" runat="server"></asp:Literal></b>
        <table>
            <tr>
                <td>
                    <Ibt:ImageButtonControl ID="LB_Accept" runat="server" OnClick="LB_Accept_OncLick" SkinType="Affirm" Text="确定">
                    </Ibt:ImageButtonControl>
                </td>
                <td>
                    <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="CancelWindow();return false;" SkinType="Cancel" Text="取消">  
                    </Ibt:ImageButtonControl>
                </td>
            </tr>
        </table>
        <asp:Label ID="Lab_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
    </div>    
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="LB_Accept">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LB_Accept" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="LB_Cancel" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

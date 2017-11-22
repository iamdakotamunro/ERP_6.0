<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddActityFilingLog.aspx.cs"
    Inherits="ERP.UI.Web.Windows.AddActityFilingLog" %>
    <%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="/JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <div>
        <asp:HiddenField ID="HF_ActivityFilingID" runat="server" />
        <table class="PanelArea">
            <tr>
                <td>
                    <asp:Repeater ID="RP_Clew" runat="server">
                        <ItemTemplate>
                            <div id='' style="background-color: #FFFFFF; width: 600px;">
                                <div id='' style="width: 580px; text-align: left; height: 26px; padding-top: 14px;
                                    padding-left: 20px; cursor: pointer;">
                                    <asp:Literal ID="Lit_Clew" runat="server" Text='<%# Eval("Description") %>'></asp:Literal>
                                </div>
                                <div id='' style="display: none; width: 580px; text-align: left; padding-top: 24px;
                                    padding-bottom: 24px; padding-left: 20px; cursor: pointer;">
                                    <asp:Label ID="Lab_ClewDetail" runat="server" Text='<%# Eval("Description") %>'></asp:Label>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>
            <tr>
                <td>
                    <rad:RadTextBox TextMode="SingleLine" ID="RTB_RemarkInput" Width="600" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="center">
                    <Ibt:ImageButtonControl ID="LB_Accept" runat="server" OnClick="LB_Save_OncLick" SkinType="Affirm" Text="确定">
                    </Ibt:ImageButtonControl>
                    <asp:Label ID="Lab_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                    <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="CancelWindow();return false;" SkinType="Cancel" Text="取消">  
                    </Ibt:ImageButtonControl>
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" UseEmbeddedScripts="false">
        <AjaxSettings>
            <%--<rad:AjaxSetting AjaxControlID="AddRadGoods">
               <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="AddRadGoods" LoadingPanelID="loading" />
                </UpdatedControls>
           </rad:AjaxSetting>--%>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

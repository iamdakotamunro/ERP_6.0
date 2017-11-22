<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.ShowClewForm"
    CodeBehind="ShowClewForm.aspx.cs" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <script src="../JavaScript/jquery.js" type="text/javascript"></script>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript">
            
        </script>
        <script language="JavaScript" type="text/javascript">
            function ToggleDisplay(id) {
                var elem = document.getElementById('pd0u' + id);
                var elem2 = document.getElementById('pd1u' + id);
                if (elem) {
                    if (elem2) {
                        $("div[id^='pd1u']").each(function (i) {
                            $(this).css("display", "none");
                            $(this).css("visibility", "hidden");
                        });
                        $("div[id^='pd0u']").each(function (i) {
                            $(this).css("display", "block");
                            $(this).css("visibility", "visible");
                        });
                        elem2.style.display = 'block';
                        elem2.style.visibility = 'visible';
                        elem.style.display = 'none';
                        elem.style.visibility = 'hidden';
                    }
                }
            } 
        </script>
    </rad:RadScriptBlock>
    <table class="PanelArea">
        <tr>
            <td align="center">
                <asp:Repeater ID="RP_Clew" runat="server" OnItemDataBound="RP_Clew_ItemDataBound">
                    <ItemTemplate>
                        <div id='u<%#Container.ItemIndex %>' style="background-color: #FFFFFF;width:600px;">
                            <div id='pd0u<%#Container.ItemIndex %>' onclick='ToggleDisplay(<%# DataBinder.Eval(Container, "ItemIndex") %>);' style="width:580px;text-align:left;height: 26px;padding-top:14px;padding-left: 20px;cursor: pointer;">
                                <asp:Literal ID="Lit_Clew" runat="server" Text='<%# Eval("Description").ToString().Substring(0,Eval("Description").ToString().Length>50?50:Eval("Description").ToString().Length) %>'></asp:Literal>
                            </div>
                            <div id='pd1u<%#Container.ItemIndex %>' style="display: none;width:580px;text-align: left;padding-top:24px;padding-bottom: 24px;padding-left: 20px;cursor: pointer;">
                                <asp:Label ID="Lab_ClewDetail" runat="server" Text='<%# Eval("Description")%>'></asp:Label>
                            </div>
                        </div>
                    </ItemTemplate>
                    <AlternatingItemTemplate>
                        <div id='u<%#Container.ItemIndex %>' style="background-color: #DAE2E8;width:600px;">
                            <div id='pd0u<%#Container.ItemIndex %>' onclick='ToggleDisplay(<%# DataBinder.Eval(Container, "ItemIndex") %>);' style="width:580px;text-align:left;height:26px;padding-top:14px;padding-left:20px;cursor: pointer;">
                                <asp:Literal ID="Lit_Clew" runat="server" Text='<%# Eval("Description").ToString().Substring(0,Eval("Description").ToString().Length>50?50:Eval("Description").ToString().Length) %>'></asp:Literal>
                            </div>
                            <div id='pd1u<%#Container.ItemIndex %>' style="display: none;text-align: left;width:580px;padding-top:24px;padding-bottom: 24px;padding-left: 20px;cursor: pointer;">
                                <asp:Label ID="Lab_ClewDetail" runat="server" Text='<%# Eval("Description")%>'></asp:Label>
                            </div>
                        </div>
                    </AlternatingItemTemplate>
                </asp:Repeater>
            </td>
        </tr>
        <tr>
            <td style="height: 20px;">
            </td>
        </tr>
        <tr>
            <td align="center">
                <rad:RadComboBox ID="RCB_Clew" runat="server" Width="600px" Height="120px" AllowCustomText="true" 
                    AccessKey="T" MarkFirstMatch="True" ShowToggleImage="True" OnSelectedIndexChanged="RCB_Clew_SelectedIndexChanged"
                      AutoPostBack="true" EnableTextSelection="false">
                </rad:RadComboBox>
                <asp:requiredfieldvalidator id="RFVClew" runat="server" errormessage="处理意见不能为空！"
                    text="*" controltovalidate="RCB_Clew">
                </asp:requiredfieldvalidator>
            </td>
        </tr>
        <tr>
            <td align="center" style="height: 30px;" colspan="3">
                <Ibt:ImageButtonControl ID="LB_Clew" runat="server" OnClick="Button_UpdateClew" SkinType="Affirm">
                </Ibt:ImageButtonControl>
                &nbsp;&nbsp;
                <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()"
                    SkinType="Cancel"></Ibt:ImageButtonControl>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RCB_Clew">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Clew"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

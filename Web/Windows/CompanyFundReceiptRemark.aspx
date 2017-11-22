<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyFundReceiptRemark.aspx.cs" Inherits="ERP.UI.Web.Windows.CompanyFundReceiptRemark" %>
<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    <script type="text/javascript" src="../JavaScript/common.js"></script>
    <script type="text/javascript" src="../JavaScript/telerik.js"></script>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server" >
        
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
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
    <asp:HiddenField ID="HF_ReceiptID" runat="server" />
    <div>
        <table class="PanelArea">
            <tr>
                <td>
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
    </form>
</body>
</html>

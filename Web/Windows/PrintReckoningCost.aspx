<%@ Page Language="C#" AutoEventWireup="true" Inherits="ERP.UI.Web.Windows.PrintReckoningCost"
    CodeBehind="PrintReckoningCost.aspx.cs" %>
<%@Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>打印单位往来帐单据</title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script language="javascript" type="text/javascript">
            function Print() {
                window.print();
                return false;
            }
        </script>

        <script type="text/javascript" src="../JavaScript/telerik.js"></script>

    </rad:RadScriptBlock>
    <asp:Panel ID="yesAuditing" runat="server">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="ControlTools">
                    <Ibt:ImageButtonControl ID="lbUpdateAuditing" runat="server" OnClick="Button_UpdateTransfer"
                        ValidationGroup="ReduceIncomeGroup" SkinType="Insert" Text="保存">
                    </Ibt:ImageButtonControl>
                    <Ibt:ImageButtonControl ID="LBUpdate" runat="server" OnClientClick="return Print()" SkinType="Print" Text="打印">
                    </Ibt:ImageButtonControl>
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td class="WinTitle" colspan="2">
                    <asp:Label ID="Lab_WinTitle" runat="server" CssClass="WinTitle"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    单据编号：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Literal ID="Lit_TradeCode" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    开票单位：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Literal ID="Lit_Filiale" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    往来单位：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Literal ID="Lit_Company" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    <asp:Literal ID="Lit_ReckoningType" runat="server"></asp:Literal>金额：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Literal ID="Lit_AccountReceivable" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    创建时间：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Literal ID="Lit_DateCreated" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    添加备注：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="txtDescription" runat="server" Width="244px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    备注：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadTextBox ID="rtbDescriptionAuditing" runat="server" Width="250" Rows="3" ReadOnly="true"
                        TextMode="MultiLine" SelectionOnFocus="CaretToEnd">
                    </rad:RadTextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <asp:Panel ID="noAuditing" runat="server">
        <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0"
            Width="100%">
            <asp:TableRow>
                <asp:TableCell CssClass="ControlTools">
                    <Ibt:ImageButtonControl ID="LB_Update" runat="server" OnClick="Button_UpdateTransfer" ValidationGroup="ReduceIncomeGroup" SkinType="Insert" Text="保存">
                    </Ibt:ImageButtonControl>
                    <asp:Label ID="Label_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                    <asp:Label ID="Label1" runat="server" Text="Label">&nbsp;</asp:Label>
                    <Ibt:ImageButtonControl ID="LinkButton_Delete" runat="server" OnClick="Button_Delete" SkinType="Delete" Text="删除">
                    </Ibt:ImageButtonControl>
                    <Ibt:ImageButtonControl ID="LinkButton_Cancel" runat="server" OnClientClick="return CancelWindow()" SkinType="Cancel" Text="取消">
                    </Ibt:ImageButtonControl>
                </asp:TableCell>
            </asp:TableRow>
        </asp:Table>
        <table class="PanelArea">
            <tr>
                <td class="WinTitle" colspan="2">
                    <asp:Label ID="Label2" runat="server" CssClass="WinTitle"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    单据编号：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Literal ID="Lit_TradeCode2" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    开票单位：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Literal ID="Lit_Filiale2" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    往来单位：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Literal ID="Lit_Company2" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    <asp:Literal ID="Lit_ReckoningType2" runat="server"></asp:Literal>金额：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="Lit_AccountReceivable2" runat="server" Width="143px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFVIncome" runat="server" ValidationGroup="ReduceIncomeGroup"
                        ControlToValidate="Lit_AccountReceivable2" ErrorMessage="金额不允许为空！" Text="*"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="REVIncome" runat="server" ValidationGroup="ReduceIncomeGroup"
                        ControlToValidate="Lit_AccountReceivable2" Text="*" ErrorMessage="请输入正数！" ValidationExpression="[0-9]{0,}\.?[0-9]{0,}$"></asp:RegularExpressionValidator>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    创建时间：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:Literal ID="Lit_DateCreated2" runat="server"></asp:Literal>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    添加备注：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="Lit_Description2" runat="server" Width="244px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td class="PromptFromRowTitle">
                    备注：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadTextBox ID="Lit_Description3" runat="server" Width="250" Rows="3" ReadOnly="true"
                        TextMode="MultiLine" SelectionOnFocus="CaretToEnd">
                    </rad:RadTextBox>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:AjaxSetting AjaxControlID="lbUpdateAuditing">
        <updatedcontrols>
                <rad:AjaxUpdatedControl ControlID="yesAuditing" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
            </updatedcontrols>
    </rad:AjaxSetting>
    <rad:AjaxSetting AjaxControlID="LB_Update">
        <updatedcontrols>
                <rad:AjaxUpdatedControl ControlID="noAuditing" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
            </updatedcontrols>
    </rad:AjaxSetting>
    </form>
</body>
</html>

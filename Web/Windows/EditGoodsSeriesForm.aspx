<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditGoodsSeriesForm.aspx.cs" Inherits="ERP.UI.Web.Windows.EditGoodsSeriesForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/common.js" type="text/javascript"></script>
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script language="javascript" type="text/javascript">
                function invokeAjaxrequest() {
                    $find("<%= RadAjaxPanel1.ClientID%>").ajaxRequestWithTarget("<%= RadAjaxPanel1.UniqueID %>", "load");
                }    
            </script>
        </rad:RadScriptBlock>

    <table class="PanelArea">
        <tr style=" height:25px;">
            <td colspan="2" align="left">
                商品名：<rad:RadTextBox ID="RT_Search" runat="server" Width="210px"></rad:RadTextBox>
                <input type="button" value="搜索" onclick="invokeAjaxrequest();"/>
            </td>
        </tr>
        <tr>
            <td class="solidtd" style="border-top: solid 0px #71879f">
                <rad:RadAjaxPanel ID="RadAjaxPanel1" runat="server" OnAjaxRequest="RadAjaxPanel1_AjaxRequest"> 
                    <rad:RadListBox ID="rlbGoodsClass" runat="server" TransferToID="rlbExpandGoodsClass"
                        Skin="WebBlue" Height="300px" Width="350px" DataValueField="GoodsId" DataTextField="GoodsName"
                        AllowTransfer="true" AllowTransferDuplicates="false" AllowTransferOnDoubleClick="true"
                        TransferMode="Copy">
                        <Localization AllToBottom="全部底对齐" AllToLeft="删除全部" AllToRight="添加全部" 
                            AllToTop="全部顶对齐" Delete="删除" MoveDown="下移" MoveUp="上移" ToBottom="底对齐" 
                            ToLeft="删除" ToRight="添加" ToTop="顶对齐" />
                        <ButtonSettings TransferButtons="All" />
                    </rad:RadListBox>
                    <rad:RadListBox runat="server" ID="rlbExpandGoodsClass" Skin="WebBlue" TransferMode="Move"
                        AllowTransferDuplicates="false" AllowTransferOnDoubleClick="true" Height="300px"
                        ValidationGroup="Update" AllowDelete="true" Width="350px" DataValueField="GoodsId"
                        DataTextField="GoodsName">
                        <Localization AllToBottom="全部底对齐" AllToLeft="添加全部" AllToRight="删除全部" 
                            AllToTop="全部顶对齐" Delete="删除" MoveDown="下移" MoveUp="上移" ToBottom="底对齐" 
                            ToLeft="添加" ToRight="删除" ToTop="顶对齐" />
                        <ButtonSettings TransferButtons="All" />
                    </rad:RadListBox>         
                </rad:RadAjaxPanel>   
            </td>
        </tr>
    </table>

    <!--商品扩展类别保存操作-->
    <asp:Panel ID="PanelSeries" runat="server">
        <table class="PanelArea" width="600px" height="50px">
            <tr>
                <td>
                    <asp:ValidationSummary ID="VSummary" ValidationGroup="addValidate" runat="server" ShowSummary="true" />
                </td>
                <td align="center">
                    <asp:LinkButton ID="lbUpdate" ValidationGroup="addValidate" runat="server" OnClick="ButtonUpdateGoods">
                        <asp:Image ID="imgUpdate" SkinID="UpdateImageButton" runat="server" ImageAlign="AbsMiddle" />更新
                    </asp:LinkButton>
                    <asp:LinkButton ID="lbCancel" runat="server" OnClientClick="return CancelWindow()">
                        <asp:Image ID="imgCancel" SkinID="CancelImageButton" runat="server" ImageAlign="AbsMiddle" />取消
                    </asp:LinkButton>
                </td>
            </tr>
        </table>
    </asp:Panel>

    <rad:RadAjaxManager ID="RAM" runat="server"  EnableHistory="false" EnableViewState="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="lbUpdate">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="PanelSeries" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

    </form>
</body>
</html>

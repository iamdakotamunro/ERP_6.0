<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopRefundMsgForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShopRefundMsgForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>退货留言</title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <table class="PanelArea">
        <tr>
            <td align="right">
                店铺名称：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="RcbShopList" DataTextField="Value" DataValueField="Key">
                </rad:RadComboBox>
            </td>
            <td colspan="2" align="right">
                留言时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RdpStartTime" runat="server" SkinID="Common" Width="120px" ></rad:RadDatePicker>
                 - <rad:RadDatePicker ID="RdpEndTime" runat="server" SkinID="Common" Width="120px" ></rad:RadDatePicker>
            </td>
            <td>&nbsp;</td>
            <td colspan="2" align="center">
                <asp:Button runat="server" ID="BtnSearch" Text="搜索"  Width="80px" OnClick="BtnSearchOnClick"/>
            </td>
        </tr>
    </table>
    <rad:RadGrid runat="server" ID="RgRefundMsgList" OnNeedDataSource="RgRefundMsgListNeedDataSource" 
    OnItemCommand="RgRefundMsgListOnItemCommand">
        <MasterTableView DataKeyNames="MsgID,ShopID" CommandItemSettings-ShowAddNewRecordButton="false" CommandItemDisplay="None">
            <Columns>
                <rad:GridBoundColumn DataField="CreateTime" ReadOnly="true" HeaderText="留言时间">
                    <HeaderStyle HorizontalAlign="Center" Width="120" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ShopName" ReadOnly="true" HeaderText="留言店铺">
                    <HeaderStyle HorizontalAlign="Center" Width="120" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ApplyContent" ReadOnly="true" HeaderText="留言内容">
                    <ItemStyle HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <HeaderStyle HorizontalAlign="Center" Width="80" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:ImageButton runat="server" CausesValidation="false" ID="IbApproved" SkinID="AffirmImageButton"  CommandName="Approved" ToolTip="通过"/>&nbsp;&nbsp;
                        <asp:ImageButton runat="server" CausesValidation="false" ID="IbDelete" SkinID="CancelImageButton" CommandName="Delete" ToolTip="不通过" />
                    </ItemTemplate>
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RefundMsgOnAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BtnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgRefundMsgList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RgRefundMsgList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgRefundMsgList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

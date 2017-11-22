<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddNewInfomation.aspx.cs" Inherits="ERP.UI.Web.Windows.AddNewInfomation" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>商品资料</title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:ScriptManager ID="RSM" runat="server"></asp:ScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script type="text/javascript" src="../JavaScript/telerik.js"></script>
            <script type="text/javascript" src="../JavaScript/jquery.js"></script>
        </rad:RadScriptBlock>
        <div id="DIV4" runat="server">
        <table width="100%">
            <tr style="background-color: #D8E5F4">
                <td colspan="6" style="font-size: 18px; font-weight: bold;">
                    商品资料
                    <br />
                </td>
            </tr>
            <tr>
                <td colspan="3" style="font-size: 18px; font-weight: bold;">
                    <asp:Button ID="BtnAddInformation" runat="server" Text="添加" ValidationGroup="addInformation"
                        OnClick="BtnAddInformation_Click" />
                </td>
                <td colspan="3" style="text-align: left">
                    <asp:Button ID="BtnSave" runat="server" Text="保存"  OnClick="BtnSave_Click" />
                </td>
            </tr>
        </table>
    </div>
    <asp:Panel ID="Panel_Info" runat="server">
        <asp:Repeater ID="Rp_Informations" OnDataBinding="Rp_Informations_OnDataBinding" 
            OnItemDataBound="Rp_Informations_ItemDataBound" OnItemCommand="Rp_Informations_OnItemCommand"
            runat="server">
            <ItemTemplate>
                <table width="100%">
                    <tr style="background-color: #D8E5F4">
                        <td colspan="6" style="font-size: 18px; font-weight: bold;">
                            商品资料
                            <br />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            商品资料名称：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_InformationName" Text='<%#Eval("Name") %>' runat="server" Width="220"
                                ToolTip="商品资料名称"></asp:TextBox>
                        </td>
                        <td align="left">
                            &nbsp;
                        </td>
                        <td>
                            商品资料：
                        </td>
                        <td colspan="2">
                            <rad:RadUpload ID="Upload_GoodsInformation" runat="server" ControlObjectsVisibility="ClearButtons"
                                Width="330px" UseEmbeddedScripts="false" Language="zh-cn" ReadOnlyFileInputs="True" AutoPostBack="true" />
                               <%-- OnValidatingFile="Upload_GoodsInformation_Upload"--%>
                        </td>
                    </tr>
                    <tr>
                        <td>
                            资料过期日期：
                        </td>
                        <td>
                            <rad:RadDatePicker ID="RDPOverdueDate" runat="server" SkinID="Common" Width="195px">
                            </rad:RadDatePicker>
                        </td>
                        <td align="left">
                        </td>
                        <td>
                        </td>
                        <td colspan="2" align="right">
                            <Ibt:ImageButtonControl runat="server" SkinType="Delete" CommandName="DeleteInformation"
                                CausesValidation="false" ID="IB_DeleteInformation" Text="删除"></Ibt:ImageButtonControl>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="hfId" Value='<%# Eval("Id") %>' runat="server" />
                <asp:HiddenField ID="hfPath" Value='<%# Eval("Path") %>' runat="server" />
                <asp:HiddenField ID="hfOverdueDate" Value='<%# Eval("OverdueDate") %>' runat="server" />
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <%--<rad:AjaxSetting AjaxControlID="BtnAddInformation">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_Info" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="BtnSave" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Rp_Informations">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_Info" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Upload_GoodsInformation">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Panel_Info" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>--%>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

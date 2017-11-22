<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.EditBrandForm"
    CodeBehind="EditBrandForm.aspx.cs" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <div class="StagePanel">
        <table border="0" cellpadding="0" cellspacing="0" width="100%">
            <tr>
                <td class="ControlTools">
                    <Ibt:ImageButtonControl ID="LB_Inster" runat="server" OnClick="Button_InsterGoods"
                        SkinType="Insert" Text="添加"></Ibt:ImageButtonControl>
                    <asp:Label ID="Lab_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                    <Ibt:ImageButtonControl ID="LB_Update" runat="server" OnClick="Button_UpdateGoods"
                        SkinType="Affirm" Text="更新"></Ibt:ImageButtonControl>
                    <asp:Label ID="Lab_UpdateSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                    <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()"
                        SkinType="Cancel" Text="取消"></Ibt:ImageButtonControl>
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td style="text-align: right">
                    品牌名称：
                </td>
                <td  style="width: 250px">
                    <asp:HiddenField ID="HF_OrderIndex" runat="server" Value='<%# Eval("OrderIndex") %>' />
                    <asp:TextBox ID="TB_Brand" runat="server" Text='<%# Eval("Brand") %>' Width="250px"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RFVBrand" runat="server" ErrorMessage="品牌名称不能为空！"
                        Text="*" ControlToValidate="TB_Brand"></asp:RequiredFieldValidator>
                </td>
                <td >
                    品牌商标：
                </td>
                <td>
                    <asp:HiddenField ID="HF_BrandLogo" runat="server" Value='<%# Eval("BrandLogo") %>' />
                    <rad:RadUpload ID="RU_BrandLogo" runat="server" Language="zh-cn" UseEmbeddedScripts="false"
                        InitialFileInputsCount="1" MaxFileInputsCount="1" ControlObjectsVisibility="None"
                        ReadOnlyFileInputs="false" Width="180px" OnUnload="Ru_BrandLogo_Upload" TargetFolder="~/UserDir/BrandImages/"
                        AllowedFileExtensions=".gif,.jpg,.png,.bmp" />
                </td>
            </tr>
            <tr>
                <td class="AreaQuaRowTitle" nowrap="nowrap">
                    <%--品牌资料--%>&nbsp;
                </td>
                <td class="AreaRowInfo">
                    <%--<asp:Button ID="BtnAddInformation" runat="server" Text="添加" ValidationGroup="addInformation"
                        OnClick="BtnAddInformation_Click" />--%>
                    &nbsp;
                </td>
                <td class="AreaRowInfo"  align="center" colspan="2">
                    <span style="font-weight: bold">注：品牌商标尺寸必须为180px*180px</span>
                </td>
            </tr>
            <asp:Repeater ID="Rp_Informations" OnDataBinding="Rp_Informations_OnDataBinding"
                OnItemDataBound="Rp_Informations_ItemDataBound" OnItemCommand="Rp_Informations_OnItemCommand"
                runat="server">
                <ItemTemplate>
                    <tr>
                        <td class="AreaRowTitle" nowrap="nowrap">
                            品牌资料名称：
                        </td>
                        <td class="AreaRowInfo">
                            <asp:TextBox ID="TB_InformationName" Text='<%#Eval("Name") %>' runat="server"></asp:TextBox>
                        </td>
                        <td class="AreaRowTitle">
                            资料过期时间：
                        </td>
                        <td class="AreaRowInfo">
                            <rad:RadDatePicker ID="RDPOverdueDate" runat="server" SkinID="Common" Width="195px">
                            </rad:RadDatePicker>
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaRowTitle">
                            品牌资料：
                        </td>
                        <td class="AreaRowInfo" colspan="3">
                            <rad:RadUpload ID="Upload_GoodsInformation" runat="server" ControlObjectsVisibility="ClearButtons"
                                Width="330px" UseEmbeddedScripts="false" Language="zh-cn" ReadOnlyFileInputs="True" 
                                AutoPostBack="true" ToolTip='<%#Eval("Id") %>' OnValidatingFile="Upload_GoodsInformation_Upload" />
                            <asp:HiddenField ID="HF_InforPath" runat="server" />
                        </td>
                    </tr>
                    <asp:HiddenField ID="hfId" Value='<%# Eval("Id") %>' runat="server" />
                <asp:HiddenField ID="hfPath" Value='<%# Eval("Path") %>' runat="server" />
                <asp:HiddenField ID="hfOverdueDate" Value='<%# Eval("OverdueDate") %>' runat="server" />
                </ItemTemplate>
            </asp:Repeater>
            <tr>
                <td class="AreaEditFromTitle">
                    品牌说明：
                </td>
                <td colspan="3">
                    <rad:RadEditor ID="Editor_Description" Width="500px" runat="server" SkinID="Common"
                        Html='<%# Eval("Description") %>'>
                    </rad:RadEditor>
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server">
    </rad:RadAjaxManager>
    </form>
</body>
</html>

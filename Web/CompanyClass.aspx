<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.CompanyClassAw" Title="Untitled Page" Codebehind="CompanyClass.aspx.cs" %>
 <%@Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
 
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" Runat="Server">
<div class="StagePanel">
    <table class="StagePanelHead" border="0" style="width: 100%;">
        <tr>
            <td>
                <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                    <tr>
                        <td>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="ControlTools">
                <asp:Panel ID="ControlPanel" runat="server">
                    <Ibt:ImageButtonControl ID="LB_Inster" runat="server" CausesValidation="false" OnClick="InsterItem" SkinType="Insert" Text="添加">
                    </Ibt:ImageButtonControl>
                    &nbsp;&nbsp;
                    <Ibt:ImageButtonControl ID="LB_Edit" runat="server" CausesValidation="false" OnClick="EditItem" SkinType="Affirm" Text="编辑">
                    </Ibt:ImageButtonControl>
                    &nbsp;&nbsp;
                    <Ibt:ImageButtonControl ID="LB_Delete" runat="server" CausesValidation="false" OnClick="Delete_Click" SkinType="Delete" Text="删除">
                    </Ibt:ImageButtonControl>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <table class="PanelArea">
        <tr>
            <td style="width: 380px; vertical-align: top;">
                <rad:RadTreeView ID="TV_CompanyClass" runat="server" SkinID="Common" OnNodeClick="TvCompanyClassNodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <asp:Panel ID="CompanyClassPanel" runat="server">
                    <table>
                        <tr>
                            <td class="EditFormItemsTitle">
                                分类编码：
                            </td>
                            <td style="width: 200px">
                                <asp:HiddenField ID="HF_CompanyClassId" runat="server" />
                                <asp:HiddenField ID="HF_ParentCompanyClassId" runat="server" />
                                <asp:TextBox ID="TB_CompanyClassCode" runat="server" SkinID="StandardInput" ReadOnly="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RFVCompanyClassCod" runat="server" ControlToValidate="TB_CompanyClassCode"
                                    ErrorMessage="分类编码不允许为空！" Text="*"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="EditFormItemsTitle">
                                分类名称：
                            </td>
                            <td style="width: 200px">
                                <asp:TextBox ID="TB_CompanyClassName" runat="server" SkinID="StandardInput" ReadOnly="true"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RFVCompanyClassName" runat="server" ControlToValidate="TB_CompanyClassName"
                                    ErrorMessage="分类名称不允许为空！" Text="*"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td style="width: 200px; text-align: right;">
                                <asp:ImageButton ID="IB_Add" SkinID="AddImageButton" runat="server" ImageAlign="AbsMiddle"
                                    AlternateText="添加" OnClick="Add_Click" />
                                <asp:Label ID="LBAddSpace" runat="server"></asp:Label>
                                <asp:ImageButton ID="IB_Update" SkinID="UpdateImageButton" runat="server" ImageAlign="AbsMiddle"
                                    AlternateText="更新" OnClick="Update_Click" />
                                <asp:Label ID="LBUpdateSpace" runat="server"></asp:Label>
                                <asp:ImageButton ID="IB_Cancel" SkinID="CancelImageButton" runat="server" CausesValidation="false"
                                    ImageAlign="AbsMiddle" AlternateText="取消" OnClick="Cancel_Click" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</div>
<rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false" DefaultLoadingPanelID="loading">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="LB_Inster">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="ControlPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="CompanyClassPanel" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
        <rad:AjaxSetting AjaxControlID="LB_Edit">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="ControlPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="CompanyClassPanel" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
        <rad:AjaxSetting AjaxControlID="LB_Delete">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="TV_CompanyClass" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="CompanyClassPanel" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
        <rad:AjaxSetting AjaxControlID="IB_Add">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="TV_CompanyClass" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="CompanyClassPanel" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
        <rad:AjaxSetting AjaxControlID="IB_Update">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="TV_CompanyClass" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="CompanyClassPanel" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
        <rad:AjaxSetting AjaxControlID="IB_Cancel">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="ControlPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="CompanyClassPanel" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
        <rad:AjaxSetting AjaxControlID="TV_CompanyClass">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="CompanyClassPanel" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
</rad:RadAjaxLoadingPanel>

</asp:Content>


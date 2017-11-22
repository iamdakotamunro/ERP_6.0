<%@ Page Title="加盟店首页图片管理" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="ShopActivityImageManager.aspx.cs" Inherits="ERP.UI.Web.ShopActivityImageManager" %>

<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <Ibt:ImageButtonControl ID="LB_Update" runat="server" OnClick="Button_UpdateGoods"
        SkinType="Affirm" Text="更新" />
    <rad:RadTabStrip runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" SelectedIndex="0">
        <Tabs>
            <rad:RadTab Text="网站" Width="200px">
            </rad:RadTab>
            <rad:RadTab Text="手机" Width="200px">
            </rad:RadTab>
        </Tabs>
    </rad:RadTabStrip>
    <rad:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0">
        <rad:RadPageView runat="server" ID="RadPageView1">
            <rad:RadEditor ID="Editor_DescriptionWeb" runat="server"  SkinID="ShopWebSite" Width="1100px" Height="550px" >
            </rad:RadEditor>
        </rad:RadPageView>
        <rad:RadPageView runat="server" ID="RadPageView2">
            <rad:RadEditor ID="Editor_DescriptionPhone" runat="server"  SkinID="ShopWebSite" Width="1100px" Height="550px" >
            </rad:RadEditor>
        </rad:RadPageView>
    </rad:RadMultiPage>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="LB_Update">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadMultiPage1" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
</asp:Content>

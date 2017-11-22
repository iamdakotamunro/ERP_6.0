<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="CommonEnterSearchControl.ascx.cs" Inherits="ERP.UI.Web.UserControl.CommonEnterSearchControl" %>
<asp:Panel ID="panSearch" runat="server" DefaultButton="SearchBtn">
    <asp:Label runat="server" ID="lblText"></asp:Label>
    <asp:TextBox ID="TB_Search" runat="server" SkinID="StandardInput" AutoCompleteType="Search"></asp:TextBox>&nbsp;&nbsp;
    <asp:ImageButton ID="SearchBtn" runat="server" OnClick="BtnToSearch" CssClass="ImageBtnIcon" SkinID="SearchButton" Text="搜索"/>
    <label class='ImageBtnLabel' for='<%= SearchBtn.ClientID%>'></label>
</asp:Panel>

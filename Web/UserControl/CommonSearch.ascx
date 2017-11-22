<%@ Control Language="C#" AutoEventWireup="true" Inherits="ERP.UI.Web.UserControl.CommonSearch" Codebehind="CommonSearch.ascx.cs" %>
<%@Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<asp:Label runat="server" ID="lblText" ></asp:Label>
<asp:TextBox ID="TB_Search" runat="server" SkinID="StandardInput"></asp:TextBox>
<Ibt:ImageButtonControl ID="LB_Search" runat="server" onclick="LbSearchClick" SkinType="Search" Text="搜索" >
</Ibt:ImageButtonControl>
&nbsp;&nbsp;&nbsp;
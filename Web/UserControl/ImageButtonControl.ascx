<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ImageButtonControl.ascx.cs"
    Inherits="ERP.UI.Web.UserControl.ImageButtonControl" %>
<div class="ImageBtn" id="divImageButton" runat="server">
    <asp:ImageButton ID="ibtnAction" runat="server" CssClass="ImageBtnIcon"  
        onclick="ibtnAction_Click" />
    <label Class='ImageBtnLabel' for='<%= ibtnAction.ClientID%>' >
        <%= Text%></label>
</div>

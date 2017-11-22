<%@ Control Language="C#" AutoEventWireup="true" Inherits="ERP.UI.Web.UserControl.ChildFieldIControl"
    CodeBehind="ChildFieldIControl.ascx.cs" %>
<div onclick="StopPropagation(event)" class="combo-item-template">
    <div style="font-weight: bolder; font-size: 14px;">
        <input id="chk1" type="checkbox" checked="false" value='<%# Eval("FieldId") %>'
            onclick='<%# "onCheckBoxClick(this.id,\""+Eval("FieldId")+"\")" %>' runat="server"></input>
        <asp:Label ID="Label1" Text='<%# Eval("FieldValue") %>' AssociatedControlID="chk1" runat="server">
        </asp:Label>
    </div>
</div>

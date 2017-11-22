<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="OverwriteCheckBox.ascx.cs" Inherits="ERP.UI.Web.UserControl.OverwriteCheckBox" %>


<asp:LinkButton ID="lbState" runat="server" OnClick="lbState_Click">
    <asp:CheckBox ID="cbState" runat="server" 
        AutoPostBack="false" onclick='<%#  "document.getElementById(\""+hfChecked.ClientID+"\").value=this.checked;if (window.confirm(!this.checked?\""+UncheckingTip+"\":\""+CheckingTip+"\")) {this.parentNode.click();} else { this.checked = !this.checked;}"%>' />
</asp:LinkButton>
<asp:HiddenField ID="hfChecked" runat="server" />
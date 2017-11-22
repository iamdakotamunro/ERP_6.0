<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyFundServiceReceiveEdit.aspx.cs" Inherits="ERP.UI.Web.Windows.CompanyFundServiceReceiveEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RadScriptManager1" runat="server"></rad:RadScriptManager>
    <asp:HiddenField ID="HF_ReceiptID" runat="server" />
    <asp:HiddenField ID="HF_ReceiptType" runat="server" />
    <div style="margin:10px;">        
        <table width="100%">           
            <asp:Panel ID="PL_ReceiveGoods" runat="server">
            <tr>
                <td>货物：</td>
                <td>
                    <label for="receivedGoods"><input name="radioGoods" id="receivedGoods" runat="server" type="radio" value="1" />已到</label>
                    <label for="noReceivedGoods"><input name="radioGoods" id="noReceivedGoods" runat="server" type="radio" value="0" />未到</label>
                    <span id="span_PurchaseOrderNo" runat="server">
                        <rad:RadTextBox ID="RTB_PurchaseOrderNo" runat="server"></rad:RadTextBox>
                    </span>
                </td>
            </tr>
            </asp:Panel>

            <tr>
                <td>往来单位：</td>
                <td>
                    <rad:RadComboBox ID="RCB_CompanyList" runat="server" DropDownWidth="180px" AutoPostBack="true"
                       Height="200" EmptyMessage="选择往来单位" HighlightTemplatedItems="true"
                            EnableLoadOnDemand="true" Filter="StartsWith" 
                        onselectedindexchanged="RCB_CompanyList_SelectedIndexChanged" >
                        <ItemTemplate>
                                <table style="width:180px" cellspacing="0" cellpadding="0">
                                    <tr>
                                        <td style="width:180px;">
                                            <%# DataBinder.Eval(Container, "Text")%>
                                        </td>
                                    </tr>
                                </table>
                            </ItemTemplate>
                    </rad:RadComboBox>
                    <asp:RequiredFieldValidator ID="RFV_CompanyList" EnableClientScript="true" ControlToValidate="RCB_CompanyList" runat="server" ErrorMessage="*" Text="*" />
                </td>
            </tr>
            <tr>
                <td>收款公司：</td>
                <td colspan="3">
                    <rad:RadComboBox ID="RCB_FilialeList" runat="server" AutoPostBack="true" DropDownWidth="180px" AllowCustomText="true" EnableLoadOnDemand="true" 
                       Height="200" EmptyMessage="选择收款公司" CausesValidation="false" Filter="StartsWith"  >
                    </rad:RadComboBox>
                    <asp:RequiredFieldValidator ID="RTF_FilialeList" EnableClientScript="true" ControlToValidate="RCB_FilialeList" runat="server" ErrorMessage="*" Text="*" />
                </td>
            </tr> 
            <tr>
                <td>结账日期：</td>
                <td>
                    <rad:RadDatePicker ID="RDP_StartDate" Width="100" DateInput-ReadOnly="true" runat="server"></rad:RadDatePicker>
                    <asp:RequiredFieldValidator ID="RFV_StartDate" EnableClientScript="true" ControlToValidate="RDP_StartDate" runat="server" ErrorMessage="*" Text="*" />
                    -
                    <rad:RadDatePicker ID="RDP_EndDate" Width="100" AutoPostBack="true" DateInput-ReadOnly="true" OnSelectedDateChanged="RDP_EndDate_OnSelectedDateChanged" runat="server"></rad:RadDatePicker>
                    <asp:RequiredFieldValidator ID="RFV_EndDate" EnableClientScript="true" ControlToValidate="RDP_EndDate" runat="server" ErrorMessage="*" Text="*" />
                </td>
            </tr>
            <tr>
                <td>后台余款：</td>
                <td><rad:RadTextBox ID="RTB_SettleBalance" ReadOnly="true" runat="server"></rad:RadTextBox></td>
            </tr>
            <tr>
                <td>对方余额：</td>
                <td>
                    <rad:RadTextBox ID="RTB_ExpectBalance" runat="server"></rad:RadTextBox>
                    <asp:RequiredFieldValidator ID="RFV_ExpectBalance" EnableClientScript="true" ControlToValidate="RTB_ExpectBalance" runat="server" ErrorMessage="*" Text="*" />
                </td>
            </tr>
            <tr>
                <td><asp:Literal ID="Literal_RealityBalance" runat="server" Text="应付金额"></asp:Literal>：</td>
                <td>                    
                    <rad:RadTextBox ID="RTB_RealityBalance" runat="server" AutoPostBack="true"
                        ontextchanged="RTB_RealityBalance_TextChanged"></rad:RadTextBox>
                    <asp:RequiredFieldValidator ID="RFV_RealityBalance" EnableClientScript="true" ControlToValidate="RTB_RealityBalance" runat="server" ErrorMessage="*" Text="*" />
                    <span><asp:Literal ID="LB_UpperCaseMoney" runat="server"></asp:Literal></span>
                </td>
            </tr>
            <tr>
                <td><asp:Literal ID="Literal_DiscountMoney" runat="server" Text="付款折扣"></asp:Literal>：</td>
                <td><rad:RadTextBox ID="RTB_DiscountMoney" Text="0" runat="server"></rad:RadTextBox></td>
            </tr>
            <tr>
                <td>折扣说明：</td>
                <td>
                    <rad:RadTextBox Width="380" Height="40" TextMode="MultiLine" ID="RTB_DiscountCaption" runat="server"></rad:RadTextBox>
                </td>
            </tr>
            <tr>
                <td>返利备注：</td>
                <td>
                    <rad:RadTextBox Height="60" Width="380" TextMode="MultiLine" ID="RTB_OtherDiscountCaption" runat="server"></rad:RadTextBox>
                </td>
            </tr>
            <tr>
                <td></td>
                <td style="padding-top:5px; padding-bottom:5px;">
                    <Ibt:ImageButtonControl ID="LB_Save" OnClick="LB_Save_OncLick" runat="server" SkinType="Insert" Text="提交">
                    </Ibt:ImageButtonControl>
                    <asp:Label ID="Lab_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                    <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="CancelWindow();return false;" SkinType="Cancel" Text="取消">  
                    </Ibt:ImageButtonControl>
                </td>
            </tr> 
        </table>
    </div>
    
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RTB_RealityBalance">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LB_UpperCaseMoney" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RDP_EndDate">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RTB_SettleBalance" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
              <rad:AjaxSetting AjaxControlID="RCB_BankAccount">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_BankAccount" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_CompanyList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartDate" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RDP_EndDate" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RTB_SettleBalance" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RTB_ExpectBalance" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RTB_RealityBalance" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="LB_UpperCaseMoney" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RTB_DiscountMoney" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RTB_DiscountCaption" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="RTB_OtherDiscountCaption" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" />
    
    </form>    
    </form>
</body>
</html>

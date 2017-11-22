<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyFundReceiveReceiptAdd.aspx.cs" Inherits="ERP.UI.Web.Windows.CompanyFundReceiveReceiptAdd" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script type="text/javascript" src="../JavaScript/jquery.js"></script>
            <script type="text/javascript" src="../JavaScript/common.js"></script>
            <script type="text/javascript" src="../JavaScript/telerik.js"></script>

            <script type="text/javascript">
                function onSubmit() {
                    var balance = parseFloat($("#RTB_SettleBalance").val());
                    if (balance < 0) {
                        if (confirm("我们还欠款此单位 ￥：" + -balance + " 元，是否还继续提交收款单？")) {
                            return true;
                        }
                        else {
                            return false;
                        }
                    }
                    return true;
                }
            </script>
        </rad:RadScriptBlock>

        <div style="margin: 10px;">
            <table width="100%">
                <tr>
                    <td style="text-align: left; width: 90px;">往来单位类型：</td>
                    <td style="padding-left: 5px;">
                        <rad:RadComboBox ID="RcbReceiveType" runat="server" AutoPostBack="true" DropDownWidth="180px" AllowCustomText="true" EnableLoadOnDemand="true"
                            Height="200" CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="RcbReceiveTypeOnSelectedIndexChanged">
                        </rad:RadComboBox>
                    </td>
                </tr>
                <%--<tr>
                    <td style="width: 100px;">
                        <asp:RadioButton ID="RbNormal" runat="server" GroupName="ReceiveType" Text="往来单位类型" AutoPostBack="true" OnCheckedChanged="RbNormalCheckedChanged" />                       
                    </td>
                    <td style="text-align: left;"><asp:RadioButton ID="RbService" runat="server" GroupName="ReceiveType" Text="劳务费类型" AutoPostBack="true" OnCheckedChanged="RbServiceCheckedChanged" /></td>
                </tr>--%>
                <tr>
                    <td colspan="2">
                        <div id="div_Service" runat="server">
                            <table>
                                <tr>
                                    <td style="text-align: left; width: 90px;">往来单位：</td>
                                    <td>
                                        <rad:RadComboBox ID="RcbShopList" runat="server" AutoPostBack="true" DropDownWidth="180px" AllowCustomText="true" EnableLoadOnDemand="true"
                                            Height="200" EmptyMessage="选择往来单位" CausesValidation="false" Filter="StartsWith" OnItemsRequested="RcbShopListListOnItemsRequested" 
                                            OnSelectedIndexChanged="RcbShopListOnSelectedIndexChanged">
                                        </rad:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>收款公司：</td>
                                    <td>
                                        <rad:RadComboBox ID="RcbSaleFiliale" runat="server" AutoPostBack="true" DropDownWidth="180px" AllowCustomText="true" EnableLoadOnDemand="true"
                                            Height="200" EmptyMessage="选择收款公司" CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="RcbSaleFilialeOnSelectedIndexChanged">
                                        </rad:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>后台余款：</td>
                                    <td>
                                        <rad:RadTextBox ID="RadBalance" ReadOnly="true" runat="server" Width="160px"></rad:RadTextBox></td>
                                </tr>
                                <tr>
                                    <td>应收金额：</td>
                                    <td>
                                        <rad:RadTextBox ID="RtbAmount" runat="server" Width="160px"></rad:RadTextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>
                <tr>
                    <td colspan="2">
                        <div id="div_Normal" runat="server">
                            <table>
                                <tr>
                                    <td style="text-align: left; width: 90px;">往来单位：</td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_CompanyList" runat="server" DropDownWidth="180px" AutoPostBack="true"
                                            Height="200" EmptyMessage="选择往来单位" HighlightTemplatedItems="true" OnSelectedIndexChanged="RcbCompanyListSelectedIndexChanged"
                                            EnableLoadOnDemand="true" Filter="StartsWith" OnItemsRequested="RcbCompanyListOnItemsRequested" CausesValidation="false">
                                            <ItemTemplate>
                                                <table style="width: 180px" cellspacing="0" cellpadding="0">
                                                    <tr>
                                                        <td style="width: 180px;">
                                                            <%# DataBinder.Eval(Container, "Text")%>
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </rad:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>收款公司：</td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_FilialeList" runat="server" AutoPostBack="true" DropDownWidth="180px" AllowCustomText="true" EnableLoadOnDemand="true"
                                            Height="200" EmptyMessage="选择收款公司" CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="RCB_FilialeList_OnSelectedIndexChanged">
                                        </rad:RadComboBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>结账日期：</td>
                                    <td>
                                        <table>
                                            <tr>
                                                <td>
                                                    <rad:RadDatePicker ID="RDP_StartDate" Width="100" DateInput-ReadOnly="true" runat="server"></rad:RadDatePicker>
                                                </td>
                                                <td>- 
                                                </td>
                                                <td>
                                                    <rad:RadDatePicker ID="RDP_EndDate" Enabled="false" Width="100" AutoPostBack="true"
                                                        DateInput-ReadOnly="true" runat="server" OnSelectedDateChanged="RdpEndDateSelectedDateChanged">
                                                    </rad:RadDatePicker>
                                                </td>
                                            </tr>
                                        </table>
                                    </td>
                                </tr>
                                <tr>
                                    <td>后台余款：</td>
                                    <td>
                                        <rad:RadTextBox ID="RTB_SettleBalance" ReadOnly="true" runat="server"></rad:RadTextBox></td>
                                </tr>
                                <tr>
                                    <td>对方余额：</td>
                                    <td>
                                        <rad:RadTextBox ID="RTB_ExpectBalance" runat="server"></rad:RadTextBox>
                                    </td>
                                </tr>
                                <tr>
                                    <td>应收金额：</td>
                                    <td>
                                        <rad:RadTextBox ID="RTB_RealityBalance" runat="server" AutoPostBack="true"
                                            OnTextChanged="RtbRealityBalanceTextChanged">
                                        </rad:RadTextBox>
                                        <span>
                                            <asp:Literal ID="LB_UpperCaseMoney" runat="server"></asp:Literal></span>
                                    </td>
                                </tr>
                                <tr>
                                    <td>收款折扣：</td>
                                    <td>
                                        <rad:RadTextBox ID="RTB_DiscountMoney" runat="server" AutoPostBack="true"
                                            OnTextChanged="RTB_DiscountMoney_TextChanged">
                                        </rad:RadTextBox>
                                    </td>
                                </tr>
                                <div id="DIV_DiscountCaption" runat="server" visible="false">
                                    <tr>
                                        <td>折扣说明：</td>
                                        <td>
                                            <rad:RadTextBox Width="380" Height="40" TextMode="MultiLine" ID="RTB_DiscountCaption" runat="server"></rad:RadTextBox>
                                        </td>
                                    </tr>
                                </div>
                                <tr>
                                    <td>差额说明：</td>
                                    <td>
                                        <rad:RadTextBox Width="380" Height="40" TextMode="MultiLine" ID="RTB_OtherDiscountCaption" runat="server"></rad:RadTextBox>
                                    </td>
                                </tr>
                            </table>
                        </div>
                    </td>
                </tr>

                <tr>
                    <td style="padding-top: 5px; padding-bottom: 5px;" colspan="2">
                        <table>
                            <tr>
                                <td>
                                    <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="CancelWindow();return false;" SkinType="Cancel" Text="取消"></Ibt:ImageButtonControl>
                                </td>
                                <td>
                                    <Ibt:ImageButtonControl ID="LB_Inster" runat="server" OnClientClick="return onSubmit()" OnClick="LbInsterOncLick" SkinType="Insert" Text="保存"></Ibt:ImageButtonControl>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
            </table>
        </div>

        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RcbReceiveType">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="div_Normal" />
                        <rad:AjaxUpdatedControl ControlID="div_Service" />
                        <rad:AjaxUpdatedControl ControlID="RcbShopList" />
                        <rad:AjaxUpdatedControl ControlID="RcbSaleFiliale" />
                        <rad:AjaxUpdatedControl ControlID="RadBalance" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RcbShopList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RadBalance" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RcbSaleFiliale">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RadBalance" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_FilialeList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_BankAccount" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
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
                <rad:AjaxSetting AjaxControlID="RTB_DiscountCaption">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="DIV_DiscountCaption" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="LB_Inster">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="LB_Inster" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

    </form>
</body>
</html>

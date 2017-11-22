<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.GoodOrderChart" Title="无标题页" CodeBehind="GoodOrderChart.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
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
        <table align="center">
            <tr>
                <td>
                    公司：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" AllowCustomText="True"
                        Width="150px" Height="100px" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged" AutoPostBack="True">
                    </rad:RadComboBox>
                </td>
                <td>
                    销售平台：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false"
                        AllowCustomText="True" Width="150px" Height="100px" OnSelectedIndexChanged="RCB_SalePlatform_OnSelectedIndexChanged" AutoPostBack="True">
                    </rad:RadComboBox>
                </td>
                <td>
                    选择年份：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Year" runat="server" UseEmbeddedScripts="false" AllowCustomText="True"
                        Width="120px" Height="100px">
                    </rad:RadComboBox>
                    <asp:RegularExpressionValidator ID="REVYear" runat="server" ControlToValidate="RCB_Year"
                        ErrorMessage="*" ValidationExpression="[0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3}"></asp:RegularExpressionValidator>
                </td>
                <td>
                    选择月份：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Month" runat="server" UseEmbeddedScripts="false" AllowCustomText="True"  Width="120px" Height="100px"></rad:RadComboBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="RCB_Year"
                        ErrorMessage="*" ValidationExpression="[0-9]{3}[1-9]|[0-9]{2}[1-9][0-9]{1}|[0-9]{1}[1-9][0-9]{2}|[1-9][0-9]{3}"></asp:RegularExpressionValidator>
                </td>
                <td>
                    <asp:ImageButton ID="IB_CreationChart" runat="server" SkinID="CreationChart" OnClick="IbCreationChartClick" />
                </td>
            </tr>
        </table>
    </div>
    <div align="center">
        <rad:RadChart ID="MemberChart" OnItemDataBound="Chart_ItemDataBound" SkinID="Common" runat="server" Width="1200px" Height="450px" EnableViewState="false"></rad:RadChart>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IB_CreationChart">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="MemberChart" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

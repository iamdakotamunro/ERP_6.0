<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyCussentAuthorityAdd.aspx.cs" Inherits="ERP.UI.Web.Windows.CompanyCussentAuthorityAdd" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="../JavaScript/jquery.js"></script>
    <script src="../JavaScript/telerik.js"></script>
    <style type="text/css">
        .solidtd {
            border: solid 2px #71879f;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <table>
            <tr>
                <td>账号：
                <rad:RadComboBox ID="RCB_Personel" runat="server" CausesValidation="false" AutoPostBack="true"
                    AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="RealName" DataValueField="PersonnelId"
                    OnItemsRequested="RcbPersonelItemsRequested" Width="220px" Height="200px">
                </rad:RadComboBox>
                </td>
                <td style="text-align: left">
                    <asp:Button ID="Btn_Serach" runat="server" Text="搜  索" OnClick="SerachClick" />
                </td>
                <td style="text-align: right;">往来单位：</td>
                <td>
                    <rad:RadComboBox ID="RcbThirdCompany" runat="server"
                        DataTextField="Value" DataValueField="Key"
                        Width="130px" Height="200px" Enabled="False">
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr id="trType">
                <td>
                    <rad:RadListBox ID="FiltrateGoodsList" runat="server" Width="260px" Height="300px" CheckBoxes="true" ShowCheckAll="true">
                    </rad:RadListBox>
                </td>
                <td>
                    <asp:Button ID="btn_Right" runat="server" Text=" > " OnClick="AddToRight" CssClass="button"
                        ToolTip="添加到右边" />
                    <br />
                    <br />
                    <asp:Button ID="btn_Left" runat="server" Text=" < " OnClick="RemoveToLeft" CssClass="button"
                        ToolTip="移除到左边" />
                    <br />
                    <br />
                    <asp:Button ID="btn_AllRight" runat="server" Text=">>" OnClick="AllAddToRight" CssClass="button"
                        ToolTip="全部添加到右边" />
                    <br />
                    <br />
                    <asp:Button ID="btn_AllLeft" runat="server" Text="<<" OnClick="AllRemoveToLeft" CssClass="button"
                        ToolTip="全部移除到左边" />
                    <br />
                </td>
                <td colspan="2">
                    <rad:RadListBox ID="ConfirmGoodsList" runat="server" Width="260px" Height="300px"
                        AutoPostBack="True">
                    </rad:RadListBox>
                </td>
            </tr>
            <tr>
                <td>销售公司：
                <asp:CheckBoxList ID="ckb_RelatedSalesGroupPlatform" runat="server" RepeatDirection="Vertical" Width="100%"></asp:CheckBoxList>
                </td>
                <td></td>
                <td colspan="2"></td>
            </tr>
            <tr>
                <td style="text-align: center" colspan="2">
                    <asp:LinkButton runat="server" ID="LB_Save" OnClick="SaveClick">
                                <asp:Image runat="server" SkinID="UpdateImageButton" ImageAlign="AbsMiddle"/>
                                保存
                    </asp:LinkButton>
                </td>
                <td style="text-align: center" colspan="2">
                    <asp:LinkButton runat="server" ID="Lb_Cancel" OnClick="CancelClick">
                                <asp:Image runat="server" SkinID="CancelImageButton" ImageAlign="AbsMiddle"/>
                                取消
                    </asp:LinkButton>
                </td>
            </tr>
        </table>
        <div>
            <rad:RadAjaxManager ID="RAM" runat="server">
                <AjaxSettings>
                    <rad:AjaxSetting AjaxControlID="Btn_Serach">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                            <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        </UpdatedControls>
                    </rad:AjaxSetting>
                    <rad:AjaxSetting AjaxControlID="btn_Right">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                            <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        </UpdatedControls>
                    </rad:AjaxSetting>
                    <rad:AjaxSetting AjaxControlID="btn_Left">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                            <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        </UpdatedControls>
                    </rad:AjaxSetting>
                    <rad:AjaxSetting AjaxControlID="btn_AllRight">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                            <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        </UpdatedControls>
                    </rad:AjaxSetting>
                    <rad:AjaxSetting AjaxControlID="btn_AllLeft">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                            <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        </UpdatedControls>
                    </rad:AjaxSetting>
                    <rad:AjaxSetting AjaxControlID="ConfirmGoodsList">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        </UpdatedControls>
                    </rad:AjaxSetting>
                </AjaxSettings>
            </rad:RadAjaxManager>
            <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
            </rad:RadAjaxLoadingPanel>
        </div>
    </form>
</body>
</html>

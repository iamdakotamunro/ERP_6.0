<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsBindGiftManager.aspx.cs"
    Inherits="ERP.UI.Web.GoodsBindGiftManager" MasterPageFile="~/MainMaster.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <style type="text/css">
            .solidtd
            {
                border: solid 2px #71879f;
            }
        </style>
    </rad:RadScriptBlock>
    <table>
        <tr>
            <td colspan="3">
                商品名称或编号：
                <rad:RadTextBox ID="TBox_SearchGoodsNameOrCode" runat="server" Text='<%# SearchGoodsNameOrCode%>'
                    Width="360px">
                </rad:RadTextBox>
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:LinkButton runat="server" ID="LB_AllSave" OnClick="AllSaveClick" OnClientClick="return confirm('温馨提示：此操作将会把所选商品的赠品都绑定为所列出赠品,确定操作？')">
                                <asp:Image runat="server" ID="AllSaveImage" SkinID="UpdateImageButton" ImageAlign="AbsMiddle" />
                                批量保存
                            </asp:LinkButton>
                        </td>
                        <td>
                            &nbsp; &nbsp; &nbsp;
                            <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="ExportExcelClick" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <table>
                    <tr>
                        <td>
                            &nbsp; &nbsp; &nbsp; 商品类型：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_GoodsKingType" runat="server" Width="145px" Height="250px"
                                AutoPostBack="true" />
                        </td>
                        <td>
                            &nbsp;商品品牌：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_GoodsBrand" runat="server" Width="145px" Height="320px"
                                AutoPostBack="true" />
                        </td>
                        <td>
                            <asp:Button ID="Btn_Serach" runat="server" Text="搜  索" OnClick="SerachClick" />
                        </td>
                    </tr>
                </table>
            </td>
            <td style="border-bottom: solid 0px #71879f; padding: 5px" class="solidtd">
                <table>
                    <tr>
                        <td>
                            选择赠送商品：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_GoodsGift" runat="server" Width="175px" Height="100px" AutoPostBack="true"
                                EnableLoadOnDemand="True" AllowCustomText="True" OnItemsRequested="RCB_GoodsGiftOnItemsRequested"
                                OnSelectedIndexChanged="RCB_GoodsGiftOnSelectedIndexChanged" />
                        </td>
                        <td>
                            <asp:LinkButton runat="server" ID="LB_Save" OnClick="SaveClick">
                                <asp:Image runat="server" SkinID="UpdateImageButton" ImageAlign="AbsMiddle"/>
                                保存
                            </asp:LinkButton>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td>
                <rad:RadListBox ID="FiltrateGoodsList" runat="server" Width="260px" Height="300px">
                </rad:RadListBox>
            </td>
            <td align="center">
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
            <td>
                <rad:RadListBox ID="ConfirmGoodsList" runat="server" Width="260px" Height="300px"
                    AutoPostBack="True" OnSelectedIndexChanged="ConfirmGoodsListOnSelectedIndexChanged">
                </rad:RadListBox>
            </td>
            <td class="solidtd" style="border-top: solid 0px #71879f; padding: 5px 5px 10px 10px">
                <table>
                    <tr>
                        <td>
                            <rad:RadListBox ID="GoodsGiftList" runat="server" Width="260px" Height="300px" OnSelectedIndexChanged="GoodsGiftListOnSelectedIndexChanged"
                                AutoPostBack="True" />
                        </td>
                        <td valign="top">
                            <asp:Button ID="Btn_Delete" runat="server" Text="删  除" OnClick="DeleteClick" />
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <div>
        <rad:RadAjaxManager ID="RAM" runat="server" >
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RCB_GoodsKingType">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_GoodsKingType" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_GoodsBrand">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_GoodsBrand" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="Btn_Serach">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="btn_Right">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="btn_Left">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="btn_AllRight">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="btn_AllLeft">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="FiltrateGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="Btn_Delete">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="LB_Save">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="LB_Save" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="LB_AllSave">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="LB_AllSave" />
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_GoodsGift">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RCB_GoodsGift"  />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="GoodsGiftList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="ConfirmGoodsList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="ConfirmGoodsList" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="GoodsGiftList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="Btn_Export">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Btn_Export" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </div>
</asp:Content>

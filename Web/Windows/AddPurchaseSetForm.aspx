<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddPurchaseSetForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.AddPurchaseSetForm" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script type="text/javascript" language="javascript">
        function RcbStockingDays_OnClientSelectedIndexChanged(sender, args) {
            var filterVal = args.get_item().get_value();
            var hidVal = document.getElementById("Hid_StockingDays").value;
            if ((parseInt(filterVal) > parseInt(hidVal)) || (parseInt(filterVal) < parseInt(hidVal))) {
                document.getElementById("Div_StockDays").style.display = "block";
            } else {
                document.getElementById("Div_StockDays").style.display = "none";
            }
        }

        function CancelStockDays() {
            document.getElementById("Div_StockDays").style.display = "none";
        }

        function tbPurchasePrice_Change(a) {
            var b = document.getElementById("HidPurchasePrice").value;
            if ((parseFloat(a) > parseFloat(b)) || (parseFloat(a) < parseFloat(b))) {
                document.getElementById("Div_PurchasePrice").style.display = "block";
            } else {
                document.getElementById("Div_PurchasePrice").style.display = "none";
            }
        }

        function CancelPurchasePrice() {
            document.getElementById("Div_PurchasePrice").style.display = "none";
            document.getElementById("tbPurchasePrice").value = document.getElementById("HidPurchasePrice").value;
        }

        function IsValidate() {
            if (document.getElementById("Div_PurchasePrice").style.display == "block") {
                if (document.getElementById("RTB_PurchasePriceReason").value.trim() == "") {
                    alert("采购价调涨原因不能为空!");
                    return false;
                }
            }
            if (document.getElementById("Div_StockDays").style.display == "block") {
                if (document.getElementById("RTXT_StockingDaysReason").value.trim() == "") {
                    alert("报备天数调涨原因不能为空!");
                    return false;
                }
            }
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    </rad:RadScriptBlock>
    <div style="margin: 0; padding-top: 10px; width: 100%;">
        <table width="980" style="margin-left: auto; margin-right: auto;">
            <tr>
                <td align="right">
                    商品名称：
                </td>
                <td style="width: 250px;">
                    <rad:RadComboBox ID="RCB_Goods" runat="server" UseEmbeddedScripts="false" Width="220px"
                        Height="200px" AllowCustomText="true" EnableLoadOnDemand="True" DataTextField="GoodsName"
                        DataValueField="GoodsId" AppendDataBoundItems="true" OnItemsRequested="RCB_Goods_OnItemsRequested">
                    </rad:RadComboBox>
                </td>
                <td style="text-align: center;width: 100px; ">
                    <asp:Button ID="BtnAddGoods" runat="server" Text=" 添 加 " OnClick="AddOnClick" CssClass="button" />  
                    <br />
                    <br /><asp:Button ID="BtnRemoveGoods" runat="server" Text=" 删 除 " OnClick="RemoveOnClick" CssClass="button" /> 
                </td>
                <td style="text-align: left;">
                    <rad:RadListBox ID="SetGoodsList" runat="server" Width="200px" Height="150px">
                    </rad:RadListBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right">
                    采购价：
                </td>
                <td>
                    <asp:TextBox ID="tbPurchasePrice" Text="0" MaxLength="10" Width="150px" runat="server"
                        onchange="tbPurchasePrice_Change(this.value)"></asp:TextBox>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" runat="server" ControlToValidate="tbPurchasePrice"
                        ErrorMessage="采购价不允许为空！"></asp:RequiredFieldValidator>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator5" runat="server" ControlToValidate="tbPurchasePrice"
                        ErrorMessage="采购价必须为数字！" ValidationExpression="([1-9][0-9]*(\.\d+)?)|(0\.\d+)|([0])" />
                    <asp:HiddenField ID="HidPurchasePrice" runat="server" />
                </td>
                <td colspan="2" align="left">
                    <div id="Div_PurchasePrice" style="display: none; margin-left: 40px">
                        <table width="185px" style="border: 2px solid #E4E4E4">
                            <tr style="background-color: #F2F2F2;">
                                <td style="width: 70%">
                                    采购价调涨原因
                                </td>
                                <td>
                                    <asp:LinkButton ID="A_PurchasePrice" Style="color: #FF9966; cursor: pointer" OnClientClick="CancelPurchasePrice()"
                                        OnClick="A_PurchasePrice_Click" runat="server">取消</asp:LinkButton>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <rad:RadTextBox runat="server" ID="RTB_PurchasePriceReason" Width="100%" TextMode="MultiLine" />
                                </td>
                            </tr>
                        </table>
                    </div>
                </td>
            </tr>
            <tr>
                <td align="right">
                    供应商：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Width="155px"
                        Height="120px" AllowCustomText="true" EnableLoadOnDemand="True" DataValueField="CompanyId"
                        DataTextField="CompanyName" AppendDataBoundItems="true" OnItemsRequested="RCB_Company_OnItemsRequested"
                        AutoPostBack="True" OnSelectedIndexChanged="RCB_Company_OnSelectedIndexChanged">
                    </rad:RadComboBox>
                </td>
                <td align="right">
                    所在仓库：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        Width="155px" Height="200px" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="Value" AutoPostBack="true" 
                        DataValueField="Key" OnSelectedIndexChanged="RCB_InStock_OnSelectedIndexChanged">
                    </rad:RadComboBox>
                </td>
                 <td align="right" style="width: 100px;">
                    物流配送公司：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Filile" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        MarkFirstMatch="True" ShowToggleImage="True" Width="120px" Height="200px"
                        AutoPostBack="true">
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    责任人：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Persion" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        AllowCustomText="True" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="RealName"
                        DataValueField="PersonnelId" Width="155px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td align="right">
                    采购分组：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_PurchaseGroup" Width="155px" Height="200px" runat="server">
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    备注：
                </td>
                <td colspan="3">
                    <asp:TextBox ID="tbMemo" Width="600px" Height="55" MaxLength="300" TextMode="MultiLine"
                        runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    是否自动报备：
                </td>
                <td colspan="3">
                    <asp:CheckBox ID="CB_IsStockUp" runat="server" />
                </td>
            </tr>
            <tr>
                <td align="right">
                    报备形式：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_FilingForm" AutoPostBack="True" OnSelectedIndexChanged="RCB_FilingForm_OnSelectedIndexChanged"
                        runat="server">
                        <Items>
                            <rad:RadComboBoxItem Value="1" Text="常规(月周期)" Selected="True" />
                            <rad:RadComboBoxItem Value="2" Text="触发报备" />
                        </Items>
                    </rad:RadComboBox>
                </td>
                <td align="right">
                    备货日：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_StockUpDay" runat="server">
                        <Items>
                            <rad:RadComboBoxItem Value="1" Text="周一" Selected="True" />
                            <rad:RadComboBoxItem Value="3" Text="周三" />
                        </Items>
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    备货量：
                </td>
                <td colspan="3">
                </td>
            </tr>
            <div id="divOne" runat="server">
                <tr>
                    <td align="right">
                        第一周：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbFirstWeek" Text="50" runat="server" onKeyup="this.value=this.value.replace(/-?\D/g,'')"></asp:TextBox>天量
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        第二周：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbSecondWeek" Text="30" runat="server" onKeyup="this.value=this.value.replace(/-?\D/g,'')"></asp:TextBox>天量
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        第三周：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbThirdWeek" Text="40" runat="server" onKeyup="this.value=this.value.replace(/-?\D/g,'')"></asp:TextBox>天量
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        第四周：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbFourthWeek" Text="20" runat="server" onKeyup="this.value=this.value.replace(/-?\D/g,'')"></asp:TextBox>天量
                    </td>
                </tr>
            </div>
            <div id="divTwo" visible="False" runat="server">
                <tr>
                    <td align="right">
                        触发时报备：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbFilingTrigger" Text="0" MaxLength="10" onKeyup="this.value=this.value.replace(/-?\D/g,'')"
                            runat="server"></asp:TextBox>
                        天量 <span style="color: #FF0000;">必填</span>
                    </td>
                </tr>
                <tr>
                    <td align="right">
                        不足：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbInsufficient" Text="0" MaxLength="10" onKeyup="this.value=this.value.replace(/-?\D/g,'')"
                            runat="server"></asp:TextBox>
                        天量触发报备 <span style="color: #FF0000;">必填</span>
                    </td>
                </tr>
            </div>
            <tr>
                <td colspan="4">
                    <hr />
                </td>
            </tr>
            <tr>
                <td align="right">
                    促销信息：
                </td>
                <td colspan="3">
                    <asp:Button ID="btnAddPromotion" runat="server" Text="添加" ValidationGroup="addPromotion"
                        OnClick="BtnAddPromotion_Click" />
                </td>
            </tr>
            <tr>
                <td>
                    &nbsp;
                </td>
                <td colspan="3">
                    <asp:Repeater ID="Rp_PurchasePromotion" OnDataBinding="RpPurchasePromotion_OnDataBinding"
                        OnItemDataBound="Rp_PurchasePromotion_ItemDataBound" OnItemCommand="RpPurchasePromotion_OnItemCommand"
                        runat="server">
                        <ItemTemplate>
                            <table width="669" border="0" cellspacing="0" cellpadding="0">
                                <tr>
                                    <td>
                                        <div>
                                            <asp:RadioButton ID="rbtnPromotionType1" Text="现返" Checked='<%# ((PurchasePromotionType)Eval("PromotionType")==PurchasePromotionType.Back) %>'
                                                runat="server" GroupName="rbtnPromotionType" />
                                            <asp:RadioButton ID="rbtnPromotionType2" Text="非现返" Checked='<%# ((PurchasePromotionType)Eval("PromotionType")==PurchasePromotionType.NoBack) %>'
                                                runat="server" GroupName="rbtnPromotionType" />
                                            <asp:CheckBox runat="server" ID="CkIsSingle" Text="单光度" Checked='<%# Convert.ToBoolean(Eval("IsSingle")) %>'/>
                                        </div>
                                        <div style="margin-top: 3px;">
                                            <asp:CheckBox ID="cbPromotionKind" Checked='<%# (((PromotionKind)Eval("PromotionKind")==PromotionKind.BuyGive)||((PromotionKind)Eval("PromotionKind")==PromotionKind.Both)) %>'
                                                runat="server" />
                                            买
                                            <asp:TextBox ID="tbBuy" Text='<%# Eval("BuyCount") %>' onKeyup="this.value=this.value.replace(/-?\D/g,'')"
                                                Width="50" runat="server"></asp:TextBox>
                                            赠
                                            <asp:TextBox ID="tbGive" Text='<%# Eval("GivingCount") %>' onKeyup="this.value=this.value.replace(/-?\D/g,'')"
                                                Width="50" runat="server"></asp:TextBox>
                                            起止：<rad:RadDatePicker ID="RDP_StartDate" runat="server" SkinID="Common" Width="100px">
                                            </rad:RadDatePicker>
                                            至
                                            <rad:RadDatePicker ID="RDP_EndDate" runat="server" SkinID="Common" Width="100px">
                                            </rad:RadDatePicker>
                                            <br />
                                            <asp:CheckBox ID="cbPromotion" Text="是否促销信息" Checked='<%# (((PromotionKind)Eval("PromotionKind")==PromotionKind.PromotionInfo)||((PromotionKind)Eval("PromotionKind")==PromotionKind.Both)) %>'
                                                runat="server" />
                                            <asp:TextBox ID="tbPromotionInfo" Text='<%# Eval("PromotionInfo") %>' ToolTip="促销信息"
                                                Width="500" runat="server"></asp:TextBox>
                                        </div>
                                    </td>
                                    <td>
                                        <Ibt:ImageButtonControl runat="server" SkinType="Delete" CommandName="DeletePurchasePromotion"
                                            CausesValidation="false" ID="IB_DeletePurchasePromotion" Text="删除"></Ibt:ImageButtonControl>
                                    </td>
                                </tr>
                                <tr>
                                    <td colspan="2">
                                        <hr />
                                    </td>
                                </tr>
                            </table>
                            <asp:HiddenField ID="hfGoodsId" Value='<%# Eval("GoodsId") %>' runat="server" />
                            <asp:HiddenField ID="hfStartDate" Value='<%# Eval("StartDate") %>' runat="server" />
                            <asp:HiddenField ID="hfEndDate" Value='<%# Eval("EndDate") %>' runat="server" />
                            <asp:HiddenField runat="server" ID="HfPromotionid" Value='<%# Eval("PromotionId") %>'/>
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>
            <tr>
                <td colspan="4" style="height: 30px; text-align: center;">
                    <asp:Button ID="btnSave" runat="server" Text="保存" OnClientClick="return IsValidate()"
                        OnClick="BtnSave_Click" Width="80" />
                    <asp:Button ID="btnEdit" runat="server" Text="保存" OnClientClick="return IsValidate()"
                        OnClick="BtnEdit_Click" Width="80" />
                    &nbsp;<input id="btnCancel" value="取消" onclick="return CancelWindow()" style="width: 80px;
                        height: 24px;" type="button" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager runat="server" ID="RAM">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RCB_Company">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_PurchaseGroup" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_StockUpDay">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="divOne" />
                    <rad:AjaxUpdatedControl ControlID="divTwo" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnAddPromotion">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="btnAddPromotion" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="Rp_PurchasePromotion" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Rp_PurchasePromotion">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rp_PurchasePromotion" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BtnAddGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="SetGoodsList" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BtnRemoveGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="SetGoodsList"/>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

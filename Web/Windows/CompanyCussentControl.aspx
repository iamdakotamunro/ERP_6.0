<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.CompanyCussentControl" EnableEventValidation="false"
    CodeBehind="CompanyCussentControl.aspx.cs" %>

<head id="Head1" runat="server">
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script type="text/javascript" src="../JavaScript/telerik.js"></script>
            <script src="../JavaScript/jquery.js"></script>
            <script type="text/javascript">
                $(function () {
                    $("#trType").hide();
                    $("#<%=rbl_SalesScope.ClientID %>").change(function () {
                        var SelectVal = $("input[name='<%=rbl_SalesScope.ClientID %>']:checked").val();
                        if (SelectVal == "0") {
                            $("#trType").hide();
                        }
                        else {
                            $("#trType").show();
                        }
                    });
                });
            </script>
        </rad:RadScriptBlock>
        <table class="StagePanel">
            <tr>
                <table class="PanelArea">
                    <tr>
                        <td class="AreaRowTitle">单位名称：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_CompanyName" runat="server" SkinID="LongInput"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVCompanyName" runat="server" ErrorMessage="添加单位名称"
                                Text="*" ControlToValidate="TB_CompanyName"></asp:RequiredFieldValidator>
                        </td>
                        <td class="AreaRowTitle">所属分类：
                        </td>
                        <td>
                            <asp:DropDownList ID="DDL_CompanyClassId" runat="server" SkinID="LongDropDown">
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFVCompanyClassId" runat="server" ControlToValidate="DDL_CompanyClassId"
                                Text="*" ErrorMessage="所属分类必须选择！" InitialValue="00000000-0000-0000-0000-000000000000"></asp:RequiredFieldValidator>
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaRowTitle">联系人：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_Linkman" runat="server" SkinID="LongInput"></asp:TextBox>
                        </td>
                        <td class="AreaRowTitle">手机号码：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_Mobile" runat="server" SkinID="LongInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaRowTitle">联系地址：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_Address" runat="server" SkinID="LongInput"></asp:TextBox>
                        </td>
                        <td class="AreaRowTitle">邮政编码：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_PostalCode" runat="server" SkinID="LongInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaRowTitle">联系电话：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_Phone" runat="server" SkinID="LongInput"></asp:TextBox>
                        </td>
                        <td class="AreaRowTitle">传真号码：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_Fax" runat="server" SkinID="LongInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaRowTitle">电子邮箱：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_Email" runat="server" SkinID="LongInput"></asp:TextBox>
                            <asp:RegularExpressionValidator ID="REVEmail" runat="server" ControlToValidate="TB_Email"
                                ErrorMessage="邮件地址不正确！" Text="*" ValidationExpression="\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*"></asp:RegularExpressionValidator>
                        </td>
                        <td class="AreaRowTitle">绑定公司：
                        </td>
                        <td>
                            <asp:DropDownList ID="DdlFiliale" runat="server" SkinID="LongDropDown" AutoPostBack="True" OnSelectedIndexChanged="DdlFilialeSelectedChanged"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">对方收款账号信息
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <table width="100%">
                                <tr>
                                    <td>
                                        <asp:Repeater ID="RepeartBank" runat="server" OnItemDataBound="RepeartBankItemDataBound">
                                            <ItemTemplate>
                                                <table width="100%">
                                                    <tr>
                                                        <td class="AreaRowTitle" style="width: 120px; text-align: right;">
                                                            <%#Eval("FilialeName") %>：
                                                        </td>
                                                        <td class="AreaRowTitle" style="width: 80px; text-align: right;">收款人：</td>
                                                        <td>
                                                            <asp:TextBox ID="TbWebSite" runat="server" SkinID="LongInput" Width="120px"></asp:TextBox></td>
                                                        <td class="AreaRowTitle" style="width: 80px; text-align: right;">开户银行：</td>
                                                        <td>
                                                            <asp:TextBox ID="TbBankAccounts" runat="server" SkinID="LongInput" Width="180px"></asp:TextBox></td>
                                                        <td class="AreaRowTitle" style="width: 80px; text-align: right;">银行帐号：</td>
                                                        <td>
                                                            <asp:TextBox ID="TbAccountNo" runat="server" SkinID="LongInput" Width="180px"></asp:TextBox></td>
                                                        <td>
                                                            <asp:HiddenField ID="HiddenBankId" runat="server" Value='<%#Eval("BankAccountsId") %>' />
                                                            <asp:HiddenField ID="HfFilialeId" runat="server" Value='<%#Eval("FilialeId") %>' />
                                                            <asp:HiddenField ID="HfCompanyId" runat="server" Value='<%#Eval("CompanyId") %>' />
                                                            <asp:HiddenField ID="HfFilialeName" runat="server" Value='<%#Eval("FilialeName") %>' />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </td>
                                </tr>
                                <tr>
                                    <td>
                                        <asp:Repeater ID="RepeaterNext" runat="server" OnItemDataBound="RepeartBankItemDataBoundNext">
                                            <ItemTemplate>
                                                <table width="100%">
                                                    <tr>
                                                        <td class="AreaRowTitle" style="width: 120px; text-align: right;"><%#Eval("FilialeName") %>：</td>
                                                        <td class="AreaRowTitle" style="width: 80px; text-align: right;">收款帐号：</td>
                                                        <td>
                                                            <asp:DropDownList runat="server" SkinID="LongDropDown" ID="DdlBankId" Width="180px"></asp:DropDownList></td>
                                                        <td>
                                                            <asp:HiddenField ID="HiddenBankId" runat="server" Value='<%#Eval("BankAccountsId") %>' />
                                                            <asp:HiddenField ID="HfFilialeId" runat="server" Value='<%#Eval("FilialeId") %>' />
                                                            <asp:HiddenField ID="HfCompanyId" runat="server" Value='<%#Eval("CompanyId") %>' />
                                                            <asp:HiddenField ID="HfFilialeName" runat="server" Value='<%#Eval("FilialeName") %>' />
                                                        </td>
                                                    </tr>
                                                </table>
                                            </ItemTemplate>
                                        </asp:Repeater>

                                    </td>
                                </tr>
                            </table>
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaRowTitle">是否开发票：
                        </td>
                        <td>
                            <asp:CheckBox runat="server" ID="CB_IsNeedInvoices" />
                        </td>
                        <td class="AreaRowTitle">账期：
                        </td>
                        <td>
                            <asp:DropDownList ID="DDL_PaymentDays" runat="server" SkinID="LongDropDown" Width="100px">
                                <Items>
                                    <asp:ListItem Selected="True" Value="0">请选择</asp:ListItem>
                                    <asp:ListItem Value="1">1个月</asp:ListItem>
                                    <asp:ListItem Value="2">2个月</asp:ListItem>
                                    <asp:ListItem Value="3">3个月</asp:ListItem>
                                    <asp:ListItem Value="4">4个月</asp:ListItem>
                                    <asp:ListItem Value="5">5个月</asp:ListItem>
                                    <asp:ListItem Value="6">6个月</asp:ListItem>
                                    <asp:ListItem Value="7">7个月</asp:ListItem>
                                    <asp:ListItem Value="8">8个月</asp:ListItem>
                                    <asp:ListItem Value="9">9个月</asp:ListItem>
                                    <asp:ListItem Value="10">10个月</asp:ListItem>
                                    <asp:ListItem Value="11">11个月</asp:ListItem>
                                    <asp:ListItem Value="12">12个月</asp:ListItem>
                                </Items>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaRowTitle">公司类型：
                        </td>
                        <td>
                            <asp:DropDownList ID="DDL_CompanyType" runat="server" AutoPostBack="true" OnSelectedIndexChanged="DdlCompanyTypeSelectedIndexChanged">
                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                <asp:ListItem Text="供应商" Value="1"></asp:ListItem>
                                <asp:ListItem Text="销售商" Value="2"></asp:ListItem>
                                <asp:ListItem Text="物流公司" Value="3"></asp:ListItem>
                                <asp:ListItem Text="会员总帐" Value="4"></asp:ListItem>
                                <asp:ListItem Text="其它" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                            <asp:RequiredFieldValidator ID="RFVCompanyType" runat="server" ControlToValidate="DDL_CompanyType"
                                Text="*" ErrorMessage="公司类型必须选择！"></asp:RequiredFieldValidator>
                        </td>
                        <td class="AreaRowTitle">当前状态：
                        </td>
                        <td>
                            <asp:RadioButtonList ID="RBL_State" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Text="使用" Value="1" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="搁置" Value="0"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                    </tr>
                    <tr>
                        <td class="AreaRowTitle">销售范围：
                        </td>
                        <td>
                            <asp:RadioButtonList ID="rbl_SalesScope" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" OnSelectedIndexChanged="RblSalesScopeSelectedIndexChanged">
                                <asp:ListItem Text="境内" Value="0"></asp:ListItem>
                                <asp:ListItem Text="境外" Value="1"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td class="AreaRowTitle"></td>
                        <td></td>
                    </tr>
                    <tr id="trType">
                        <td class="AreaRowTitle">发货类型：
                        </td>
                        <td>
                            <asp:RadioButtonList ID="rbl_DeliverType" runat="server" RepeatDirection="Horizontal">
                                <asp:ListItem Text="直邮" Value="0"></asp:ListItem>
                                <asp:ListItem Text="转运" Value="1"></asp:ListItem>
                            </asp:RadioButtonList>
                        </td>
                        <td class="AreaRowTitle"></td>
                        <td></td>
                    </tr>
                    <tr>
                        <td class="AreaRowTitle">备注说明：
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="TB_Description" runat="server" TextMode="MultiLine" SkinID="AutoTextarea"></asp:TextBox>
                    </tr>
                    <tr>
                        <td align="center" colspan="4">
                            <asp:ImageButton ID="btnUpdate" runat="server" SkinID="InsertImageButton" AlternateText="保存"
                                OnClick="BtnUpdateClick" />&nbsp;
                        <asp:ImageButton ID="btnCancel" runat="server" SkinID="CancelImageButton" AlternateText="取消"
                            OnClientClick="return CancelWindow()" CausesValidation="false" />
                        </td>
                    </tr>
                </table>
            </tr>
        </table>
        <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="DdlFiliale">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RepeartBank" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="RepeaterNext" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="rbl_SalesScope">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="rbl_DeliverType" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="DDL_CompanyType">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="rbl_DeliverType" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>

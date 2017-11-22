<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyCussentForm.aspx.cs" Inherits="ERP.UI.Web.Windows.CompanyCussentForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
        ul {
            margin-left: -53px;
            margin-top: 0px;
        }

        li {
            list-style: none;
            float: left;
            margin-left: 15px;
            text-align: center;
            height: 30px;
            width: 120px;
            padding-top: 10px;
            border: 1px solid;
            border-bottom: none;
            font-size: larger;
            color: black;
        }

        .item_div {
            border: 1px solid lightskyblue;
            clear: both;
            height: 400px;
            width: 700px;
        }

        .title_r {
            text-align: right;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script type="text/javascript" src="../JavaScript/telerik.js"></script>
            <script src="../JavaScript/jquery.js"></script>
            <script type="text/javascript">
                $(function () {
                    $("#divBasicInfo").css("display", "");
                    $("#divBindFiliale").css("display", "none");
                    $("#divBindAccount").css("display", "none");
                    $("#divOther").css("display", "none");
                    $("li").eq(0).css("color", "lightskyblue");

                    $("li").bind("click", function () {
                        $("#divBasicInfo").css("display", this.value == "1" ? "" : "none");
                        $("#divBindFiliale").css("display", this.value == "2" ? "" : "none");
                        $("#divBindAccount").css("display", this.value == "3" ? "" : "none");
                        $("#divOther").css("display", this.value == "4" ? "" : "none");

                        this.style.color = "lightskyblue";

                        $(this).siblings().each(function (index) {
                            $(this).css("color", "black");
                        })
                    });
                });

                function itemClick() {
                    alert(this.value);
                }
            </script>
        </rad:RadScriptBlock>
        <div id="divItem">
            <ul>
                <li value="1">基础信息</li>
                <li value="2">联系人</li>
                <li value="3">供应商账务信息</li>
                <li value="4">绑定公司</li>
            </ul>
        </div>
        <div id="divBasicInfo" class="item_div">
            <table>
                <tr>
                    <td class="title_r">单位名称：</td>
                    <td><asp:TextBox ID="TbCompanyName" runat="server"></asp:TextBox></td>
                    <td style="width: 50px;">&nbsp;</td>
                    <td class="title_r">单位简称：</td>
                    <td><asp:TextBox ID="TextBox1" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="title_r">单位编码：</td>
                    <td>
                        <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox></td>
                    <td style="width: 50px;">&nbsp;</td>
                    <td class="title_r">企业类型：</td>
                    <td>
                        <asp:DropDownList ID="DropDownList1" runat="server" Width="170"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="title_r">区域：</td>
                    <td colspan="4">
                        <asp:DropDownList ID="DropDownList2" runat="server" Width="170"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="title_r">经营范围：</td>
                    <td>
                        <asp:DropDownList ID="DropDownList3" runat="server" Width="170"></asp:DropDownList></td>
                    <td style="width: 50px;">&nbsp;</td>
                    <td class="title_r">所属分类、客户管理类型：</td>
                    <td>
                        <asp:DropDownList ID="DropDownList4" runat="server" Width="170"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="title_r">授权人姓名：</td>
                    <td>
                        <asp:TextBox ID="TextBox10" runat="server"></asp:TextBox></td>
                    <td style="width: 50px;">&nbsp;</td>
                    <td class="title_r">法人代表：</td>
                    <td>
                        <asp:TextBox ID="TextBox11" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="title_r">授权书号：</td>
                    <td>
                        <asp:TextBox ID="TextBox12" runat="server"></asp:TextBox></td>
                    <td style="width: 50px;">&nbsp;</td>
                    <td class="title_r">授权书有效期：</td>
                    <td>
                        <asp:TextBox ID="TextBox13" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="title_r">税号：</td>
                    <td>
                        <asp:TextBox ID="TextBox14" runat="server"></asp:TextBox></td>
                    <td style="width: 50px;">&nbsp;</td>
                    <td class="title_r">税号有效期：</td>
                    <td>
                        <asp:TextBox ID="TextBox15" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="title_r">委托人姓名：</td>
                    <td>
                        <asp:TextBox ID="TextBox16" runat="server"></asp:TextBox></td>
                    <td style="width: 50px;">&nbsp;</td>
                    <td class="title_r">委托人身份证号码：</td>
                    <td>
                        <asp:TextBox ID="TextBox17" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="title_r">当前状态：</td>
                    <td>
                        <asp:TextBox ID="TextBox7" runat="server"></asp:TextBox></td>
                    <td style="width: 50px;">&nbsp;</td>
                    <td class="title_r">税率：</td>
                    <td>
                        <asp:DropDownList ID="DropDownList5" runat="server"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td>销售范围：</td>
                    <td colspan="4">
                        <asp:TextBox ID="TextBox21" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="title_r">币种：</td>
                    <td>
                        <asp:DropDownList ID="DropDownList7" runat="server" Width="170"></asp:DropDownList></td>
                    <td style="width: 50px;">&nbsp;</td>
                    <td class="title_r">绑定公司：</td>
                    <td>
                        <asp:DropDownList ID="DropDownList6" runat="server" Width="170"></asp:DropDownList></td>
                </tr>
                <tr>
                    <td class="title_r">备注说明：</td>
                    <td colspan="4">
                        <asp:TextBox ID="TextBox20" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td colspan="5">&nbsp;</td>
                </tr>
            </table>
        </div>
        <div id="divBindFiliale" class="item_div">
            <table>
                <tr>
                    <td>
                        <asp:CheckBox ID="IsSame" runat="server" />是否一致</td>
                    <td colspan="6"></td>
                </tr>
                <tr>
                    <td colspan="7">
                        <hr />
                    </td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>联系人：</td>
                    <td>
                        <asp:TextBox ID="TextBox23" runat="server"></asp:TextBox></td>
                    <td>手机号码：</td>
                    <td>
                        <asp:TextBox ID="TextBox24" runat="server"></asp:TextBox></td>
                    <td>电子邮箱：</td>
                    <td>
                        <asp:TextBox ID="TextBox25" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>联系地址：</td>
                    <td>
                        <asp:TextBox ID="TextBox26" runat="server"></asp:TextBox></td>
                    <td>邮政编码：</td>
                    <td>
                        <asp:TextBox ID="TextBox27" runat="server"></asp:TextBox></td>
                    <td>传真号码：</td>
                    <td>
                        <asp:TextBox ID="TextBox28" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>&nbsp;</td>
                    <td>联系电话：</td>
                    <td>
                        <asp:TextBox ID="TextBox29" runat="server"></asp:TextBox></td>
                    <td colspan="4">&nbsp;</td>
                </tr>
            </table>
        </div>
        <div id="divBindAccount" class="item_div">
            <table>
                <tr>
                    <td>是否开发票：<asp:CheckBox ID="CkContainsInvoice" runat="server" /></td>
                    <td>账期：</td>
                    <td>
                        <asp:DropDownList ID="DdlPaymentDays" runat="server" Width="170"></asp:DropDownList></td>
                    <td>&nbsp;</td>
                </tr>
                <tr>
                    <td colspan="4">供应商收款账号信息</td>
                </tr>
                <tr>
                    <td colspan="4">
                        <asp:Repeater ID="RepeartBank" runat="server"><%--OnItemDataBound="RepeartBankItemDataBound"--%>
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
                                            <asp:TextBox ID="TbBankAccounts" runat="server" SkinID="LongInput" Width="170px"></asp:TextBox></td>
                                        <td class="AreaRowTitle" style="width: 80px; text-align: right;">银行帐号：</td>
                                        <td>
                                            <asp:TextBox ID="TbAccountNo" runat="server" SkinID="LongInput" Width="170px"></asp:TextBox></td>
                                        <td class="AreaRowTitle" style="width: 80px; text-align: right;">税号：</td>
                                        <td>
                                            <asp:TextBox ID="TbTaxNo" runat="server" SkinID="LongInput" Width="170px"></asp:TextBox></td>
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
        </div>
        <div id="divOther" class="item_div">
            <table>
                <tr>
                    <td>绑定到我方公司</td>
                </tr>
                <tr>
                    <td>
                        <asp:CheckBoxList ID="CkBindFiliale" runat="server" RepeatDirection="Horizontal" Width="100%"></asp:CheckBoxList>
                    </td>
                </tr>
            </table>
            
        </div>
        <div id="divSave" class="item_div">
        </div>
    </form>
</body>
</html>

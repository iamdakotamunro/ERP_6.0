<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SetPrice.aspx.cs" Inherits="ERP.UI.Web.Windows.SetPrice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <style>
        .allsize
        {
            font-size: 24px;
            font-weight: bold;
        }
    </style>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
        <script language="javascript" type="text/javascript">
            function CheckDecimal(control, defaultValue) {
                var reg = /^\d+(\.\d{1,2})?$/;
                if (reg.test(control.value) == false) {
                    control.value = defaultValue;
                    alert("格式不正确，请重新输入");
                }
            }
        </script>
    </rad:RadScriptBlock>

    <div>
        <table class="StagePanel" align="center">
            <tr>
                <td colspan="2" class="allsize">
                    商品名称：<asp:Label ID="Lbl_GoodsName" runat="server"></asp:Label>
                </td>
            </tr>
            <tr>
                <td class="allsize">
                    市场价：<asp:Label ID="Lbl_MarketPrice" runat="server"></asp:Label>
                </td>
                <td align="left" class="allsize">
                    参考价：
                    <asp:TextBox ID="TB_ReferencePrice" MaxLength="5" runat="server" Width="70px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" style="font-size:14px;" ControlToValidate="TB_ReferencePrice" ErrorMessage="价格必须为大于零的数字！" ValidationExpression="([1-9][0-9]*(\.\d+)?)|(0\.\d+)"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator1" ErrorMessage="请输入参考价" Text="*" ControlToValidate="TB_ReferencePrice" runat="server"></asp:RequiredFieldValidator>
                    <asp:Button ID="BT_SavePrice" runat="server" OnClick="Bt_SavePrice_Click" Text="更新参考价" />
                </td>
            </tr>
            <tr>
                <td>&nbsp;</td>
                <td align="left" class="allsize">
                    加盟价： 
                    <asp:TextBox ID="TB_JoinPirce" MaxLength="5" runat="server" Width="70px"></asp:TextBox>
                    <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" style="font-size:14px;" ControlToValidate="TB_JoinPirce" ErrorMessage="价格必须为大于零的数字！" ValidationExpression="([1-9][0-9]*(\.\d+)?)|(0\.\d+)"></asp:RegularExpressionValidator>
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator3" ErrorMessage="请输入加盟价" Text="*" ControlToValidate="TB_JoinPirce" runat="server"></asp:RequiredFieldValidator>
                    <asp:Button ID="BT_JoinPirce" runat="server" OnClick="Bt_JoinPirce_Click" Text="更新加盟价" />
                </td>
            </tr>
            <tr style="display: none">
                <td>&nbsp;</td>
                <td align="left" class="allsize">
                    隐性成本： 
                    <asp:TextBox ID="TB_ImplicitCost" Text="0" onblur="CheckDecimal(this,'0');" MaxLength="4" runat="server" Width="70px"></asp:TextBox>（范围0+）
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator4" ErrorMessage="请输入隐性成本" Text="*" ControlToValidate="TB_ImplicitCost" runat="server"></asp:RequiredFieldValidator>
                    <asp:Button ID="BT_ImplicitCost" runat="server" OnClick="Bt_ImplicitCost_Click" Text="更新隐性成本" />
                </td>
            </tr>
            <tr style="display: none">
                <td>&nbsp;</td>
                <td align="left" class="allsize">
                    年终扣率： 
                    <asp:TextBox ID="TB_YearDiscount" Text="1" onblur="CheckDecimal(this,'1');" MaxLength="4" runat="server" Width="70px"></asp:TextBox>（范围1～2，默认1为无折扣）
                    <asp:RequiredFieldValidator ID="RequiredFieldValidator5" ErrorMessage="请输入年终扣率" Text="*" ControlToValidate="TB_YearDiscount" runat="server"></asp:RequiredFieldValidator>
                    <asp:Button ID="BT_YearDiscount" runat="server" OnClick="Bt_YearDiscount_Click" Text="更新年终扣率" />
                </td>
            </tr>
            <tr>
                <td colspan="2" align="right" style="border-top: 3px double #ccc;">
                    <asp:Button ID="Btn_UpdateAll" runat="server" Text="全部更新" OnClick="Btn_UpdateAll_Click" Visible="False"/>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <asp:Repeater ID="repPriceList" OnItemDataBound="RepPriceList_ItemDataBound" OnItemCommand="RepPriceList_ItemCommand" runat="server">
                        <ItemTemplate>
                            <fieldset style="background-color: #F2F2F2;">
                                <legend><asp:Label ID="lbGroupName" Text='<%#Eval("GroupName") %>' style="font-size: 22px;font-weight: bold;background-color: #FF9933;" runat="server"></asp:Label></legend>
                                <table width="100%">
                                    <tr>
                                        <td align="center">
                                            <asp:HiddenField ID="hfGroupId" Value='<%#Eval("GroupId") %>' runat="server"></asp:HiddenField>
                                            官网价：<asp:TextBox ID="tbPrice" MaxLength="5" runat="server"></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="rev" ControlToValidate="tbPrice" ErrorMessage="价格必须为大于零的数字！" ValidationExpression="([1-9][0-9]*(\.\d+)?)|(0\.\d+)" runat="server"></asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="aa" ErrorMessage="请输入价格" Text="*" ControlToValidate="tbPrice" runat="server"></asp:RequiredFieldValidator>
                                            &nbsp;<asp:Button ID="BtnCreateEyesMemberPrice" runat="server" Text="生成会员价" CommandName="CreateEyesMemberPrice" CausesValidation="False" Visible="False"/>
                                            &emsp;&emsp;&emsp;<asp:Button ID="btnPrice" runat="server" Text="更 新" CommandName="UpdatePrice" Visible="False"/>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <rad:RadGrid ID="RG_Price" AllowPaging="False" runat="server">
                                                <ClientSettings>
                                                    <Selecting AllowRowSelect="False" EnableDragToSelectRows="false" />
                                                </ClientSettings>
                                                <MasterTableView DataKeyNames="RoleId" CommandItemDisplay="None">
                                                    <Columns>
                                                        <rad:GridBoundColumn DataField="RoleID" HeaderText="RoleID" UniqueName="RoleID" Display="false"></rad:GridBoundColumn>
                                                        <rad:GridBoundColumn HeaderText="等级名称" DataField="RoleName" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
                                                        <rad:GridTemplateColumn HeaderText="折扣">
                                                            <ItemTemplate>
                                                                <asp:Label ID="Lbl_Discount" runat="server" Text='<%# GetDiscount(Eval("Discount").ToString()) %>'></asp:Label>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </rad:GridTemplateColumn>
                                                        <rad:GridTemplateColumn HeaderText="价格">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="TB_Price" Text='<%# Eval("Price") %>' MaxLength="5" runat="server"></asp:TextBox>
                                                                <asp:RegularExpressionValidator ID="REVCostes" ControlToValidate="TB_Price" ErrorMessage="价格必须为大于零的数字！" ValidationExpression="([1-9][0-9]*(\.\d+)?)|(0\.\d+)" runat="server"></asp:RegularExpressionValidator>
                                                                <asp:RequiredFieldValidator ID="bb" ErrorMessage="请输入价格" Text="*" ControlToValidate="TB_Price" runat="server"></asp:RequiredFieldValidator>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </rad:GridTemplateColumn>
                                                    </Columns>
                                                </MasterTableView>
                                            </rad:RadGrid>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            <hr/>
                                            <rad:RadGrid ID="RG_ThirdPrice" AllowPaging="False" Visible="False" runat="server" OnItemDataBound="RgThirdPriceItemDataBound">
                                                <ClientSettings>
                                                    <Selecting AllowRowSelect="False" EnableDragToSelectRows="false" />
                                                </ClientSettings>
                                                <MasterTableView DataKeyNames="SalePlatformId,IsDefault" CommandItemDisplay="None"> 
                                                    <Columns>
                                                        <rad:GridBoundColumn DataField="SalePlatformId" HeaderText="SalePlatformId" UniqueName="SalePlatformId" Display="false"></rad:GridBoundColumn>
                                                        <rad:GridBoundColumn HeaderText="第三方销售平台" DataField="SalePlatformName" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
                                                        <rad:GridTemplateColumn HeaderText="价格">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="TB_ThirdPrice" Text='<%# Eval("Price") %>' MaxLength="5" runat="server"></asp:TextBox>
                                                                <asp:RegularExpressionValidator ID="REVThirdPrice" ControlToValidate="TB_ThirdPrice" ErrorMessage="价格必须为大于零的数字！" ValidationExpression="([1-9][0-9]*(\.\d+)?)|(0\.\d+)" runat="server"></asp:RegularExpressionValidator>
                                                                <asp:RequiredFieldValidator ID="bbThirdPrice" ErrorMessage="请输入价格" Text="*" ControlToValidate="TB_ThirdPrice" runat="server"></asp:RequiredFieldValidator>
                                                            </ItemTemplate>
                                                            <HeaderStyle HorizontalAlign="Center" />
                                                            <ItemStyle HorizontalAlign="Center" />
                                                        </rad:GridTemplateColumn>
                                                    </Columns>
                                                </MasterTableView>
                                            </rad:RadGrid>
                                        </td>
                                    </tr>
                                </table>
                            </fieldset>
                            
                        </ItemTemplate>
                    </asp:Repeater>
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="repPriceList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="repPriceList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>
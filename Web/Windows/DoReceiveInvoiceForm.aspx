<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DoReceiveInvoiceForm.aspx.cs" Inherits="ERP.UI.Web.Windows.DoReceiveInvoiceForm" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>开具发票</title>
    <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    <script type="text/javascript" src="../JavaScript/common.js"></script>
    <script type="text/javascript" src="../JavaScript/telerik.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RadScriptManager1" runat="server"></rad:RadScriptManager>
        <div style="margin: 10px;">
            <table width="100%">
                <tr>
                    <td>票据类型：</td>
                    <td style="width: 100px;">
                        <asp:RadioButton ID="RbNormal" runat="server" GroupName="ReceiveType" Text="往来单位类型" Enabled="False" />
                    </td>
                    <td style="text-align: left;">
                        <asp:RadioButton ID="RbService" runat="server" GroupName="ReceiveType" Text="劳务费类型" Enabled="False" /></td>
                    <td colspan="2">&nbsp;</td>
                </tr>
                <tr>
                    <td style="text-align: right;">票据信息：</td>
                    <td colspan="2">
                        <rad:RadTextBox ID="RtbTradeCode" ReadOnly="true" runat="server"></rad:RadTextBox></td>
                    <td style="text-align: right;">往来单位：</td>
                    <td>
                        <rad:RadTextBox ID="RtbCompany" ReadOnly="true" runat="server"></rad:RadTextBox></td>
                </tr>
                <tr>
                    <td style="text-align: right;">收款单位：</td>
                    <td colspan="2">
                        <rad:RadTextBox ID="RtbReciverUnit" ReadOnly="true" runat="server"></rad:RadTextBox></td>
                    <td style="text-align: right;">
                        <asp:Label ID="LbAmountOrDate" runat="server" Text="应收金额："></asp:Label></td>
                    <td>
                        <rad:RadTextBox ID="RtbAmountOrDate" ReadOnly="true" runat="server"></rad:RadTextBox></td>
                </tr>
                <tr>
                    <td style="text-align: right;">
                        <asp:Label ID="LbReceiver" runat="server" Text="应收金额："></asp:Label></td>
                    <td colspan="2">
                        <rad:RadTextBox ID="RtbReceiver" ReadOnly="true" runat="server"></rad:RadTextBox></td>
                    <td style="text-align: right;">
                        <asp:Label ID="LbDiscount" runat="server" Text="收款折扣："></asp:Label></td>
                    <td>
                        <rad:RadTextBox ID="RtbDisCount" ReadOnly="true" runat="server"></rad:RadTextBox></td>
                </tr>
                <tr>
                    <td colspan="5">
                        <table width="100%">
                            <tr>
                                <td style="width: 67px;">开票单位：</td>
                                <td colspan="5">
                                    <rad:RadTextBox ID="RtbUnit" runat="server" Width="200"></rad:RadTextBox></td>
                            </tr>
                            <tr>
                                <td colspan="6">
                                    <asp:FileUpload ID="FupFile" runat="server"/>
                                    <asp:Button runat="server" ID="BtnImport" Text="发票信息导入" OnClick="BtnImportOnClick"/> 
                                    <asp:Button runat="server" ID="BtnExport" Text="发票模板导出" OnClick="BtnExportOnClick"/> 
                                </td>
                            </tr>
                            <tr>
                                <td colspan="1">票据类型：</td>
                                <td colspan="2">
                                    <rad:RadComboBox ID="RcbInvoiceType" runat="server" AutoPostBack="true" DropDownWidth="150px" AllowCustomText="true" EnableLoadOnDemand="true"
                                        Height="80" CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="RcbInvoiceTypeOnSelectedIndexChanged">
                                    </rad:RadComboBox>
                                </td>
                                <td colspan="3" style="text-align: right;">
                                    <asp:Button runat="server" ID="BtnAdd" Text="添加" /></td>
                            </tr>
                            <tr>
                                <td colspan="6">
                                <rad:RadGrid ID="RgInvoice" SkinID="Common" runat="server">
                                    <MasterTableView CommandItemDisplay="None" DataKeyNames="InvoiceId,IsCanEdit">
                                        <Columns>
                                            <rad:GridTemplateColumn HeaderText="开票日期" DataField="RequestTime">
                                                <ItemTemplate>
                                                    <rad:RadDatePicker ID="RdpRequestTime" runat="server" SkinID="Common" Width="95px" SelectedDate='<%# GetDate(Eval("RequestTime")) %>'></rad:RadDatePicker>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                            </rad:GridTemplateColumn>
                                            <rad:GridTemplateColumn HeaderText="票据号码" DataField="InvoiceNo" UniqueName="InvoiceNo">
                                                <ItemTemplate>
                                                    <asp:TextBox runat="server" ID="TbInvoiceNo" Text='<%# Eval("InvoiceNo").ToString() %>' Width="80px" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                            </rad:GridTemplateColumn>
                                            <rad:GridTemplateColumn HeaderText="发票代码" DataField="InvoiceCode" UniqueName="InvoiceCode">
                                                <ItemTemplate>
                                                    <asp:TextBox runat="server" ID="TbInvoiceCode" Text='<%# Eval("InvoiceCode").ToString() %>' Width="80px" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                            </rad:GridTemplateColumn>
                                            <rad:GridTemplateColumn HeaderText="未税金额" DataField="UnTaxFee" UniqueName="UnTaxFee">
                                                <ItemTemplate>
                                                    <asp:TextBox runat="server" ID="TbUnTaxFee" Text='<%# Convert.ToDecimal(Eval("UnTaxFee"))==0?"":Eval("UnTaxFee").ToString() %> ' onkeyup="if(isNaN(value))execCommand('undo')"
                                                        Width="80px" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                            </rad:GridTemplateColumn>
                                            <rad:GridTemplateColumn HeaderText="税额" DataField="TaxFee" UniqueName="TaxFee">
                                                <ItemTemplate>
                                                    <asp:TextBox runat="server" ID="TbTaxFee" Text='<%# Convert.ToDecimal(Eval("TaxFee"))==0?"":Eval("TaxFee").ToString() %> ' onkeyup="if(isNaN(value))execCommand('undo')"
                                                        Width="80px" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                            </rad:GridTemplateColumn>
                                            <rad:GridTemplateColumn HeaderText="含税金额" DataField="TotalFee">
                                                <ItemTemplate>
                                                    <asp:TextBox runat="server" ID="TbTotalFee" Text='<%# Convert.ToDecimal(Eval("TotalFee"))==0?"":Eval("TotalFee").ToString() %> ' onkeyup="if(isNaN(value))execCommand('undo')"
                                                        Width="80px" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                            </rad:GridTemplateColumn>
                                            <rad:GridTemplateColumn HeaderText="备注" DataField="Remark">
                                                <ItemTemplate>
                                                    <asp:TextBox runat="server" ID="TbRemark" Text='<%# Eval("Remark").ToString() %>' Width="80px" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                            </rad:GridTemplateColumn>
                                            <rad:GridTemplateColumn HeaderText="操作">
                                                <ItemTemplate>
                                                    <asp:Button runat="server" ID="BtnDelete" Text="删除" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>' OnClick="BtnDeleteOnClick"></asp:Button>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                                <ItemStyle HorizontalAlign="Center" Width="60"></ItemStyle>
                                            </rad:GridTemplateColumn>
                                        </Columns>
                                    </MasterTableView>
                                </rad:RadGrid></td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td colspan="5"><font color="red">发票金额不能大于应收金额</font></td>
                </tr>
                <tr>
                    <td>拒绝理由：
                    </td>
                    <td colspan="4">
                        <rad:RadTextBox Height="60" Width="530" TextMode="MultiLine" ID="RtbRefuseReason" runat="server"></rad:RadTextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center;" colspan="5">
                        <Ibt:ImageButtonControl ID="LbBack" runat="server" OnClick="LbBackOncLick" SkinType="Cancel" Text="退回"></Ibt:ImageButtonControl>
                        <asp:Label ID="Lab_InsterSpace" runat="server" Text="Label" Width="50px">&nbsp;</asp:Label>
                        <Ibt:ImageButtonControl ID="LbSave" OnClick="LbSaveOncLick" runat="server" SkinType="Insert" Text="开票"></Ibt:ImageButtonControl>
                    </td>
                </tr>
            </table>
        </div>
        
        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
                <%--<rad:AjaxSetting AjaxControlID="RgInvoice">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RgInvoice"/>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="BtnImport">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RgInvoice"/>
                    </UpdatedControls>
                </rad:AjaxSetting>--%>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

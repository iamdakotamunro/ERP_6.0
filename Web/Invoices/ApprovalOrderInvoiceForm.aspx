<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalOrderInvoiceForm.aspx.cs" Inherits="ERP.UI.Web.Invoices.ApprovalOrderInvoiceForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .back {
            background-color:cornflowerblue;
            border: none;
        }

        .pass {
            background-color: orange;
            border: none;
        }

        .company{}
        .per{}
        .invoice{}
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
                function load(per,type) {
                    if (per == "1") {
                        $("tr[class='per']").each(function() {
                            $(this).css("display", "");
                        });
                        $("tr[class='company']").each(function () {
                            $(this).css("display", "none");
                        });
                    } else {
                        $("tr[class='per']").each(function () {
                            $(this).css("display", "none");
                        });
                        $("tr[class='company']").each(function () {
                            $(this).css("display", "");
                        });
                    }
                    $("tr[class='invoice']").each(function () {
                        $(this).css("display", type=="0"?"none":"");
                    });
                }
            </script>
        </rad:RadScriptBlock>
        <div runat="server" id="DivAuditingPanel">
            <table>
                <tr>
                    <td colspan="4">订单信息</td>
                </tr>
                <tr>
                    <td class="ShortFromRowTitle">订单编号：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbOrderNo" runat="server" Enabled="False" Width="180"></asp:TextBox>
                    </td>
                    <td class="ShortFromRowTitle">第三方订单编号：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbThirdPartyOrderNo" runat="server" Enabled="False" Width="180"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ShortFromRowTitle">收货人：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbReceiver" runat="server" Enabled="False" Width="180"></asp:TextBox>
                    </td>
                    <td class="ShortFromRowTitle">手机号码：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbTelephone" runat="server" Enabled="False" Width="180"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ShortFromRowTitle">收货地址：
                    </td>
                    <td class="AreaEditFromRowInfo" colspan="3">
                        <asp:TextBox ID="TbAddress" runat="server" Enabled="False" Width="600"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="ShortFromRowTitle">申请理由：
                    </td>
                    <td class="AreaEditFromRowInfo" colspan="3">
                        <asp:TextBox ID="TbApplyRemark" runat="server" Enabled="False" Width="600" TextMode="MultiLine" Height="40"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="4"><hr/></td>
                </tr>
                <tr>
                    <td colspan="4">申请发票信息</td>
                </tr>
                <tr>
                    <td class="ShortFromRowTitle">发票抬头：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbTitle" runat="server" Width="180" Enabled="False"></asp:TextBox>
                    </td>
                    <td class="ShortFromRowTitle" style="width: 120px;">发票类型：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbTitleType" runat="server" Width="180" Enabled="False"></asp:TextBox>
                    </td>
                </tr>
                <tr class="company">
                    <td class="ShortFromRowTitle">发票金额：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbAmount" runat="server" Width="180" Enabled="False"></asp:TextBox>
                    </td>
                    <td class="ShortFromRowTitle" style="width: 120px;">纳税人识别号：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbTaxNumber" runat="server" Width="180" Enabled="False"></asp:TextBox>
                    </td>
                </tr>
                <tr class="company">
                    <td class="ShortFromRowTitle">开户行名称：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbBankName" runat="server" Width="180" Enabled="False"></asp:TextBox>
                    </td>
                    <td class="ShortFromRowTitle" style="width: 120px;">银行账号：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbBankAccountNo" runat="server" Width="200" Enabled="False"></asp:TextBox>
                    </td>
                </tr>
                <tr class="company">
                    <td class="ShortFromRowTitle">公司地址：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbContactAddress" runat="server" Width="180" Enabled="False"></asp:TextBox>
                    </td>
                    <td class="ShortFromRowTitle" style="width: 120px;">联系电话：
                    </td>
                    <td class="AreaEditFromRowInfo">
                        <asp:TextBox ID="TbContactPhone" runat="server" Width="180" Enabled="False"></asp:TextBox>
                    </td>
                </tr>
                <tr class="per">
                    <td class="ShortFromRowTitle">发票金额：
                    </td>
                    <td class="AreaEditFromRowInfo" colspan="3">
                        <asp:TextBox ID="TbPerAmount" runat="server" Width="180" Enabled="False"></asp:TextBox>
                    </td>
                </tr>
                <tr class="invoice">
                    <td colspan="4">发票信息</td>
                </tr>
                <tr class="invoice">
                    <td colspan="3" style="text-align: left;">
                        <asp:FileUpload ID="FupFile" runat="server"/>
                        <asp:Button runat="server" ID="BtnImport" Text="发票信息导入" OnClick="BtnImportOnClick"/> 
                        <asp:Button runat="server" ID="BtnExport" Text="发票模板导出" OnClick="BtnExportOnClick"/> 
                    </td>
                    <td style="text-align: right;">
                       <asp:Button runat="server" ID="BtnAdd" Text="添加" OnClick="BtnAddOnClick"/> 
                    </td>
                </tr>
                <tr class="invoice">
                    <td colspan="4">
                        <rad:RadGrid ID="RgInvoice" SkinID="Common" runat="server" ShowFooter="true">
                            <MasterTableView CommandItemDisplay="None" DataKeyNames="InvoiceId,IsCanEdit">
                                <Columns>
                                    <rad:GridTemplateColumn HeaderText="开票日期" DataField="RequestTime">
                                        <ItemTemplate>
                                            <rad:RadDatePicker ID="RdpRequestTime" runat="server" SkinID="Common" Width="95px" SelectedDate='<%# GetDate(Eval("RequestTime")) %>'></rad:RadDatePicker>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="发票号码" DataField="InvoiceNo" UniqueName="InvoiceNo">
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
                                            <asp:TextBox runat="server" ID="TbUnTaxFee" Text='<%# Convert.ToDecimal(Eval("UnTaxFee"))==0?"":Eval("UnTaxFee").ToString() %>' onkeyup="if(isNaN(value))execCommand('undo')"
                                                 Width="80px" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>'></asp:TextBox>
                                        </ItemTemplate>
                                        <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="税额" DataField="TaxFee" UniqueName="TaxFee">
                                        <ItemTemplate>
                                            <asp:TextBox runat="server" ID="TbTaxFee" Text='<%# Convert.ToDecimal(Eval("TaxFee"))==0?"":Eval("TaxFee").ToString() %>' onkeyup="if(isNaN(value))execCommand('undo')" 
                                                Width="80px" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>'></asp:TextBox>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" Width="80"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn HeaderText="含税金额" DataField="TotalFee" UniqueName="TotalFee">
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
                                            <asp:Button runat="server" ID="BtnDelete" Text="删除" OnClick="BtnDeleteOnClick" Enabled='<%# Convert.ToBoolean(Eval("IsCanEdit")) %>'></asp:Button>
                                        </ItemTemplate>
                                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                                        <ItemStyle HorizontalAlign="Center" Width="60"></ItemStyle>
                                    </rad:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </rad:RadGrid>
                    </td>
                </tr>

                <tr>
                    <td class="ShortFromRowTitle">拒绝理由：
                    </td>
                    <td class="AreaEditFromRowInfo" colspan="3">
                        <asp:TextBox ID="TbRetreat" runat="server" Width="600" TextMode="MultiLine" Height="40"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td colspan="4">
                        &nbsp;
                    </td>
                </tr>
                <tr>
                    <td align="center" colspan="4">
                        <asp:Button ID="BtBack" runat="server" Text="核退" OnClick="BtBackClick" CssClass="back"/>&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="BtSave" runat="server" Text="核准" OnClick="BtSaveClick" CssClass="pass" />
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

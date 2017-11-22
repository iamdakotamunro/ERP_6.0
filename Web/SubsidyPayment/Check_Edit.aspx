<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Check_Edit.aspx.cs" Inherits="ERP.UI.Web.SubsidyPayment.Check_Edit" %>

<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!--当前页面：补贴审核——编辑-->
    <title></title>
    <script src="/JavaScript/jquery.js" type="text/javascript"></script>
    <link href="../Styles/bootstrap.min.css" rel="stylesheet" />
    <script src="../JavaScript/bootstrap.min.js" type="text/javascript"></script>

    <style type="text/css">
        label {
            font-weight: normal;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">

            <script type="text/javascript" src="../JavaScript/telerik.js"></script>

        </rad:RadScriptBlock>
        <div runat="server" id="DIV_AuditingPanel">
            <br />
            <div class="panel panel-default">
                <div class="panel-heading">费用补贴信息</div>
                <div class="panel-body">
                    
                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right; padding-left: 0px;">第三方订单编号：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <asp:TextBox ID="txt_ThirdPartyOrderNumber" runat="server" Enabled="false"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="text-align: right; padding-top: 5px;">订单编号：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <asp:TextBox ID="txt_OrderNumber" runat="server" Enabled="false"></asp:TextBox>
                        </div>
                    </div>
                    
                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right; padding-left: 0px;">第三方账户名：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <asp:TextBox ID="txt_ThirdPartyAccountName" runat="server" Enabled="false"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="text-align: right; padding-top: 5px;">销售平台：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <rad:RadComboBox ID="RCB_SalePlatform" runat="server" Enabled="false" UseEmbeddedScripts="false">
                            </rad:RadComboBox>
                        </div>
                    </div>

                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right;">订单金额：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <asp:TextBox ID="txt_RefundsAmount" runat="server" Enabled="false"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="text-align: right; padding-top: 5px;">销售公司：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" Enabled="false" UseEmbeddedScripts="false">
                            </rad:RadComboBox>
                        </div>
                    </div>
                    
                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right;">补贴金额：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <asp:TextBox ID="txt_SubsidyAmount" runat="server" Enabled="false"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right;">补贴类型：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <rad:RadComboBox ID="RCB_SubsidyType" runat="server" Enabled="false" UseEmbeddedScripts="false">
                            </rad:RadComboBox>
                        </div>
                    </div>
                    
                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right;">问题类型：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <rad:RadComboBox ID="RCB_QuestionType" runat="server" Enabled="false" UseEmbeddedScripts="false">
                            </rad:RadComboBox>
                        </div>
                    </div>
                </div>

            </div>


            <div class="panel panel-default">
                <div class="panel-heading">客户收款账户信息</div>
                <div class="panel-body">
                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right;">机构名称：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <asp:TextBox ID="txt_BankName" runat="server" Enabled="false"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right;">客户支付宝\银行账户：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <asp:TextBox ID="txt_BankAccountNo" runat="server" Enabled="false"></asp:TextBox>
                        </div>
                    </div>

                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right;">客户姓名：</label>
                        <div class="col-sm-8 control-label" style="text-align: left;">
                            <asp:TextBox ID="txt_UserName" runat="server" Enabled="false"></asp:TextBox>
                        </div>
                    </div>
                </div>

            </div>

            <div class="panel panel-default">
                <div class="panel-heading">审核操作</div>
                <div class="panel-body">
                    <div class="form-group col-sm-12 has-feedback">
                        <label class="col-sm-4 control-label" style="padding-top: 5px; text-align: right; width: 16%;">拒绝理由：</label>
                        <div class="col-sm-8 control-label" style="text-align: left; width: 77%;">
                            <asp:TextBox ID="txt_RejectReason" runat="server" TextMode="MultiLine" Width="100%" Height="80px"></asp:TextBox>
                        </div>
                    </div>
                </div>

            </div>

            <div class="popBtns">
                <div class="form-group">
                    <div class="col-sm-12">
                        <div class="text-center">
                            <asp:Button ID="BT_Back" runat="server" Text="核退" OnClick="BtnBackClick" ValidationGroup="save" CssClass="btn btn-danger" Style="margin-right: 20px;" />
                            <asp:Button ID="BT_Pass" runat="server" Text="核准" OnClick="BtnPassClick" CssClass="btn btn-success" Style="margin-right: 20px;" />
                            <asp:Button ID="BT_Cancl" runat="server" Text="关闭窗口" OnClientClick="return CancelWindow()" CssClass="btn btn-default" />
                        </div>
                    </div>
                </div>
            </div>

            <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" UseEmbeddedScripts="false">
                <AjaxSettings>
                    <rad:AjaxSetting AjaxControlID="BT_Back">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" />
                        </UpdatedControls>
                    </rad:AjaxSetting>
                    <rad:AjaxSetting AjaxControlID="BT_Pass">
                        <UpdatedControls>
                            <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" />
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

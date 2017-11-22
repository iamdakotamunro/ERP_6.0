<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="RefundsMoney.aspx.cs" Inherits="ERP.UI.Web.RefundsMoney.RefundsMoney" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <!--当前页面：退款打款-->

    <style>
        .highlight {
            padding: 10px 0px;
            margin-bottom: 14px;
            background-color: #f7f7f9;
            border: 1px solid #e1e1e8;
            border-radius: 4px;
        }

        .input[type="radio"], input[type="checkbox"] {
            margin: 0;
        }

        .w_auto {
            margin-right: 5px;
        }

        .search {
            padding-left: 30px;
        }

        input, select, textarea {
            max-width: 400px;
        }

        .detail {
            padding-left: 20px;
        }

        .search_Condition .search {
            text-align: left;
        }

        label {
            font-weight: normal;
        }

        html:first-child .RadWindow ul {
            border: solid 0px transparent;
        }
    </style>

    <div class="body-content">

        <div class="search_Condition box_1k">

            <div class="highlight">

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="padding-top: 5px; text-align: right;">订单号：</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">
                        <rad:RadTextBox ID="RTB_OrderNumber" runat="server" EmptyMessage="订单号、第三方订单号、售后订单号" Width="200"></rad:RadTextBox>
                    </div>
                </div>

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="text-align: right; padding-top: 5px;">时间：</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">

                        <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="100px">
                        </rad:RadDatePicker>
                        <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="100px">
                        </rad:RadDatePicker>
                    </div>
                </div>

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="text-align: right; padding-top: 5px;">状态：</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">

                        <asp:DropDownList ID="ddl_Status" runat="server" Style="width: 120px;">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="padding-top: 5px; text-align: right;">销售公司：</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">
                        <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" Width="120px"
                            Height="100px" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged"
                            AutoPostBack="True" EmptyMessage="请选择销售公司">
                        </rad:RadComboBox>
                    </div>
                </div>

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="text-align: right; padding-top: 5px;">销售平台：</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">
                        <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false" Width="120px" Height="100px" EmptyMessage="请选择销售平台">
                        </rad:RadComboBox>
                    </div>
                </div>

                <div class="search">
                    <asp:Button ID="btn_Search" CssClass="btn btn-success" runat="server" Text="查询" Style="margin-left: 40px;" OnClick="btn_Search_Click" />
                </div>
            </div>
        </div>
    </div>

    <rad:RadGrid ID="RG" runat="server" SkinID="CustomPaging" OnNeedDataSource="RG_NeedDataSource">
        <MasterTableView DataKeyNames="Id" ClientDataKeyNames="Id" NoMasterRecordsText="无可用记录。"
            CommandItemDisplay="None">
            <Columns>
                <rad:GridTemplateColumn HeaderText="申请时间">
                    <ItemTemplate>
                        &nbsp;<%# DateTime.Parse(Eval("CreateTime").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("CreateTime").ToString()).ToString("yyyy-MM-dd HH:mm:ss") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle HorizontalAlign="Center" Width="140px" />
                </rad:GridTemplateColumn>

                <rad:GridBoundColumn DataField="OrderNumber" HeaderText="订单号">
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle Width="140px" />
                </rad:GridBoundColumn>

                <rad:GridBoundColumn DataField="ThirdPartyOrderNumber" HeaderText="第三方订单号">
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle Width="140px" />
                </rad:GridBoundColumn>

                <rad:GridBoundColumn DataField="AfterSalesNumber" HeaderText="退换货号">
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle Width="140px" />
                </rad:GridBoundColumn>

                <rad:GridTemplateColumn HeaderText="退款金额(元)">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("RefundsAmount"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RefundsAmount")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn HeaderText="状态">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((ERP.Enum.RefundsMoney. RefundsMoneyStatusEnum)Eval("Status"))  %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>

                <rad:GridBoundColumn DataField="CreateUser" HeaderText="提交人">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>

                <rad:GridBoundColumn DataField="BankAccountNo" HeaderText="支付宝/银行账户">
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle Width="140px" />
                </rad:GridBoundColumn>

                <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Remark">
                    <ItemTemplate>
                        <asp:ImageButton ID="ImgBtn_Remark" runat="server" OnClientClick='<%# "RemarkForm(\"" + Eval("ID")+ "\",\"" + Eval("SalePlatformId")+ "\");return false;" %>'
                            SkinID="InsertImageButton" onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                            onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                        <div style="position: absolute;">
                            <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative; display: none; background-color: #CCFFFF; border: solid 1px #666; width: 250px; font-weight: bold; height: auto; overflow: visible; word-break: break-all;"
                                runat="server">
                                <%# Eval("Remark").ToString().Replace("\n","<br />")%>
                            </div>
                        </div>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn DataField="SalePlatformId" HeaderText="销售平台">
                    <ItemTemplate>
                        <asp:Label ID="Lab_SaleFilialeId" runat="server" Text='<%# GetSalePlatformName(Eval("SalePlatformId")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle HorizontalAlign="Center" Width="140px" />
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn HeaderText="操作" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton ID="LB_Process" runat="server" Text="受理" SkinID="AffirmImageButton"
                            OnClientClick='<%# "return ProcessClick(\"" + Eval("ID")+ "\",\"" + Eval("SalePlatformId")+ "\")" %>'></asp:ImageButton>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <asp:HiddenField ID="Hid_SelectedValue" runat="server" />
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager runat="server">
        <Windows>
            <rad:RadWindow ID="RW1" runat="server" Title="退款打款" Width="900" Height="540" />
            <rad:RadWindow ID="RW2" runat="server" Title="管理意见" Width="700" Height="500" />
        </Windows>
    </rad:RadWindowManager>

    <script src="/JavaScript/jquery.js"></script>
    <link href="/Styles/bootstrap.min.css" rel="stylesheet" />
    <script src="/JavaScript/bootstrap.min.js"></script>

    <script type="text/javascript">
        function ProcessClick(applyId, salePlatformId) {
            window.radopen("/RefundsMoney/RefundsMoney_Edit.aspx?ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW1");
        }

        function RemarkForm(applyId, salePlatformId) {
            window.radopen("/RefundsMoney/RefundsMoney_RemarkForm.aspx?ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW2");
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
        
        function ShowImg(obj) {
            var object = eval(obj);
            object.style.display = "block";
        }

        function HiddleImg(obj) {
            var object = eval(obj);
            object.style.display = "none";
        }
    </script>
</asp:Content>

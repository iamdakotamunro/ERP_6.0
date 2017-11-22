<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="Payment.aspx.cs" Inherits="ERP.UI.Web.SubsidyPayment.Payment" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum.SubsidyPayment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <!--当前页面：补贴审核-->

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
                    <label class="col-sm-3 control-label" style="padding-top: 5px; text-align: right;">单据号：</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">
                        <rad:RadTextBox ID="RTB_OrderNumber" runat="server" EmptyMessage="订单号、第三方订单号" Width="200"></rad:RadTextBox>
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
                    <label class="col-sm-3 control-label" style="text-align: right; padding-top: 5px;">补贴类型：</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">

                        <asp:DropDownList ID="ddl_SubsidyType" runat="server" Style="width: 120px;">
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
                    <asp:Button ID="btn_Accept" CssClass="btn btn-info" runat="server" Text="批量操作" Style="margin-left: 10px;" OnClick="btn_Accept_Click" />
                    <asp:Button ID="btn_ExportExcel" CssClass="btn btn-primary" runat="server" Text="导出Excel" Style="margin-left: 10px;" OnClick="btn_ExportExcel_Click" OnClientClick="return GiveTip(event,'您确定要导出Excel吗？')" />
                </div>
            </div>
        </div>
    </div>

    <rad:RadGrid ID="RG" runat="server" SkinID="CustomPaging" OnNeedDataSource="RG_NeedDataSource">
        <MasterTableView DataKeyNames="Id" ClientDataKeyNames="Id" NoMasterRecordsText="无可用记录。"
            CommandItemDisplay="None">
            <Columns>
                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox value=-1 onclick=checkAll()&gt;全选">
                    <ItemTemplate>
                        <input title="请选择" type="checkbox" name="ckId" value='<%# Eval("ID")%>' <%# (Eval("Status").Equals((int)SubsidyPaymentStatusEnum.PendingPayment))?"":"disabled"%> /><span>选择</span>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle Width="80px" HorizontalAlign="Center" VerticalAlign="Middle" />
                </rad:GridTemplateColumn>

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
                <rad:GridTemplateColumn HeaderText="补贴金额(元)">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("SubsidyAmount"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("SubsidyAmount")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn HeaderText="状态">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((SubsidyPaymentStatusEnum)Eval("Status"))  %>
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

                <rad:GridTemplateColumn HeaderText="补贴类型">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((SubsidyTypeEnum)Eval("SubsidyType"))  %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>

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

                <rad:GridTemplateColumn HeaderText="最后修改时间">
                    <ItemTemplate>
                        &nbsp;<%# DateTime.Parse(Eval("ModifyTime").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("ModifyTime").ToString()).ToString("yyyy-MM-dd HH:mm:ss") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle HorizontalAlign="Center" Width="140px" />
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn HeaderText="操作" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton ID="LB_Process" runat="server" Text="受理" SkinID="AffirmImageButton"
                            OnClientClick='<%# "return ProcessClick(\"" + Eval("Id")+ "\",\"" + Eval("SalePlatformId")+ "\")" %>'></asp:ImageButton>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <asp:HiddenField ID="Hid_ID" runat="server" />
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
            <rad:RadWindow ID="RW1" runat="server" Title="补贴打款" Width="900" Height="540" />
            <rad:RadWindow ID="RW2" runat="server" Title="管理意见" Width="700" Height="500" />
            <rad:RadWindow ID="RW3" runat="server" Title="批量操作" Width="700" Height="500" />
        </Windows>
    </rad:RadWindowManager>

    <div id="divContent" style="display: none; position: absolute; _position: absolute; z-index: 100; border: solid 1px #718CA1; background-color: #F1F1F1; width: 600px; font-size: 13px;">


        <div class="panel panel-default">
            <div class="panel-heading">批量受理</div>
            <div class="panel-body">

                <div class="form-group col-sm-6 has-feedback">
                    <label class="col-sm-4 control-label" style="text-align: right;">总笔数：</label>
                    <div class="col-sm-8 control-label" style="text-align: left;">
                        <asp:Label ID="lbl_Total" runat="server"></asp:Label>
                    </div>
                </div>

                <div class="form-group col-sm-6 has-feedback">
                    <label class="col-sm-6 control-label" style="text-align: right;">补贴总金额：</label>
                    <div class="col-sm-6 control-label" style="text-align: left;">
                        <asp:Label ID="lbl_SumSubsidyAmount" runat="server"></asp:Label>
                    </div>
                </div>

                <div class="form-group col-sm-12 has-feedback">
                    <label class="col-sm-4 control-label" style="padding-left: 0; text-align: right; width: 16%;">资金账户：</label>
                    <div class="col-sm-8 control-label" style="text-align: left; width: 77%;">
                        <rad:RadComboBox ID="RCB_AccountID" runat="server" UseEmbeddedScripts="false">
                        </rad:RadComboBox>
                    </div>
                </div>

                <div class="form-group col-sm-12 has-feedback" style="float: left; margin-top: 20px;">
                    <label class="col-sm-4 control-label" style="padding-left: 0; text-align: right; width: 16%;">拒绝理由：</label>
                    <div class="col-sm-8 control-label" style="text-align: left; width: 77%;">
                        <asp:TextBox ID="txt_RejectReason" runat="server" TextMode="MultiLine" Width="100%" Height="80px"></asp:TextBox>
                    </div>
                </div>


                <div class="form-group col-sm-12 has-feedback" style="float: left;">
                    <div style="text-align: center;">
                        <asp:Button ID="BT_Back" runat="server" Text="退回申请" OnClick="BtnBackClick" ValidationGroup="save" CssClass="btn btn-danger" Style="margin-right: 20px;" />
                        <asp:Button ID="BT_Pass" runat="server" Text="打款完成" OnClick="BtnPassClick" CssClass="btn btn-success" Style="margin-right: 20px;" />
                        <asp:Button ID="BT_Cancl" runat="server" Text="关闭窗口" OnClientClick="return moveHide()" CssClass="btn btn-default" />
                    </div>
                </div>

            </div>
        </div>
    </div>

    <script src="/JavaScript/jquery.js"></script>
    <link href="/Styles/bootstrap.min.css" rel="stylesheet" />
    <script src="/JavaScript/bootstrap.min.js"></script>

    <script type="text/javascript">
        $(function () {
            //new ToolTipMsg().bindToolTip("[tooltipmsg]");
            $("#divContent").css({ "top": $(document.body).height() / 2, "left": $(document.body).width() / 3 });
        });

        function ProcessClick(applyId, salePlatformId) {
            window.radopen("/SubsidyPayment/Payment_Edit.aspx?ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW1");
        }

        function RemarkForm(applyId, salePlatformId) {
            window.radopen("/SubsidyPayment/Check_RemarkForm.aspx?ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW2");
        }

        function BatchEdit(applyId) {
            return;
            window.radopen("/SubsidyPayment/Payment_BatchEdit.aspx?ApplyId=" + applyId, "RW3");
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


        //显示提示框
        function moveShow() {
            $("#divContent").show();
        }

        //隐藏提示框
        function moveHide() {
            $("#divContent").hide();
        }

        //全选
        function checkAll() {
            var a = $("input[type='checkbox'][name='ckId']:not(:disabled)");
            var n = a.length;
            for (var i = 0; i < n; i++) {
                a[i].checked = window.event.srcElement.checked;
            }
        }

        //显示选中值
        function ShowValue(value) {
            if (value.length === 0) {
                $("input[id$='btn_Pass']").hide();
                return;
            } else {
                $("input[id$='btn_Pass']").show();
            }
            var arr = value.split(',');
            $("input[type='checkbox'][name='ckId']").each(function () {
                if (arr.indexOf($(this).val().split('&')[0]) > -1) {
                    $(this).attr('checked', true);
                } else {
                    $(this).attr('checked', false);
                }
            });

            var checkItemLength = $("input[type='checkbox'][name='ckId']:not([value='-1'])").length;
            var checkedLength = $("input[type='checkbox'][name='ckId']:checked").length;
            if (checkItemLength === checkedLength) {
                $("input[value='-1'][name='ckId']").prop('checked', true);
            }
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
    </script>
</asp:Content>

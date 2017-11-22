<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="MemberMentionApply.aspx.cs" Inherits="ERP.UI.Web.MemberMentionApply" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">销售公司:
            </td>
            <td>
                <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" Width="120px"
                    Height="100px" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged"
                    AutoPostBack="True" EmptyMessage="请选择销售公司">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">销售平台:
            </td>
            <td>
                <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false" Width="120px" Height="100px" EmptyMessage="请选择销售平台">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">会员名称：
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="会员名称" ID="RTB_Member" Width="120px"
                    Enabled="False" ToolTip="会员名搜索需选择具体的销售平台" />
            </td>

            <td style="text-align: right;">时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
                <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">申请单号：
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="申请单号" ID="RdApplyNo" Width="120px" />
            </td>
            <td style="text-align: right;">账号：
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="账号" ID="RdBankAccounts" Width="120px" Enabled="False" />
            </td>
            <td style="text-align: right;">银行：
            </td>
            <td>
                <asp:DropDownList ID="DDL_BankAccount" runat="server">
                    <asp:ListItem Text="全部" Value="0" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="支付宝" Value="1"></asp:ListItem>
                    <asp:ListItem Text="工商银行" Value="2"></asp:ListItem>
                    <asp:ListItem Text="建设银行" Value="3"></asp:ListItem>
                    <asp:ListItem Text="农业银行" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">状态：
            </td>
            <td>
                <asp:DropDownList ID="ddl_MemberMentionState" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="8" style="text-align: center;">
                <asp:Button ID="btn_Search" runat="server" Text="查询" Style="margin-left: 40px;" OnClick="btn_Search_Click" />
                &nbsp;&nbsp;<asp:Button ID="btn_Batch" runat="server" Text="批量操作" OnClick="btn_Batch_Click" />
                &nbsp;&nbsp;<asp:Button ID="btn_ExportExcel" runat="server" Text="导出Excel" OnClick="btn_ExportExcel_Click" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_MemberMentionApply" runat="server" SkinID="CustomPaging" OnNeedDataSource="RG_MemberMentionApply_NeedDataSource">
        <MasterTableView DataKeyNames="Id,MemberId" ClientDataKeyNames="Id,MemberId" NoMasterRecordsText="无可用记录。"
            CommandItemDisplay="None">
            <Columns>
                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox value=-1 onclick=checkAll()&gt;全选">
                    <ItemTemplate>
                        <input title="请选择" type="checkbox" name="ckId" value='<%# Eval("Id")+"&"+Eval("SalePlatformId")+"$"+Eval("Amount")+"$"+Eval("SaleFilialeId")+"$"+Eval("UserName")%>' <%# Eval("State").Equals(0)?"":"disabled"%> /><span>选择</span>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="时间">
                    <ItemTemplate>
                        &nbsp;<%# DateTime.Parse(Eval("ApplyTime").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("ApplyTime").ToString()).ToString("yyyy-MM-dd HH:mm:ss") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="UserName" HeaderText="会员名">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="金额(元)">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("Amount"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="70px" />
                    <ItemStyle HorizontalAlign="Center" Width="70px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="账户余额(元)">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("Balance"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Balance")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="BankAccountName" HeaderText="银行名">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="BankName" HeaderText="账户名">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="帐号">
                    <ItemTemplate>
                        &nbsp;<%# Eval("BankAccounts") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle Width="150px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="状态">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((MemberMentionState)Eval("State"))  %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="销售公司">
                    <ItemTemplate>
                        <%#GetSaleFilialeName(Eval("SaleFilialeId"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="90px" />
                    <ItemStyle HorizontalAlign="Center" Width="90px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="销售平台">
                    <ItemTemplate>
                        <%#GetSalePlatformName(Eval("SalePlatformId"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="90px" />
                    <ItemStyle HorizontalAlign="Center" Width="90px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注" UniqueName="Description">
                    <ItemTemplate>
                        <%# Eval("Description").ToString() %>
                        <asp:ImageButton ID="ImageButton1" SkinID="EditImageButton" OnClientClick='<%# "return AddMemoClick(\"" + Eval("Id")+ "\",\"" + Eval("SalePlatformId")+ "\")" %>'
                            runat="server" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="200px" />
                    <ItemStyle HorizontalAlign="Center" Width="200px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="余额流水" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Execute" SkinID="InsertImageButton"
                            Text="余额流水" OnClientClick='<%# "return DeclineClick(\"" + Eval("Id")+ "\",\"" + Eval("MemberId")+ "\",\"" + Eval("SalePlatformId")+ "\")" %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton ID="LB_Process" runat="server" Text="受理" SkinID="AffirmImageButton"
                            OnClientClick='<%# "return ProcessClick(\"" + Eval("Id")+ "\",\"" + Eval("SalePlatformId")+ "\")" %>'
                            Visible='<%# int.Parse(Eval("State").ToString())==0 %>'></asp:ImageButton>
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
                    <rad:AjaxUpdatedControl ControlID="RG_MemberMentionApply" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTB_Member" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RdBankAccounts" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager runat="server">
        <Windows>
            <rad:RadWindow ID="RW1" runat="server" Title="受理" Width="700" Height="540" />
            <rad:RadWindow ID="RW2" runat="server" Title="批量受理" Width="400" Height="250" />
            <rad:RadWindow ID="RW3" runat="server" Title="余额流水" Width="985" Height="580" />
            <rad:RadWindow ID="RW4" runat="server" Title="添加备注" Width="420" Height="110" />
        </Windows>
    </rad:RadWindowManager>

    <script src="JavaScript/jquery.js"></script>
    <script type="text/javascript">
        function ProcessClick(applyId, salePlatformId) {
            window.radopen("./Windows/MemberMentionProcess.aspx?ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW1");
        }

        function DeclineClick(applyId, memberId, salePlatformId) {
            window.radopen("./Windows/MemberMentionAuditingDetail.aspx?Type=2&ApplyId=" + applyId + "&MemberId=" + memberId + "&SalePlatformId=" + salePlatformId, "RW3");
        }

        function AddMemoClick(applyId, salePlatformId) {
            window.radopen("./Windows/MemberMentionAddMemo.aspx?Type=2&ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW4");
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
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
            var arr = value.split(',');
            $("input[type='checkbox'][name='ckId']").each(function () {
                if (arr.indexOf($(this).val().split('$')[0]) > -1) {
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

        //批量操作
        function BatchOperate(paras, saleFilialeId, amountTotal, count) {
            window.radopen("./Windows/MemberMentionProcessBatch.aspx?Paras=" + paras + "&SaleFilialeId=" + saleFilialeId + "&AmountTotal=" + amountTotal + "&Count=" + count, "RW2");
        }
    </script>
</asp:Content>

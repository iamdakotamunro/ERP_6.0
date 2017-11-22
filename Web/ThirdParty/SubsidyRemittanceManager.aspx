<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SubsidyRemittanceManager.aspx.cs" Inherits="ERP.UI.Web.ThirdParty.SubsidyRemittanceManager" %>

<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum.ThirdParty" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">销售公司:
            </td>
            <td>
                <rad:RadComboBox ID="RcbSaleFiliale" runat="server" UseEmbeddedScripts="false" Width="120px"
                    Height="100px" OnSelectedIndexChanged="RcbSaleFilialeOnSelectedIndexChanged"
                    AutoPostBack="True" EmptyMessage="请选择销售公司">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">销售平台:
            </td>
            <td>
                <rad:RadComboBox ID="RcbSalePlatform" runat="server" UseEmbeddedScripts="false" Width="120px" Height="100px" EmptyMessage="请选择销售平台">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">会员名称：
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="会员名称" ID="RtbMember" Width="120px"
                    Enabled="False" ToolTip="会员名搜索需选择具体的销售平台" />
            </td>
            <td style="text-align: right;">银行：
            </td>
            <td>
                <asp:DropDownList ID="DdlBankAccount" runat="server">
                    <asp:ListItem Text="全部" Value="0" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="支付宝" Value="1"></asp:ListItem>
                    <asp:ListItem Text="工商银行" Value="2"></asp:ListItem>
                    <asp:ListItem Text="建设银行" Value="3"></asp:ListItem>
                    <asp:ListItem Text="农业银行" Value="4"></asp:ListItem>
                    <asp:ListItem Text="微信" Value="5"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
            </td>
            <td>
                -
            </td>
            <td>
                <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
            </td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td style="text-align: right;">订单单号：
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="订单单号" ID="RdApplyNo" Width="120px" />
            </td>
            <td style="text-align: right;">会员账号：
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="会员账号" ID="RdMemberAccountNo" Width="120px" Enabled="False" />
            </td>
            <td style="text-align: right;">补贴类型：
            </td>
            <td>
                <asp:DropDownList ID="DdlSubsidyType" runat="server">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">状态：
            </td>
            <td>
                <asp:DropDownList ID="DdlSubsidyState" runat="server">
                </asp:DropDownList>
            </td>
            <td colspan="5" style="text-align: center;">
                <asp:Button ID="BtnSearch" runat="server" Text="查询" Style="margin-left: 40px;" OnClick="BtnSearchClick" />
                &nbsp;&nbsp;<asp:Button ID="BtnBatch" runat="server" Text="批量操作" OnClick="BtnBatchClick" />
                &nbsp;&nbsp;<asp:Button ID="BtnExportExcel" runat="server" Text="导出Excel" OnClick="BtnExportExcelClick" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RgRemittance" runat="server" SkinID="CustomPaging" OnNeedDataSource="RgRemittanceNeedDataSource">
        <MasterTableView DataKeyNames="Id,MemberId" ClientDataKeyNames="Id,MemberId" NoMasterRecordsText="无可用记录。"
            CommandItemDisplay="None">
            <Columns>
                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox value=-1 onclick=checkAll()&gt;全选">
                    <ItemTemplate>
                        <input type="checkbox" name="ckId" value='<%# Eval("Id")+"&"+Eval("SalePlatformId")+"$"+Eval("Amount")+"$"+Eval("SaleFilialeId")+"$"+Eval("RealName")%>' <%# Eval("ApplyDate").Equals(2)?"":"disabled"%> /><span></span>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="时间">
                    <ItemTemplate>
                        &nbsp;<%# DateTime.Parse(Eval("ApplyDate").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("ApplyDate").ToString()).ToString("yyyy-MM-dd HH:mm:ss") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ThirdPartyOrderNo" HeaderText="订单号">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="MemberNo" HeaderText="会员名">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="补贴金额(元)">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("Amount"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="机构名称" HeaderText="银行名称">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="AccountsNo" HeaderText="账户名">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="补贴类型">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((SubsidyType)Eval("SubsidyType"))  %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="状态">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((SubsidyApplyState)Eval("ApplyState"))  %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注" UniqueName="Description">
                    <ItemTemplate>
                        <%# Eval("Description").ToString() %>
                        <asp:ImageButton ID="ImageButton1" SkinID="EditImageButton" OnClientClick='<%# "return AddMemoClick(\"" + Eval("Id")+ "\")" %>'
                            runat="server" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="200px" />
                    <ItemStyle HorizontalAlign="Center" Width="200px" />
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
                <rad:GridTemplateColumn HeaderText="操作" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton ID="LB_Process" runat="server" Text="打款" SkinID="AffirmImageButton"
                            OnClientClick='<%# "return ProcessClick(\"" + Eval("Id")+ "\")" %>'
                            Visible='<%# Convert.ToInt32(Eval("State"))==(int)SubsidyApplyState.WaitRemittance %>'></asp:ImageButton>
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
            <rad:AjaxSetting AjaxControlID="BtnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgSubsidy" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RcbSaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RcbSalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RtbMember" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager runat="server">
        <Windows>
            <rad:RadWindow ID="RwAudit" runat="server" Title="审核操作" Width="700" Height="540" />
            <rad:RadWindow ID="RwShow" runat="server" Title="补贴详情" Width="700" Height="540" />
            <rad:RadWindow ID="RwDescription" runat="server" Title="添加备注" Width="420" Height="110" />
        </Windows>
    </rad:RadWindowManager>

    <script src="JavaScript/jquery.js"></script>
    <script type="text/javascript">
        function RowDblClick(obj, args) {
            var id = args.getDataKeyValue("Id");
            window.radopen("./ThirdParty/SubsidyAuditForm.aspx?Type=0&Id=" + id, "RwShow");
            return false;
        }

        function ProcessClick(id) {
            window.radopen("./ThirdParty/SubsidyAuditForm.aspx?Type=1&Id=" + id, "RwAudit");
        }

        function AddMemoClick(id) {
            window.radopen("./ThirdParty/SubsidyAddMemo.aspx?Id=" + id, "RwDescription");
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='BtnSearch']").click();
        }

        //全选
        function checkAll() {
            var a = $("input[type='checkbox'][name='ckId']:not(:disabled)");
            var n = a.length;
            for (var i = 0; i < n; i++) {
                a[i].checked = window.event.srcElement.checked;
            }
        }
    </script>
</asp:Content>





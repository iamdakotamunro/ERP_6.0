<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="MemberMentionApply.aspx.cs" Inherits="ERP.UI.Web.MemberMentionApply" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">���۹�˾:
            </td>
            <td>
                <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" Width="120px"
                    Height="100px" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged"
                    AutoPostBack="True" EmptyMessage="��ѡ�����۹�˾">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">����ƽ̨:
            </td>
            <td>
                <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false" Width="120px" Height="100px" EmptyMessage="��ѡ������ƽ̨">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">��Ա���ƣ�
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="��Ա����" ID="RTB_Member" Width="120px"
                    Enabled="False" ToolTip="��Ա��������ѡ����������ƽ̨" />
            </td>

            <td style="text-align: right;">ʱ�䣺
            </td>
            <td>
                <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
                <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="95px">
                </rad:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">���뵥�ţ�
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="���뵥��" ID="RdApplyNo" Width="120px" />
            </td>
            <td style="text-align: right;">�˺ţ�
            </td>
            <td>
                <rad:RadTextBox runat="server" EmptyMessage="�˺�" ID="RdBankAccounts" Width="120px" Enabled="False" />
            </td>
            <td style="text-align: right;">���У�
            </td>
            <td>
                <asp:DropDownList ID="DDL_BankAccount" runat="server">
                    <asp:ListItem Text="ȫ��" Value="0" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="֧����" Value="1"></asp:ListItem>
                    <asp:ListItem Text="��������" Value="2"></asp:ListItem>
                    <asp:ListItem Text="��������" Value="3"></asp:ListItem>
                    <asp:ListItem Text="ũҵ����" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">״̬��
            </td>
            <td>
                <asp:DropDownList ID="ddl_MemberMentionState" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="8" style="text-align: center;">
                <asp:Button ID="btn_Search" runat="server" Text="��ѯ" Style="margin-left: 40px;" OnClick="btn_Search_Click" />
                &nbsp;&nbsp;<asp:Button ID="btn_Batch" runat="server" Text="��������" OnClick="btn_Batch_Click" />
                &nbsp;&nbsp;<asp:Button ID="btn_ExportExcel" runat="server" Text="����Excel" OnClick="btn_ExportExcel_Click" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_MemberMentionApply" runat="server" SkinID="CustomPaging" OnNeedDataSource="RG_MemberMentionApply_NeedDataSource">
        <MasterTableView DataKeyNames="Id,MemberId" ClientDataKeyNames="Id,MemberId" NoMasterRecordsText="�޿��ü�¼��"
            CommandItemDisplay="None">
            <Columns>
                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox value=-1 onclick=checkAll()&gt;ȫѡ">
                    <ItemTemplate>
                        <input title="��ѡ��" type="checkbox" name="ckId" value='<%# Eval("Id")+"&"+Eval("SalePlatformId")+"$"+Eval("Amount")+"$"+Eval("SaleFilialeId")+"$"+Eval("UserName")%>' <%# Eval("State").Equals(0)?"":"disabled"%> /><span>ѡ��</span>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="ʱ��">
                    <ItemTemplate>
                        &nbsp;<%# DateTime.Parse(Eval("ApplyTime").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("ApplyTime").ToString()).ToString("yyyy-MM-dd HH:mm:ss") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="UserName" HeaderText="��Ա��">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="���(Ԫ)">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("Amount"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="70px" />
                    <ItemStyle HorizontalAlign="Center" Width="70px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="�˻����(Ԫ)">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("Balance"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Balance")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="BankAccountName" HeaderText="������">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="BankName" HeaderText="�˻���">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="�ʺ�">
                    <ItemTemplate>
                        &nbsp;<%# Eval("BankAccounts") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle Width="150px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="״̬">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((MemberMentionState)Eval("State"))  %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="���۹�˾">
                    <ItemTemplate>
                        <%#GetSaleFilialeName(Eval("SaleFilialeId"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="90px" />
                    <ItemStyle HorizontalAlign="Center" Width="90px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="����ƽ̨">
                    <ItemTemplate>
                        <%#GetSalePlatformName(Eval("SalePlatformId"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="90px" />
                    <ItemStyle HorizontalAlign="Center" Width="90px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="��ע" UniqueName="Description">
                    <ItemTemplate>
                        <%# Eval("Description").ToString() %>
                        <asp:ImageButton ID="ImageButton1" SkinID="EditImageButton" OnClientClick='<%# "return AddMemoClick(\"" + Eval("Id")+ "\",\"" + Eval("SalePlatformId")+ "\")" %>'
                            runat="server" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="200px" />
                    <ItemStyle HorizontalAlign="Center" Width="200px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="�����ˮ" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Execute" SkinID="InsertImageButton"
                            Text="�����ˮ" OnClientClick='<%# "return DeclineClick(\"" + Eval("Id")+ "\",\"" + Eval("MemberId")+ "\",\"" + Eval("SalePlatformId")+ "\")" %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="����" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton ID="LB_Process" runat="server" Text="����" SkinID="AffirmImageButton"
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
            <rad:RadWindow ID="RW1" runat="server" Title="����" Width="700" Height="540" />
            <rad:RadWindow ID="RW2" runat="server" Title="��������" Width="400" Height="250" />
            <rad:RadWindow ID="RW3" runat="server" Title="�����ˮ" Width="985" Height="580" />
            <rad:RadWindow ID="RW4" runat="server" Title="��ӱ�ע" Width="420" Height="110" />
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

        //���°�Grid�¼�
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }

        //ȫѡ
        function checkAll() {
            var a = $("input[type='checkbox'][name='ckId']:not(:disabled)");
            var n = a.length;
            for (var i = 0; i < n; i++) {
                a[i].checked = window.event.srcElement.checked;
            }
        }

        //��ʾѡ��ֵ
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

        //��������
        function BatchOperate(paras, saleFilialeId, amountTotal, count) {
            window.radopen("./Windows/MemberMentionProcessBatch.aspx?Paras=" + paras + "&SaleFilialeId=" + saleFilialeId + "&AmountTotal=" + amountTotal + "&Count=" + count, "RW2");
        }
    </script>
</asp:Content>

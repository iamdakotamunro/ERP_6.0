<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="InvoiceOperation.aspx.cs" Inherits="ERP.UI.Web.InvoiceOperation" %>

<%@ Import Namespace="ERP.SAL" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <script src="My97DatePicker/WdatePicker.js"></script>
    <script src="JavaScript/ToolTipMsg.js"></script>
    <script src="JavaScript/GiveTip.js"></script>
    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">往来单位：</td>
            <td>
                <rad:RadComboBox ID="rcb_CompanyList" runat="server" AutoPostBack="true" Width="172px" Height="200px" AllowCustomText="true" EnableLoadOnDemand="true"
                    OnItemsRequested="rcb_CompanyList_OnItemsRequested">
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">公司：</td>
            <td>
                <asp:DropDownList ID="ddl_SaleFiliale" runat="server">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">申请日期：</td>
            <td>
                <asp:TextBox ID="txt_ApplyDateTimeStart" runat="server" Width="100px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',maxDate:$('input[id$=rdp_EndTime]').val()})"></asp:TextBox>
                至
                    <asp:TextBox ID="txt_ApplyDateTimeEnd" runat="server" Width="100px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',minDate:$('input[id$=rdp_StartTime]').val()})"></asp:TextBox>
            </td>
            <td style="text-align: right;">付款状态：</td>
            <td>
                <%--未付款：待审核(1)，已审核(3)，未通过(0)，已执行(4)； 已付款：完成打款(9)；--%>
                <asp:DropDownList ID="ddl_PayState" runat="server">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    <asp:ListItem Text="未付款" Value="0,1,3,4"></asp:ListItem>
                    <asp:ListItem Text="已付款" Value="9"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">发票类型：</td>
            <td>
                <asp:DropDownList ID="ddl_InvoiceType" runat="server" onchange="ChangeBillingUnit();">
                    <asp:ListItem Text="请选择" Value="" Selected="True"></asp:ListItem>
                    <asp:ListItem Text="普通发票" Value="1"></asp:ListItem>
                    <asp:ListItem Text="增值发票" Value="5"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">单号：</td>
            <td>
                <asp:TextBox ID="txt_ReceipNo" runat="server"> </asp:TextBox>
            </td>
            <td style="text-align: right;">发票号码：</td>
            <td>
                <asp:TextBox ID="txt_InvoiceNo" runat="server"> </asp:TextBox>
            </td>
            <td style="text-align: right;">发票操作时间：</td>
            <td>
                <asp:TextBox ID="txt_OperatingTimeStart" runat="server" Width="100px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',maxDate:$('input[id$=rdp_EndTime]').val()})"></asp:TextBox>
                至
                    <asp:TextBox ID="txt_OperatingTimeEnd" runat="server" Width="100px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',minDate:$('input[id$=rdp_StartTime]').val()})"></asp:TextBox>
            </td>
            <td style="text-align: right;">发票状态：</td>
            <td>
                <asp:DropDownList ID="ddl_InvoiceState" runat="server">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    <asp:ListItem Text="未提交" Value="0"></asp:ListItem>
                    <asp:ListItem Text="已提交" Value="1"></asp:ListItem>
                    <asp:ListItem Text="已接收" Value="2"></asp:ListItem>
                    <asp:ListItem Text="待认证" Value="3"></asp:ListItem>
                    <asp:ListItem Text="认证完成" Value="4"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">开票单位：</td>
            <td>
                <asp:TextBox ID="txt_BillingUnit" runat="server"> </asp:TextBox>
            </td>
        </tr>
        <tr>
            <td colspan="9" style="text-align: center;">
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
                <asp:Button ID="btn_BatchSubmit" runat="server" Text="批量提交" OnClick="btn_BatchSubmit_Click" OnClientClick="return GiveTip(event,'您确定批量提交吗？')" />
                <asp:Button ID="btn_BatchReceive" runat="server" Text="批量接收" OnClick="btn_BatchReceive_Click" OnClientClick="return GiveTip(event,'您确定批量接收吗？')" />
                <asp:Button ID="btn_BatchAuthenticate" runat="server" Text="批量待认证" OnClick="btn_BatchAuthenticate_Click" OnClientClick="return GiveTip(event,'您确定批量待认证吗？')" />
                <asp:Button ID="btn_BatchVerification" runat="server" Text="批量认证完成" OnClick="btn_BatchVerification_Click" OnClientClick="return GiveTip(event,'您确定批量认证完成吗？')" />
                <asp:Button ID="btn_ExportExcel" runat="server" Text="导出Excel" OnClick="btn_ExportExcel_Click" OnClientClick="return GiveTip(event,'您确定要导出Excel吗？')" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_ReceiptInvoice" runat="server" ShowFooter="true" SkinID="CustomPaging" OnNeedDataSource="RG_ReceiptInvoice_NeedDataSource" OnItemDataBound="RG_ReceiptInvoice_ItemDataBound">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox onclick=checkAll('ckId')&gt;全选">
                    <ItemTemplate>
                        <input title="请选择" type="checkbox" name="ckId" value='<%# Eval("InvoiceId")%>' />选择
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="单号" UniqueName="InvoiceNum">
                    <ItemTemplate>
                        &nbsp;<%#Eval("ReceiptNo")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="公司">
                    <ItemTemplate>
                        <%#GetFilialeName(Eval("FilialeId").ToString()) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="单据金额">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealityBalance"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申请人">
                    <ItemTemplate>
                        <%#new PersonnelSao().GetName(new Guid(Eval("ApplicantID").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="付款状态">
                    <ItemTemplate>
                        <%#"0,1,3,4".Contains(Eval("ReceiptStatus").ToString())?"未付款":(Eval("ReceiptStatus").Equals(9)?"已付款":"") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发票类型">
                    <ItemTemplate>
                        <%#GetInvoiceTypeName(Eval("InvoiceType").ToString())%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="BillingUnit" HeaderText="开票单位">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="150px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="BillingDate" HeaderText="开票日期" DataFormatString="{0:yyyy-MM-dd}">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="发票号码">
                    <ItemTemplate>
                        &nbsp;<%#Eval("InvoiceNo")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="InvoiceCode" HeaderText="发票代码">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="未税金额" UniqueName="NoTaxAmount">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("NoTaxAmount").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="税额" UniqueName="Tax">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("Tax").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="含税金额" UniqueName="TaxAmount">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(decimal.Parse(Eval("TaxAmount").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发票状态">
                    <ItemTemplate>
                        <%#EnumAttribute.GetKeyName((CompanyFundReceiptInvoiceState)Convert.ToInt32(Eval("InvoiceState")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="60%" tooltipmsg='<%#Eval("Memo").ToString().Replace("\n","<br/>")%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作日志">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="60%" tooltipmsg='<%#Eval("Remark")==null?"": Eval("Remark").ToString().Replace("\n","<br/>")%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发票操作">
                    <ItemTemplate>
                        <a href="javascript:void(0);" title="编辑" onclick="Edit(<%#"'"+Eval("InvoiceId")+"'"%>)" <%# Eval("InvoiceState").Equals((int)CompanyFundReceiptInvoiceState.Verification)?"style=\"display: none;\"":""%>>
                            <img src="App_Themes/Default/images/Edit.gif" />
                        </a>
                        <asp:ImageButton ID="imgbtn_Del" runat="server" SkinID="DeleteImageButton" ToolTip="删除" CommandArgument='<%# Eval("InvoiceId") %>' OnClientClick="return GiveTip(event,'您确定删除吗？')" OnClick="imgbtn_Del_Click" Visible='<%# Eval("InvoiceState").Equals((int)CompanyFundReceiptInvoiceState.UnSubmit)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发票提交">
                    <ItemTemplate>
                        <asp:ImageButton ID="imgbtn_Submit" runat="server" SkinID="AffirmImageButton" ToolTip="提交" CommandArgument='<%# Eval("InvoiceId") %>' OnClientClick="return GiveTip(event,'您确定提交吗？')" OnClick="imgbtn_Submit_Click" Visible='<%# Eval("InvoiceState").Equals((int)CompanyFundReceiptInvoiceState.UnSubmit)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发票接收">
                    <ItemTemplate>
                        <asp:ImageButton ID="imgbtn_Receive" runat="server" SkinID="AffirmImageButton" ToolTip="接收" CommandArgument='<%# Eval("InvoiceId") +","+Eval("InvoiceType")%>' OnClientClick="return GiveTip(event,'您确定接收吗？')" OnClick="imgbtn_Receive_Click" Visible='<%# Eval("InvoiceState").Equals((int)CompanyFundReceiptInvoiceState.Submit)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发票待认证">
                    <ItemTemplate>
                        <asp:ImageButton ID="imgbtn_Authenticate" runat="server" SkinID="AffirmImageButton" ToolTip="待认证" CommandArgument='<%# Eval("InvoiceId") %>' OnClientClick="return GiveTip(event,'您确定待认证吗？')" OnClick="imgbtn_Authenticate_Click" Visible='<%# Eval("InvoiceState").Equals((int)CompanyFundReceiptInvoiceState.Receive)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发票认证完成">
                    <ItemTemplate>
                        <asp:ImageButton ID="imgbtn_Verification" runat="server" SkinID="AffirmImageButton" ToolTip="认证完成" CommandArgument='<%# Eval("InvoiceId") %>' OnClientClick="return GiveTip(event,'您确定认证完成吗？')" OnClick="imgbtn_Verification_Click" Visible='<%# Eval("InvoiceState").Equals((int)CompanyFundReceiptInvoiceState.Authenticate)%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="编辑发票" Width="800px" Height="300px" />
        </Windows>
    </rad:RadWindowManager>
    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
        });

        //编辑
        function Edit(invoiceId) {
            window.radopen("../Windows/InvoiceEdit.aspx?InvoiceId=" + invoiceId, "raw");
        }

        //全选
        function checkAll(str) {
            var a = document.getElementsByName(str);
            var n = a.length;
            for (var i = 0; i < n; i++) {
                a[i].checked = window.event.srcElement.checked;
            }
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
    </script>
</asp:Content>

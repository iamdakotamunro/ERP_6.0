<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="DemandInvoice.aspx.cs" Inherits="ERP.UI.Web.DemandInvoice" %>

<%@ Import Namespace="ERP.SAL" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.BLL.Implement" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <script src="My97DatePicker/WdatePicker.js"></script>
    <script src="JavaScript/ToolTipMsg.js"></script>
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
        </tr>
        <tr>
            <td style="text-align: right;">单据号：</td>
            <td>
                <asp:TextBox ID="txt_ReceipNo" runat="server"> </asp:TextBox>
            </td>
            <td style="text-align: right;">单据状态：</td>
            <td>
                <asp:DropDownList ID="ddl_ReceiptStatus" runat="server">
                    <asp:ListItem Text="请选择" Value=""></asp:ListItem>
                    <asp:ListItem Text="已审核" Value="3"></asp:ListItem>
                    <asp:ListItem Text="已执行" Value="4"></asp:ListItem>
                    <asp:ListItem Text="完成打款" Value="9" Selected="True"></asp:ListItem>
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">发票：
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <asp:DropDownList ID="dll_InvoiceType" runat="server" onchange="ChangeBillingUnit();">
                                <asp:ListItem Text="未开票" Value="" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="普通发票" Value="1"></asp:ListItem>
                                <asp:ListItem Text="增值发票" Value="5"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td id="tdBillingUnit">开票单位：
                         <asp:TextBox ID="txtBillingUnit" runat="server" Width="100px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
        <tr>
            <td colspan="6" style="text-align: center;">
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_ReceiptInvoice" runat="server" OnNeedDataSource="RG_ReceiptInvoice_NeedDataSource">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridBoundColumn DataField="ReceiptNo" HeaderText="单号">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="公司">
                    <ItemTemplate>
                        <%#GetFilialeName(Eval("FilialeId").ToString()) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="往来单位">
                    <ItemTemplate>
                        <%# GetCompanyName(Eval("CompanyID").ToString()) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申请人">
                    <ItemTemplate>
                        <%#new PersonnelSao().GetName(new Guid(Eval("ApplicantID").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ApplyDateTime" HeaderText="申请日期" DataFormatString="{0:yyyy-MM-dd}">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="单据金额">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealityBalance"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="联系人">
                    <ItemTemplate>
                        <%# GetCompanyInfo(new Guid(Eval("CompanyID").ToString()), 1) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="手机">
                    <ItemTemplate>
                        <%# GetCompanyInfo(new Guid(Eval("CompanyID").ToString()), 2) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="70px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="单据状态">
                    <ItemTemplate>
                        <%#EnumAttribute.GetKeyName((CompanyFundReceiptState)Convert.ToInt32(Eval("ReceiptStatus")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="管理意见">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="70%" tooltipmsg='<%#Eval("Remark").ToString().Replace("\n","<br/>")%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="补充发票">
                    <ItemTemplate>
                        <input type="button" value="添加" onclick="Add(<%#"'"+Eval("ReceiptID")+"'"%>)" <%#ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID").ToLower().Equals(Eval("FilialeId").ToString())?"disabled=\"disabled\"":"" %> />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="补充发票" Width="900px" Height="500px" />
        </Windows>
    </rad:RadWindowManager>
    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
            ChangeBillingUnit();
        });

        //添加
        function Add(receiptId) {
            window.radopen("../Windows/CompanyFundPayReceiptAdd.aspx?ID="+receiptId+"&Type=1", "raw");
        }

        function ChangeBillingUnit() {
            var selectedValue = $("select[id$='dll_InvoiceType']").val();
            if (selectedValue === "") {
                $("#tdBillingUnit").hide();
            } else {
                $("#tdBillingUnit").show();
            }
        }
    </script>
</asp:Content>

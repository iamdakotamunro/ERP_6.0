<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="LonmeShop.AdminWeb.InvoiceAw" Title="" CodeBehind="Invoice.aspx.cs" %>
<%@ Import Namespace="Framework.Common" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript" language="javascript">
            function RowDblClick(obj, args) {
                window.radopen("./Windows/ShowInvoiceOrder.aspx?InvoiceId=" + args.getDataKeyValue("InvoiceId") + "&MemberId=" + args.getDataKeyValue("MemberId"), "");
            }
            function printDblClick(invoiceId) {
                getpage("./Windows/InvicePrint.aspx?gid=" + invoiceId, fun);
            }
            function fun(txt) {
                var html = document.body.innerHTML;
                document.body.innerHTML = txt;
                window.print();
                document.body.innerHTML = html;
            }

            function ShowEditForm(id) {
                if (id != "00000000-0000-0000-0000-000000000000")
                    window.radopen("./Windows/AddInvoice.aspx?InvoiceId=" + id, "RW1");
                return false;
            }
        </script>
        <script language="javascript" type="text/javascript">

            function AddFieldToHidden(value, fieldOrderIndex, hiddenClientId) {
                var hiddenValue = document.getElementById(hiddenClientId).value;
                var hiddenArray;
                hiddenArray = hiddenValue.split(",");
                hiddenArray[fieldOrderIndex] = value;
                document.getElementById(hiddenClientId).value = hiddenArray.join(",");
            }

            function SumPrice(quantityId, unitPriceId, sumPriceId) {
                var quantity = document.getElementById(quantityId).value;
                var unitPrice = document.getElementById(unitPriceId).value;
                document.getElementById(sumPriceId).value = Math.round(quantity * 100 * unitPrice * 100) / 10000;
                Total();
            }

            function Total() {
                var objTable = document.getElementById("RGGoods_ctl01");
                var aInput = objTable.getElementsByTagName("input");
                var totalPrice = 0;
                var totalNumber = 0;
                for (var j = 0; j < aInput.length; j += 4) {
                    totalNumber += Number(aInput[j].value);
                }
                totalNumber = Math.round(totalNumber, 2);
                for (var j = 2; j < aInput.length; j += 4) {
                    totalPrice += Number(aInput[j].value);
                }
                totalPrice = totalPrice.toFixed(2);
                Lab_TotalPrice.innerHTML = totalPrice;
                Lab_TotalNumber.innerHTML = totalNumber;
            }

            function ShowConfirm() {
                var conf = window.confirm('提示：是否确认执行？');
                if (!conf)
                    return false;
                return true;
            }

            

            function TextOnFocus(objId) {
                var hfSearch = document.getElementById("<%=hfSearch.ClientID %>");
                var textValue = objId.value;
                if (textValue == "订单号/发票号/开票地址") {
                    objId.value = "";
                    objId.style.color = "#000";
                }
            }
            function TextOnBlur(objId) {
                var hfSearch = document.getElementById("<%=hfSearch.ClientID %>");
                var textValue = objId.value;
                if (textValue == "") {
                    objId.value = "订单号/发票号/开票地址";
                    objId.style.color = "#CCC";
                    hfSearch.value = "";
                }
                else {
                    objId.style.color = "#000";
                    hfSearch.value = textValue;
                }
            }
            function AddInvoice() {
                alert("电子发票发票版已上线，当前功能已禁用");
                return false;
                window.radopen("./Windows/AddInvoice.aspx", "RW1");
            }

            function BatchAddInvoice() {
                alert("电子发票发票版已上线，当前功能已禁用");
                return false;
                window.radopen("./Windows/BatchAddInvoice.aspx", "RW2");
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
        </script>
    </rad:RadScriptBlock>
    <table class="StagePanel">
        <tr>
            <td class="AreaRowTitle">
                票据类型：
            </td>
            <td>
                <rad:RadComboBox ID="rcbInvoiceType" Width="120" AutoPostBack="False" CausesValidation="false"
                    runat="server"  >
                    <Items>
                        <rad:RadComboBoxItem Text="" Value="-1" />
                        <rad:RadComboBoxItem Text="正票" Value="0" />
                        <rad:RadComboBoxItem Text="废票" Value="1" />
                        <rad:RadComboBoxItem Text="退票" Value="2" />
                    </Items>
                </rad:RadComboBox>
            </td>
            <td class="AreaRowTitle">
                票据状态：
            </td>
            <td>
                <rad:RadComboBox ID="rcbInvoiceState" Width="120" AutoPostBack="False" CausesValidation="false"
                    runat="server" OnInit="RCBInvoiceState_Init">
                    <Items>
                        <rad:RadComboBoxItem Text="" Value="-1" />
                        <rad:RadComboBoxItem Text="申请发票" Value="1" />
                        <rad:RadComboBoxItem Text="已开发票" Value="2" />
                        <rad:RadComboBoxItem Text="取消发票" Value="3" />
                        <rad:RadComboBoxItem Text="作废申请" Value="4" />
                    </Items>
                </rad:RadComboBox>
            </td>
            <td class="AreaRowTitle">
                订单来源：
            </td>
            <td>
                <rad:RadComboBox ID="RCB_SaleFiliale" Width="120" runat="server" AutoPostBack="false"
                    CausesValidation="false" OnLoad="RCBFromSource_Load" SelectedValue='<%# SelectSaleFilialeId %>'>
                    <Items>
                        <rad:RadComboBoxItem Text="" Value="00000000-0000-0000-0000-000000000000" />
                    </Items>
                </rad:RadComboBox>
            </td>
            <td class="AreaRowTitle">
                授权仓库：
            </td>
            <td>
                <rad:RadComboBox ID="RCB_Warehouse" Width="120" runat="server" AutoPostBack="false"
                    CausesValidation="false" OnLoad="RCBWarehouse_Load" SelectedValue='<%# SelectWarehouse %>'>
                    <Items>
                        <rad:RadComboBoxItem Text="" Value="00000000-0000-0000-0000-000000000000" />
                    </Items>
                </rad:RadComboBox>
            </td>
            <td class="AreaRowTitle">
                时间段：
            </td>
            <td>
                <rad:RadDatePicker ID="RDP_StartTime" SkinID="Common" runat="server" Width="120"
                    SelectedDate='<%# StartTime==DateTime.MinValue ? (DateTime?)null : StartTime%>'>
                </rad:RadDatePicker>
            </td>
            <td class="AreaRowTitle">
                至：
            </td>
            <td style="text-align: left;" colspan="2">
                <rad:RadDatePicker ID="RDP_EndTime" SkinID="Common" runat="server" Width="120" SelectedDate='<%# EndTime==DateTime.MinValue ? (DateTime?)null : EndTime%>'>
                </rad:RadDatePicker>
            </td>
        </tr>
        <tr>
            <td colspan="2" class="AreaRowTitle">
                <asp:CheckBox runat="server" ID="CB_IsOrderComplete" Checked="<%#IsOrderComplete %>"
                    Text="事后申请发票" />
            </td>
            <td colspan="2" class="AreaRowTitle">
                <asp:CheckBox runat="server" ID="CB_IsNeedManual" Checked="<%#IsNeedManual %>" Text="需手动打发票" />
            </td>
            <td colspan="10">&nbsp;</td>
        </tr>
        <tr>
            <td class="AreaRowTitle" >
                作废申请人：
            </td>
            <td>
                <asp:TextBox ID="TB_CancelPersonel" Width="100px" runat="server" SkinID="StandardInput"
                    Text='<%# CancelPersonel %>'></asp:TextBox>
            </td>
            <td class="AreaRowTitle">
                订单号：
            </td>
            <td class="ShortArea">
                <asp:TextBox ID="TB_OrderNo" Width="200px" runat="server" SkinID="StandardInput" Text='<%# OrderNo %>'></asp:TextBox>
            </td>
            <td class="AreaRowTitle">
                发票抬头：
            </td>
            <td class="ShortArea">
                <asp:TextBox ID="TB_InvoiceName" Width="200px" runat="server" SkinID="StandardInput" Text='<%# InvoiceName %>'></asp:TextBox>
            </td>
            <td class="AreaRowTitle">
                发票号：
            </td>
            <td class="ShortArea">
                <asp:TextBox ID="TB_InvoiceNo" Width="200px" runat="server" SkinID="StandardInput" Text='<%# InvoiceNo %>'></asp:TextBox>
            </td>
            <td class="AreaRowTitle">
                发票地址：
            </td>
            <td class="ShortArea">
                <asp:TextBox ID="TB_Address" Width="200px" runat="server" SkinID="StandardInput" Text='<%# Address %>'></asp:TextBox>
            </td>
            <td class="AreaRowTitle">
                发票名称：
            </td>
            <td class="ShortArea">
                <asp:TextBox ID="TB_InvoiceContent" Width="200px" runat="server" SkinID="StandardInput" Text='<%# InvoiceContent %>'></asp:TextBox>
            </td>
            <td class="AreaRowTitle">
                 <asp:Button runat="server" ID="Btn_Search" Text="搜索" OnClick="OnClick_Search" />
            </td>
        </tr>
    </table>
    <asp:HiddenField ID="hfSearch" Value="" runat="server" />
    <rad:RadGrid ID="RGInvoice" runat="server" SkinID="CustomPaging" AllowPaging="true"
        PageSize="10" OnNeedDataSource="RGInvoice_NeedDataSource" OnItemCommand="RGInvoice_ItemCommand">
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>
        <MasterTableView DataKeyNames="InvoiceId,InvoiceCode,MemberId,InvoiceState" ClientDataKeyNames="InvoiceId,InvoiceCode,MemberId,InvoiceState">
            <CommandItemTemplate>
                <Ibt:ImageButtonControl ID="LB_Add" runat="server" CommandName="RebindGrid" SkinType="Insert"
                    Text="新建发票" OnClientClick="AddInvoice()"></Ibt:ImageButtonControl>
                    <Ibt:ImageButtonControl ID="ImageButtonControl1" runat="server" CommandName="RebindGrid" SkinType="Insert"
                    Text="批量新建发票" OnClientClick="BatchAddInvoice()"></Ibt:ImageButtonControl>
                <Ibt:ImageButtonControl ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid"
                    SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <Columns>
                <rad:GridBoundColumn DataField="InvoiceName" HeaderText="发票抬头" UniqueName="InvoiceName">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <%--<rad:GridBoundColumn DataField="Receiver" HeaderText="开票人姓名" UniqueName="Receiver">
                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
            </rad:GridBoundColumn>--%>
                <rad:GridBoundColumn DataField="InvoiceContent" HeaderText="品名SKU" UniqueName="InvoiceContent">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Address" HeaderText="发票寄送地址" UniqueName="Address">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="RequestTime" HeaderText="索要时间" UniqueName="RequestTime">
                    <ItemTemplate>
                        <%# Convert.ToDateTime(Eval("RequestTime")).ToString("yyyy-MM-dd")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="AcceptedTime" HeaderText="开票时间" UniqueName="AcceptedTime">
                    <ItemTemplate>
                        <%# Convert.ToDateTime(Eval("AcceptedTime")) == DateTime.MinValue ? "-" : Convert.ToDateTime(Eval("AcceptedTime")).ToString("yyyy-MM-dd")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="InvoiceCode" HeaderText="发票代码" UniqueName="InvoiceCode">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="InvoiceNo" HeaderText="发票号码" UniqueName="InvoiceNo">
                    <ItemTemplate>
                        <%# (long)Eval("InvoiceNo") == 0 ? "-" : Convert.ToInt64(Eval("InvoiceNo")).ToMendString(8)%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="InvoiceSum" HeaderText="发票总额" UniqueName="InvoiceSum">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("InvoiceSum"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="InvoiceState" HeaderText="发票状态" UniqueName="InvoiceState">
                    <ItemTemplate>
                        <%#  EnumAttribute.GetKeyName((InvoiceState)Eval("InvoiceState")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="IsNeedManual" HeaderText="需手动打发票" UniqueName="InvoiceState">
                    <ItemTemplate>
                        <%# (bool)Eval("IsNeedManual")?"是":"否" %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="NoteType" HeaderText="票据" UniqueName="NoteType">
                    <ItemTemplate>
                        <%# GetNoteType(Eval("InvoiceState"), Eval("NoteType"))%></ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="IsCommit" HeaderText="报送" UniqueName="IsCommit">
                    <ItemTemplate>
                        <%# Eval("IsCommit").ToString()=="True"?"√":"-"%></ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="订单来源" UniqueName="SaleFilialeId">
                    <ItemTemplate>
                        <%#FromSourceMessage((Guid) Eval("SaleFilialeId")) %></ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="打印" UniqueName="Print">
                    <ItemTemplate>
                        <img alt="" src="../App_Themes/default/Images/Print.gif" onclick="printDblClick('<%# Convert.ToString(Eval("InvoiceId"))%>')" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="受理" UniqueName="Approved">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Approved" SkinID="AffirmImageButton"
                            CommandName="Approved" Enabled='<%# (Convert.ToInt32(Eval("InvoiceState"))==1) %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="修改" UniqueName="Edited">
                    <ItemTemplate>
                            <asp:ImageButton ID="IbEidt" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                            OnClientClick='<%# "return ShowEditForm(\"" + Eval("InvoiceId") + "\");" %>' Visible='<%# !(bool)Eval("IsCommit")  %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="取消" UniqueName="Cancel">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Cancel" SkinID="CancelImageButton"
                            CommandName="Cancel" Enabled='<%# (Convert.ToInt32(Eval("InvoiceState"))==1) %>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="InvoiceState" HeaderText="操作" UniqueName="InvoiceState">
                    <ItemTemplate>
                        <Ibt:ImageButtonControl runat="server" OnClientClick='<%# "return ShowConfirm();"%>'
                            CausesValidation="false" ID="IB_ModifyOrder" CommandName="waste" Visible='<%# (Convert.ToInt32(Eval("InvoiceState"))==(int)(InvoiceState.WasteRequest)) && !(bool)Eval("IsCommit")  %>'
                            Text="允许作废" SkinID="InsertImageButton"></Ibt:ImageButtonControl>
                        <br />
                        <%--note 拒绝作废功能暂时不需要 2015-03-19 陈重文--%>
                     <%--   <Ibt:ImageButtonControl runat="server" CausesValidation="false" ID="ImageButtonControl1"
                            OnClientClick='<%# "return ShowConfirm();"%>' CommandName="return" Visible='<%# (Convert.ToInt32(Eval("InvoiceState"))==(int)(InvoiceState.WasteRequest))  %>'
                            Text="拒绝作废" SkinID="InsertImageButton" ></Ibt:ImageButtonControl>--%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="WMInvoiceOrder"  runat="server" Height="280px" Width="720px" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="RW1" runat="server" Title="申报" Width="550" Height="300" />
            <rad:RadWindow ID="RW2" runat="server" Title="批量申请" Width="760" Height="400" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RadGridAjaxRequest" >
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGInvoice" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGInvoice">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGInvoice" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGInvoice" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

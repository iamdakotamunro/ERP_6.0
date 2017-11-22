<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceStatisticsPage.aspx.cs" Inherits="ERP.UI.Web.InvoiceStatisticsPage" %>

<%@ Import Namespace="Framework.Common" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>发票汇总</title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="JavaScript/telerik.js" type="text/javascript"></script>
            <script src="JavaScript/common.js" type="text/javascript"></script>
            <script type="text/javascript">
                function ShowConfirm() {
                    var conf = window.confirm('提示：是否确认执行？');
                    if (!conf)
                        return false;
                    return true;
                }

            </script>
        </rad:RadScriptBlock>
        <!--搜索-->
        <table class="PanelArea" width="100%">
            <tr>
                <td style="width: 200px; text-align: right">公司：<rad:RadComboBox ID="RCB_FilialeList" DropDownWidth="160px" runat="server">
                </rad:RadComboBox>
                </td>
                <td style="width: 50px; text-align: right">时间段:
                </td>
                <td style="width: 80px; text-align: left">
                    <rad:RadDatePicker ID="RDP_StartTime" SkinID="Common" runat="server" Width="95px">
                        <Calendar UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False" ViewSelectorText="x">
                        </Calendar>
                        <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                        <DateInput DisplayDateFormat="yyyy/MM/dd" DateFormat="yyyy/MM/dd">
                        </DateInput>
                    </rad:RadDatePicker>
                </td>
                <td style="width: 20px; text-align: left">至
                </td>
                <td style="width: 80px; text-align: left">
                    <rad:RadDatePicker ID="RDP_EndTime" SkinID="Common" runat="server" Width="95px">
                        <Calendar ID="Calendar1" runat="server" UseRowHeadersAsSelectors="False" UseColumnHeadersAsSelectors="False"
                            ViewSelectorText="x">
                        </Calendar>
                        <DatePopupButton ImageUrl="" HoverImageUrl=""></DatePopupButton>
                        <DateInput ID="DateInput1" runat="server" DisplayDateFormat="yyyy/MM/dd" DateFormat="yyyy/MM/dd">
                        </DateInput>
                    </rad:RadDatePicker>
                </td>
                <td style="width: 60px; text-align: left">发票种类:
                </td>
                <td style="width: 90px; text-align: left">
                    <rad:RadComboBox ID="RcbKindType" runat="server" Width="80" AutoPostBack="True" DataTextField="Value" DataValueField="Key" OnSelectedIndexChanged="RcbKindTypeSelectedChanged">
                    </rad:RadComboBox>
                </td>
                <td style="width: 60px; text-align: left">发票类型:
                </td>
                <td style="width: 70px; text-align: left">
                    <rad:RadComboBox ID="RCB_NoteType" runat="server" Width="60" DataTextField="Value" DataValueField="Key">
                    </rad:RadComboBox>
                </td>
                <td style="width: 180px; text-align: left">发票号：<asp:TextBox Width="120" ID="tbx_keyword" onkeyup="this.value=this.value.replace(/-?\D/g,'')"
                    SkinID="AutoInput" runat="server"></asp:TextBox>
                </td>
                <td style="width: 180px; text-align: left">
                    <asp:CheckBox ID="ckb_CrossInvoicing" runat="server" />跨月开票数据
                </td>
                <td style="width: 85px; text-align: left">
                    <asp:ImageButton ID="img_search" SkinID="SearchButton" OnClick="Search_Click" runat="server"
                        ImageAlign="Left" AlternateText="fsf" />
                </td>
                <td style="width: 85px; text-align: left">
                    <asp:ImageButton ID="img_Export" SkinID="ExportData" runat="server" OnClick="img_Export_Click"
                        ImageAlign="Left" />
                </td>
                <td style="width: 130px; text-align: left">
                    <asp:Button ID="Button_InvoiceExport" runat="server" Text="导出需报送发票" OnClick="Button_InvoiceExport_Click"
                        Style="text-align: left" />
                </td>
                <td style="text-align: left">
                    <asp:Button ID="Button_InvoiceCommit" runat="server" Text="发票报送" OnClick="Button_InvoiceCommit_Click" />
                </td>
            </tr>
        </table>
        <table class="PanelArea" width="100%" runat="server" id="table_ele">
            <tr>
                <td>蓝票总数:
                <asp:Label ID="lb_lp" runat="server" Text="-"></asp:Label>
                </td>
                <td>蓝票金额:
                <asp:Label ID="lb_lpsum" runat="server" Text="-"></asp:Label>
                </td>
                <td>红票总数:
                <asp:Label ID="lb_hp" runat="server" Text="-"></asp:Label>
                </td>
                <td>红票金额:
                <asp:Label ID="lb_hpsum" runat="server" Text="-"></asp:Label>
                </td>
                <td>实际总金额:
                <asp:Label ID="lb_tolsum" runat="server" Text="-"></asp:Label>
                </td>
            </tr>
        </table>
        <table class="PanelArea" width="100%" runat="server" id="table_count">
            <tr>
                <td>正票总数:
                <asp:Label ID="lab_zp" runat="server" Text="-"></asp:Label>
                </td>
                <td>正票金额:
                <asp:Label ID="lab_zpsum" runat="server" Text="-"></asp:Label>
                </td>
                <td>废票总数:
                <asp:Label ID="lab_fp" runat="server" Text="-"></asp:Label>
                </td>
                <td>废票金额:
                <asp:Label ID="Label_fpje" runat="server" Text="-"></asp:Label>
                </td>
                <td>退票总数：
                <asp:Label ID="Label_tpzs" runat="server" Text="-"></asp:Label>
                </td>
                <td>退票金额：
                <asp:Label ID="Label_tpje" runat="server" Text="-"></asp:Label>
                </td>
                <td>实际总金额:
                <asp:Label ID="lab_tol" runat="server" Text="-"></asp:Label>
                </td>
            </tr>
        </table>
        <rad:RadGrid ID="rad_invoce" runat="server" OnNeedDataSource="Rad_invoce_NeedDataSource"
            OnItemCommand="Rad_invoce_ItemCommand" SkinID="CustomPaging" Skin="WebBlue">
            <MasterTableView DataKeyNames="InvoiceId,OrderId">
                <CommandItemTemplate>
                    <Ibt:ImageButtonControl ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search"
                        SkinType="Search" Text="检索不连号发票"></Ibt:ImageButtonControl>
                </CommandItemTemplate>
                <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                <Columns>
                    <rad:GridTemplateColumn DataField="PrintDate" HeaderText="开票时间" UniqueName="PrintDate">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Convert.ToDateTime(Eval("PrintDate")).ToString("yyyy-MM-dd") %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="InvoicePayerName" HeaderText="抬头" UniqueName="InvoicePayerName">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="InvoiceCode" HeaderText="发票代码" UniqueName="InvoiceCode">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("InvoiceCode") %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="InvoiceNo" HeaderText="发票号码" UniqueName="InvoiceNo">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Convert.ToInt64(Eval("InvoiceNo")).ToMendString(8) %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="RetreatInvoiceCode" HeaderText="退票原代码" UniqueName="RetreatInvoiceCode">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="Label1" runat="server" Text='<%# Eval("RetreatInvoiceCode") %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="RetreatInvoiceNo" HeaderText="退票原号码" UniqueName="RetreatInvoiceNo">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="Label2" runat="server" Text='<%# Convert.ToInt64(Eval("RetreatInvoiceNo")).ToMendString(8) %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="TotalMoney" HeaderText="金额" UniqueName="TotalMoney">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("TotalMoney") %>' ForeColor='<%#GetInvoiceMoneyForeColor(Eval("State"), Eval("NoteType")) %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="RateMoney" HeaderText="税额" UniqueName="RateMoney">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("RateMoney") %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="ActualMoney" HeaderText="价税合计" UniqueName="ActualMoney">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label runat="server" Text='<%# Eval("ActualMoney") %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="NoteTypeName" HeaderText="票据" UniqueName="NoteTypeName">
                        <ItemTemplate>
                            <%# Eval("NoteTypeName")%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="OrderNo" HeaderText="订单号" UniqueName="OrderNo">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="StateName" HeaderText="状态" UniqueName="StateName">
                        <ItemTemplate>
                            <%# Eval("StateName")%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="IsCommit" HeaderText="报送" UniqueName="IsCommit">
                        <ItemTemplate>
                            <%# Eval("IsCommit").ToString()=="True"?"已报送":"-"%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="State" HeaderText="操作" UniqueName="State">
                        <ItemTemplate>
                            <Ibt:ImageButtonControl runat="server" OnClientClick='<%# "return ShowConfirm();"%>'
                                CausesValidation="false" ID="IB_ModifyOrder" CommandName="waste" Visible='<%# (Convert.ToInt32(Eval("State"))==(int)(InvoiceState.WasteRequest)) && !(bool)Eval("IsCommit") && !IsElectron %>'
                                Text="允许作废" SkinID="InsertImageButton"></Ibt:ImageButtonControl>
                            <Ibt:ImageButtonControl runat="server" CausesValidation="false" ID="ImageButtonControl1"
                                OnClientClick='<%# "return ShowConfirm();"%>' CommandName="return" Visible='<%# (Convert.ToInt32(Eval("State"))==(int)(InvoiceState.WasteRequest)) && !IsElectron %>'
                                Text="拒绝作废" SkinID="InsertImageButton"></Ibt:ImageButtonControl>
                        </ItemTemplate>
                        <HeaderStyle Width="160" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <rad:RadWindowManager ID="WindowManager" runat="server">
            <Windows>
                <rad:RadWindow ID="printForm" runat="server" Width="750" Height="450">
                </rad:RadWindow>
            </Windows>
        </rad:RadWindowManager>
        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="rad_invoce">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="rad_invoce" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="img_search">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="rad_invoce" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="img_search" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="table_count" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="table_ele" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="Button_InvoiceExport">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="rad_invoce" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="Button_InvoiceExport" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="table_count" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="table_ele" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RAM">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="rad_invoce" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RAM">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Btn_Print" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="LB_Search">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="rad_invoce" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RcbKindType">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_NoteType" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="table_ele" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="table_count" LoadingPanelID="loading" />
                        <rad:AjaxUpdatedControl ControlID="rad_invoce" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

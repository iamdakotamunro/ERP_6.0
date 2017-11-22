<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.AccountsT" Title="无标题页" CodeBehind="AccountsT.aspx.cs" %>

<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content2" ContentPlaceHolderID="CPHStage" runat="Server">
    <style type="text/css">
        .displaydiv div div {
            display: none;
        }
    </style>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowInsertForm() {
                var tree = $find('<%= RTVBankAccounts.ClientID %>');
                var node = tree.get_selectedNode();
                var bankAccountsId;
                var filialeId;
                if (node != null) {
                    if (node && node.get_enabled()) {
                        if (node.get_category() == "BankAccount") {
                            bankAccountsId = node.get_value();
                            filialeId = node.get_parent().get_value();
                        } else {
                            filialeId = "";
                            bankAccountsId = "";
                        }
                    }
                }
                window.radopen("./Windows/TransferForm.aspx?BankAccountsId=" + bankAccountsId + "&FilialeId=" + filialeId, "RW2");
                return false;
            }

            function ShowAdjustAccountsForm(wasteBookType) {
                var tree = $find('<%= RTVBankAccounts.ClientID %>');
                var node = tree.get_selectedNode();
                var bankAccountsId;
                var filialeId;
                if (node != null) {
                    if (node && node.get_enabled()) {
                        if (node.get_category() == "BankAccount") {
                            bankAccountsId = node.get_value();
                            filialeId = node.get_parent().get_value();
                        } else {
                            filialeId = "";
                            bankAccountsId = "";
                        }
                    }
                }
                window.radopen("./Windows/AdjustAccountsForm.aspx?WasteBookType=" + wasteBookType + "&BankAccountsId=" + bankAccountsId + "&pagename=" + '<%= ERP.UI.Web.Common.WebControl.FileName %>' + "&FilialeId=" + filialeId, "RW1");
                return false;
            }

            function RowDblClick(obj, args) {
                var wasteBookId = args.getDataKeyValue("WasteBookId");
                var filialeId = args.getDataKeyValue("SaleFilialeId");
                var bankAccountsId = args.getDataKeyValue("BankAccountsId");
                var auditingState = args.getDataKeyValue("AuditingState");
                var wbType = args.getDataKeyValue("WasteBookType");
                var dateCreated = args.getDataKeyValue("DateCreated");
                var isTranfer = args.getDataKeyValue("IsTranfer");
                var canDo1 = "<%=GetPowerOperationPoint("Transfer")%>";
                var canDo2 = "<%=GetPowerOperationPoint("Increasing")%>";
                var canDo3 = "<%=GetPowerOperationPoint("Subtract")%>";
                if (auditingState == 1) {
                    window.radopen("./Windows/PrintWasteBook.aspx?WasteBookId=" + wasteBookId + "&FilialeId=" + filialeId + "&DateCreated=" + dateCreated, "RW2");
                }
                else if (auditingState == 0) {
                    if (wbType == 1 && canDo1 == "True" && isTranfer == "True") {
                        window.radopen("./Windows/TransferForm.aspx?WasteBookId=" + wasteBookId + "&FilialeId=" + filialeId + "&BankAccountsId=" + bankAccountsId + "&optype=edit", "RW3");
                    }
                    else if (wbType == 0 && canDo2 == "True") {
                        window.radopen("./Windows/AdjustAccountsForm.aspx?WasteBookId=" + wasteBookId + "&FilialeId=" + filialeId + "&BankAccountsId=" + bankAccountsId + "&optype=edit", "RW4");
                    }
                    else if (wbType == 1 && canDo3 == "True") {
                        window.radopen("./Windows/AdjustAccountsForm.aspx?WasteBookId=" + wasteBookId + "&FilialeId=" + filialeId + "&BankAccountsId=" + bankAccountsId + "&optype=edit", "RW4");
                    }
                }
                return false;
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
            function ShowCheckForm(wasteBookId) {
                window.radopen("./Windows/AddWasteBookCheckForm.aspx?WasteBookId=" + wasteBookId + "", "RW5");
                return false;
            }
            function ShowWaste(index, isShow) {
                var waste = document.getElementById("A_Waste" + index);
                if (isShow == 1) {
                    waste.style.display = "block";
                } else {
                    waste.style.display = "none";
                }
            }
        </script>
    </rad:RadScriptBlock>
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <span id="span_TotalMoney" runat="server" style="font-weight: bold"></span>
                <rad:RadTreeView ID="RTVBankAccounts" runat="server" useembeddedscripts="false" Height="500px"
                    Width="250px" autopostback="true" CausesValidation="false" OnNodeClick="RtvBankAccountsNodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <div>
                    <table>
                        <tr>
                            <td class="displaydiv">
                                <table id="search" runat="server">
                                    <tr>
                                        <td colspan="4">
                                            <table>
                                                <tr>
                                                    <td>年份：<asp:DropDownList runat="server" ID="DDL_Years" AutoPostBack="true" OnSelectedIndexChanged="DdlYearsSelectedIndexChanged">
                                                    </asp:DropDownList>
                                                    </td>
                                                    <td>
                                                        <rad:RadDatePicker ID="RDP_StartDate" runat="server" AutoPostBack="true" SkinID="Common"
                                                            Width="90px">
                                                        </rad:RadDatePicker>
                                                    </td>
                                                    <td>
                                                        <rad:RadDatePicker ID="RDP_EndDate" runat="server" AutoPostBack="true" SkinID="Common"
                                                            Width="90px">
                                                        </rad:RadDatePicker>
                                                    </td>
                                                </tr>
                                            </table>
                                        </td>
                                        <td>是否核对：
                                        </td>
                                        <td>
                                            <rad:RadComboBox ID="RCB_IsCheck" Width="90" runat="server" useembeddedscripts="false"
                                                ShowToggleImage="True">
                                                <Items>
                                                    <rad:RadComboBoxItem runat="server" Value="-1" Text="" />
                                                    <rad:RadComboBoxItem runat="server" Value="1" Text="已核对" />
                                                    <rad:RadComboBoxItem runat="server" Value="0" Text="未核对" />
                                                </Items>
                                            </rad:RadComboBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>搜索：<asp:TextBox ID="txtTradeCode" runat="server" Width="200"></asp:TextBox>
                                        </td>
                                        <td>&nbsp;&nbsp;&nbsp;单据类型:
                                        </td>
                                        <td>
                                            <rad:RadComboBox ID="RCB_ReceiptType" Width="50" runat="server" useembeddedscripts="false"
                                                ShowToggleImage="True">
                                                <Items>
                                                    <rad:RadComboBoxItem runat="server" Value="-1" Text="" />
                                                    <rad:RadComboBoxItem runat="server" Value="1" Text="收入" />
                                                    <rad:RadComboBoxItem runat="server" Value="0" Text="支出" />
                                                </Items>
                                            </rad:RadComboBox>
                                        </td>
                                        <td>&nbsp;&nbsp;&nbsp;金额范围：
                                            <asp:TextBox ID="txtMinIncome" runat="server" Width="50"></asp:TextBox>
                                            -
                                            <asp:TextBox ID="txtMaxIncome" runat="server" Width="50"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="LB_Search" runat="server" SkinID="SearchButton" OnClick="LbSearchClick" />
                                        </td>
                                        <td>&nbsp; &nbsp; &nbsp; &nbsp;
                                            <asp:LinkButton ID="lbUnverify" runat="server" CausesValidation="false" OnClick="LbUnverifyClick">>>未审核单据</asp:LinkButton>
                                            &nbsp; &nbsp; &nbsp; &nbsp;
                                        </td>
                                    </tr>
                                </table>
                            </td>
                            <td align="right">
                                <table>
                                    <tr>
                                        <td>
                                            <Ibt:ImageButtonControl ID="LBXLS" runat="server" OnClick="LbxlsClick" SkinType="ExportExcel"></Ibt:ImageButtonControl>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <rad:RadGrid ID="RadGridWasteBook" runat="server" SkinID="CustomPaging" OnNeedDataSource="RadGridWasteBook_NeedDataSource"
                    OnItemDataBound="RadGridWasteBookItemDataBound">
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="WasteBookId,AuditingState,BankAccountsId,TradeCode,WasteBookType,SaleFilialeId,DateCreated"
                        ClientDataKeyNames="WasteBookId,AuditingState,BankAccountsId,TradeCode,WasteBookType,SaleFilialeId,DateCreated,IsTranfer">
                        <CommandItemTemplate>
                            <Ibt:ImageButtonControl ID="LBReceivable" runat="server" OnClientClick="return ShowInsertForm()"
                                Enabled='<%# GetPowerOperationPoint("Transfer") %>' Visible='<%# !RadGridWasteBook.MasterTableView.IsItemInserted %>'
                                SkinType="Insert" Text="转帐"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LBAdd" runat="server" OnClientClick="return ShowAdjustAccountsForm('Increase')"
                                Enabled='<%# GetPowerOperationPoint("Increasing") %>' SkinType="Insert" Text="增加资金"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LBReduce" runat="server" OnClientClick="return ShowAdjustAccountsForm('Decrease')"
                                Enabled='<%# GetPowerOperationPoint("Subtract") %>' SkinType="Insert" Text="减少资金"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridTemplateColumn HeaderText="单据编号" UniqueName="TradeCode">
                                <ItemTemplate>
                                    <%# Eval("TradeCode").ToString().Trim() %>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="130px" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="DateCreated" HeaderText="创建日期" UniqueName="DateCreated">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="110px" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="备注说明" UniqueName="Description">
                                <ItemTemplate>
                                    &nbsp;<%#Eval("Description")%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Income" HeaderText="收入" UniqueName="AccountReceivable">
                                <ItemTemplate>
                                    <asp:Label ID="Lab_AccountPayment" runat="server" Text='<%# Convert.ToDouble(Eval("Income"))>0 ?ERP.UI.Web.Common.WebControl.NumberSeparator( Convert.ToDouble(Eval("Income"))) : "" %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="70px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Income" HeaderText="支出" UniqueName="AccountPayment">
                                <ItemTemplate>
                                    <asp:Label ID="Lab_AccountReceivable" runat="server" Text='<%#  Convert.ToDouble(Eval("Income"))<0 ? ERP.UI.Web.Common.WebControl.NumberSeparator((-Convert.ToDouble(Eval("Income")))) : "" %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="70px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="NonceBalance" HeaderText="当前余额" UniqueName="NonceBalance">
                                <ItemTemplate>
                                    <asp:HiddenField ID="HF_WasteBookId" Value='<%#Eval("WasteBookId") %>' runat="server" />
                                    <asp:Label ID="lbNonceBalance" runat="server" ToolTip="点击核对账目" Text='<%#ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("NonceBalance")) %>'
                                        onclick='<%# "return ShowCheckForm(\"" + Eval("WasteBookId") + "\");" %>' Style="float: left; width: 90px;"></asp:Label>
                                    <asp:Label ID="lbDiff" runat="server"></asp:Label>
                                    <%--<div id="div_Balance" onmouseover='<%# "ShowWaste(\"" +Container.ItemIndex + "\",1);" %>' onmouseout='<%# "ShowWaste(\"" + Container.ItemIndex + "\",0);" %>'>
                                    <asp:HiddenField ID="HF_WasteBookId" Value='<%#Eval("WasteBookId") %>' runat="server" />
                                    <asp:HiddenField ID="HF_NonceBalance" Value='<%#Eval("NonceBalance") %>' runat="server" />
                                    <a id='NonceBalance' style="float: left;width:90px" onclick='<%# "return ShowCheckForm(\"" + Eval("WasteBookId") + "\");" %>' href="javascript:void(0);"><%#(Eval("NonceBalance"))%></a>
                                    <div id='A_Waste<%#Container.ItemIndex %>' style="display: none;float: left;width:35px;"><asp:LinkButton ID='LinkButton1' runat="server" Text='核对' OnClientClick='<%# "return ShowCheckForm(\"" + Eval("WasteBookId") + "\");" %>'></asp:LinkButton></div>
                                </div>--%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="120px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="AuditingState" HeaderText="单据状态" UniqueName="AuditingState">
                                <ItemTemplate>
                                    <asp:Label ID="Lab_AuditingState" runat="server" Text='<%# Convert.ToInt32(Eval("auditingState"))<1 ? "未审核" : "已审核" %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="LinkTradeCode" HeaderText="来源编号" UniqueName="LinkTradeCode">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# Eval("LinkTradeCode") %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="130px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="LinkTradeType" HeaderText="来源类型" UniqueName="LinkTradeType">
                                <ItemTemplate>
                                    <asp:Label ID="Label2" runat="server" Text='<%# GetLinkTradeType(Eval("LinkTradeType")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="90px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadWindowManager ID="TransferWindowManager" runat="server" Height="240px" Width="400px"
        ReloadOnShow="true" onajaxrequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="RW1" Width="400" Height="200" runat="server" Title="增加资金" />
            <rad:RadWindow ID="RW2" Width="500" Height="400" runat="server" Title="审核资金记录单据" />
            <rad:RadWindow ID="RW3" Width="600" Height="480" runat="server" Title="转帐" />
            <rad:RadWindow ID="RW4" Width="600" Height="480" runat="server" Title="转帐" />
            <rad:RadWindow ID="RW5" Width="450" Height="300" runat="server" Title="增加资金核对" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RTVBankAccounts">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RTVBankAccounts" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RadGridWasteBook" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RDOWasteBook" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RDOReduceWasteBook" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RDOWasteBook">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridWasteBook" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RDOReduceWasteBook">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridWasteBook" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridWasteBook" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridWasteBook" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RadGridWasteBook">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridWasteBook" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="lbUnverify">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridWasteBook" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="lbUnverify" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_Years">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartDate" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RDP_EndDate" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RDP_StartDate">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_StartDate" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RDP_EndDate">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_EndDate" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

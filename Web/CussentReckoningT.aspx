<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    Inherits="ERP.UI.Web.CussentReckoningT" Title="无标题页" CodeBehind="CussentReckoningT.aspx.cs" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="System.Globalization" %>
<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <style type="text/css">
        .displaydiv div div {
            display: none;
        }
    </style>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">

            function RowDblClick(obj, args) {
                var auditingState = args.getDataKeyValue("AuditingState");
                var reckoningType = args.getDataKeyValue("ReckoningType");
                var reckoningId = args.getDataKeyValue("ReckoningId");
                var dateCreated = args.getDataKeyValue("DateCreated");
                //            var canDo1 = "<%=GetPowerOperationPoint("CollectionOrder")%>";
                //            var canDo2 = "<%=GetPowerOperationPoint("PaymentOrder")%>";
                var canDo3 = "<%=GetPowerOperationPoint("Increase")%>";
                var canDo4 = "<%=GetPowerOperationPoint("Receivable")%>";

                if (auditingState == 1) {
                    window.radopen("./Windows/PrintReckoning.aspx?ReckoningId=" + reckoningId + "&DateCreated=" + dateCreated, "RowDblClickPage");
                }
                else if (auditingState == 0) {
                    if (reckoningType == 0 && canDo3 == "True") {
                        window.radopen("./Windows/AdjustReckoningForm.aspx?ReckoningId=" + reckoningId + "&optype=edit", "ShowAdjustReckoningFormPage");
                    }
                    else if (reckoningType == 1 && canDo4 == "True") {
                        window.radopen("./Windows/AdjustReckoningForm.aspx?ReckoningId=" + reckoningId + "&optype=edit", "ShowAdjustReckoningFormPage");
                    }
                }
                return false;
            }
            function ShowCheckingForm() {
                var tree = $find('<%= RTVCompanyCussent.ClientID %>');
                var node = tree.get_selectedNode();
                var companyId;
                var filialeId = "00000000-0000-0000-0000-000000000000";
                if (node != null) {
                    if (node && node.get_enabled()) {
                        if (node.get_toolTip() == "Filiale") {
                            companyId = node.get_parent().get_value();
                            filialeId = node.get_value();
                        } else {
                            companyId = node.get_value();
                        }
                    }
                }
                if (companyId != "00000000-0000-0000-0000-000000000000") {
                    window.radopen("./Windows/ShowCheckingForm.aspx?CompanyId=" + companyId + "&FilialeId=" + filialeId, "ShowCheckingFormPage");
                }
                return false;
            }

            //填写收款单和付款单
            function ShowReckoningForm(reckoningType) {
                var tree = $find("<%= RTVCompanyCussent.ClientID %>");
                var node = tree.get_selectedNode();
                var companyId;
                var filialeId = "00000000-0000-0000-0000-000000000000";
                if (node != null) {
                    if (node && node.get_enabled()) {
                        if (node.get_toolTip() == "Filiale") {
                            companyId = node.get_parent().get_value();
                            filialeId = node.get_value();
                        } else {
                            companyId = node.get_value();
                        }
                    }
                }
                if (companyId != "00000000-0000-0000-0000-000000000000") {
                    window.radopen("./Windows/ReckoningForm.aspx?ReckoningType=" + reckoningType + "&CompanyId=" + companyId + "&FilialeId=" + filialeId, "ShowReckoningFormPage");
                }
                return false;
            }

            function ShowReckoningInfoForm(type, id) {
                var tree = $find("<%= RTVCompanyCussent.ClientID %>");
                var node = tree.get_selectedNode();
                var companyId = "00000000-0000-0000-0000-000000000000";
                var filialeId = "00000000-0000-0000-0000-000000000000";
                if (node != null) {
                    if (node && node.get_enabled()) {
                        if (node.get_toolTip() == "Filiale") {
                            companyId = node.get_parent().get_value();
                            filialeId = node.get_value();
                        } else {
                            companyId = node.get_value();
                        }
                    }
                }
                if (filialeId == "00000000-0000-0000-0000-000000000000") filialeId = id;
                if (companyId != "00000000-0000-0000-0000-000000000000") {
                    window.radopen("./Windows/ShowReckoning.aspx?CompanyId=" + companyId + "&Type=" + type + "&FilialeId=" + filialeId, "ShowReckoningInfoFormPage");
                }
                return false;
            }

            //应收增加应收减少
            function ShowAdjustReckoningForm(reckoningType) {
                var tree = $find("<%= RTVCompanyCussent.ClientID %>");
                var node = tree.get_selectedNode();
                if (node.get_toolTip() == "Company") {
                    return false;
                }
                if (node.get_toolTip() == "Filiale") {
                    node.get_parent().get_value();
                }
                var companyId = "00000000-0000-0000-0000-000000000000";
                var filialeId = "00000000-0000-0000-0000-000000000000";
                if (node != null) {
                    if (node && node.get_enabled()) {
                        if (node.get_toolTip() == "Filiale") {
                            companyId = node.get_parent().get_value();
                            filialeId = node.get_value();
                        } else {
                            companyId = node.get_value();
                        }
                    }
                }

                if (companyId != "00000000-0000-0000-0000-000000000000") {
                    window.radopen("./Windows/AdjustReckoningForm.aspx?ReckoningType=" + reckoningType + "&CompanyId=" + companyId + "&FilialeId=" + filialeId, "ShowAdjustReckoningFormPage");
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

            function ShowCheckForm(reckoningId) {
                window.radopen("./Windows/AddReckoningCheck.aspx?ReckoningId=" + reckoningId + "", "ShowReckoningCheck");
                return false;
            }

            function ShowWaste(index, isShow) {
                var waste = document.getElementById("A_Reckoning" + index);
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
                <span style="font-weight: bold">公司:</span>
                <rad:RadComboBox ID="RCB_FilialeList" runat="server" DropDownWidth="180px" AutoPostBack="True"
                    AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择公司"
                    CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="Rcb_FilialeListSelectedIndexChanged">
                </rad:RadComboBox>
                <br />
                <br />
                <rad:RadTreeView ID="RTVCompanyCussent" runat="server" Height="500px" Width="250px"
                    AutoPostBack="true" CausesValidation="True" OnNodeClick="RadTreeViewCompanyCussent_NodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <div align="left">
                    <table>
                        <tr>
                            <td class="displaydiv">
                                <table id="search" runat="server">
                                    <tr>
                                        <%-- <td>
                                            公司：
                                        </td>
                                        <td>
                                            <rad:RadComboBox ID="RCB_FilialeList" runat="server" DropDownWidth="180px" AutoPostBack="True"
                                                AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择公司"
                                                CausesValidation="false" Filter="StartsWith" OnSelectedIndexChanged="Rcb_FilialeListSelectedIndexChanged">
                                            </rad:RadComboBox>
                                        </td>--%>
                                        <td>
                                            <rad:RadDatePicker ID="RDP_StartDate" runat="server" SkinID="Common" Width="90px">
                                            </rad:RadDatePicker>
                                        </td>
                                        <td>
                                            <rad:RadDatePicker ID="RDP_EndDate" runat="server" SkinID="Common" Width="90px">
                                            </rad:RadDatePicker>
                                        </td>
                                        <td>&nbsp;单据类型:
                                        </td>
                                        <td>
                                            <rad:RadComboBox ID="RCB_ReceiptType" runat="server" ShowToggleImage="True" Width="50">
                                                <Items>
                                                    <rad:RadComboBoxItem runat="server" Value="-1" Text="" />
                                                    <rad:RadComboBoxItem runat="server" Value="1" Text="收入" />
                                                    <rad:RadComboBoxItem runat="server" Value="0" Text="支出" />
                                                </Items>
                                            </rad:RadComboBox>
                                        </td>
                                        <td>&nbsp;是否对账:
                                        </td>
                                        <td>
                                            <rad:RadComboBox ID="rcbIsChecked" runat="server" ShowToggleImage="True" Width="70">
                                                <Items>
                                                    <rad:RadComboBoxItem ID="RadComboBoxItem1" runat="server" Value="-1" Text="" />
                                                    <rad:RadComboBoxItem ID="RadComboBoxItem3" runat="server" Value="0" Text="没有对账" />
                                                    <rad:RadComboBoxItem ID="RadComboBoxItem2" runat="server" Value="1" Text="成功对账" />
                                                    <rad:RadComboBoxItem ID="RadComboBoxItem4" runat="server" Value="2" Text="异常对账" />
                                                </Items>
                                            </rad:RadComboBox>
                                        </td>
                                        <td>&nbsp;对账类型:
                                        </td>
                                        <td>
                                            <rad:RadComboBox ID="Rcb_ReckoningCheckType" runat="server" ShowToggleImage="True"
                                                Width="70">
                                                <Items>
                                                    <rad:RadComboBoxItem ID="RadComboBoxItem5" runat="server" Value="-1" Text="" />
                                                    <rad:RadComboBoxItem ID="RadComboBoxItem6" runat="server" Value="1" Text="快递运费账" />
                                                    <rad:RadComboBoxItem ID="RadComboBoxItem7" runat="server" Value="2" Text="快递代收账" />
                                                    <rad:RadComboBoxItem ID="RadComboBoxItem8" runat="server" Value="3" Text="其它" />
                                                </Items>
                                            </rad:RadComboBox>
                                        </td>
                                        <td>&nbsp; 所属仓库:
                                        </td>
                                        <td>
                                            <rad:RadComboBox ID="RCB_Warehouse" runat="server" Width="90" DataTextField="Value"
                                                DataValueField="Key">
                                            </rad:RadComboBox>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <table>
                                    <tr>
                                        <td>搜索：
                                        </td>
                                        <td><asp:TextBox ID="txtTradeCode" runat="server" Width="200px"></asp:TextBox></td>
                                        <td>&nbsp;按金额:
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TB_MinMoney" runat="server" Width="100px"></asp:TextBox>&nbsp;到&nbsp;
                                            <asp:TextBox ID="TB_MaxMoney" runat="server" Width="100px"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:CheckBox ID="CB_IsOut" runat="server" Text="是否ERP" />
                                        </td>
                                        <td>
                                            <asp:ImageButton ID="LB_Search" runat="server" SkinID="SearchButton" OnClick="LbSearchClick" />
                                        </td>
                                        <td>&nbsp;
                                            <asp:LinkButton ID="lbUnverify" runat="server" CausesValidation="false" OnClick="LbUnverifyClick">>>未审核单据</asp:LinkButton>
                                            &nbsp;
                                        </td>
                                        <td>
                                            <Ibt:ImageButtonControl ID="imgBtn" runat="server" OnClick="imgBtn_ExportExcel_Click" SkinType="ExportExcel"
                                                Text="导出EXCEL"></Ibt:ImageButtonControl>
                                        </td>
                                    </tr>
                                </table>
                            </td>
                        </tr>
                    </table>
                </div>
                <div>
                    <asp:Label ID="LB_CompanyCussentInfo" runat="server" Text="" ForeColor="Red" Font-Bold="true"></asp:Label>
                </div>

                <rad:RadGrid ID="RGReckoning" runat="server" SkinID="CustomPaging" OnNeedDataSource="RgReckoningNeedDataSource"
                    OnItemDataBound="RadGridReckoningItemDataBound" OnPageIndexChanged="RGReckoning_PageIndexChanged">
                    <ClientSettings>
                        <ClientEvents OnRowDblClick="RowDblClick" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="ReckoningId,ReckoningType,DateCreated,AuditingState,State"
                        ClientDataKeyNames="ReckoningId,ReckoningType,DateCreated,AuditingState,State" CurrentResetPageIndexAction="SetPageIndexToLast">
                        <CommandItemTemplate>
                            <Ibt:ImageButtonControl ID="LinkButton1" runat="server" OnClientClick="return ShowCheckingForm()"
                                Enabled='<%# GetPowerOperationPoint("Reconciliation") %>' Visible='<%# !RGReckoning.MasterTableView.IsItemInserted %>'
                                SkinType="Insert" Text="预对账"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LBAddBankroll" runat="server" Enabled='<%# GetPowerOperationPoint("Increase") %>'
                                OnClientClick="return ShowAdjustReckoningForm('Income')" Visible='<%# !RGReckoning.MasterTableView.IsItemInserted %>'
                                SkinType="Insert" Text="应收增加"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LBDecreaseBankroll" runat="server" Enabled='<%# GetPowerOperationPoint("Receivable") %>'
                                OnClientClick="return ShowAdjustReckoningForm('Defray')" Visible='<%# !RGReckoning.MasterTableView.IsItemInserted %>'
                                SkinType="Insert" Text="应收减少"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <%--<Ibt:ImageButtonControl ID="LBPayment" runat="server" Enabled='<%# GetPowerOperationPoint("PaymentOrder") %>'
                                OnClientClick="return ShowReckoningForm('AccountPayment')" Visible='<%# !RGReckoning.MasterTableView.IsItemInserted %>'
                                SkinType="Insert" Text="填写付款单"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LBGathering" runat="server" Enabled='<%# GetPowerOperationPoint("CollectionOrder") %>'
                                OnClientClick="return ShowReckoningForm('AccountGathering')" Visible='<%# !RGReckoning.MasterTableView.IsItemInserted %>'
                                SkinType="Insert" Text="填写收款单"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;--%>
                            <Ibt:ImageButtonControl ID="LBInfo" runat="server" OnClientClick="<%# ShowReckoningForm(1) %>"
                                SkinType="Insert" Text="差额说明"></Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="ImageButtonControl1" runat="server" OnClientClick="<%# ShowReckoningForm(2) %>"
                                SkinType="Insert" Text="折扣说明"></Ibt:ImageButtonControl>
                            <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="TradeCode" HeaderText="单据编号" UniqueName="TradeCode">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="130px" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="往来单位">
                                <ItemTemplate>
                                    <%#Eval("CompanyName")%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="160px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="公司">
                                <ItemTemplate>
                                    <%#Eval("FilialeName")%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="60px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="原始编号">
                                <ItemTemplate>
                                    &nbsp;<%#Eval("LinkTradeCode")%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="130px" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="DateCreated" HeaderText="创建日期" UniqueName="DateCreated">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="110px" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Description" HeaderText="备注说明" UniqueName="Description">
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="AccountReceivable" HeaderText="增加" UniqueName="AccountReceivable">
                                <ItemTemplate>
                                    <asp:Label ID="AccountPaymentLabel" runat="server" Text='<%# (ReckoningType)Eval("ReckoningType")==ReckoningType.Income ? ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(Convert.ToDecimal(Eval("AccountReceivable").ToString()))).ToString(CultureInfo.InvariantCulture) : " " %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="AccountReceivable" HeaderText="减少" UniqueName="AccountPaymentLabel">
                                <ItemTemplate>
                                    <asp:Label ID="AccountReceivableLabel" runat="server" Text='<%# (ReckoningType)Eval("ReckoningType")==ReckoningType.Defray ? ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(Convert.ToDecimal(Eval("AccountReceivable").ToString()))).ToString(CultureInfo.InvariantCulture) : " " %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="合计" UniqueName="NonceTotalled">
                                <ItemTemplate>
                                    <asp:HiddenField ID="HF_ReckoningId" Value='<%#Eval("ReckoningId") %>' runat="server" />
                                    <asp:HiddenField ID="HF_Memo" Value='<%#Eval("Memo") %>' runat="server" />
                                    <asp:Label ID="lbNonceBalance" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("NonceTotalled").ToString())) %>'
                                        Style="float: left; width: 90px;"></asp:Label>
                                    <asp:Label ID="lbDiff" runat="server"></asp:Label>
                                    <%--<div id="div_Balance" onmouseover='<%# "ShowWaste(\"" +Container.ItemIndex + "\",1);" %>' onmouseout='<%# "ShowWaste(\"" + Container.ItemIndex + "\",0);" %>'>
                                        <asp:HiddenField ID="HF_ReckoningId" Value='<%#Eval("ReckoningId") %>' runat="server" />
                                        <asp:LinkButton ID="HL_NonceBalance" runat="server" style="float: left;width:90px;" Text='<%#Eval("NonceTotalled") %>'
                                        OnClientClick='<%# "return ShowCheckForm(\"" + Eval("ReckoningId") + "\");" %>' ToolTip="点击核对账目"></asp:LinkButton>
                                        <a id='A_Reckoning<%#Container.ItemIndex %>' style="display: none;float: left;width:30px" onclick='<%# "return ShowCheckForm(\"" + Eval("ReckoningId") + "\");" %>'  href="javascript:void(0);">核对</a>
                                    </div>--%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="100px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="ComCurrBalance" HeaderText="总合计" UniqueName="ComCurrBalance">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("ComCurrBalance").ToString()))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="120px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="ReckoningCheckType" HeaderText="对账类型" UniqueName="ReckoningCheckType">
                                <ItemTemplate>
                                    <asp:Label ID="LbCheckedType" runat="server" Text='<%# GetCheckType(Eval("ReckoningCheckType")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="IsChecked" HeaderText="是否对账" UniqueName="IsChecked">
                                <ItemTemplate>
                                    <asp:Label CssClass='<%=GetCss(2) %>' ID="IsCheckedLabel" runat="server" Text='<%# SetCheckingState(Convert.ToInt32(Eval("IsChecked").ToString())) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="AuditingState" HeaderText="单据状态" UniqueName="AuditingState">
                                <ItemTemplate>
                                    <asp:Label ID="Lab_AuditingState" runat="server" Text='<%# Convert.ToInt32(Eval("AuditingState"))<1 ? "未审核" : "已审核" %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="CurrentTotalled" HeaderText="余额存档" UniqueName="CurrentTotalled">
                                <ItemTemplate>
                                    <asp:Label ID="Label1" runat="server" Text='<%# ShowCurrentNotolled(Eval("CurrentTotalled"),Eval("NonceTotalled"),Eval("IsChecked")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>

    <rad:RadWindowManager ID="WMReckoning" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="ShowCheckingFormPage" Width="400" Height="250" runat="server"
                Title="对账" />
            <rad:RadWindow ID="ShowReckoningFormPage" Width="400" Height="400" runat="server"
                Title="付款单" />
            <rad:RadWindow ID="ShowReckoningInfoFormPage" Width="700" Height="500" runat="server"
                Title="往来信息" />
            <rad:RadWindow ID="ShowAdjustReckoningFormPage" Width="400" Height="240" runat="server"
                Title="应收增加" />
            <rad:RadWindow ID="RowDblClickPage" Width="400" Height="320" runat="server" Title="审核单位往来帐单据" />
            <rad:RadWindow ID="ShowReckoningCheck" runat="server" Width="500" Height="300" Title="往来账核对" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RTVCompanyCussent">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <%--  <rad:AjaxUpdatedControl ControlID="search"></rad:AjaxUpdatedControl>--%>
                    <rad:AjaxUpdatedControl ControlID="LB_CompanyCussentInfo"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="txtTradeCode"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGReckoning">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="lbUnverify">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="lbUnverify" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_FilialeList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RTVCompanyCussent" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RGReckoning" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="CB_IsOut" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="DebitNoteManage.aspx.cs" Inherits="ERP.UI.Web.DebitNoteManage" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
        <script src="JavaScript/common.js" type="text/javascript"></script>
        <script src="My97DatePicker/WdatePicker.js"></script>
        <script type="text/javascript" language="javascript">
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
            function clientShow(sender, eventArgs) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }
            function RowClick(obj, args) {
                obj.set_selectedItemsInternal = true;
            }
            function OnLogoutConfirm() {
                var conf = window.confirm('提示：是否确认注销吗？');
                if (!conf)
                    return false;
                return true;
            }
            function OnDeleteConfirm() {
                var conf = window.confirm('提示：是否确认删除吗？');
                if (!conf)
                    return false;
                return true;
            }
            function ShowImg(obj) {
                var object = eval(obj);
                object.style.display = "block";
            }
            function HiddleImg(obj) {
                var object = eval(obj);
                object.style.display = "none";
            }
            function Show(purchasingId) {
                window.radopen("./Windows/EditDebitNoteForm.aspx?PurchasingId=" + purchasingId + "", "EditDebitNoteForm");
            }

            //手动添加赠品借记单
            function AddDebitNote() {
                window.radopen("./Windows/DebitNoteAddForm.aspx", "RM1");
                return false;
            }

            //添加赠品借记单备注
            function AddMemoClick(PurchasingId) {
                window.radopen("./Windows/DebitNoteAddMemoForm.aspx?PurchasingId=" + PurchasingId, "RM2");
            }

            //查看赠品借记单备注
            function ReadMemoClick(PurchasingId) {
                window.radopen("./Windows/DebitNoteAddMemoForm.aspx?PurchasingId=" + PurchasingId + "&Read=read", "RM2");
            }

            //是否确认完成
            function Showconfirm() {
                var conf = window.confirm('系统提示：确认此单据已完成？');
                if (!conf)
                    return false;
                return true;
            }

            //是否确定核销
            function ChargeOffShowconfirm() {
                var conf = window.confirm('系统提示：确认核销此单据？');
                if (!conf)
                    return false;
                return true;
            }

        </script>
    </rad:RadScriptBlock>
    <div class="StagePanel" width="100%">
        <table class="StagePanelHead" border="0" style="width: 100%;">
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table class="PanelArea" height="38">
            <tr>
                <td style="text-align: right;">创建时间：
                </td>
                <td>
                    <asp:TextBox ID="txt_StartDate" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',maxDate:$('input[id$=txt_EndDate]').val()})"></asp:TextBox>
                    至
                    <asp:TextBox ID="txt_EndDate" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',minDate:$('input[id$=txt_StartDate]').val()})"></asp:TextBox>
                </td>
                <td style="text-align: right;">借记单状态：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_State" runat="server" UseEmbeddedScripts="false" MarkFirstMatch="True"
                        Width="100px" Height="120px" DataValueField="Key" DataTextField="Value" AppendDataBoundItems="true">
                    </rad:RadComboBox>
                </td>
                <td style="text-align: right;">仓库：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" UseEmbeddedScripts="false" Width="155px"
                        Height="200px" MarkFirstMatch="True" ShowToggleImage="True" AppendDataBoundItems="true"
                        DataTextField="Value" DataValueField="Key">
                        <Items>
                            <rad:RadComboBoxItem Value="00000000-0000-0000-0000-000000000000" Text="所有仓库" Selected="True" />
                        </Items>
                    </rad:RadComboBox>
                </td>
                <td style="text-align: right;">供应商：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Width="120px"
                        AllowCustomText="true" EnableLoadOnDemand="True" Height="120px" DataValueField="CompanyId"
                        DataTextField="CompanyName" AppendDataBoundItems="true" OnItemsRequested="RCB_Company_OnItemsRequested">
                        <Items>
                            <rad:RadComboBoxItem Value="00000000-0000-0000-0000-000000000000" Text="所有公司" Selected="True" />
                        </Items>
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">活动周期</td>
                <td>
                    <asp:TextBox ID="txt_ActivityTimeStart" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',maxDate:$('input[id$=txt_ActivityTimeEnd]').val()})"></asp:TextBox>
                    至
                    <asp:TextBox ID="txt_ActivityTimeEnd" runat="server" Width="70px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',minDate:$('input[id$=txt_ActivityTimeStart]').val()})"></asp:TextBox>
                </td>
                <td style="text-align: right;">责任人：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Persion" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        AllowCustomText="True" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="RealName"
                        DataValueField="PersonnelId" Width="120px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td style="text-align: right;">标题或备注：</td>
                <td>
                    <asp:TextBox ID="txt_TitleOrMemo" runat="server" Width="300px"></asp:TextBox>
                </td>
                <td style="text-align: right;">
                    <asp:ImageButton ID="IbtnSearch" runat="server" SkinID="SearchButton" OnClick="IbtnSearch_Click" />
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td class="Footer" align="right" colspan="11">
                    <Ibt:ImageButtonControl ID="ImageButtonControl1" Text="添加借记单" runat="server" SkinType="Insert"
                        OnClientClick="return AddDebitNote()"></Ibt:ImageButtonControl>
                    <Ibt:ImageButtonControl ID="IB_Purchasing" Text="生成采购单" runat="server" SkinType="Insert"
                        CausesValidation="false" OnClick="IbnPurchasing_Click"></Ibt:ImageButtonControl>
                    <Ibt:ImageButtonControl ID="IB_Logout" Text="注销" runat="server" SkinType="Insert"
                        CausesValidation="false" OnClick="IbnLogout_Click" OnClientClick="return OnLogoutConfirm()"></Ibt:ImageButtonControl>
                    <%--<Ibt:ImageButtonControl ID="IB_Delete" Text="删除" runat="server" SkinType="Delete"
                        CausesValidation="false" OnClick="IbDelete_Click" OnClientClick="return OnDeleteConfirm()"></Ibt:ImageButtonControl>--%>
                    <Ibt:ImageButtonControl ID="IB_ExportData" runat="server" SkinType="ExportExcel"
                        CausesValidation="false" OnClick="LbxlsClick"></Ibt:ImageButtonControl>
                    <%--<Ibt:ImageButtonControl ID="IB_ExportData" Text="" runat="server" CausesValidation="false" OnClick="LbxlsClick"></Ibt:ImageButtonControl>--%>
                </td>
            </tr>
        </table>
        <rad:RadGrid ID="RG_DebitNote" runat="server" SkinID="CustomPaging" OnNeedDataSource="RgDebitNote_OnNeedDataSource"
            AllowMultiRowSelection="true" Width="100%" OnItemCommand="RG_DebitNote_OnItemCommand">
            <ClientSettings>
                <Selecting AllowRowSelect="true" EnableDragToSelectRows="False" />
            </ClientSettings>
            <MasterTableView DataKeyNames="PurchasingId,PersonResponsible" ClientDataKeyNames="PurchasingId,PersonResponsible"
                CommandItemDisplay="None" Width="100%">
                <Columns>
                    <rad:GridBoundColumn DataField="PurchasingId" HeaderText="PurchasingId" UniqueName="PurchasingId"
                        Visible="False">
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="State" HeaderText="State" UniqueName="State" Visible="False">
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="" HeaderStyle-Width="30px" HeaderStyle-HorizontalAlign="Center"
                        ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:CheckBox ID="CB_Check" Checked="False" Enabled='<%# ((int)Eval("State")==(int)DebitNoteState.ToPurchase) %>'
                                runat="server" />
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="PurchasingNo" HeaderText="对应采购单号" UniqueName="PurchasingNo">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="供应商">
                        <ItemTemplate>
                            <%# GetCompanyName(new Guid(Eval("CompanyId").ToString())) %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="标题">
                        <ItemTemplate>
                            <%# Eval("Title")%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" Width="120" />
                        <ItemStyle HorizontalAlign="Center" Width="120" />
                    </rad:GridTemplateColumn>
                    <%--<rad:GridTemplateColumn DataField="PurchaseGroupId" ReadOnly="true" HeaderText="分组" Visible="False"
                        UniqueName="PurchaseGroupId">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# GetPurchaseGroupIdName(Eval("PurchaseGroupId"))%>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>--%>
                    <rad:GridTemplateColumn DataField="PresentAmount" HeaderText="赠品总价" UniqueName="PresentAmount">
                        <ItemTemplate>
                            <%# Convert.ToDecimal(Eval("PresentAmount")) == 0 ? "-" : ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("PresentAmount"))%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="ActivityTimeStart" HeaderText="活动开始" DataFormatString="{0:yyyy-MM-dd}">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="ActivityTimeEnd" HeaderText="活动结束" DataFormatString="{0:yyyy-MM-dd}">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="借记单状态">
                        <ItemTemplate>
                            <%# GetStateName(int.Parse(Eval("State").ToString())) %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="仓库">
                        <ItemTemplate>
                            <%# GetWarehouseName(new Guid(Eval("WarehouseId").ToString()))%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="备注">
                        <ItemTemplate>
                            <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                                onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                            <div style="position: absolute;">
                                <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative; display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px; font-weight: bold; height: auto; overflow: visible; word-break: break-all;"
                                    runat="server">
                                    <%# Eval("Memo")%>
                                </div>
                            </div>
                            <asp:ImageButton ID="ImageButton1" SkinID="EditImageButton" runat="server" OnClientClick='<%# "return AddMemoClick(\"" + Eval("PurchasingId")+ "\")" %>'
                                ToolTip="添加备注" />
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="责任人">
                        <ItemTemplate>
                            <%# GetPersonName(new Guid(Eval("PersonResponsible").ToString()))%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="操作">
                        <ItemTemplate>
                            <Ibt:ImageButtonControl ID="Ibn_Part" Text="查看" runat="server" CausesValidation="false"
                                OnClientClick='<%# "return Show(\"" + Eval("PurchasingId")+ "\");" %>' Visible='<%# Ibn_PartIsVisible(Eval("PurchasingNo")) %>'
                                SkinID="InsertImageButton"></Ibt:ImageButtonControl>
                            <Ibt:ImageButtonControl ID="Ibn_Look" Text="查看" runat="server" CausesValidation="false"
                                Visible='<%# Ibn_LookIsVisible(Eval("PurchasingNo"),(int)Eval("State")) %>' SkinID="AffirmImageButton"
                                OnClientClick='<%# "return ReadMemoClick(\"" + Eval("PurchasingId")+ "\");" %>'></Ibt:ImageButtonControl>
                            <Ibt:ImageButtonControl ID="Ibn_OK" Text="完成" runat="server" CausesValidation="false"
                                CommandName="Complete" Visible='<%# Ibn_OKIsVisible(Eval("PurchasingNo"),(int)Eval("State")) %>'
                                OnClientClick='<%# "return Showconfirm();"%>' SkinID="AffirmImageButton"></Ibt:ImageButtonControl>
                            <Ibt:ImageButtonControl ID="Ibn_ChargeOff" Text="待核销" runat="server" CausesValidation="false"
                                CommandName="ChargeOff" Visible='<%# Ibn_ChargeOffIsVisible(Eval("PurchasingNo"),(int)Eval("State")) %>'
                                OnClientClick='<%# "return ChargeOffShowconfirm();"%>' SkinID="AffirmImageButton"></Ibt:ImageButtonControl>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="PersonResponsible" HeaderText="PersonResponsible"
                        UniqueName="PersonResponsible" Visible="False">
                    </rad:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>
    <rad:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
            <rad:RadWindow ID="EditDebitNoteForm" Title="赠品借记单" runat="server" OnClientShow="clientShow" />
            <rad:RadWindow ID="RM1" Title="添加赠品借记单" runat="server" Width="550" Height="300" />
            <rad:RadWindow ID="RM2" Title="添加赠品借记单备注" runat="server" Width="660" Height="500" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager runat="server" ID="RAM" OnAjaxRequest="Ram_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_DebitNote" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IbtnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_DebitNote" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_Purchasing">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_DebitNote" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_Logout">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_DebitNote" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_Delete">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_DebitNote" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RG_DebitNote">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_DebitNote" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

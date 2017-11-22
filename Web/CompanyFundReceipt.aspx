<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MainMaster.master"
    Debug="true" CodeBehind="CompanyFundReceipt.aspx.cs" Inherits="ERP.UI.Web.CompanyFundReceipt" %>

<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">

    <script type="text/javascript" src="/JavaScript/telerik.js"></script>

    <div style="margin-left: 10px; margin-right: 10px;">
        <table>
            <tr>
                <td>
                    收付款类型：
                    <rad:RadComboBox ID="RCB_ReceiptType" Width="60" runat="server">
                    </rad:RadComboBox>
                </td>
                <td>
                    审核状态：
                    <rad:RadComboBox ID="RCB_VerifyStatus" Width="100" runat="server">
                        
                    </rad:RadComboBox>
                </td>
                <td>
                    <rad:RadDatePicker ID="RC_StartDate" Width="95px" DateInput-EmptyMessage="起始时间" runat="server">
                    </rad:RadDatePicker>
                    -
                    <rad:RadDatePicker ID="RC_EndDate" Width="95px" DateInput-EmptyMessage="结束时间" runat="server">
                    </rad:RadDatePicker>
                </td>
                <td>
                    <rad:RadTextBox ID="RTB_SearchReceiptNO" EmptyMessage="搜索单据号" runat="server">
                    </rad:RadTextBox>
                </td>
                <td>
                    供应商：
                    <rad:RadComboBox ID="RCB_Company" Width="150" runat="server"  Height="300"
                        CausesValidation="false" AutoPostBack="true"  AllowCustomText="True" 
                        EnableLoadOnDemand="True" onitemsrequested="RCB_Company_ItemsRequested" >
                    </rad:RadComboBox>
                </td>
                <td>
                    <asp:Button runat="server" ID="Btn_Search" Text="搜索" OnClick="Btn_Search_Click" />
                </td>
            </tr>
        </table>
        <br />
        <rad:RadGrid ID="RG_ReceiptGridList" OnNeedDataSource="RG_ReceiptGridList_OnNeedDataSource"
            SkinID="CustomPaging" runat="server" OnItemDataBound="RG_ReceiptGridList_ItemDataBound">
            <MasterTableView>
                <CommandItemTemplate>
                    <Ibt:ImageButtonControl ID="Btn_OpenPayReceiptForm" OnClientClick="openWindow('receive');return false;"
                        Text="填写收款单" runat="server" SkinType="Insert"></Ibt:ImageButtonControl>
                    <Ibt:ImageButtonControl ID="Btn_OpenReceiveReceiptForm" Text="填写付款单" OnClientClick="openWindow('pay');return false;"
                        runat="server" SkinType="Insert"></Ibt:ImageButtonControl>
                    <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" CausesValidation="false"
                        SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl>
                </CommandItemTemplate>
                <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                <Columns>
                    <rad:GridBoundColumn HeaderText="单据号" DataField="ReceiptNo">
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="收付款类型" DataField="ReceiptType">
                        <ItemTemplate>
                            <%#GetReceiptType(Eval("ReceiptType").ToString())%>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="公司" UniqueName="Filiale">
                    <ItemTemplate>
                        <asp:Label ID="LbFiliale" runat="server" Text='<%# GetFilialeName(Eval("FilialeId").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="往来单位">
                        <ItemTemplate>
                            <%#GetCompanyName(Eval("CompanyId").ToString())%>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="申请人" DataField="ApplicantID">
                        <ItemTemplate>
                            <%#GetApplicantName(Eval("ApplicantID").ToString()) %></ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="申请日期" DataField="ApplyDateTime">
                        <ItemTemplate>
                            <%#Convert.ToDateTime(Eval("ApplyDateTime")).ToShortDateString() %></ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="申请金额" DataField="RealityBalance">
                        <ItemTemplate>
                            <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealityBalance"))%>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="紧急程度" DataField="IsUrgent">
                        <ItemTemplate>
                            <%# Convert.ToBoolean(Eval("IsUrgent"))?"加急":"普通"%>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="单据状态" DataField="ReceiptStatus">
                        <ItemTemplate>
                            <%#GetReceiptStatus(Eval("ReceiptStatus").ToString())%></ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Remark">
                        <ItemTemplate>
                            <asp:ImageButton ID="ImgBtn_Remark" runat="server" OnClientClick='<%# "openWindow(\"remark\",\"ID=" + Eval("ReceiptID") + "\");return false;" %>'
                                SkinID="InsertImageButton" onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                            <div style="position: absolute;">
                                <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative;
                                            display: none; background-color: #CCFFFF; border: solid 1px #666; width: 250px;
                                            font-weight: bold; height: auto; overflow: visible; word-break: break-all;" runat="server">
                                    <%# Eval("Remark").ToString().Replace("\n","<br/>")%>
                                </div>
                            </div>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="作废">
                        <ItemTemplate>
                            <asp:ImageButton ID="ImgBtn_LoseReceipt" OnClientClick='<%#"openWindow(\"cancelreceipt\",\"ID="+Eval("ReceiptID") +"&No="+Eval("ReceiptNo") +"\");return false;"%>'
                                runat="server" SkinID="DeleteImageButton" />
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="操作">
                        <ItemTemplate>
                            <asp:Button ID="btn_Edit" runat="server" Text="编辑" OnClientClick='<%#"openEditWindow("+Eval("ReceiptType") +",\"ID="+Eval("ReceiptID") +"&Flag=0\");return false;"%>'/>
                            <asp:Button ID="btn_Look" runat="server" Text="查看" OnClientClick='<%#"openEditWindow("+Eval("ReceiptType") +",\"ID="+Eval("ReceiptID") +"&Flag=1\");return false;"%>'/>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                        <ItemStyle Width="50px" HorizontalAlign="Center"></ItemStyle>
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>
    <!--付款单表单-->
    <rad:RadWindowManager ID="RWM_FormPanel" runat="server">
        <Windows>
            <rad:RadWindow runat="server" Width="600" Height="400" ID="RW_AddReceiveReceiptForm"
                Title="填写收款单" />
            <rad:RadWindow runat="server" Width="900" Height="500" ID="RW_AddPayReceiptForm"
                Title="填写付款单" />
            <rad:RadWindow runat="server" Width="400" Height="200" ID="RW_CancelReceiptForm"
                Title="单据作废" />
            <rad:RadWindow runat="server" Width="700" Height="500" ID="RW_ReceiptRemarkForm"
                Title="管理意见" />
            <rad:RadWindow runat="server" Width="900" Height="500" ID="RW_EditReceiptForm" 
                Title="编辑单据" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager runat="server" ID="RAM" OnAjaxRequest="RAM_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RG_ReceiptGridList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_ReceiptGridList" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_ReceiptGridList" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Company">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Btn_Search" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_ReceiptGridList" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server">
    </rad:RadAjaxLoadingPanel>

<rad:RadScriptBlock ID="RSB" runat="server">
    <script type="text/javascript" src="JavaScript/telerik.js"></script>

    <script type="text/javascript">
        function openWindow(type, query) {
            if (type == "receive") {
                var win = window.radopen("./Windows/CompanyFundReceiveReceiptAdd.aspx", "RW_AddReceiveReceiptForm");
                win.center();
            }
            else if (type == "pay") {
                var win = window.radopen("./Windows/CompanyFundPayReceiptAdd.aspx", "RW_AddPayReceiptForm");
                win.center();
            }
            else if (type == "cancelreceipt") {
                var win = window.radopen("./Windows/CompanyFundReceiptCancel.aspx?" + query, "RW_CancelReceiptForm");
                win.center();
            }
            else if (type == "editreceipt") {
                var win = window.radopen("./Windows/CompanyFundReceiptEdit.aspx?" + query, "RW_EditReceiptForm");
                win.center();
            }
            else if (type == "remark") {
                var win = window.radopen("./Windows/CompanyFundReceiptRemark.aspx?" + query, "RW_ReceiptRemarkForm");
                win.center();
            }
        }

        function openEditWindow(type, query) {
            if (type == "0") {
                var win = window.radopen("./Windows/CompanyFundReceiptEdit.aspx?" + query, "RW_EditReceiptForm");
                win.center();
            }
            else {
                var win = window.radopen("./Windows/CompanyFundPayReceiptAdd.aspx?" + query, "RW_EditReceiptForm");
                win.center();
            }
        }

        function refreshGrid(arg) {
            if (!arg) {
                $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
            }
            else {
                $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
            }
        }

        function ShowImg(obj) {
            var object = eval(obj);
            object.style.display = "block";
        }

        function HiddleImg(obj) {
            var object = eval(obj);
            object.style.display = "none";
        }
        
    </script>
</rad:RadScriptBlock>
</asp:Content>

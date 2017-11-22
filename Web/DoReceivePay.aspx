<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="DoReceivePay.aspx.cs" Inherits="ERP.UI.Web.DoReceivePay" %>
<%@ Import Namespace="ERP.Enum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script type="text/javascript">

            function ShowRemarkForm(ReceiptID) {
                window.radopen("Windows/ShowRemarkForm.aspx?RceiptId=" + ReceiptID, "RemarkWindow");
                return false;
            }

            function ShowCheckForm(ReceiptID) {
                window.radopen("Windows/CheckReceiptForm.aspx?RceiptId=" + ReceiptID + "&type=1", "DoCheck");
                return false;
            }

            function ShowImg(obj) {
                var object = eval(obj);
                object.style.display = "block";
            }

            function HiddleImg(obj) {
                var object = eval(obj);
                object.style.display = "none";
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
    <rad:RadGrid ID="RG_CheckInfo" runat="server" OnNeedDataSource="RgCheckInfoNeedDataSource" SkinID="Common_Foot" ShowFooter="true"
        OnItemCommand="RgCheckInfoItemCommand" OnItemDataBound="RG_CheckInfo_ItemDataBound" AllowMultiRowSelection="True" >
        <ClientSettings>
            <Selecting EnableDragToSelectRows="false" />
        </ClientSettings>
        <MasterTableView DataKeyNames="ReceiptID,CompanyID">
            <CommandItemTemplate>
                公司：<asp:DropDownList ID="DdlSaleFiliale" DataSource='<%# BindFilialeDataBound() %>'
                    DataTextField="Value" DataValueField="Key" SelectedValue='<%# SelectSaleFilialeId %>'
                    runat="server">
                </asp:DropDownList>
                收付款类型：<asp:DropDownList ID="DDL_ReceivePayType" DataSource='<%# BindTypeDataBound() %>' 
                                DataTextField="Value" DataValueField="Key" SelectedValue='<%# ((Int32)Type).ToString() %>' runat="server">
                            </asp:DropDownList>
                状态：<asp:DropDownList ID="DDL_CheckState" DataSource='<%# BindStatusDataBound() %>'
                            DataTextField="Value" DataValueField="Key"  SelectedValue='<%# ((Int32)Status).ToString() %>' runat="server">
                      </asp:DropDownList>
                付款银行：<asp:DropDownList ID="DDL_Bank" DataSource='<%# BindBankDataBound() %>' DataTextField="text" DataValueField="value" runat="server" Width="120px" SelectedValue='<%# BankId.ToString() %>'></asp:DropDownList>
                申请日期：<rad:RadDatePicker ID="RDP_StartTime" runat="server" Width="95px"></rad:RadDatePicker>
                -
                <rad:RadDatePicker ID="RDP_EndTime" runat="server" Width="95px"></rad:RadDatePicker>
                打款日期：<rad:RadDatePicker ID="RDP_SExecuteTime" runat="server" Width="95px"></rad:RadDatePicker>
                -
                <rad:RadDatePicker ID="RDP_EExecuteTime" runat="server" Width="95px"></rad:RadDatePicker>
                单据号：<asp:TextBox ID="TB_CompanyFundReciptNO" runat="server" Width="120" Text='<%# ReceiptNo %>'></asp:TextBox>
                &nbsp;&nbsp;
                <asp:LinkButton ID="LB_Search" runat="server" CommandName="Search">
                    <asp:Image ID="ImgSearch" runat="server" ImageAlign="AbsMiddle" SkinID="searchimagebutton" />搜索
                </asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="LB_Finish" runat="server" CommandName="AllDo">
                    <asp:Image ID="ImgFinish" runat="server" ImageAlign="AbsMiddle" SkinID="InsertImageButton" />批量执行
                </asp:LinkButton>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Center" Height="26px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="" HeaderStyle-Width="30px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="CB_Check" Checked="False" Enabled='<%# 
                        Eval("PayBankAccountsId")!=null && Eval("PayBankAccountsId").ToString()!=Guid.Empty.ToString() && !string.IsNullOrEmpty(Eval("DealFlowNo").ToString()) &&
                         Convert.ToInt32(Eval("ReceiptStatus"))<(int)CompanyFundReceiptState.Executed %>' runat="server"/>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ReceiptNo" HeaderText="单号" UniqueName="ReceiptNo">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="收付款类型" UniqueName="ReceiptType">
                    <ItemTemplate>
                        <font color='<%# GetColor(Eval("ReceiptType").ToString())%>'>
                            <asp:Label ID="ReceiptType" runat="server" Text='<%# GetReceiptTypeName(Eval("ReceiptType").ToString()) %>'></asp:Label>
                        </font>
                    </ItemTemplate>
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="公司" UniqueName="Filiale">
            <ItemTemplate>
                <asp:Label ID="LbFiliale" runat="server" Text='<%# GetFilialeName(Eval("FilialeId").ToString()) %>'></asp:Label>
            </ItemTemplate>
            <HeaderStyle Width="100px" HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
        </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="往来单位" UniqueName="CompanyName">
                    <ItemTemplate>
                        <asp:Label ID="compName" runat="server" Text='<%# GetCompName(Eval("CompanyID").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="收款人" UniqueName="WebSite">
                    <ItemTemplate>
                        <asp:Label ID="WebSite" runat="server" Text='<%# GetCompanyCussentByCompanyId(new Guid(Eval("CompanyID").ToString()),new Guid(Eval("FilialeId").ToString()),0) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="收款账号" UniqueName="AccountNumber">
                    <ItemTemplate>
                        <asp:Label ID="AccountNumber" runat="server" Text='<%# GetCompanyCussentByCompanyId(new Guid(Eval("CompanyID").ToString()),new Guid(Eval("FilialeId").ToString()),1) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="付款银行" UniqueName="PayBankAccountsId">
                    <ItemTemplate>
                        <asp:Label ID="BankName" runat="server" Text='<%# GetBankNameById(Eval("PayBankAccountsId"))%>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="RealityBalance" HeaderText="申请金额"
                    SortExpression="RealityBalance" UniqueName="RealityBalance" >
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealityBalance"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FooterStyle Font-Bold="true" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申请日期" UniqueName="ApplyDateTime">
                    <ItemTemplate>
                        <asp:Label ID="LB_ApplyDateTime" runat="server" Text='<%# DateTime.Parse(Eval("ApplyDateTime").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("ApplyDateTime").ToString()).ToString("yyyy年MM月dd日") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="审核日期" UniqueName="AuditingDate">
                    <ItemTemplate>
                        <asp:Label ID="LB_AuditingDate" runat="server" Text='<%# DateTime.Parse(Eval("AuditingDate").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("AuditingDate").ToString()).ToString("yyyy年MM月dd日") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="单据状态" UniqueName="ReceiptStatus">
                    <ItemTemplate>
                        <asp:Label ID="LB_ReceiptStatus" runat="server" Text='<%# GetReceiptStatus(Eval("ReceiptStatus").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
<%--                <rad:GridTemplateColumn HeaderText="付款银行" UniqueName="PayBank">
                    <ItemTemplate>
                        <asp:Label ID="LbBankName" runat="server" Text='<%# GetBankName(Eval("FilialeId").ToString(),Eval("CompanyID").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>--%>
                <rad:GridTemplateColumn HeaderText="执行" UniqueName="DoReceivePay">
                    <ItemTemplate>
                        <asp:ImageButton ID="DoReceivePayImageButton" CommandName="DoReceivePay" runat="server"
                                         SkinID="AffirmImageButton"  OnClientClick='<%# "return ShowCheckForm(\"" + Eval("ReceiptID") + "\",\"1\");" %>'
                                         />
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Remark">
                    <ItemTemplate>
                        <asp:ImageButton ID="RemarkImageButton" CommandName="Remark" runat="server" SkinID="InsertImageButton"
                            OnClientClick='<%# "return ShowRemarkForm(\"" + Eval("ReceiptID") + "\",\"1\");" %>'
                            onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                            onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                        <div style="position: absolute;">
                            <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative;
                                display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px;
                                font-weight: bold; height: auto; overflow: visible; word-break: break-all;" runat="server">
                                <%# Eval("Remark").ToString().Replace("\n","<br/>")%>
                            </div>
                        </div>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RG_CheckInfo">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_CheckInfo" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_CheckInfo" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager ID="RWM" runat="server" Height="630px" Width="800px" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="RemarkWindow" runat="server" Title="管理意见" Height="500" Width="700" />
            <rad:RadWindow ID="DoCheck" runat="server" Title="收付款执行" Height="500" Width="900" />
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

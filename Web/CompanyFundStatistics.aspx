<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="CompanyFundStatistics.aspx.cs" Inherits="ERP.UI.Web.CompanyFundStatistics" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">

    <script type="text/javascript" src="JavaScript/telerik.js"></script>

    <script type="text/javascript">
        function ShowImg(obj) {
            var object = eval(obj);
            object.style.display = "block";
        }

        function HiddleImg(obj) {
            var object = eval(obj);
            object.style.display = "none";
        }
    </script>

    <table width="100%">
        <tr>
            <td align="right">
                搜索起止时间：
            </td>
            <td>
                <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="95px" DateInput-EmptyMessage="起始时间" >
                </rad:RadDatePicker>
            </td>
            <td>
                -
            </td>
            <td>
                <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="95px" DateInput-EmptyMessage="结束时间" >
                </rad:RadDatePicker>
            </td>
            <td>
                审批人：
            </td>
            <td>
                <rad:RadComboBox ID="RCB_Auditing" runat="server" AutoPostBack="true" CausesValidation="false"
                    AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="RealName" DataValueField="Id"
                    Width="120px" Height="200px" OnItemsRequested="RcbAuditingItemsRequested" EmptyMessage="输入搜索审批人">
                </rad:RadComboBox>
            </td>
            <td>
                往来单位：
            </td>
            <td>
                <rad:RadComboBox runat="server" ID="DDL_Company" AutoPostBack="true" CausesValidation="false"
                    AllowCustomText="True" EnableLoadOnDemand="True" OnItemsRequested="DDL_Company_ItemRequested" Width="160px" MaxHeight="300"></rad:RadComboBox>
                <%--<asp:DropDownList ID="DDL_Company" runat="server">
                </asp:DropDownList>--%>
            </td>
            <td>
                公司：
            </td>
            <td>
                <asp:DropDownList ID="DdlFiliale" runat="server" DataTextField="Value" DataValueField="Key" Width="120px"></asp:DropDownList>
            </td>
            <td>
                付款银行：
            </td>
            <td>
                <asp:DropDownList ID="DdlBankAccount" DataTextField="Value" DataValueField="Key" runat="server" Width="160"></asp:DropDownList>
            </td>
            <td align="right">
                <asp:ImageButton Style='vertical-align: middle' ID="LB_Search" runat="server" SkinID="CreationChart"
                    OnClick="LbSearchClick" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RGCompany" runat="server" SkinID="Common_Foot" OnNeedDataSource="RgCompanyNeedDataSource"
        OnDetailTableDataBind="RgCompanyDetailTableDataBind" OnItemDataBound="RgCompanyItemDataBound" 
        AllowPaging="True" DataMember="One" ShowFooter="true" 
        GridLines="None">
        <MasterTableView DataKeyNames="CompanyID" DataMember="One" EditMode="InPlace">
            <CommandItemTemplate>
                &nbsp;&nbsp;&nbsp;
                <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh">
                </Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="审批人" UniqueName="AuditorID">
                    <ItemTemplate>
                        <asp:Label ID="Lab_AuditorID" runat="server" Text='<%# Eval("AuditorID").ToString()==Guid.Empty.ToString()?"": GetAuditingById(Eval("AuditorID")) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    <HeaderStyle HorizontalAlign="center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="往来单位" UniqueName="CompanyID">
                    <ItemTemplate>
                        <asp:Label ID="Lab_CompanyID" runat="server" Text='<%# GetCompanyName(Eval("CompanyID").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    <HeaderStyle HorizontalAlign="center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="付款" UniqueName="PayRealityBalance">
                    <ItemTemplate>
                        <asp:Label ID="LabPayRealityBalance" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("PayRealityBalance")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="center" />
                    <FooterStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    <FooterTemplate>
                        <asp:Label runat="server" ID="TbTotalAmount" Text=""></asp:Label>
                    </FooterTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="当年折扣" UniqueName="DiscountMoney">
                    <ItemTemplate>
                        <asp:Label ID="LabDiscountMoney" runat="server" Text='<%# Eval("DiscountMoney") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    <HeaderStyle HorizontalAlign="center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="去年返利" UniqueName="LastRebate">
                    <ItemTemplate>
                        <asp:Label ID="LabLastRebate" runat="server" Text='<%# Eval("LastRebate") %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    <HeaderStyle HorizontalAlign="center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="收款" UniqueName="RealityBalance">
                    <ItemTemplate>
                        <asp:Label ID="Lab_RealityBalance" runat="server" Text='<%#  ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealityBalance")) %>'></asp:Label>
                    </ItemTemplate>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    <HeaderStyle HorizontalAlign="center" />
                    <FooterStyle HorizontalAlign="Center" />
                    <FooterTemplate>
                        <asp:Label runat="server" ID="TB_TotalAmount" Text=""></asp:Label>
                    </FooterTemplate>
                </rad:GridTemplateColumn>
            </Columns>
            <DetailTables>
                <rad:GridTableView EditMode="InPlace" DataKeyNames="ReceiptID" DataMember="Two" CommandItemDisplay="Top" 
                    Width="100%" runat="server" PageSize="15" NoDetailRecordsText="无子记录信息。">
                    <CommandItemTemplate>
                    </CommandItemTemplate>
                    <Columns>
                        <rad:GridBoundColumn HeaderText="单据号" DataField="ReceiptNo">
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn HeaderText="收付款公司" DataField="FilialeId">
                            <ItemTemplate>
                                <%# GetFilialeName(Eval("FilialeId"))%>
                            </ItemTemplate>
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="收付款类型" DataField="ReceiptType">
                            <ItemTemplate>
                                <%#GetReceiptType(Eval("ReceiptType").ToString())%>
                            </ItemTemplate>
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
                                <%#Convert.ToDateTime(Eval("ApplyDateTime")).ToString("yyyy-MM-dd") %></ItemTemplate>
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="申请金额" UniqueName="Cost">
                            <ItemTemplate>
                                <asp:Label ID="Lab_Cost" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealityBalance")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="center" />
                            <FooterStyle HorizontalAlign="Center" />
                            <FooterTemplate>
                                <asp:Label runat="server" ID="TB_CostTotal" Text='<%# GetDetailSum() %>'></asp:Label>
                            </FooterTemplate>
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="备注" UniqueName="Memo">
                            <ItemTemplate>
                                <asp:ImageButton ID="ClewImageButton" runat="server" SkinID="InsertImageButton" onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                    onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                                <div style="position: absolute;">
                                    <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative;
                                        display: none; background-color: #CCFFFF; border: solid 1px #666; width: 250px;
                                        font-weight: bold; height: auto; overflow: visible; word-break: break-all;" runat="server">
                                        <%# Eval("Remark").ToString()%>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                </rad:GridTableView>
            </DetailTables>
        </MasterTableView>
        <HierarchySettings CollapseTooltip="关闭" ExpandTooltip="展开" />
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="Loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RCB_Auditing">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Auditing"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DDL_Company">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DDL_Company"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGCompany">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCompany"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGCompany"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue" EnableViewState="false">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

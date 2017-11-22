<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="ReturnGoodsCheck.aspx.cs" Inherits="ERP.UI.Web.ReturnGoodsCheck" %>

<%@Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
        <script src="JavaScript/common.js" type="text/javascript"></script>
        <script type="text/javascript">
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
            
            function ShowClewForm(orderId, saleFilialeId) {
                window.radopen("Windows/ShowClewForm.aspx?OrderId=" + orderId + "&SaleFilialeId=" + saleFilialeId, "ClewWin");
                return false;
            }

            function ShowProcessForm(refundId) {
                window.radopen("Windows/ReturnGoodsChecking.aspx?RefundId=" + refundId, "CheckWin"); 
                return false;
            }
        </script>
    </rad:RadScriptBlock>
    <rad:RadGrid ID="RGRefund" runat="server" SkinID="Common_Foot" OnNeedDataSource="RgRefund_NeedDataSource"  OnItemCommand="RgRefund_ItemCommand">
    <MasterTableView DataKeyNames="RefundId" ClientDataKeyNames="RefundId">
        <CommandItemTemplate>
            退回时间：<rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="90px" SelectedDate='<%# StartTime %>'></rad:RadDatePicker>
            -&nbsp;<rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="90px" SelectedDate='<%# EndTime %>'></rad:RadDatePicker>
            &nbsp;&nbsp;检查状态：
            <asp:DropDownList ID="DDL_CheckState" runat="server" SelectedValue='<%# SearchCheckState %>'>
                <asp:ListItem Text="全部" Value="-1"></asp:ListItem>
                <asp:ListItem Text="待检查" Value="0"></asp:ListItem>
                <asp:ListItem Text="检查通过" Value="1"></asp:ListItem>
                <asp:ListItem Text="退回" Value="2"></asp:ListItem>
            </asp:DropDownList>
            <%--所属公司：<rad:RadComboBox ID="RCB_FromSource" runat="server" 
                DataSource='<%# FilialeList %>' SelectedValue='<%# SelectedFilialeId %>'
                DataTextField="FilialeName" DataValueField="FilialeId">
            </rad:RadComboBox>--%>
            <asp:TextBox ID="TB_Search" runat="server" SkinID="StandardInput" Text='<%# SearchKey %>'></asp:TextBox>
            &nbsp;&nbsp;
            <Ibt:ImageButtonControl ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search" SkinType="Search" Text="搜索"></Ibt:ImageButtonControl>
            &nbsp;&nbsp;
            <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" CausesValidation="false" SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl>
        </CommandItemTemplate>
        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
        <Columns>
            <rad:GridBoundColumn DataField="RefundId" HeaderText="退换编号" UniqueName="RefundId" HeaderStyle-Width="140px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" Visible="False" ></rad:GridBoundColumn>
            <rad:GridBoundColumn DataField="RefundNo" HeaderText="退换货号" UniqueName="RefundNo" HeaderStyle-Width="140px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center" ></rad:GridBoundColumn>
            <rad:GridBoundColumn DataField="OrderId" HeaderText="OrderId" UniqueName="OrderId" Visible="false"></rad:GridBoundColumn>
            <rad:GridTemplateColumn HeaderText="订单编号" HeaderStyle-HorizontalAlign="Center" HeaderStyle-Width="140px" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="lbOrderNo" Text='<%# Eval("OrderNo")%>' runat="server"></asp:Label>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="收货人" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:Label ID="lbConsignee" Text='<%# Eval("Consignee")%>' runat="server"></asp:Label>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="物流编号" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%--<asp:TextBox runat="server" ID="TbExpressNo" Text='<%# Eval("ExpressNo")%>' 
                     OnTextChanged="TbExpressNoOnTextChanged" AutoPostBack="True" Width="140px" Visible='<%# !IsAllowEditExpressNo(Eval("ExpressNo"),Eval("CheckState")) %>'></asp:TextBox>--%>
                     <asp:Label ID="lbExpressNo" Text='<%# Eval("ExpressNo")%>' runat="server"></asp:Label>
                </ItemTemplate>
                <ItemStyle Width="150"></ItemStyle>
            </rad:GridTemplateColumn>
            <rad:GridBoundColumn DataField="ExpressName" HeaderText="物流名称" UniqueName="ExpressName" Visible="False" HeaderStyle-Width="140px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
            <rad:GridBoundColumn DataField="Amount" HeaderText="退款金额" UniqueName="Amount" Visible="False" HeaderStyle-Width="140px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
            <rad:GridTemplateColumn HeaderText="检查状态" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%# GetCheckState(Eval("CheckState").ToString()) %>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="检查公司" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%# GetCheckFilialeName(Eval("CheckFilialeId")) %>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="移交" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%# (bool.Parse(Eval("IsTransfer").ToString())? "√":"-") %>
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="管理意见" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <%--<img id="imgClew" src="App_Themes/Default/images/Memo.gif" title='<%# GetClew(Eval("Clew"))%>' alt="" />--%>
                    <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                        OnClientClick='<%# "return ShowClewForm(\"" + Eval("OrderID") + "\",\"" + Eval("SaleFilialeId") + "\");" %>' 
                        ToolTip='<%#GetMisClew(Eval("OrderId"))%>' />
                </ItemTemplate>
            </rad:GridTemplateColumn>
            <rad:GridTemplateColumn HeaderText="退回检查" UniqueName="Clew" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                <ItemTemplate>
                    <asp:ImageButton ID="IbtCheck" CommandName="Check" runat="server" SkinID="AffirmImageButton"  
                    OnClientClick='<%# "return ShowProcessForm(\"" + Eval("RefundId")  + "\");" %>' ToolTip="退回检查" />
                    <%--' <%# ShowReturnGoodsCheckJs(Eval("RefundId"),Eval("CheckFilialeId")) %>--%>
                </ItemTemplate>
            </rad:GridTemplateColumn>
        </Columns>
    </MasterTableView>
</rad:RadGrid>

<rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="Ram_AjaxRequest">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="RGRefund">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="RGRefund" LoadingPanelID="loading" />
            </UpdatedControls>
        </rad:AjaxSetting>
        <rad:AjaxSetting AjaxControlID="RAM">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="RGRefund" LoadingPanelID="loading" />
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

<rad:RadWindowManager ID="ClewWindowManager" runat="server" Title="订单查询" Height="630px" Width="800px" ReloadOnShow="true">
    <Windows>
        <rad:RadWindow ID="ClewWin" runat="server" Title="备注" Height="500" Width="700" />
        <rad:RadWindow ID="CheckWin" runat="server" Title="退回检查" Height="500" Width="800" />
    </Windows>
</rad:RadWindowManager>

</asp:Content>

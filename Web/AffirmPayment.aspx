<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="AffirmPayment.aspx.cs" Inherits="ERP.UI.Web.AffirmPayment" %>
<%@ Import Namespace="ERP.UI.Web.Common" %>

<%@ Register Src="~/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock runat="server" ID="RSB">

        <script type="text/javascript">

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }

            function ShowClewForm(orderId) {
                window.radopen("./Windows/ShowClewForm.aspx?OrderId=" + orderId, "SCForm");
                return false;
            }

            function RowDbClick(obj, args) {
                var PayId = args.getDataKeyValue("PayId");
                var payState = args.getDataKeyValue("PayState");
                var orderNo = args.getDataKeyValue("OrderNo");
                if (2 == payState) {
                    alert('已经到款确认了!');
                    return;
                }
                window.radopen("../Windows/AffirmSum.aspx?PayId=" + PayId+"&OrderNo="+orderNo, "AffSum");
                return false;
            }

            function DeleteConfirm(orderId) {
                var position = '<%= CryptPublic.GetEncryptText(((int)ERP.Enum.CancelPosition.AffirmPayment).ToString())%>';
                position = encodeURIComponent(position);
                window.radopen("./Windows/CancelReturn.aspx?cp=" + position + "&orderid=" + orderId, "DelConfirm");
                return false;
            }
               
        </script>

    </rad:RadScriptBlock>
    <rad:RadGrid ID="RG_PayNotice" runat="server" OnNeedDataSource="RG_PayNotice_NeedDataSource"
        OnItemCommand="RgGoodsOrderItemCommand" SkinID="Common_Foot">
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDbClick" />
        </ClientSettings>
        <MasterTableView DataKeyNames="OrderId,PayState,OrderNo" ClientDataKeyNames="PayId,OrderId,PayState,OrderNo">
            <CommandItemTemplate>
                <rad:RadComboBox ID="RCB_PayState" runat="server" CommandName="RebindGrid" useembeddedscripts="false"
                    AccessKey="T" MarkFirstMatch="True" ShowToggleImage="True" SelectedValue='<%#State %>'
                    Width="120px" Height="200px">
                    <Items>
                        <rad:RadComboBoxItem Value="2" Text='已支付' />
                        <rad:RadComboBoxItem Value="3" Text='待确认' Selected="true" />
                    </Items>
                </rad:RadComboBox>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:TextBox ID="TB_Search" runat="server" SkinID="StandardInput" Text='<%# SearchKey %>'></asp:TextBox>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="LB_Search" runat="server" CommandName="Search" ValidationGroup="Search">
                    <asp:Image ID="ImgSearch" runat="server" ImageAlign="AbsMiddle" SkinID="searchimagebutton" />
                    搜索
                </asp:LinkButton>
                &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" 
                    SkinType="Refresh" Text="刷新">
                </Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridBoundColumn DataField="OrderNo" HeaderText="订单号" UniqueName="OrderNo">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="PayName" HeaderText="付款人" UniqueName="PayName">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="PayBank" HeaderText="付款银行" UniqueName="PayBank">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="PayPrince" HeaderText="付款金额" UniqueName="PayPrince">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("PayPrince"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="PayTime" HeaderText="付款时间" UniqueName="PayTime">
                    <ItemTemplate>
                        <%#(DateTime)Eval("PayTime") == DateTime.MinValue ? "" : ((DateTime)Eval("PayTime")).ToString("MM-dd HH:mm")%>
                    </ItemTemplate>
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="管理意见" Visible="false">
                    <ItemTemplate>
                        <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                          OnClientClick='<%# "return ShowClewForm(\"" + Eval("OrderId") + "\");" %>' ToolTip='<%# Eval("PayDes") %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="ClewWindowManager" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="SCForm" runat="server" Title="管理意见" Height="500" Width="700" />
            <rad:RadWindow ID="AffSum" runat="server" Title="" Height="400px" Width="700px" />
            <rad:RadWindow ID="DelConfirm" runat="server" Title="" Height="195px" Width="420px" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading"  OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_PayNotice" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
             <rad:AjaxSetting AjaxControlID="RG_PayNotice">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_PayNotice" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_PayNotice" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

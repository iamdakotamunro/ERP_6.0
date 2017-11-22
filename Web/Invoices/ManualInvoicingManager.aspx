<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="ManualInvoicingManager.aspx.cs" Inherits="ERP.UI.Web.Invoices.ManualInvoicingManager" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum.ApplyInvocie" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
<script type="text/javascript">
    function ShowRemarkForm(applyId,type) {
        window.radopen("../Windows/ShowRemarkForm.aspx?ApplyId=" + applyId+"&Type="+type, "RemarkWindow");
        return false;
    }

    function ShowLeagueForm(applyId) {
        window.radopen("./ApprovalLeagueInvoiceForm.aspx?ApplyId=" + applyId + "&Type=1", "RwInvoice");
        return false;
    }

    function ShowOrderForm(applyId) {
        window.radopen("./ApprovalOrderInvoiceForm.aspx?ApplyId=" + applyId + "&Type=1", "RwInvoice");
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
    <table width="100%">
          <tr>
            <td style="width: 50px;text-align: left;">单据号：</td>
            <td style="width: 190px;">
                <asp:TextBox ID="TbTradeCode" runat="server" Width="180px"></asp:TextBox></td>
            <td style="width: 80px; text-align: right;">审核状态：</td>
            <td style="width: 100px;">
                <asp:DropDownList ID="DdlState" DataTextField="Value" DataValueField="Key" runat="server" Width="100px"></asp:DropDownList></td>
            <td style="width: 80px;text-align: right;">发票类型：</td>
            <td style="width: 100px;">
                <asp:DropDownList ID="DdlType" DataTextField="Value" DataValueField="Key" runat="server" Width="100px"></asp:DropDownList></td>
            <td style="width: 80px;text-align: right;">票据来源：</td>
            <td style="width: 120px;">
                <asp:DropDownList ID="DdlKindType" DataTextField="Value" DataValueField="Key" runat="server" AutoPostBack="True" Width="120px"></asp:DropDownList></td>
            <td>&nbsp;</td>
            <td style="text-align: center;">
                <asp:ImageButton Style='vertical-align: middle' ID="LB_Search" runat="server" ValidationGroup="Search"
                    SkinID="SearchButton" OnClick="LbSearchClick" />
                <%--<asp:LinkButton ID="LbSearch" runat="server" CommandName="Search" OnClick="LbSearchClick">
                    <asp:Image ID="ImgSearch" runat="server" ImageAlign="AbsMiddle" SkinID="searchimagebutton" />搜索</asp:LinkButton>--%></td>
        </tr>
      </table>
<rad:RadGrid ID="RgManualInvoice" runat="server" OnNeedDataSource="RgManualInvoiceNeedDataSource">
<MasterTableView DataKeyNames="ApplyId,TradeCode">
  <CommandItemTemplate>
      
  </CommandItemTemplate><CommandItemStyle HorizontalAlign="Center" Height="26px" /> 
      <Columns> 
         <rad:GridBoundColumn DataField="TradeCode" HeaderText="单据编号" UniqueName="TradeCode"><HeaderStyle HorizontalAlign="Center" /><ItemStyle HorizontalAlign="Center" /></rad:GridBoundColumn>
          <rad:GridBoundColumn DataField="ApplyDateTime" HeaderText="申请日期" UniqueName="ApplyDateTime"><HeaderStyle HorizontalAlign="Center" /><ItemStyle HorizontalAlign="Center" /></rad:GridBoundColumn>
          <rad:GridBoundColumn DataField="Receiver" HeaderText="收货人" UniqueName="Receiver"><HeaderStyle HorizontalAlign="Center" /><ItemStyle HorizontalAlign="Center" /></rad:GridBoundColumn>
         <rad:GridTemplateColumn HeaderText="发票状态" UniqueName="ApplyState">
             <ItemTemplate>
                  <%# EnumAttribute.GetKeyName((ApplyInvoiceState)(int)Eval("ApplyState")) %>
             </ItemTemplate>
             <HeaderStyle Width="100px" HorizontalAlign="Center" />
             <ItemStyle HorizontalAlign="Center" />
          </rad:GridTemplateColumn>
         <rad:GridTemplateColumn DataField="Amount" HeaderText="发票金额" UniqueName="Amount">
             <ItemTemplate>
                 <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
             </ItemTemplate>
             <HeaderStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
         </rad:GridTemplateColumn>
         <rad:GridTemplateColumn DataField="ApplyType" HeaderText="发票类型" UniqueName="ApplyType">
             <ItemTemplate>
                 <%# EnumAttribute.GetKeyName((ApplyInvoiceType)(int)Eval("ApplyType")) %>
             </ItemTemplate>
             <HeaderStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
         </rad:GridTemplateColumn>
         <rad:GridTemplateColumn DataField="ApplyKind" HeaderText="票据分类" UniqueName="ApplyKind">
             <ItemTemplate>
                 <%# EnumAttribute.GetKeyName((ApplyInvoiceKindType)(int)Eval("ApplyKind")) %>
             </ItemTemplate>
             <HeaderStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
         </rad:GridTemplateColumn>
         <rad:GridTemplateColumn HeaderText="开具发票" UniqueName="Check">
             <ItemTemplate>
                    <asp:ImageButton ID="CheckImageButton"  Visible='<%# Convert.ToInt32(Eval("ApplyKind"))==(int)ApplyInvoiceKindType.League 
                            && (Eval("ApplyState").ToString()==string.Format("{0}",(int)ApplyInvoiceState.WaitInvoicing) || Eval("ApplyState").ToString()==string.Format("{0}",(int)ApplyInvoiceState.Invoicing)) %>'
                        CommandName="DoFoundReceipt" runat="server" SkinID="AffirmImageButton" OnClientClick='<%# "return ShowLeagueForm(\"" + Eval("ApplyId") + "\");" %>' />
                 <asp:ImageButton ID="ImageButton1"  Visible='<%# Convert.ToInt32(Eval("ApplyKind"))==(int)ApplyInvoiceKindType.Order &&
                         (Eval("ApplyState").ToString()==string.Format("{0}",(int)ApplyInvoiceState.WaitInvoicing) || Eval("ApplyState").ToString()==string.Format("{0}",(int)ApplyInvoiceState.Invoicing)) %>'
                        CommandName="DoFoundReceipt" runat="server" SkinID="AffirmImageButton" OnClientClick='<%# "return ShowOrderForm(\"" + Eval("ApplyId") + "\");" %>' />
                </ItemTemplate>
                <HeaderStyle Width="80px" HorizontalAlign="Center" />
             <ItemStyle HorizontalAlign="Center" />
         </rad:GridTemplateColumn>
         
         <rad:GridTemplateColumn HeaderText="操作记录" UniqueName="Remark">
                <ItemTemplate>
                    <asp:ImageButton ID="RemarkImageButton" 
                        CommandName="Remark" runat="server" SkinID="InsertImageButton" OnClientClick='<%# "return ShowRemarkForm(\"" + Eval("ApplyId") + "\",\"2\");" %>'
                        onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                        onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                    <div style="position: absolute;">  
                        <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative;
                            display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px;
                            font-weight: bold; height: auto; overflow: visible; word-break: break-all;" runat="server">
                            <%# Eval("ApplyRemark").ToString().Replace("\n","<br/>")%>
                        </div>
                    </div>
                </ItemTemplate>
                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
         </rad:GridTemplateColumn>
      </Columns>
</MasterTableView>
</rad:RadGrid>
<rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="RgManualInvoice">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="RgManualInvoice" LoadingPanelID="loading" />
            </UpdatedControls>
        </rad:AjaxSetting>
        <rad:AjaxSetting AjaxControlID="RAM">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="RgManualInvoice" LoadingPanelID="loading" />
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
</rad:RadAjaxLoadingPanel>
<rad:RadWindowManager ID="RWM" runat="server" Height="630px" Width="850px" ReloadOnShow="true">
    <Windows>
        <rad:RadWindow ID="RemarkWindow" runat="server" Title="管理意见" Height="500" Width="700" />
        <rad:RadWindow ID="RwInvoice" runat="server" Title="手工开票" Height="600" Width="850" />
    </Windows>
</rad:RadWindowManager>
</asp:Content>

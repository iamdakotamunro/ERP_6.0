<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="DoFoudReceive.aspx.cs" Inherits="ERP.UI.Web.DoFoudReceive" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
<script type="text/javascript">
    function ShowRemarkForm(ReceiptID) {
        window.radopen("./Windows/ShowRemarkForm.aspx?RceiptId=" + ReceiptID, "RemarkWindow");
        return false;
    }

    function ShowInvoiceForm(receiptId,service) {
        window.radopen("./Windows/DoReceiveInvoiceForm.aspx?ReceiptId=" + receiptId + "&IsService=" + service, "RwInvoice");
        return false;
    }

    function ShowAsk(receiptno) {
        if (confirm("你确定对单据" + receiptno + "开具发票吗?")) {
            //document.getElementById('<%=isdo.ClientID %>').value = "true";
            window.radopen("./Windows/DoReceiveInvoiceForm.aspx?RceiptNo=" + receiptno, "RwInvoice");
        }
        <%--else 
            document.getElementById('<%=isdo.ClientID %>').value = "false";--%>
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
<input type="hidden" id="isdo" runat="server" />        
<rad:RadGrid ID="RG_CheckInfo" runat="server" OnNeedDataSource="RG_CheckInfo_NeedDataSource" 
OnItemCommand="RG_CheckInfo_ItemCommand" OnItemDataBound="RG_CheckInfo_ItemDataBound" ShowFooter="True">
<MasterTableView DataKeyNames="ReceiptID,ReceiptNo,CompanyID,FilialeId">
  <CommandItemTemplate>
      审核状态：<asp:DropDownList ID="DDL_CheckState" DataSource='<%# BindStatusDataBound() %>' DataTextField="Value" DataValueField="Key" runat="server" SelectedValue='<%# (Status).ToString() %>'></asp:DropDownList>&nbsp;
                <rad:RadDatePicker ID="RDP_StartTime" Width="95px" runat="server"></rad:RadDatePicker>&nbsp;-&nbsp;<rad:RadDatePicker ID="RDP_EndTime" Width="95px" runat="server"></rad:RadDatePicker>&nbsp;单据号：<asp:TextBox ID="TB_CompanyFundReciptNO" runat="server"></asp:TextBox>&nbsp;<asp:LinkButton ID="LB_Search" runat="server" CommandName="Search"> <asp:Image ID="ImgSearch" runat="server" ImageAlign="AbsMiddle" SkinID="searchimagebutton" />
      搜索</asp:LinkButton></CommandItemTemplate><CommandItemStyle HorizontalAlign="Center" Height="26px" /> 
      <Columns> 
         <rad:GridBoundColumn DataField="ReceiptNo" HeaderText="单号" UniqueName="ReceiptNo"><HeaderStyle HorizontalAlign="Center" /><ItemStyle HorizontalAlign="Center" /></rad:GridBoundColumn>
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
             <HeaderStyle Width="150px" HorizontalAlign="Center" />
             <ItemStyle HorizontalAlign="Center" />
         </rad:GridTemplateColumn>
                  
         <rad:GridTemplateColumn HeaderText="申请人" UniqueName="ApplicantName">
             <ItemTemplate>
                  <asp:Label ID="personName" runat="server" Text='<%# GetPersonName(Eval("ApplicantID").ToString()) %>'></asp:Label>
             </ItemTemplate>
             <HeaderStyle Width="100px" HorizontalAlign="Center" />
             <ItemStyle HorizontalAlign="Center" />
         </rad:GridTemplateColumn>
         
         
         <rad:GridBoundColumn DataField="ApplyDateTime" HeaderText="申请日期" UniqueName="ApplyDateTime"><HeaderStyle HorizontalAlign="Center" /><ItemStyle HorizontalAlign="Center" /></rad:GridBoundColumn>
          <rad:GridTemplateColumn DataField="IsServiceFee" HeaderText="票据类型" UniqueName="IsServiceFee">
             <ItemTemplate>
                 <%# Convert.ToBoolean(Eval("IsServiceFee"))?"劳务费":"往来" %>
             </ItemTemplate>
             <HeaderStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
             <FooterStyle HorizontalAlign="Center"></FooterStyle>
              <FooterTemplate>
                  合计：
              </FooterTemplate>
         </rad:GridTemplateColumn>
         <rad:GridTemplateColumn DataField="RealityBalance" HeaderText="申请金额" UniqueName="RealityBalance">
             <ItemTemplate>
                 <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealityBalance"))%>
             </ItemTemplate>
             <HeaderStyle HorizontalAlign="Center" />
            <ItemStyle HorizontalAlign="Center" />
             <FooterStyle HorizontalAlign="Center"></FooterStyle>
              <FooterTemplate>
                  <%# ERP.UI.Web.Common.WebControl.NumberSeparator(TotalAmount)%>
              </FooterTemplate>
         </rad:GridTemplateColumn>
                  
         <rad:GridTemplateColumn HeaderText="单据状态" UniqueName="ReceiptStatus">
             <ItemTemplate>
                  <asp:Label ID="LB_ReceiptStatus" runat="server" Text='<%# GetReceiptStatus(Eval("ReceiptStatus"),Eval("InvoiceState")) %>'></asp:Label>
             </ItemTemplate>
             <HeaderStyle Width="100px" HorizontalAlign="Center" />
             <ItemStyle HorizontalAlign="Center" />
          </rad:GridTemplateColumn>
         
         <rad:GridTemplateColumn HeaderText="开具发票" UniqueName="Check">
             <ItemTemplate>
                    <asp:ImageButton ID="CheckImageButton"  Visible='<%# IsShow(Eval("ReceiptStatus").ToString(),Eval("CompanyID").ToString()) %>'
                        CommandName="DoFoundReceipt" runat="server" SkinID="AffirmImageButton" OnClientClick='<%# "return ShowInvoiceForm(\"" + Eval("ReceiptID") + "\",\"" + Eval("IsServiceFee") + "\");" %>' />
                </ItemTemplate>
                <HeaderStyle Width="80px" HorizontalAlign="Center" />
             <ItemStyle HorizontalAlign="Center" />
         </rad:GridTemplateColumn>
         
         <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Remark">
                <ItemTemplate>
                    <asp:ImageButton ID="RemarkImageButton" 
                        CommandName="Remark" runat="server" SkinID="InsertImageButton" OnClientClick='<%# "return ShowRemarkForm(\"" + Eval("ReceiptID") + "\",\"1\");" %>'
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
                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                <ItemStyle HorizontalAlign="Center" />
         </rad:GridTemplateColumn>
      </Columns>
</MasterTableView>
</rad:RadGrid>
<rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RAM_AjaxRequest">
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
        <rad:RadWindow ID="RwInvoice" runat="server" Title="开具发票" Height="500" Width="700" />
    </Windows>
</rad:RadWindowManager>
</asp:Content>

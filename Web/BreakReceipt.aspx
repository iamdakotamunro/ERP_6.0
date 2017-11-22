<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="BreakReceipt.aspx.cs" Inherits="ERP.UI.Web.BreakReceipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowRemarkForm(ReceiptID) {
                window.radopen("./Windows/ShowRemarkForm.aspx?RceiptId=" + ReceiptID, "RemarkWindow");
                return false;
            }

            function ShowAsk(receiptno) {
                if (confirm("你确定对单据" + receiptno + "核销发票吗?")) {
                    document.getElementById('<%=isdo.ClientID %>').value = "true";
                }
                else
                    document.getElementById('<%=isdo.ClientID %>').value = "false";
            }

            function ShowConfirm() {
                if (confirm("批量核销发票，确认核销?")) {
                    document.getElementById('<%=verification.ClientID %>').value = "true";
                }
                else
                    document.getElementById('<%=verification.ClientID %>').value = "false";
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
    <input type="hidden" id="verification" runat="server" />
    <rad:RadGrid ID="RG_CheckInfo" runat="server" OnNeedDataSource="RG_CheckInfo_NeedDataSource"
        OnItemCommand="RG_CheckInfo_ItemCommand" AllowMultiRowSelection="True" OnItemDataBound="RG_CheckInfo_ItemDataBound">
        <ClientSettings>
            <Selecting EnableDragToSelectRows="false" />
        </ClientSettings>
        <MasterTableView DataKeyNames="ReceiptID,ReceiptNo,CompanyID,FilialeId">
            <CommandItemTemplate>
                公司：
                <asp:DropDownList ID="DdlSaleFiliale" DataSource='<%# BindFilialeDataBound() %>'
                    DataTextField="Value" DataValueField="Key" SelectedValue='<%# SelectSaleFilialeId %>'
                    runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp;付款银行：
                <asp:DropDownList ID="DdlBankAccount" OnDataBound="BindBankDataBound" Width="120px"
                    runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp; 审核状态：<asp:DropDownList ID="DDL_CheckState" DataSource='<%# BindStatusDataBound() %>'
                    DataTextField="Value" DataValueField="Key" SelectedValue='<%# ((Int32)State).ToString() %>'
                    runat="server">
                </asp:DropDownList>
                发票管理员：<rad:RadTextBox ID="RTB_InvioceManager" runat="server" Width="95">
                </rad:RadTextBox>&nbsp;
                <rad:RadDatePicker ID="RDP_StartTime" Width="95px" runat="server">
                </rad:RadDatePicker>
                &nbsp;-&nbsp;
                <rad:RadDatePicker ID="RDP_EndTime" Width="95px" runat="server">
                </rad:RadDatePicker>
                &nbsp; 单据号：<asp:TextBox ID="TB_CompanyFundReciptNO" runat="server" Text='<%# ReceiptNo %>'></asp:TextBox>&nbsp;
                <asp:LinkButton ID="LB_Search" runat="server" CommandName="Search">
                    <asp:Image ID="ImgSearch" runat="server" ImageAlign="AbsMiddle" SkinID="searchimagebutton" />
                    搜索</asp:LinkButton>
                &nbsp;&nbsp;<asp:LinkButton ID="LbVerification" runat="server" CommandName="Verification"
                    OnClientClick='<%# "return ShowConfirm();" %>'>
                    批量核销</asp:LinkButton>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Center" Height="26px" />
            <Columns>
                <rad:GridClientSelectColumn UniqueName="column">
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridClientSelectColumn>
                <rad:GridBoundColumn DataField="ReceiptNo" HeaderText="单号" UniqueName="ReceiptNo">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
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
                <rad:GridTemplateColumn HeaderText="收付款类型" UniqueName="ReceiptType">
                    <ItemTemplate>
                        <asp:Label ID="ReceiptType" runat="server" Text='<%# GetReceiptTypeName(Eval("ReceiptType").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申请人" UniqueName="ApplicantName">
                    <ItemTemplate>
                        <asp:Label ID="personName" runat="server" Text='<%# GetPersonName(Eval("ApplicantID").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ApplyDateTime" HeaderText="申请日期" UniqueName="ApplyDateTime">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="RealityBalance" HeaderText="申请金额" UniqueName="RealityBalance">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealityBalance")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="单据状态" UniqueName="ReceiptStatus">
                    <ItemTemplate>
                        <asp:Label ID="LB_ReceiptStatus" runat="server" Text='<%# GetReceiptStatus(Eval("ReceiptStatus").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <%--                <rad:GridTemplateColumn HeaderText="付款银行" UniqueName="PayBank">
                    <ItemTemplate>
                        <asp:Label ID="LbBankName" runat="server" Text='<%# GetBankName(Eval("FilialeId").ToString(),Eval("CompanyID").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="公司" UniqueName="Filiale">
                    <ItemTemplate>
                        <asp:Label ID="LbFiliale" runat="server" Text='<%# GetFilialeName(Eval("FilialeId").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>--%>
                <rad:GridTemplateColumn HeaderText="核销发票" UniqueName="Check">
                    <ItemTemplate>
                        <asp:ImageButton ID="CheckImageButton" Visible='<%# IsShow(Eval("ReceiptStatus").ToString()) %>'
                            CommandName="DemandReceipt" runat="server" SkinID="AffirmImageButton" OnClientClick='<%# "return ShowAsk(\"" + Eval("ReceiptNo") + "\",\"1\");" %>' />
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
                                <%# Eval("Remark")%>
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
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

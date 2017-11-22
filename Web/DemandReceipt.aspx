<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="DemandReceipt.aspx.cs" Inherits="ERP.UI.Web.DemandReceipt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowRemarkForm(ReceiptID) {
                window.radopen("./Windows/ShowRemarkForm.aspx?RceiptId=" + ReceiptID, "RemarkWindow");
                return false;
            }

            function ShowAsk(receiptno) {

                if (confirm("确定单据" + receiptno + "发票已收到?")) {
                    document.getElementById('<%=isdo.ClientID %>').value = "true";
                }
                else
                    document.getElementById('<%=isdo.ClientID %>').value = "false";
            }

            function ShowAskAttestation(receiptno) {

                if (confirm("确定单据" + receiptno + "发票已认证?")) {
                    document.getElementById('<%=isdo.ClientID %>').value = "true";
                }
                else
                    document.getElementById('<%=isdo.ClientID %>').value = "false";
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
    <rad:RadGrid ID="RG_CheckInfo" runat="server" OnNeedDataSource="RgCheckInfoNeedDataSource"
        SkinID="Common_Foot" ShowFooter="true" OnItemCommand="RgCheckInfoItemCommand" OnItemDataBound="RG_CheckInfo_ItemDataBound">
        <MasterTableView DataKeyNames="ReceiptID,ReceiptNo,CompanyID,FilialeId">
            <CommandItemTemplate>
                公司：
                <asp:DropDownList ID="DdlSaleFiliale" DataSource='<%# BindFilialeDataBound() %>'
                    DataTextField="Value" DataValueField="Key" SelectedValue='<%# SelectSaleFilialeId %>'
                    runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp;付款银行：
                <asp:DropDownList ID="DdlBankAccount" OnDataBound="BindBankDataBound"  Width="120px" runat="server">
                </asp:DropDownList>
                &nbsp;&nbsp; 审核状态：<asp:DropDownList ID="DDL_CheckState" DataSource='<%# BindStatusDataBound() %>'
                    DataTextField="Value" DataValueField="Key" runat="server" SelectedValue='<%# ((Int32)Status).ToString() %>'>
                </asp:DropDownList>
                &nbsp;<rad:RadDatePicker ID="RDP_StartTime" runat="server" Width="95px">
                </rad:RadDatePicker>
                &nbsp;-&nbsp;<rad:RadDatePicker ID="RDP_EndTime" runat="server" Width="95px">
                </rad:RadDatePicker>
                &nbsp;单据号：<asp:TextBox ID="TB_CompanyFundReciptNO" runat="server"></asp:TextBox>&nbsp;
                <asp:LinkButton ID="LB_Search" runat="server" CommandName="Search">
                    <asp:Image ID="ImgSearch" runat="server" ImageAlign="AbsMiddle" SkinID="searchimagebutton" />搜索
                </asp:LinkButton>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Center" Height="26px" />
            <Columns>
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
                <rad:GridTemplateColumn HeaderText="申请人" UniqueName="ApplicantName">
                    <ItemTemplate>
                        <asp:Label ID="personName" runat="server" Text='<%# GetPersonName(Eval("ApplicantID").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="FinishDate" HeaderText="打款日期" UniqueName="FinishDate">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FooterStyle Font-Bold="true" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="RealityBalance"  HeaderText="申请金额"
                    SortExpression="RealityBalance" UniqueName="RealityBalance">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RealityBalance")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <FooterStyle Font-Bold="true" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="联系人" UniqueName="LinkMan">
                    <ItemTemplate>
                        <asp:Label ID="LB_LinkMan" runat="server" Text='<%# GetCompany(new Guid(Eval("CompanyID").ToString()), 1) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="手机" UniqueName="Moblie">
                    <ItemTemplate>
                        <asp:Label ID="LB_Moblie" runat="server" Text='<%# GetCompany(new Guid(Eval("CompanyID").ToString()), 2) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="单据状态" UniqueName="ReceiptStatus">
                    <ItemTemplate>
                        <asp:Label ID="LB_ReceiptStatus" runat="server" Text='<%# GetReceiptStatus(Eval("ReceiptStatus").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="确定已收" UniqueName="Check">
                    <ItemTemplate>
                        <asp:ImageButton ID="CheckImageButton" Visible='<%# IsShow(Eval("ReceiptStatus").ToString(),Eval("CompanyID").ToString()) %>'
                            CommandName="DemandReceipt" runat="server" SkinID="AffirmImageButton" OnClientClick='<%# "return ShowAsk(\"" + Eval("ReceiptNo") + "\",\"1\");" %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="认证操作" UniqueName="Attestation">
                    <ItemTemplate>
                        <asp:ImageButton ID="CheckImageButton2" Visible='<%# IsShowAttestation(Eval("ReceiptStatus").ToString(),Eval("CompanyID").ToString()) %>'
                            CommandName="OperateAttestation" runat="server" SkinID="AffirmImageButton" OnClientClick='<%# "return ShowAskAttestation(\"" + Eval("ReceiptNo") + "\",\"1\");" %>' />
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

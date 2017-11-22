<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="CompanyFundPayReceiptFinish.aspx.cs" Inherits="ERP.UI.Web.CompanyFundPayReceiptFinish" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script type="text/javascript">
            function RowDblClick(obj, args) {
                var ReceiptId = args.getDataKeyValue("ReceiptID");
                window.radopen("Windows/CheckReceiptForm.aspx?RceiptId=" + ReceiptId + "&type=2", "DoCheck");
            }

            function ShowRemarkForm(ReceiptID) {
                window.radopen("Windows/ShowRemarkForm.aspx?RceiptId=" + ReceiptID, "RemarkWindow");
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
            function CheckFinsh(num, cash) {
                if (confirm("您同意这“" + num + "”笔“" + cash + "”元金额支付？")) {
                    document.getElementById("<%=btnFinish.ClientID %>").click();
                }
            }
        </script>

    </rad:RadScriptBlock>
    <asp:Button ID="btnFinish" runat="server" Width="100px" Text="完成" OnClick="BtnFinishClick" Style="display: none;" />
    <rad:RadGrid ID="RG_CheckInfo" runat="server" OnNeedDataSource="RgCheckInfoNeedDataSource" AllowMultiRowSelection="True"
        SkinID="Common_Foot" ShowFooter="true" OnItemCommand="RgCheckInfoItemCommand" OnItemDataBound="RG_CheckInfo_ItemDataBound">
        <ClientSettings>
            <Selecting EnableDragToSelectRows="false" />
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>
        <MasterTableView DataKeyNames="ReceiptID,ReceiptStatus" ClientDataKeyNames="ReceiptID">
            <CommandItemTemplate>
                公司：<asp:DropDownList ID="DdlSaleFiliale" DataSource='<%# BindFilialeDataBound() %>'
                    DataTextField="Value" DataValueField="Key" SelectedValue='<%# SelectSaleFilialeId %>'
                    runat="server">
                </asp:DropDownList>
                收付款类型：<asp:DropDownList ID="DDL_ReceivePayType" DataSource='<%# BindTypeDataBound() %>' DataTextField="Value" DataValueField="Key" SelectedValue='<%# ((Int32)Type).ToString() %>' runat="server"></asp:DropDownList>&nbsp;&nbsp; 
                状态：<asp:DropDownList ID="DDL_CheckState" DataSource='<%# BindStatusDataBound() %>' DataTextField="Value" DataValueField="Key" SelectedValue='<%# ((Int32)Status).ToString() %>' runat="server"></asp:DropDownList>&nbsp;&nbsp; 
                付款银行：<asp:DropDownList ID="DDL_Bank" DataSource='<%# BindBankDataBound() %>' DataTextField="text" DataValueField="value" runat="server" Width="120px" SelectedValue='<%# BankId.ToString() %>'></asp:DropDownList>&nbsp;&nbsp;
                申请日期：<rad:RadDatePicker ID="RDP_StartTime" runat="server" Width="95px"></rad:RadDatePicker>
                &nbsp;
                -<rad:RadDatePicker ID="RDP_EndTime" runat="server" Width="95px"></rad:RadDatePicker>
                &nbsp;&nbsp; 
                打款日期：<rad:RadDatePicker ID="RDP_SExecuteTime" runat="server" Width="95px"></rad:RadDatePicker>
                &nbsp;
                -<rad:RadDatePicker ID="RDP_EExecuteTime" runat="server" Width="95px    "></rad:RadDatePicker>
                &nbsp;&nbsp; 
               <%-- 往来单位：<asp:DropDownList ID="DdlCompanyList" DataSource='<%# BindCompanyDataBound() %>' DataTextField="Value" DataValueField="Key" SelectedValue='<%# CompanyId %>' runat="server" Width="150"></asp:DropDownList>&nbsp;&nbsp;&nbsp;--%>
                收款人：<asp:TextBox ID="TbWebSite" runat="server" Width="110" Text='<%# WebSite %>'></asp:TextBox>&nbsp;&nbsp;
                单据号：<asp:TextBox ID="TB_CompanyFundReciptNO" runat="server" Width="110" Text='<%# ReceiptNo %>'></asp:TextBox>&nbsp;
                <asp:LinkButton ID="LB_Search" runat="server" CommandName="Search">
                    <asp:Image ID="ImgSearch" runat="server" ImageAlign="AbsMiddle" SkinID="searchimagebutton" />搜索
                </asp:LinkButton>&nbsp;
                <asp:LinkButton ID="LB_Finish" runat="server" CommandName="Finish">
                    <asp:Image ID="ImgFinish" runat="server" ImageAlign="AbsMiddle" SkinID="InsertImageButton" />完成
                </asp:LinkButton>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Center" Height="26px" />
            <Columns>
                <rad:GridBoundColumn DataField="ReceiptID" HeaderText="ReceiptID" UniqueName="ReceiptID" Visible="False"></rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="" HeaderStyle-Width="30px" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                    <ItemTemplate>
                        <asp:CheckBox ID="CB_Check" Checked="False" Enabled='<%# Convert.ToDateTime(Eval("FinishDate"))==DateTime.MinValue %>' runat="server" />
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <%--                <rad:GridClientSelectColumn UniqueName="column">
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridClientSelectColumn>--%>
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
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
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
                    SortExpression="RealityBalance" UniqueName="RealityBalance">
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
                <rad:GridTemplateColumn HeaderText="申请打款时间" UniqueName="ExecuteDateTime">
                    <ItemTemplate>
                        <asp:Label ID="LB_ExecuteDate" runat="server" Text='<%# DateTime.Parse(Eval("ExecuteDateTime").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("ExecuteDateTime").ToString()).ToString("yyyy年MM月dd日") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <%-- %><rad:GridTemplateColumn HeaderText="付款银行" UniqueName="PayBank">
                    <ItemTemplate>
                        <asp:Label ID="LbBankName" runat="server" Text='<%# GetBankName(Eval("FilialeId").ToString(),Eval("CompanyID").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>--%>
                <rad:GridTemplateColumn HeaderText="交易流水号" UniqueName="FlowNo">
                    <ItemTemplate>
                        <asp:Label ID="LbFlowNo" runat="server" Text='<%# Eval("DealFlowNo")==null?string.Empty:Eval("DealFlowNo").ToString() %>'></asp:Label>
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
                <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Remark">
                    <ItemTemplate>
                        <asp:ImageButton ID="RemarkImageButton" CommandName="Remark" runat="server" SkinID="InsertImageButton"
                            OnClientClick='<%# "return ShowRemarkForm(\"" + Eval("ReceiptID") + "\",\"1\");" %>'
                            onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                            onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                        <div style="position: absolute;">
                            <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative; display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px; font-weight: bold; height: auto; overflow: visible; word-break: break-all;"
                                runat="server">
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
            <rad:AjaxSetting AjaxControlID="btnFinish">
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
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager ID="RWM" runat="server" Height="630px" Width="800px" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="RemarkWindow" runat="server" Title="管理意见" Height="500" Width="700" />
            <rad:RadWindow ID="DoCheck" runat="server" Title="收付款执行" Height="500" Width="900" />
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

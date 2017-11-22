<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PayCheckList.aspx.cs" Inherits="ERP.UI.Web.PayCheckList"
    MasterPageFile="~/MainMaster.master" %>

<asp:Content ID="PayCheck" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/jquery.js"></script>
        <script src="My97DatePicker/WdatePicker.js"></script>
        <script type="text/javascript" src="/JavaScript/telerik.js"></script>
        <script type="text/javascript">
            function ShowRemarkForm(ReceiptID) {
                window.radopen("./Windows/ShowRemarkForm.aspx?RceiptId=" + ReceiptID, "RemarkWindow");
                return false;
            }

            function ShowCheckForm(ReceiptID) {
                window.radopen("./Windows/CheckReceiptDetail.aspx?RceiptId=" + ReceiptID, "DoCheck");
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
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <rad:RadTreeView ID="RT_CompanyClass" runat="server" SkinID="Common" AutoPostBack="true"
                    CausesValidation="True" OnNodeClick="RtCompanyClassNodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">公司：</td>
                        <td>
                            <asp:DropDownList ID="DdlSaleFiliale" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">往来单位：</td>
                        <td>
                            <rad:RadComboBox ID="RCB_CompanyList" runat="server" AutoPostBack="true" Width="180px" AllowCustomText="true" EnableLoadOnDemand="true"
                                Height="200" OnItemsRequested="RCB_CompanyList_OnItemsRequested">
                            </rad:RadComboBox>
                        </td>
                        <td style="text-align: right;">申请日期：</td>
                        <td>
                            <asp:TextBox ID="txt_StartTime" runat="server" Width="100px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',maxDate:$('input[id$=rdp_EndTime]').val()})"></asp:TextBox>
                            至
                            <asp:TextBox ID="txt_EndTime" runat="server" Width="100px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',minDate:$('input[id$=rdp_StartTime]').val()})"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">审核状态：</td>
                        <td>
                            <asp:DropDownList ID="DDL_CheckState" runat="server">
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">付款类型：</td>
                        <td>
                            <asp:DropDownList ID="DDL_PayType" runat="server">
                                <asp:ListItem Text="" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="按日期付款" Value="0"></asp:ListItem>
                                <asp:ListItem Text="按入库单付款" Value="1"></asp:ListItem>
                                <asp:ListItem Text="按采购单付款" Value="2"></asp:ListItem>
                                <asp:ListItem Text="预付款" Value="3"></asp:ListItem>
                                <asp:ListItem Text="排除按日期付款" Value="4"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">单据号：</td>
                        <td>
                            <asp:TextBox ID="TB_CompanyFundReciptNO" runat="server" Width="120px"> </asp:TextBox>
                        </td>
                        <td style="text-align: right;">付款期：</td>
                        <td>
                            <asp:TextBox ID="txt_PaymentDate" runat="server" Width="100px" onfocus="this.blur();" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM'})"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">申请人：</td>
                        <td>
                            <rad:RadComboBox ID="rcb_Applicant" Width="172px" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入申请人" runat="server" Height="200px" OnItemsRequested="rcb_Applicant_ItemsRequested"></rad:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="8" style="text-align: center;">
                            <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
                        </td>
                    </tr>
                </table>
                <rad:RadGrid ID="RG_CheckInfo" runat="server" SkinID="Common_Foot" ShowFooter="true"
                    OnNeedDataSource="RgCheckInfoNeedDataSource">
                    <MasterTableView>
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle Height="0px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="ReceiptNo" HeaderText="单号" UniqueName="ReceipNo">
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
                            <rad:GridBoundColumn DataField="ApplyDateTime" HeaderText="申请日期" UniqueName="ApplyDateTime">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="付款期">
                                <ItemTemplate>
                                    <%# Eval("PaymentDate").Equals(DateTime.MinValue)?"":Convert.ToDateTime(Eval("PaymentDate")).ToString("yyyy-MM")%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <FooterStyle Font-Bold="true" HorizontalAlign="Center" />
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
                            <rad:GridTemplateColumn HeaderText="单据状态" UniqueName="ReceiptStatus">
                                <ItemTemplate>
                                    <asp:Label ID="LB_ReceiptStatus" runat="server" Text='<%# GetReceiptStatus(Eval("ReceiptStatus").ToString()) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审核" UniqueName="Check">
                                <ItemTemplate>
                                    <asp:ImageButton ID="CheckImageButton" Visible='<%# IsShow(Eval("ReceiptStatus").ToString(),Eval("RealityBalance").ToString(),Eval("CompanyID").ToString()) %>'
                                        CommandName="Check" runat="server" SkinID="AffirmImageButton" OnClientClick='<%# "return ShowCheckForm(\"" + Eval("ReceiptID") + "\");" %>' />
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
            </td>
        </tr>
    </table>
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
            <rad:AjaxSetting AjaxControlID="RT_CompanyClass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_CheckInfo" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager ID="RWM" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="RemarkWindow" runat="server" Title="管理意见" Height="500" Width="700" />
            <rad:RadWindow ID="DoCheck" runat="server" Title="单据审核" Height="500" Width="900" />
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="CheckHistory.aspx.cs" Inherits="ERP.UI.Web.CheckHistory" %>
<%@ Import Namespace="ERP.Enum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RadScriptBlock1" runat="server">
        <script type="text/javascript">

            function ShowCheckingForm(checkId, state) {
                if (checkId != "00000000-0000-0000-0000-000000000000" && state >= 2) {
                    window.radopen("./Windows/ShowConfirmCheckForm.aspx?CheckId=" + checkId, "ShowConfirmForm");
                }
                return false;
            }

            function ShowCheckHistoryForm(receiptNo) {
                window.radopen("./Windows/CheckHistoryForm.aspx?ReceiptNo=" + receiptNo, "RW1");
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

        </script>
    </rad:RadScriptBlock>
    <table class="StagePanel">
        <tr>
            <td>
                &nbsp;
            </td>
            <td align="right">
                起始时间：
            </td>
            <td style="width: 110">
                <rad:RadDatePicker ID="RDP_StartDate" runat="server" Width="110px">
                </rad:RadDatePicker>
            </td>
            <td align="right">
                结束时间：
            </td>
            <td style="width: 110">
                <rad:RadDatePicker ID="RDP_EndDate" runat="server" Width="110px">
                </rad:RadDatePicker>
            </td>
            <td align="right">
                对账人：
            </td>
            <td>
                <asp:TextBox ID="txtChecker" runat="server" Width="100px"></asp:TextBox>
            </td>
            <td align="right">
                对账单位：
            </td>
            <td>
                <rad:RadComboBox ID="RcbCompanyList" runat="server" DropDownWidth="150px" AutoPostBack="False"
                    AllowCustomText="true" EnableLoadOnDemand="true" Height="200" EmptyMessage="选择快递公司"
                    CausesValidation="false" Filter="StartsWith">
                </rad:RadComboBox>
            </td>
            <td align="right">
                是否对账:
            </td>
            <td>
                <asp:DropDownList runat="server" ID="DdlState" >
                    <Items>
                        <asp:ListItem Text=" " Value="-1"></asp:ListItem>
                        <asp:ListItem Text="未对账" Value="0" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="已对账" Value="1"></asp:ListItem>
                    </Items>
                </asp:DropDownList>
            </td>
            <td>
                <asp:ImageButton ID="LB_Search" runat="server" SkinID="SearchButton" OnClick="LbSearchClick" />&nbsp;&nbsp;
            </td>
            <td>
                <asp:CheckBox ID="CKB_Merger" runat="server" Checked="True" Text="公司账务合并" />
            </td>
            <td>
                <asp:LinkButton runat="server" ID="IbFinishControl" OnClick="OnFinishClick">生成收付款</asp:LinkButton>&nbsp;&nbsp;
            </td>
            <td>
                <asp:ImageButton runat="server" ID="IbDelete" ImageUrl="App_Themes/default/Images/Delete.gif"
                    OnClick="OnDeleteClick" AlternateText="删除" />
            </td>
        </tr>
        <tr>
            <td colspan="14">
                <rad:RadGrid ID="rgCheckHistory" runat="server" SkinID="Common_Foot" AllowMultiRowSelection="True"
                    MasterTableView-CommandItemDisplay="None" OnNeedDataSource="RgCheckHistoryNeedDataSource">
                    <GroupPanel Visible="True">
                    </GroupPanel>
                    <ClientSettings>
                        <Selecting EnableDragToSelectRows="false" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="CheckId" CurrentResetPageIndexAction="SetPageIndexToLast"
                        ClientDataKeyNames="CheckId">
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridClientSelectColumn UniqueName="column">
                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridClientSelectColumn>
                            <rad:GridBoundColumn DataField="CheckUser" HeaderText="对账人" UniqueName="CheckUser">
                                <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="CheckType" HeaderText="对账类型" UniqueName="CheckType">
                                <ItemTemplate>
                                    <%# Convert.ToInt32(Eval("CheckType"))==1?"运费":"代收" %>
                                </ItemTemplate>
                                <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="对账金额" UniqueName="ShowReceivable">
                                <ItemTemplate>
                                    <%# ShowAccountReceivable(Eval("CheckId"), Eval("CheckDataState"))%>
                                </ItemTemplate>
                                <HeaderStyle Width="70px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="CompanyName" HeaderText="对账单位" UniqueName="CompanyName">
                                <HeaderStyle Width="130px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="CheckDescription" HeaderText="描述" UniqueName="CheckDescription">
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="CheckCreateDate" HeaderText="对账日期" UniqueName="CheckCreateDate">
                                <HeaderStyle Width="140px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="OriginalFilePath" HeaderText="对账原始表" UniqueName="OriginalFilePath">
                                <ItemTemplate>
                                    <a href='Windows/DownloadPage.aspx?tag=f1&fullname=<%# HttpUtility.UrlEncode(Eval("OriginalFilePath").ToString()) %>'>
                                        下载</a>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="ContrastFilePath" HeaderText="双方对比表" UniqueName="ContrastFilePath">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="LbCreate1" Text="生成中" Visible='<%# !IsVisible(Eval("ContrastFilePath"),Eval("CheckDataState"),1) %>'
                                        Enabled="False"></asp:LinkButton>
                                    <span id="Span1" runat="server" visible='<%# IsVisible(Eval("ContrastFilePath"),Eval("CheckDataState"),1) %>'>
                                        <a href='Windows/DownloadPage.aspx?tag=f2&fullname=<%# Eval("ContrastFilePath")==null  || string.IsNullOrEmpty(Eval("ContrastFilePath").ToString())?string.Empty:HttpUtility.UrlEncode(Eval("ContrastFilePath").ToString()) %>'>
                                            下载</a></span>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="ConfirmFilePath" HeaderText="财务确认表" UniqueName="ConfirmFilePath">
                                <ItemTemplate>
                                    <span id="span3" runat="server" visible='<%# Convert.ToInt32(Eval("CheckDataState"))<=(int)CheckDataState.Contrasted %>'>
                                        <asp:LinkButton runat="server" ID="LbCreate2" Text="上传" OnClientClick='<%# "return ShowCheckingForm(\"" + Eval("CheckId")+ "\",\""+Eval("CheckDataState")+"\");" %>'
                                            Enabled='<%# Convert.ToInt32(Eval("CheckDataState"))==(int)CheckDataState.Contrasted %>'></asp:LinkButton><font
                                                color="red"><%# ShowIsUploadMsg(Eval("ConfirmFilePath"), Eval("CheckDataState"))%></font></span>
                                    <span runat="server" visible='<%# Convert.ToInt32(Eval("CheckDataState"))>=(int)CheckDataState.Confirmed %>'>
                                        <a href='Windows/DownloadPage.aspx?tag=f3&fullname=<%# Eval("ConfirmFilePath")==null  || string.IsNullOrEmpty(Eval("ConfirmFilePath").ToString())?string.Empty:HttpUtility.UrlEncode(Eval("ConfirmFilePath").ToString()) %>'>
                                            下载</a></span>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="FinishFilePath" HeaderText="对账结果表" UniqueName="FinishFilePath">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" ID="LbCreate3" Text='<%# GetStateString(Eval("CheckDataState")) %>'
                                        Visible='<%# !IsVisible(Eval("FinishFilePath"),Eval("CheckDataState"),3) %>'
                                        Enabled="False"></asp:LinkButton>
                                    <span id="Span2" runat="server" visible='<%# IsVisible(Eval("FinishFilePath"),Eval("CheckDataState"),3) %>'>
                                        <a href='Windows/DownloadPage.aspx?tag=f4&fullname=<%# Eval("FinishFilePath")==null  || string.IsNullOrEmpty(Eval("ConfirmFilePath").ToString())?string.Empty:HttpUtility.UrlEncode(Eval("FinishFilePath").ToString()) %>'>
                                            下载</a><font color="red"><%# IsCheckedString(Eval("CheckDataState"))%></font></span>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                    <ExportSettings>
                        <Pdf PageBottomMargin="" PageFooterMargin="" PageHeaderMargin="" PageHeight="11in"
                            PageLeftMargin="" PageRightMargin="" PageTopMargin="" PageWidth="8.5in" />
                    </ExportSettings>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgCheckHistory" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IbDelete">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgCheckHistory" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IbFinishControl">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgCheckHistory" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadWindowManager ID="WMReckoning" runat="server" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="ShowConfirmForm" Width="350" Height="150" runat="server" Title="对账确认" />
            
            <rad:RadWindow ID="RW1" Width="520" Height="300" runat="server" Title="收款单据填写收款公司和账号" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

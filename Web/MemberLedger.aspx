<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="MemberLedger.aspx.cs" Inherits="ERP.UI.Web.MemberLedger"
    MasterPageFile="~/MainMaster.master" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <!--当前页面：会员总账-->
    <table style="float: right;">
        <tr>
            <td class="ShortFromRowTitle">
                销售公司:
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" Width="150px"
                    Height="150px" AutoPostBack="True" EmptyMessage="请选择销售公司">
                </rad:RadComboBox>
            </td>
            <td class="ShortFromRowTitle">
                时间:
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadDatePicker ID="RDP_StartTime" runat="server" Width="100px">
                </rad:RadDatePicker>
                -
                <rad:RadDatePicker ID="RDP_EndTime" runat="server" Width="100px">
                </rad:RadDatePicker>
            </td>
            <td class="ShortFromRowTitle">
                资金流向:
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadComboBox ID="RCB_FlowType" runat="server" UseEmbeddedScripts="false"
                        Width="150px" Height="100px" AutoPostBack="True" EmptyMessage="请选择销售平台">
                    </rad:RadComboBox>
            </td>
            <td class="ShortFromRowTitle">
                单据编号:
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadTextBox runat="server" EmptyMessage="单据编号" ID="RTB_TradeCode" Width="150px" />
            </td>
            <td>
                <asp:ImageButton ID="IB_Search" runat="server" SkinID="SearchButton" OnClick="IB_Search_Click"
                    ValidationGroup="Search" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RGMemberReckoning" runat="server" SkinID="CustomPaging" OnNeedDataSource="RGMemberReckoning_NeedDataSource">
        <MasterTableView DataKeyNames="" ClientDataKeyNames="">
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <CommandItemTemplate>
            </CommandItemTemplate>
            <Columns>
                <rad:GridBoundColumn DataField="TradeCode" HeaderText="单据编号" UniqueName="TradeCode">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>

                <rad:GridTemplateColumn HeaderText="公司" UniqueName="FilialeName">
                    <ItemTemplate>
                        <%#GetFromSourceNameById(Eval("SaleFilialeID"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" Width="150px" />
                </rad:GridTemplateColumn>

                <rad:GridBoundColumn DataField="OriginalTradeCode" HeaderText="原始单据号" UniqueName="OriginalTradeCode">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>

                <rad:GridBoundColumn DataField="CreateTime" HeaderText="创建时间" UniqueName="CreateTime">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                
                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="存入充值">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="存入补偿">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="存入赠送">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                
                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="支出充值">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="支出补偿">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="支出赠送">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                
                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="充值总额">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="补偿总额">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="赠送总额">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="Amount" UniqueName="Amount" HeaderText="支出/收入（元）" Visible="false">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn DataField="CurrentBalance" UniqueName="CurrentBalance" HeaderText="当前余额（元）" Visible="false">
                    <HeaderStyle HorizontalAlign="Center" Width="150px" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                   <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("CurrentBalance"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridBoundColumn DataField="Description" HeaderText="备注" UniqueName="Description">
                    <HeaderStyle Width="350px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server"  >
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGMemberReckoning" LoadingPanelID="loading"/>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_FlowType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_FlowType" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGMemberReckoning" LoadingPanelID="loading"/>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SaleFiliale"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

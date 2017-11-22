<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="ApprovaInvoiceManager.aspx.cs" Inherits="ERP.UI.Web.Invoices.ApprovaInvoiceManager" %>
<%@ Import Namespace="ERP.Enum.ApplyInvocie" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowRemarkForm(applyId,type) {
                window.radopen("../Windows/ShowRemarkForm.aspx?ApplyId=" + applyId+"&Type="+type, "RemarkWindow");
                return false;
            }

            function ShowAuditForm(applyId) {
                window.radopen("./ApprovalLeagueInvoiceForm.aspx?ApplyId=" + applyId+"&Type=0", "RwInvoice");
                return false;
            }

            function ShowOrderAuditForm(applyId) {
                window.radopen("./ApprovalOrderInvoiceForm.aspx?ApplyId=" + applyId + "&Type=0", "RwInvoice");
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
        
        <style type="text/css">
            .title td{ text-align: left;}
        </style>
    </rad:RadScriptBlock>
    <table width="100%" class="title">
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
                <asp:DropDownList ID="DdlKindType" DataTextField="Value" DataValueField="Key" runat="server" AutoPostBack="True" OnSelectedIndexChanged="DdlKindTypeSelectedIndexChanged" Width="120px"></asp:DropDownList></td>
            <td style="width: 80px; text-align: right;"><asp:Label runat="server" ID="LbSaleFiliale" Text="销售公司："></asp:Label></td>
            <td style="width: 150px;">
                <asp:DropDownList ID="DdlSaleFiliale" DataTextField="Name" DataValueField="ID" runat="server" Width="150" AutoPostBack="True" OnSelectedIndexChanged="DdlSaleFilialeSelectedIndexChanged"></asp:DropDownList></td>
            <td style="width: 80px; text-align: right;"><asp:Label runat="server" ID="LbSalePlatform" Text="加盟店名："></asp:Label></td>
            <td style="width: 150px;">
                <asp:DropDownList ID="DdlSalePlatform" DataTextField="Name" DataValueField="ID" runat="server" Width="150"></asp:DropDownList></td>
            <td>&nbsp;</td>
            <td style="text-align: center;">
                <asp:ImageButton Style='vertical-align: middle' ID="LB_Search" runat="server" ValidationGroup="Search"
                    SkinID="SearchButton" OnClick="LbSearchClick" /></td>
        </tr>
    </table>
    <rad:RadGrid ID="RgInvoiceAudit" runat="server" OnNeedDataSource="RgInvoiceAuditNeedDataSource">
        <MasterTableView DataKeyNames="ApplyId,ApplyType" CommandItemDisplay="None">
            <Columns>
                <rad:GridBoundColumn DataField="TradeCode" HeaderText="订单编号" UniqueName="TradeCode">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ApplyDateTime" HeaderText="申请日期" UniqueName="ApplyDateTime">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Receiver" HeaderText="收货人" UniqueName="Receiver">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="申请单位" DataField="TargetId" UniqueName="TargetId">
                    <ItemTemplate>
                        <asp:Label ID="LbFiliale" runat="server" Text='<%# Convert.ToInt32(Eval("ApplyKind"))==(int)ApplyInvoiceKindType.League?GetFilialeName(Eval("TargetId").ToString()):"" %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="IsServiceFee" HeaderText="发票类型" UniqueName="ApplyType">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((ApplyInvoiceType)Convert.ToInt32(Eval("ApplyType"))) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="发票状态" UniqueName="ApplyState">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((ApplyInvoiceState)Convert.ToInt32(Eval("ApplyState"))) %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="Amount" HeaderText="余额支付金额" UniqueName="Amount">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="Amount" HeaderText="发票金额" UniqueName="Amount" Display="False">
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("Amount"))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                

                <rad:GridTemplateColumn HeaderText="操作" UniqueName="Check">
                    <ItemTemplate>
                        <asp:ImageButton ID="CheckImageButton" Visible='<%# Convert.ToInt32(Eval("ApplyState"))==(int)ApplyInvoiceState.WaitAudit && Convert.ToInt32(Eval("ApplyKind"))==(int)ApplyInvoiceKindType.League%>'
                             runat="server" SkinID="AffirmImageButton" OnClientClick='<%# "return ShowAuditForm(\"" + Eval("ApplyId") + "\");" %>' />
                        <asp:ImageButton ID="CkOrder" Visible='<%# Convert.ToInt32(Eval("ApplyState"))==(int)ApplyInvoiceState.WaitAudit && Convert.ToInt32(Eval("ApplyKind"))==(int)ApplyInvoiceKindType.Order %>'
                             runat="server" SkinID="AffirmImageButton" OnClientClick='<%# "return ShowOrderAuditForm(\"" + Eval("ApplyId") + "\");" %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Remark">
                    <ItemTemplate>
                        <asp:ImageButton ID="RemarkImageButton"
                            CommandName="Remark" runat="server" SkinID="InsertImageButton" OnClientClick='<%# "return ShowRemarkForm(\"" + Eval("ApplyId") + "\",\"2\");" %>'
                            onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                            onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                        <div style="position: absolute;">
                            <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative; display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px; font-weight: bold; height: auto; overflow: visible; word-break: break-all;"
                                runat="server">
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
            <rad:AjaxSetting AjaxControlID="LB_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgInvoiceAudit" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DdlType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgInvoiceAudit" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="DdlSaleFiliale" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="DdlSalePlatform" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="DdlSaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DdlSalePlatform" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgInvoiceAudit" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager ID="RWM" runat="server" Height="630px" Width="800px" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="RemarkWindow" runat="server" Title="管理意见" Height="500" Width="600" />
            <rad:RadWindow ID="RwInvoice" runat="server" Title="发票申请审核" Height="500" Width="800" />
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

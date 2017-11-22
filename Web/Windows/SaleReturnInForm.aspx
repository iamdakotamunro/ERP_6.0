<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SaleReturnInForm.aspx.cs" Inherits="ERP.UI.Web.Windows.SaleReturnInForm" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style type="text/css">
        .warehousecss div {
            float: left;
            padding-right: 5px;
        }
    </style>
</head>
<body scroll="no">
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/jquery.js" type="text/javascript"></script>
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/common.js" type="text/javascript"></script>
            <script src="../My97DatePicker/WdatePicker.js"></script>
        </rad:RadScriptBlock>
        <asp:Panel ID="Panel_SemiStockInForm" runat="server">
            <div class="StagePanel">
                <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0"
                    Width="100%">
                    <asp:TableRow>
                        <asp:TableCell CssClass="ControlTools">
                            <asp:LinkButton ID="LB_Inster" runat="server" OnClick="btn_InsterStock">
                                <asp:Image ID="IB_Inster" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                                    BorderStyle="None" />保存
                            </asp:LinkButton>
                            <asp:Label ID="Lab_InsterSpace" runat="server">&nbsp;</asp:Label>
                            <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()"
                                SkinType="Cancel" Text="取消"></Ibt:ImageButtonControl>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <table class="PanelArea">
                    <tr>
                        <td>申请时间：
                        </td>
                        <td>
                            <asp:TextBox ID="txt_DateCreated" runat="server" ReadOnly="true" SkinID="LongInput"></asp:TextBox>
                        </td>
                        <td>操 作 人：
                        </td>
                        <td>
                            <asp:TextBox ID="txt_Transactor" runat="server" ReadOnly="true" SkinID="LongInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>供 应 商：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_CompanyId" runat="server" AccessKey="T" Filter="Contains"
                                MarkFirstMatch="true" DataTextField="CompanyName" DataValueField="CompanyId"
                                Width="250px" AutoPostBack="True" OnSelectedIndexChanged="Rcb_CompanyOnSelectedIndexChanged">
                            </rad:RadComboBox>
                        </td>

                    </tr>
                    <tr>
                        <td>入库仓储：
                        </td>
                        <td class="warehousecss">
                            <rad:RadComboBox ID="RCB_Warehouse" runat="server" MarkFirstMatch="True" ShowToggleImage="True"
                                Width="123px" OnSelectedIndexChanged="RcbInStockOnSelectedIndexChanged" AutoPostBack="true">
                            </rad:RadComboBox>
                            <rad:RadComboBox ID="RCB_StorageAuth" runat="server" AccessKey="T" ShowToggleImage="True"
                                DataTextField="StorageTypeName" DataValueField="StorageType" Width="121px"
                                OnSelectedIndexChanged="RcbStorageAuthOnSelectedIndexChanged" AutoPostBack="true">
                            </rad:RadComboBox>
                        </td>
                        <td>公司：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_HostingFilialeAuth" runat="server" AccessKey="T" ShowToggleImage="True"
                                DataTextField="HostingFilialeName" DataValueField="HostingFilialeId" Width="250px" OnSelectedIndexChanged="RCB_HostingFilialeAuth_OnSelectedIndexChanged" AutoPostBack="true">
                            </rad:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td>采购负责人：
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="TB_Personnel" runat="server" SkinID="LongInput" Enabled="false"></asp:TextBox>
                        </td>

                    </tr>
                    <tr>
                        <td>备注说明：
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txt_Description" runat="server" Width="74%" MaxLength="100" Height="30px" TextMode="MultiLine"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td colspan="4">
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td>销售出库单号：</td>
                        <td colspan="3">
                            <rad:RadComboBox ID="RCB_CreateNo" runat="server" Width="250px" Height="200px" DataTextField="Value"
                                DataValueField="Key" UseEmbeddedScripts="false" AllowCustomText="true" EnableLoadOnDemand="True"
                                AutoPostBack="true" CausesValidation="False" OnItemsRequested="Rcb_CreateNoOnItemsRequested"
                                OnSelectedIndexChanged="Rcb_CreateNoOnSelectedIndexChanged">
                            </rad:RadComboBox>
                        </td>
                    </tr>
                </table>
                <rad:RadGrid ID="RGGoods" AllowPaging="False" Height="300px" runat="server" SkinID="Common"
                    OnNeedDataSource="RgGoodsNeedDataSource" OnDeleteCommand="RgGoodsDeleteCommand">
                    <ClientSettings>
                        <Resizing AllowColumnResize="True"></Resizing>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                        <ClientMessages DragToResize="调整大小" />
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="GoodsId,RealGoodsId,UnitPrice">
                        <CommandItemTemplate>
                            <asp:LinkButton ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid">
                                <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />刷新
                            </asp:LinkButton>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode" Visible="False">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>

                            <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="Quantity" HeaderText="入库数" UniqueName="Quantity">
                                <ItemTemplate>
                                    <asp:TextBox ID="TB_Quantity" runat="server" Text='<%# Bind("Quantity") %>' SkinID="ShortInput"
                                        onblur='<%# "SumPrice(\"" + Container.FindControl("TB_Quantity").ClientID + "\",\"" + Container.FindControl("TB_UnitPrice").ClientID + "\")" %>'>
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RFVQuantity" runat="server" ControlToValidate="TB_Quantity"
                                        ErrorMessage="数量必须填写" Text="*">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="REVnums" runat="server" ControlToValidate="TB_Quantity"
                                        ValidationExpression="^\d+$" ErrorMessage="*">
                                    </asp:RegularExpressionValidator>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                                <ItemTemplate>
                                    <asp:TextBox ID="TB_UnitPrice" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString())) %>'
                                        onblur='<%# "SumPrice(\"" + Container.FindControl("TB_Quantity").ClientID + "\",\"" + Container.FindControl("TB_UnitPrice").ClientID + "\")" %>'
                                        SkinID="ShortInput">
                                    </asp:TextBox>
                                    <asp:RegularExpressionValidator ID="Rev_UnitPrice" runat="server" ControlToValidate="TB_UnitPrice"
                                        Text="*" ErrorMessage="请输入正数!" Display="Static" ValidationGroup="TB_UnitPrice,LB_Inster,LB_ModifyStock"
                                        ValidationExpression="^(([0-9]+[\.]?[0-9]+)|[1-9])$"></asp:RegularExpressionValidator>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="是否赠品" UniqueName="type">
                                <ItemTemplate>
                                    <%# Convert.ToDecimal(Eval("UnitPrice"))==0?"赠品":"" %>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="ApprovalNO" HeaderText="批文号" UniqueName="ApprovalNO">
                                <ItemTemplate>
                                    <asp:Label ID="lbApprovalNO" runat="server" Text='<%# Bind("ApprovalNO") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Description" HeaderText="备注" Visible="false" UniqueName="Description">
                                <ItemTemplate>
                                    <asp:TextBox ID="TB_Description" runat="server" Text='<%# Bind("Description") %>'>
                                    </asp:TextBox>
                                </ItemTemplate>
                                <HeaderStyle Width="140px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn UniqueName="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButton1" SkinID="DeleteImageButton" runat="server" CommandName="Delete" />
                                </ItemTemplate>
                                <HeaderStyle Width="35px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                                Visible="False">
                            </rad:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
                <table class="PanelArea">
                    <tr>
                        <td class="AreaRowTitle">合计数量：
                        </td>
                        <td class="AreaRowInfo">
                            <asp:Label ID="Lab_TotalNumber" runat="server" Text="0"></asp:Label>
                        </td>
                        <td class="AreaRowTitle">合计金额：
                        </td>
                        <td class="AreaRowInfo">
                            <asp:Label ID="Lab_TotalAmount" runat="server" Text="0"></asp:Label>
                        </td>
                    </tr>
                </table>
            </div>
        </asp:Panel>

        <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RCB_CompanyId">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_CreateNo" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RGGoods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalNumber"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalAmount"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_StorageAuth" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RCB_HostingFilialeAuth" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RCB_CreateNo" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RGGoods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalNumber"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalAmount"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_HostingFilialeAuth">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_CreateNo" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_StorageAuth">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_HostingFilialeAuth" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_CreateNo">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGGoods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalNumber"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalAmount"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

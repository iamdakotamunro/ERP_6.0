<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditGoodsPurchaseInForm.aspx.cs" Inherits="ERP.UI.Web.Windows.EditGoodsPurchaseInForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
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
            <script type="text/javascript">
                function Total() {
                    var objTable = $find("<%=RGGoods.ClientID%>");
                    var mstview = objTable.get_masterTableView();
                    var aInput = mstview.get_element().getElementsByTagName("input");
                    var totalNumber = 0;
                    for (var j = 0; j < aInput.length; j += 1) {
                        if (aInput[j].name.indexOf("$TB_Quantity") !== -1) {
                            totalNumber += Number(aInput[j].value);
                        }
                    }
                    document.getElementById("<%=Lab_TotalNumber.ClientID %>").innerHTML = totalNumber;
                }
            </script>
        </rad:RadScriptBlock>
        <asp:Panel ID="Panel_SemiStockInForm" runat="server">
            <div class="StagePanel">

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
                        <td>入 库 仓：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_Warehouse" runat="server" ShowToggleImage="True" Enabled="False"
                                Width="250px">
                            </rad:RadComboBox>
                        </td>
                        <td>选择采购单：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_Purchasing" runat="server" AccessKey="T" ShowToggleImage="True"
                                AllowCustomText="true" DataTextField="PurchasingNo" DataValueField="PurchasingId"
                                Width="250px" EnableLoadOnDemand="True"
                                Height="200px" Enabled="False">
                            </rad:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td>供 应 商：
                        </td>
                        <td>
                            <asp:TextBox ID="txt_CompanyId" runat="server" ReadOnly="true" SkinID="LongInput"></asp:TextBox>
                            <asp:HiddenField ID="HF_CompanyId" runat="server" />
                        </td>
                        <td>收货公司：
                        </td>
                        <td>
                            <asp:TextBox ID="txt_Filiale" runat="server" ReadOnly="true" SkinID="LongInput"></asp:TextBox>
                            <asp:HiddenField ID="HF_FilialeId" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td>入 库 储：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_StorageAuth" runat="server" AccessKey="T" ShowToggleImage="True"
                                DataTextField="StorageTypeName" DataValueField="StorageType" Width="250px"
                                OnSelectedIndexChanged="RcbStorageAuthOnSelectedIndexChanged" AutoPostBack="true">
                            </rad:RadComboBox>
                        </td>
                        <td>公司：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_HostingFilialeAuth" runat="server" AccessKey="T" ShowToggleImage="True"
                                DataTextField="HostingFilialeName" DataValueField="HostingFilialeId"
                                Width="270px" Enabled="False">
                            </rad:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td>采购负责人：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_Personnel" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox>
                        </td>
                        <td>原始单号：
                        </td>
                        <td>
                            <asp:TextBox ID="txt_OriginalCode" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>备注说明：
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="txt_Description" runat="server" Width="89%" MaxLength="100" Height="30px" TextMode="MultiLine"></asp:TextBox>
                            <asp:TextBox ID="TB_DescriptionVisible" runat="server" Visible="False"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <rad:RadGrid ID="RGGoods" AllowPaging="False" Height="280px" runat="server" SkinID="Common"
                    OnNeedDataSource="RgGoodsNeedDataSource" OnItemDataBound="RgGoods_OnItemDataBound" OnDeleteCommand="RGGoods_DeleteCommand">
                    <ClientSettings>
                        <Resizing AllowColumnResize="True"></Resizing>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                        <ClientMessages DragToResize="调整大小" />
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="GoodsId,RealGoodsId,GoodsCode,GoodsName,Specification,Units,Quantity,UnitPrice,ApprovalNO">
                        <CommandItemTemplate>
                            <asp:LinkButton ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid">
                                <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />刷新
                            </asp:LinkButton>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="24px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                                Visible="False">
                            </rad:GridBoundColumn>
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
                                    <asp:TextBox ID="TB_Quantity" runat="server" Text='<%# Bind("Quantity") %>' SkinID="ShortInput" onblur="Total();">
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
                                    <asp:Label ID="lbUnitPrice" runat="server" Text='<%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString())) %>'></asp:Label>
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
                            <rad:GridButtonColumn HeaderText="操作" CommandName="Delete" Text="删除" ConfirmText="确实要删除吗？"
                                UniqueName="Delete" ButtonType="ImageButton">
                                <HeaderStyle Width="35px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridButtonColumn>
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
            <div style="text-align: center; padding-top: 10px;">
                <asp:Button ID="Button1" runat="server" Text="  重送  " OnClick="btn_InsterStock" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="Button2" runat="server" Text="  取消  " OnClientClick="return CancelWindow()" />
            </div>
        </asp:Panel>
        <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_Purchasing"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RCB_StorageAuth"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RCB_HostingFilialeAuth"></rad:AjaxUpdatedControl>
                         <rad:AjaxUpdatedControl ControlID="Lab_TotalNumber"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalAmount"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_StorageAuth">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_HostingFilialeAuth" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_Purchasing">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="txt_Filiale"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RCB_Warehouse"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="txt_Description"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="txt_CompanyId"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="TB_Personnel"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="HF_FilialeId"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="HF_CompanyId"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="txt_OriginalCode"></rad:AjaxUpdatedControl>
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

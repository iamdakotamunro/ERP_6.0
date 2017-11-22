<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="GoodsPurchaseSet.aspx.cs" Inherits="ERP.UI.Web.GoodsPurchaseSet" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
        <script src="JavaScript/common.js" type="text/javascript"></script>
        <script type="text/javascript" language="javascript">
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
            function clientShow(sender, eventArgs) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }
            function RowClick(obj, args) {
                obj.set_selectedItemsInternal = true;
            }
            function OnEnabledConfirm() {
                var conf = window.confirm('提示：是否确认启用吗？');
                if (!conf)
                    return false;
                return true;
            }
            function OnDisableConfirm() {
                var conf = window.confirm('提示：是否确认禁用吗？');
                if (!conf)
                    return false;
                return true;
            }
            function ShowImg(obj) {
                var object = eval(obj);
                object.style.display = "block";
            }
            function HiddleImg(obj) {
                var object = eval(obj);
                object.style.display = "none";
            }

            function ShowCompanyPurchaseGroupForm() {
                window.radopen("./Windows/CompanyPurchaseGroupForm.aspx", "PurchaseGroupForm");
                return false;
            }

            function AddPurchaseSetForm() {
                window.radopen("./Windows/AddPurchaseSetForm.aspx", "AddPurchaseSetInfoForm");
                return false;
            }
            function Show(goodsId, warehouseId, hostingFilialeId) {
                window.radopen("./Windows/AddPurchaseSetForm.aspx?GoodsId=" + goodsId + "&WarehouseId=" + warehouseId + "&HostingFilialeId=" + hostingFilialeId  , "EditPurchaseSetForm");
                return false;
            }
            function DistributionsForm() {
                window.radopen("./Windows/PurchaseDistributionsForm.aspx", "PurchaseDistributionsForm");
                return false;
            }

            function ShowPurchaseSetLog(goodsId, warehouseId,hostingFilialeId, type) {
                window.radopen("./Windows/PurchingSetLogForm.aspx?GoodsId=" + goodsId + "&WarehouseId=" + warehouseId +"&HostingFilialeId=" + hostingFilialeId + "&Type=" + type, "PurchaseSetInfoForm");
                return false;
            }

            function ShowPurchaseSetLogForm() {
                window.radopen("./Windows/PurchingSetLogForm.aspx", "PurchaseSetInfoForm");
                return false;
            }
        </script>
    </rad:RadScriptBlock>
    <table class="PanelArea">
        <tr>
            <td style="text-align: right;">商品名称/编号：
            </td>
            <td>
                <asp:TextBox ID="tbGoodsName" Width="218px" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: right;">供应商：
            </td>
            <td>
                <table>
                    <tr>
                        <td>
                            <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Width="120px"
                                AllowCustomText="true" EnableLoadOnDemand="True" Height="120px" DataValueField="CompanyId"
                                DataTextField="CompanyName" AppendDataBoundItems="true" OnItemsRequested="RCB_Company_OnItemsRequested"
                                AutoPostBack="True" OnSelectedIndexChanged="RCB_Company_OnSelectedIndexChanged">
                                <Items>
                                    <rad:RadComboBoxItem Value="00000000-0000-0000-0000-000000000000" Text="所有公司" Selected="True" />
                                </Items>
                            </rad:RadComboBox>
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_PurchaseGroup" MaxHeight="200px" runat="server">
                            </rad:RadComboBox>
                        </td>
                        <td>
                            <asp:Button ID="btnUpdatePurchaseGroup" Text="修改选中分组" OnClick="BtnUpdatePurchaseGroup_OnClick"
                                runat="server" />
                        </td>
                        <td style="text-align: right;">责任人：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_Persion" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                                AllowCustomText="True" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="RealName"
                                DataValueField="PersonnelId" Width="80px" Height="200px">
                            </rad:RadComboBox>
                        </td>
                    </tr>
                </table>
            </td>
            <td>
                <asp:ImageButton ID="IbtnSearch" runat="server" SkinID="SearchButton" OnClick="IbtnSearch_Click" />
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">报备形式：
            </td>
            <td>
                <rad:RadComboBox ID="RCB_FilingForm" runat="server">
                    <Items>
                        <rad:RadComboBoxItem Value="0" Text="全部" Selected="True" />
                        <rad:RadComboBoxItem Value="1" Text="常规(月周期)" />
                        <rad:RadComboBoxItem Value="2" Text="触发报备" />
                    </Items>
                </rad:RadComboBox>
            </td>
            <td style="text-align: right;">备货日：
            </td>
            <td>
                <rad:RadComboBox ID="RCB_StockUpDay" runat="server">
                    <Items>
                        <rad:RadComboBoxItem Value="0" Text="全部" Selected="True" />
                        <rad:RadComboBoxItem Value="1" Text="周一" />
                        <rad:RadComboBoxItem Value="3" Text="周三" />
                    </Items>
                </rad:RadComboBox>
                <span style="padding-left: 60px;">审核状态：</span>
                <rad:RadComboBox ID="RCB_AuditStatue" Width="80px" Height="200px" runat="server">
                </rad:RadComboBox>
            </td>
            <td>
                <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="ExportData" OnClick="LbxlsClick" />
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_PurchaseSet" runat="server" SkinID="CustomPaging" OnNeedDataSource="RgPurchaseSet_OnNeedDataSource" OnItemDataBound="RgPurchaseSetOnItemDataBound"
        AllowMultiRowSelection="True" Width="100%">
        <ClientSettings>
            <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" />
        </ClientSettings>
        <MasterTableView DataKeyNames="GoodsId,WarehouseId,CompanyId,IsDelete,HostingFilialeId" ClientDataKeyNames="GoodsId,WarehouseId,CompanyId,IsDelete,HostingFilialeId"
            Width="100%">
            <CommandItemTemplate>
                <Ibt:ImageButtonControl ID="IB_CompanyPurchaseGroup" Text="定义分组" OnClientClick="return ShowCompanyPurchaseGroupForm()"
                    runat="server" SkinType="Insert" CausesValidation="false"></Ibt:ImageButtonControl>
                <Ibt:ImageButtonControl ID="IB_PurchaseSetLog" Text="调价历史" OnClientClick="return ShowPurchaseSetLogForm()"
                    runat="server" SkinType="Insert" CausesValidation="false"></Ibt:ImageButtonControl>
                <Ibt:ImageButtonControl ID="IB_AddPurchaseSet" Text="添加" OnClientClick="return AddPurchaseSetForm()"
                    runat="server" SkinType="Insert" CausesValidation="false"></Ibt:ImageButtonControl>
                <Ibt:ImageButtonControl ID="IB_Distributions" Text="已分配管理" OnClientClick="return DistributionsForm()"
                    runat="server" SkinType="Insert" CausesValidation="false"></Ibt:ImageButtonControl>
                <Ibt:ImageButtonControl ID="IB_EnabledPurchaseSet" Text="启用" OnClientClick="return OnEnabledConfirm()"
                    runat="server" SkinType="Affirm" CausesValidation="false" OnClick="OnClick_EnabledPurchaseSet"></Ibt:ImageButtonControl>
                <Ibt:ImageButtonControl ID="IB_DisablePurchaseSet" Text="禁用" OnClientClick="return OnDisableConfirm()"
                    runat="server" SkinType="Delete" CausesValidation="false" OnClick="OnClick_DisablePurchaseSet"></Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <Columns>
                <rad:GridClientSelectColumn UniqueName="column">
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridClientSelectColumn>
                <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                    Visible="False">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="WarehouseId" HeaderText="WarehouseId" UniqueName="WarehouseId"
                    Visible="False">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="PersonResponsible" HeaderText="PersonResponsible"
                    UniqueName="PersonResponsible" Visible="False">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="PromotionId" HeaderText="PromotionId" UniqueName="PromotionId"
                    Visible="False">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="供应商">
                    <ItemTemplate>
                        <%# GetCompanyName(new Guid(Eval("CompanyId").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="分组">
                    <ItemTemplate>
                        <%# Eval("PurchaseGroupName")%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="仓库">
                    <ItemTemplate>
                        <%# GetWarehouseName(new Guid(Eval("WarehouseId").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="采购价">
                    <ItemTemplate>
                        <a href="javascript:void(0);" id="A_PurchasePrice" runat="server" onclick='<%# "return ShowPurchaseSetLog(\""+Eval("GoodsId")+"\",\""+Eval("WarehouseId")+"\",\""+Eval("HostingFilialeId")+"\",\""+(int)PurchaseSetLogType.PurchasePrice+"\");"%>'>
                            <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("PurchasePrice").ToString()))%></a>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="报备形式">
                    <ItemTemplate>
                        <%# GetFilingForm(int.Parse(Eval("FilingForm").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备货日">
                    <ItemTemplate>
                        <%# GetStockUpDay(int.Parse(Eval("StockUpDay").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备货量">
                    <ItemTemplate>
                        <%# GetStockUpQuantity(int.Parse(Eval("FilingForm").ToString()), int.Parse(Eval("FilingTrigger").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注">
                    <ItemTemplate>
                        <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                            onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                            onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                        <div style="position: absolute;">
                            <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative; display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px; font-weight: bold; height: auto; overflow: visible; word-break: break-all;"
                                runat="server">
                                <%# Eval("Memo")%>
                            </div>
                        </div>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="责任人">
                    <ItemTemplate>
                        <%# GetPersonName(new Guid(Eval("PersonResponsible").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="是否启用" DataField="IsDelete" UniqueName="IsDelete">
                    <ItemTemplate>
                        <%# Convert.ToInt32(Eval("IsDelete")) == 0?"禁用":""%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <ItemTemplate>
                        <Ibt:ImageButtonControl ID="Ibn_Part" Text="修改" runat="server" CausesValidation="false"
                            OnClientClick='<%# "return Show(\"" + Eval("GoodsId")+ "\",\"" + Eval("WarehouseId")+ "\",\"" + Eval("HostingFilialeId")+ "\");" %>'
                            SkinID="InsertImageButton"></Ibt:ImageButtonControl>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
            <rad:RadWindow ID="AddPurchaseSetInfoForm" Title="新建商品采购设置" runat="server" OnClientShow="clientShow" />
            <rad:RadWindow ID="EditPurchaseSetForm" Title="编辑商品采购设置" runat="server" OnClientShow="clientShow" />
            <rad:RadWindow ID="PurchaseDistributionsForm" Title="已分配管理" runat="server" Width="900px"
                Height="640px" />
            <rad:RadWindow ID="PurchaseSetInfoForm" Title="商品采购设置更改列表" runat="server" Width="900"
                Height="640" />
            <rad:RadWindow ID="PurchaseGroupForm" Title="定义分组" Behaviors="Close,Reload,Resize"
                runat="server" Width="500" Height="350" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager runat="server" ID="RAM" OnAjaxRequest="Ram_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_PurchaseSet" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RG_PurchaseSet">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_PurchaseSet" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IbtnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_PurchaseSet" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnUpdatePurchaseGroup">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_PurchaseSet" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Company">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_PurchaseGroup" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

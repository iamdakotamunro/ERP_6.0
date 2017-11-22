<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.StockDeclareAw" Title="" CodeBehind="StockDeclare.aspx.cs" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">

    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowEditForm(realGoodsId) {
                var warehouseList = $find('<%=RCB_Warehouse.ClientID %>');
                if (warehouseList.get_selectedItem() != null) {
                    var warehouseId = warehouseList.get_selectedItem().get_value();
                    window.radopen("./Windows/ShowGoodsOrderInfo.aspx?realGoodsId=" + realGoodsId + "&warehouseId=" + warehouseId, "UserListDialog");
                }
                return false;
            }

            function Check(obj, quantity) {
                var num = obj.value;
                if (parseInt(num) < parseInt(quantity)) {
                    alert("采购数量(" + num + ")不能小于缺货数(" + quantity + ")");
                    obj.value = quantity;
                }
                if (num == "") {
                    obj.value = "0";
                }
            }
        </script>
    </rad:RadScriptBlock>

    <div class="StagePanel">
        <table class="StagePanelHead" border="0" style="width: 100%;">
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>

        <table class="PanelArea">
            <tr>
                <td>&nbsp;</td>
                <td class="ShortFromRowTitle">仓库：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        MarkFirstMatch="True" ShowToggleImage="True" DataTextField="Value" DataValueField="Key"
                        Height="200px">
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">供应商：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Height="200px"
                        DataValueField="CompanyId" DataTextField="CompanyName" AppendDataBoundItems="true" AllowCustomText="true"  Filter="StartsWith" >
                    </rad:RadComboBox>
                </td>
                <td class="ShortFromRowTitle">关键字：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TextBoxKeys" runat="server" Width="300px"></asp:TextBox>
                </td>
                <td class="Footer" align="right">
                    <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="CreationData" OnClick="Ib_CreationData_Click" />
                </td>
            </tr>
        </table>
        <table class="PanelArea">
            <tr>
                <td style="text-align: right;">&nbsp;</td>
                <td style="text-align: center; width: 100px;">到货日期:
                </td>
                <td style="text-align: left; width: 170px;">
                    <rad:RadDateTimePicker ID="RDP_ArrivalTime" runat="server" Width="150px" EnableTyping="False"></rad:RadDateTimePicker>
                </td>
                <td class="Footer" align="right" style="width: 120px;">
                    <asp:ImageButton ID="IB_ExportData" OnClick="Ib_CreatePurchase_Click" OnClientClick="return confirm('确定生成采购单？')" SkinID="PurchaseData" runat="server" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="RGGoodsDemand" runat="server" SkinID="Common" OnNeedDataSource="GridRGGoodsDemand_NeedDataSource" OnDetailTableDataBind="RGGoodsDemandDetail_NeedDataSource"
        OnSortCommand="RGGoodsDemand_SortCommand" AllowSorting="true" SortingSettings-SortToolTip="点击排序" AutoGenerateColumns="False"
        GroupingEnabled="False">
        <MasterTableView DataKeyNames="RealGoodsId,GoodsName,Sku,CompanyName" ClientDataKeyNames="RealGoodsId,GoodsName,Sku,CompanyName">
            <CommandItemTemplate>
                <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"></Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>
            <Columns>
                <rad:GridTemplateColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName" SortExpression="GoodsName">
                    <ItemTemplate>
                        <asp:LinkButton ID="LbDetail" runat="server" Text='<%# Eval("GoodsName")%>' OnClientClick='<%# "return ShowEditForm(\"" + Eval("RealGoodsId")+ "\");" %>'></asp:LinkButton>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Top" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="Sku" HeaderText="SKU" UniqueName="Sku" SortExpression="Sku">
                    <HeaderStyle Width="200px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="NonceGoodsStock" HeaderText="可用库存" UniqueName="NonceGoodsStock" SortExpression="NonceGoodsStock">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="PurchaseQuantity" HeaderText="缺货量" UniqueName="PurchaseQuantity" SortExpression="Demand">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="PurchaseQuantity" HeaderText="需采购量" UniqueName="PurchaseQuantity" SortExpression="PurchaseQuantity">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="CompanyName" HeaderText="供应商" UniqueName="CompanyName" SortExpression="CompanyName">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                </rad:GridBoundColumn>
            </Columns>
            <DetailTables>
                <rad:GridTableView runat="server" DataKeyNames="FilialeId,RealGoodsId" ShowFooter="true" ShowGroupFooter="true" CommandItemDisplay="None" Width="100%" ShowHeader="False"
                    NoDetailRecordsText="无子记录信息。">
                    <CommandItemTemplate>
                    </CommandItemTemplate>
                    <Columns>
                        <rad:GridTemplateColumn HeaderText="物流配送公司">
                            <ItemTemplate>
                                <%#GetFilialeName(Eval("FilialeId").ToString()) %>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridBoundColumn HeaderText="库存量" DataField="CurrentQuantity" UniqueName="CurrentQuantity">
                            <HeaderStyle Width="100" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn HeaderText="缺货量" DataField="Quantity" UniqueName="Quantity">
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn HeaderText="采购量">
                            <ItemTemplate>
                                <asp:TextBox ID="TB_Demand" Text='<%#Convert.ToDouble(Eval("PurchaseQuantity"))%>' onkeyup="this.value=this.value.replace(/-?\D/g,'')" runat="server" Width="80px" SkinID="ShortInput" onblur='Check(this,this.title);'
                                    ToolTip='<%# Eval("Quantity") %>' OnTextChanged="TbTextChanged" AutoPostBack="True"></asp:TextBox>
                                <%--<asp:RegularExpressionValidator ID="REVSpecialOfferGifts" ControlToValidate="TB_Demand"
                                    Text="*" ErrorMessage="采购量必须为数字类型!" ValidationExpression="^[+-]?\d*([\.]?\d*)?$" runat="server"></asp:RegularExpressionValidator>--%>
                            </ItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="供应商">
                            <ItemTemplate>
                                <asp:Label ID="LbText" runat="server" Text=''></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" Width="100px" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                </rad:GridTableView>
            </DetailTables>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="IB_CreationData">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGGoodsDemand" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_ExcelTemp">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="tbxCompany" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="tbxUsername"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="tbxUser"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="tbxAddress"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="tbxRemarks"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_ExportData">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RDP_ArrivalTime" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RGGoodsDemand" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_AllCommanyList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_AllCommanyList" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGGoodsDemand">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGGoodsDemand" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager ID="StockWindowManager" runat="server" Height="746px" Width="900px" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="UserListDialog" runat="server" Title="缺货单据列表" Height="606px" Width="900px" />
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditGoodsBorrowApplyForm.aspx.cs" Inherits="ERP.UI.Web.Windows.EditGoodsBorrowApplyForm" %>

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
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/jquery.js"></script>
            <script src="../My97DatePicker/WdatePicker.js"></script>
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/common.js" type="text/javascript"></script>
            <script language="javascript" type="text/javascript">
                function AddHFSonGoods(sonGoods) {
                    if (document.getElementById("<%=HFSonGoods.ClientID %>").value == "")
                        document.getElementById("<%=HFSonGoods.ClientID %>").value += sonGoods;
                    else
                        document.getElementById("<%=HFSonGoods.ClientID %>").value += "@" + sonGoods;
                }
                function DelHFSonGoods(sonGoods) {
                    var str = document.getElementById("<%=HFSonGoods.ClientID %>").value;
                    if (str != "") {
                        if (str.indexOf("@") != -1) {
                            if (str.indexOf(sonGoods + "@") != -1)
                                str = str.replace(sonGoods + "@", "");
                            else
                                str = str.replace("@" + sonGoods, "");
                        } else {
                            str = str.replace(sonGoods, "");
                        }
                    }
                    document.getElementById("<%=HFSonGoods.ClientID %>").value = str;
                }

                function onCheckBoxClick(chk, cid) {
                    var objCid = document.getElementById(chk);
                    if (objCid.checked)
                        AddHFSonGoods(chk + "|" + cid);
                    else
                        DelHFSonGoods(chk + "|" + cid);
                }

                var flag = '1';
                function HiddenIbtnQuantityOut(obj) {
                    flag = '0';
                    var tbId = obj.id;
                    var ibtnId = tbId.replace("TB_Quantity", "ibtnQuantityOut");
                    document.getElementById(ibtnId).click();
                }

                
            </script>
        </rad:RadScriptBlock>
        <div runat="server" id="div_Refresh">
            <table width="100%">
                <tr>
                    <td style="text-align: right; font-size: 14px; font-weight: bold;">注：如果借入单和借入返还单不同，请点击借入返还单进行修改。(系统默认一致)
                    </td>
                    <td width="70px">
                        <asp:Button ID="btnSave" runat="server" Text="保存" OnClick="BtnSave_Click" />
                    </td>
                    <td width="70px">
                        <asp:Button ID="btnCancel" runat="server" Text="取消" OnClientClick="return CancelWindow();" />
                    </td>
                    <td width="10px">&nbsp;
                    </td>
                </tr>
            </table>
            <table class="PanelArea">
                <tr>
                    <td style="text-align: right;">申请时间：
                    </td>
                    <td>
                        <asp:TextBox ID="txt_DateCreated" runat="server" ReadOnly="true" Width="245px"></asp:TextBox>
                    </td>
                    <td style="text-align: right;">操作人：
                    </td>
                    <td>
                        <asp:TextBox ID="txt_Transactor" runat="server" ReadOnly="true" Width="245px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">供应商:
                    </td>
                    <td>
                        <rad:RadComboBox ID="RCB_CompanyId" runat="server" DataTextField="CompanyName" DataValueField="CompanyId" Filter="Contains"
                            Width="250px" Height="200px">
                        </rad:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td width="15%" style="text-align: right;">入库仓储：
                    </td>
                    <td class="warehousecss">
                        <rad:RadComboBox ID="RCB_Warehouse" runat="server" OnSelectedIndexChanged="RcbInStockOnSelectedIndexChanged" AutoPostBack="true"
                            Width="123px">
                        </rad:RadComboBox>
                        <rad:RadComboBox ID="RCB_StorageAuth" runat="server" DataTextField="StorageTypeName" DataValueField="StorageType"
                            Width="121px" OnSelectedIndexChanged="RcbStorageAuthOnSelectedIndexChanged" AutoPostBack="true">
                        </rad:RadComboBox>
                    </td>


                    <td width="20%" style="text-align: right;">物流配送公司：
                    </td>
                    <td>
                        <rad:RadComboBox ID="RCB_HostingFilialeAuth" runat="server" AccessKey="T" ShowToggleImage="True"
                            DataTextField="HostingFilialeName" DataValueField="HostingFilialeId"
                            Width="250px">
                        </rad:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">备注说明：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbDescription" runat="server" Width="74%" MaxLength="100" Height="30px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
            </table>
            <table class="PanelArea">
                <tr>
                    <td>
                        <table width="100%">
                            <tr>
                                <td width="88px;" style="height: 30px;">当前商品清单：
                                </td>
                                <td>
                                    <asp:Label ID="lbTitle" Text="借入单" Style="font-weight: bold; font-size: 16px;" runat="server"></asp:Label>
                                </td>
                                <td style="text-align: right;">
                                    <asp:CheckBox ID="cbStockBack" Text="借入返还单" AutoPostBack="True" OnCheckedChanged="CbStockBack_CheckedChanged"
                                        runat="server" />
                                </td>
                                <td width="100px">
                                    <asp:Button ID="btnShowAddGoods" runat="server" Text="选择商品" OnClientClick="return ShowObject('GoodsPanel');" />
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:HiddenField ID="HF_TradeCode" runat="server" />
                        <asp:HiddenField ID="HFSonGoods" runat="server" />
                        <rad:RadGrid ID="RG_Goods" OnNeedDataSource="RgGoods_NeedDataSource" AllowPaging="false"
                            runat="server" OnDeleteCommand="RgGoods_DeleteCommand">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                                <Selecting AllowRowSelect="True" />
                            </ClientSettings>
                            <MasterTableView DataKeyNames="GoodsId,RealGoodsId,UnitPrice" CommandItemDisplay="None">
                                <Columns>
                                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode">
                                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                                        <ItemTemplate>
                                            <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("UnitPrice").ToString()))%>
                                        </ItemTemplate>
                                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                        <ItemStyle Width="60px" HorizontalAlign="Center" />
                                    </rad:GridTemplateColumn>
                                    <rad:GridTemplateColumn DataField="Quantity" HeaderText="借入数" UniqueName="Quantity">
                                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemStyle Width="80px" HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:TextBox ID="TB_Quantity" runat="server" Font-Bold="true" Text='<%# Eval("Quantity") %>'
                                                Style="width: 55px;" MaxLength="10" onblur="Total();"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RFVQuantity" runat="server" ControlToValidate="TB_Quantity"
                                                ErrorMessage="数量必须填写" Text="*"></asp:RequiredFieldValidator>
                                            <asp:RegularExpressionValidator ID="REVnums" runat="server" ControlToValidate="TB_Quantity"
                                                ValidationExpression="^\d+$" ErrorMessage="*"></asp:RegularExpressionValidator>
                                        </ItemTemplate>
                                    </rad:GridTemplateColumn>

                                    <rad:GridButtonColumn HeaderText="操作" CommandName="Delete" Text="删除" ConfirmText="确实要删除吗？"
                                        UniqueName="Delete" ButtonType="ImageButton">
                                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                        <ItemStyle Width="50px" HorizontalAlign="Center" />
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridButtonColumn>
                                    <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                                        Visible="False">
                                    </rad:GridBoundColumn>
                                </Columns>
                            </MasterTableView>
                        </rad:RadGrid>
                        <rad:RadGrid ID="RG_GoodsBack" OnNeedDataSource="RgGoodsBack_NeedDataSource" OnItemDataBound="RgGoodsBack_OnItemDataBound"
                            OnItemCommand="rgGoodsOrderDetail_OnItemCommand" AllowPaging="False" Visible="False"
                            runat="server">
                            <ClientSettings>
                                <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                                <Selecting AllowRowSelect="True" />
                            </ClientSettings>
                            <MasterTableView DataKeyNames="GoodsId,RealGoodsId,UnitPrice" CommandItemDisplay="None">
                                <GroupByExpressions>
                                    <rad:GridGroupByExpression>
                                        <GroupByFields>
                                            <rad:GridGroupByField FieldName="GoodsName" HeaderText=" " HeaderValueSeparator=" " />
                                        </GroupByFields>
                                        <SelectFields>
                                            <rad:GridGroupByField FieldName="GoodsName" HeaderText=" " HeaderValueSeparator=" " />
                                        </SelectFields>
                                    </rad:GridGroupByExpression>
                                </GroupByExpressions>
                                <Columns>
                                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="BatchNo" HeaderText="批号" UniqueName="BatchNo" Visible="False">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridTemplateColumn HeaderText="SKU">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <rad:RadComboBox ID="rcbSpecification" OnSelectedIndexChanged="RcbSpecification_OnSelectedIndexChanged"
                                                AutoPostBack="true" MaxHeight="250px" Visible="False" runat="server">
                                            </rad:RadComboBox>
                                        </ItemTemplate>
                                    </rad:GridTemplateColumn>
                                    <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridBoundColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                    </rad:GridBoundColumn>
                                    <rad:GridTemplateColumn HeaderText="借入返还数">
                                        <HeaderStyle HorizontalAlign="Center" />
                                        <ItemStyle HorizontalAlign="Center" />
                                        <ItemTemplate>
                                            <asp:TextBox ID="TB_Quantity" Text='<%# Eval("Quantity") %>' onchange="HiddenIbtnQuantityOut(this);"
                                                Style="width: 55px;" MaxLength="10" runat="server"></asp:TextBox>
                                            <asp:ImageButton ID="ibtnQuantityOut" Style="visibility: hidden;" CommandName="QuantityOut"
                                                runat="server" SkinID="AffirmImageButton" />
                                        </ItemTemplate>
                                    </rad:GridTemplateColumn>
                                </Columns>
                            </MasterTableView>
                        </rad:RadGrid>
                    </td>
                </tr>
            </table>
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
        <div id="GoodsPanel" style="background-color: #FFFFFF; width: 100%; height: 222px; left: 0; position: absolute; top: 0; z-index: -1; visibility: hidden">
            <table width="100%">
                <tr>
                    <td style="width: 90px; height: 20px; text-align: right;">选择分类：
                    </td>
                    <td style="width: 230px;">
                        <rad:RadComboBox ID="RCB_GoodsClass" runat="server" CausesValidation="false" AutoPostBack="true"
                            DataValueField="ClassId" DataTextField="ClassName" OnSelectedIndexChanged="GoodsClass_SelectedIndexChanged"
                            Width="220px" Height="200px">
                        </rad:RadComboBox>
                    </td>
                    <td style="width: 90px; text-align: right;">编号搜索：
                    </td>
                    <td style="width: 230px;">
                        <rad:RadComboBox ID="RCB_Goods" runat="server" CausesValidation="false" AutoPostBack="true"
                            AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="GoodsName" DataValueField="GoodsId"
                            OnSelectedIndexChanged="Goods_SelectedIndexChanged" OnItemsRequested="RcbGoodsItemsRequested"
                            Width="220px" Height="200px">
                        </rad:RadComboBox>
                    </td>
                    <td style="text-align: right;">
                        <asp:Button ID="Btn_SelectGoods" runat="server" Text="添加商品" OnClick="SelectGoods_Click"
                            CausesValidation="false" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <input id="CloseGoodsPanel" type="button" value="关闭添加" onclick="return HiddenObject('GoodsPanel');" />
                    </td>
                </tr>
            </table>
            <rad:RadGrid runat="server" ID="RGSelectGoods" AutoGenerateColumns="False" MasterTableView-CommandItemDisplay="None"
                Height="175px" SkinID="Common" OnNeedDataSource="RgSelectGoodsNeedDataSource" AllowMultiRowSelection="true"
                AllowPaging="false" OnItemDataBound="RgSelectGoodsItemDataBound">
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                </ClientSettings>
                <MasterTableView ClientDataKeyNames="GoodsId" DataKeyNames="GoodsId">
                    <CommandItemStyle HorizontalAlign="Right" Height="0px" />
                    <Columns>
                        <rad:GridClientSelectColumn UniqueName="column">
                            <HeaderStyle Width="50" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridClientSelectColumn>
                        <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名" UniqueName="GoodsName">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn DataField="Luminosity" HeaderText="光度" UniqueName="Luminosity">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" Width="200px" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn DataField="Astigmia" HeaderText="散光" UniqueName="Astigmia">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn DataField="Axial" HeaderText="轴位" UniqueName="Axial">
                            <ItemTemplate>
                            </ItemTemplate>
                            <HeaderStyle Width="200px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </rad:RadGrid>
        </div>
        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_StorageAuth" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RCB_HostingFilialeAuth" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_StorageAuth">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RCB_HostingFilialeAuth" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_GoodsClass">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGSelectGoods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RCB_Goods">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGSelectGoods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="Btn_SelectGoods">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RG_Goods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalNumber"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalAmount"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="cbStockBack">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="cbStockBack"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="lbTitle" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="btnShowAddGoods"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RG_Goods" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="RG_GoodsBack" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalNumber"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalAmount"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RG_Goods">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalNumber"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Lab_TotalAmount"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="btnSave">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="btnSave" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

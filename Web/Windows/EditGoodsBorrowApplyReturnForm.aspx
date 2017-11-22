<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditGoodsBorrowApplyReturnForm.aspx.cs" Inherits="ERP.UI.Web.Windows.EditGoodsBorrowApplyReturnForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
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
                    <td style="text-align: right; font-size: 14px; font-weight: bold;">
                    </td>
                    <td width="70px">
                        <asp:Button ID="btnSave" runat="server" Text="重送"   OnClick="BtnSave_Click" />
                    </td>
                    <td width="70px">
                        <asp:Button ID="btnCancel" runat="server" Text="取消" OnClientClick="return CancelWindow();" />
                    </td>
                    <td width="10px">&nbsp;
                    </td>
                </tr>
            </table>
            <table class="PanelArea" >
                <tr>
                    <td style="text-align: right;">申请时间：
                    </td>
                    <td >
                        <asp:TextBox ID="txt_DateCreated" runat="server" ReadOnly="true" Width="245px"></asp:TextBox>
                    </td>
                    <td style="text-align: right;">操作人：
                    </td>
                    <td >
                        <asp:TextBox ID="txt_Transactor" runat="server" ReadOnly="true" Width="245px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">供应商:
                    </td>
                    <td>
                        <rad:RadComboBox ID="RCB_CompanyId" runat="server" DataTextField="CompanyName" DataValueField="CompanyId"
                            Width="250px" Height="200px" Enabled="False">
                        </rad:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td width="15%" style="text-align: right;">入库仓储：
                    </td>
                    <td class="warehousecss">
                        <rad:RadComboBox ID="RCB_Warehouse" runat="server" Enabled="False"
                            Width="123px" >
                        </rad:RadComboBox>
                        <rad:RadComboBox ID="RCB_StorageAuth" runat="server" DataTextField="StorageTypeName" DataValueField="StorageType"
                            Width="121px"  Enabled="False">
                        </rad:RadComboBox>
                    </td>
                    

                    <td width="20%" style="text-align: right;">物流配送公司：
                    </td>
                    <td>
                        <rad:RadComboBox ID="RCB_HostingFilialeAuth" runat="server" AccessKey="T" ShowToggleImage="True"
                            DataTextField="HostingFilialeName" DataValueField="HostingFilialeId"
                            Width="250px" Enabled="False">
                        </rad:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: right;">备注说明：
                    </td>
                    <td colspan="3">
                        <asp:TextBox ID="tbDescription" runat="server" Width="74%" MaxLength="100" Height="30px"  TextMode="MultiLine"></asp:TextBox>
                          <asp:HiddenField ID="HF_Description" runat="server" />
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
                                    <asp:Label ID="lbTitle" Text="借入返还单" Style="font-weight: bold; font-size: 16px;" runat="server"></asp:Label>
                                </td>
                            </tr>
                        </table>
                    </td>
                </tr>
                <tr>
                    <td>
                         <asp:HiddenField ID="HFSonGoods" runat="server" />
                        
                        <rad:RadGrid ID="RG_GoodsBack" OnNeedDataSource="RgGoodsBack_NeedDataSource" OnItemDataBound="RgGoodsBack_OnItemDataBound"
                            OnItemCommand="rgGoodsOrderDetail_OnItemCommand" AllowPaging="False"  
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
                                            <asp:TextBox ID="TB_Quantity" Text='<%# Math.Abs(Convert.ToDouble(Eval("Quantity"))) %>' onchange="HiddenIbtnQuantityOut(this);"
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
                </tr>
            </table>
        </div>
         
        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

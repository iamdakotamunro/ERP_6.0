<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovalInnerPurchaseForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ApprovalInnerPurchaseForm" %>

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
            <script src="../JavaScript/jquery.js"></script>
            <script type="text/javascript" src="../JavaScript/telerik.js"></script>
            <script type="text/javascript" src="../JavaScript/common.js"></script>
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

                function AddFieldToHidden(value, fieldOrderIndex, hiddenClientId) {
                    var hiddenValue = document.getElementById(hiddenClientId).value;
                    var hiddenArray = new Array();
                    hiddenArray = hiddenValue.split(",");
                    hiddenArray[fieldOrderIndex] = value;
                    document.getElementById(hiddenClientId).value = hiddenArray.join(",");
                }
                function SumPrice(quantityId, unitPriceId, sumPriceId) {
                    var quantity = document.getElementById(quantityId).value;
                    var unitPrice = document.getElementById(unitPriceId).value;
                    document.getElementById(sumPriceId).value = Math.round(quantity * 100 * unitPrice * 100) / 10000;
                    Total();
                }

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


            function onDropDownClosing() {
                cancelDropDownClosing = false;
            }
            function StopPropagation(e) {
                //cancel bubbling
                e.cancelBubble = true;
                if (e.stopPropagation) {
                    e.stopPropagation();
                }
            }
            function onCheckBoxClick(chk, cId) {
                var combo = document.getElementById(cId);
                //prevent second combo from closing
                cancelDropDownClosing = true;
                //holds the text of all checked items
                var text = "";
                //holds the values of all checked items
                var values = "";



                //enumerate all items

            }

            function OnClientDropDownClosing(CId) {
                // debugger;
                var objTable = document.getElementById(CId);
                var aInput = objTable.getElementsByTagName("input");

                for (var j = 1; j < aInput.length; j += 1) {
                    //totalNumber += Number(aInput[j].value);

                    alert(aInput[j].value);
                }

            }
            function onCheckBoxClick(chk, cid) {
                var objCid = document.getElementById(chk);
                var objParment = objCid.parentElement.parentElement.parentElement.parentElement.parentElement;
                // debugger;
                if (objCid.checked)
                    AddHFSonGoods(chk + "|" + cid);
                else
                    DelHFSonGoods(chk + "|" + cid);


            }
            //判断数组是否存在 增加元素数组
            function AddElementToArray(element) {
                if (isUndefined(FieldArray)) {
                    var FieldArray = new Array();
                }
                FieldArray.push(element);
            }

            function isUndefined(variable) {
                return typeof variable == 'undefined' ? true : false;
            }
            </script>
        </rad:RadScriptBlock>
        <asp:Button ID="BtnIsDelete" runat="server" Text="Button" Style="display: none;" />
        <asp:Panel class="StagePanel" runat="server">
            <table class="PanelArea">
                <tr>
                    <td class="AreaRowTitle">录单时间：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label runat="server" ID="txt_DateCreated"></asp:Label>
                    </td>
                    <td class="AreaRowTitle">操 作 人：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label runat="server" ID="txt_Transactor"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="AreaRowTitle">出库仓储：
                    </td>
                    <td class="AreaRowInfo warehousecss">
                        <asp:Label runat="server" ID="RCB_OutWarehouse"></asp:Label>
                        <asp:Label runat="server" ID="RCB_OutStorageAuth"></asp:Label>
                    </td>
                    <td class="AreaRowTitle">出库物流配送公司：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label runat="server" ID="RCB_OutHostingFilialeAuth"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="AreaRowTitle">入库仓储：
                    </td>
                    <td class="AreaRowInfo warehousecss">
                        <asp:Label runat="server" ID="RCB_InWarehouse"></asp:Label>
                        <asp:Label runat="server" ID="RCB_InStorageAuth"></asp:Label>
                    </td>
                    <td class="AreaRowTitle">入库物流配送公司：
                    </td>
                    <td class="AreaRowInfo">
                        <asp:Label runat="server" ID="RCB_InHostingFilialeAuth"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td class="AreaRowTitle">备注说明：
                    </td>
                    <td class="AreaRowInfo" colspan="3">
                        <asp:TextBox ID="txt_Description" runat="server" Width="74%" MaxLength="100" Height="30px" TextMode="MultiLine"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td class="AreaRowInfo" colspan="4">
                        <hr />
                    </td>
                </tr>
            </table>

            <asp:HiddenField ID="HFSonGoods" runat="server" />
            <rad:RadGrid ID="RGGoods" AllowPaging="false" runat="server" SkinID="Common" Height="266px"
                OnNeedDataSource="RGGoods_NeedDataSource">
                <ClientSettings>
                    <Resizing AllowColumnResize="True"></Resizing>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                    <ClientMessages DragToResize="调整大小" />
                </ClientSettings>
                <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="RealGoodsId,GoodsId,GoodsCode,GoodsName,Specification,Units,Quantity">
                    <CommandItemTemplate>
                    </CommandItemTemplate>
                    <CommandItemStyle HorizontalAlign="Right" Height="24px" />
                    <Columns>
                        <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode" Visible="False">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                            <HeaderStyle Width="205px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>

                        <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                            <HeaderStyle Width="60px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn DataField="NonceWarehouseGoodsStock" HeaderText="可用库数" UniqueName="NonceWarehouseGoodsStock">
                            <ItemTemplate>
                                <asp:Literal ID="Lab_NonceWarehouseGoodsStock" runat="server" Text='<%#Eval("NonceWarehouseGoodsStock")  %>'></asp:Literal>
                            </ItemTemplate>
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                        </rad:GridTemplateColumn>
                        <rad:GridBoundColumn DataField="Quantity" HeaderText="出库数" UniqueName="Quantity">
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
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
                </tr>
            </table>
            <div style="text-align: center; padding-top: 10px;">
                <asp:Button ID="btn_Approval" runat="server" Text="  核准  " OnClick="btnApproval_Click" />&nbsp;&nbsp;&nbsp;
                <asp:Button ID="btn_Return" runat="server" Text="  核退  " OnClick="btnReturn_Click" />
            </div>
        </asp:Panel>
        <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
        <AjaxSettings>
           
        </AjaxSettings>
    </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

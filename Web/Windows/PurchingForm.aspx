<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PurchingForm.aspx.cs" Inherits="ERP.UI.Web.Windows.PurchingForm" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Register Src="~/UserControl/ChildFieldIControl.ascx" TagPrefix="uc1" TagName="ChildFieldIControl" %>


<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
        <script language="javascript" type="text/javascript">

            function rcGoodsRequested(combBox, text, more) {
                if (text.get_text().length < 10)
                    return false;
                return true;
            }

            document.onkeydown = function enterToTab() {
                if (event.srcElement.type != 'textarea' && event.keyCode == 13) {
                    event.keyCode = 9;
                }
            };

            function AddHFSonGoods(sonGoods) {
                if (document.getElementById("HFSonGoods").value == "")
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


            ///
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

            function ShowImg(obj) {
                var object = eval(obj);
                object.style.display = "block";
            }

            function HiddleImg(obj) {
                var object = eval(obj);
                object.style.display = "none";
            }
        </script>
    </rad:RadScriptBlock>

    <div id="GoodsPanel" style="background-color: #FFFFFF; width: 100%; height: 205px;left: 0px; position: absolute; top: 0px;">
        <table width="100%">
            <tr>
                <td style="width: 90px; height: 25px; text-align: left;">
                    选择分类:
                </td>
                <td style="width: 230px;">
                    <rad:RadComboBox ID="RCB_GoodsClass" runat="server" CausesValidation="false" AutoPostBack="true"
                        DataValueField="ClassId" DataTextField="ClassName" OnSelectedIndexChanged="GoodsClass_SelectedIndexChanged"
                        Width="220px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td style="width: 90px; text-align: left;">
                    编号搜索:
                </td>
                <td style="width: 230px;">
                    <rad:RadComboBox ID="RCB_Goods" runat="server" CausesValidation="false" AutoPostBack="true"
                        AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="GoodsName" DataValueField="GoodsId"
                        OnSelectedIndexChanged="Goods_SelectedIndexChanged" Width="220px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td width="80px" align="left">
                    是否赠品:
                </td>
                <td width="150px" align="left">
                    <asp:CheckBox ID='Cbx_GoodsType' runat='server' />
                </td>
                <td style="text-align: right;">
                    <asp:Button ID="Button_SelectGoods" Text="添加商品" CssClass="Button"
                        OnClick="SelectGoods_Click" CausesValidation="false" runat="server" />
                </td>
            </tr>
        </table>

        <rad:RadGrid runat="server" ID="RgGoods" AutoGenerateColumns="False" MasterTableView-CommandItemDisplay="None"
            Height="175px" SkinID="Common" OnNeedDataSource="RgGoodsNeedDataSource" AllowMultiRowSelection="true" 
            AllowPaging="false" OnItemDataBound="RgGoodsItemDataBound">
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
                        <HeaderStyle  HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名" UniqueName="GoodsName">
                        <HeaderStyle  HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="Luminosity" HeaderText="光度" UniqueName="Luminosity">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle  HorizontalAlign="Center" Width="200px" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                     <rad:GridTemplateColumn DataField="Astigmia" HeaderText="散光" UniqueName="Astigmia">
                         <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                     <rad:GridTemplateColumn DataField="Axial" HeaderText="轴位" UniqueName="Axial" >
                         <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>

        <asp:HiddenField ID="HFSonGoods" runat="server" />
        <table border="0" width="100%">
            <tr>
                <td style="text-align: right;">仓 库：</td>
                <td style="width: 130px;text-align: left;">
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" AutoPostBack="True" DataTextField="WarehouseName" DataValueField="WarehouseId" Width="120px" Height="200px" OnSelectedIndexChanged="RcbWarehouseSelectedChanged">
                    </rad:RadComboBox>
                </td>
                <td style="width: 150px;text-align: left;"><rad:RadComboBox ID="RCB_Filiale" runat="server" AutoPostBack="True" Width="120px" DataTextField="HostingFilialeName" DataValueField="HostingFilialeId"
                     OnSelectedIndexChanged="RcbFilialeSelectedChanged"></rad:RadComboBox></td>
                <td style="width: 80px;text-align: right;">到货日期：</td>
                <td style="width: 160px;text-align: right;"><rad:RadDateTimePicker ID="RDP_ArrivalTime" runat="server" Width="150px" EnableTyping="False" ></rad:RadDateTimePicker></td>
                <td style="width: 100px;text-align: right;">创建人：</td>
                <td style="width: 80px;text-align: left;"><asp:Label runat="server" ID="LB_Director"></asp:Label></td>
                <td style="width: 100px;text-align: center;"><asp:Button ID="btnSubmit" runat="server" Text="生成采购单" CssClass="Button" OnClick="Button_InsterStock" CausesValidation="false" /></td>
                <td width="55" align="left">
                    <asp:Button ID="btnDelete" runat="server" Text="删  除" CssClass="Button" OnClick="Button_Delete" CausesValidation="false" />
                </td>
            </tr>
        </table>
        <rad:RadGrid ID="Rgd_PurchasingDetail" OnNeedDataSource="Rgd_PurchasingDetail_OnNeedDataSource"
            AllowMultiRowSelection="true" Width="100%" AllowPaging="false" runat="server">
            <MasterTableView DataKeyNames="GoodsID,GoodsName,Units,GoodsCode,Specification,Price,PurchasingGoodsType"
                ClientDataKeyNames="GoodsID,GoodsCode,Specification">
                <CommandItemTemplate>
                    <asp:ImageButton ID="IB_ExportData" runat="server" SkinID="RefreshImageButton" />
                </CommandItemTemplate>
                <CommandItemStyle HorizontalAlign="right" Height="25px" />
                <Columns>
                    <rad:GridClientSelectColumn UniqueName="column">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridClientSelectColumn>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名" UniqueName="GoodsName">
                        <HeaderStyle Width="280px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" UniqueName="GoodsCode">
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="SixtyDaySales" HeaderText="前第2月销量" UniqueName="SixtyDaySales" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="ThirtyDaySales" HeaderText="前第1月销量" UniqueName="ThirtyDaySales" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="ElevenDaySales" HeaderText="日均销量<br/>(11天)" UniqueName="ElevenDaySales" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="PlanQuantity" HeaderText="采购数量" UniqueName="PlanQuantity">
                        <ItemTemplate>
                            <asp:TextBox ID="tbx_quantity" Width="80px" SkinID="ShortInput" runat="server" Text='<%#Eval("PlanQuantity") %>'></asp:TextBox>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="是否赠品" DataField="PurchasingGoodsType" UniqueName="PurchasingGoodsType">
                        <ItemTemplate>
                            <%#(Convert.ToInt32(Eval("PurchasingGoodsType").ToString())==(int)PurchasingGoodsType.Gift)?"赠品":"" %>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                   <%-- <rad:GridTemplateColumn HeaderText="备注" DataField="Descrioption" UniqueName="Description">
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# this.GetDescription(Eval("GoodsID").ToString())%>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>--%>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>

    <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Filiale" LoadingPanelID="Loading" />
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Filiale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnSubmit">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnDelete">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_GoodsClass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgGoods" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Button_SelectGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Rgd_PurchasingDetail">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Inster">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="ControlToolsTab" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_ModifyStock">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="ControlToolsTab" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Goods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="HFSonGoods" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Goods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgGoods" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_GoodsClass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="HFSonGoods" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

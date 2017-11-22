<%@ Page Language="C#" AutoEventWireup="true" Inherits="ERP.UI.Web.Windows.ShowGoodsOrderInfo" CodeBehind="ShowGoodsOrderInfo.aspx.cs" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script type="text/javascript" src="../JavaScript/jquery.js"></script>
            <script type="text/javascript" src="../JavaScript/telerik.js"></script>
            <script type="text/javascript" src="../JavaScript/common.js"></script>

            <script type="text/javascript" language="javascript">

                function ShowClewForm(orderId, type, saleFilialeId) {
                    if (type == "1") {
                        //window.radopen("ShowClewForm.aspx?OrderId=" + orderId + "&memo=1&SaleFilialeId=" + saleFilialeId, "ClewForm");
                        window.radopen("ShowClewForm.aspx?OrderId=" + orderId + "&SaleFilialeId=" + saleFilialeId, "UserListDialog");
                    }
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

            function RowDblClick(obj, args) {
                var orderId = args.getDataKeyValue("OrderId");
                var type = args.getDataKeyValue("BusinessType");
                if (type == 1) {
                    window.radopen("ShowGoodsOrder.aspx?OrderId=" + orderId+"&Type=1", "GoodsOrder");
                } 
            }

            function DeleteConfirm(src) {
                var tr = src.parentNode.parentNode;
                var orderno = src.parentNode.parentNode.cells[0].innerText;
                var name = src.parentNode.parentNode.cells[1].innerText;
                tr.style.fontWeight = "bold";
                return window.confirm("是否确定删除订单 [" + name + " " + orderno + "]？");
            }

            function ShowImg(obj) {
                var obj = eval(obj);
                obj.style.display = "block";
            }

            function HiddleImg(obj) {
                var obj = eval(obj);
                obj.style.display = "none";
            }
            </script>

        </rad:RadScriptBlock>
        <div>
            <rad:RadGrid ID="RGGoodsOrder" runat="server" ShowStatusBar="True" ShowGroupPanel="True"
                SkinID="Common" OnNeedDataSource="RGGoodsOrder_NeedSource" OnItemDataBound="RGGoodsOrderItemDataBound">
                <ClientSettings>
                    <ClientEvents OnRowDblClick="RowDblClick" />
                </ClientSettings>
                <MasterTableView ClientDataKeyNames="OrderId,BusinessType,LackQuantity,SaleFilialeId" DataKeyNames="OrderId,BusinessType">
                    <CommandItemTemplate>
                        <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"></Ibt:ImageButtonControl>
                    </CommandItemTemplate>
                    <Columns>
                        <rad:GridTemplateColumn DataField="OrderNo" HeaderText="单据号" UniqueName="OrderNo">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate><%# Eval("BusinessType").ToString()=="1"?"<span style='color:red;'>" + Eval("OrderNo").ToString() + "</span>":Eval("OrderNo").ToString() %></ItemTemplate>
                        </rad:GridTemplateColumn>
                        <rad:GridBoundColumn DataField="Quantity" HeaderText="数量" UniqueName="Quantity">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn DataField="BusinessType" HeaderText="单据类型" UniqueName="BusinessType">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                            <ItemTemplate><%# Eval("BusinessType").ToString()=="1"?"订单":"出库单" %></ItemTemplate>
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Clew">
                            <ItemTemplate>
                                <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                                    OnClientClick='<%# "return ShowClewForm(\"" + Eval("OrderId") + "\",\"" + Eval("BusinessType") + "\",\"" + Eval("SaleFilialeId") + "\");" %>'
                                    ToolTip='<%#GetMisClew(Eval("OrderId"))%>' />
                            </ItemTemplate>
                            <HeaderStyle Width="70px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </rad:RadGrid>
        </div>
        <rad:RadWindowManager ID="ClewWindowManager" runat="server" UseEmbeddedScripts="false"
            Height="545" Width="860" ReloadOnShow="true">
            <Windows>
                <rad:RadWindow ID="GoodsOrder" runat="server" Width="870" Height="500" Title="订单查询" />
                <rad:RadWindow ID="ClewForm" runat="server" Width="420" Height="331" Title="备注" />
                <rad:RadWindow ID="Member" runat="server" Width="700" Height="442" Title="用户信息" />
            </Windows>
        </rad:RadWindowManager>
        <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RAM_AjaxRequest">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RGGoodsOrder">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGGoodsOrder" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RAM">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGGoodsOrder" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

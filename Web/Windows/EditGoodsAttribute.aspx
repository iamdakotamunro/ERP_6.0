<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditGoodsAttribute.aspx.cs" Inherits="ERP.UI.Web.Windows.EditGoodsAttribute" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>

</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/jquery.js" type="text/javascript"></script>
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/common.js" type="text/javascript"></script>
            <script type="text/javascript">
                function checkOutletGroup(obj) {
                    var info = new Array();
                    var arr = $(obj).attr("id").split('_');
                    var name = arr[0] + "_" + arr[1] + "_" + arr[2] + "_" + arr[3] + "_" + arr[4];
                    var id = name + "_DropDown";
                    $("#" + id).find("li").find("input[type='checkbox']:checkbox:checked").each(function () {
                        var checkId = $(this).attr("id");
                        info.push($("label[for=" + checkId + "]")[0].innerText);
                    });
                    $("#" + name + "_Input").val(info.join(","));
                }
            </script>
        </rad:RadScriptBlock>
        <asp:Panel ID="Panel_GoodsAttribute" runat="server">
            <asp:Panel ID="PanelMsg" runat="server" Visible="false">
                <asp:Label ID="Lb_Msg" runat="server" Text=""></asp:Label>
            </asp:Panel>
            <div style="padding: 5px;">
                <asp:Label ID="lbGoodsInfo" runat="server"></asp:Label>
            </div>
            <asp:DataList ID="DL_GroupsList" runat="server" OnItemDataBound="DlGroupsList_OnItemDataBound" DataKeyField="GroupId">
                <ItemTemplate>
                    <table>
                        <tr>
                            <td align="center" style="width: 50px">
                                <%# GetGroupName(Eval("GroupId").ToString()) %>
                            </td>
                            <td>
                                <asp:HiddenField ID="hidden_ShopFiliale" runat="server" />

                                <rad:RadComboBox ID="rdb_Groups" runat="server" Width="180px" AllowCustomText="True" EnableLoadOnDemand="True" OnItemsRequested="rdb_Groups_ItemsRequested">
                                </rad:RadComboBox>

                                <asp:TextBox ID="Txt_Groups" runat="server" Width="176px"></asp:TextBox>

                                <rad:RadComboBox ID="rdb_Outlet" runat="server" Width="180px" AllowCustomText="True">
                                    <ItemTemplate>
                                        <asp:CheckBox ID="cbbox" ToolTip='<%#Eval("WordId")??"" %>' runat="server" Text='<%#Eval("Word")??"" %>' onclick="checkOutletGroup(this)" />
                                    </ItemTemplate>
                                </rad:RadComboBox>
                            </td>
                        </tr>
                    </table>
                </ItemTemplate>
            </asp:DataList>

            <div style="margin-top: 20px; text-align: center;">
                <asp:LinkButton ID="LB_Inster" runat="server" OnClick="LbtnSetGoodsAttribute_OnClick">
                    <asp:Image ID="IB_Inster" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />确定
                </asp:LinkButton>
                <asp:LinkButton ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()">
                    <asp:Image ID="IB_Cancel" SkinID="CancelImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />取消
                </asp:LinkButton>
            </div>
        </asp:Panel>
        <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RAM">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Panel_GoodsAttribute" LoadingPanelID="Loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="LB_Inster">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Panel_GoodsAttribute" LoadingPanelID="Loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

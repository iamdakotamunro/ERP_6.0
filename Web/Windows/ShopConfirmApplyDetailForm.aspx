<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShopConfirmApplyDetailForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShopConfirmApplyDetailForm" %>
<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>要货确认</title>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script type="text/javascript" src="../JavaScript/common.js"></script>
        <script type="text/javascript">
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
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360"></rad:RadScriptManager>
    
    <table class="PanelArea">
        <tr>
            <td align="right" style="padding-right: 10px">
                <Ibt:ImageButtonControl runat="server" CausesValidation="false" ID="IbcConfirm"
                    OnClick="IbcConfirmOnClick" Text="发货确认" SkinType="Affirm"></Ibt:ImageButtonControl>
            </td>
        </tr>
    </table>
    <rad:RadGrid runat="server" ID="RgApplyStockDetail" SkinID="Common" OnNeedDataSource="RgApplyStockDetailOnNeedDataSource"
        OnItemDataBound="RgApplyStockDetailItemDataBound" >
        <MasterTableView DataKeyNames="ApplyId,GoodsId,Specification,Price,GoodsStock" CommandItemSettings-ShowAddNewRecordButton="false"
            AllowPaging="False">
            <CommandItemTemplate>
            </CommandItemTemplate>
            <Columns>
                <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Specification" HeaderText="商品SKU">
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Quantity" HeaderText="需求总数">
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="库存">
                    <ItemTemplate>
                        <asp:Label runat="server" ID="LB_Stock" Text='<%# Eval("GoodsStock")%>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="确认提示">
                        <ItemTemplate>
                            <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                                onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                            <div style="position: absolute;">
                                <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative;
                                    display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px;
                                    font-weight: bold; height: auto; overflow: visible; word-break: break-all;" runat="server">
                                    <%# Eval("ComfirmTips") %>
                                </div>
                            </div>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="ShopFilialeId" Display="False">
                </rad:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgApplyStockDetail" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RgApplyStockDetail">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgApplyStockDetail" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IbcConfirm">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgApplyStockDetail" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompleteOrderTaskExpressForm.aspx.cs" Inherits="ERP.UI.Web.Windows.CompleteOrderTaskExpressForm" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360"></rad:RadScriptManager>
        <rad:RadSkinManager ID="rsmSkin" runat="server" Skin="WebBlue"></rad:RadSkinManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/jquery.js" type="text/javascript"></script>
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/common.js" type="text/javascript"></script>
        </rad:RadScriptBlock>
        
        <rad:RadGrid ID="RGTaskExpress" SkinID="Common" AllowMultiRowSelection="true"
            OnNeedDataSource="RGTaskExpress_NeedDataSource"
            OnItemCommand="RGTaskExpress_ItemCommand"
            MasterTableView-CommandItemDisplay="None" AllowPaging="False"
            runat="server">
            <ClientSettings>
                <Selecting EnableDragToSelectRows="false" />
            </ClientSettings>
            <MasterTableView DataKeyNames="ExpressId" ClientDataKeyNames="ExpressId">
                <Columns>
                    <rad:GridTemplateColumn HeaderText="快递">
                        <ItemTemplate>
                            <asp:Label ID="lbExpress" runat="server" Text='<%# GetExpressName(new Guid(Eval("ExpressId").ToString())) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="数量">
                        <ItemTemplate>
                            <asp:Label ID="lbTotalQuantity" runat="server" Text='<%# Eval("TotalQuantity") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="操作">
                        <ItemTemplate>
                            <Ibt:ImageButtonControl ID="ibtnExportExcel" runat="server" SkinType="ExportExcel" CommandName="ExportExcel"></Ibt:ImageButtonControl>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" Width="180" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        
        <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RAM_AjaxRequest">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RGTaskExpress">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGTaskExpress" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

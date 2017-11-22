<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompleteOrderTaskDetailForm.aspx.cs" Inherits="ERP.UI.Web.Windows.CompleteOrderTaskDetailForm" %>

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
            <script>
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


        <rad:RadGrid ID="RGTaskDetails" SkinID="CustomPaging" AllowMultiRowSelection="true"
            OnNeedDataSource="RGTaskDetails_NeedDataSource"
            OnItemDataBound="RGTaskDetails_ItemDataBound"
            OnItemCommand="RGTaskDetails_ItemCommand"
            MasterTableView-CommandItemDisplay="None" AllowPaging="False"
            runat="server">
            <ClientSettings>
                <Selecting EnableDragToSelectRows="false" />
            </ClientSettings>
            <MasterTableView DataKeyNames="ID,OrderId" ClientDataKeyNames="ID,OrderId">
                <Columns>
                    <rad:GridTemplateColumn HeaderText="编号">
                        <ItemTemplate>
                            <asp:Label ID="lbNo" runat="server" Text='<%# Container.ItemIndex + 1%>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" Width="35" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="OrderNo" HeaderText="订单编号" UniqueName="OrderNo">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="销售公司">
                        <ItemTemplate>
                            <asp:Label ID="lbSaleFiliale" runat="server" Text='<%# GetSaleFilialeName(new Guid(Eval("SaleFilialeId").ToString())) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="ERP">
                        <ItemTemplate>
                            <asp:Label ID="lbIsSuccessERP" runat="server" Text='<%# bool.Parse(Eval("IsSuccessERP").ToString())?"✔":"✘" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="B2C">
                        <ItemTemplate>
                            <asp:Label ID="lbIsSuccessB2C" runat="server" Text='<%# bool.Parse(Eval("IsSuccessB2C").ToString())?"✔":"✘" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="库存中心">
                        <ItemTemplate>
                            <asp:Label ID="lbIsSuccessStock" runat="server" Text='<%# bool.Parse(Eval("IsSuccessStock").ToString())?"✔":"✘" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="促销中心">
                        <ItemTemplate>
                            <asp:Label ID="lbIsSuccessPromotion" runat="server" Text='<%# bool.Parse(Eval("IsSuccessPromotion").ToString())?"✔":"✘" %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="错误信息">
                        <ItemTemplate>
                            <asp:Label ID="lbTemp" runat="server" Text="无" Visible="False"></asp:Label>
                            <asp:ImageButton ID="ibtnDescription" SkinID="InsertImageButton" runat="server" OnClientClick="return false;"
                                keyid='<%# Eval("OrderNo") %>' Visible="False"
                                onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                            <div style="position: absolute;">
                                <div id="ImaDiv1" style="z-index: 50; left: -393px; top: -3px; position: relative; display: none; background-color: #CCFFFF; border: solid 1px #666; width: 400px; max-height: 300px; min-height: 0; font-weight: bold; overflow: visible; word-break: break-all; overflow-y: scroll"
                                    runat="server"
                                    onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                    onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'>
                                    <%#Eval("Description")%>
                                </div>
                            </div>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <br/>
        <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RAM_AjaxRequest">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RGTaskDetails">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RGTaskDetails" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

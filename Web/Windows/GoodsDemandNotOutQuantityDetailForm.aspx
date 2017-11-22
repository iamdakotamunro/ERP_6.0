<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="GoodsDemandNotOutQuantityDetailForm.aspx.cs" Inherits="ERP.UI.Web.Windows.GoodsDemandNotOutQuantityDetailForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowClewForm(orderId, saleFilialeId) {
                window.radopen("./ShowClewForm.aspx?OrderId=" + orderId + "&SaleFilialeId=" + saleFilialeId, "ClewWindow");
                return false;
            }
        </script>
    </rad:RadScriptBlock>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <div>
            <%--订单--%>
            <rad:RadGrid ID="RgGoodsOrder" runat="server" Skin="WebBlue" SkinID="Common" OnNeedDataSource="RgGoodsOrderNeedDataSource"
                AllowPaging="False" MasterTableView-CommandItemDisplay="None">
                <ClientSettings>
                </ClientSettings>
                <MasterTableView ClientDataKeyNames="OrderId,OrderNo,OrderState,PayMode,OrderTime" DataKeyNames="OrderId,OrderNo,OrderState,PayMode,OrderTime,SaleFilialeId">
                    <Columns>
                        <rad:GridBoundColumn DataField="OrderNo" HeaderText="订单编号" UniqueName="OrderNo">
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="Consignee" HeaderText="收获人" UniqueName="Consignee">
                            <HeaderStyle Width="80" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn HeaderText="联系电话" UniqueName="TemplateColumn2">
                            <ItemTemplate>
                                <asp:Label ID="Phone" runat="server" Text='<%# Eval("Phone")==null || string.IsNullOrEmpty(Eval("Phone").ToString()) ? "" : "电话："+Eval("Phone") %>'></asp:Label>
                                <br />
                                <asp:Label ID="Mobile" runat="server" Text='<%# Eval("Mobile")==null || string.IsNullOrEmpty(Eval("Mobile").ToString()) ? "" : "手机："+Eval("Mobile") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="160px" HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridBoundColumn DataField="Direction" HeaderText="收货地址" UniqueName="Direction">
                            <HeaderStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="ExpressNo" HeaderText="快递单号" UniqueName="ExpressNo">
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn HeaderText="支付方式">
                            <ItemTemplate>
                                <asp:Label ID="StockTypeLabel" runat="server" Text='<%# GetPayMode(Convert.ToInt32(Eval("PayMode"))) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle Width="80px" HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridBoundColumn DataField="RealTotalPrice" HeaderText="实收金额" UniqueName="RealTotalPrice">
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridBoundColumn DataField="Carriage" HeaderText="运费" UniqueName="Carriage">
                            <HeaderStyle Width="80" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridTemplateColumn DataField="OrderState" HeaderText="订单状态" UniqueName="OrderState">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemTemplate>
                                <asp:Label ID="OrderStateLabel" runat="server" Text='<%# GetOrderState(Convert.ToInt32(Eval("OrderState"))) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle Width="80px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="付言" UniqueName="Memo">
                    <ItemTemplate>
                        <asp:Image ID="ImageButton" runat="server" SkinID="MemoImg" Visible='<%# Eval("Memo")!=null &&  !string.IsNullOrEmpty(Eval("Memo").ToString()) %>'
                            ToolTip='<%# Eval("Memo") %>' />
                    </ItemTemplate>
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="管理意见" UniqueName="Clew">
                    <ItemTemplate>
                        <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                            OnClientClick='<%# "return ShowClewForm(\"" + Eval("OrderId") + "\",\"" + Eval("SaleFilialeId") + "\");" %>'
                            ToolTip='<%#GetMisClew(Eval("OrderId"))%>' />
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </rad:RadGrid>            

            <rad:RadWindowManager ID="ClewWindowManager" runat="server" Height="500" Width="700"
                ReloadOnShow="true">
                <Windows>
                    <rad:RadWindow ID="Clew" runat="server" Title="订单留言" Width="700" Height="500" />
                </Windows>
            </rad:RadWindowManager>
        </div>
    </form>
</body>
</html>

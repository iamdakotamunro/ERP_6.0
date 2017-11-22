<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.RequirementOrder"
    CodeBehind="RequirementOrder.aspx.cs" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<html>
<head runat="server">
    <title>商品需求订单</title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">

        <script type="text/javascript" language="javascript">
            function RowDblClick(obj, args) {
                window.radopen("./ShowGoodsOrder.aspx?OrderId=" + args.getDataKeyValue("OrderId") + '&Search=search', "GoodsOrder");
            }
            function ShowClewForm(orderId, SaleFilialeId) {
                window.radopen("ShowClewForm.aspx?OrderId=" + orderId + "&SaleFilialeId=" + SaleFilialeId, "SCF");
                return false;
            }
            function DemandQuantityRowDblClick(obj, args) {
                var id = args.getDataKeyValue("ID");
                var type = args.getDataKeyValue("type");
                if (type == 1) {
                    //订单
                    window.radopen("./ShowGoodsOrder.aspx?OrderId=" + id + '&Search=search', "GoodsOrder");
                }
                else if (type == 2) {
                    //加工单
                    window.radopen("./ShowLensProcessForm.aspx?Id=" + id, "LensProcessForm");
                }
                else if(type==3) {
                    //出库单
                    window.radopen("./EditSemiStockOutFormDoorShop.aspx?StockId=" + id, "SemiStockOutForm");
                }
            }
            function LensProcessRowDblClick(obj, args) {
                var id = args.getDataKeyValue("ID");
                window.radopen("./ShowLensProcessForm.aspx?Id=" + id, "LensProcessForm");
            }
            function ApplyStockRowDblClick(obj, args) {
                var applyId = args.getDataKeyValue("ApplyId");
                window.radopen("./ShopFrontApplyStockOut.aspx?applyid=" + applyId, "ApplyStockOut");
            }
            function clientShow(sender) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }
        </script>

    </rad:RadScriptBlock>
    <div>
        <rad:RadGrid ID="RGGoodsOrder" runat="server" Skin="WebBlue" SkinID="Common" AllowPaging="False" MasterTableView-CommandItemDisplay="None" >
            <ClientSettings>
                <ClientEvents OnRowDblClick="RowDblClick" />
            </ClientSettings>
            <MasterTableView DataKeyNames="OrderId" ClientDataKeyNames="OrderId">
                <Columns>
                    <rad:GridBoundColumn DataField="OrderNo" HeaderText="订单编号" UniqueName="OrderNo">
                        <HeaderStyle Width="140px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Consignee" HeaderText="收货人" UniqueName="Consignee" SortExpression="Consignee">
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Mobile" HeaderText="手机号" UniqueName="Mobile" SortExpression="Mobile">
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Direction" HeaderText="收货地址" UniqueName="Direction" AllowSorting="false">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="ExpressNo" HeaderText="快递单号" UniqueName="ExpressNo">
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="支付方式" UniqueName="PayMode">
                        <ItemTemplate>
                            <asp:Label ID="PayMode" runat="server" Text='<%# GetPayMode(Eval("PayMode")) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="90px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="RealTotalPrice" HeaderText="实收金额" UniqueName="RealTotalPrice">
                        <ItemTemplate>
                            <asp:Label ID="PriceLabel" runat="server" Text='<%# Convert.ToDecimal(Eval("RealTotalPrice")).ToString("N") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="运费" UniqueName="Price">
                        <ItemTemplate>
                            <asp:Label ID="CarriageLabel" runat="server" Text='<%# Eval("Carriage") %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="订单状态" UniqueName="OrderState">
                        <ItemTemplate>
                            <asp:Label ID="OrderState" runat="server" Text='<%# GetOrderState(Eval("OrderState")) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="付言" UniqueName="Memo">
                        <ItemTemplate>
                            <asp:Image ID="ImageButton" runat="server" SkinID="MemoImg" Visible='<%#Eval("Memo")!=null && !string.IsNullOrEmpty(Eval("Memo").ToString()) %>'
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
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
            <SortingSettings SortedAscToolTip="递增排序" SortedDescToolTip="递减排序" SortToolTip="单击排序" />
        </rad:RadGrid>
        
        <rad:RadGrid ID="RG_LensProcess" runat="server" Skin="WebBlue" SkinID="Common" AllowPaging="False" MasterTableView-CommandItemDisplay="None" >
            <ClientSettings>
                <Selecting EnableDragToSelectRows="false" />
                <ClientEvents OnRowDblClick="LensProcessRowDblClick"></ClientEvents>
            </ClientSettings>
            <MasterTableView DataKeyNames="ID" ClientDataKeyNames="ID">
                <CommandItemTemplate>
                </CommandItemTemplate>
                <Columns>
                    <rad:GridBoundColumn DataField="ID" HeaderText="ID" UniqueName="ID" Visible="false"></rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="ProcessNo" HeaderText="加工单号" UniqueName="ProcessNo" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center"></rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="操作状态" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="lbOperateState" Text='<%# EnumAttribute.GetKeyName((LensProcessOperateState)Eval("OperateState"))%>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="单据状态" HeaderStyle-HorizontalAlign="Center" ItemStyle-HorizontalAlign="Center">
                        <ItemTemplate>
                            <asp:Label ID="lbState" Text='<%# EnumAttribute.GetKeyName((LensProcessState)Eval("State"))%>' runat="server"></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="信息" UniqueName="Clew">
                        <ItemTemplate>
                            <asp:Image ID="ImageClew" SkinID="InsertImageButton" ToolTip='<%# Eval("Clew") %>' runat="server"/>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        
        <rad:RadGrid ID="RG_ApplyStockList" runat="server" Skin="WebBlue" SkinID="Common" AllowPaging="False" MasterTableView-CommandItemDisplay="None" >
            <ClientSettings>
                <Selecting EnableDragToSelectRows="false" />
                <ClientEvents OnRowDblClick="ApplyStockRowDblClick"></ClientEvents>
            </ClientSettings>
            <MasterTableView DataKeyNames="ApplyId" ClientDataKeyNames="ApplyId" CommandItemSettings-ShowAddNewRecordButton="false">
                <Columns>
                    <rad:GridBoundColumn DataField="TradeCode" ReadOnly="true" HeaderText="申请单号">
                        <HeaderStyle HorizontalAlign="Center" Width="100" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="SemiStockCode" ReadOnly="true" HeaderText="出库单据号">
                        <HeaderStyle HorizontalAlign="Center" Width="360" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="SubtotalQuantity" ReadOnly="true" HeaderText="数量">
                        <HeaderStyle HorizontalAlign="Center" Width="60" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="FilialeName" ReadOnly="true" HeaderText="申请门店">
                        <ItemStyle HorizontalAlign="Center" Width="120" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Transactor" ReadOnly="true" HeaderText="申请采购者">
                        <ItemStyle HorizontalAlign="Center" Width="120" />
                        <HeaderStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="WarehouseName" ReadOnly="true" HeaderText="申请仓库">
                        <HeaderStyle HorizontalAlign="Center" Width="100" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="PurchaseType" ReadOnly="true" HeaderText="采购类型">
                        <HeaderStyle HorizontalAlign="Center" Width="80" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# GetPurchaseTypeName(Eval("PurchaseType"))%>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="StockState" ReadOnly="true" HeaderText="状态">
                        <HeaderStyle HorizontalAlign="Center" Width="60" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# ReturnApplyState(Eval("StockState"))%>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <br/>
         <rad:RadGrid ID="RgGoodsNeed" runat="server" Skin="WebBlue" SkinID="Common" AllowPaging="False"
          OnNeedDataSource="RgGoodsNeedDataSource" MasterTableView-CommandItemDisplay="None" >
            <ClientSettings>
                <Selecting EnableDragToSelectRows="false" />
            </ClientSettings>
            <MasterTableView DataKeyNames="Key" ClientDataKeyNames="Key" CommandItemSettings-ShowAddNewRecordButton="false">
                <Columns>
                    <rad:GridBoundColumn DataField="Key" ReadOnly="true" HeaderText="订单编号">
                        <HeaderStyle HorizontalAlign="Center" Width="100" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Value" ReadOnly="true" HeaderText="占用数">
                        <HeaderStyle HorizontalAlign="Center" Width="360" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <br />
        <rad:RadGrid ID="rgDemandQuantity" runat="server" Skin="WebBlue" SkinID="Common"  AllowPaging="False" OnNeedDataSource="RgDemandQuantity_OnNeedDataSource" MasterTableView-CommandItemDisplay="None">
            <ClientSettings>
                <ClientEvents OnRowDblClick="DemandQuantityRowDblClick" />
            </ClientSettings>
            <MasterTableView DataKeyNames="ID,type" ClientDataKeyNames="ID,type">
                <Columns>
                    <rad:GridTemplateColumn HeaderText="类型" UniqueName="type">
                        <ItemTemplate>
                            <asp:Label ID="lbType" runat="server" Text='<%# GetType(Eval("type")) %>'></asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="70px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="No" HeaderText="编号" UniqueName="No">
                        <HeaderStyle Width="140px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>

        <rad:RadWindowManager ID="ClewWindowManager" runat="server" Title="订单查询" Height="450px" Width="920px" ReloadOnShow="true">
            <Windows>
                <rad:RadWindow ID="SCF" Width="700" Height="500" Title="备注" runat="server"></rad:RadWindow>
                <rad:RadWindow ID="LensProcessForm" Width="700" Height="500" Title="加工单详细" runat="server"></rad:RadWindow>
                <rad:RadWindow ID="SemiStockOutForm" Width="900" Height="500" Title="查看调拨出库" runat="server"></rad:RadWindow>
                <rad:RadWindow ID="ApplyStockOut" runat="server" Title="门店调拨申请处理" OnClientShow="clientShow" />
            </Windows>
        </rad:RadWindowManager>
    </div>
    </form>
</body>
</html>

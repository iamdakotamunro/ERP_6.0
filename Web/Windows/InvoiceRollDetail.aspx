<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InvoiceRollDetail.aspx.cs"
    Inherits="ERP.UI.Web.Windows.InvoiceRollDetail" %>
<%@ Import Namespace="Framework.Common" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>发票分卷信息</title>
    <script type="text/javascript" src="../JavaScript/jquery.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <div style="text-align: center; padding-top: 5px; padding-bottom: 5px;">
            <asp:Button ID="btn_Distribution" runat="server" Text="批量分发" ForeColor="red" OnClientClick="return Check();" OnClick="btn_Distribution_Click" />
        </div>
        <rad:RadGrid ID="RadGrid_InvoiceRollDetailList" runat="server" SkinID="Common" MasterTableView-CommandItemDisplay="None"
            OnNeedDataSource="RadGrid_InvoiceRollDetailList_NeedDataSource" OnItemCommand="RadGrid_InvoiceRollDetailList_ItemCommand"
            AllowMultiRowSelection="True">
            <MasterTableView DataKeyNames="RollId,StartNo,EndNo,State">
                <Columns>
                    <rad:GridClientSelectColumn UniqueName="column">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                    </rad:GridClientSelectColumn>
                    <rad:GridTemplateColumn DataField="StartNo" UniqueName="StartNo" HeaderText="发票卷起始号码">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="Label_Start" runat="server" Text='<%# Convert.ToInt64(Eval("StartNo")).ToMendString(8) %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="EndNo" UniqueName="EndNo" HeaderText="发票卷结束号码">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="Label_End" runat="server" Text='<%# Convert.ToInt64(Eval("EndNo")).ToMendString(8) %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="State" UniqueName="State" HeaderText="发票卷启用状态">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:Label ID="Label_State" runat="server" Text='<%#GetState(Eval("State")) %>'></asp:Label>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="Remark" Display="false" UniqueName="" HeaderText="备注">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:ImageButton SkinID="AddImageButton" ToolTip='<%#Eval("Remark") %>' runat="server" />
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="遗失上报">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <asp:ImageButton runat="server" ID="IB_Lost" SkinID="UpdateImageButton" OnClientClick="return confirm('确定上报？')"
                                CommandName="Lost" Visible='<%#IsVisble(Eval("State"),Eval("IsSubmit")) %>' />
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <rad:RadWindowManager runat="server" ReloadOnShow="true" EnableShadow="true">
            <Windows>
                <rad:RadWindow runat="server" ID="RadWindow_Distribute" Width="330" Height="220">
                </rad:RadWindow>
            </Windows>
        </rad:RadWindowManager>
        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RadGrid_InvoiceRollDetailList">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RadGrid_InvoiceRollDetailList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
                <rad:AjaxSetting AjaxControlID="RAM">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RadGrid_InvoiceRollDetailList" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>

    <script type="text/javascript">
        function Distribute(startNo, endNo) {
            var rollId = '<%=InvoiceRollId %>';
            var owd = window.radopen("/windows/InvoiceRollDistribute.aspx?rollid=" + rollId + "&startno=" + startNo + "&endno=" + endNo, "RadWindow_Distribute");
        }

        function refreshGrid(arg) {
            if (!arg) {
                $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
            }
            else {
                $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
            }
        };

        function RebindGird() {
            var tableView = $find("<%= RadGrid_InvoiceRollDetailList.ClientID %>").get_masterTableView();
            tableView.rebind();
            $find("<%= loading.ClientID %>").show();
        };

        //验证是否选中发票
        function Check() {
            var selectLength = $(".rgSelectedRow input[type='checkbox']:checked").length;
            if (selectLength == 0) {
                alert("请选择发票卷！");
                return false;
            }
            return true;
        }
    </script>

</body>
</html>

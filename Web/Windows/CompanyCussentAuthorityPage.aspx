<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyCussentAuthorityPage.aspx.cs" Inherits="ERP.UI.Web.Windows.CompanyCussentAuthorityPage" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <script src="../JavaScript/jquery.js"></script>
</head>
<body>
    <form id="form1" runat="server">
        <script type="text/javascript">
            function ShowAdd() {
                var companyId = $("#<%=HiddenField1.ClientID %>").val();
                window.radopen("./CompanyCussentAuthorityAdd.aspx?companyId=" + companyId, "RW1");
                return false;
            }
            function ShowEdit(companyId, accountNo) {
                window.radopen("./CompanyCussentAuthorityAdd.aspx?type=1&companyId=" + companyId + "&accountNo=" + accountNo, "RW2");
                return false;
            }

            //重新绑定Grid事件
            function refreshGrid() {
                window.location.reload();
            }

            function OnDeleteConfirm() {
                var conf = window.confirm('提示：是否确认删除吗？');
                if (!conf)
                    return false;
                return true;
            }
        </script>
        <div style="padding: 10px 0 10px 0;">
            <table width="100%">
                <tr>
                    <td style="width: 50%;">
                        <asp:HiddenField ID="HiddenField1" runat="server" />
                    </td>
                    <td style="width: 50%; text-align: right;">
                        <asp:Button runat="server" ID="Btn_Add" Text="添加" OnClientClick="return ShowAdd()" />
                    </td>
                </tr>
            </table>
        </div>
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadGrid ID="RG_CompanyCussentAuthority" runat="server" PageSize="20" OnNeedDataSource="RG_CompanyCussentAuthority_NeedDataSource" OnItemCommand="RG_CompanyCussentAuthority_OnItemCommand">
            <MasterTableView DataKeyNames="AccountNo,CompanyId,SaleFilialeId">
                <CommandItemTemplate>
                </CommandItemTemplate>
                <CommandItemStyle Height="0px" />
                <Columns>
                    <rad:GridBoundColumn DataField="AccountName" HeaderText="账号">
                        <HeaderStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="CompanyName" HeaderText="往来单位名称">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="SaleFilialeName" HeaderText="销售公司">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="操作" UniqueName="Approved">
                        <ItemTemplate>
                            <asp:ImageButton ID="IB_Approved" runat="server" SkinID="EditImageButton" OnClientClick='<%# "return ShowEdit(\"" + Eval("CompanyId") + "\",\"" + Eval("AccountNo") + "\");" %>' />
                            <asp:ImageButton ID="ImageButton1" runat="server" SkinID="DeleteImageButton" CommandName="Delete" />
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
        <rad:RadAjaxManager ID="RAM" runat="server"></rad:RadAjaxManager>
        <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
            <Windows>
                <rad:RadWindow ID="RW1" runat="server" Title="添加境外往来单位权限" Width="980" Height="500" />
                <rad:RadWindow ID="RW2" runat="server" Title="修改往来账号权限" Width="980" Height="500" />
            </Windows>
        </rad:RadWindowManager>
    </form>
</body>
</html>

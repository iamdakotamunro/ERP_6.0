<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManuallyCheckBillDetail.aspx.cs" Inherits="ERP.UI.Web.Windows.ManuallyCheckBillDetail" %>

<%@ Import Namespace="ERP.SAL.MemberCenterSAL" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360">
        </rad:RadScriptManager>

        <table style="width: 100%;">
            <tr>
                <td style="text-align: right; width: 115px;">系统/第三方订单号：
                </td>
                <td style="width: 175px;">
                    <asp:TextBox ID="txt_OrderNo" runat="server"></asp:TextBox>
                </td>
                <td style="width: 50px;">
                    <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
                </td>
                <div id="UploadConfirm" runat="server">
                    <td style="text-align: right;">财务确认表：
                    </td>
                    <td style="width: 300px;">
                        <asp:TextBox ID="UploadExcelName" runat="server" onfocus="this.blur();"></asp:TextBox>
                        <asp:FileUpload ID="UploadExcel" runat="server" Style="display: none;" onchange="CheckFile()" />
                        <input type="button" value="选择文件" title="文件格式(.xls)!" onclick="SelectFile(this);" />
                        <asp:Button ID="btn_Upload" runat="server" Text="导入" />
                    </td>
                </div>
                <td>
                    <asp:Button ID="btn_Export" runat="server" Text="导出" />
                </td>
            </tr>
        </table>

        <rad:RadGrid ID="RG_ManuallyCheckBillDetail" runat="server" ShowFooter="true" OnNeedDataSource="RG_ManuallyCheckBillDetail_NeedDataSource">
            <MasterTableView>
                <CommandItemTemplate>
                </CommandItemTemplate>
                <CommandItemStyle Height="0px" />
                <Columns>
                    <rad:GridBoundColumn DataField="SystemOrderNo" HeaderText="系统订单号">
                        <HeaderStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="ThirdOrderNo" HeaderText="第三方订单号">
                        <HeaderStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="下单时间">
                        <ItemTemplate>
                            <%#Convert.ToDateTime(Eval("OrderTime")).ToString("yyyy-MM-dd").Equals("1900-01-01")?"":Eval("OrderTime")%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="会员">
                        <ItemTemplate>
                            <%#MemberCenterSao.GetUserBase(new Guid(Request.QueryString["SalePlatformId"]), new Guid(Eval("MemberId").ToString()))%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="系统订单金额">
                        <ItemTemplate>
                            <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("SystemOrderAmount"))) %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="第三方订单金额">
                        <ItemTemplate>
                            <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("ThirdOrderAmount"))) %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="差额">
                        <ItemTemplate>
                            <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("Balance"))) %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="财务确认金额">
                        <ItemTemplate>
                            <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("ConfirmAmount"))) %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="往来账差异">
                        <ItemTemplate>
                            <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("ContactsReckoningDifference"))) %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </form>
</body>
</html>

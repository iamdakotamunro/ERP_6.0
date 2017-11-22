<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowProportionForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShowProportionForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="/JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        </rad:RadScriptBlock>
        <table width="100%">
            <tr>
                <td>
                    <rad:RadGrid ID="RgRecord" runat="server" SkinID="Common">
                        <MasterTableView DataKeyNames="Id">
                            <CommandItemTemplate>
                            </CommandItemTemplate>
                            <Columns>
                                <rad:GridBoundColumn DataField="Id" HeaderText="序号" UniqueName="Id">
                                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridBoundColumn>
                                <rad:GridBoundColumn DataField="UpdateDate" HeaderText="操作时间" UniqueName="UpdateDate">
                                    <HeaderStyle Width="180px" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridBoundColumn>
                                <rad:GridBoundColumn DataField="Operator" HeaderText="操作人" UniqueName="Operator">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridBoundColumn>
                                <rad:GridBoundColumn DataField="OperateType" HeaderText="操作动作" UniqueName="OperateType">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridBoundColumn>
                                <rad:GridBoundColumn DataField="CurrentTaxrate" HeaderText="当前税率" UniqueName="CurrentTaxrate">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridBoundColumn>
                                <rad:GridBoundColumn DataField="Remark" HeaderText="操作内容" UniqueName="Remark">
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridBoundColumn>
                            </Columns>
                        </MasterTableView>
                    </rad:RadGrid>
                </td>
            </tr>
            <tr>
                <td style="text-align: center;">
                    <asp:Button runat="server" Text="确认" Width="60" ID="BtnOk" OnClick="BtnOkClick"/>
                </td>
            </tr>
        </table>
        
        <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.ShowReckoning" CodeBehind="ShowReckoning.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    </rad:RadScriptBlock>
    
    <table width="100%">
<%--        <tr>
            <td>
                往来公司：
            </td>
            <td colspan="2">
                <rad:RadComboBox ID="RcbFiliale" runat="server" UseEmbeddedScripts="false" Width="150px"
                        Height="100px" OnSelectedIndexChanged="RcbFilialeOnSelectedIndexChanged"
                        AutoPostBack="True" EmptyMessage="请选择销售公司">
                    </rad:RadComboBox>
            </td>
        </tr>
        <tr>
            <td>
                备注说明：
            </td>
            <td>
                <asp:TextBox ID="TbDescription" runat="server" TextMode="MultiLine" SkinID="LongTextarea"
                    Rows="3" Width="250px"></asp:TextBox>
            </td>
            <td>
                <asp:Button runat="server" ID="BtnAdd" OnClick="BtnAddOnClick" Text="添加" Enabled ="False"/>
            </td>
        </tr>--%>
        <tr>
            <td colspan="3">
                <rad:RadGrid ID="RgSubjectDiscount" runat="server" SkinID="Common_Foot"
        OnNeedDataSource="RgSubjectDiscountNeedDataSource" OnPageIndexChanged="RgSubjectDiscountOnPageIndexChanged">
        <MasterTableView DataKeyNames="ID" ClientDataKeyNames="ID">
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <CommandItemTemplate>
            </CommandItemTemplate>
            <Columns>
                <rad:GridBoundColumn DataField="CompanyName" HeaderText="往来单位" UniqueName="CompanyName">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="FilialeId" HeaderText="公司名称" UniqueName="FilialeId">
                    <ItemTemplate>
                        <span style="font-weight: bold;">
                            <%# GetFilialeName(Eval("FilialeId")) %></span>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="PersonnelName" HeaderText="创建人" UniqueName="PersonnelName">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridDateTimeColumn DataField="DateCreated" HeaderText="创建日期" UniqueName="DateCreated">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridDateTimeColumn>
                <rad:GridBoundColumn DataField="Income" HeaderText="折扣金额" UniqueName="Income" DataFormatString="{0:F}">
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Memo" HeaderText="备注" UniqueName="Memo">
                    <HeaderStyle Width="250px" HorizontalAlign="Center" />
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
            </td>
        </tr>    
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BtnAdd">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgSubjectDiscount" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

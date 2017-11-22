<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowPurchasingAdjustForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShowPurchasingAdjustForm" %>
<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl" Src="~/UserControl/ImageButtonControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>

    <div id="GoodsPanel" style="background-color: #FFFFFF; width: 100%; height: 205px;left: 0px; position: absolute; top: 0px;">
        <rad:RadGrid ID="RgPurchingAdjust" runat="server" 
            OnItemDataBound="RgPurchingAdjust_ItemDataBound" OnNeedDataSource="RgPurchingAdjust_OnNeedDataSource"
            AllowMultiRowSelection="true" Width="100%" AllowPaging="false" >
            <MasterTableView DataKeyNames="LogId,GoodsID,GoodsName,Statue" ClientDataKeyNames="LogId,GoodsID,GoodsName">
                <CommandItemTemplate>
                    <asp:LinkButton ID="LB_Auditing" runat="server" OnClientClick="return confirm('确定要通过吗？');" Visible='<%# AllowAudit %>' OnClick="LbAuditingOnClick">
                        <asp:Image ID="IB_Auditing" SkinID="AffirmImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />通过
                    </asp:LinkButton>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <asp:LinkButton ID="LB_NotAudit" runat="server" OnClientClick="return confirm('确定要不通过吗？');" Visible='<%# AllowAudit %>'  OnClick="LbNoAuditingOnClick">
                        <asp:Image ID="IB_NotAudit" SkinID="CancelImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />不通过
                    </asp:LinkButton>
                    &nbsp;&nbsp;&nbsp;&nbsp;
                    <Ibt:ImageButtonControl ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid"
                                            SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl>
                </CommandItemTemplate>
                <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                <Columns>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名" UniqueName="GoodsName">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="OldValue" HeaderText="原采购价" UniqueName="OldValue">
                        <ItemTemplate>
                            <asp:Literal ID="LitOldValue" runat="server"></asp:Literal>
                        </ItemTemplate>
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="ChangeValue" HeaderText="调价" UniqueName="ChangeValue">
                        <ItemTemplate>
                            <asp:Literal ID="LitChangeValue" runat="server"></asp:Literal>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="NewValue" HeaderText="采购价" UniqueName="NewValue">
                        <ItemTemplate>
                            <asp:Literal ID="LitNewValue" runat="server"></asp:Literal>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="Statue" HeaderText="状态" UniqueName="Statue">
                        <ItemTemplate>
                            <asp:Literal ID="LitStatue" runat="server"></asp:Literal>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="ChangeReason" HeaderText="变更事由" UniqueName="ChangeReason">
                        <ItemTemplate>
                            <%#Eval("ChangeReason") %>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="ChangeDate" HeaderText="变更日期" UniqueName="ChangeDate" HtmlEncode="false" DataFormatString="{0:yyyy-MM-dd}">
                        <HeaderStyle Width="80px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                   <rad:GridTemplateColumn DataField="Applicant" HeaderText="执行人" UniqueName="Applicant">
                        <ItemTemplate>
                            <%# GetPersonName(new Guid(Eval("Applicant").ToString()))%>
                        </ItemTemplate>
                        <HeaderStyle Width="60px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

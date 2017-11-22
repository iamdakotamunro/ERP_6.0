<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddComparePro.aspx.cs" Inherits="ERP.UI.Web.Windows.AddComparePro" %>
<html>
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form runat="server">
    
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="/JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <table class="StagePanel" align="center">
        <tr height="40px">
            <td colspan="4">
                <asp:Label ID="lbInfo" runat="server"></asp:Label>
            </td>
            <td>
                <asp:TextBox ID="tbGoodsName" runat="server" Width="300px"></asp:TextBox>
                <asp:Button ID="btnSearch" Text="搜索" OnClick="BtnSearch_Click" runat="server" />
                <asp:Button ID="btnGoodsBind" Text="提交绑定" OnClick="BtnGoodsBind_Click" runat="server" />
            </td>
            <td style="width:200px;">
                <asp:Button ID="btnCloseRefresh" Text="关闭" OnClick="btnCloseRefresh_Click" runat="server" />
            </td>
        </tr>
    </table>
    <div id="divSearch" runat="server" Visible="False">
        <rad:RadGrid ID="rgFetchDataSearch" OnNeedDataSource="RgFetchDataSearch_NeedDataSource" AllowMultiRowSelection="true" SkinID="CustomPaging" runat="server">
            <ClientSettings>
                <Selecting AllowRowSelect="true" EnableDragToSelectRows="true" />
            </ClientSettings>
            <MasterTableView CommandItemDisplay="None" DataKeyNames="Id,SiteId,IsChecked">
                <Columns>
                    <rad:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="Id" Display="False"></rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="SiteId" HeaderText="SiteId" UniqueName="SiteId" Display="False"></rad:GridBoundColumn>
                    <rad:GridClientSelectColumn UniqueName="column">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridClientSelectColumn>
                    <rad:GridBoundColumn DataField="SiteName" HeaderText="网站名称" UniqueName="SiteName">
                        <HeaderStyle Width="58px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName"></rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsPrice" HeaderText="商品价格" UniqueName="GoodsPrice"></rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="商品URL" UniqueName="GoodsUrl">
                        <ItemTemplate>
                            <a href='<%#Eval("GoodsUrl") %>' target="blank" style="color: blue;"><%#Eval("GoodsUrl") %></a>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="GoodsGuId" HeaderText="GoodsGuId" UniqueName="GoodsGuId" Display="False"></rad:GridBoundColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>
    
    <rad:RadGrid ID="rgFetchData" OnNeedDataSource="RgFetchData_NeedDataSource" OnUpdateCommand="RgFetchData_UpdateCommand" SkinID="Common" runat="server">
        <MasterTableView CommandItemDisplay="None" DataKeyNames="Id,SiteId,IsChecked" EditMode="InPlace">
            <Columns>
                <rad:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="Id" Display="False"></rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="SiteId" HeaderText="SiteId" UniqueName="SiteId" Display="False"></rad:GridBoundColumn>
                <rad:GridTemplateColumn>
                    <ItemTemplate>
                        <asp:CheckBox ID="cbInteraction" Checked='<%#Eval("IsChecked")%>' Enabled="False" runat="server" />
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="cbIsChecked" Checked='<%#Eval("IsChecked")%>' runat="server" />
                    </EditItemTemplate>
                    <HeaderStyle Width="40px" HorizontalAlign="Center"/>
                    <ItemStyle Width="40px" HorizontalAlign="Center"/>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="网站名称" UniqueName="SiteName">
                    <ItemTemplate>
                        <%#Eval("SiteName")%>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <%#Eval("SiteName")%>
                    </EditItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                    <ItemStyle Width="50px" HorizontalAlign="Center"/>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="商品名称" UniqueName="GoodsName">
                    <ItemTemplate>
                        <%#Eval("GoodsName")%>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbGoodsName" Text='<%#Eval("GoodsName")%>' Width="340" runat="server"/>
                        <asp:RequiredFieldValidator ID="RFVGoodsName" runat="server" ControlToValidate="tbGoodsName" ErrorMessage="请填写商品名称" Text="*" />
                    </EditItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="商品价格" UniqueName="GoodsPrice">
                    <ItemTemplate>
                        <%#Eval("GoodsPrice")%>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbPrice" Text='<%#Eval("GoodsPrice")%>' Width="45" runat="server"/>
                        <asp:RequiredFieldValidator ID="RFVPrice" runat="server" ControlToValidate="tbPrice" ErrorMessage="请填写商品价格" Text="*" />
                    </EditItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center"/>
                    <ItemStyle Width="60px" HorizontalAlign="Center"/>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="商品URL" UniqueName="GoodsUrl">
                    <ItemTemplate>
                        <a href='<%#Eval("GoodsUrl") %>' target="blank" style="color: blue;"><%#Eval("GoodsUrl") %></a>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="tbGoodsUrl" Text='<%#Eval("GoodsUrl")%>' Width="600" runat="server"/>
                        <asp:RequiredFieldValidator ID="RFVGoodsUrl" runat="server" ControlToValidate="tbGoodsUrl" ErrorMessage="请填写商品URL" Text="*" />
                    </EditItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridEditCommandColumn HeaderText="操作" ButtonType="ImageButton" EditText="编辑" UpdateText="更新" CancelText="取消">
                    <HeaderStyle Width="50px" HorizontalAlign="Center"/>
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridEditCommandColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="btnSearch">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="divSearch"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="rgFetchDataSearch" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnGoodsBind">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="divSearch"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="rgFetchData" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="rgFetchDataSearch" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="rgFetchData">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rgFetchData" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>
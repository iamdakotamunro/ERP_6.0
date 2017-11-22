<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.BrandAw" Title="Untitled Page" CodeBehind="Brand.aspx.cs" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowInsertForm() {
                window.radopen("./Windows/EditBrandForm.aspx", "Insert");
                return false;
            }

            function RowDblClick(obj, args) {
                window.radopen("./Windows/EditBrandForm.aspx?BrandId=" + args.getDataKeyValue("BrandId") + "&OrderIndex=" + args.getDataKeyValue("OrderIndex"), "Edit");
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }

            function ShowImg(obj) {
                var obj = eval(obj);
                obj.style.display = "block";
            }

            function HiddleImg(obj) {
                var obj = eval(obj);
                obj.style.display = "none";
            }
        </script>
    </rad:RadScriptBlock>

    <table width="100%">
        <tr>
            <td style="width: 200px">品牌资料：<asp:DropDownList ID="DDL_HaveInformation" runat="server" Width="100px">
                <asp:ListItem Text="全部" Value="-1"></asp:ListItem>
                <asp:ListItem Text="无资料品牌" Value="0"></asp:ListItem>
                <asp:ListItem Text="有资料品牌" Value="1"></asp:ListItem>
            </asp:DropDownList>
            </td>
            <td style="width: 400px">品牌名称：<asp:TextBox ID="txt_Brand" runat="server" Width="300px"></asp:TextBox>
            </td>
            <td style="width: 100px">
                <asp:Button ID="btn_Search" runat="server" Text="搜索" OnClick="btn_Search_Click" />
            </td>
            <td>
                <asp:LinkButton ID="LB_AddRecord" runat="server" OnClientClick="return ShowInsertForm();">
                    <asp:Image ID="Image2" SkinID="AddImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />添加品牌
                </asp:LinkButton>
            </td>
            <td style="text-align: right;">
                <asp:LinkButton ID="LB_Refresh" runat="server" OnClick="Lb_Refresh_Click">
                    <asp:Image ID="IB_Refresh" SkinID="RefreshImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />刷新
                </asp:LinkButton>
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="BrandGrid" runat="server" SkinID="Common_Foot" OnItemCommand="BrandGrid_ItemCommand"
        OnNeedDataSource="BrandGrid_NeedDataSource" OnDeleteCommand="BrandGrid_DeleteCommand">
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>
        <MasterTableView DataKeyNames="BrandId,OrderIndex" ClientDataKeyNames="BrandId,OrderIndex" EditMode="EditForms" DataMember="Brand">
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="0px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="序号">
                    <ItemTemplate>
                        <%#(int)DataBinder.Eval(Container, "DataSetIndex")+1%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="品牌名称" UniqueName="Brand">
                    <ItemTemplate>
                        <asp:Label ID="Lab_Brand" runat="server" Text='<%# Eval("Brand") %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="品牌商标" UniqueName="BrandLogo">
                    <ItemTemplate>
                        <asp:Image ID="Image1" runat="server" SkinID="ShowPicImg" onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv").ClientID + "\")" %>'
                            onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv").ClientID + "\")" %>' />
                        <div style="position: absolute">
                            <div id="ImaDiv" style="z-index: 1000; left: 0px; top: 5px; position: relative; display: none;" runat="server">
                                <asp:Image ID="GoodsImg" runat="server" ImageUrl='<%# Eval("BrandLogo") == null ? "~/App_Themes/default/images/WantImg.gif" : ResourceServerInformation+Eval("BrandLogo").ToString().Replace("~",string.Empty)  %>' />
                            </div>
                        </div>
                    </ItemTemplate>
                    <HeaderStyle Width="250px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="排序">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_OrderIndex" runat="server" Text='<%#Eval("OrderIndex")%>' OnTextChanged="txt_OrderIndex_OnTextChanged" AutoPostBack="True"></asp:TextBox>
                    </ItemTemplate>
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" HeaderText="删除" Text="删除" ConfirmText="确实要删除吗？" UniqueName="Delete">
                    <HeaderStyle Width="40px" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridButtonColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="WMBrand" runat="server" Height="470px" Width="780px" ReloadOnShow="true">
        <AlertTemplate>test</AlertTemplate>
        <Windows>
            <rad:RadWindow ID="Insert" runat="server" Title="添加商品品牌"></rad:RadWindow>
            <rad:RadWindow ID="Edit" runat="server" Title="编辑商品品牌"></rad:RadWindow>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="BrandGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BrandGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="BrandGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="BrandGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="LB_Refresh">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="BrandGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

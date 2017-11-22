<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="ExcelTemplatePage.aspx.cs" Inherits="ERP.UI.Web.ExcelTemplatePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script>
            function AddExcelTemplatePage() {
                window.radopen("./Windows/AddExcelTemplatePageForm.aspx", "AddExcelTemplatePageForm");
                return false;
            }
        </script>
    </rad:RadScriptBlock>
    <script type="text/javascript">
        $(function() {
            $("input[id$='btn_Search']").css("display", "none");
        });
        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
    </script>
    <table width="100%" style="background: #F4F4F4;">
        <tr>
            <td width="40">仓库：</td>
            <td> <rad:RadComboBox ID="RCB_Stock" runat="server" Width="150px"  AutoPostBack="True" OnSelectedIndexChanged="RCB_Stock_SelectedIndexChanged" >
                            </rad:RadComboBox>
                <asp:Button ID="btn_Search" runat="server"  OnClick="btn_Search_Click"/>
            </td>
        </tr>
    </table>
    <rad:RadGrid ID="TempRadGrid" runat="server" SkinID="Common_Foot" AllowMultiRowSelection="True"
        OnNeedDataSource="TempRadGrid_NeedDataSource" OnUpdateCommand="TempRadGrid_UpdateCommand"
        OnDeleteCommand="TempRadGrid_DeleteCommand" OnInsertCommand="TempRadGrid_InsertCommand">
        <MasterTableView DataKeyNames="TempId" EditMode="InPlace">
            <CommandItemTemplate>
                <asp:LinkButton ID="LinkButtonAddRecord" runat="server" OnClientClick="AddExcelTemplatePage();"
                    Visible='<%# !TempRadGrid.MasterTableView.IsItemInserted %>'>
                    <asp:Image ID="AddRecord" runat="server" ImageAlign="AbsMiddle" SkinID="AddImageButton" />
                    添加模板</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="LinkButtonDeleteSelected" OnClientClick="javascript:return confirm('确实要删除选定的组吗?')"
                    runat="server" CommandName="DeleteSelected">
                    <asp:Image ID="ImageButtonDeleteSelected" runat="server" ImageAlign="AbsMiddle" SkinID="DeleteImageButton" />
                    删除</asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid">
                    <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />
                    刷新</asp:LinkButton>
            </CommandItemTemplate>
            <Columns >
                <rad:GridClientSelectColumn UniqueName="column">
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign=Center  VerticalAlign=Top/>
                </rad:GridClientSelectColumn>
                 <rad:GridTemplateColumn HeaderText="订货仓库" UniqueName="Remarks">
                    <ItemTemplate>
                        <asp:Label ID="lbWarehouseId" runat="server" Text='<%# GetWarehouseName(Eval("WarehouseId").ToString()) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle Width="160px" HorizontalAlign="Center"  />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign=Top />
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn HeaderText="订货单位" UniqueName="TemplateName">
                    <ItemTemplate>
                        <asp:Label ID="lbCustomer" runat="server" Text='<%# Eval("Customer") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" Text='<%# Eval("Customer") %>' ID="TB_Customer"  TextMode="MultiLine" Height="35px" Width="200px"
                            CssClass="StandardInput"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="CustomerCodeRequiredFieldValidator" runat="server"
                            ErrorMessage="客户名称不能为空！" Text="*" ControlToValidate="TB_Customer"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center"  VerticalAlign=Top />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="模板名称" UniqueName="TemplateName">
                    <ItemTemplate>
                        <asp:Label ID="lbTemplateName" runat="server" Text='<%# Eval("TemplateName") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" Text='<%# Eval("TemplateName") %>' ID="TB_TemplateName"  TextMode="MultiLine" Height="35px" Width="200px"
                            CssClass="StandardInput"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="TemplateNameCodeRequiredFieldValidator" runat="server"
                            ErrorMessage="Excel模板名称不能为空！" Text="*" ControlToValidate="TB_TemplateName"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign=Center/>
                    <ItemStyle HorizontalAlign="Center"  VerticalAlign=Top />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="收货人及联系方式" UniqueName="Shipper">
                    <ItemTemplate>
                        <asp:Label ID="lbShipper" runat="server" Text='<%# Eval("Shipper") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" Text='<%# Eval("Shipper") %>' ID="TB_Shipper"  TextMode="MultiLine" Height="35px" Width="200px"  CssClass="StandardInput"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="ShipperCodeRequiredFieldValidator" runat="server"
                            ErrorMessage="收货人及联系方式不能为空！" Text="*" ControlToValidate="TB_Shipper"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <HeaderStyle Width="160px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center"  VerticalAlign=Top />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="联系人及电话" UniqueName="ContactPerson">
                    <ItemTemplate>
                        <asp:Label ID="lbContactPerson" runat="server" Text='<%# Eval("ContactPerson") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" Text='<%# Eval("ContactPerson") %>' ID="TB_ContactPerson"
                            TextMode="MultiLine" Height="35px" Width="200px" CssClass="StandardInput"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="ContactPersonCodeRequiredFieldValidator" runat="server"
                            ErrorMessage="联系人及电话不能为空！" Text="*" ControlToValidate="TB_ContactPerson"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <HeaderStyle Width="160px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center"  VerticalAlign=Top />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="联系地址" UniqueName="ContactAddress">
                    <ItemTemplate>
                        <asp:Label ID="lbContactAddress" runat="server" Text='<%# Eval("ContactAddress") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" Text='<%# Eval("ContactAddress") %>' ID="TB_ContactAddress"
                            TextMode="MultiLine" Height="35px" Width="200px" CssClass="StandardInput"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="ContactAddressCodeRequiredFieldValidator" runat="server"
                            ErrorMessage="联系地址不能为空！" Text="*" ControlToValidate="TB_ContactAddress"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <HeaderStyle Width="160px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center"  VerticalAlign=Top />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注" UniqueName="Remarks">
                    <ItemTemplate>
                        <asp:Label ID="lbRemarks" runat="server" Text='<%# Eval("Remarks") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox runat="server" Text='<%# Eval("Remarks") %>' ID="TB_Remarks" TextMode="MultiLine"
                            Height="35px" Width="200px" CssClass="StandardInput"></asp:TextBox>
                    </EditItemTemplate>
                    <HeaderStyle Width="160px" HorizontalAlign="Center"  />
                    <ItemStyle HorizontalAlign="Center" VerticalAlign=Top />
                </rad:GridTemplateColumn>
                <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" EditText="编辑"
                    InsertText="添加" UpdateText="更新">
                    <HeaderStyle Width="50px" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridEditCommandColumn>
                <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="删除" ConfirmText="确实要删除吗？"
                    UniqueName="Delete">
                    <HeaderStyle Width="35px" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridButtonColumn>
            </Columns>
        </MasterTableView>
        <GroupPanel Visible="True">
        </GroupPanel>
    </rad:RadGrid>
    <rad:RadWindowManager ID="TemplateWindowManager" runat="server" Height="577px" Width="900px"
        ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="AddExcelTemplatePageForm" runat="server" Title="新增模板"  Height="300" Width="650"/>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="TempRadGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="TempRadGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

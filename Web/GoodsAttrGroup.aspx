<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="GoodsAttrGroup.aspx.cs" Inherits="ERP.UI.Web.GoodsAttrGroup" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    
    <rad:RadGrid ID="GroupGrid" runat="server" SkinID="Common" OnNeedDataSource="GroupGrid_NeedDataSource"
        OnInsertCommand="GroupGrid_InsertCommand" 
        OnUpdateCommand="GroupGrid_UpdateCommand" 
        OnDeleteCommand="GroupGrid_DeleteCommand" 
        OnItemCommand="GroupGrid_ItemCommand">
        <MasterTableView DataKeyNames="GroupId" EditMode="InPlace">
            <CommandItemTemplate>
                <asp:LinkButton ID="LB_AddRecord" runat="server" CommandName="InitInsert">
                    <asp:Image ID="AddRecord" runat="server" ImageAlign="AbsMiddle" SkinID="InsertImageButton" />添加分组
                </asp:LinkButton>
                &nbsp;&nbsp;&nbsp;
                <asp:LinkButton ID="LB_Refresh" runat="server" CommandName="RebindGrid">
                    <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />刷新
                </asp:LinkButton>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridTemplateColumn DataField="GroupName" HeaderText="分组名称" UniqueName="GroupName">
                    <ItemTemplate>
                        <asp:Label ID="Lab_GroupName" runat="server" Text='<%# Eval("GroupName") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TB_GroupName" runat="server" Text='<%# Eval("GroupName") %>' SkinID="ShortInput" Width="200px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RFVGroupName" runat="server" ControlToValidate="TB_GroupName"
                            ErrorMessage="属性组名不允许为空！" Text="*"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="MatchType" HeaderText="匹配类型" UniqueName="MatchType">
                    <ItemTemplate>
                        <asp:Label ID="Lab_MatchType" runat="server" Text='<%# GetMatchType(Eval("MatchType")) %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:DropDownList ID="DDL_MatchType" runat="server" SelectedValue='<%# Convert.ToInt32(string.IsNullOrEmpty(Eval("MatchType").ToString())?"0":Eval("MatchType")) %>'>
                            <asp:ListItem Text="文字匹配" Value="0"></asp:ListItem>
                            <asp:ListItem Text="数值匹配" Value="1"></asp:ListItem>
                            <asp:ListItem Text="不匹配" Value="2"></asp:ListItem>
                            <asp:ListItem Text="价格匹配" Value="3"></asp:ListItem>
                            <asp:ListItem Text="尺寸匹配" Value="4"></asp:ListItem>
                            <asp:ListItem Text="类型匹配" Value="5"></asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="排序字段" UniqueName="OrderIndex">
                    <ItemTemplate>
                        <asp:Label ID="Lab_OrderIndex" runat="server" Text='<%# Eval("OrderIndex") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="Txt_OrderIndex" runat="server" Text='<%# Eval("OrderIndex") %>'></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RFV_OrderIndex" runat="server" ErrorMessage="排序字段不能为空!" ControlToValidate="Txt_OrderIndex" Text="*"></asp:RequiredFieldValidator>
                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ErrorMessage="输入不符要求" ValidationExpression="^[1-9]\d*|0$" Text="请输入正整数!" ControlToValidate="Txt_OrderIndex">
                        </asp:RegularExpressionValidator> 
                    </EditItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="计量单位" UniqueName="Unit">
                    <ItemTemplate>
                        <asp:Label ID="Lab_Unit" runat="server" Text='<%# Eval("Unit") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="Txt_Unit" runat="server" Text='<%# Eval("Unit") %>' MaxLength="10"></asp:TextBox>
                    </EditItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="是否显示" UniqueName="ExchangeRate">
                    <ItemTemplate>
                        <asp:CheckBox ID="CK_EnableFilter" runat="server" AutoPostBack="true" Checked='<%# Eval("EnabledFilter") %>' Enabled="false" /> 
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="CK_EditFilter" runat="server" Checked='<%# IsShow(Eval("EnabledFilter").ToString()) %>' Enabled="true" /> 
                    </EditItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>

                 <rad:GridTemplateColumn HeaderText="是否多选" UniqueName="IsMChoice">
                    <ItemTemplate>
                        <asp:CheckBox ID="CK_EnableIsMChoice" runat="server" AutoPostBack="true" Checked='<%# Eval("IsMChoice") %>' Enabled="false" /> 
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="CK_EditIsMChoice" runat="server" Checked='<%# IsShow(Eval("IsMChoice").ToString()) %>' Enabled="true" /> 
                    </EditItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                
                <rad:GridTemplateColumn HeaderText="是否优先筛选">
                    <ItemTemplate>
                        <asp:CheckBox ID="CK_EnableIsPriorityFilter" runat="server" AutoPostBack="true" Checked='<%# Eval("IsPriorityFilter") %>' Enabled="false" /> 
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="CK_EditIsPriorityFilter" runat="server" Checked='<%# IsShow(Eval("IsPriorityFilter").ToString()) %>' Enabled="true" /> 
                    </EditItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                
                <rad:GridTemplateColumn HeaderText="是否上传图片">
                    <ItemTemplate>
                        <asp:CheckBox ID="CK_EnableIsUploadImage" runat="server" AutoPostBack="true" Checked='<%# Eval("IsUploadImage") %>' Enabled="false" /> 
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:CheckBox ID="CK_EditIsUploadImage" runat="server" Checked='<%# IsShow(Eval("IsUploadImage").ToString()) %>' Enabled="true" /> 
                    </EditItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                
                <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" EditText="编辑"
                    UpdateText="更新" InsertText="添加">
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
    </rad:RadGrid>

    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="GroupGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GroupGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

</asp:Content>

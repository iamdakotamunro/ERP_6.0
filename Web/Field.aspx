<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.FieldAw" Title="Untitled Page" CodeBehind="Field.aspx.cs" %>

<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadGrid ID="FieldGrid" runat="server" SkinID="Common_Foot" OnNeedDataSource="FieldGrid_NeedDataSource"
        OnInsertCommand="FieldGrid_InsertCommand" OnDetailTableDataBind="FieldGrid_DetailTableDataBind"
        OnUpdateCommand="FieldGrid_UpdateCommand" OnDeleteCommand="FieldGrid_DeleteCommand"
        DataMember="TopField">
        <MasterTableView DataKeyNames="FieldId,FieldName,FieldValue,OrderIndex" EditMode="InPlace" DataMember="TopField">
            <CommandItemTemplate>
                <Ibt:ImageButtonControl ID="LBAddRecord" runat="server" CommandName="InitInsert" Visible='<%# !FieldGrid.MasterTableView.IsItemInserted %>' SkinType="Insert" Text="添加属性"></Ibt:ImageButtonControl>
                &nbsp;&nbsp;&nbsp;
            <Ibt:ImageButtonControl ID="LBRefresh" runat="server" CommandName="RebindGrid" SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="序号">
                    <ItemTemplate>
                        <%#(int)DataBinder.Eval(Container, "DataSetIndex")+1%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="属性名称" UniqueName="FieldName">
                    <ItemTemplate>
                        <asp:Label ID="LB_FieldName" runat="server" Text='<%# Eval("FieldName") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TB_FieldName" runat="server" Text='<%# Eval("FieldName") %>' Width="600px"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RFVFieldName" runat="server" ErrorMessage="属性名称不能为空！"
                            Text="*" ControlToValidate="TB_FieldName"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="属性值" UniqueName="FieldValue">
                    <ItemTemplate>
                        <asp:Label ID="LB_FieldValue" runat="server" Text='<%# Eval("FieldValue") %>'></asp:Label>
                    </ItemTemplate>
                    <EditItemTemplate>
                        <asp:TextBox ID="TB_FieldValue" runat="server" Text='<%# Eval("FieldValue") %>' ToolTip="复合属性该值仅作为默认项，且复合值中必须带有该项。"></asp:TextBox>
                        <asp:RequiredFieldValidator ID="RFVFieldValue" runat="server" ErrorMessage="属性名称不能为空！"
                            Text="*" ControlToValidate="TB_FieldValue"></asp:RequiredFieldValidator>
                    </EditItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Left" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="排序">
                    <ItemTemplate>
                        <asp:TextBox ID="txt_OrderIndex" runat="server" Text='<%#Eval("OrderIndex")%>' OnTextChanged="txt_OrderIndex_OnTextChanged" AutoPostBack="True"></asp:TextBox>
                    </ItemTemplate>
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" HeaderText="编辑" EditText="编辑" InsertText="添加">
                    <HeaderStyle Width="40px" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridEditCommandColumn>
                <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" HeaderText="删除" Text="删除" ConfirmText="确实要删除吗？" UniqueName="Delete">
                    <HeaderStyle Width="40px" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridButtonColumn>
            </Columns>
            <DetailTables>
                <rad:GridTableView runat="server" EditMode="InPlace" DataKeyNames="FieldId,FieldName,FieldValue,OrderIndex" DataMember="CombField"
                    CommandItemDisplay="Top" Width="100%" NoDetailRecordsText="无子记录信息。" Visible="False">
                    <CommandItemTemplate>
                        <asp:LinkButton ID="LinkButtonAddRecord" runat="server" CommandName="InitInsert">
                            <asp:ImageButton ID="AddRecord" runat="server" SkinID="AddImageButton" />添加复合属性
                        </asp:LinkButton>
                    </CommandItemTemplate>
                    <Columns>
                        <rad:GridTemplateColumn HeaderText="复合属性值" UniqueName="FieldValue">
                            <ItemTemplate>
                                <asp:Label ID="LB_FieldValue" runat="server" Text='<%# Eval("FieldValue") %>'></asp:Label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:TextBox ID="TB_FieldValue" runat="server" Text='<%# Eval("FieldValue") %>' CssClass="StandardInput"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RFVFieldValue" runat="server" ErrorMessage="复合属性值不能为空！"
                                    Text="*" ControlToValidate="TB_FieldValue"></asp:RequiredFieldValidator>
                            </EditItemTemplate>
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn UniqueName="IsCombField" HeaderText="排序">
                            <ItemTemplate>
                                <asp:TextBox ID="txt_OrderIndex" runat="server" Text='<%#Eval("OrderIndex")%>' OnTextChanged="txt_OrderIndex_OnTextChanged" AutoPostBack="True"></asp:TextBox>
                            </ItemTemplate>
                            <HeaderStyle Width="150px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" HeaderText="编辑" EditText="编辑" InsertText="添加">
                            <HeaderStyle Width="40px" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridEditCommandColumn>
                        <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" HeaderText="删除" Text="删除" ConfirmText="确实要删除吗？" UniqueName="Delete">
                            <HeaderStyle Width="40px" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridButtonColumn>
                    </Columns>
                    <PagerStyle Mode="NumericPages" HorizontalAlign="Left" />
                </rad:GridTableView>
            </DetailTables>
            <ExpandCollapseColumn>
                <HeaderStyle Width="19px" />
            </ExpandCollapseColumn>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="FieldGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="FieldGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>


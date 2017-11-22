<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="GoodsExtendTitlePage.aspx.cs" Inherits="ERP.UI.Web.GoodsExtendTitlePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RGTitle" runat="server" SkinID="Common" OnNeedDataSource="RGTitle_NeedDataSource"
                    OnInsertCommand="RGTitle_InsertCommand" OnUpdateCommand="RGTitle_UpdateCommand"
                    OnDeleteCommand="RGTitle_DeleteCommand">
                    <GroupPanel Visible="True">
                    </GroupPanel>
                    <MasterTableView DataKeyNames="Id">
                        <CommandItemTemplate>
                            <asp:LinkButton ID="LB_AddRecord" runat="server" CommandName="InitInsert" Visible='<%# !RGTitle.MasterTableView.IsItemInserted %>'>
                                <asp:Image ID="AddRecord" runat="server" ImageAlign="AbsMiddle" SkinID="AddImageButton" />添加标题
                            </asp:LinkButton>
                            &nbsp;&nbsp;&nbsp;
                            <asp:LinkButton ID="LB_Refresh" runat="server" CommandName="RebindGrid">
                                <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />刷新
                            </asp:LinkButton>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="Title" HeaderText="标题名称" UniqueName="Title">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="Position" HeaderText="展示位置" UniqueName="Position">
                                 <ItemTemplate>
                                    <%#GetEnumDic(Convert.ToInt32(Eval("Position")))%>                                                                                       
                                 </ItemTemplate>
                                 <HeaderStyle HorizontalAlign="Center" />
                                 <ItemStyle HorizontalAlign="Center" />
                             </rad:GridTemplateColumn>
                            <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" EditText="编辑" InsertText="添加">
                                <HeaderStyle Width="50px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridEditCommandColumn>
                            <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="删除" ConfirmText="删除会将已经引用的该标题的模板一起删除,确实要删除吗？"
                                UniqueName="Delete">
                                <HeaderStyle Width="35px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridButtonColumn>
                        </Columns>
                        <EditFormSettings EditFormType="Template">
                            <FormTemplate>
                                <table class="PanelArea">
                                    <tr>
                                        <td class="AreaRowTitle">
                                            模板名称：
                                        </td>
                                        <td>
                                            <asp:TextBox ID="TB_TitleName" Text='<%# Eval("Title") %>' SkinID="LongInput" runat="server" Width="600px"></asp:TextBox>
                                            <asp:RequiredFieldValidator ID="RFVTitleName" ErrorMessage="添加名称" Text="*" ControlToValidate="TB_TitleName" runat="server"></asp:RequiredFieldValidator>
                                        </td>
                                        <td class="AreaRowTitle">
                                            展示位置：
                                        </td>
                                        <td>
                                          <rad:RadComboBox ID="RCB_Position" DataSource='<%#GetEnumDic() %>' SelectedValue='<%#Eval("Position") %>'
                                              DataTextField="value" DataValueField="key" Width="200px" runat="server">
                                          </rad:RadComboBox>
                                        </td>
                                        <td align="left" >
                                            <asp:ImageButton ID="btnUpdate" CommandName='<%# Container.ItemIndex==-1 ? "PerformInsert" : "Update" %>'
                                                AlternateText='<%# Container.ItemIndex==-1 ? "添加" : "编辑" %>' SkinID="InsertImageButton" runat="server" />
                                            &nbsp;
                                            <asp:ImageButton ID="btnCancel" CommandName="Cancel" AlternateText="取消" SkinID="CancelImageButton" CausesValidation="false" runat="server" />
                                        </td>
                                    </tr>
                                </table>
                            </FormTemplate>
                        </EditFormSettings>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGTitle" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RGTitle">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RGTitle" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

</asp:Content>

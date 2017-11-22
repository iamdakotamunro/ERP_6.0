<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="True"
    CodeBehind="InvoiceRoll.aspx.cs" Inherits="ERP.UI.Web.InvoiceRoll" Title="无标题页" %>
<%@ Import Namespace="Framework.Common" %>
<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl_1" Src="~/UserControl/ImageButtonControl.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadGrid ID="RadGrid_InvoiceRollList" runat="server" SkinID="Common_Foot" OnNeedDataSource="RadGrid_InvoiceRollList_NeedDataSource"
        OnInsertCommand="RadGrid_InvoiceRollList_InsertCommand" OnUpdateCommand="RadGrid_InvoiceRollList_UpdateCommand" OnItemDataBound="RadGrid_InvoiceRollList_OnItemDataBound">
        <MasterTableView DataKeyNames="Id" ClientDataKeyNames="Id">
            <CommandItemTemplate>
                <Ibt:ImageButtonControl_1 ID="LB_AddRecord" runat="server" CommandName="InitInsert"
                    Visible='<%# !RadGrid_InvoiceRollList.MasterTableView.IsItemInserted %>' SkinType="Insert"
                    Text="纸质发票入库"></Ibt:ImageButtonControl_1>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" />
            <Columns>
                <rad:GridBoundColumn DataField="Receiptor" UniqueName="Receiptor" HeaderText="购买人">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="CreateTime" UniqueName="CreateTime" HeaderText="购买时间">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="FilialeName" UniqueName="InvoiceStartNo" HeaderText="购买公司">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Lab_FilialeName" runat="server" Text='<%# GetFilialeName(Eval("FilialeId")) %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="InvoiceCode" UniqueName="InvoiceCode" HeaderText="发票代码">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="InvoiceStartNo" UniqueName="InvoiceStartNo" HeaderText="起始号码">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Convert.ToInt64(Eval("InvoiceStartNo")).ToMendString(8) %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="InvoiceEndNo" UniqueName="InvoiceEndNo" HeaderText="结束号码">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Convert.ToInt64(Eval("InvoiceEndNo")).ToMendString(8) %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="InvoiceCount" UniqueName="InvoiceCount" HeaderText="发票份数">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="InvoiceRollCount" UniqueName="InvoiceRollCount" HeaderText="发票卷数">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="遗失" UniqueName="Lost">
                    <ItemTemplate>
                        <asp:ImageButton ID="LB_Lost" runat="server" SkinID="EditImageButton" OnClientClick='<%# "return LostClick(\"" + Eval("Id")+ "\")" %>'>
                        </asp:ImageButton>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridEditCommandColumn HeaderText="编辑" ButtonType="ImageButton" CancelText="取消"
                    EditText="编辑">
                    <HeaderStyle Width="50px" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                </rad:GridEditCommandColumn>
            </Columns>
            <EditFormSettings EditFormType="Template">
                <FormTemplate>
                    <table>
                        <tr>
                            <td>
                                购买人：
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_Receiptor" Text='<%# GetReceiptorName(Eval("Receiptor").ToString()) %>'
                                    runat="server"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                购买时间：
                            </td>
                            <td>
                                <rad:RadDatePicker runat="server" ID="RadDatePicker_Time" SelectedDate='<%# (Container).ItemIndex==-1?DateTime.Now:(DateTime)Eval("CreateTime") %>'>
                                </rad:RadDatePicker>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                购买公司：
                            </td>
                            <td>
                                 <asp:HiddenField ID="hfFilialeId" Value='<%# Eval("FilialeId") %>' runat="server"/>
                                <rad:RadComboBox runat="server" ID="RCB_Filiale" DataSource='<%# LoadFiliale() %>'
                                    DataTextField="Name" DataValueField="ID">
                                </rad:RadComboBox>
                                <asp:RequiredFieldValidator ID="RFVFiliale" runat="server" ErrorMessage="请选择公司" Text="*"
                                    ControlToValidate="RCB_Filiale" ValidationGroup="Save"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                发票代码：
                            </td>
                            <td>
                                <asp:TextBox runat="server" ID="TextBox_InvoiceCode" Text='<%#Eval("InvoiceCode") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                发票号码范围：
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_InvoiceStartNo" ToolTip="起始号码" runat="server" Text='<%#Eval("InvoiceStartNo") %>'
                                    MaxLength="8"></asp:TextBox>
                                -
                                <asp:TextBox ID="TextBox_InvoiceEndNo" ToolTip="结束号码" runat="server" Text='<%#Eval("InvoiceEndNo") %>'
                                    MaxLength="8"></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                发票卷数：
                            </td>
                            <td>
                                <asp:TextBox ID="TextBox_InvoiceRollCount" runat="server" Text='<%#Eval("InvoiceRollCount") %>'></asp:TextBox>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <asp:ImageButton ID="btnUpdate" runat="server" SkinID="UpdateImageButton" CommandName='<%# (Container).ItemIndex==-1 ? "PerformInsert" : "Update" %>'
                                    AlternateText='<%# (Container).ItemIndex==-1 ? "添加" : "编辑" %>' />
                                &nbsp;
                                <asp:ImageButton ID="btnCancel" runat="server" SkinID="CancelImageButton" CommandName="Cancel"
                                    AlternateText="取消" CausesValidation="false" />
                            </td>
                        </tr>
                    </table>
                </FormTemplate>
            </EditFormSettings>
        </MasterTableView>
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDbClick" />
        </ClientSettings>
    </rad:RadGrid>
    <rad:RadWindowManager ReloadOnShow="true" EnableShadow="true" ID="RadWindowManager_Roll"
        runat="server">
        <Windows>
            <rad:RadWindow Title="发票分卷信息" ID="RadWindow_RollDetailList" runat="server" Width="560"
                Height="320">
            </rad:RadWindow>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RadWindow_RollDetailList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGrid_InvoiceRollList" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <script type="text/javascript">
        function RowDbClick(obj, args) {
            var rollId = args.getDataKeyValue("Id");
            openInvoiceRollDetail(rollId);

        }
        function openInvoiceRollDetail(rollId) {
            var oWnd = window.radopen("/Windows/InvoiceRollDetail.aspx?rollid=" + rollId, "RadWindow_RollDetailList");
        }
        function LostClick(rollId) {
            window.radopen("/Windows/InvoiceLost.aspx?RollId=" + rollId, "RadWindow_RollDetailList");
        }        
    </script>
</asp:Content>

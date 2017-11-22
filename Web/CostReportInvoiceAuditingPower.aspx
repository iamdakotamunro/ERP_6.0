<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" 
CodeBehind="CostReportInvoiceAuditingPower.aspx.cs" Inherits="ERP.UI.Web.CostReportInvoiceAuditingPower" %>

<%@Register Src="/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
<table class="StagePanel">
    <tr>
        <td style="vertical-align: top;">
            <rad:RadGrid ID="RG_Power" runat="server" SkinID="Common_Foot" OnNeedDataSource="RG_Power_NeedDataSource"
                OnUpdateCommand="RG_Power_UpdateCommand" OnDeleteCommand="RG_Power_DeleteCommand"
                OnInsertCommand="RG_Power_InsertCommand">
                <MasterTableView DataKeyNames="PowerId" CommandItemDisplay="Top" NoMasterRecordsText="无可用记录。">
                    <CommandItemTemplate>
                        <Ibt:ImageButtonControl ID="LB_AddReport" runat="server" CommandName="InitInsert" Visible='<%# !RG_Power.MasterTableView.IsItemInserted %>' SkinType="Insert" Text="添加票据受理权限">
                        </Ibt:ImageButtonControl>
                        &nbsp;&nbsp;&nbsp;
                        <Ibt:ImageButtonControl ID="LB_Refresh" runat="server" CommandName="RebindGrid" SkinType="Refresh" Text="刷新">
                        </Ibt:ImageButtonControl>
                    </CommandItemTemplate>
                    <Columns>
                        <rad:GridTemplateColumn HeaderText="受理人公司" UniqueName="ReportCost">
                            <ItemTemplate>
                                <asp:Label ID="LB_Filiale" runat="server" Text='<%# GetFilialeName(Eval("AuditingFilialeId")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle  HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="受理人部门" UniqueName="ReportCost">
                            <ItemTemplate>
                                <asp:Label ID="LB_Branch" runat="server" Text='<%#GetBranchName(Eval("AuditingFilialeId"),Eval("AuditingBranchId")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="受理人职务" UniqueName="ReportCost">
                            <ItemTemplate>
                                <asp:Label ID="LB_Position" runat="server" Text='<%# GetPositionName(Eval("AuditingFilialeId"),Eval("AuditingBranchId"),Eval("AuditingPositionId")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="申报部门" UniqueName="ReportCost">
                            <ItemTemplate>
                                <asp:Label ID="LB_ReportBranch" runat="server" Text='<%# GetBindBranch(new Guid(Eval("AuditingFilialeId").ToString()),Eval("ReportBranchId").ToString()) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridBoundColumn DataField="Description" UniqueName="Description" HeaderText="描述">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridBoundColumn>
                        <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" EditText="编辑" HeaderText="编辑"
                            InsertText="添加">
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridEditCommandColumn>
                        <rad:GridTemplateColumn HeaderText="删除" UniqueName="Delete" >
                            <ItemTemplate>
                                <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Execute" CommandName="Delete" SkinID="DeleteImageButton" OnClientClick="return confirm('确定删除？')" />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                    <EditFormSettings EditFormType="Template">
                        <FormTemplate>
                            <table class="PanelArea">
                                <tr>
                                    <td >
                                        受理人公司：
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="DDL_Filiale" runat="server" Width="100px" AutoPostBack="true"
                                            DataSource='<%# LoadFiliale() %>' OnSelectedIndexChanged="DDL_Filiale_SelectedIndexChanged"
                                            DataTextField="Name" DataValueField="ID" >
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RFVFiliale" runat="server" ErrorMessage="请选择公司" Text="*"
                                            ControlToValidate="DDL_Filiale" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                    <td >
                                        受理人部门：
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="DDL_Branch" runat="server" Width="80px" AutoPostBack="true"
                                            OnSelectedIndexChanged="DDL_Branch_SelectedIndexChanged" >
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RFVBranch" runat="server" ErrorMessage="请选择部门" Text="*"
                                            ControlToValidate="DDL_Branch" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                    <td >
                                        受理人职务：
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="DDL_Position" runat="server" Width="80px" AutoPostBack="true">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RFVPosition" runat="server" ErrorMessage="请选择职务"
                                            Text="*" ControlToValidate="DDL_Position" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                    <td >
                                        申报公司：
                                    </td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_ReportFiliale" Width="80px" runat="server" AutoPostBack="true" DataSource='<%# LoadFiliale() %>'
                                            DataTextField="Name" DataValueField="ID" OnSelectedIndexChanged="RCB_ReportFiliale_SelectedIndexChanged">
                                        </rad:RadComboBox>
                                    </td>
                                    <td >
                                        申报部门：
                                    </td>
                                    <td>
                                        <rad:RadComboBox ID="RCB_ReportBranch" Width="80px" runat="server"
                                            DataTextField="Name" DataValueField="ID">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="ccb" runat="server"  OnDataBinding="ccb_OnLoad" />
                                                <%# Eval("Name") %>
                                                <asp:Label ID="lblID" runat="server" Visible="false" Text='<%#Eval("ID")%>'></asp:Label>
                                            </ItemTemplate>
                                        </rad:RadComboBox>
                                    </td>
                                    <td  align="center">
                                         描述：<asp:TextBox ID="TB_Description" runat="server" Width="400px" Text='<%# (Container as GridItem).ItemIndex==-1 ?"":Eval("Description").ToString() %>'></asp:TextBox>
                                    </td>
                                    <td align="center" colspan="6">
                                        <asp:ImageButton ID="btnUpdate" runat="server" SkinID="UpdateImageButton" CommandName='<%# (Container as GridItem).ItemIndex==-1 ? "PerformInsert" : "Update" %>'
                                            AlternateText='<%# (Container as GridItem).ItemIndex==-1 ? "添加" : "编辑" %>' ValidationGroup="Save" />&nbsp;
                                        <asp:ImageButton ID="btnCancel" runat="server" SkinID="CancelImageButton" CommandName="Cancel"
                                            AlternateText="取消" CausesValidation="false" />
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
<rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="RG_Power">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="RG_Power"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
</rad:RadAjaxLoadingPanel>
</asp:Content>

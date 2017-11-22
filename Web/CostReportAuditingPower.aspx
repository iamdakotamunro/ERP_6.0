<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="CostReportAuditingPower.aspx.cs" Inherits="ERP.UI.Web.CostReportAuditingPower" %>

<%@ Register Src="/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RG_Power" runat="server" SkinID="Common_Foot" OnNeedDataSource="RgPowerNeedDataSource"
                    OnUpdateCommand="RgPowerUpdateCommand" OnDeleteCommand="RgPowerDeleteCommand"
                    OnInsertCommand="RgPowerInsertCommand" OnItemDataBound="RgPower_ItemDataBound">
                    <MasterTableView DataKeyNames="PowerId" CommandItemDisplay="Top" NoMasterRecordsText="无可用记录。">
                        <CommandItemTemplate>
                            <Ibt:ImageButtonControl ID="LB_AddReport" runat="server" CommandName="InitInsert"
                                Visible='<%# !RG_Power.MasterTableView.IsItemInserted %>' SkinType="Insert" Text="添加审批权限">
                            </Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LB_Refresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新"></Ibt:ImageButtonControl>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <ExpandCollapseColumn Visible="False">
                            <HeaderStyle Width="19px" />
                        </ExpandCollapseColumn>
                        <RowIndicatorColumn Visible="False">
                            <HeaderStyle Width="20px" />
                        </RowIndicatorColumn>
                        <CommandItemSettings ExportToPdfText="Export to Pdf"></CommandItemSettings>
                        <Columns>
                            <rad:GridTemplateColumn HeaderText="审批公司" UniqueName="ReportCost">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Filiale" runat="server" Text='<%# GetFilialeName(Eval("AuditingFilialeId")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审批部门" UniqueName="ReportCost">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Branch" runat="server" Text='<%#  GetBranchName(Eval("AuditingFilialeId"),Eval("AuditingBranchId"))%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审批职务" UniqueName="ReportCost">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Position" runat="server" Text='<%# GetPositionName(Eval("AuditingFilialeId"),Eval("AuditingBranchId"),Eval("AuditingPositionId")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="100px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审批金额范围" UniqueName="ReportCost">
                                <ItemTemplate>
                                    <asp:Label ID="LB_ReportCost" runat="server" Text='<%# Convert.ToDecimal(Eval("MinAmount")).ToString("#0.00")+"~"+Convert.ToDecimal(Eval("MaxAmount")).ToString("#0.00") %>'></asp:Label>
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
                            <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" EditText="编辑"
                                HeaderText="编辑" InsertText="添加">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridEditCommandColumn>
                            <rad:GridTemplateColumn HeaderText="删除" UniqueName="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Execute" CommandName="Delete"
                                        SkinID="DeleteImageButton" OnClientClick="return confirm('确定删除？')" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                        <EditFormSettings EditFormType="Template">
                            <EditColumn UniqueName="EditCommandColumn1">
                            </EditColumn>
                            <FormTemplate>
                                <table class="PanelArea">
                                    <tr>
                                        <td>
                                            审批人公司：
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hfAuditingFilialeId" Value='<%# Eval("AuditingFilialeId") %>'
                                                runat="server" />
                                            <asp:DropDownList ID="DDL_Filiale" runat="server" Width="130px" AutoPostBack="true"
                                                DataSource='<%# LoadFiliale() %>' OnSelectedIndexChanged="DdlFilialeSelectedIndexChanged"
                                                DataTextField="Name" DataValueField="ID">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RFVFiliale" runat="server" ErrorMessage="请选择公司" Text="*"
                                                ControlToValidate="DDL_Filiale" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            审批人部门：
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hfAuditingBranchId" Value='<%# Eval("AuditingBranchId") %>'
                                                runat="server" />
                                            <asp:DropDownList ID="DDL_Branch" runat="server" Width="130px" AutoPostBack="true"
                                                OnSelectedIndexChanged="DdlBranchSelectedIndexChanged">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RFVBranch" runat="server" ErrorMessage="请选择部门" Text="*"
                                                ControlToValidate="DDL_Branch" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            审批人职务：
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hfAuditingPositionId" Value='<%# Eval("AuditingPositionId") %>'
                                                runat="server" />
                                            <asp:DropDownList ID="DDL_Position" runat="server" Width="130px" AutoPostBack="true">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RFVPosition" runat="server" ErrorMessage="请选择职务"
                                                Text="*" ControlToValidate="DDL_Position" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            金额范围：
                                        </td>
                                        <td colspan="5">
                                            <asp:TextBox ID="TB_MinAmount" runat="server" Width="80px" Text='<%# (Container as GridItem).ItemIndex==-1 ?"":Convert.ToDecimal(Eval("MinAmount")).ToString("#0.00") %>'></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TB_MinAmount"
                                                Text="*" ErrorMessage="金额必须为数字！" ValidationExpression="\d*(\.\d*){0,1}" ValidationGroup="Save"></asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="请输入金额"
                                                Text="*" ControlToValidate="TB_MinAmount" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                            ~<asp:TextBox ID="TB_MaxAmount" runat="server" Width="80px" Text='<%# (Container as GridItem).ItemIndex==-1 ?"":Convert.ToDecimal(Eval("MaxAmount")).ToString("#0.00") %>'></asp:TextBox>
                                            <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TB_MaxAmount"
                                                Text="*" ErrorMessage="金额必须为数字！" ValidationExpression="\d*(\.\d*){0,1}" ValidationGroup="Save"></asp:RegularExpressionValidator>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="请输入金额"
                                                Text="*" ControlToValidate="TB_MaxAmount" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            描述：
                                        </td>
                                        <td colspan="5">
                                            <asp:TextBox ID="TB_Description" runat="server" Width="700px" Text='<%# (Container as GridItem).ItemIndex==-1 ?"":Eval("Description").ToString() %>'></asp:TextBox>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="6">
                                            申报部门：
                                        </td>
                                    </tr>
                                    <tr>
                                        <td colspan="6">
                                            <asp:HiddenField ID="hfReportBranchIds" Value="" runat="server" />
                                            <asp:Repeater ID="RP_ReportFiliale" runat="server" OnItemDataBound="Rp_Filiale_ItemDataBound">
                                                <ItemTemplate>
                                                    <table width="100%">
                                                        <tr>
                                                            <td width="100px">
                                                                <asp:Label ID="lbReportFiliale" Text='<%#Eval("Name")%>' runat="server"></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:CheckBoxList ID="cblReportBranch" RepeatColumns="6" runat="server" />
                                                            </td>
                                                        </tr>
                                                    </table>
                                                    <hr />
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </td>
                                    </tr>
                                    <tr>
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
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" UseEmbeddedScripts="false">
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

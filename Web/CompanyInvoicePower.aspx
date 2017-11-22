<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="CompanyInvoicePower.aspx.cs" Inherits="ERP.UI.Web.CompanyInvoicePower" %>

<%@ Register Src="/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    <script type="text/javascript" language="javascript">
        function Change() {
            var ddlInvoiceType = $("select[id*=DDL_InvoiceType]").val();
            if (ddlInvoiceType == "-1") {
                alert("请选择发票类型");
                return false;
            }
            var ddlPosition = $("select[id*=DDL_Position]").val();
            if (ddlPosition == "-1") {
                alert("请选择职务");
                return false;
            }
        }
    </script>

    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <rad:RadTreeView ID="RT_CompanyClass" runat="server" SkinID="Common" AutoPostBack="true"
                    CausesValidation="True" OnNodeClick="RT_CompanyClass_NodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="RG_Power" runat="server" SkinID="Common_Foot" OnNeedDataSource="RG_Power_NeedDataSource"
                    OnUpdateCommand="RG_Power_UpdateCommand" OnDeleteCommand="RG_Power_DeleteCommand"
                    OnInsertCommand="RG_Power_InsertCommand" OnItemDataBound="RgPower_ItemDataBound">
                    <MasterTableView DataKeyNames="PowerID" CommandItemDisplay="Top" NoMasterRecordsText="无可用记录。">
                        <CommandItemTemplate>
                            <Ibt:ImageButtonControl ID="LB_AddReport" runat="server" CommandName="InitInsert"
                                Visible='<%# !RG_Power.MasterTableView.IsItemInserted %>' SkinType="Insert" Text="添加审批权限">
                            </Ibt:ImageButtonControl>
                            &nbsp;&nbsp;&nbsp;
                            <Ibt:ImageButtonControl ID="LB_Refresh" runat="server" CommandName="RebindGrid" SkinType="Refresh"
                                Text="刷新">
                            </Ibt:ImageButtonControl>
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
                            <rad:GridTemplateColumn HeaderText="发票类型" UniqueName="InvoiceType">
                                <ItemTemplate>
                                    <asp:Label ID="LB_InvoiceType" runat="server" Text='<%# GetEnumIntro(Eval("InvoicesType")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审批公司" UniqueName="Filiale">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Filiale" runat="server" Text='<%# GetFilialeName(Eval("FilialeId")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审批部门" UniqueName="Branch">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Branch" runat="server" Text='<%#  GetBranchName(Eval("FilialeId"),Eval("BranchId")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审批职务" UniqueName="Position">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Position" runat="server" Text='<%# GetPositionName(Eval("FilialeId"),Eval("BranchId"),Eval("PositionId")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审批人" UniqueName="Personnel">
                                <ItemTemplate>
                                    <asp:Label ID="LB_Personnel" runat="server" Text='<%# GetPersonnelName(Eval("AuditorID")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="编辑" UniqueName="EditCommandColumn">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Update" CommandName="Edit"
                                        SkinID="EditImageButton" Visible='<%# Eval("BindingType").ToString()=="0" %>' />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="删除" UniqueName="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Execute" CommandName="Delete"
                                        SkinID="DeleteImageButton" OnClientClick="return confirm('确定删除？')" Visible='<%# Eval("BindingType").ToString()=="0" %>' />
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
                                            发票类型：
                                        </td>
                                        <td colspan="3">
                                            <asp:HiddenField ID="hfInvoicesType" Value='<%# Eval("InvoicesType") %>' runat="server"/>
                                            <asp:DropDownList ID="DDL_InvoiceType" runat="server" Width="130px" DataSource='<%#GetInvoiceType() %>'
                                                DataTextField="Value" DataValueField="Key" SelectedValue='<%# (Container as GridItem).ItemIndex==-1 ?"-1":Eval("InvoicesType").ToString() %>'>
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="请选择发票类型"
                                                Text="*" ControlToValidate="DDL_InvoiceType" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            审批人公司：
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hfFilialeId" Value='<%# Eval("FilialeId") %>' runat="server"/>
                                            <asp:DropDownList ID="DDL_Filiale" runat="server" Width="130px" AutoPostBack="true"
                                                DataSource='<%# LoadFiliale() %>' OnSelectedIndexChanged="DDL_Filiale_SelectedIndexChanged"
                                                DataTextField="Name" DataValueField="ID">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RFVFiliale" runat="server" ErrorMessage="请选择公司" Text="*"
                                                ControlToValidate="DDL_Filiale" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            审批人部门：
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hfBranchId" Value='<%# Eval("BranchId") %>' runat="server"/>
                                            <asp:DropDownList ID="DDL_Branch" runat="server" Width="130px" AutoPostBack="true"
                                                OnSelectedIndexChanged="DDL_Branch_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RFVBranch" runat="server" ErrorMessage="请选择部门" Text="*"
                                                ControlToValidate="DDL_Branch" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td>
                                            审批人职务：
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hfPositionId" Value='<%# Eval("PositionId") %>' runat="server"/>
                                            <asp:DropDownList ID="DDL_Position" runat="server" Width="130px" AutoPostBack="true"
                                                OnSelectedIndexChanged="DDL_Position_SelectedIndexChanged">
                                            </asp:DropDownList>
                                            <asp:RequiredFieldValidator ID="RFVPosition" runat="server" ErrorMessage="请选择职务"
                                                Text="*" ControlToValidate="DDL_Position" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        </td>
                                        <td>
                                            审批人：
                                        </td>
                                        <td>
                                            <asp:HiddenField ID="hfPersonnel" Value='<%# Eval("AuditorID") %>' runat="server"/>
                                            <asp:DropDownList ID="DDL_Personnel" runat="server" Width="130px">
                                            </asp:DropDownList>
                                        </td>
                                    </tr>
                                    <tr>
                                        <td align="center" colspan="4">
                                            <asp:ImageButton ID="btnUpdate" runat="server" SkinID="UpdateImageButton" CommandName='<%# (Container as GridItem).ItemIndex==-1 ? "PerformInsert" : "Update" %>'
                                                AlternateText='<%# (Container as GridItem).ItemIndex==-1 ? "添加" : "编辑" %>' ValidationGroup="Save" OnClientClick="return Change();" />&nbsp;
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
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RT_CompanyClass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_Power" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RG_Power">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_Power" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="CompanyAuditingPower.aspx.cs" Inherits="ERP.UI.Web.CompanyAuditingPower" %>

<%@Register Src="/UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
<script type="text/javascript" src="../JavaScript/telerik.js"></script>
    <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    <script type="text/javascript" language="javascript">
        function Change() {
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
            <rad:RadTreeView ID="RT_CompanyClass" runat="server"  SkinID="Common" AutoPostBack="true" CausesValidation="True"
                OnNodeClick="RT_CompanyClass_NodeClick">
            </rad:RadTreeView>
        </td>
        <td style="vertical-align: top;">
            <rad:RadGrid ID="RG_Power" runat="server" SkinID="Common_Foot" OnNeedDataSource="RG_Power_NeedDataSource"
                OnUpdateCommand="RG_Power_UpdateCommand" OnDeleteCommand="RG_Power_DeleteCommand"
                OnInsertCommand="RG_Power_InsertCommand">
                <MasterTableView DataKeyNames="PowerID" CommandItemDisplay="Top" NoMasterRecordsText="无可用记录。">
                    <CommandItemTemplate>
                        <Ibt:ImageButtonControl ID="LB_AddReport" runat="server" CommandName="InitInsert" Visible='<%# !RG_Power.MasterTableView.IsItemInserted %>' SkinType="Insert" Text="添加审批权限">
                        </Ibt:ImageButtonControl>
                        &nbsp;&nbsp;&nbsp;
                        <Ibt:ImageButtonControl ID="LB_Refresh" runat="server" CommandName="RebindGrid" SkinType="Refresh" Text="刷新">
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
                        <rad:GridTemplateColumn HeaderText="审批公司" UniqueName="ReportCost">
                            <ItemTemplate>
                                <asp:Label ID="LB_Filiale" runat="server" Text='<%# GetFilialeName(Eval("FilialeId")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle  HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="审批部门" UniqueName="ReportCost">
                            <ItemTemplate>
                                <asp:Label ID="LB_Branch" runat="server" Text='<%#  GetBranchName(Eval("FilialeId"),Eval("BranchID")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="审批职务" UniqueName="ReportCost">
                            <ItemTemplate>
                                <asp:Label ID="LB_Position" runat="server" Text='<%# GetPositionName(Eval("FilialeId"),Eval("BranchID"),Eval("PositionID")) %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="审批金额范围" UniqueName="ReportCost">
                            <ItemTemplate>
                                <asp:Label ID="LB_ReportCost" runat="server" Text='<%# Convert.ToDecimal(Eval("LowerMoney")).ToString("#0.00")+"~"+Convert.ToDecimal(Eval("UpperMoney")).ToString("#0.00") %>'></asp:Label>
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="编辑" UniqueName="EditCommandColumn" >
                            <ItemTemplate>
                                <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Update" CommandName="Edit" SkinID="EditImageButton"
                                 Visible='<%# Eval("BindingType").ToString()=="0" %>' />
                            </ItemTemplate>
                            <HeaderStyle HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="删除" UniqueName="Delete" >
                            <ItemTemplate>
                                <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Execute" CommandName="Delete" SkinID="DeleteImageButton" OnClientClick="return confirm('确定删除？')"
                                 Visible='<%# Eval("BindingType").ToString()=="0" %>' />
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
                                    <td >
                                        审批人公司：
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="DDL_Filiale" runat="server" Width="130px" AutoPostBack="true"
                                            DataSource='<%# LoadFiliale() %>' OnSelectedIndexChanged="DDL_Filiale_SelectedIndexChanged"
                                            DataTextField="Name" DataValueField="ID" >
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RFVFiliale" runat="server" ErrorMessage="请选择公司" Text="*"
                                            ControlToValidate="DDL_Filiale" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                    <td >
                                        审批人部门：
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="DDL_Branch" runat="server" Width="130px" AutoPostBack="true"
                                            OnSelectedIndexChanged="DDL_Branch_SelectedIndexChanged" >
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RFVBranch" runat="server" ErrorMessage="请选择部门" Text="*"
                                            ControlToValidate="DDL_Branch" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                </tr>
                                <tr>
                                    <td >
                                        审批人职务：
                                    </td>
                                    <td>
                                        <asp:DropDownList ID="DDL_Position" runat="server" Width="130px" AutoPostBack="true">
                                        </asp:DropDownList>
                                        <asp:RequiredFieldValidator ID="RFVPosition" runat="server" ErrorMessage="请选择职务"
                                            Text="*" ControlToValidate="DDL_Position" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                    <td >
                                        金额范围：
                                    </td>
                                    <td>
                                        <asp:TextBox ID="TB_MinAmount" runat="server" width="80px" Text='<%# (Container as GridItem).ItemIndex==-1 ?"":Convert.ToDecimal(Eval("LowerMoney")).ToString("#0.00") %>'></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator1" runat="server" ControlToValidate="TB_MinAmount"
                                            Text="*" ErrorMessage="金额必须为数字！" ValidationExpression="\d*(\.\d*){0,1}" ValidationGroup="Save"></asp:RegularExpressionValidator>
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="请输入金额"
                                            Text="*" ControlToValidate="TB_MinAmount" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                        ~<asp:TextBox ID="TB_MaxAmount" runat="server" width="80px" Text='<%# (Container as GridItem).ItemIndex==-1 ?"":Convert.ToDecimal(Eval("UpperMoney")).ToString("#0.00") %>'></asp:TextBox>
                                        <asp:RegularExpressionValidator ID="RegularExpressionValidator2" runat="server" ControlToValidate="TB_MaxAmount"
                                            Text="*" ErrorMessage="金额必须为数字！" ValidationExpression="\d*(\.\d*){0,1}" ValidationGroup="Save"></asp:RegularExpressionValidator>    
                                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="请输入金额"
                                            Text="*" ControlToValidate="TB_MaxAmount" ValidationGroup="Save"></asp:RequiredFieldValidator>
                                    </td>
                                    
                                </tr>
                                <tr>
                                    <td align="center" colspan="4">
                                        <asp:ImageButton ID="btnUpdate" runat="server" SkinID="UpdateImageButton" CommandName='<%# (Container as GridItem).ItemIndex==-1 ? "PerformInsert" : "Update" %>'
                                            AlternateText='<%# (Container as GridItem).ItemIndex==-1 ? "添加" : "编辑" %>' ValidationGroup="Save" OnClientClick="return Change();"/>&nbsp;
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
<rad:RadAjaxManager ID="RAM" runat="server" 
    UseEmbeddedScripts="false">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="RT_CompanyClass">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="RG_Power" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server"  Skin="WebBlue">
</rad:RadAjaxLoadingPanel>
</asp:Content>

<%@ Page Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" Inherits="ERP.UI.Web.GoodsClassAw" Title="Untitled Page" Codebehind="GoodsClass.aspx.cs" %>
<%@Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
<div class="StagePanel">
    <table class="StagePanelHead" border="0" style="width: 100%;">
        <tr>
            <td>
                <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                    <tr>
                        <td>
                        </td>
                    </tr>
                </table>
            </td>
        </tr>
    </table>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="ControlTools">
                <asp:Panel ID="ControlPanel" runat="server">
                    <Ibt:ImageButtonControl ID="LB_Inster" runat="server" CausesValidation="false" OnClick="InsterItem" SkinType="Insert" Text="添加">
                    </Ibt:ImageButtonControl>
                    &nbsp;&nbsp;
                    <Ibt:ImageButtonControl ID="LB_Edit" runat="server" CausesValidation="false" OnClick="EditItem" SkinType="Edit" Text="编辑">
                    </Ibt:ImageButtonControl>
                    &nbsp;&nbsp;
                    <asp:LinkButton ID="LB_Delete" CausesValidation="false" OnClick="Delete_Click" runat="server">
                        <asp:Image ID="IB_Delete" SkinID="DeleteImageButton" ImageAlign="AbsMiddle" BorderStyle="None" runat="server" />删除
                    </asp:LinkButton>
                </asp:Panel>
            </td>
        </tr>
    </table>
    <table style="width: 100%;">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <rad:RadTreeView ID="TVGoodsClass" runat="server" UseEmbeddedScripts="false" Height="500px"
                    Width="200px" AutoPostBack="true" CausesValidation="false" OnNodeClick="TvGoodsClassNodeClick">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <asp:Panel ID="GoodsClassPanel" runat="server">
                    <table class="PanelArea">
                        <tr>
                            <td class="AreaRowTitle">
                                分类名称：
                            </td>
                            <td>
                                <asp:TextBox ID="TB_ClassName" runat="server" SkinID="LongInput"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RFVClassName" runat="server" ErrorMessage="分类名称不能为空！"
                                    Text="*" ControlToValidate="TB_ClassName"></asp:RequiredFieldValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="AreaRowTitle">
                                显示顺序：
                            </td>
                            <td>
                                <asp:TextBox ID="TB_OrderIndex" runat="server" SkinID="LongInput"></asp:TextBox>
                                <asp:RequiredFieldValidator ID="RFVOrderIndex" runat="server" ErrorMessage="顺序编号不能为空！"
                                    Text="*" ControlToValidate="TB_OrderIndex"></asp:RequiredFieldValidator>
                                <asp:RegularExpressionValidator ID="REV_PostalCode" runat="server" ValidationExpression="^\d{1,8}$"
                                    ErrorMessage="顺序号不正确！" Text="*" ControlToValidate="TB_OrderIndex"></asp:RegularExpressionValidator>
                            </td>
                        </tr>
                        <tr>
                            <td class="AreaRowTitle">
                                所属分类：
                            </td>
                            <td>
                                <asp:DropDownList ID="DDL_ParentClass" runat="server" SkinID="LongdropDown" DataTextField="ClassName"
                                    DataValueField="ClassId">
                                </asp:DropDownList>
                            </td>
                        </tr>
                        <tr>
                            <td class="AreaRowTitle">
                                相关属性选择：
                            </td>
                            <td>
                                <rad:RadGrid ID="FieldGrid" runat="server" AllowMultiRowSelection="True" Height="220"
                                    OnNeedDataSource="FieldGrid_NeedDataSource" SkinID="Common" AllowPaging="False" Width="600" MasterTableView-CommandItemDisplay="None">
                                    <ClientSettings>
                                        <Selecting AllowRowSelect="True" EnableDragToSelectRows="false" />
                                        <Scrolling AllowScroll="True" />
                                    </ClientSettings>
                                    <MasterTableView DataKeyNames="FieldId">
                                        <Columns>
                                            <rad:GridClientSelectColumn UniqueName="column">
                                                <HeaderStyle HorizontalAlign="Center" Width="40" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </rad:GridClientSelectColumn>
                                            <rad:GridBoundColumn DataField="FieldName" HeaderText="属性名称" UniqueName="FieldName">
                                                <HeaderStyle HorizontalAlign="Center" Width="78" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </rad:GridBoundColumn>
                                            <%--<rad:GridTemplateColumn DataField="IsCompField" HeaderText="属性类型" UniqueName="IsCompField">
                                                <ItemTemplate>
                                                    <asp:Label ID="IsCompFieldLabel" runat="server" Text='<%# (int) Eval("IsCompField")==1 ? "常规属性" : "复合属性" %>'></asp:Label>
                                                </ItemTemplate>
                                                <HeaderStyle HorizontalAlign="Center" Width="140" />
                                                <ItemStyle HorizontalAlign="Center" />
                                            </rad:GridTemplateColumn>--%>
                                        </Columns>
                                    </MasterTableView>
                                </rad:RadGrid>
                            </td>
                        </tr>
                        <tr>
                            <td>
                            </td>
                            <td style="text-align: right;">
                                <asp:ImageButton ID="IB_Add" runat="server" AlternateText="添加" ImageAlign="AbsMiddle"
                                    OnClick="Add_Click" SkinID="AddImageButton" />
                                <asp:Label ID="LB_AddSpace" runat="server"></asp:Label>
                                <asp:ImageButton ID="IB_Update" runat="server" AlternateText="更新" ImageAlign="AbsMiddle"
                                    OnClick="Update_Click" SkinID="UpdateImageButton" />
                                <asp:Label ID="LB_UpdateSpace" runat="server"></asp:Label>
                                <asp:ImageButton ID="IB_Cancel" runat="server" AlternateText="取消" CausesValidation="false"
                                    ImageAlign="AbsMiddle" OnClick="Cancel_Click" SkinID="CancelImageButton" />
                            </td>
                        </tr>
                    </table>
                </asp:Panel>
            </td>
        </tr>
    </table>
</div>
<rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" UseEmbeddedScripts="false">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="TVGoodsClass">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="GoodsClassPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
        <rad:AjaxSetting AjaxControlID="LB_Inster">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="ControlPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="GoodsClassPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="LB_Edit">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="ControlPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="GoodsClassPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="LB_Delete">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="TVGoodsClass"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="GoodsClassPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="IB_Add">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="TVGoodsClass"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="GoodsClassPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="IB_Update">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="TVGoodsClass"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="GoodsClassPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="IB_Cancel">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="ControlPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="GoodsClassPanel"></rad:AjaxUpdatedControl>
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
</rad:RadAjaxLoadingPanel>
</asp:Content>

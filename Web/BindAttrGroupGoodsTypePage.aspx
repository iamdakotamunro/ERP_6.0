<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="BindAttrGroupGoodsTypePage.aspx.cs" Inherits="ERP.UI.Web.BindAttrGroupGoodsTypePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="ControlTools" style="padding-right:10px;">
                <asp:Panel ID="ControlPanel" runat="server">
                    <asp:LinkButton ID="lbtnUpdate" OnClick="LbtnUpdate_OnClick" runat="server" CausesValidation="false">
                        <asp:Image ID="imgEdit" SkinID="EditImageButton" runat="server" ImageAlign="AbsMiddle" />更新
                    </asp:LinkButton>
                </asp:Panel>
            </td>
        </tr>
    </table>

    <table class="PanelArea">
        <tr>
            <td style="vertical-align: top; width: 200px;">
                <rad:RadTreeView ID="RTV_GoodsType" OnNodeClick="Rtv_GoodsType_NodeClick" runat="server" SkinID="Common">
                </rad:RadTreeView>
            </td>
            <td style="vertical-align: top;">
                <rad:RadGrid ID="GroupGrid" runat="server" SkinID="Common" OnNeedDataSource="GroupGrid_NeedDataSource" AllowMultiRowSelection="True" AllowPaging="False">
                    <ClientSettings>
                        <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" />
                    </ClientSettings>
                    <MasterTableView DataKeyNames="GroupId" CommandItemDisplay="None">
                        <Columns>
                            <rad:GridTemplateColumn DataField="MatchType" HeaderText="匹配类型" UniqueName="MatchType">
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbIsChecked" Text="" runat="server"></asp:CheckBox>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="GroupName" HeaderText="分组名称" UniqueName="GroupName">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="MatchType" HeaderText="匹配类型" UniqueName="MatchType">
                                <ItemTemplate>
                                    <asp:Label ID="Lab_MatchType" runat="server" Text='<%# GetMatchType(Eval("MatchType")) %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="GoodsQuantity" HeaderText="数量" UniqueName="GoodsQuantity">
                                <ItemTemplate>
                                    <asp:Label ID="Lab_GoodsQuantity" runat="server" Text='<%# Eval("GoodsQuantity")%>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="OrderIndex" HeaderText="排序字段" UniqueName="OrderIndex">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="是否显示" UniqueName="ExchangeRate">
                                <ItemTemplate>
                                    <asp:CheckBox ID="CK_EnableFilter" runat="server" AutoPostBack="true" Checked='<%# Eval("EnabledFilter") %>' Enabled="false" /> 
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                                <rad:GridTemplateColumn HeaderText="是否多选" UniqueName="IsMChoice">
                                <ItemTemplate>
                                    <asp:CheckBox ID="CK_EnableIsMChoice" runat="server" AutoPostBack="true" Checked='<%# Eval("IsMChoice") %>' Enabled="false" /> 
                                </ItemTemplate>
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>

    
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RTV_GoodsType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GroupGrid" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTV_GoodsType" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="lbtnUpdate">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GroupGrid" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RTV_GoodsType" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>

</asp:Content>

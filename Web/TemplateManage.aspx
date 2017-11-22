<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MainMaster.master"
    CodeBehind="TemplateManage.aspx.cs" Inherits="ERP.UI.Web.TemplateManage" %>
<%@ Register Src="~/UserControl/ConfirmCheckBox.ascx" TagName="ConfirmCheckBox" TagPrefix="kd" %>

<asp:content id="TemplateManageControl" contentplaceholderid="CPHStage" runat="server">
<table class="StagePanel">
    <tr>
        <td style="vertical-align: top;">
            <rad:RadGrid ID="rgTemp" runat="server" SkinID="Common_Foot" AllowMultiRowSelection="true"
                OnNeedDataSource="RgTempNeedDataSource" OnUpdateCommand="RgTempUpdateCommand"
                OnDeleteCommand="RgTempDeleteCommand" OnInsertCommand="RgTempInsertCommand">
                <MasterTableView DataKeyNames="TemplateID" EditMode="InPlace">
                    <CommandItemTemplate>
                        <asp:linkbutton id="LBAdd" runat="server" commandname="InitInsert">
                    <asp:image id="IMGAdd" runat="server" imagealign="AbsMiddle" skinid="AddImageButton">
                    </asp:image>
                    添加模板
                </asp:linkbutton>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:linkbutton id="LBDel" runat="server" commandname="DeleteSelected">
                    <asp:image id="IMGDel" runat="server" imagealign="AbsMiddle" skinid="DeleteImageButton">
                    </asp:image>
                    删除模板
                </asp:linkbutton>
                        &nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:linkbutton id="LBRefresh" runat="server" commandname="RefreshGrid">
                    <asp:image id="IMGRef" runat="server" imagealign="AbsMiddle" skinid="RefreshImageButton">
                    </asp:image>
                    刷新
                </asp:linkbutton>
                    </CommandItemTemplate>
                    <Columns>
                        <rad:GridClientSelectColumn UniqueName="column">
                            <HeaderStyle Width="40px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                        </rad:GridClientSelectColumn>
                        <rad:GridTemplateColumn HeaderText="模版标题" UniqueName="TemplateCaption">
                            <ItemTemplate>
                                <asp:label id="LbTemplateCaption" runat="server" text='<%# Eval("TemplateCaption") %>'>
                                </asp:label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:textbox runat="server" text='<%# Eval("TemplateCaption") %>' id="TBTemplateCaption"
                                    textmode="MultiLine" height="35px" width="200px" cssclass="StandardInput">
                                </asp:textbox>
                                <asp:requiredfieldvalidator id="RFVTemplateCaption" runat="server" errormessage="模版标题不能为空！"
                                    text="*" controltovalidate="TBTemplateCaption">
                                </asp:requiredfieldvalidator>
                            </EditItemTemplate>
                            <HeaderStyle Width="100px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="模板内容" UniqueName="TemplateContent">
                            <ItemTemplate>
                                <asp:label id="LbTemplateContent" runat="server" text='<%# Eval("TemplateContent") %>'>
                                </asp:label>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <asp:textbox runat="server" id="TBTemplateContent" text='<%# Eval("TemplateContent") %>'
                                    textmode="MultiLine" height="35px" width="200px" cssclass="StandardInput">
                                </asp:textbox>
                                <asp:requiredfieldvalidator id="RFVTemplateContent" runat="server" errormessage="模板内容不能为空！"
                                    text="*" controltovalidate="TBTemplateContent">
                                </asp:requiredfieldvalidator>
                            </EditItemTemplate>
                            <HeaderStyle Width="160px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn HeaderText="模板类型" UniqueName="TemplateType">
                            <ItemTemplate>
                                <rad:RadComboBox ID="RCBTemplateType" runat="server" DataSource='<%#GetEnumDic() %>'
                                    DataTextField="value" DataValueField="key" OnDataBinding="RcbDataBinding" Enabled="false">
                                </rad:RadComboBox>
                            </ItemTemplate>
                            <EditItemTemplate>
                                <rad:RadComboBox ID="RCBTemplateType" runat="server" DataSource='<%#GetEnumDic() %>'
                                    DataTextField="value" DataValueField="key" Enabled="true" OnDataBinding="RcbDataBinding">
                                </rad:RadComboBox>
                            </EditItemTemplate>
                            <HeaderStyle Width="160px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                        </rad:GridTemplateColumn>
                        <rad:GridTemplateColumn DataField="TemplateState" HeaderText="启用" UniqueName="TemplateState">
                            <ItemTemplate>
                                <kd:ConfirmCheckBox ID="CCBTemplateState" runat="server" OnCheckedChanged="TemplateState_CheckedChanged"
                                    Checked='<%# Convert.ToInt32(Eval("TemplateState"))==0 ? false : true %>' />
                            </ItemTemplate>
                            <EditItemTemplate>
                                <kd:ConfirmCheckBox ID="CCBTemplateState" runat="server" OnCheckedChanged="TemplateState_CheckedChanged"
                                    Checked="true" Visible="false" />
                            </EditItemTemplate>
                            <HeaderStyle Width="40px" HorizontalAlign="Center" />
                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Top" />
                        </rad:GridTemplateColumn>
                        <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" EditText="编辑"
                            InsertText="添加" UpdateText="更新">
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
                <GroupPanel Visible="True">
                </GroupPanel>
            </rad:RadGrid>
        </td>
    </tr>
</table>
<rad:RadAjaxManager ID="RAM" runat="server">
    <AjaxSettings>
        <rad:AjaxSetting AjaxControlID="rgTemp">
            <UpdatedControls>
                <rad:AjaxUpdatedControl ControlID="rgTemp" LoadingPanelID="loading" />
            </UpdatedControls>
        </rad:AjaxSetting>
    </AjaxSettings>
</rad:RadAjaxManager>
<rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
</rad:RadAjaxLoadingPanel>
</asp:content>
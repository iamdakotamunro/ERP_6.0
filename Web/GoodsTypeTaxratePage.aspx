<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="GoodsTypeTaxratePage.aspx.cs" Inherits="ERP.UI.Web.GoodsTypeTaxratePage" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>

        <script type="text/javascript" language="javascript">

            function ShowRecord(obj, type) {
                debugger;
                window.radopen("./Windows/ShowProportionForm.aspx?GoodsType=" + type, "RW1");
                return false;
            }

            function refreshGrid() {
                location.reload();
            }

            window.onload = function () {
                document.body.onkeydown = function (event) {
                    if (event.keyCode == 13) {
                        return false;
                    }
                }
            }
        </script>
    </rad:RadScriptBlock>
    <table border="0" cellpadding="0" cellspacing="0" width="100%">
        <tr>
            <td class="ControlTools" style="text-align: center"></td>
        </tr>
    </table>

    <rad:RadGrid ID="RgGoodsTypeList" runat="server" SkinID="Common_Foot" OnNeedDataSource="RgGoodsTypeListNeedDataSource"
        OnItemDataBound="RgGoodsTypeListItemDataBound" OnDeleteCommand="RGGoodsTyp_DeleteCommand">
        <MasterTableView DataKeyNames="GoodsType,OriginalValue,GoodsTypeName,GoodsTypeCode" ClientDataKeyNames="GoodsType" CommandItemDisplay="None">
            <Columns>
                <rad:GridBoundColumn HeaderText="序号" DataField="OrderIndex">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" Width="60px" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn HeaderText="商品类型" DataField="GoodsTypeName">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="类型编码" DataField="GoodsTypeCode">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:TextBox ID="TbTypeCode" runat="server" SkinID="LongInput" Text='<%# Eval("GoodsTypeCode") %>' Width="150px"></asp:TextBox>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderStyle-HorizontalAlign="Center" HeaderText="税率">
                    <HeaderStyle HorizontalAlign="Center" Width="350" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:DropDownList runat="server" ID="DdlProportion" SkinID="LongDropDown" Width="120" AutoPostBack="True"
                            OnSelectedIndexChanged="DdlProportionSelectChanged">
                        </asp:DropDownList>
                        <asp:TextBox ID="TbProportion" runat="server" SkinID="LongInput" Width="80px"></asp:TextBox>
                        <asp:Label ID="LbUnit" runat="server" Text="%"></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作记录">
                    <ItemTemplate>
                        <asp:ImageButton ID="BtnFlow" OnClientClick='<%# "return ShowRecord(this,\"" + Eval("GoodsType")+ "\")" %>' runat="server" SkinID="InsertImageButton"></asp:ImageButton>
                    </ItemTemplate>
                    <HeaderStyle Width="150px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridButtonColumn ButtonType="PushButton" CommandName="Delete" Text="保存" ConfirmText="确实要此操作吗？" HeaderText="操作"
                    UniqueName="Delete">
                    <HeaderStyle Width="40px" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridButtonColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="ClewWindowManager" runat="server" Title="比例调整记录" Height="400px" Width="800px" ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="RW1" Width="750" Height="400" runat="server" Title="比例调整记录" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RgGoodsTypeList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RgGoodsTypeList" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
</asp:Content>

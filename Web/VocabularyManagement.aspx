<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="VocabularyManagement.aspx.cs" Inherits="ERP.UI.Web.VocabularyManagement" %>

<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <div style="padding-bottom: 5px;">
        <asp:TextBox ID="txt_VocabularyName" runat="server" Width="300px" placeholder="请输入词语名称"></asp:TextBox>
        <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
        <input type="button" onclick="Add()" value="添加违禁词" />
        <asp:Button ID="btn_BatchDel" runat="server" Text="批量删除" OnClick="btn_BatchDel_Click" />
        &nbsp;&nbsp;<asp:Button ID="btn_Txt" runat="server" Text="生成TXT" OnClick="btn_Txt_Click" />
    </div>

    <rad:RadGrid ID="RG_Vocabulary" runat="server" ShowFooter="true" OnNeedDataSource="RG_Vocabulary_NeedDataSource">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox value=-1 onclick=checkAll()&gt;全选">
                    <ItemTemplate>
                        <input title="请选择" type="checkbox" name="ckId" value='<%# Eval("Id")%>' />选择
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" VerticalAlign="Middle" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="VocabularyName" HeaderText="词语名称">
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="状态">
                    <ItemTemplate>
                        <%#Eval("State").Equals((int)VocabularyState.Enable)?"<input type=\"checkbox\" checked=\"checked\" onclick=\"CheckState('"+Eval("Id")+"','"+(int)VocabularyState.Disable+"');\" />":"<input type=\"checkbox\" onclick=\"CheckState('"+Eval("Id")+"','"+(int)VocabularyState.Enable+"');\" />" %>
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <ItemTemplate>
                        <asp:Button ID="btn_Del" runat="server" Text="删除" OnClientClick="return GiveTip(event,'您确定删除吗？')" CommandArgument='<%# Eval("Id") %>' OnClick="btn_Del_Click" />
                    </ItemTemplate>
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle Width="40px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <asp:Button ID="btn_CheckState" runat="server" Style="display: none;" OnClick="btn_CheckState_Click" />
    <asp:HiddenField ID="Hid_Id" runat="server" />
    <asp:HiddenField ID="Hid_State" runat="server" />
    <a id="a_Look" href="UserDir/Vocabulary/Export/bannedword.txt" target="_blank" style="color:red; display: none;">违禁词查看</a>

    <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="添加违禁词" Width="450px" Height="200px" />
        </Windows>
    </rad:RadWindowManager>

    <script src="JavaScript/GiveTip.js"></script>
    <script type="text/javascript">
        //添加
        function Add() {
            window.radopen("../Windows/AddVocabulary.aspx", "raw");
        }

        function CheckState(id, state) {
            $("input[id$='Hid_Id']").val(id);
            $("input[id$='Hid_State']").val(state);
            $("input[id$='btn_CheckState']").click();
        }

        //全选
        function checkAll() {
            var a = $("input[type='checkbox'][name='ckId']:not(:disabled)");
            var n = a.length;
            for (var i = 0; i < n; i++) {
                a[i].checked = window.event.srcElement.checked;
            }
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
    </script>
</asp:Content>

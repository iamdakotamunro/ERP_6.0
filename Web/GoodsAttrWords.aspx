<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="GoodsAttrWords.aspx.cs" Inherits="ERP.UI.Web.GoodsAttrWords" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <script src="JavaScript/GiveTip.js"></script>
    <table style="width: 100%;">
        <tr>
            <td style="width: 200px; vertical-align: top;">
                <rad:RadTreeView ID="RTV_AttrGroup" runat="server" UseEmbeddedScripts="false" Height="460px"
                    Width="250px" AutoPostBack="true" CausesValidation="false" OnNodeClick="Rtv_AttrGroup_NodeClick">
                </rad:RadTreeView>
                <asp:HiddenField ID="Hid_IsMChoice" runat="server" Value="-1" />
                <asp:HiddenField ID="Hid_MatchType" runat="server" Value="-1" />
                <asp:HiddenField ID="Hid_GroupId" runat="server" Value="0" />
            </td>
            <td style="vertical-align: top;">
                <asp:Button ID="btn_Search" runat="server" OnClick="btn_Search_Click" Style="display: none;" />
                <rad:RadGrid ID="RG_AttrWords" runat="server" OnNeedDataSource="RG_AttrWords_NeedDataSource">
                    <MasterTableView>
                        <CommandItemTemplate>
                            <asp:LinkButton ID="LB_AddWord" runat="server" OnClientClick="return AddOrEditGoodsAttrWords('');" Visible='<%#IsShow%>'>
                                <asp:Image ID="AddWord" runat="server" ImageAlign="AbsMiddle" SkinID="InsertImageButton" />添加属性名称
                            </asp:LinkButton>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="Word" HeaderText="属性名称">
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="IsShow" HeaderText="前台显示">
                                <ItemTemplate>
                                    <input type="checkbox" checked='<%# Eval("IsShow") %>' disabled="disabled" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="OrderIndex" HeaderText="排序字段">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="CompareType" HeaderText="比较类型">
                                <ItemTemplate>
                                    <asp:DropDownList ID="ddl_CompareType" runat="server" Enabled="False" SelectedValue='<%# Eval("CompareType") %>'>
                                        <asp:ListItem Text="等于" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="两者之间" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="小于等于" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="大于等于" Value="3"></asp:ListItem>
                                    </asp:DropDownList>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="WordValue" HeaderText="比较的值">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="TopValue" HeaderText="比较值上限">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="图片">
                                <ItemTemplate>
                                    <a <%# (Eval("AttrWordImage")==null?"title='暂无图片！'":(string.IsNullOrEmpty(Eval("AttrWordImage").ToString()))? "title='暂无图片！'":"href='"+Eval("AttrWordImage"))+"'"%> target="_blank">
                                        <%# Eval("AttrWordImage")==null?"":(string.IsNullOrEmpty(Eval("AttrWordImage").ToString())? "": "<img src='Images/goodsimg.png' />")%> 
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="操作">
                                <ItemTemplate>
                                    <input type="button" value="编辑" onclick='<%#"AddOrEditGoodsAttrWords(\""+Eval("WordId")+"\")"%>' />
                                    <asp:Button ID="btn_Del" runat="server" Text="删除" OnClick="btn_Del_Click" OnClientClick="return GiveTip(event,'您确认删除吗？');" CommandName='<%# Eval("WordId")%>' Enabled='<%# !(Convert.ToInt32(Eval("GoodsQuantity"))>0)%>' />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="100px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="属性对应商品">
                                <ItemTemplate>
                                    <input type="button" value='编辑(<%# Eval("GoodsQuantity")%>)' onclick='<%#"AttrWordsGoods(\"" + Eval("WordId") + "\");" %>' <%# Convert.ToInt32(Eval("GoodsQuantity"))>0?"":"disabled"%> />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="100px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>

    <rad:RadWindowManager ID="AttrWordsGoodManager" runat="server">
        <Windows>
            <rad:RadWindow ID="GoodsAttr" runat="server" Title="商品属性" Width="450px" Height="250px" />
            <rad:RadWindow ID="AttrWordsGoodsForm" runat="server" Title="属性对应商品" Height="577px" Width="900px" />
        </Windows>
    </rad:RadWindowManager>

    <script src="JavaScript/ToolTipMsg.js"></script>
    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
        });

        //添加商品属性
        function AddOrEditGoodsAttrWords(wordId) {
            window.radopen("./Windows/AddOrEditGoodsAttrWords.aspx?GroupId=" + $("input[id$='Hid_GroupId']").val() + "&WordId=" + wordId, "GoodsAttr");
            return false;
        }

        //属性对应商品
        function AttrWordsGoods(wordId) {
            window.radopen("./Windows/ShowAttrWordsGoodsForm.aspx?WordId=" + wordId + "&GroupId=" + $("input[id$='Hid_GroupId']").val() + "&MatchType=" + $("input[id$='Hid_MatchType']").val() + "&IsMChoice=" + $("input[id$='Hid_IsMChoice']").val(), "AttrWordsGoodsForm");
            return false;
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
    </script>
</asp:Content>

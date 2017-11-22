<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="NewGoods.aspx.cs" Inherits="ERP.UI.Web.NewGoods" %>

<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<%@ Register Src="~/UserControl/ConfirmCheckBox.ascx" TagName="ConfirmCheckBox" TagPrefix="kd" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <script src="JavaScript/GiveTip.js"></script>
    <script src="JavaScript/ToolTipMsg.js"></script>
    <table style="width: 100%;">
        <tr>
            <td style="vertical-align: top;">
                <div id="tree_GoodsClass" runat="server" lang="0">
                    <rad:RadTreeView ID="TVGoodsClass" runat="server" SkinID="Common" Height="550px"
                        Width="200px" CausesValidation="false" OnNodeClick="TvGoodsClassNodeClick">
                    </rad:RadTreeView>
                </div>
            </td>
            <td style="border: 1px solid #cccccc; width: 10px;">
                <div style="height: 45%;"></div>
                <img id="treeshow" alt="隐藏菜单" src="Images/move2.gif" width="10" height="56" onclick="TreeToggle();" style="cursor: pointer;" />
            </td>
            <td style="vertical-align: top;">
                <table style="width: 100%;">
                    <tr>
                        <td>卖库存状态：
                            <asp:DropDownList ID="ddl_SaleStockType" runat="server">
                                <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                <asp:ListItem Text="非卖库存" Value="0"></asp:ListItem>
                                <asp:ListItem Text="卖完缺货" Value="1"></asp:ListItem>
                                <asp:ListItem Text="卖完断货" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                            商品名称/编号：
                            <asp:TextBox ID="txt_GoodsNameOrCode" runat="server"></asp:TextBox>
                            商品资料：
                            <asp:DropDownList ID="ddl_HasInformation" runat="server">
                                <asp:ListItem Text="全部" Value=""></asp:ListItem>
                                <asp:ListItem Text="有" Value="True"></asp:ListItem>
                                <asp:ListItem Text="无" Value="False"></asp:ListItem>
                            </asp:DropDownList>
                            审核状态：
                            <rad:RadComboBox ID="rcb_GoodsAuditState" runat="server" Height="200px"></rad:RadComboBox>
                            <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
                        </td>
                    </tr>
                    <tr style="background-color: #F4F4F4;">
                        <td style="text-align: right;">
                            <input type="button" value="首营商品" onclick="Insert()" />
                            <input type="button" value="批量转移" onclick="moveShow()" />
                            <asp:Button ID="btn_Audit" runat="server" Text="批量审核" Visible="False" OnClientClick="return GiveTip(event,'您确定批量审核吗？')" OnClick="btn_Audit_Click" />
                            <asp:Button ID="btn_Del" runat="server" Text="批量删除" OnClientClick="return GiveTip(event,'您确定批量删除吗？')" OnClick="btn_Del_Click" />
                            <asp:Button ID="btn_ForceDel" runat="server" Text="批量强制删除" OnClientClick="return GiveTip(event,'您确定批量强制删除吗？')" OnClick="btn_ForceDel_Click" />
                        </td>
                    </tr>
                </table>

                <rad:RadGrid ID="RG_Goods" runat="server" SkinID="CustomPaging" OnNeedDataSource="RG_Goods_NeedDataSource">
                    <MasterTableView DataKeyNames="GoodsId">
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle Height="0px" />
                        <Columns>
                            <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox onclick=checkAll()&gt;全选">
                                <ItemTemplate>
                                    <input title="请选择" type="checkbox" name="ckId" value='<%# Eval("GoodsId")+"&"+Eval("GoodsName")+"&"+Eval("GoodsAuditState")+"&"+Eval("GoodsCode")%>' />选择
                                </ItemTemplate>
                                <HeaderStyle Width="50px" HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号">
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称">
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="市场价(元)">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("MarketPrice"))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="参考价(元)">
                                <ItemTemplate>
                                    <%# ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("ExpandInfo.ReferencePrice"))%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="ClassInfo.ClassName" HeaderText="商品分类">
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="商品类型">
                                <ItemTemplate>
                                    <%# EnumAttribute.GetKeyName((GoodsKindType)Eval("GoodsType")) %>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审核状态">
                                <ItemTemplate>
                                    <%# EnumAttribute.GetKeyName((GoodsAuditState)Eval("GoodsAuditState")) %>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="100px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="商品图片">
                                <ItemTemplate>
                                    <a <%# (Eval("ImgPath")==null?"title='暂无图片！'":string.IsNullOrEmpty(Eval("ImgPath").ToString())? "title='暂无图片！'":"href='"+Eval("ImgPath"))+"' target='_blank'"%>>
                                        <%# Eval("ImgPath")==null?"":(string.IsNullOrEmpty(Eval("ImgPath").ToString())? "": "<img src='Images/goodsimg.png' />")%> 
                                    </a>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="25px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="备注">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="80%" tooltipmsg='<%#Eval("GoodsAuditStateMemo")==null?"":Eval("GoodsAuditStateMemo").ToString().Replace("\n","<br/>")%>' />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="25px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="卖完缺货">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ccbSaleStock" Checked='<%# Convert.ToInt32(Eval("SaleStockType"))==(int)SaleStockType.ShortStock  %>' Enabled="False" runat="server" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="25px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="卖完断货">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ccbSaleStockDown" Checked='<%# Convert.ToInt32(Eval("SaleStockType"))==(int)SaleStockType.OutOfStock  %>' Enabled="False" runat="server" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="25px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="缺货">
                                <ItemTemplate>
                                    <kd:ConfirmCheckBox ID="ccb_IsStockScarcity" OnCheckedChanged="ccb_IsStockScarcity_CheckedChanged"
                                        Checked='<%# Convert.ToBoolean(Eval("IsStockScarcity")) %>' runat="server" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="25px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="上架">
                                <ItemTemplate>
                                    <kd:ConfirmCheckBox ID="ccb_IsOnShelf" OnCheckedChanged="ccb_IsOnShelf_CheckedChanged"
                                        Checked='<%# Convert.ToInt32(Eval("IsOnShelf"))==1 %>' runat="server" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="25px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="商品编辑">
                                <ItemTemplate>
                                    <input type="button" value="编辑" onclick="<%#"EditGoods('"+Eval("GoodsId")+"')"%>" <%#(Eval("GoodsAuditState").ToString().Equals("0")||Eval("GoodsAuditState").ToString().Equals("1")||Eval("GoodsAuditState").ToString().Equals("4"))?"":"disabled=\"disabled\""%> />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="商品属性">
                                <ItemTemplate>
                                    <input type="button" value="编辑" onclick="<%#"EditGoodsAttribute('"+Eval("GoodsId")+"')"%>" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="子商品">
                                <ItemTemplate>
                                    <input type="button" value="编辑" onclick="<%#"EditChildGoods('"+Eval("GoodsId")+"','"+Eval("ClassId")+"')"%>" <%# Convert.ToBoolean(Eval("HasRealGoods"))?"":"disabled=\"disabled\"" %> />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="40px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="卖库存申请">
                                <ItemTemplate>
                                    <input type="button" value='<%# GetSaleStockState(Convert.ToInt32(Eval("SaleStockState")))  %>' onclick="<%#"SaleStockStateApply('"+Eval("GoodsId")+"')"%>" <%#Eval("GoodsAuditState").ToString().Equals("4")?"":"disabled=\"disabled\""%> />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle Width="60px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="审核">
                                <ItemTemplate>
                                    <input type="button" value='<%#GetApproved(Convert.ToInt32(Eval("GoodsAuditState"))).Equals(0)?"查看": EnumAttribute.GetKeyName((GoodsAuditState)Convert.ToInt32(Eval("GoodsAuditState")))%>' onclick="<%#"Approved('"+Eval("GoodsId")+"','"+GetApproved(Convert.ToInt32(Eval("GoodsAuditState")))+"')"%>" />
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

    <rad:RadWindowManager runat="server">
        <Windows>
            <rad:RadWindow ID="FirstGoods" runat="server" Width="1600px" Height="700px" />
            <rad:RadWindow ID="EditGoodsAttributeForm" Title="商品属性" runat="server" Width="350px" Height="400px" />
            <rad:RadWindow ID="ApplySaleStockForm" Title="商品信息" runat="server" Width="500px" Height="300px" />
        </Windows>
    </rad:RadWindowManager>

    <asp:HiddenField ID="Hid_GoodsClassId" runat="server" Value="00000000-0000-0000-0000-000000000000" />
    <asp:HiddenField ID="Hid_TreeToggle" runat="server" Value="0" />
    <div id="div_GoodsClass" style="display: none; position: absolute; _position: absolute; z-index: 100; border: solid 1px #718CA1; background-color: #DAE2E8; width: 400px; font-size: 13px;">
        <table style="width: 100%;" cellpadding="0" cellspacing="0">
            <tr style="background: url(../App_Themes/layout-browser-hd-bg.gif) repeat-x #526ea6; height: 25px;">
                <td style="color: blue; font-weight: bolder;">转移分类</td>
                <td colspan="2" style="text-align: right;"><a href="javascript:moveHide();" style="color: red; font-size: 16px; padding-right: 5px;">X</a></td>
            </tr>
            <tr>
                <td>商品分类：
                </td>
                <td>
                    <asp:DropDownList ID="ddl_GoodsClass" runat="server">
                    </asp:DropDownList>
                </td>
                <td>
                    <asp:Button ID="btn_Move" runat="server" Text="转移" OnClick="btn_Move_Click"></asp:Button>
                </td>
            </tr>
        </table>
    </div>

    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
            $("#div_GoodsClass").css({ "top": $(document.body).height() / 3, "left": $(document.body).width() / 3 });
            if ($("input[id$='Hid_GoodsClassId']").val() === "00000000-0000-0000-0000-000000000000" && $("input[id$='Hid_TreeToggle']").val() === "0") {
                TreeToggle();//商品分类
            }
        });

        //商品分类
        function TreeToggle() {
            if ($("div[id$='tree_GoodsClass']").attr("lang") === "0") {
                $("div[id$='tree_GoodsClass']").css("display", "none");
                $("div[id$='tree_GoodsClass']").attr("lang", "1");
                $("#treeshow").attr("src", "Images/move2.gif");
            } else {
                $("div[id$='tree_GoodsClass']").css("display", "");
                $("div[id$='tree_GoodsClass']").attr("lang", "0");
                $("#treeshow").attr("src", "Images/move1.gif");
            }
        }

        //全选
        function checkAll() {
            var a = $("input[type='checkbox'][name='ckId']");
            var n = a.length;
            for (var i = 0; i < n; i++) {
                a[i].checked = window.event.srcElement.checked;
            }
        }

        //转移显示
        function moveShow() {
            $("#div_GoodsClass").show();
        }

        //转移隐藏
        function moveHide() {
            $("#div_GoodsClass").hide();
        }

        //新增首营商品
        function Insert() {
            window.radopen("./Windows/AddFirstGoods.aspx", "FirstGoods");
        }

        //审核商品
        function Approved(goodsId, isShow) {
            window.radopen("./Windows/ApprovedFirstGoods.aspx?GoodsId=" + goodsId + "&IsShow=" + isShow, "FirstGoods");
        }

        //编辑商品
        function EditGoods(goodsId) {
            var w = screen.availWidth;
            var h = screen.availHeight;
            window.radopen("./Windows/EditFirstGoods.aspx?GoodsId=" + goodsId, "FirstGoods");
        }

        function clientShow(sender, eventArgs) {
            sender.set_initialBehaviors("Maximize,Close");
            sender.Maximize(true);
            sender.IsMaximized(true); //最大化
        }

        //商品属性
        function EditGoodsAttribute(goodsId) {
            window.radopen("./Windows/EditGoodsAttribute.aspx?GoodsId=" + goodsId, "EditGoodsAttributeForm");
        }

        //子商品
        function EditChildGoods(goodsId, classId) {
            window.radopen("./Windows/BuildChildGoodsForm.aspx?GoodsId=" + goodsId + "&ClassId=" + classId, "FirstGoods");
        }

        //卖库存申请
        function SaleStockStateApply(goodsId) {
            window.radopen("./Windows/AddGoodSaleStockForm.aspx?GoodsId=" + goodsId + "&Type=Apply", "ApplySaleStockForm");
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
    </script>
</asp:Content>

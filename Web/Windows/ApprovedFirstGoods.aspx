<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ApprovedFirstGoods.aspx.cs" Inherits="ERP.UI.Web.Windows.ApprovedFirstGoods" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>商品审核</title>
    <link href="../Styles/tab.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360">
        </rad:RadScriptManager>
        <div style="background-color: #F2F2F2;">
            <div class="tab">
                <a href="javascript:void(0);" onclick="ChangeTab(this,'BasicInfo')" class="now">基本信息</a>
                <a href="javascript:void(0);" onclick="ChangeTab(this,'OtherInfo')">其他信息</a>
                <a id="A_FrameInfo" href="javascript:void(0);" onclick="ChangeTab(this,'FrameInfo')" style="display: none; color: blue;">框架信息</a>
                <a href="javascript:void(0);" onclick="ChangeTab(this,'RelatedCompanyPurchasingGroup')">关联公司采购组</a>
                <a href="javascript:void(0);" onclick="ChangeTab(this,'RelatedSalesGroupPlatform')">关联销售组平台</a>
                <a id="A_SubProductAttributes" href="javascript:void(0);" onclick="ChangeTab(this,'SubProductAttributes')" style="display: none; color: blue;">子商品属性</a>
                <a id="A_Medicament" href="javascript:void(0);" onclick="ChangeTab(this,'Medicament')" style="display: none; color: blue;">药品</a>
                <asp:HiddenField ID="Hid_FrameInfo" runat="server" />
                <asp:HiddenField ID="Hid_SubProductAttributes" runat="server" />
                <asp:HiddenField ID="Hid_Medicament" runat="server" />
            </div>
            <div id="BasicInfo" style="padding: 10px 0 10px 0;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">商品名称：</td>
                        <td>
                            <asp:TextBox ID="txt_GoodsName" runat="server" ReadOnly="True" Width="300px"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">商品名称首字母：</td>
                        <td>
                            <asp:TextBox ID="txt_PurchaseNameFirstLetter" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">商品品牌：</td>
                        <td>
                            <asp:DropDownList ID="ddl_Brand" runat="server" Enabled="False"></asp:DropDownList>
                        </td>
                        <td style="text-align: right;">商品编号(供应商)：</td>
                        <td>
                            <asp:TextBox ID="txt_SupplierGoodsCode" runat="server" ReadOnly="True" Width="237px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">商品类型：</td>
                        <td>
                            <asp:DropDownList ID="ddl_GoodsType" runat="server" Enabled="False"></asp:DropDownList>
                        </td>
                        <td style="text-align: right;">商品分类：</td>
                        <td>
                            <asp:DropDownList ID="ddl_GoodsClass" runat="server" Enabled="False"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">计量单位：</td>
                        <td>
                            <asp:DropDownList ID="ddl_Units" runat="server" Enabled="False"></asp:DropDownList>
                        </td>
                        <td style="text-align: right;">规格：</td>
                        <td>
                            <asp:TextBox ID="txt_Specification" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">卖库存：</td>
                        <td>
                            <asp:DropDownList ID="ddl_SaleStockType" runat="server" Enabled="False">
                                <asp:ListItem Text="非卖库存" Value="0"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">库存状况：</td>
                        <td>
                            <asp:TextBox ID="txt_StockStatus" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">每箱数量：</td>
                        <td>
                            <asp:TextBox ID="txt_PackCount" runat="server" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">条形码：</td>
                        <td>
                            <asp:TextBox ID="txt_BarCode" runat="server" ReadOnly="True" Width="237px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">市场价(元)：</td>
                        <td>
                            <asp:TextBox ID="txt_MarketPrice" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">参考价(元)：</td>
                        <td>
                            <asp:TextBox ID="txt_ReferencePrice" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">保质期：</td>
                        <td>
                            <asp:TextBox ID="txt_ShelfLife" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">进口商品：</td>
                        <td>
                            <asp:CheckBox ID="ckb_IsImportedGoods" runat="server" Enabled="False" />
                            奢侈品：<asp:CheckBox ID="ckb_IsLuxury" runat="server" Enabled="False" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">禁采：</td>
                        <td>
                            <asp:CheckBox ID="ckb_IsBannedPurchase" runat="server" Enabled="False" />
                            禁销：<asp:CheckBox ID="ckb_IsBannedSale" runat="server" Enabled="False" />
                        </td>
                        <td style="text-align: right;">统计绩效：</td>
                        <td>
                            <asp:CheckBox ID="ckb_IsStatisticalPerformance" runat="server" Enabled="False" />
                            上(下)架：<asp:CheckBox ID="ckb_IsOnShelf" runat="server" Enabled="False" />
                        </td>
                    </tr>
                    <tr id="UploadImgName" runat="server">
                        <td style="text-align: right;">图片：</td>
                        <td colspan="3">
                            <a id="PreA" runat="server" href="javascript:void(0);" target="_blank"><b style="color: red;">预览</b></a>
                            <img id="imgPre" style="display: none;" />
                        </td>
                    </tr>
                </table>
            </div>
            <div id="OtherInfo" style="display: none; padding: 10px 0 10px 0;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">重量(g)：</td>
                        <td>
                            <asp:TextBox ID="txt_Weight" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">长度(mm)：</td>
                        <td>
                            <asp:TextBox ID="txt_Length" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">宽度(mm)：</td>
                        <td>
                            <asp:TextBox ID="txt_Width" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">高度(mm)：</td>
                        <td>
                            <asp:TextBox ID="txt_Height" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="FrameInfo" style="display: none; padding: 10px 0 10px 0;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">框架腿长：</td>
                        <td>
                            <asp:TextBox ID="txt_TempleLength" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">框架外宽：</td>
                        <td>
                            <asp:TextBox ID="txt_Besiclometer" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">框架内宽：</td>
                        <td>
                            <asp:TextBox ID="txt_FrameWithinWidth" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">鼻梁宽度：</td>
                        <td>
                            <asp:TextBox ID="txt_NoseWidth" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">镜片高度：</td>
                        <td>
                            <asp:TextBox ID="txt_OpticalVerticalHeight" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">镜片宽度：</td>
                        <td>
                            <asp:TextBox ID="txt_EyeSize" runat="server" Width="110px" Text="0" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="RelatedCompanyPurchasingGroup" style="display: none; padding: 10px 0 10px 0;">
                <rad:RadGrid ID="RG_RelatedCompanyPurchasingGroup" runat="server" AllowPaging="False" OnNeedDataSource="RG_RelatedCompanyPurchasingGroup_NeedDataSource">
                    <MasterTableView DataKeyNames="ID">
                        <CommandItemTemplate>
                        </CommandItemTemplate>
                        <CommandItemStyle Height="0px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="Name" HeaderText="公司">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="100px"></ItemStyle>
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="全选">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckb_All" runat="server" Checked='<%# Eval("IsAll") %>' Enabled="False" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="直营">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckb_IsDirect" runat="server" Checked='<%# Eval("IsDirect") %>' Enabled="False" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="加盟">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckb_IsJoin" runat="server" Checked='<%# Eval("IsJoin") %>' Enabled="False" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="联盟">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckb_IsAlliance" runat="server" Checked='<%# Eval("IsAlliance") %>' Enabled="False" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </div>
            <div id="RelatedSalesGroupPlatform" style="display: none; padding: 10px 0 10px 0;">
                <asp:CheckBoxList ID="ckb_RelatedSalesGroupPlatform" runat="server" RepeatDirection="Horizontal" Width="100%" Enabled="False"></asp:CheckBoxList>
            </div>
            <div id="Medicament" style="display: none; padding: 10px 0 10px 0;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">商品化学名：</td>
                        <td>
                            <asp:TextBox ID="txt_ChemistryName" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">商品化学名首字母：</td>
                        <td>
                            <asp:TextBox ID="txt_ChemistryNameFirstLetter" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">质检分类：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineQualityType" runat="server" Enabled="False">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">销售品种：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineSaleKindType" runat="server" Enabled="False">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">批发价：</td>
                        <td>
                            <asp:TextBox ID="txt_MedicineWholesalePrice" runat="server" ReadOnly="True"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">税率：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineTaxRateType" runat="server" Enabled="False">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="0%" Value="0"></asp:ListItem>
                                <asp:ListItem Text="6%" Value="6"></asp:ListItem>
                                <asp:ListItem Text="13%" Value="13"></asp:ListItem>
                                <asp:ListItem Text="17%" Value="17"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">剂型：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineDosageFormType" runat="server" Enabled="False">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">储存条件：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineStorageConditionType" runat="server" Enabled="False">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">养护类别：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineCuringKindType" runat="server" Enabled="False">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="重点养护" Value="0"></asp:ListItem>
                                <asp:ListItem Text="一般养护" Value="1"></asp:ListItem>
                                <asp:ListItem Text="阴凉养护" Value="2"></asp:ListItem>
                                <asp:ListItem Text="冷藏养护" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">养护周期：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineCuringCycleType" runat="server" Enabled="False">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="每周" Value="0"></asp:ListItem>
                                <asp:ListItem Text="每月" Value="1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">门店柜台：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineStoreCounterType" runat="server" Enabled="False">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">库位管理</td>
                        <td>
                            <asp:DropDownList ID="ddl_LibraryManageType" runat="server"  Enabled="False">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">质量标准：</td>
                        <td colspan="3">
                            <asp:TextBox ID="txt_QualityStandardDescription" runat="server" TextMode="MultiLine" Width="99%" ReadOnly="True"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div style="padding-top: 10px;">
            <table style="width: 100%;">
                <tr>
                    <td style="text-align: right; width: 100px;">审核说明：</td>
                    <td>
                        <asp:TextBox ID="txt_GoodsAuditStateMemo" runat="server" TextMode="MultiLine" Width="99%" Height="50px"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td style="text-align: center;" colspan="2">
                        <asp:Button ID="btn_Pass" runat="server" Text="核准" OnClick="btn_Pass_Click" />&nbsp;&nbsp;&nbsp;&nbsp;
                        <asp:Button ID="btn_NoPass" runat="server" Text="核退" OnClientClick="return CheckEmpty();" OnClick="btn_NoPass_Click" />
                        <asp:HiddenField ID="Hid_GoodsAuditState" runat="server" />
                    </td>
                </tr>
            </table>
        </div>

        <script src="../JavaScript/jquery.js"></script>
        <script src="../JavaScript/tool.js"></script>
        <script src="../JavaScript/PreviewImage.js"></script>
        <script src="../JavaScript/telerik.js"></script>

        <script type="text/javascript">
            var preA = null, preContainer = null;
            $(function () {
                preA = $(".tab a").get(0);
                preContainer = $("#BasicInfo");

                if ($("input[id$='Hid_FrameInfo']").val() === "1") {
                    $("#A_FrameInfo").css("display", "");
                }
                if ($("input[id$='Hid_Medicament']").val() === "1") {
                    $("#A_Medicament").css("display", "");
                }
                if ($("input[id$='Hid_SubProductAttributes']").val() === "1") {
                    $("#A_SubProductAttributes").css("display", "");
                }
            });

            function ChangeTab(obj, id) {
                var ddlGoodsType = $("select[id$='ddl_GoodsType']");
                var ddlGoodsClass = $("select[id$='ddl_GoodsClass']");
                if (ddlGoodsType.val().length !== 0 && ddlGoodsClass.val().length !== 0) {
                    if (preA != null && preContainer != null) {
                        preA.className = "";
                        preContainer.css("display", "none");
                    }
                    preA = obj;
                    preContainer = $("#" + id);
                    obj.className = "now";
                    preContainer.css("display", "");
                    obj.blur();
                } else {
                    alert("请选择“商品类型”和“商品分类”!");
                }
            }


            //验证
            function CheckEmpty() {
                $("span[class='error']").remove();//移除所有错误提示

                //审核说明
                CheckGoodsAuditStateMemo();

                if ($("span[class='error']").length === 0) {
                    return true;
                } else {
                    return false;
                }
            }

            //审核说明
            function CheckGoodsAuditStateMemo() {
                var txtGoodsAuditStateMemo = $("#txt_GoodsAuditStateMemo");
                if (txtGoodsAuditStateMemo.val().length === 0 || !$.chineseLengthValid(txtGoodsAuditStateMemo.val(), 0, 400)) {
                    if (txtGoodsAuditStateMemo.next("span[class='error']").length === 0) {
                        txtGoodsAuditStateMemo.after("<span class='error'></span>");
                        txtGoodsAuditStateMemo.css("border-color", "red");
                    }
                } else {
                    txtGoodsAuditStateMemo.next("span[class='error']").remove();
                    txtGoodsAuditStateMemo.css("border-color", "");
                }
            }
        </script>
    </form>
</body>
</html>

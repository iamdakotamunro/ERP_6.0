<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddFirstGoods.aspx.cs" Inherits="ERP.UI.Web.Windows.AddFirstGoods" %>

<%@ Register TagPrefix="Ibt" TagName="Imagebuttoncontrol" Src="~/UserControl/ImageButtonControl.ascx" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title>首营商品</title>
    <link href="../Styles/tab.css" rel="stylesheet" />
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360">
        </rad:RadScriptManager>
        <div style="background-color: #F2F2F2;">
            <div class="tab">
                <a href="javascript:void(0);" onclick="ChangeTab(this,'BasicInfo')" id="A_BasicInfo" class="now">基本信息</a>
                <a href="javascript:void(0);" onclick="ChangeTab(this,'OtherInfo')">其他信息</a>
                <a id="A_FrameInfo" href="javascript:void(0);" onclick="ChangeTab(this,'FrameInfo')" style="display: none; color: blue;">框架信息</a>
                <a href="javascript:void(0);" onclick="ChangeTab(this,'RelatedCompanyPurchasingGroup')">设置商品可采购门店</a>
                <a href="javascript:void(0);" onclick="ChangeTab(this,'RelatedSalesGroupPlatform')">设置商品可销售平台</a>
                <a id="A_SubProductAttributes" href="javascript:void(0);" onclick="ChangeTab(this,'SubProductAttributes')" style="display: none; color: blue;">子商品属性</a>
                <a id="A_Medicament" href="javascript:void(0);" onclick="ChangeTab(this,'Medicament')" style="display: none; color: blue;">药品</a>
                <a id="A_GoodsQualification" href="javascript:void(0);" onclick="ChangeTab(this,'GoodsQualification')">商品资质</a>
                <asp:HiddenField ID="Hid_FrameInfo" runat="server" />
                <asp:HiddenField ID="Hid_SubProductAttributes" runat="server" />
                <asp:HiddenField ID="Hid_Medicament" runat="server" />
                <asp:HiddenField ID="Hid_Other" runat="server" />
                <asp:HiddenField ID="Hid_GoodsQualification" runat="server" />
            </div>
            <div id="BasicInfo" style="padding: 10px 0 10px 0;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">商品名称：</td>
                        <td>
                            <asp:TextBox ID="txt_GoodsName" runat="server" Width="300px" CssClass="Check"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">商品名称首字母：</td>
                        <td>
                            <asp:TextBox ID="txt_PurchaseNameFirstLetter" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">商品品牌：</td>
                        <td>
                            <rad:RadComboBox ID="rcb_Brand" CssClass="Check" AllowCustomText="true" EnableLoadOnDemand="True" runat="server" Height="200px" OnItemsRequested="rcb_Brand_ItemsRequested"></rad:RadComboBox>
                        </td>
                        <td style="text-align: right;">商品编号(供应商)：</td>
                        <td>
                            <asp:TextBox ID="txt_SupplierGoodsCode" runat="server" Width="237px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">商品类型：</td>
                        <td>
                            <rad:RadComboBox ID="rcb_GoodsType" CssClass="Check" AllowCustomText="true" EnableLoadOnDemand="True" runat="server" Height="200px" OnItemsRequested="rcb_GoodsType_ItemsRequested" OnSelectedIndexChanged="rcb_GoodsType_SelectedIndexChanged" AutoPostBack="True"></rad:RadComboBox>
                            <asp:HiddenField ID="Hid_GoodsType" runat="server" />
                        </td>
                        <td style="text-align: right;">商品分类：</td>
                        <td>
                            <asp:DropDownList ID="ddl_GoodsClass" runat="server" CssClass="Check" OnSelectedIndexChanged="ddl_GoodsClass_SelectedIndexChanged" AutoPostBack="True"></asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">计量单位：</td>
                        <td>
                            <asp:DropDownList ID="ddl_Units" runat="server"></asp:DropDownList>
                        </td>
                        <td style="text-align: right;">规格：</td>
                        <td>
                            <asp:TextBox ID="txt_Specification" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">卖库存：</td>
                        <td>
                            <asp:DropDownList ID="ddl_SaleStockType" runat="server" Enabled="False">
                                <asp:ListItem Text="非卖库存" Value="0" Selected="True"></asp:ListItem>
                                <asp:ListItem Text="卖完缺货" Value="1"></asp:ListItem>
                                <asp:ListItem Text="卖完断货" Value="2"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">库存状况：</td>
                        <td>
                            <asp:TextBox ID="txt_StockStatus" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">每箱数量：</td>
                        <td>
                            <asp:TextBox ID="txt_PackCount" runat="server" Text="0" onblur="check(this,'Int');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">条形码：</td>
                        <td>
                            <asp:TextBox ID="txt_BarCode" runat="server" Width="237px"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">市场价(元)：</td>
                        <td>
                            <asp:TextBox ID="txt_MarketPrice" runat="server" CssClass="Check" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">参考价(元)：</td>
                        <td>
                            <asp:TextBox ID="txt_ReferencePrice" runat="server" CssClass="Check" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">保质期：</td>
                        <td>
                            <asp:TextBox ID="txt_ShelfLife" runat="server"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">进口商品：</td>
                        <td>
                            <asp:CheckBox ID="ckb_IsImportedGoods" runat="server" />
                            奢侈品：<asp:CheckBox ID="ckb_IsLuxury" runat="server" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">禁采：</td>
                        <td>
                            <asp:CheckBox ID="ckb_IsBannedPurchase" runat="server" />
                            禁销：<asp:CheckBox ID="ckb_IsBannedSale" runat="server" />
                        </td>
                        <td style="text-align: right;">统计绩效：</td>
                        <td>
                            <asp:CheckBox ID="ckb_IsStatisticalPerformance" runat="server" />
                            上(下)架：<asp:CheckBox ID="ckb_IsOnShelf" runat="server" Checked="True" />
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">图片：</td>
                        <td colspan="3">
                            <asp:TextBox ID="UploadImgName" runat="server" onfocus="this.blur();"></asp:TextBox>
                            <asp:FileUpload ID="UploadImg" runat="server" Style="display: none;" />
                            <input id="btnUploadImg" type="button" value="选择文件" title="选择图片!" onclick="UploadImg.click()" />
                            <a id="PreA" runat="server" href="javascript:void(0);" target="_blank"><b style="color: red;">预览</b></a>
                            <img id="imgPre" style="display: none;" />
                            <a href="javascript:void(0);" onclick="clearImg()"><b style="color: red;">删除</b></a>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="OtherInfo" style="display: none; padding: 10px 0 10px 0;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">重量(g)：</td>
                        <td>
                            <asp:TextBox ID="txt_Weight" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">长度(mm)：</td>
                        <td>
                            <asp:TextBox ID="txt_Length" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">宽度(mm)：</td>
                        <td>
                            <asp:TextBox ID="txt_Width" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">高度(mm)：</td>
                        <td>
                            <asp:TextBox ID="txt_Height" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="FrameInfo" style="display: none; padding: 10px 0 10px 0;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">框架腿长：</td>
                        <td>
                            <asp:TextBox ID="txt_TempleLength" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">框架外宽：</td>
                        <td>
                            <asp:TextBox ID="txt_Besiclometer" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">框架内宽：</td>
                        <td>
                            <asp:TextBox ID="txt_FrameWithinWidth" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">鼻梁宽度：</td>
                        <td>
                            <asp:TextBox ID="txt_NoseWidth" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">镜片高度：</td>
                        <td>
                            <asp:TextBox ID="txt_OpticalVerticalHeight" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">镜片宽度：</td>
                        <td>
                            <asp:TextBox ID="txt_EyeSize" runat="server" Width="110px" Text="0" onblur="check(this,'Decimal');"></asp:TextBox>
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
                                    <asp:CheckBox ID="ckb_All" runat="server" onclick="checkPurchasingGroup(this)" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="直营">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckb_IsDirect" runat="server" onclick="checkPurchasingGroup(this)" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="加盟">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckb_IsJoin" runat="server" onclick="checkPurchasingGroup(this)" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="联盟">
                                <ItemTemplate>
                                    <asp:CheckBox ID="ckb_IsAlliance" runat="server" onclick="checkPurchasingGroup(this)" />
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center"></ItemStyle>
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </div>
            <div id="RelatedSalesGroupPlatform" style="display: none; padding: 10px 0 10px 0;">
                <asp:CheckBoxList ID="ckb_RelatedSalesGroupPlatform" runat="server" RepeatDirection="Horizontal" Width="100%"></asp:CheckBoxList>
            </div>
            <div id="GoodsQualification" style="display: none; padding: 10px 0 10px 0;">
                <asp:Button ID="btn_AddInfo" runat="server" Text="添&nbsp;&nbsp;&nbsp;&nbsp;加" Width="100%" Height="30px" ForeColor="red" Font-Size="16" Font-Bold="true" OnClick="btn_AddInfo_Click" />
                <asp:Repeater ID="Repeater_GoodsInformations" runat="server" OnItemDataBound="Repeater_GoodsInformations_ItemDataBound">
                    <ItemTemplate>
                        <table style="width: 100%;">
                            <tr>
                                <td colspan="8">
                                    <hr />
                                </td>
                            </tr>
                            <tr>
                                <td style="width: 371px;">商品资质：
                                    <asp:DropDownList ID="ddl_GoodsQualificationType" runat="server" OnSelectedIndexChanged="ddl_GoodsQualificationType_SelectedIndexChanged" AutoPostBack="true">
                                    </asp:DropDownList>
                                    <asp:Literal ID="Lit_State" runat="server"></asp:Literal>
                                </td>
                                <td style="width: 371px;" id="td_UnitName" runat="server">单位名称：
                                    <asp:TextBox ID="txt_UnitName" Text='<%#Eval("Number") %>' runat="server"></asp:TextBox>
                                </td>
                                <td style="width: 371px;" id="td_CertificateNumber" runat="server">证书号码：
                                    <asp:TextBox ID="txt_CertificateNumber" Text='<%#Eval("Number") %>' runat="server"></asp:TextBox>
                                </td>
                                <td style="width: 371px;" id="td_UploadImgNameInfo" runat="server">资质资料：
                                    <asp:TextBox ID="UploadImgNameInfo" runat="server" Text='<%#(Eval("Path")!=null&&Eval("Path").ToString().Contains("/"))?Eval("Path").ToString().Substring(Eval("Path").ToString().LastIndexOf("/") + 1):Eval("Path")%>' onfocus="this.blur();"></asp:TextBox>
                                    <asp:FileUpload ID="UploadImgInfo" runat="server" Style="display: none;" />
                                    <input id="btnUploadImgInfo" type="button" value="选择文件" onclick="SelectImgInfo(this);" />
                                    <a id="PreAInfo" runat="server" href='<%#Eval("Path")!=null?Eval("Path"):"javascript:void(0);"%>' target="_blank"><b style="color: red;">预览</b></a>
                                    <img id="imgPreInfo" runat="server" style="display: none;" />
                                    <a href="javascript:void(0);" onclick="clearImgInfo(this);"><b style="color: red;">删除</b></a>
                                    <asp:HiddenField ID="Hid_ImgPath" runat="server" Value='<%#Eval("Path")%>' />
                                </td>
                                <td style="width: 371px;" id="td_OverdueDate" runat="server">过期日期：
                                    <asp:TextBox ID="txt_OverdueDate" runat="server" onfocus="this.blur();" Text='<%#Eval("OverdueDate") %>' onclick="WdatePicker({skin:'blue',minDate:'%y-%M-{%d}'})"></asp:TextBox>
                                </td>
                                <td style="text-align: right;">
                                    <Ibt:Imagebuttoncontrol ID="Imgbtn_Del" runat="server" Text="删除" SkinType="Delete" CommandName='<%#Eval("Id") %>' OnClick="Imgbtn_Del_Click" OnClientClick="return GiveTip(event,'确认删除？')"></Ibt:Imagebuttoncontrol>
                                </td>
                            </tr>
                        </table>
                    </ItemTemplate>
                </asp:Repeater>
            </div>
            <div id="SubProductAttributes" style="display: none; padding: 10px 0 10px 0;">
                <table style="width: 100%;">
                    <tr>
                        <td style="width: 60px;">光度：</td>
                        <td style="border-bottom: 1px solid #D7D7D7;">
                            <asp:HiddenField ID="Hid_LightParentFieldId" runat="server" />
                            <asp:CheckBoxList ID="ckb_LightFieldList" runat="server" RepeatColumns="12" RepeatDirection="Horizontal"></asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr style="border-bottom: 1px solid #D7D7D7;">
                        <td>散光：</td>
                        <td style="border-bottom: 1px solid #D7D7D7;">
                            <asp:HiddenField ID="Hid_AstigmatismParentFieldId" runat="server" />
                            <asp:CheckBoxList ID="ckb_AstigmatismFieldList" runat="server" RepeatColumns="12" RepeatDirection="Horizontal"></asp:CheckBoxList>
                        </td>
                    </tr>
                    <tr style="border-bottom: 1px solid #D7D7D7;">
                        <td>轴位：</td>
                        <td style="border-bottom: 1px solid #D7D7D7;">
                            <asp:HiddenField ID="Hid_AxialParentFieldId" runat="server" />
                            <asp:CheckBoxList ID="ckb_AxialViewFieldList" runat="server" RepeatColumns="12" RepeatDirection="Horizontal"></asp:CheckBoxList>
                        </td>
                    </tr>
                </table>
            </div>
            <div id="Medicament" style="display: none; padding: 10px 0 10px 0;">
                <table style="width: 100%;">
                    <tr>
                        <td style="text-align: right;">商品化学名：</td>
                        <td>
                            <asp:TextBox ID="txt_ChemistryName" runat="server"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">商品化学名首字母：</td>
                        <td>
                            <asp:TextBox ID="txt_ChemistryNameFirstLetter" runat="server"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">质检分类：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineQualityType" runat="server">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">销售品种：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineSaleKindType" runat="server">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">批发价：</td>
                        <td>
                            <asp:TextBox ID="txt_MedicineWholesalePrice" runat="server" onblur="check(this,'Decimal');"></asp:TextBox>
                        </td>
                        <td style="text-align: right;">税率：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineTaxRateType" runat="server">
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
                            <asp:DropDownList ID="ddl_MedicineDosageFormType" runat="server">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">储存条件：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineStorageConditionType" runat="server">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">养护类别：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineCuringKindType" runat="server">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="重点养护" Value="0"></asp:ListItem>
                                <asp:ListItem Text="一般养护" Value="1"></asp:ListItem>
                                <asp:ListItem Text="阴凉养护" Value="2"></asp:ListItem>
                                <asp:ListItem Text="冷藏养护" Value="3"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                        <td style="text-align: right;">养护周期：</td>
                        <td>
                            <asp:DropDownList ID="ddl_MedicineCuringCycleType" runat="server">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                                <asp:ListItem Text="每周" Value="0"></asp:ListItem>
                                <asp:ListItem Text="每月" Value="1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">门店柜台：</td>
                        <td colspan="3">
                            <asp:DropDownList ID="ddl_MedicineStoreCounterType" runat="server">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>

                    </tr>
                    <tr>
                        <td style="text-align: right;">库位管理</td>
                        <td>
                            <asp:DropDownList ID="ddl_LibraryManageType" runat="server">
                                <asp:ListItem Text="请选择" Value="-1"></asp:ListItem>
                            </asp:DropDownList>
                        </td>
                    </tr>
                    <tr>
                        <td style="text-align: right;">质量标准：</td>
                        <td colspan="3">
                            <asp:TextBox ID="txt_QualityStandardDescription" runat="server" TextMode="MultiLine" Width="99%"></asp:TextBox>
                        </td>
                    </tr>
                </table>
            </div>
        </div>
        <div style="text-align: center; padding-top: 10px;">
            <asp:Button ID="btn_Save" runat="server" Text="保存" OnClientClick="return CheckEmpty();" OnClick="btn_Save_Click" />
            <asp:Button ID="btn_SaveAdd" runat="server" Text="保存并添加" OnClientClick="return CheckEmpty();" OnClick="btn_SaveAdd_Click" />
            <asp:Button ID="btn_SaveCopyAdd" runat="server" Text="保存并复制添加" OnClientClick="return CheckEmpty();" OnClick="btn_SaveCopyAdd_Click" />
        </div>

        <script src="../JavaScript/telerik.js"></script>
        <script src="../JavaScript/jquery.js"></script>
        <script src="../My97DatePicker/WdatePicker.js"></script>
        <script src="../JavaScript/tool.js"></script>
        <script src="../JavaScript/PreviewImage.js"></script>
        <script src="../JavaScript/GiveTip.js"></script>

        <script type="text/javascript">
            var preA = null, preContainer = null;
            $(function () {
                preA = $(".tab a").get(0);
                preContainer = $("#BasicInfo");
                PreviewImage();//图片预览

                if ($("input[id$='Hid_FrameInfo']").val() === "1") {
                    $("#A_FrameInfo").css("display", "");
                }
                if ($("input[id$='Hid_Medicament']").val() === "1") {
                    $("#A_Medicament").css("display", "");
                    changeGoodsType('Add');
                    checkMedicament('Add');
                }
                if ($("input[id$='Hid_Other']").val() === "1") {
                    changeGoodsType('Add');
                }
                if ($("input[id$='Hid_SubProductAttributes']").val() === "1") {
                    $("#A_SubProductAttributes").css("display", "");
                }
                if ($("input[id$='Hid_GoodsQualification']").val() === "1") {
                    ChangeTab($("#A_GoodsQualification").get(0), 'GoodsQualification');
                }
            });

            function ChangeTab(obj, id) {
                var rcbGoodsType = jQuery.parseJSON($("input[id$='rcb_GoodsType_ClientState']").val());
                var ddlGoodsClass = $("select[id$='ddl_GoodsClass']");
                if (rcbGoodsType.value.length !== 0 && ddlGoodsClass.val().length !== 0) {
                    if (preA != null && preContainer != null) {
                        preA.className = "";
                        preContainer.css("display", "none");
                    }
                    preA = obj;
                    preContainer = $("#" + id);
                    obj.className = "now";
                    preContainer.css("display", "");
                    obj.blur();

                    if (id !== "GoodsQualification") {
                        $("input[id$='Hid_GoodsQualification']").val("");
                    }
                } else {
                    alert("请选择“商品类型”和“商品分类”!");
                }
            }

            //关联公司采购组
            function checkPurchasingGroup(obj) {
                var containerId = $(obj).attr("id");
                var ckbAll, ckbIsDirect, ckbIsJoin, ckbIsAlliance;
                if (containerId.indexOf("ckb_All") > -1) {
                    ckbIsDirect = containerId.replace("ckb_All", "ckb_IsDirect");
                    ckbIsJoin = containerId.replace("ckb_All", "ckb_IsJoin");
                    ckbIsAlliance = containerId.replace("ckb_All", "ckb_IsAlliance");
                    if ($(obj).is(":checked")) {
                        $("#" + ckbIsDirect).prop("checked", true);
                        $("#" + ckbIsJoin).prop("checked", true);
                        $("#" + ckbIsAlliance).prop("checked", true);
                    } else {
                        $("#" + ckbIsDirect).prop("checked", false);
                        $("#" + ckbIsJoin).prop("checked", false);
                        $("#" + ckbIsAlliance).prop("checked", false);
                    }
                } else if (containerId.indexOf("ckb_IsDirect") > -1) {
                    ckbAll = containerId.replace("ckb_IsDirect", "ckb_All");
                    ckbIsJoin = containerId.replace("ckb_IsDirect", "ckb_IsJoin");
                    ckbIsAlliance = containerId.replace("ckb_IsDirect", "ckb_IsAlliance");
                    if ($(obj).is(":checked") && $("#" + ckbIsJoin).is(":checked") && $("#" + ckbIsAlliance).is(":checked")) {
                        $("#" + ckbAll).prop("checked", true);
                    } else {
                        $("#" + ckbAll).prop("checked", false);
                    }
                }
                else if (containerId.indexOf("ckb_IsJoin") > -1) {
                    ckbAll = containerId.replace("ckb_IsJoin", "ckb_All");
                    ckbIsDirect = containerId.replace("ckb_IsJoin", "ckb_IsDirect");
                    ckbIsAlliance = containerId.replace("ckb_IsJoin", "ckb_IsAlliance");
                    if ($(obj).is(":checked") && $("#" + ckbIsDirect).is(":checked") && $("#" + ckbIsAlliance).is(":checked")) {
                        $("#" + ckbAll).prop("checked", true);
                    } else {
                        $("#" + ckbAll).prop("checked", false);
                    }
                }
                else if (containerId.indexOf("ckb_IsAlliance") > -1) {
                    ckbAll = containerId.replace("ckb_IsAlliance", "ckb_All");
                    ckbIsDirect = containerId.replace("ckb_IsAlliance", "ckb_IsDirect");
                    ckbIsJoin = containerId.replace("ckb_IsAlliance", "ckb_IsJoin");
                    if ($(obj).is(":checked") && $("#" + ckbIsDirect).is(":checked") && $("#" + ckbIsJoin).is(":checked")) {
                        $("#" + ckbAll).prop("checked", true);
                    } else {
                        $("#" + ckbAll).prop("checked", false);
                    }
                }
            }

            //商品类型相关数据
            function changeGoodsType(type) {
                var txtPurchaseNameFirstLetter = $("input[id$='txt_PurchaseNameFirstLetter']");
                var txtSpecification = $("input[id$='txt_Specification']");
                var txtShelfLife = $("input[id$='txt_ShelfLife']");
                if (type === "Add") {
                    txtPurchaseNameFirstLetter.addClass("Check");
                    txtSpecification.addClass("Check");
                    txtShelfLife.addClass("Check");
                } else {
                    txtPurchaseNameFirstLetter.removeClass("Check");
                    txtSpecification.removeClass("Check");
                    txtShelfLife.removeClass("Check");

                    $("#A_FrameInfo").css("display", "none");
                    $("#A_Medicament").css("display", "none");
                    $("input[id$='Hid_FrameInfo']").val("");
                    $("input[id$='Hid_Medicament']").val("");
                }
            }

            //添加/取消 药品验证
            function checkMedicament(type) {
                var txtChemistryName = $("input[id$='txt_ChemistryName']");
                var txtChemistryNameFirstLetter = $("input[id$='txt_ChemistryNameFirstLetter']");
                var ddlMedicineQualityType = $("select[id$='ddl_MedicineQualityType']");
                var ddlMedicineSaleKindType = $("select[id$='ddl_MedicineSaleKindType']");
                var txtMedicineWholesalePrice = $("input[id$='txt_MedicineWholesalePrice']");
                var ddlMedicineTaxRateType = $("select[id$='ddl_MedicineTaxRateType']");
                var ddlMedicineDosageFormType = $("select[id$='ddl_MedicineDosageFormType']");
                var ddlMedicineStorageConditionType = $("select[id$='ddl_MedicineStorageConditionType']");
                var ddlMedicineCuringKindType = $("select[id$='ddl_MedicineCuringKindType']");
                var ddlMedicineCuringCycleType = $("select[id$='ddl_MedicineCuringCycleType']");
                var ddlMedicineStoreCounterType = $("select[id$='ddl_MedicineStoreCounterType']");

                if (type === "Add") {
                    txtChemistryName.addClass("CheckMedicamentInput");
                    txtChemistryNameFirstLetter.addClass("CheckMedicamentInput");
                    ddlMedicineQualityType.addClass("CheckMedicamentSelect");
                    ddlMedicineSaleKindType.addClass("CheckMedicamentSelect");
                    txtMedicineWholesalePrice.addClass("CheckMedicamentInput");
                    ddlMedicineTaxRateType.addClass("CheckMedicamentSelect");
                    ddlMedicineDosageFormType.addClass("CheckMedicamentSelect");
                    ddlMedicineStorageConditionType.addClass("CheckMedicamentSelect");
                    ddlMedicineCuringKindType.addClass("CheckMedicamentSelect");
                    ddlMedicineCuringCycleType.addClass("CheckMedicamentSelect");
                    ddlMedicineStoreCounterType.addClass("CheckMedicamentSelect");
                } else {
                    txtChemistryName.removeClass("CheckMedicamentInput");
                    txtChemistryNameFirstLetter.removeClass("CheckMedicamentInput");
                    ddlMedicineQualityType.removeClass("CheckMedicamentSelect");
                    ddlMedicineSaleKindType.removeClass("CheckMedicamentSelect");
                    txtMedicineWholesalePrice.removeClass("CheckMedicamentInput");
                    ddlMedicineTaxRateType.removeClass("CheckMedicamentSelect");
                    ddlMedicineDosageFormType.removeClass("CheckMedicamentSelect");
                    ddlMedicineStorageConditionType.removeClass("CheckMedicamentSelect");
                    ddlMedicineCuringKindType.removeClass("CheckMedicamentSelect");
                    ddlMedicineCuringCycleType.removeClass("CheckMedicamentSelect");
                    ddlMedicineStoreCounterType.removeClass("CheckMedicamentSelect");
                }
            }

            //验证类型
            function check(obj, type) {
                if ($.checkType(type).test($(obj).val())) {
                } else {
                    $(obj).val("");
                    $(obj).attr("placeholder", castErrorMessage(type));
                }
            }

            //验证
            function CheckEmpty() {
                $("span[class='error']").remove();//移除所有错误提示

                $(".Check").each(function () {
                    var obj = $(this);
                    if (obj.val().length === 0) {
                        if (obj.next("span[class='error']").length === 0) {
                            obj.after("<span class='error' style='color:red;'>*</span>");
                        }
                    } else {
                        obj.next("span[class='error']").remove();
                    }
                });

                //处方药、非处方药
                var rcbGoodsType = jQuery.parseJSON($("input[id$='rcb_GoodsType_ClientState']").val());
                if (rcbGoodsType.value === "12" || rcbGoodsType.value === "17") {
                    $(".CheckMedicamentSelect").each(function () {
                        var obj = $(this);
                        if (obj.val() === "-1") {
                            if (obj.next("span[class='error']").length === 0) {
                                obj.after("<span class='error' style='color:red;'>*</span>");
                            }
                        } else {
                            obj.next("span[class='error']").remove();
                        }
                    });

                    $(".CheckMedicamentInput").each(function () {
                        var obj = $(this);
                        if (obj.val().length === 0) {
                            if (obj.next("span[class='error']").length === 0) {
                                obj.after("<span class='error' style='color:red;'>*</span>");
                            }
                        } else {
                            obj.next("span[class='error']").remove();
                        }
                    });
                    //质量标准
                    CheckQualityStandardDescription();
                }

                if ($("span[class='error']").length === 0) {
                    return true;
                } else {
                    alert("请完善必填信息！");
                    return false;
                }
            }

            //质量标准
            function CheckQualityStandardDescription() {
                var txtQualityStandardDescription = $("#txt_QualityStandardDescription");
                if (txtQualityStandardDescription.val().length === 0 || !$.chineseLengthValid(txtQualityStandardDescription.val(), 0, 400)) {
                    if (txtQualityStandardDescription.next("span[class='error']").length === 0) {
                        txtQualityStandardDescription.after("<span class='error'></span>");
                        txtQualityStandardDescription.css("border-color", "red");
                    }
                } else {
                    txtQualityStandardDescription.next("span[class='error']").remove();
                    txtQualityStandardDescription.css("border-color", "");
                }
            }

            //图片预览
            function PreviewImage() {
                $("#UploadImg").uploadPreview({
                    Img: "imgPre", Callback: function () {
                        $("#UploadImgName").val($("#UploadImg").val());
                        $("#PreA").attr("href", $("#imgPre").attr("src"));
                    }
                });
            }

            //清除上传的图片信息
            function clearImg() {
                $("input[id$='UploadImgName']").val("");
                $("#UploadImg").val("");
                $("#PreA").attr("href", "javascript:void(0);");
            }

            //选择商品资质图片
            function SelectImgInfo(obj) {
                var uploadImgNameInfo = $(obj).prevAll("input[id$='UploadImgNameInfo'][type='text']");
                var uploadImgInfo = $(obj).prevAll("input[id$='UploadImgInfo'][type='file']");
                var preAInfo = $(obj).nextAll("a[id$='PreAInfo']");
                var imgPreInfo = $(obj).nextAll("img[id$='imgPreInfo']");

                uploadImgInfo.click();
                PreviewImageInfo(uploadImgNameInfo, uploadImgInfo, preAInfo, imgPreInfo);
            }

            //商品资质图片预览
            function PreviewImageInfo(uploadImgNameInfo, uploadImgInfo, preAInfo, imgPreInfo) {
                uploadImgInfo.uploadPreview({
                    Img: imgPreInfo.attr("id"), Callback: function () {
                        uploadImgNameInfo.val(uploadImgInfo.val());
                        preAInfo.attr("href", imgPreInfo.attr("src"));
                    }
                });
            }

            //清除上传的商品资质图片信息
            function clearImgInfo(obj) {
                $(obj).prevAll("input[id$='UploadImgNameInfo'][type='text']").val("");
                $(obj).prevAll("input[id$='UploadImgInfo'][type='file']").val("");
                $(obj).prevAll("a[id$='PreAInfo']").attr("href", "javascript:void(0);");
            }
        </script>
    </form>
</body>
</html>

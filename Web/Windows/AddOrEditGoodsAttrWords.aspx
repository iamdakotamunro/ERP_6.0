<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddOrEditGoodsAttrWords.aspx.cs" Inherits="ERP.UI.Web.Windows.AddOrEditGoodsAttrWords" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <table style="width: 100%;">
            <tr>
                <td style="text-align: right;">属性名称：</td>
                <td>
                    <asp:TextBox ID="txt_Word" runat="server" CssClass="Check" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">前台显示：</td>
                <td>
                    <asp:CheckBox ID="ckb_IsShow" runat="server" Checked="True" />
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">排序字段：</td>
                <td>
                    <asp:TextBox ID="txt_OrderIndex" runat="server" CssClass="Check" onblur="check(this,'Int');" MaxLength="5" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">比较类型：</td>
                <td>
                    <asp:DropDownList ID="ddl_CompareType" runat="server" onchange="changeCompareType(this)">
                        <asp:ListItem Text="等于" Value="0"></asp:ListItem>
                        <asp:ListItem Text="两者之间" Value="1"></asp:ListItem>
                        <asp:ListItem Text="小于等于" Value="2"></asp:ListItem>
                        <asp:ListItem Text="大于等于" Value="3"></asp:ListItem>
                    </asp:DropDownList>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">比较值：</td>
                <td>
                    <asp:TextBox ID="txt_WordValue" runat="server" CssClass="Check" MaxLength="20" Width="300px"></asp:TextBox>
                </td>
            </tr>
            <tr id="topValue" style="display: none;">
                <td style="text-align: right;">比较上限：</td>
                <td>
                    <asp:TextBox ID="txt_TopValue" runat="server" MaxLength="20"></asp:TextBox>
                </td>
            </tr>
            <tr id="attrWordImage" runat="server">
                <td style="text-align: right;">上传图片：</td>
                <td>
                    <asp:TextBox ID="UploadImgName" runat="server" onfocus="this.blur();"></asp:TextBox>
                    <asp:FileUpload ID="UploadImg" runat="server" Style="display: none;" />
                    <input id="btnUploadImg" type="button" value="选择文件" title="选择图片!" onclick="UploadImg.click()" />
                    <a id="PreA" runat="server" href="javascript:void(0);" target="_blank"><b style="color: red;">预览</b></a>
                    <img id="imgPre" style="display: none;" />
                    <asp:HiddenField ID="Hid_AttrWordImage" runat="server" />
                    <a href="javascript:void(0);" onclick="clearImg()"><b style="color: red;">删除</b></a>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="text-align: center;">
                    <asp:Button ID="btn_Save" runat="server" Text="保存" OnClientClick="return CheckEmpty();" OnClick="btn_Save_Click" />
                </td>
            </tr>
        </table>

        <script src="../JavaScript/jquery.js"></script>
        <script src="../JavaScript/PreviewImage.js"></script>
        <script src="../JavaScript/tool.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script type="text/javascript">
            $(function () {
                PreviewImage();//图片预览
                $("select[id$='ddl_CompareType']").change();
            });

            //比较类型
            function changeCompareType(obj) {
                var txtTopValue = $("#topValue input");
                if ($(obj).val() === "1") {
                    $("#topValue").css("display", "");
                    txtTopValue.addClass("Check");
                } else {
                    $("#topValue").css("display", "none");
                    txtTopValue.removeClass("Check");
                    txtTopValue.val("");
                }
            }

            //清除上传的图片信息
            function clearImg() {
                $("input[id$='UploadImgName']").val("");
                $("#UploadImg").val("");
                $("#PreA").attr("href", "javascript:void(0);");
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

                if ($("span[class='error']").length === 0) {
                    return true;
                } else {
                    return false;
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
        </script>
    </form>
</body>
</html>

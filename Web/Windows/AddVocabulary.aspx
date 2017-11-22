<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddVocabulary.aspx.cs" Inherits="ERP.UI.Web.Windows.AddVocabulary" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div style="padding-top:10px;">
            Excel文档：<asp:TextBox ID="UploadExcelName" runat="server" onfocus="this.blur();"></asp:TextBox>
            <asp:FileUpload ID="UploadExcel" runat="server" Style="display: none;" onchange="CheckFile()" />
            <input type="button" value="选择文件" title="文件格式(.xls)!" onclick="UploadExcel.click()" />
            <asp:Button ID="btn_Upload" runat="server" Text="导入" OnClick="btn_Upload_Click" />
            <a id="Template" runat="server" href="../App_Themes/违禁词模板.xls" style="color: red; font-weight: bold;">模板下载</a>
        </div>
        <div>
            <asp:Literal ID="Lit_errorMsg" runat="server"></asp:Literal>
        </div>
        <script src="../JavaScript/telerik.js"></script>
        <script type="text/javascript">
            //验证文件格式
            function CheckFile() {
                var filePath = document.getElementById("UploadExcel").value;
                var ext = filePath.substr(filePath.length - 4, 4).toLowerCase();
                if (ext !== ".xls") {
                    alert("请选择格式为“.xls”文件！");
                    document.getElementById("UploadExcelName").value = "";
                } else {
                    document.getElementById("UploadExcelName").value = filePath;
                }
            }
        </script>
    </form>
</body>
</html>

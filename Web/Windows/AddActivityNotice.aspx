<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddActivityNotice.aspx.cs" Inherits="ERP.UI.Web.Windows.AddActivityNotice" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>添加加盟店公告</title>
        <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <style>
        td {
            padding: 5px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
         <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <div>
    <table width="100%" style="padding: 10px">
        <tr>
            <td width="100px">文章标题：</td>
            <td colspan="3"><asp:TextBox ID="TB_Title" runat="server" Width="300px"></asp:TextBox></td>
        </tr>
         <tr>
            <td>是否显示：</td>
            <td><asp:CheckBox ID="CB_IsShow" runat="server" /></td>
            <td width="100px">是否公告：</td>
            <td><asp:CheckBox ID="CB_IsNotice" runat="server" /></td>
        </tr>
         <tr>
               <td colspan="4">
                    <rad:RadEditor ID="Editor_NoticeContent" runat="server" SkinID="ShopWebSite">
                </rad:RadEditor>
               </td>
        </tr>
         <tr>
              <td>&nbsp;</td>
               <td>&nbsp;</td>
            <td>
                <asp:Button ID="btnAdd" runat="server" Text="确定" onclick="BtnAdd_Click" />
                <asp:Button ID="btnUpdate" runat="server" Text="修改" onclick="BtnUpdate_Click" />
            </td>
            <td>&nbsp;</td>
        </tr>
    </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server">
    </rad:RadAjaxManager>
    </form>
</body>
</html>

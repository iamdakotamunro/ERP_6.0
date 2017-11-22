<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddExcelTemplatePageForm.aspx.cs" Inherits="ERP.UI.Web.Windows.AddExcelTemplatePageForm" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style>
        .tdalign {
            text-align: right;
        }
    </style>
    <rad:RadScriptBlock ID="RSB" runat="server">
         <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <script type="text/javascript">
        function Check() {
            if ($("#txt_TemplateName").val() == "") {
                alert("模板名称不能为空！");
                return false;
            }
            if ($("#txt_Customer").val() == "") {
                alert("订货单位不能为空！");
                return false;
            }
            if ($("#txt_Shipper").val() == "") {
                alert("收货人及联系方式不能为空！");
                return false;
            }
            if ($("#txt_ContactPerson").val() == "") {
                alert("联系人及电话不能为空！");
                return false;
            }
            if ($("#txt_ContactAddress").val() == "") {
                alert("联系地址不能为空！");
                return false;
            }
            return true;
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <div>
        <table width="100%" style="line-height: 30px;">
            <tr>
                <td class="tdalign">仓库：</td>
                <td colspan="3">
                    <rad:RadComboBox ID="RCB_Stock" runat="server" Width="173px" >
                            </rad:RadComboBox >
                </td>
            </tr>
            <tr>
                <td class="tdalign">模板名称：</td>
                <td>
                    <asp:TextBox ID="txt_TemplateName" runat="server"></asp:TextBox></td>
                <td class="tdalign">订货单位：</td>
                <td>
                    <asp:TextBox ID="txt_Customer" runat="server"></asp:TextBox></td>
            </tr>
             <tr>
                <td class="tdalign">收货人及联系方式：</td>
                <td colspan="3"> <asp:TextBox ID="txt_Shipper" runat="server" Width="457px"></asp:TextBox></td>
            </tr>
              <tr>
                <td class="tdalign">联系人及电话：</td>
                <td colspan="3"> <asp:TextBox ID="txt_ContactPerson" runat="server" Width="457px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="tdalign">联系地址：</td>
                <td colspan="3"> <asp:TextBox ID="txt_ContactAddress" runat="server" Width="457px"></asp:TextBox></td>
            </tr>
            <tr>
                <td class="tdalign">备注：</td>
                <td colspan="3"> <asp:TextBox ID="txt_Remarks" runat="server" Width="457px"></asp:TextBox></td>
            </tr>
           <tr >
               <td colspan="4" style="text-align: center;">
                   <asp:Button ID="btn_Save" runat="server" Text="保存" Width="80" OnClientClick="return Check();" OnClick="btn_Save_Click"/>&nbsp;&nbsp;&nbsp;
                   <asp:Button ID="btn_qx" runat="server" Text="取消" Width="80" OnClientClick="CancelWindow(); return false;" />
               </td>
           </tr>
        </table>
    </div>
         <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            
        </AjaxSettings>
    </rad:RadAjaxManager>
    </form>
</body>
</html>

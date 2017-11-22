<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_Control.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_Control" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <table>
            <tr>
                <td style="width: 84px; text-align: right;">申报类型：</td>
                <td>
                    <asp:RadioButtonList ID="rbl_ReportKind" runat="server" RepeatDirection="Horizontal" onchange="ChangeReportKind();">
                        <asp:ListItem Text="预借款" Value="Before" Selected="True"></asp:ListItem>
                        <asp:ListItem Text="凭证报销" Value="Later"></asp:ListItem>
                        <asp:ListItem Text="已扣款核销" Value="Paying"></asp:ListItem>
                        <asp:ListItem Text="费用收入" Value="FeeIncome"></asp:ListItem>
                    </asp:RadioButtonList>
                </td>
            </tr>
        </table>
        
        <div>
            <iframe id="Iframe" allowtransparency="true" frameborder="0" scrolling="auto" src="../loading.htm"
                width="100%"></iframe>
        </div>
        <script src="../JavaScript/jquery.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script type="text/javascript">
            $(function () {
                $("#Iframe").height($(document).height() - 80);
                ChangeReportKind();
            });

            //申报类型
            function ChangeReportKind() {
                var url = '';
                if ('<%=ReportId%>' == "") {//添加
                    url = "CostReport_" + $("input[type='radio'][name='rbl_ReportKind']:checked").val() + ".aspx";
                } else {//修改
                    url = "CostReport_<%=ReportKind%>.aspx?ReportId=<%=ReportId%>";
                }
                $('#Iframe').attr('src', url);
            }
        </script>
    </form>
</body>
</html>

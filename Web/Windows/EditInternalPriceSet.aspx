<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditInternalPriceSet.aspx.cs" Inherits="ERP.UI.Web.Windows.EditInternalPriceSet" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/jquery.js"></script>
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
             <script src="../JavaScript/common.js" type="text/javascript"></script>
            <script type="text/javascript">
                function btnSubmit() {
                    var list = "";
                    var goodsType = "";
                    $("#PriceSet input[type=text]").each(function (i, input) {
                        list += $(this).attr("id") + '=' + $(this).val() + '&';
                    });
                    $("#PriceSet input[type=hidden]").each(function (i, input) {
                        goodsType = $(this).val();
                    });
                    $.ajax({
                        type: 'post',
                        url: 'Handle.aspx?goodsType=' + goodsType + '&' + list,
                        async: false,
                        success: function (result) {
                            alert("更新成功!");
                            CloseAndRebind();
                        },
                        error: function (result) {
                            alert(result.responseText.match(/<title>([^</title>]*)/)[1]);
                        }
                    });
                }

                function btnCancel(parameters) {
                    CancelWindow();
                }
            </script>
        </rad:RadScriptBlock>
        <br>
        <div id="PriceSet" runat="server">
        </div>
        <rad:RadAjaxManager ID="RAM" runat="server">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="btn_add">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="Lbl_errMsg" LoadingPanelID="loading" />
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

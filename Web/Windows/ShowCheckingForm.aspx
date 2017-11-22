<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.ShowCheckingForm" CodeBehind="ShowCheckingForm.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>对账</title>
    <style type="text/css">
        .textbody
        {
            margin: 0px;
            padding: 0px;
            font-size: 12px;
        }
    </style>
</head>
<body>
    <form id="form2" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
        <script type="text/javascript" src="../JavaScript/common.js"></script>
        <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    </rad:RadScriptBlock>

    <asp:HiddenField ID="hfTime" runat="server" />
    <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0" Width="100%">
        <asp:TableRow>
            <asp:TableCell CssClass="ControlTools">
                <asp:LinkButton ID="lbChecking" runat="server" OnClientClick="showmask()" OnClick="LbCheckingClick">
                    <asp:Image ID="Image1" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />对账
                </asp:LinkButton>
                <asp:Label ID="Label_InsterSpace" runat="server" Text="Label">&nbsp;</asp:Label>
                <asp:LinkButton ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()">
                    <asp:Image ID="IB_Cancel" SkinID="CancelImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />取消
                </asp:LinkButton>
            </asp:TableCell>
        </asp:TableRow>
    </asp:Table>
    <table class="PanelArea">
        <tr>
            <td class="PromptFromRowTitle">
                上传对账：
            </td>
            <td class="AreaEditFromRowInfo">
                <rad:RadUpload ID="upXLS" runat="server" ControlObjectsVisibility="None" Width="250px"
                    AllowedFileExtensions=".xls,xlsx,.csv" ReadOnlyFileInputs="True"/>
            </td>
        </tr>
        <tr>
           <td class="PromptFromRowTitle">
               对账类型：
           </td> 
           <td class="AreaEditFromRowInfo">
               <asp:RadioButton runat="server" ID="RdbtnCarriage" Text="快递运费账" GroupName="checkType"/>&nbsp;
               <asp:RadioButton runat="server" ID="RdbtndCollection"  Text="快递代收账" Checked="True" GroupName="checkType"/>  
           </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                列名模板：
            </td>
            <td class="AreaEditFromRowInfo">
                快递单号&nbsp;&nbsp;|&nbsp;&nbsp;实收货款&nbsp;&nbsp;|&nbsp;&nbsp;订单称重
            </td>
        </tr>
        <tr>
            <td class="PromptFromRowTitle">
                备注信息：
            </td>
            <td class="AreaEditFromRowInfo">
                <asp:TextBox ID="TB_Description" runat="server" TextMode="MultiLine" SkinID="LongTextarea"></asp:TextBox>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" UseEmbeddedScripts="false">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <%--这里不能用刷新，因为一用刷新，则上传文件无法找到,所以用js控制其Loading页面  comment by dyy at 2010.Jan.1st--%>
<%--     <rad:RadAjaxManager ID="RAM" runat="server"  UseEmbeddedScripts="false">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="lbChecking">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="TB_Description" LoadingPanelID="Loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>--%>

<%--    <rad:RadAjaxLoadingPanel ID="Loading" Skin="WebBlue" runat="server" ZIndex="10" BackgroundPosition="TopRight">
        <img id="imgLoading" src="~/App_Themes/default/Images/Loading.gif" runat="server" />
    </rad:RadAjaxLoadingPanel>--%>
    </form>
</body>
</html>

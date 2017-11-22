<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TempForm.aspx.cs" Inherits="ERP.UI.Web.Windows.TempForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
    <rad:RadScriptManager ID="radscr" runat="server"></rad:RadScriptManager>

    <div id="GoodsPanel" style="background-color: #FFFFFF; width: 300px; height: 300px;
        left: 0px; position: absolute; top: 0px;">
        <table width="100%">
            <tr>
                <td class="ShortFromRowTitle">
                    模版名称
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:textbox id="tbxName" width="200px" runat="server" skinid="ShortInput">
                    </asp:textbox>
                </td>
            </tr>
            <tr>
                <td class="ShortFromRowTitle">
                    订货单位
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:textbox id="tbxCompany" width="200px" runat="server" skinid="ShortInput">
                    </asp:textbox>
                </td>
            </tr>
            <tr>
                <td class="ShortFromRowTitle">
                    收货人
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:textbox id="tbxUsername" width="200px" textmode="MultiLine" height="40" runat="server"
                        skinid="ShortInput">
                    </asp:textbox>
                </td>
            </tr>
            <tr>
                <td class="ShortFromRowTitle">
                    订货联系人
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:textbox id="tbxUser" width="200px" textmode="MultiLine" runat="server" height="40"
                        skinid="ShortInput">
                    </asp:textbox>
                </td>
            </tr>
            <tr>
                <td class="ShortFromRowTitle">
                    收货地址
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:textbox id="tbxAddress" width="200px" textmode="MultiLine" runat="server" height="40"
                        skinid="ShortInput">
                    </asp:textbox>
                </td>
            </tr>
            <tr>
                <td class="ShortFromRowTitle">
                    备注
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:textbox id="tbxRemarks" width="200px" textmode="MultiLine" runat="server" height="40"
                        skinid="ShortInput">
                    </asp:textbox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:button id="Bt_Temp" runat="server" text="保存" cssclass="Button" causesvalidation="false"
                        onclick="Bt_Temp_Save"  />
                </td>
                <td >
                    <asp:button id="Bt_Cancel" runat="server" text="关闭" cssclass="Button" causesvalidation="false" onclientclick="CancelWindow()"/>
                </td>
            </tr>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server">
    </rad:RadAjaxManager>
    </form>
</body>
</html>

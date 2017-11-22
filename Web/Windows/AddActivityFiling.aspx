<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddActivityFiling.aspx.cs"
    Inherits="ERP.UI.Web.Windows.AddActivityFiling" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="/JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
    </rad:RadScriptBlock>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <div>
        
        <table style="width: 780px; margin-left: auto; margin-right: auto; height: 230px;">
            <tbody>
                <tr>
                    <td>
                        申报标题：
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="addActivityFiling_Title"></asp:TextBox>
                    </td>
                    <td>
                        起止时间
                    </td>
                    <td>
                        <rad:RadDatePicker runat="server" ID="addStartDateTime" EnableTyping="False" AutoPostBack="True" OnSelectedDateChanged="addStartDateTime_OnSelectedDateChanged">
                        </rad:RadDatePicker>
                        -
                        <rad:RadDatePicker runat="server" ID="addEndDateTime" EnableTyping="False" AutoPostBack="True" OnSelectedDateChanged="addEndDateTime_OnSelectedDateChanged">
                        </rad:RadDatePicker>
                    </td>
                </tr>
                <tr>
                    <td>
                        申报公司：
                    </td>
                    <td>
                        <rad:RadComboBox ID="AddRadSaleTerrace" DataValueField="ID" AccessKey="T" MarkFirstMatch="True"
                            DataTextField="Name" AutoPostBack="True" runat="server" Width="173" MaxHeight="200"
                            OnSelectedIndexChanged="Add_SaleTerraceOnSelectedIndexChanged">
                        </rad:RadComboBox>
                    </td>
                    <td>
                        申报平台：
                    </td>
                    <td>
                        <rad:RadComboBox ID="AddRadTerraceFiling" DataValueField="ID" AccessKey="T" MarkFirstMatch="True" AutoPostBack="True" DataTextField="Name"
                            runat="server" Width="173" MaxHeight="200" OnSelectedIndexChanged="AddRadTerraceFiling_OnSelectedIndexChanged">
                        </rad:RadComboBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        仓库：
                    </td>
                    <td>
                        <rad:RadComboBox runat="server" ID="AddRadWarehouse" AutoPostBack="True" OnSelectedIndexChanged="ActivityFiling_WarehouseSelect">
                        </rad:RadComboBox>
                    </td>
                    <td>
                        活动商品：
                    </td>
                    <td>
                        <rad:RadComboBox ID="AddRadGoods" runat="server"  Width="220px" EnableLoadOnDemand="True"
                            Height="200px" AllowCustomText="true"  DataTextField="GoodsName"
                            AutoPostBack="True" DataValueField="GoodsId"
                            OnSelectedIndexChanged="ActivityFiling_OnSelectedDateChanged" >
                        </rad:RadComboBox>
                        <asp:HiddenField runat="server" ID="HdGoodsID"/>
                    </td>
                </tr>
                <tr>
                    <td>
                        正常销量：
                    </td>
                    <td>
                        <div runat="server" ID="LoadDiv">
                        <asp:Label runat="server" ID="GoodsSaleLab" Text="0"></asp:Label>
                        <asp:Label runat="server" ID="DanWeiLab" Text="个"></asp:Label>
                        </div>
                    </td>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="预估销量"></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox ID="TxtProspectReadyNumber" runat="server" onKeyup="this.value=this.value.replace(/-?\D/g,'')"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" Text="采购员" ID="addPursePersonnel"></asp:Label>
                    </td>
                    <td>
                        <rad:RadComboBox runat="server" ID="addRadPursePersonnel" DataTextField="RealName"
                            DataValueField="PersonnelId" AccessKey="T" AllowCustomText="True" MarkFirstMatch="True"
                            Height="100">
                        </rad:RadComboBox>
                    </td>
                    <td>
                        <asp:Label runat="server" Text="预估备货" ID="addLadProspectSaleNumber" ></asp:Label>
                    </td>
                    <td>
                        <asp:TextBox runat="server" ID="addTxtProspectSaleNumber" onKeyup="this.value=this.value.replace(/-?\D/g,'')"></asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label runat="server" ID="addTxtActualSaleNumber" Text="实际销量"></asp:Label>
                    </td>
                    <td>
                        <asp:Label runat="server" ID="addLabActualSaleNumber"></asp:Label>
                    </td>
                </tr>
            </tbody>
            <tfoot>
                <tr>
                    <td>
                    </td>
                    <td>
                        <asp:Button runat="server" ID="BtnOK" Text="确定" OnClick="AddActivityFiling_Click" />
                    </td>
                    <td>
                        <asp:Button runat="server" ID="BtnCannel" Text="取消" OnClick="AddCannelActivityFiling_Click" />
                    </td>
                    <td>
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="AddRadSaleTerrace">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="AddRadTerraceFiling" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="AddRadGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="AddRadGoods" LoadingPanelID="loading"/>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="AddRadGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LoadDiv"/>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="AddRadWarehouse">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LoadDiv"/>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="AddRadTerraceFiling">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LoadDiv"/>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="AddRadSaleTerrace">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LoadDiv"/>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="addStartDateTime">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LoadDiv"/>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="addEndDateTime">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="LoadDiv"/>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

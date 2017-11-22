<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyPurchaseGroupForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.CompanyPurchaseGroupForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
        <script language="javascript" type="text/javascript">
            function doCondFocusGroupName(cond) {
                if (cond.value == "请填写分组名称") {
                    cond.value = "";
                }
            }
            function doCondBlurGroupName(cond) {
                if (cond.value == "") {
                    cond.value = "请填写分组名称";
                }
            }
        </script>
    </rad:RadScriptBlock>
    <div id="divOperator" runat="server">
        <table style="width: 100%;text-align: right;">
            <tr>
                <td>
                    <asp:Button ID="btnSave" runat="server" Text="保存" OnClick="BtnSave_OnClick" />
                    <asp:Button ID="btnCancel" runat="server" Text="取消" OnClientClick="return CancelWindow();" />
                </td>
            </tr>
        </table>
    </div>
    
    <hr />
    
    <table width="470" style="margin: 5px;">
        <tr>
            <td width="60">供应商：</td>
            <td width="130">
                 <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Width="155px" Height="220px"
                        AllowCustomText="true" EnableLoadOnDemand="True" DataValueField="CompanyId" DataTextField="CompanyName"
                        AppendDataBoundItems="true" OnItemsRequested="RCB_Company_OnItemsRequested" AutoPostBack="True" OnSelectedIndexChanged="RcbCompany_OnSelectedIndexChanged">
                       <Items>
                        <rad:RadComboBoxItem Value="00000000-0000-0000-0000-000000000000" Text="请选择" Selected="True" />
                    </Items>
                    </rad:RadComboBox>

              <%--  <rad:RadComboBox ID="RCB_Company" runat="server" AutoPostBack="True" OnSelectedIndexChanged="RcbCompany_OnSelectedIndexChanged" UseEmbeddedScripts="false"
                        MaxHeight="220px" DataValueField="CompanyId" DataTextField="CompanyName" AppendDataBoundItems="true">
                    <Items>
                        <rad:RadComboBoxItem Value="00000000-0000-0000-0000-000000000000" Text="请选择" Selected="True" />
                    </Items>
                </rad:RadComboBox>--%>
            </td>
            <td>
                <asp:TextBox ID="tbPurchaseGroupName" Text="请填写分组名称" onfocus="doCondFocusGroupName(this);" onblur="doCondBlurGroupName(this);" MaxLength="30" runat="server"></asp:TextBox>
            </td>
            <td>
                <asp:Button ID="btnAddGroup" Text="添加分组" OnClick="BtnAddGroup_OnClick" runat="server" />
            </td>
        </tr>
        <tr>
            <td colspan="4" style="padding-top: 5px;">
                <rad:RadGrid ID="RG_Group" OnNeedDataSource="RgGroup_NeedDataSource" OnItemCommand="RgGroup_ItemCommand" AllowPaging="false" 
                    Width="388" runat="server">
                    <ClientSettings>
                        <Selecting AllowRowSelect="False" EnableDragToSelectRows="False" />
                    </ClientSettings>
                    <MasterTableView CommandItemDisplay="None" ShowHeader="False" ShowFooter="False" DataKeyNames="CompanyId,PurchaseGroupId,OrderIndex">
                        <Columns>
                            <rad:GridTemplateColumn>
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                                <ItemStyle Width="120px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:TextBox ID="tbPurchaseGroupName" Text='<%# Eval("PurchaseGroupName") %>' Enabled='<%# int.Parse(Eval("OrderIndex").ToString())!=0 %>' MaxLength="20" runat="server"></asp:TextBox>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle Width="80px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:ImageButton ID="ibtnDelete" SkinID="DeleteImageButton" Visible='<%# int.Parse(Eval("OrderIndex").ToString())!=0 %>' OnClientClick="return confirm('确认删除吗？')" CommandName="Delete" runat="server"/>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>

    

    <rad:RadAjaxManager ID="RAM" runat="server">
        <ajaxsettings>
            <rad:AjaxSetting AjaxControlID="RCB_Company">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_Group" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnAddGroup">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="tbPurchaseGroupName" ></rad:AjaxUpdatedControl>
                    <rad:AjaxUpdatedControl ControlID="RG_Group" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RG_Group">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_Group" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btnSave">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="divOperator" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </ajaxsettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

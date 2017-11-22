<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PurchaseDistributionsForm.aspx.cs" Inherits="ERP.UI.Web.Windows.PurchaseDistributionsFrom" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
    <table class="PanelArea">
        <tr>
            <td class="ShortFromRowTitle" align="right" style="width:120px">责任人：</td>
            <td class="ShortFromRowTitle" align="right">
                <rad:RadComboBox ID="RCB_Persion" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                              AllowCustomText="True"   MarkFirstMatch="True" ShowToggleImage="True" DataTextField="RealName" DataValueField="PersonnelId"
                        Width="120px" Height="200px">
                    </rad:RadComboBox>
            </td>
            <td class="ShortFromRowTitle" align="right">供应商：</td>
            <td class="ShortFromRowTitle" align="right">
                <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Width="120px"
                        AllowCustomText="true" EnableLoadOnDemand="True" Height="120px" DataValueField="CompanyId"
                        DataTextField="CompanyName" AppendDataBoundItems="true" OnItemsRequested="RCB_Company_OnItemsRequested">
                        <Items>
                            <rad:RadComboBoxItem Value="00000000-0000-0000-0000-000000000000" Text="所有公司" Selected="True" />
                        </Items>
                    </rad:RadComboBox>
            </td>
            <td class="ShortFromRowTitle" align="right">所在仓库：</td>
            <td class="ShortFromRowTitle" align="right">
                <rad:RadComboBox ID="RCB_Warehouse" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        MarkFirstMatch="True" ShowToggleImage="True" DataTextField="WarehouseName" DataValueField="WarehouseId"
                        Width="120px" Height="200px">
                    </rad:RadComboBox>
            </td>
            <td>
                <asp:Button ID="Btn_Search" runat="server"  OnClick="Btn_Search_OnClick" Text="搜索" />
            </td>
        </tr>
        <tr>
            <td class="ShortFromRowTitle" align="right" style="width:120px">
                转移商品至责任人：
            </td>
            <td class="ShortFromRowTitle" align="right">
                <rad:RadComboBox ID="RCB_TargetPersion" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                              AllowCustomText="True"   MarkFirstMatch="True" ShowToggleImage="True" DataTextField="RealName" DataValueField="PersonnelId"
                        Width="120px" Height="200px">
                    </rad:RadComboBox>
            </td>
            
            <td class="ShortFromRowTitle" align="right" style="width:120px">
                转移商品至供应商：
            </td>
            <td class="ShortFromRowTitle" align="right">
                <rad:RadComboBox ID="RCB_TargetCompany" runat="server" UseEmbeddedScripts="false" Width="120px"
                        AllowCustomText="true" EnableLoadOnDemand="True" Height="120px" DataValueField="CompanyId"
                        DataTextField="CompanyName" AppendDataBoundItems="true" OnItemsRequested="RCB_Company_OnItemsRequested">
                        <Items>
                            <rad:RadComboBoxItem Value="00000000-0000-0000-0000-000000000000" Text="" Selected="True" />
                        </Items>
                    </rad:RadComboBox>
            </td>
            <td class="ShortFromRowTitle">
                <asp:Button ID="Btn_Save" runat="server"  OnClick="Btn_Save_OnClick" Text="勾选保存" OnClientClick="return confirm('确定勾选商品转移至所选责任人或供应商？')"/>
            </td>

            <td class="ShortFromRowTitle" >
                <asp:Button ID="Button1" runat="server"  OnClick="Btn_Save_OnClick_Company" Text="批量转移" OnClientClick="return confirm('确定批量转移至所选供应商？')" />
            </td>
            
        </tr>
        <tr>
              <td style="width: 850px;color: gray;size:12px;font-weight: bold;" colspan="4">(勾选保存 注：同时转移勾选商品至所选中责任人或供应商，无选择不转移)</td>
        </tr>
        <tr>
            <td style="width: 850px;color: gray;size:12px;font-weight: bold;" colspan="4">(批量转移按钮 注：搜索具体供应商下所有商品转移至所选的供应商，<span style="color: blue">只转移供应商</span>)</td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_PurchaseSet" runat="server" SkinID="Common_Foot" 
                OnNeedDataSource="RgPurchaseSet_OnNeedDataSource" AllowMultiRowSelection="True" Width="100%">
            <ClientSettings>
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="True" />
            </ClientSettings>
            <MasterTableView DataKeyNames="GoodsId" ClientDataKeyNames="GoodsId" Width="100%">
                <CommandItemTemplate>
                </CommandItemTemplate>
                <Columns>
                    <rad:GridClientSelectColumn UniqueName="column">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridClientSelectColumn>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="供应商">
                        <ItemTemplate>
                            <%# GetCompanyName(new Guid(Eval("CompanyId").ToString())) %>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    <rad:RadAjaxManager runat="server" ID="RAM">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingMembers" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_add">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingMembers" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Persion">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Persion" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Rgd_PurchasingMembers">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingMembers" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    </form>
</body>
</html>

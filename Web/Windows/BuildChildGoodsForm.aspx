<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="BuildChildGoodsForm.aspx.cs" Inherits="ERP.UI.Web.Windows.BuildChildGoods" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>子商品</title>
</head>
<body>
    <form id="form1" runat="server">
        <x:PageManager AjaxLoadingType="Mask" EnableAjax="true" runat="server" ID="PageManager1" />
        <x:HiddenField runat="server" ID="HF_LightFieldParentId"></x:HiddenField>
        <x:HiddenField runat="server" ID="HF_AstigmatismFieldParentId"></x:HiddenField>
        <x:HiddenField runat="server" ID="HF_AxialViewFieldParentId"></x:HiddenField>
        <x:Form ID="from_ChildGoods" Title="子商品列表" LabelWidth="50px" EnableAjax="true" EnableCollapse="true" runat="server">
            <Rows>
                <x:FormRow>
                    <Items>
                        <x:DropDownList Label="光度" ID="DDL_LightFields" Width="80px" runat="server"></x:DropDownList>
                        <x:DropDownList Label="散光" ID="DDL_AstigmatismFields" Width="80px" runat="server"></x:DropDownList>
                        <x:DropDownList Label="轴位" ID="DDL_AxialFields" Width="80px" runat="server"></x:DropDownList>
                        <x:Button ID="BT_SearchChildGoods" Text="搜索" OnClick="BT_SearchChildGoods_OnClick" EnableAjax="true" Icon="Accept" runat="server" />
                        <x:HiddenField ID="hfRealGoodsId" runat="server"></x:HiddenField>
                        <x:HiddenField ID="hfIsSelected" runat="server"></x:HiddenField>
                        <x:Button ID="BT_IsActive" OnClick="BT_IsActive_OnClick" EnablePostBack="False" CssStyle="display: none;" runat="server" />
                        <x:Button ID="BT_IsScarcity" OnClick="BT_IsScarcity_OnClick" EnablePostBack="False" CssStyle="display: none;" runat="server" />
                        <x:Button ID="BT_Disable" OnClick="BT_Disable_OnClick" EnablePostBack="False" CssStyle="display: none;" runat="server" />
                        <x:Button ID="BT_Cancel" OnClick="BT_Cancel_OnClick" EnablePostBack="False" CssStyle="display: none;" runat="server" />
                    </Items>
                </x:FormRow>
            </Rows>
            <Rows>
                <x:FormRow>
                    <Items>
                        <x:Grid ID="GD_ChildGoodsList" AllowCellEditing="true" ClicksToEdit="2" 
                                 DataKeyNames="RealGoodsId" EnableAfterEditEvent="true" 
                                 OnAfterEdit="GD_ChildGoodsList_OnAfterEdit" ShowHeader="False"
                                 AllowPaging="true" PageSize="20" IsDatabasePaging="true" 
                                 OnPageIndexChange="GridChildGoodsList_PageIndexChange" runat="server">
                            <Columns>
                                <x:BoundField ID="BoundField1" runat="server" DataField="Specification" Width="190px" TextAlign="Left" HeaderText="属性" />
                                <x:RenderField ID="RenderField1" runat="server" ColumnID="Column_Barcode" EnableAjax="true" DataField="Barcode" TextAlign="Center" HeaderText="条码">
                                    <Editor>
                                        <x:TextBox runat="server" ID="TB_Barcode"></x:TextBox>
                                    </Editor>
                                </x:RenderField>
                                <x:RenderCheckField ID="RenderCheckField3" runat="server" ColumnID="Column_Disable" DataField="Disable" TextAlign="Center"  HeaderText="前台禁用" />
                                <x:RenderCheckField ID="RenderCheckField1" runat="server" ColumnID="Column_IsScarcity" DataField="IsScarcity" TextAlign="Center" HeaderText="缺货" />
                                <x:RenderCheckField ID="RenderCheckField2" runat="server" ColumnID="Column_IsActive" DataField="IsActive" TextAlign="Center"  HeaderText="删除" />
                            </Columns>
                        </x:Grid>
                    </Items>
                </x:FormRow>
            </Rows>
        </x:Form>
        <br/>
        <x:Form ID="form2" LabelWidth="50px" EnableAjax="true" runat="server" EnableCollapse="true" Collapsed="False" Title="删除方式">
            <Rows>
                <x:FormRow>
                    <Items>
                        <x:RadioButtonList ID="RblDelete" Label="" runat="server">
                            <x:RadioItem Text="普通删除" Value="0" Selected="True"/>
                            <x:RadioItem Text="强制删除" Value="1" />
                        </x:RadioButtonList>
                    </Items>
                </x:FormRow>
            </Rows>
        </x:Form>
        <x:Form ID="form_Field" LabelWidth="50px" EnableAjax="true" runat="server" EnableCollapse="true" Title="属性列表" BodyPadding="10px" >
            <Rows>
                <x:FormRow>
                    <Items>
                        <x:CheckBoxList Label="光度" ID="CBL_LightFieldList" OnSelectedIndexChanged="CblField_SelectedIndexChanged" 
                        AutoPostBack="true"
                        CssStyle="border-bottom:solid 1px #ccc;padding-bottom:5px;"
                         ColumnNumber="10" DataTextField="FieldValue" DataValueField="FieldId" runat="server"></x:CheckBoxList>
                    </Items>
                </x:FormRow>
                <x:FormRow>
                    <Items>
                        <x:CheckBoxList Label="散光" ID="CBL_AstigmatismFieldList" OnSelectedIndexChanged="CblField_SelectedIndexChanged" AutoPostBack="true" CssStyle="border-bottom:solid 1px #ccc;padding-bottom:5px;" ColumnNumber="10" DataTextField="FieldValue" DataValueField="FieldId" runat="server"></x:CheckBoxList>
                    </Items>
                </x:FormRow>
                <x:FormRow>
                    <Items>
                        <x:CheckBoxList Label="轴位" ID="CBL_AxialViewFieldList" OnSelectedIndexChanged="CblField_SelectedIndexChanged" AutoPostBack="true" ColumnNumber="10" DataTextField="FieldValue" DataValueField="FieldId" runat="server"></x:CheckBoxList>
                    </Items>
                </x:FormRow>
            </Rows>
            <Rows>
                <x:FormRow>
                    <Items>
                        <x:Panel ID="Panel1" CssStyle="text-align: center;" runat="server" ShowBorder="false"
                            BodyPadding="10px" ShowHeader="false">
                            <Items>
                                <x:Button runat="server" EnableAjax="true" CssStyle="display: inline-block;*display: inline;margin-right: 10px;" Icon="Accept" Text="生成子商品" ConfirmText="你确定被选择的子商品属性需要更新？" ConfirmTitle="生成确认" ID="BT_BuildChildGoods" OnClick="BT_BuildChildGoods_OnClick" />
                                <%--<x:Button runat="server" EnableAjax="true" CssStyle="display: inline-block;*display: inline;margin-right: 10px;" Icon="Delete" Text="强删子商品" ConfirmText="你确定要强制删除被选择的子商品？" ConfirmTitle="删除确认" ID="BtDeleteConfirm"  />--%>
                            </Items>
                        </x:Panel>
                    </Items>
                </x:FormRow>
            </Rows>
        </x:Form>
        <br />
    </form>
</body>
</html>

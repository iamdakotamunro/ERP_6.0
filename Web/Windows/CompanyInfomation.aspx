﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyInfomation.aspx.cs" Inherits="ERP.UI.Web.Windows.CompanyInfomation" %>
<%@ Register TagPrefix="Ibt" TagName="ImageButtonControl" Src="~/UserControl/ImageButtonControl.ascx" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>供应商资质</title>
    <style type="text/css">
       .red {
            color:red;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <asp:ScriptManager ID="RSM" runat="server"></asp:ScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
    <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    <script type="text/javascript" src="../JavaScript/jquery.js"></script>
    <script type="text/javascript">
        function OnDeleteConfirm() {
            var conf = window.confirm('提示：是否确认删除？');
            if (!conf)
                return false;
            return true;
        }
    </script>
    </rad:RadScriptBlock>
    <div id="DIV4" runat="server">
        <table width="100%" >
            <tr style="background-color: #D8E5F4">
                <td colspan="6" style="font-size: 18px; font-weight: bold;">
                    <asp:Label ID="lb_supplierName" runat="server" ></asp:Label> 
                    <br />
                </td>
            </tr>

            <tr  style="background-color: #D8E5F4">
                <td colspan="5" style="font-size: 18px; font-weight: bold;">
                    供应商资质
                </td>
                <td colspan="1" style="text-align:right">
                    <asp:Button ID="BtnAddInformation" runat="server" Text="添加" OnClick="BtnAddInformationClick" />
                    <asp:Button ID="BtnSave" runat="server" Text="保存"  OnClick="BtnSaveClick" />
                </td>

            </tr>
        </table>
    </div>
        <asp:Panel ID="PanelInfomation" runat="server">
        <asp:Repeater ID="RpCompanyInformations" OnDataBinding="RpCompanyInformationsOnDataBinding" 
            OnItemDataBound="RpCompanyInformationsItemDataBound" OnItemCommand="RpCompanyInformationsOnItemCommand"
            runat="server">
            <ItemTemplate>
                <table width="100%">
                    <tr style="background-color: #D8E5F4">
                        <td colspan="8" >
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            <asp:Label ID="lb_RedSign" runat="server" CssClass="red" Visible="false">*</asp:Label>
                            单位资质：
                        </td>
                        <td>
                            <asp:DropDownList ID="DDL_SupplierQualification" OnSelectedIndexChanged="SupplierQualification_Change" AutoPostBack="true" runat="server">
                            </asp:DropDownList>
                        </td>
                        <td align="center" style="width: 60px;">
                            <asp:Label ID="lb_state" runat="server"></asp:Label>
                        </td>
                        <td style="text-align: right;">
                            证书号码：
                        </td>
                        <td>
                            <asp:TextBox ID="TbInformationNumber" Text='<%#Eval("Number") %>' runat="server" Width="150"></asp:TextBox>
                        </td>
                        <td>
                            单位资料：
                        </td>
                        <td colspan="2">
                            <rad:RadUpload ID="UploadInformation" runat="server" ControlObjectsVisibility="ClearButtons"
                                Width="330px" UseEmbeddedScripts="false" Language="zh-cn" ReadOnlyFileInputs="True" AutoPostBack="true" />
                        </td>
                    </tr>
                    <tr>
                        <td>
                            过期日期：
                        </td>
                        <td>
                            <rad:RadDatePicker ID="RDPOverdueDate" runat="server" SkinID="Common" Width="180px">
                            </rad:RadDatePicker>
                        </td>
                        <td align="left">
                            <asp:DropDownList ID="DDL_Filiale" runat="server"></asp:DropDownList>
                        </td>
                        <td colspan="3" style="text-align: center;">
                            <a href="<%# Eval("Path")==null?"javascript:void(0)":ResourceServerInformation+Eval("Path").ToString().Replace("~",string.Empty) %>" 
                                style="color:red;font-weight:bold" target="_blank"><%# Eval("Path") == null || Eval("Path").ToString()==string.Empty ? "" : "点击下载资料查阅"%></a>
                        </td>
                        <td colspan="2" align="right">
                            <Ibt:ImageButtonControl runat="server" SkinType="Delete" CommandName="DeleteInformation"
                                CausesValidation="false" ID="IbDeleteInformation" Text="删除" OnClientClick="return OnDeleteConfirm()"></Ibt:ImageButtonControl>
                        </td>
                    </tr>
                </table>
                <asp:HiddenField ID="HfId" Value='<%# Eval("Id") %>' runat="server" />
                <asp:HiddenField ID="HfSupplierQualification" Value='<%# Eval("QualificationType") %>' runat="server" />
                <asp:HiddenField ID="HfNumber" Value='<%# Eval("Number") %>' runat="server" />
                <asp:HiddenField ID="HfPath" Value='<%# Eval("Path") %>' runat="server" />
                <asp:HiddenField ID="HfOverdueDate" Value='<%# Eval("OverdueDate") %>' runat="server" />
                <asp:HiddenField ID="HfFilialeId" Value='<%# Eval("FilialeID") %>' runat="server" />
                <asp:HiddenField ID="HfIsNew" Value='<%# Eval("IsNew") %>' runat="server" />
            </ItemTemplate>
        </asp:Repeater>
    </asp:Panel>
    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

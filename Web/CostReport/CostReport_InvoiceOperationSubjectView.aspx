<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_InvoiceOperationSubjectView.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_InvoiceOperationSubjectView" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!--当前页面：费用发票——查看科目-->
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    
    <script src="/JavaScript/jquery.js" type="text/javascript"></script>
    <link href="../Styles/bootstrap.min.css" rel="stylesheet" />
    <script src="../JavaScript/bootstrap.min.js" type="text/javascript"></script>

    <script src="../My97DatePicker/WdatePicker.js"></script>
    <script src="../JavaScript/telerik.js"></script>
    <script src="../JavaScript/tool.js"></script>

    <style type="text/css">
        label {
            font-weight: normal;
        }

    </style>

    <script type="text/javascript">
        function onToolBarClientButtonClicking(sender, args) {
            var button = args.get_item();
            if (button.get_commandName() == "DeleteSelected") {
                args.set_cancel(!confirm('Delete all selected customers?'));
            }
        }

        //验证类型
        function check(obj, type) {
            if (!$.checkType(type).test($(obj).val())) {
                $(obj).val("");
                $(obj).attr("placeholder", castErrorMessage(type));
            }
        }
    </script>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager runat="server" ID="RadScriptManager1" />
        <rad:RadSkinManager ID="RadSkinManager1" runat="server" />
        <rad:RadAjaxManager ID="RadAjaxManager1" runat="server">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="RadGrid1">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RadGrid1"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Label1"></rad:AjaxUpdatedControl>
                        <rad:AjaxUpdatedControl ControlID="Label2"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>

        <br />
        <div class="panel panel-default">
            <div class="panel-heading">
                编辑科目
            </div>
            <div class="panel-body">

                <asp:Label ID="Label1" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="#FF8080"></asp:Label>
                <asp:Label ID="Label2" runat="server" EnableViewState="False" Font-Bold="True" ForeColor="#00C000"></asp:Label>
                <br />
                <rad:RadGrid ID="RadGrid1" SkinID="CustomPaging" OnNeedDataSource="RadGrid1_NeedDataSource" OnItemDataBound="RadGrid1_ItemDataBound" RenderMode="Lightweight" AllowAutomaticUpdates="true" AllowAutomaticDeletes="true"
                    Width="97%" AllowSorting="True" AutoGenerateColumns="false"
                    AllowPaging="True" GridLines="None" runat="server" ShowFooter="True" AllowMultiRowSelection="True"
                    PageSize="7" AllowMultiRowEdit="True" HorizontalAlign="NotSet" OnItemCreated="RadGrid1_ItemCreated"
                    OnItemDeleted="RadGrid1_ItemDeleted" OnItemUpdated="RadGrid1_ItemUpdated" OnItemInserted="RadGrid1_ItemInserted">

                    <MasterTableView Width="100%" GridLines="None" CommandItemDisplay="Top"
                        EditMode="InPlace" HorizontalAlign="NotSet" DataKeyNames="SubjectName" AllowAutomaticInserts="True">
                        <CommandItemTemplate>
                            <rad:RadToolBar RenderMode="Lightweight" ID="RadToolBar1" runat="server" OnClientButtonClicking="onToolBarClientButtonClicking"
                                AutoPostBack="true" >
                                <Items>
                                    <%--<rad:RadToolBarButton Text="编辑选中" CommandName="EditSelected" ImageUrl="~/icon/Edit.gif">
                                    </rad:RadToolBarButton>

                                    <rad:RadToolBarButton Text="编辑" CommandName="UpdateEdited" ImageUrl="~/icon/Update.gif">
                                    </rad:RadToolBarButton>

                                    <rad:RadToolBarButton Text="取消编辑" CommandName="CancelAll" ImageUrl="~/icon/Cancel.gif">
                                    </rad:RadToolBarButton>--%>

                                    <rad:RadToolBarButton Text="添加" CommandName="InitInsert" ImageUrl="~/icon/AddRecord.gif">
                                    </rad:RadToolBarButton>

                                    <%--<rad:RadToolBarButton Text="Add this Customer" CommandName="PerformInsert" ImageUrl="~/icon/Insert.gif">
                                    </rad:RadToolBarButton>

                                    <rad:RadToolBarButton Text="删除选中" CommandName="DeleteSelected"
                                        ImageUrl="~/icon/Delete.gif">
                                    </rad:RadToolBarButton>

                                    <rad:RadToolBarButton Text="刷新" CommandName="RebindGrid" ImageUrl="~/icon/Refresh.gif">
                                    </rad:RadToolBarButton>--%>
                                </Items>
                            </rad:RadToolBar>
                        </CommandItemTemplate>


                        <CommandItemStyle Height="0px" />

                        <Columns>

                            <rad:GridTemplateColumn HeaderText="科目" UniqueName="SubjectName">
                                <ItemTemplate>
                                    <%# Eval("SubjectName")%>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <FooterStyle HorizontalAlign="Right"></FooterStyle>
                            </rad:GridTemplateColumn>

                            <rad:GridTemplateColumn HeaderText="金额">
                                <ItemTemplate>
                                    
                                    <asp:TextBox ID="txt_ModifyPrice" runat="server" Font-Bold="true" onblur="check(this,'Decimal');" Text='<%# Eval("Amount")%>' Width="99%"></asp:TextBox>
                                </ItemTemplate>
                                <HeaderStyle Width="120" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>

                        </Columns>
                    </MasterTableView>

                </rad:RadGrid>
            </div>
        </div>


    </form>
</body>
</html>

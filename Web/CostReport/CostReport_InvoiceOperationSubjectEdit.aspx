<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_InvoiceOperationSubjectEdit.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_InvoiceOperationSubjectEdit" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <!--当前页面：费用发票——科目归类-->
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

        .lableCss {
            text-align: right;
            width: 19.5%;
            padding-left: 0;
            padding-top: 8px;
        }

        .txtReportMoney {
            width: 160px;
            float: left;
            margin-right: 20px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360">
        </rad:RadScriptManager>
        <rad:RadSkinManager ID="rsmSkin" runat="server" Skin="WebBlue">
        </rad:RadSkinManager>
        <br />

        <div class="form-group col-sm-13 has-feedback">

            <div class="panel panel-default">
                <div class="panel-heading">会计科目信息</div>
                <div class="panel-body">

                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-5 control-label" style="text-align: right;">单据编号：</label>
                        <div class="col-sm-7 control-label" style="text-align: left;">
                            <asp:Literal ID="Lit_Count" runat="server" Text="333"></asp:Literal>
                        </div>
                    </div>
                    <div class="form-group col-sm-6 has-feedback">
                        <label class="col-sm-5 control-label" style="text-align: right;">单据金额：</label>
                        <div class="col-sm-7 control-label" style="text-align: left;">
                            <asp:Literal ID="Lit_AmountTotal" runat="server" Text="2.22"></asp:Literal>
                        </div>
                    </div>
                </div>

            </div>

            <div class="panel panel-default">
                <div class="panel-heading">
                    编辑科目
                </div>
                <div class="panel-body">

                    <rad:RadGrid ID="RadGrid1" runat="server" ShowFooter="true" SkinID="CustomPaging" OnNeedDataSource="RadGrid1_NeedDataSource" OnItemDataBound="RadGrid1_ItemDataBound" AllowPaging="true" AllowAutomaticInserts="true" AllowMultiRowEdit="false"
                        OnInsertCommand="RadGrid1_InsertCommand"
                        OnUpdateCommand="RadGrid1_UpdateCommand"
                        OnDeleteCommand="RadGrid1_DeleteCommand"
                        OnItemCommand="RadGrid1_ItemCommand">
                        <MasterTableView DataKeyNames="SubjectID" EditMode="InPlace">
                            <CommandItemTemplate>

                                <rad:RadToolBar RenderMode="Lightweight" ID="RadToolBar1" runat="server" OnClientButtonClicking="onToolBarClientButtonClicking"
                                    AutoPostBack="true">
                                    <Items>
                                        <rad:RadToolBarButton Text="添加" CommandName="InitInsert" ImageUrl="~/icon/add.png">
                                        </rad:RadToolBarButton>

                                    </Items>
                                </rad:RadToolBar>

                            </CommandItemTemplate>

                            <CommandItemStyle Height="0px" />
                            <Columns>

                                <rad:GridTemplateColumn UniqueName="SubjectName" HeaderText="科目" SortExpression="SubjectName"
                                    ItemStyle-Width="400px">
                                    <ItemTemplate>
                                        <%#DataBinder.Eval(Container.DataItem, "SubjectName")%>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <!-- Show employees in the treeview -->
                                        <rad:RadComboBox RenderMode="Lightweight" ID="RadComboBox1" runat="server" Width="400px"
                                            ShowToggleImage="True" Style="vertical-align: middle;" OnClientDropDownOpened="OnClientDropDownOpenedHandler"
                                            ExpandAnimation-Type="None" CollapseAnimation-Type="None" 
                                            DataTextField="SubjectName" DataValueField="SubjectID" DataFieldID="SubjectID" >
                                            <ItemTemplate>
                                                <div id="div1" onclick="StopPropagation(event);">
                                                    <input type="button" value="展开"/>
                                                    <input type="button" value="收缩"/>
                                                    <br />
                                                    <rad:RadTreeView RenderMode="Lightweight" runat="server" ID="RadTreeView1" 
                                                        OnClientNodeClicking="nodeClicking" OnClientNodeEditStart="nodeEditStart"
                                                        Width="400px" Height="150px" DataTextField="SubjectName" DataValueField="SubjectID" DataFieldID="SubjectID"
                                                        DataFieldParentID="SubjectID" OnDataBound="RadTreeView1_DataBound" />
                                                </div>
                                            </ItemTemplate>
                                            <Items>
                                                <rad:RadComboBoxItem Text="" />
                                            </Items>
                                        </rad:RadComboBox>
                                    </EditItemTemplate>
                                </rad:GridTemplateColumn>

                                <rad:GridTemplateColumn HeaderText="科目编号" UniqueName="SubjectID" Visible="false">
                                    <ItemTemplate>
                                        <asp:Label ID="Lab_SubjectID" runat="server" Text='<%# Eval("SubjectID") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="Txt_SubjectID" runat="server" Text='<%# Eval("SubjectID") %>'></asp:TextBox>
                                    </EditItemTemplate>

                                    <HeaderStyle Width="120" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridTemplateColumn>


                                <rad:GridTemplateColumn HeaderText="金额">
                                    <ItemTemplate>
                                        <asp:Label ID="Lab_Amount" runat="server" Text='<%# Eval("Amount") %>'></asp:Label>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox ID="Txt_Amount" runat="server" onblur="check(this,'Decimal');" Text='<%# Eval("Amount") %>' MaxLength="10"></asp:TextBox>
                                    </EditItemTemplate>

                                    <HeaderStyle Width="100" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridTemplateColumn>


                                <rad:GridEditCommandColumn ButtonType="ImageButton" CancelText="取消" EditText="编辑" UpdateText="更新" InsertText="添加" HeaderText="编辑">
                                    <HeaderStyle Width="80" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridEditCommandColumn>

                                <rad:GridButtonColumn ButtonType="ImageButton" CommandName="Delete" Text="删除" ConfirmText="确实要删除吗？" UniqueName="Delete" HeaderText="删除">
                                    <HeaderStyle Width="50" HorizontalAlign="Center" />
                                    <ItemStyle HorizontalAlign="Center" />
                                </rad:GridButtonColumn>

                            </Columns>
                        </MasterTableView>

                        <ClientSettings>
                            <ClientEvents OnRowDblClick="RowDblClick" />
                        </ClientSettings>
                    </rad:RadGrid>
                </div>


            </div>


            <div class="popBtns">
                <div class="form-group">
                    <div class="col-sm-12">
                        <div class="text-center">
                            <asp:Button ID="Button1" runat="server" Text="确认接收" OnClientClick="return CancelWindow()" CssClass="btn btn-success" />
                            <asp:Button ID="BT_Cancl" runat="server" Text="关闭窗口" OnClientClick="return CancelWindow()" CssClass="btn btn-default" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="alert alert-warning">
                温馨提示：
                <ol>
                    <li>金额之和不能大于单据金额！</li>
                    <li>操作方式：先选择科目类型，再点击“添加”按钮。</li>
                </ol>
            </div>
        </div>

        <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading" UseEmbeddedScripts="false">
            <AjaxSettings>
                <rad:AjaxSetting AjaxControlID="TVGoodsClass">
                    <UpdatedControls>
                        <rad:AjaxUpdatedControl ControlID="RadGrid1"></rad:AjaxUpdatedControl>
                    </UpdatedControls>
                </rad:AjaxSetting>
            </AjaxSettings>
        </rad:RadAjaxManager>

        <asp:TextBox ID="hidden_EditIndex" runat="server" Visible="true"></asp:TextBox>

        <br />
        <asp:TextBox ID="hidden_EditComboBoxID" runat="server" Visible="true" Width="400"></asp:TextBox>

        <script type="text/javascript">
            //验证
            function CheckEmpty() {
                $("span[class='error']").remove();//移除所有错误提示

                $(".Check").each(function () {
                    var obj = $(this);
                    if (obj.val().length === 0) {
                        if (obj.next("span[class='error']").length === 0) {
                            obj.after("<span class='error' style='color:red;'>*</span>");
                        }
                    } else {
                        obj.next("span[class='error']").remove();
                    }
                });

                if ($("span[class='error']").length === 0) {
                    return true;
                } else {
                    return false;
                }
            }

            //验证类型
            function check(obj, type) {
                if (!$.checkType(type).test($(obj).val())) {
                    $(obj).val("");
                    $(obj).attr("placeholder", castErrorMessage(type));
                }
            }

            function onToolBarClientButtonClicking(sender, args) {
                var button = args.get_item();
                if (button.get_commandName() == "DeleteSelected") {
                    args.set_cancel(!confirm('Delete all selected customers?'));
                }
            }

            function RowDblClick(sender, eventArgs) {
                sender.get_masterTableView().editItem(eventArgs.get_itemIndexHierarchical());
            }
        </script>

        <script type="text/javascript">

            //stop the event bubbling
            function StopPropagation(e) {
                if (!e) {
                    e = window.event;
                }

                e.cancelBubble = true;
            }

            //find the selected node in the treeview inside the combobox and scroll it into view
            function OnClientDropDownOpenedHandler(sender, eventArgs,a,b,c) {
                //debugger
                var tree = sender.get_items().getItem(0).findControl("RadTreeView1");
                var selectedNode = tree.get_selectedNode();

                if (selectedNode) {
                    selectedNode.scrollIntoView();
                }
            }

            function nodeEditStart(a,b,c,d,e,f) {
            }

            //when tree node is clicked, set the text and value for the item in the combobox and commit the changes
            function nodeClicking(sender, args) {

                var radComboBox1 =$("#hidden_EditComboBoxID").val();

                
                //get the id of the employeesCombo in the edited row (passed from the server in the ItemDataBound event handler)
                var comboBox = $find(radComboBox1);
                var node = args.get_node();
                
                comboBox.set_text(node.get_text());

                comboBox.trackChanges();
                comboBox.get_items().getItem(0).set_text(node.get_text());
                comboBox.get_items().getItem(0).set_value(node.get_value());
                comboBox.commitChanges();

                comboBox.hideDropDown();

                // Call comboBox.attachDropDown if:
                // 1) The RadComboBox is inside an AJAX panel.
                // 2) The RadTreeView has a server-side event handler for the NodeClick event, i.e. it initiates a postback when clicking on a Node.
                // Otherwise the AJAX postback becomes a normal postback regardless of the outer AJAX panel.

                comboBox.attachDropDown();
            }
            function freightComboClientSelectedIndexChangedHandler(sender, eventArgs) {
                //get reference to the grid row DOM element
                var gridRow = sender.get_element().parentNode.parentNode;
                //locate the customers combobox in the same row using the $telerik.findControl method from the Telerik Client Static Library
                //note that the id of the combobox concatenates RCB_ + UniqueName value for the column, i.e. RCB_CustomerName in this particular case
                var customersCombo = $telerik.findControl(gridRow, "RCB_CustomerName");
                // this will fire the ItemsRequested server event and hook the OnClientItemsRequested client event of the
                // customers combobox passing the freight as a parameter to the first event
                customersCombo.add_itemsRequested(customersComboItemsRequested);
                customersCombo.requestItems(eventArgs.get_item().get_value(), false);
            }
            function customersComboItemsRequested(sender, eventArgs) {
                if (sender.get_items().get_count() > 0) {
                    // pre-select the first item
                    sender.findItemByText(sender.get_items().getItem(0).get_text()).select();
                }
                //detach the client items requested event as it not needed any more
                sender.remove_itemsRequested(customersComboItemsRequested);
            }
        </script>
    </form>
</body>
</html>

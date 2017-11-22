<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="PurchasingPage.aspx.cs"
    Inherits="ERP.UI.Web.PurchasingPage" MasterPageFile="~/MainMaster.master" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/jquery.js" type="text/javascript"></script>
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
        <script src="JavaScript/common.js" type="text/javascript"></script>
        <script type="text/javascript" language="javascript">
            function ShowPurchasingFrom(purchasingId) {
                window.radopen("./Windows/PurchasingDetailForm.aspx?PurchasingID=" + purchasingId, "EditFrom");
            }
            function ShowClewForm(purchasingId, type) {
                window.radopen("./Windows/ShowPurasingClewForm.aspx?PurchasingID=" + purchasingId, "AddForm");
                return false;
            }
            function ShowStockStatementForm(purchasingId) {
                window.radopen("./Windows/StockStatementForm.aspx?PurchasingID=" + purchasingId, "EditFrom");
                return false;
            }
            function Show(purchasingId) {
                window.radopen("./Windows/PurchasingDetailForm.aspx?PurchasingID=" + purchasingId + "&readly=1", "EditFrom");

            }
            function AddPurchingForm() {
                window.radopen("./Windows/PurchingForm.aspx", "EditFrom");
                return false;
            }
            function AddGoodsFrom(purchasingId) {
                window.radopen("./Windows/AddGoodsFrom.aspx?PurchasingID=" + purchasingId, "EditFrom");
            }
            <%--function ShowTempForm() {
                // var tempId = $("#CPHStage_RCB_ExcelTemp").val();
                var excelTempObj = document.getElementById("<%=RCB_ExcelTemp.ClientID  %>");
                var excelTempObjIndex = excelTempObj.selectedIndex;
                var excelTempObjValue = excelTempObj.options[excelTempObjIndex].value;
                window.radopen("./Windows/TempForm.aspx?TempId=" + excelTempObjValue, "AddForm");
            }--%>
            function RowDblClick(obj, args) {
                if (args.getDataKeyValue("PurchasingState") == 1 || args.getDataKeyValue("PurchasingState") == 0)
                    window.radopen("./Windows/PurchasingGoodsPrint.aspx?PurchasingID=" + args.getDataKeyValue("PurchasingID") + "&readly=1&WarehouseId=" + args.getDataKeyValue("WarehouseID"), "EditFrom");
                else
                    window.radopen("./Windows/PurchasingStockingFrom.aspx?PurchasingID=" + args.getDataKeyValue("PurchasingID") + "&readly=1", "EditFrom");
            }
            function RowClick(obj, args) {
                obj.set_selectedItemsInternal = true;
            }
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
            function Showconfirm() {
                var conf = window.confirm('提示：是否确认完成？');
                if (!conf)
                    return false;
                return true;
            }
            function OnDeleteConfirm() {
                var conf = window.confirm('提示：是否确认删除吗？');
                if (!conf)
                    return false;
                return true;
            }
            function ShowImg(obj) {
                var object = eval(obj);
                object.style.display = "block";
            }

            function HiddleImg(obj) {
                var object = eval(obj);
                object.style.display = "none";
            }
            function clientShow(sender, eventArgs) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }

            function ShowAudit(purchasingId, warehouseId, hostingFilialeId) {
                var conf = window.confirm('提示：是否确认审核？');
                if (conf) {
                    window.radopen("./Windows/ShowPurchasingAdjustForm.aspx?PurchasingID=" + purchasingId + "&WarehouseID=" + warehouseId + "&HostingFilialeId="+hostingFilialeId, "AuditForm");
                }
                return false;
            }

            //合并采购单
            function ConfirmMerger(purchasingId, arrivalTime) {
                var conf = window.confirm('您确定将选择的采购单进行合并吗？');
                if (conf) {
                    $("input[id$='Hid_purchasingId']").val(purchasingId);
                    $("input[id$='Hid_arrivalTime']").val(arrivalTime);
                    $("input[id$='btn_Merger']").click();
                }
                return false;
            }

            function Check() {
                var mergerCount = $(".rgMasterTable .rgSelectedRow").length;
                if (mergerCount == 0) {
                    alert("请选择您要合并的采购单！");
                    return false;
                }
                else if (mergerCount == 1) {
                    alert("单个采购单不能合并！");
                    return false;
                }
            }

            //重新绑定Grid事件
            function refreshGrid() {
                $("input[id$='IB_CreationData']").click();
            }
        </script>
    </rad:RadScriptBlock>
    <div class="StagePanel" width="100%" id="  ">
        <table class="StagePanelHead" border="0" style="width: 100%;">
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td></td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <%--搜索框 --%>
        <table class="PanelArea">
            <tr>
                <td style="text-align: right;">起止日期：
                </td>
                <td>
                    <rad:RadDatePicker ID="RDP_StartTime" runat="server" Width="120px">
                    </rad:RadDatePicker>
                    -
                    <rad:RadDatePicker ID="RDP_EndTime" runat="server" Width="120px">
                    </rad:RadDatePicker>
                </td>
                <td style="text-align: right;">供应商：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Company" runat="server" UseEmbeddedScripts="false" Width="200px"
                        AllowCustomText="true" EnableLoadOnDemand="True" Height="120px" DataValueField="CompanyId" AutoPostBack="True"
                        DataTextField="CompanyName" AppendDataBoundItems="true" OnItemsRequested="RCB_Company_OnItemsRequested" OnSelectedIndexChanged="RCB_Company_OnSelectedIndexChanged">
                        <Items>
                        </Items>
                    </rad:RadComboBox>
                </td>
                <td style="text-align: right;">仓库：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        MarkFirstMatch="True" ShowToggleImage="True" DataTextField="Value" DataValueField="Key"
                        Width="120px" Height="200px" AutoPostBack="True" OnSelectedIndexChanged="RCBWarehouse_OnSelectedIndexChanged">
                    </rad:RadComboBox>
                </td>
                <td style="text-align: right;">采购公司：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Filiale" runat="server" Width="120px" DataTextField="HostingFilialeName" DataValueField="HostingFilialeId"></rad:RadComboBox>
                </td>
                
                <td style="text-align: right;">状态：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_State" runat="server" UseEmbeddedScripts="false" MarkFirstMatch="True"
                        Width="100px" Height="120px" DataValueField="Key" DataTextField="Value" AppendDataBoundItems="true">
                    </rad:RadComboBox>
                </td>
            </tr>
            <tr>
                <td style="text-align: right;">商品搜索：
                </td>
                <td class="AreaEditFromRowInfo" >
                    <rad:RadComboBox ID="RCB_Goods" runat="server" CausesValidation="false" AutoPostBack="true"
                        AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="GoodsName" DataValueField="GoodsId"
                        Width="200px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td style="text-align: right;">采购单号：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TextBoxKeys" Width="120px" runat="server" SkinID="ShortInput"></asp:TextBox>
                </td>
                <td style="text-align: right;">责任人：
                </td>
                <td>
                    <rad:RadComboBox ID="RCB_Persion" runat="server" UseEmbeddedScripts="false" AccessKey="T"
                        AllowCustomText="True" MarkFirstMatch="True" ShowToggleImage="True" DataTextField="RealName"
                        DataValueField="PersonnelId" Width="120px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td style="text-align: right;">采购类别：
                </td>
                <td class="AreaEditFromRowInfo" align="left">
                    <rad:RadComboBox ID="RCB_Type" runat="server" UseEmbeddedScripts="false" MarkFirstMatch="True"
                        Width="120px" Height="120px" DataValueField="Key" DataTextField="Value" AppendDataBoundItems="true"
                        Text='<%# GetPurchasingType(Type) %>' Value='<%# (int)Type %>'>
                    </rad:RadComboBox>
                </td>
                <td style="text-align: right;">
                    <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="SearchButton" OnClick="Ib_CreationData_Click" />
                </td>
            </tr>
        </table>
        <%--模板框--%>
        <table class="PanelArea">
            <tr>
                <td class="Footer" style="text-align: right;">
                    <rad:RadComboBox ID="RCB_PurchasingFiliale" runat="server" Width="120px" DataTextField="Value" DataValueField="Key"></rad:RadComboBox>
                    <asp:LinkButton ID="IB_BindingFiliale" OnClick="IB_BindingFiliale_OnClick" CausesValidation="false" runat="server">
                        <asp:Image ID="Image3" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />绑定采购公司
                    </asp:LinkButton>&nbsp;
                </td>
                <td style="text-align: right; width: 100px;">
                    <asp:LinkButton ID="IB_AddPurching" OnClientClick="return AddPurchingForm()" CausesValidation="false" runat="server">
                        <asp:Image ID="Image2" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />新建采购单
                    </asp:LinkButton>
                    &nbsp;
                </td>
                <td style="text-align: right; width: 80px;">
                    <asp:LinkButton ID="IB_InPurchasing" OnClick="IB_InPurchasing_OnClick" CausesValidation="false"
                        runat="server">
                        <asp:Image ID="Image1" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />采购中
                    </asp:LinkButton></td>
                <td style="text-align: right; width: 80px;">
                    <asp:LinkButton ID="lb_Merger" CausesValidation="false" runat="server" OnClientClick="return Check();" OnClick="lb_Merger_Click">
                        <asp:Image ID="Image5" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />合并
                    </asp:LinkButton>
                    <div style="display: none;">
                        <asp:Button ID="btn_Merger" runat="server" OnClick="btn_Merger_Click" />
                        <asp:HiddenField ID="Hid_purchasingId" runat="server" />
                        <asp:HiddenField ID="Hid_arrivalTime" runat="server" />
                    </div>
                </td>
                <td style="text-align: right; width: 80px;">
                    <asp:LinkButton ID="IB_DeletOrder" OnClientClick="return OnDeleteConfirm()" OnClick="Ib_DeleteAll_Click"
                        CausesValidation="false" runat="server">
                        <asp:Image ID="Image4" SkinID="DeleteImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />删除
                    </asp:LinkButton></td>
                <td style="text-align: right; width: 170px;">
                    <asp:DropDownList ID="RCB_ExcelTemp" runat="server" DataTextField="Value"
                        DataValueField="Key" SkinID="ShortDropDown" AutoPostBack="true"
                        Width="150px">
                        <asp:ListItem Selected="True" Value="00000000-0000-0000-0000-000000000000">选择你预先设好的模板</asp:ListItem>
                    </asp:DropDownList></td>
                <td style="text-align: center; width: 100px; padding-top: 3px;">
                    <asp:ImageButton ID="IB_ExportData" SkinID="ExportData" OnClick="Ib_ExportData_Click" runat="server" /></td>
            </tr>
        </table>
        <%--采购表--%>
        <rad:RadGrid ID="Rgd_Purchasing" runat="server" SkinID="CustomPaging" OnNeedDataSource="RGPP_OnNeedDataSource"
            OnItemDataBound="RgdPurchasingItemDataBound" AllowMultiRowSelection="True" Width="100%"
            OnItemCommand="Rgd_Purchasing_ItemCommand" ShowFooter="true" PageSize="20">
            <ClientSettings>
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" />
                <ClientEvents OnRowDblClick="RowDblClick" />
            </ClientSettings>
            <MasterTableView DataKeyNames="PurchasingID,PurchasingNo,CompanyName,PurchasingState,WarehouseID,CompanyID,PurchasingFilialeId,PurchasingType,FilialeID,PersonResponsible,IsOut,ArrivalTime,PurchasingPersonName"
                ClientDataKeyNames="PurchasingID,PurchasingState,WarehouseID" CommandItemDisplay="None" Width="100%">
                <CommandItemStyle HorizontalAlign="Left" Height="0px" />
                <Columns>
                    <rad:GridClientSelectColumn UniqueName="column">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridClientSelectColumn>
                    <rad:GridTemplateColumn DataField="PurchasingNo" HeaderText="采购单号" UniqueName="PurchasingNo">
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <label id="lab_purachno" style='<%#(DateTime.Now-Convert.ToDateTime(Eval("StartTime").ToString())).Days>=7&&Convert.ToInt32(Eval("PurchasingState"))
                             ==(int)(PurchasingState.Purchasing)&&Convert.ToInt32(Eval("PurchasingType"))==(int)(PurchasingType.AutoStock)?"color:Red": "" %>'>
                                <%#Eval("PurchasingNo") %></label>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="PurchasingFilialeId" ReadOnly="true" HeaderText="采购公司"
                        UniqueName="PurchasingFilialeId">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# GetPurchasingFilialeName(Eval("PurchasingFilialeId")) %>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="CompanyName" ReadOnly="true" HeaderText="供应商" UniqueName="CompanyName">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="PurchaseGroupId" ReadOnly="true" HeaderText="分组"
                        UniqueName="PurchaseGroupId">
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# GetPurchaseGroupIdName(Eval("PurchaseGroupId"))%>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="SumPrice" ReadOnly="true" HeaderText="总价(元)" UniqueName="SumPrice">
                        <ItemTemplate>
                            <%#ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("SumPrice"))%>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="SurplusMoney" ReadOnly="true" HeaderText="剩余金额" Aggregate="Sum"
                        FooterText="剩余金额合计：" UniqueName="SurplusMoney" Visible="False">
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="StartTime" HeaderText='采购时间' ReadOnly="true" UniqueName="StartTime"
                        DataFormatString="{0:MM-dd HH:mm}">
                        <HeaderStyle Width="130px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="EndTime" HeaderText='完成时间' ReadOnly="true" UniqueName="EndTime">
                        <HeaderStyle Width="130px" HorizontalAlign="Center" />
                        <ItemTemplate>
                            <%# (DateTime)Eval("EndTime")==DateTime.MaxValue||(DateTime)Eval("EndTime")==DateTime.MinValue?"":((DateTime)Eval("EndTime")).ToString("MM-dd HH:mm") %>
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="PurchasingState" HeaderText="状态" UniqueName="PurchasingState">
                        <ItemTemplate>
                            <asp:Label ID="lab_State" runat="server" Text='<%# EnumAttribute.GetKeyName((PurchasingState)Eval("PurchasingState")) %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <rad:RadComboBox ID="RCB_State" runat="server" Width="120px" Height="120px">
                            </rad:RadComboBox>
                        </EditItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="PurchasingType" ReadOnly="true" HeaderText="采购类别"
                        UniqueName="PurchasingType">
                        <ItemTemplate>
                            <asp:Label ID="lab_Type" runat="server" Text='<%# EnumAttribute.GetKeyName((PurchasingType)Eval("PurchasingType")) %>'>
                            </asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="WarehouseID" ReadOnly="true" HeaderText="所在仓库"
                        UniqueName="WarehouseID">
                        <ItemTemplate>
                            <asp:Label ID="lab_wareName" runat="server" Text='<%#GetWareGhouseName(Eval("WarehouseID"))%>'>
                            </asp:Label>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="备注" UniqueName="Description">
                        <ItemTemplate>
                            <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                                OnClientClick='<%# "return ShowClewForm(\"" + Eval("PurchasingID") + "\",\"1\");" %>'
                                onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                            <div style="position: absolute;">
                                <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative; display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px; font-weight: bold; height: auto; overflow: visible; word-break: break-all;"
                                    runat="server">
                                    <%# Eval("Description")%>
                                </div>
                            </div>
                        </ItemTemplate>
                        <HeaderStyle Width="50px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="PmName" ReadOnly="true" HeaderText="采购组" UniqueName="PmName"
                        Visible="False">
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn ReadOnly="true" HeaderText="责任人">
                        <ItemTemplate>
                            <%# GetPersonName(new Guid(Eval("PersonResponsible").ToString()))%>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="PurchasingPersonName" ReadOnly="true" HeaderText="提交人" UniqueName="PurchasingPersonName">
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn DataField="PurchasingID" HeaderText="操作" UniqueName="PurchasingID">
                        <ItemTemplate>
                            <Ibt:ImageButtonControl runat="server" CausesValidation="false" ID="IB_ModifyOrder"
                                OnClientClick='<%# "return ShowPurchasingFrom(\"" + Eval("PurchasingID")+ "\");" %>'
                                Visible='<%# Convert.ToInt32(Eval("PurchasingState"))==(int)(PurchasingState.NoSubmit) || Convert.ToInt32(Eval("PurchasingState"))==(int)(PurchasingState.Refusing)%>'
                                Text="修改"></Ibt:ImageButtonControl>
                            <%--<Ibt:ImageButtonControl runat="server" CausesValidation="false" ID="Ibn_Part" OnClientClick='<%# "return Show(\"" + Eval("PurchasingID")+ "\");" %>'
                                Visible='<%#(Convert.ToInt32(Eval("PurchasingState"))==(int)(PurchasingState.NoSubmit) || Convert.ToInt32(Eval("PurchasingState"))==(int)(PurchasingState.Refusing)) &&Convert.ToInt32(Eval("PurchasingState"))==(int)(PurchasingState.PartComplete) %>'
                                Text="修改" SkinID="InsertImageButton"></Ibt:ImageButtonControl>--%>
                            <Ibt:ImageButtonControl runat="server" CausesValidation="false" ID="IB_addGoods"
                                Visible='false' OnClientClick='<%# "return AddGoodsFrom(\"" + Eval("PurchasingID")+ "\");" %>'
                                Text="添加商品"></Ibt:ImageButtonControl>
                            <Ibt:ImageButtonControl ID="IB_Approved" runat="server" CommandName="Consignee" Visible='<%# Convert.ToInt32(Eval("PurchasingState"))==(int)(PurchasingState.PartComplete)  %>'
                                OnClientClick='<%# "return Showconfirm();"%>' Text="已完成" />
                            <Ibt:ImageButtonControl runat="server" SkinType="Insert" CausesValidation="false"
                                ID="IB_StockStatement" OnClientClick='<%# "return ShowStockStatementForm(\"" + Eval("PurchasingID")+ "\");" %>'
                                Text="到货说明" Visible='<%#((Convert.ToInt32(Eval("PurchasingState"))==(int)(PurchasingState.Purchasing)||Convert.ToInt32(Eval("PurchasingState"))==(int)(PurchasingState.StockIn)||Convert.ToInt32(Eval("PurchasingState"))==(int)(PurchasingState.PartComplete)))  %>'></Ibt:ImageButtonControl>
                        </ItemTemplate>
                        <HeaderStyle Width="120" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="审核" UniqueName="Approved">
                        <ItemTemplate>
                            <asp:ImageButton runat="server" CausesValidation="false" ID="IbApproved" SkinID="AffirmImageButton"
                                OnClientClick='<%# "return ShowAudit(\"" + Eval("PurchasingID")+ "\",\"" + Eval("WarehouseID")+ "\",\"" + Eval("PurchasingFilialeId")+ "\");" %>'
                                CommandName="Approved" Visible='<%# AllowAuditing(Eval("PurchasingState")) %>' />
                        </ItemTemplate>
                        <HeaderStyle Width="50" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>
    <rad:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
            <rad:RadWindow ID="PurchasingForm" runat="server" Width="800px" Height="500px" />
            <rad:RadWindow ID="EditFrom" runat="server" OnClientShow="clientShow" />
            <rad:RadWindow ID="AddForm" runat="server" Width="610" Height="350" />
            <rad:RadWindow ID="StockForm" runat="server" Width="750" Height="500" />
            <rad:RadWindow ID="AuditForm" runat="server" Width="750" Height="350" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager runat="server" ID="RAM" OnAjaxRequest="Ram_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="Rgd_Purchasing">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_Purchasing" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_Purchasing" LoadingPanelID="loading" />
                </UpdatedControls>
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_ExcelTemp" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="Bt_Temp">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_Purchasing" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_ExcelTemp">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Bt_Temp" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Goods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Goods" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_SavePurching">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_Purchasing" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_CreationData">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_Purchasing" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_DeletOrder">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_Purchasing" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_InPurchasing">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_Purchasing" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_BindingFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_Purchasing" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="lb_Merger">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="btn_Merger" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_Merger">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_Purchasing" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Warehouse">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_ExcelTemp" LoadingPanelID="loading" />
                </UpdatedControls>
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Filiale" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_Company">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_Filiale" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

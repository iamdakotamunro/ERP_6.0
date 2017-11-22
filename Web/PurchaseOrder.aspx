<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="PurchaseOrder.aspx.cs" Inherits="ERP.UI.Web.PurchaseOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/jquery.js" type="text/javascript"></script>
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
        <script src="JavaScript/common.js" type="text/javascript"></script>
        <script type="text/javascript" language="javascript">
            function ShowAddPurchaseOrderForm() {
                window.radopen("./Windows/AddPurchaseOrderForm.aspx", "AddPurchaseOrderForm");
                return false;
            }
            
            function clientShow(sender, eventArgs) {
                sender.set_initialBehaviors("Maximize,Close");
                sender.Maximize(true);
                sender.IsMaximized(true); //最大化
            }

            //修改
            function ShowPurchaseOrderEditForm() {
                window.radopen("./Windows/PurchaseOrderEditForm.aspx", "PurchaseOrderDetailForm");
            }

            function RowDblClick() {
                window.radopen("./Windows/PurchaseOrderDetailForm.aspx", "PurchaseOrderDetailForm");
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
                <td width="100px" align="right">起止日期：
                </td>
                <td width="250px" align="left">
                    <table width="100%">
                        <tr>
                            <td align="left">
                                <rad:RadDatePicker ID="RDP_StartTime" runat="server" Width="120px">
                                </rad:RadDatePicker>
                            </td>
                            <td>-
                            </td>
                            <td align="left">
                                <rad:RadDatePicker ID="RDP_EndTime" runat="server" Width="120px">
                                </rad:RadDatePicker>
                            </td>
                        </tr>
                    </table>
                </td>
                <td width="100px" align="right">仓库：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Warehouse" runat="server" Width="120px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td width="100px" align="right">供应商：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Company" runat="server"  Width="120px" Height="120px" >
                    </rad:RadComboBox>
                </td>
                  <td width="100px" align="right">公司：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_State" runat="server"  Width="120px" Height="120px">
                    </rad:RadComboBox>
                </td>
                <td width="100px" align="right">状态：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBox1" runat="server"  Width="120px" Height="120px">
                    </rad:RadComboBox>
                </td>
               
            </tr>
            <tr>
                <td class="ShortFromRowTitle" colspan="2">
                    
                </td>
                <td width="100px" align="right">采购单号：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TextBoxKeys" Width="120px" runat="server"></asp:TextBox>
                </td>
                <td width="100px" align="right">商品搜索：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Goods" runat="server"  Width="120px" Height="200px">
                    </rad:RadComboBox>
                </td>
                <td align="right">责任人：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RCB_Persion" runat="server" Width="120px" Height="200px">
                    </rad:RadComboBox>
                </td>
                
                 <td width="100px" align="right">采购类别：
                </td>
                <td class="AreaEditFromRowInfo" align="left">
                    <rad:RadComboBox ID="RCB_Type" runat="server"  Width="120px" Height="120px" >
                    </rad:RadComboBox>
                </td>
                <td align="right" colspan="3">
                    <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="SearchButton" />
                </td>
            </tr>
        </table>
        <%--模板框--%>
        <table class="PanelArea">
            <tr>
                <td class="Footer" align="right" colspan="11" valign="top">
                    <asp:LinkButton ID="IB_InPurchasing"  CausesValidation="false"
                        runat="server">
                        <asp:Image ID="Image1" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />审批
                    </asp:LinkButton>&nbsp;
                    <asp:LinkButton ID="IB_AddPurching" OnClientClick="return ShowAddPurchaseOrderForm();" CausesValidation="false"
                        runat="server">
                        <asp:Image ID="Image2" SkinID="InsertImageButton"   runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />新建采购单
                    </asp:LinkButton>&nbsp;
                    <asp:LinkButton ID="lb_Merger" CausesValidation="false" runat="server">
                        <asp:Image ID="Image5" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />合并
                    </asp:LinkButton>&nbsp;
                    <asp:LinkButton ID="IB_DeletOrder"
                        CausesValidation="false" runat="server">
                        <asp:Image ID="Image4" SkinID="DeleteImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />删除
                    </asp:LinkButton>&nbsp;
                    <asp:ImageButton ID="IB_ExportData" SkinID="ExportData" 
                        runat="server" />
                </td>
            </tr>
        </table>
        <%--采购表--%>
        <rad:RadGrid ID="Rgd_Purchasing" runat="server" SkinID="CustomPaging" OnNeedDataSource="RGPP_OnNeedDataSource"
             AllowMultiRowSelection="True" Width="100%"
            ShowFooter="true">
            <ClientSettings>
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" />
                 <ClientEvents OnRowDblClick="RowDblClick" />
            </ClientSettings>
            <MasterTableView DataKeyNames="PurchasingID,PurchasingNo,CompanyName,PurchasingState,WarehouseID,CompanyID,PurchasingFilialeId,PurchasingType,FilialeID,PersonResponsible,IsOut,ArrivalTime"
                ClientDataKeyNames="PurchasingID,PurchasingState" CommandItemDisplay="None" Width="100%">
                <CommandItemStyle HorizontalAlign="Left" Height="0px" />
                <Columns>
                    <rad:GridClientSelectColumn UniqueName="column">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridClientSelectColumn>
                    <rad:GridTemplateColumn DataField="PurchasingNo" HeaderText="采购单号" UniqueName="PurchasingNo">
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemTemplate>
                           
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                      <rad:GridTemplateColumn DataField="PurchasingType" ReadOnly="true" HeaderText="采购类别"
                        UniqueName="PurchasingType">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="120px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
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
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="PurchasingFilialeId" ReadOnly="true" HeaderText="采购公司"
                        UniqueName="PurchasingFilialeId">
                        <HeaderStyle Width="200px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                        <ItemTemplate>
                        </ItemTemplate>
                    </rad:GridTemplateColumn>
                   <rad:GridTemplateColumn DataField="WarehouseID" ReadOnly="true" HeaderText="所在仓库"
                        UniqueName="WarehouseID">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    

                   
                    <rad:GridTemplateColumn DataField="SumPrice" ReadOnly="true" HeaderText="总价(元)" UniqueName="SumPrice">
                        <ItemTemplate>
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
                        </ItemTemplate>
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="PurchasingState" HeaderText="状态" UniqueName="PurchasingState">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="备注" UniqueName="Description">
                        <ItemTemplate>
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
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="PurchasingID" HeaderText="操作" UniqueName="PurchasingID">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="120" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>
    <rad:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
            <rad:RadWindow ID="PurchaseOrderDetailForm" runat="server" OnClientShow="clientShow" Title="采购单" />
            <rad:RadWindow ID="AddPurchaseOrderForm" runat="server" OnClientShow="clientShow" Title="采购单新增" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager runat="server" ID="RAM" >
        
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>
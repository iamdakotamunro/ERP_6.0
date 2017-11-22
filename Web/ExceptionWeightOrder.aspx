<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="ExceptionWeightOrder.aspx.cs" Inherits="ERP.UI.Web.ExceptionWeightOrder" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/jquery.js" type="text/javascript"></script>
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
        <script src="JavaScript/common.js" type="text/javascript"></script>
        
    </rad:RadScriptBlock>
    <div class="StagePanel" width="100%" id="  ">
       
        <%--搜索框 --%>
        <table class="PanelArea">
            <tr>
                  <td width="100px" align="right">完成时间：
                </td>
                <td width="250px" align="left" class="AreaEditFromRowInfo">
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
                  <td width="100px" align="right">单据号：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                </td>
                  <td width="100px" align="right">商品：
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>
                </td>
                  <td align="right">处理状态：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBox4" runat="server" Width="120px" Height="200px">
                    </rad:RadComboBox>
                </td>
                 <td align="right">
                    <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="SearchButton" />
                </td>
            </tr>
            
        </table>
       
        
        <rad:RadGrid ID="Rgd_Purchasing" runat="server" OnNeedDataSource="RGPP_OnNeedDataSource"
             AllowMultiRowSelection="True" Width="100%">
            <ClientSettings>
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" />
            </ClientSettings>
            <MasterTableView DataKeyNames="PurchasingID,PurchasingNo,CompanyName,PurchasingState,WarehouseID,CompanyID,PurchasingFilialeId,PurchasingType,FilialeID,PersonResponsible,IsOut,ArrivalTime"
                ClientDataKeyNames="PurchasingID,PurchasingState" Width="100%">
                  <CommandItemTemplate>
                      <asp:Button ID="Button1" runat="server" Text="批量处理" />&nbsp;&nbsp;&nbsp;
                </CommandItemTemplate>
               <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                <Columns>
                      <rad:GridClientSelectColumn UniqueName="column">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridClientSelectColumn>
                    <rad:GridTemplateColumn DataField="" HeaderText="仓库" UniqueName="">
                        <ItemTemplate>
                           
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                      <rad:GridTemplateColumn DataField="" HeaderText="商品"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                     <rad:GridTemplateColumn DataField="" HeaderText="出货单号"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                     <rad:GridTemplateColumn DataField="" HeaderText="状态"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="" HeaderText="完成时间"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="" HeaderText="系统重量"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="" HeaderText="实际称重重量"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="" HeaderText="重量差异"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>
    <rad:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager runat="server" ID="RAM" >
        
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

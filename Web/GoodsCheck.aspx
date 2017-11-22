<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="GoodsCheck.aspx.cs" Inherits="ERP.UI.Web.GoodsCheck" %>
<%@ Register Src="UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="JavaScript/jquery.js" type="text/javascript"></script>
        <script src="JavaScript/telerik.js" type="text/javascript"></script>
        <script src="JavaScript/common.js" type="text/javascript"></script>
        <script type="text/javascript" language="javascript">
            //退回检查
            function ShowGoodsReturnCheckForm() {
                window.radopen("./Windows/GoodsReturnCheckForm.aspx", "GoodsReturnCheckForm");
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
        <table class="PanelArea" >
            <tr>
                  <td width="20%" align="right">退回时间：
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
                 <td width="20%" align="right">检查状态：
                </td>
                <td class="AreaEditFromRowInfo">
                    <rad:RadComboBox ID="RadComboBox1" runat="server" Width="120px" Height="200px">
                    </rad:RadComboBox>
                </td>
                  <td width="20%" align="right">
                </td>
                <td class="AreaEditFromRowInfo">
                    <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                </td>
                   <td width="20%" align="right">
                </td>
                 <td align="left">
                    <asp:ImageButton ID="IB_CreationData" runat="server" SkinID="SearchButton" />
                </td>
            </tr>
        </table>
       
        <rad:RadGrid ID="Rgd_Purchasing" runat="server"  OnNeedDataSource="RGPP_OnNeedDataSource"
             AllowMultiRowSelection="True" Width="100%"
            ShowFooter="true">
            <ClientSettings>
                <Selecting AllowRowSelect="True" EnableDragToSelectRows="False" />
            </ClientSettings>
            <MasterTableView DataKeyNames="PurchasingID,PurchasingNo,CompanyName,PurchasingState,WarehouseID,CompanyID,PurchasingFilialeId,PurchasingType,FilialeID,PersonResponsible,IsOut,ArrivalTime"
                ClientDataKeyNames="PurchasingID,PurchasingState" Width="100%">
                <CommandItemTemplate>
                    &nbsp;&nbsp;&nbsp;
                   <Ibt:ImageButtonControl runat="server" ID="LinkButtonRefresh" CommandName="RebindGrid"
                       SkinType="Refresh" Text="刷新"></Ibt:ImageButtonControl>
                </CommandItemTemplate>
                <CommandItemStyle HorizontalAlign="Right" Height="26px" />
                <Columns>
                  
                    <rad:GridTemplateColumn DataField="" HeaderText="退换货号" UniqueName="">
                        <ItemTemplate>
                           
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                      <rad:GridTemplateColumn DataField="" HeaderText="售后店铺"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                     <rad:GridTemplateColumn DataField="" HeaderText="售后时间"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                     <rad:GridTemplateColumn DataField="" HeaderText="检查状态"
                        UniqueName="">
                        <ItemTemplate>
                        </ItemTemplate>
                        <HeaderStyle Width="100px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn DataField="" HeaderText="退回检查"
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
            <rad:RadWindow ID="GoodsReturnCheckForm" runat="server" Title="退回检查" Height="540" Width="900"/>

        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager runat="server" ID="RAM" >
        
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

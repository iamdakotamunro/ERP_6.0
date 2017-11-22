<%@ Page Language="C#" AutoEventWireup="True" Inherits="ERP.UI.Web.Windows.PurchasingDetailForm" CodeBehind="PurchasingDetailForm.aspx.cs" %>
<%@ Import Namespace="ERP.Enum" %>

<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl" TagPrefix="Ibt" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head" runat="server">
    <title>
    </title>
    <style type="text/css">
        .red {
            color: red;
        }
        
        .normal {
            color: black;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
        <script language="javascript" type="text/javascript">
            
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
                window.location.reload();
            }        
            function ShowPurchasingFrom() { 
                window.radopen("/Windows/AddGoodsFrom.aspx?PurchasingID=<%=Request["PurchasingID"]%>","PurchasingForm"); 
            }
        
            function ShowImg(obj) {
                var object = eval(obj);
                object.style.display = "block";
            }

            function HiddleImg(obj) {
                var object = eval(obj);
                object.style.display = "none";
            }

            function OnDeleteConfirm() {
                var conf = window.confirm('提示：是否确认删除吗？');
                if (!conf)
                    return false;
                return true;
            }
        </script>

    </rad:RadScriptBlock>
    <div style="padding-top: 5px;text-align:center; width: 100%; height: 40px;">
            <asp:Label ID="lab_Purchasing" runat="server"></asp:Label>
            仓库：
            <asp:DropDownList ID="RCB_Warehouse" runat="server" SkinID="ShortDropDown" Width="150px">
            </asp:DropDownList>
            <br/>
            <asp:Label ID="lab_bhdate" runat="server"></asp:Label>
            <asp:Label ID="lab_dhdate" runat="server"></asp:Label>
    </div>
    <%--采购表--%>
    
    <rad:RadGrid ID="Rgd_PurchasingDetail" runat="server" OnNeedDataSource="Rgd_PurchasingDetail_OnNeedDataSource"
        OnItemCommand="Rgd_PurchasingDetail_ItemCommand" AllowMultiRowSelection="true"
        OnItemDataBound="Rgd_PurchasingDetail_ItemDataBound" Width="100%" AllowPaging="true"
        OnPageIndexChanged="Rgd_PurchasingDetail_PageIndexChanged" 
        PageSize="40">
        <ClientSettings>
            <%--<Scrolling AllowScroll="True" UseStaticHeaders="True" />--%>
            <Selecting AllowRowSelect="True" />
        </ClientSettings>
        <MasterTableView DataKeyNames="GoodsID,GoodsCode,Specification,PlanQuantity,PurchasingGoodsID,Price,DayAvgStocking,PlanStocking,PurchasingGoodsType,CPrice,CompanyID"
            ShowFooter="true" EditMode="InPlace" ClientDataKeyNames="GoodsID,GoodsCode,Specification"
            ShowGroupFooter="true">
            <CommandItemTemplate>
                <table>
                    <tr>
                        <td>&nbsp;</td>
                        <td style="text-align: right;"><asp:Label ID="Label1" Text="主商品数量:" Visible='<%#string.IsNullOrEmpty(Request["readly"])%>' runat="server"></asp:Label></td>
                         <td style="text-align: right;"><asp:TextBox ID="tbx_GoodsNum" SkinID="ShortInput" Visible='<%#string.IsNullOrEmpty(Request["readly"])%>' runat="server"></asp:TextBox></td>
                         <td style="text-align: right;"><Ibt:ImageButtonControl ID="ibt_GoodsNum" Text="分配" runat="server" SkinType="Insert" CausesValidation="false" Visible='<%#string.IsNullOrEmpty(Request["readly"])%>' CommandName="Distribution"></Ibt:ImageButtonControl></td>
                         <td style="text-align: right;"><asp:Label ID="lab_price" Text="采购价格:" Visible='False' runat="server"></asp:Label></td>
                         <td style="text-align: right;"><asp:TextBox ID="tbx_Price" SkinID="ShortInput" Visible='False' runat="server"></asp:TextBox></td>
                         <td style="text-align: right;"><asp:Label ID="lab_stocking" Text="备货天数增加:" Visible='<%#string.IsNullOrEmpty(Request["readly"]) && ShowFlag%>' runat="server"></asp:Label></td>
                         <td style="text-align: right;"><asp:TextBox ID="tbx_stockingDay" Visible='<%#string.IsNullOrEmpty(Request["readly"]) && ShowFlag%>' SkinID="ShortInput" runat="server"></asp:TextBox></td>
                         <td style="text-align: right;"><Ibt:ImageButtonControl ID="IB_Update" Text="修改" runat="server" SkinType="Insert" CommandName="Stocking" Visible='<%#string.IsNullOrEmpty(Request["readly"]) && ShowFlag %>' CausesValidation="false"></Ibt:ImageButtonControl></td>
                         <td style="text-align: right;"><rad:RadComboBox ID="RCB_AllCommanyList" runat="server" DataSource="<%#Rcb_CommanyDataSource()%>"
                                    CausesValidation="false" AutoPostBack="true" ShowToggleImage="True" EnableLoadOnDemand="True"
                                    Height="180px" Width="120px" AllowCustomText="True" DataTextField="CompanyName"
                                    DataValueField="CompanyId" Visible='<%#string.IsNullOrEmpty(Request["readly"])%>'
                                    OnItemsRequested="Rcb_AllCommanyList_ItemsRequested">
                                </rad:RadComboBox></td>
                        <td style="text-align: right;"><Ibt:ImageButtonControl runat="server" SkinType="Delete" CommandName="UpdateCompany"
                                    CausesValidation="false" Visible='<%#string.IsNullOrEmpty(Request["readly"])%>'
                                    ID="IB_UpdateCompanyName" Text="更改供应商">
                                </Ibt:ImageButtonControl></td>
                                        <td> <Ibt:ImageButtonControl runat="server" SkinType="Delete" CausesValidation="false"
                                    Visible='<%#string.IsNullOrEmpty(Request["readly"])%>' ID="IB_AddGoods" OnClientClick="return ShowPurchasingFrom();"
                                    Text="添加商品">
                                </Ibt:ImageButtonControl></td>
                        <td style="text-align: right;"><Ibt:ImageButtonControl runat="server" SkinType="Delete" Visible='<%#string.IsNullOrEmpty(Request["readly"])%>'
                                CausesValidation="false" ID="IB_DeletOrder" OnClick="Ib_DeleteAll_Click" Text="删除">
                            </Ibt:ImageButtonControl></td>
                        <td style="text-align: right;">到货日期：</td>
                        <td style="text-align: right;"><rad:RadDateTimePicker ID="RDP_ArrivalTime" runat="server" Width="150px" EnableTyping="False" ></rad:RadDateTimePicker></td>
                        <td style="text-align: right;">
                            <Ibt:ImageButtonControl runat="server" SkinType="Insert" CausesValidation="false"
                                Visible='<%#string.IsNullOrEmpty(Request["readly"])%>' ID="IB_SaveGoods" OnClick="Ib_SaveGoods_Click"
                                Text="保存">
                            </Ibt:ImageButtonControl>
                        </td>
                    </tr>
                </table>
            </CommandItemTemplate>
            <GroupByExpressions>
                <rad:GridGroupByExpression>
                    <GroupByFields>
                        <rad:GridGroupByField FieldName="GoodsName" HeaderText=" " HeaderValueSeparator=" " />
                    </GroupByFields>
                    <SelectFields>
                        <rad:GridGroupByField FieldName="GoodsName" HeaderText=" " HeaderValueSeparator=" " />
                    </SelectFields>
                </rad:GridGroupByExpression>
            </GroupByExpressions>
            <Columns>
                <rad:GridClientSelectColumn UniqueName="column">
                    <HeaderStyle Width="40px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridClientSelectColumn>
                <rad:GridBoundColumn DataField="GoodsCode" HeaderText="商品编号" ReadOnly="true" UniqueName="GoodsCode">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" ReadOnly="true" UniqueName="Specification">
                    <HeaderStyle Width="120px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="备注" DataField="Description" UniqueName="Description">
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%# GetDescription(Eval("Description"))%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="GoodsId" HeaderText="公司实际库存" UniqueName="GoodsId">
                    <ItemTemplate>
                        <%# GetUsableStock(Eval("GoodsId"))%>
                    </ItemTemplate>
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="SixtyDaySales" HeaderText="前第2月销量" ReadOnly="true"
                    UniqueName="SixtyDaySales">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ThirtyDaySales" HeaderText="前第1月销量" ReadOnly="true"
                    UniqueName="ThirtyDaySales">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="ElevenDaySales" HeaderText="日均销量<br>(11天)" ReadOnly="true"
                    UniqueName="ElevenDaySales">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="PlanStocking" HeaderText="建议采购数量" ReadOnly="true"
                    UniqueName="PlanStocking">
                    <HeaderStyle Width="70px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="PlanQuantity" Aggregate="Sum" FooterText="小计数量:"
                    HeaderText="采购数量" UniqueName="PlanQuantity">
                    <ItemTemplate>
                        <asp:TextBox ID="tbx_PlanQuantity" Width="80px" runat="server" Visible='<%#Enable()%>'
                            Text='<%# Eval("PlanQuantity")%>' SkinID="ShortInput"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="rev_tbx_planQuantity" runat="server" ControlToValidate="tbx_PlanQuantity"
                            Text="*" ErrorMessage="采购量必须为数字类型!" ValidationExpression="^(([0-9]+[\.]?[0-9]+)|[1-9])$"></asp:RegularExpressionValidator>
                        <%#Enable()?"":Eval("PlanQuantity")%>
                    </ItemTemplate>
                    <FooterTemplate>
                        <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>
                    </FooterTemplate>
                    <FooterStyle HorizontalAlign="Center" />
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="Price" HeaderText="采购价格" UniqueName="Price">
                    <ItemTemplate>
                        <asp:TextBox ID="tbx_Price" Width="80px" runat="server" Visible='<%# Enable()%>'
                            Enabled='<%# Convert.ToDecimal(Eval("Price"))==0 || !AllowEidtPrice()?false:true%>' CssClass='<%# Convert.ToDecimal(Eval("Price")) > Convert.ToDecimal(Eval("CPrice"))?"red":"normal"  %>' Text='<%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("Price").ToString()))%> '
                            SkinID="ShortInput"></asp:TextBox>
                        <asp:RegularExpressionValidator ID="REVCarriage" runat="server" ControlToValidate="tbx_Price"
                            Text="*" ErrorMessage="请输入正数!" ValidationGroup="tbx_Price,IB_SaveGoods" ValidationExpression="^(([0-9]+[\.]?[0-9]+)|[1-9])$"></asp:RegularExpressionValidator>
                        <%#Enable()?"":ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("Price").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <%--Convert.ToDecimal(Eval("CPrice"))--%>
                <rad:GridTemplateColumn DataField="CPrice" HeaderText="前价格" ReadOnly="true" UniqueName="CPrice" >
                    <ItemTemplate>
                        <%# ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("CPrice").ToString()))%>
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridCalculatedColumn DataFields="Price,DPlanQuantity" DataType="System.Decimal"
                    UniqueName="PriceResult" HeaderText="金额" Expression="{0}*{1}" FooterText="小计金额 : "
                    Aggregate="Sum">
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridCalculatedColumn>
                <rad:GridBoundColumn DataField="RealityQuantity" HeaderText="实际来货" ReadOnly="true"
                    UniqueName="RealityQuantity">
                    <HeaderStyle Width="80px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="State" HeaderText="采购状态" ReadOnly="true" UniqueName="State">
                    <ItemTemplate>
                        <%# Eval("State").ToString()=="0"?"未完成":"已完成" %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="PurchasingGoodsType" HeaderText="赠品" ReadOnly="true"
                    UniqueName="PurchasingGoodsType">
                    <ItemTemplate>
                        <%# Eval("PurchasingGoodsType").ToString()=="1"?"赠品":"" %>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="PurchasingID" HeaderText="操作" UniqueName="PurchasingID">
                    <ItemTemplate>
                        <Ibt:ImageButtonControl CommandName="UpdateState" ID="Ib_GoodsState" Text="完成" SkinID="InsertImageButton"
                            Visible='<%# (Convert.ToInt32(Eval("State"))==0) && PurchasingInfo1.PurchasingState==(int)PurchasingState.PartComplete  %>' runat="server">
                        </Ibt:ImageButtonControl>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadWindowManager ID="StockWindowManager" runat="server" Width="800px" Height="500px">
        <Windows>
            <rad:RadWindow ID="PurchasingForm" runat="server" Width="800px" Height="500px" />
        </Windows>
    </rad:RadWindowManager>

    <rad:RadAjaxManager runat="server" ID="RAM" OnAjaxRequest="Ram_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_AllCommanyList">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="tbx_Price">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="rev_tbx_Price" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_AddGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_SaveGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="IB_DeletOrder">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
              <rad:AjaxSetting AjaxControlID="Rgd_PurchasingDetail">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="Rgd_PurchasingDetail" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

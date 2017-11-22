<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditStorageRecordApplyInForm.aspx.cs" Inherits="ERP.UI.Web.Windows.EditStorageRecordApplyInForm" %>
<%@ Register Src="../UserControl/ImageButtonControl.ascx" TagName="ImageButtonControl"
    TagPrefix="Ibt" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body scroll="no">
    <form id="form1" runat="server">
        <style type="text/css">
            .imgclass {
                z-index: 1000;
                border: 1px #ccc solid;
                position: fixed;
                display: none;
                left: 350px;
                top: 250px;
                background-color: #fcE1E7;
            }
        </style>
        <rad:RadScriptManager ID="RSM" runat="server">
        </rad:RadScriptManager>
        <rad:RadScriptBlock ID="RSB" runat="server">
            <script src="../JavaScript/jquery.js" type="text/javascript"></script>
            <script src="../JavaScript/telerik.js" type="text/javascript"></script>
            <script src="../JavaScript/common.js" type="text/javascript"></script>
            <script src="../My97DatePicker/WdatePicker.js"></script>
        </rad:RadScriptBlock>
        <asp:Panel ID="Panel_SemiStockInForm" runat="server">
            <div class="StagePanel">
                <asp:Table ID="ControlToolsTab" runat="server" BorderWidth="0" CellPadding="0" CellSpacing="0"
                    Width="100%">
                    <asp:TableRow>
                        <asp:TableCell CssClass="ControlTools">
                            <asp:LinkButton ID="LB_ModifyStock" runat="server" >
                                <asp:Image ID="IB_ModifyStock" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                                    BorderStyle="None" />提交
                            </asp:LinkButton>
                            <Ibt:ImageButtonControl ID="LB_Cancel" runat="server" OnClientClick="return CancelWindow()"
                                SkinType="Cancel" Text="取消"></Ibt:ImageButtonControl>
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
                <table class="PanelArea">
                    <tr>
                        <td>单据编号：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_TradeCode" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox>
                        </td>
                        <td>
                            选择采购单：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_Purchasing" runat="server" Width="250px" >
                            </rad:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td>申请时间：</td>
                        <td><asp:TextBox ID="TextBox1" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox></td>
                        <td>操 作 人：</td>
                        <td><asp:TextBox ID="TextBox2" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox></td>
                    </tr>
                    <tr>
                        <td>单位名称：
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox3" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox>
                        </td>
                        <td>收货公司：
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox4" runat="server" SkinID="LongInput" ReadOnly="true"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                       
                      
                          <td>入库仓储：
                        </td>
                        <td>
                            <rad:RadComboBox ID="RCB_InStock" runat="server" Width="123px" >
                            </rad:RadComboBox>
                            <rad:RadComboBox ID="RadComboBox2" runat="server" Width="122px" >
                            </rad:RadComboBox>
                        </td>
                         <td>物流配送公司：
                        </td>
                        <td>
                              <rad:RadComboBox ID="RadComboBox1" runat="server" 
                                Width="250px">
                            </rad:RadComboBox>
                        </td>
                    </tr>
                    <tr>
                        <td>采购责任人：
                        </td>
                        <td>
                            <asp:TextBox ID="TextBox5" runat="server" ReadOnly="true" SkinID="LongInput"></asp:TextBox>
                        </td>
                        <td>原始编号：
                        </td>
                        <td>
                            <asp:TextBox ID="TB_OriginalCode" runat="server" SkinID="LongInput"></asp:TextBox>
                        </td>
                    </tr>
                    <tr>
                        <td>备注说明：
                        </td>
                        <td colspan="3">
                            <asp:TextBox ID="TB_Description" runat="server" TextMode="MultiLine" Width="74%" Height="30px"></asp:TextBox>
                        </td>
                    </tr>
                </table>
                <rad:RadGrid ID="RGGoods" AllowPaging="False" Height="300px" runat="server" SkinID="Common"
                    OnNeedDataSource="RgGoodsNeedDataSource">
                    <ClientSettings>
                        <Resizing AllowColumnResize="True"></Resizing>
                        <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                        <ClientMessages DragToResize="调整大小" />
                        <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                    <MasterTableView ClientDataKeyNames="RealGoodsId" DataKeyNames="GoodsId,RealGoodsId,UnitPrice,GoodsType,EffectiveDate,RegistrationNumber,ProductionPermitNo,ProductionUnit">
                        <CommandItemTemplate>
                            <asp:LinkButton ID="lbtnSelectGoods" OnClientClick="return ShowObject('GoodsPanel');" runat="server">
                                <asp:Image ID="AddRecord" runat="server" ImageAlign="AbsMiddle" SkinID="AddImageButton" />
                                选择商品
                            </asp:LinkButton>&nbsp;&nbsp;&nbsp;
                        <asp:LinkButton ID="LinkButtonRefresh" runat="server" CommandName="RebindGrid">
                            <asp:Image ID="Refresh" runat="server" ImageAlign="AbsMiddle" SkinID="RefreshImageButton" />刷新
                        </asp:LinkButton>
                        </CommandItemTemplate>
                        <CommandItemStyle HorizontalAlign="Right" Height="24px" />
                        <Columns>
                            <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode" Visible="False">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                                <HeaderStyle Width="150px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn HeaderText="缩图">
                                <ItemTemplate>
                                    <asp:Image ID="imgShowPic" runat="server" SkinID="ShowPicImg" CssClass="ImageBtnLabel"
                                        onclick='<%# "ShowGoodsImg(\"" + Container.FindControl("imgdiv").ClientID + "\",\""+Eval("GoodsId")+"\")" %>'
                                        ToolTip="单击会在右边显示图片" />
                                    <div style="position: absolute;">
                                        <div id="imgdiv" class="imgclass" runat="server">
                                        </div>
                                    </div>
                                </ItemTemplate>
                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" Width="40px" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="Specification" HeaderText="SKU" UniqueName="Specification">
                                <HeaderStyle Width="90px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridBoundColumn DataField="Units" HeaderText="计量单位" UniqueName="Units">
                                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="单价" UniqueName="UnitPrice">
                                <ItemTemplate>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Quantity" HeaderText="数量" UniqueName="Quantity">
                                <ItemTemplate>
                                    <asp:TextBox ID="TB_Quantity" runat="server" Text='<%# Bind("Quantity") %>' SkinID="ShortInput"
                                        onblur='<%# "SumPrice(\"" + Container.FindControl("TB_Quantity").ClientID + "\",\"" + Container.FindControl("TB_UnitPrice").ClientID + "\")" %>'>
                                    </asp:TextBox>
                                    <asp:RequiredFieldValidator ID="RFVQuantity" runat="server" ControlToValidate="TB_Quantity"
                                        ErrorMessage="数量必须填写" Text="*">
                                    </asp:RequiredFieldValidator>
                                    <asp:RegularExpressionValidator ID="REVnums" runat="server" ControlToValidate="TB_Quantity"
                                        ValidationExpression="^\d+$" ErrorMessage="*">
                                    </asp:RegularExpressionValidator>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="生产单位">
                                <ItemTemplate>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="110px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" Width="110px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="许可证号">
                                <ItemTemplate>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="110px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" Width="110px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="注册证号">
                                <ItemTemplate>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="110px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" Width="110px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="有效日期">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt_EffectiveDate" runat="server" Text='<%#Convert.ToDateTime(Eval("EffectiveDate")).ToString("yyyy-MM-dd").Equals("1900-01-01")?"":Convert.ToDateTime(Eval("EffectiveDate")).ToString("yyyy-MM-dd")%>' Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="85px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" Width="85px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="批次号">
                                <ItemTemplate>
                                    <asp:TextBox ID="txt_BatchNo" runat="server" Text='<%# Eval("BatchNo") %>' Width="90px" onclick="CheckEffectiveDate(this);"></asp:TextBox>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" Width="110px"></HeaderStyle>
                                <ItemStyle HorizontalAlign="Center" Width="110px" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn HeaderText="备注">
                                <HeaderStyle Width="100px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%# Eval("Description")%>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="UnitPrice" HeaderText="是否赠品" UniqueName="type">
                                <ItemTemplate>
                                    <%# Convert.ToDecimal(Eval("UnitPrice"))==0?"赠品":"" %>
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="ApprovalNO" HeaderText="批文号" UniqueName="ApprovalNO">
                                <ItemTemplate>
                                    <asp:Label ID="lbApprovalNO" runat="server" Text='<%# Bind("ApprovalNO") %>'></asp:Label>
                                </ItemTemplate>
                                <ItemStyle HorizontalAlign="Center" />
                                <HeaderStyle Width="120px" HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn UniqueName="Delete">
                                <ItemTemplate>
                                    <asp:ImageButton ID="ImageButton1" SkinID="DeleteImageButton" runat="server" CommandName="Delete" />
                                </ItemTemplate>
                                <HeaderStyle Width="35px" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="GoodsId" HeaderText="GoodsId" UniqueName="GoodsId"
                                Visible="False">
                            </rad:GridBoundColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
                
            </div>
        </asp:Panel>
        <div id="GoodsPanel" style="background-color: #FFFFFF; width: 100%; height: 205px; left: 0; position: absolute; top: 0; z-index: -1; visibility: hidden">
            <table width="100%">
                <tr>
                    <td style="width: 90px; height: 20px; text-align: right;">选择分类：
                    </td>
                    <td style="width: 230px;">
                        <rad:RadComboBox ID="RCB_GoodsClass" runat="server" 
                            Width="220px" Height="200px">
                        </rad:RadComboBox>
                    </td>
                    <td style="width: 90px; text-align: right;">编号搜索：
                    </td>
                    <td style="width: 230px;">
                        <rad:RadComboBox ID="RCB_Goods" runat="server" 
                            Width="220px" Height="200px">
                        </rad:RadComboBox>
                    </td>
                    <td>
                        <asp:CheckBox ID="cbIncludeUnsale" runat="server" Text="是否赠品" />
                    </td>
                    <td style="text-align: right;">
                        <asp:Button ID="Button_SelectGoods" Text="添加商品" CssClass="Button" 
                            CausesValidation="false" runat="server" />&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                    <input id="CloseGoodsPanel" type="button" value="关闭添加" class="Button" onclick="return HiddenObject('GoodsPanel');" />
                    </td>
                </tr>
            </table>
            <rad:RadGrid runat="server" ID="RGSelectGoods" AutoGenerateColumns="true" AllowPaging="false"
                Height="170px" SkinID="Common" OnNeedDataSource="RgSelectGoodsNeedDataSource"
                MasterTableView-CommandItemDisplay="None" >
                <ClientSettings>
                    <Scrolling AllowScroll="True" UseStaticHeaders="True" />
                </ClientSettings>
                <MasterTableView DataKeyNames="GoodsId,IsRealGoods">
                    <CommandItemStyle HorizontalAlign="Right" Height="0px" />
                    <Columns>
                        <rad:GridTemplateColumn UniqueName="CheckBoxColumn">
                            <ItemTemplate>
                                <asp:CheckBox ID="CheckGoods" runat="server" />
                                <asp:HiddenField ID="ItemFieldValue" runat="server" />
                            </ItemTemplate>
                            <HeaderStyle Width="30px" />
                            <ItemStyle HorizontalAlign="Center" />
                        </rad:GridTemplateColumn>
                    </Columns>
                </MasterTableView>
            </rad:RadGrid>
        </div>
        <rad:RadAjaxManager ID="RAM" runat="server" useembeddedscripts="false">
            
        </rad:RadAjaxManager>
        <rad:RadAjaxLoadingPanel ID="Loading" runat="server" Skin="WebBlue">
        </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

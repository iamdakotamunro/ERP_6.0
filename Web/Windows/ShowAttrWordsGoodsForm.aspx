<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ShowAttrWordsGoodsForm.aspx.cs" Inherits="ERP.UI.Web.Windows.ShowAttrWordsGoodsForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server" AsyncPostBackTimeout="360"></rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/jquery.js" type="text/javascript"></script>
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script src="../JavaScript/common.js" type="text/javascript"></script>
    </rad:RadScriptBlock>

    <div style="padding: 5px;">
        <rad:RadGrid ID="GridAttrWordsGoods" runat="server" SkinID="CustomPaging" OnNeedDataSource="GridAttrWordsGoods_NeedDataSource"
                OnItemDataBound="GridAttrWordsGoods_ItemDataBound" 
                OnItemCommand="GridAttrWordsGoods_ItemCommand">
            <MasterTableView  DataKeyNames="GoodsId" CommandItemDisplay="None">
                <Columns>
                    <rad:GridBoundColumn DataField="GoodsCode" HeaderText="编号" UniqueName="GoodsCode">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GoodsName" HeaderText="商品名称" UniqueName="GoodsName">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="属性名称">
                        <ItemTemplate>
                            <rad:RadComboBox ID="RCB_WordName" DataSource='<%# Eval("WordIdAndWordName") %>' DataValueField="Key" DataTextField="Value" runat="server" ></rad:RadComboBox>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="属性名称">
                        <ItemTemplate>
                            <rad:RadComboBox ID="RCB_MoreWordName" ShowToggleImage="True" EnableLoadOnDemand="False" DataValueField="Key" DataTextField="Value" runat="server">
                                <ItemTemplate>
                                    <asp:CheckBox ID="cbbox" Text='<%#Eval("Value")??"" %>' ToolTip='<%#Eval("Key")??"" %>' runat="server" />
                                </ItemTemplate>
                            </rad:RadComboBox>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="属性名称">
                        <ItemTemplate>
                            <asp:TextBox ID="tbValue" Text='<%# Eval("Value") %>' runat="server"></asp:TextBox>
                            <asp:RequiredFieldValidator ID="RFVValue" ControlToValidate="tbValue"
                                        ErrorMessage="属性名称不允许为空！" Text="*" runat="server"></asp:RequiredFieldValidator>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridTemplateColumn HeaderText="操作">
                        <ItemTemplate>
                            <asp:LinkButton ID="LB_Inster" runat="server" CommandName="Submit">
                                <asp:Image ID="IB_Inster" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle" BorderStyle="None" />确定
                            </asp:LinkButton>
                        </ItemTemplate>
                        <HeaderStyle Width="150px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
            </MasterTableView>
        </rad:RadGrid>
    </div>

    <rad:RadAjaxManager ID="RAM" runat="server">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="GridAttrWordsGoods">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="GridAttrWordsGoods" LoadingPanelID="loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

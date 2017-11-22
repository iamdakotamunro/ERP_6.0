<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EditDebitNoteForm.aspx.cs" Inherits="ERP.UI.Web.Windows.EditDebitNoteForm" %>
<%@ Import Namespace="ERP.Enum" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script src="../JavaScript/telerik.js" type="text/javascript"></script>
        <script type="text/javascript" language="javascript">
            function ShowImg(obj) {
                var object = eval(obj);
                object.style.display = "block";
            }
            function HiddleImg(obj) {
                var object = eval(obj);
                object.style.display = "none";
            }
        </script>
    </rad:RadScriptBlock>
    <div style="margin: 0; padding-top: 10px; width: 100%;">
        <table class="StagePanelHead" border="0" style="width: 100%;">
            <tr>
                <td>
                    <table cellspacing="0" cellpadding="0" border="0" style="width: 100%; border-collapse: collapse;">
                        <tr>
                            <td>
                            </td>
                        </tr>
                    </table>
                </td>
            </tr>
        </table>
        <table class="PanelArea" height="38">
            <tr>
                <td align="center">
                    <input id="btnCancel" value="关闭" onclick="return CancelWindow()" style="width: 70px;height: 24px;" type="button"/>
                </td>
            </tr>
        </table>
        <rad:RadGrid ID="RG_DebitNoteDetail" OnNeedDataSource="RgDebitNoteDetail_OnNeedDataSource"
            AllowMultiRowSelection="true" runat="server" AllowPaging="true" PageSize="40"
            Width="100%" Height="550px">
            <GroupingSettings GroupContinuesFormatString="组将继续在下一页上。" GroupContinuedFormatString="...组继续从以前的页面。"
                GroupSplitDisplayFormat="显示 {0} 到 {1} 项。" ExpandTooltip="展开组" CollapseTooltip="关闭组"
                UnGroupTooltip="拖动出区域取消组合" UnGroupButtonTooltip="点击这里取消组合"></GroupingSettings>
            <ClientSettings>
                <Scrolling AllowScroll="True" UseStaticHeaders="true" />
                <Selecting EnableDragToSelectRows="false" />
                <Selecting AllowRowSelect="true" UseClientSelectColumnOnly="true" />
                <ClientMessages DragToGroupOrReorder="拖动到组，重新排序" DragToResize="拖动以调整" PagerTooltipFormatString="第&lt;strong&gt;{0}&lt;/strong&gt; 页，共 &lt;strong&gt;{1}&lt;/strong&gt;页"
                    ColumnResizeTooltipFormatString="宽度: &lt;strong&gt;{0}&lt;/strong&gt; &lt;em&gt;像素&lt;/em&gt;">
                </ClientMessages>
            </ClientSettings>
            <PagerStyle FirstPageToolTip="首页" NextPageToolTip="下一页" LastPageToolTip="尾页" PrevPageToolTip="上一页"
                NextPagesToolTip="下一组" PrevPagesToolTip="上一组" PageSizeLabelText="单页行数:" PagerTextFormat="{4}当前记录&lt;strong&gt;{2}&lt;/strong&gt;至&lt;strong&gt;{3},&lt;/strong&gt;共&lt;strong&gt;{5}&lt;/strong&gt;条记录.">
            </PagerStyle>
            <MasterTableView DataKeyNames="Id" ClientDataKeyNames="Id" CommandItemDisplay="None"
                Width="100%">
                <PagerStyle FirstPageToolTip="首页" NextPageToolTip="下一页" LastPageToolTip="尾页" PrevPageToolTip="上一页"
                    NextPagesToolTip="下一组" PrevPagesToolTip="上一组" PageSizeLabelText="单页行数:" PagerTextFormat="{4}当前记录&lt;strong&gt;{2}&lt;/strong&gt;至&lt;strong&gt;{3},&lt;/strong&gt;共&lt;strong&gt;{5}&lt;/strong&gt;条记录.">
                </PagerStyle>
                <CommandItemStyle HorizontalAlign="right" Height="25px" />
                <RowIndicatorColumn FilterImageToolTip="过滤器">
                </RowIndicatorColumn>
                <ExpandCollapseColumn FilterImageToolTip="过滤器">
                </ExpandCollapseColumn>
                <Columns>
                    <%--<rad:GridClientSelectColumn UniqueName="column">
                        <HeaderStyle Width="40px" HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridClientSelectColumn>--%>
                    <rad:GridBoundColumn DataField="Id" HeaderText="Id" UniqueName="Id" Visible="False">
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="PurchasingId" HeaderText="PurchasingId" UniqueName="PurchasingId"
                        Visible="False">
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Specification" HeaderText="赠品SKU" UniqueName="Specification">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="GivingCount" HeaderText="赠品数量" UniqueName="GivingCount">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="ArrivalCount" HeaderText="实到数量" UniqueName="ArrivalCount">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridBoundColumn DataField="Price" HeaderText="价格" UniqueName="Price">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="状态">
                        <ItemTemplate>
                            <%# ((int)Eval("State") == (int)YesOrNo.No ? "未完成" : "已完成")%>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                    <rad:GridBoundColumn DataField="Amount" HeaderText="金额" UniqueName="Amount">
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridBoundColumn>
                    <rad:GridTemplateColumn HeaderText="备注">
                        <ItemTemplate>
                            <asp:ImageButton ID="ClewImageButton" CommandName="Clew" runat="server" SkinID="InsertImageButton"
                                onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                                onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                            <div style="position: absolute;">
                                <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative;
                                    display: none; background-color: #CCFFFF; border: solid 1px #666; width: 200px;
                                    font-weight: bold; height: auto; overflow: visible; word-break: break-all;" runat="server">
                                    <%# Eval("Memo")%>
                                </div>
                            </div>
                        </ItemTemplate>
                        <HeaderStyle HorizontalAlign="Center" />
                        <ItemStyle HorizontalAlign="Center" />
                    </rad:GridTemplateColumn>
                </Columns>
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
            </MasterTableView>
            <SortingSettings SortToolTip="点击这里进行排序" SortedAscToolTip="递增排序" SortedDescToolTip="递减排序">
            </SortingSettings>
            <StatusBarSettings ReadyText="准备" LoadingText="载入中..."></StatusBarSettings>
            <HierarchySettings ExpandTooltip="展开" CollapseTooltip="收起"></HierarchySettings>
        </rad:RadGrid>
    </div>

    <rad:RadAjaxManager runat="server" ID="RAM" OnAjaxRequest="Ram_AjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RG_DebitNoteDetail">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG_DebitNoteDetail" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue"></rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

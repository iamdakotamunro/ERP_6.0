<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MainMaster.master"
    CodeBehind="ActivityFiling.aspx.cs" Inherits="ERP.UI.Web.ActivityFiling" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="JavaScript/telerik.js"></script>
        <script type="text/javascript">
            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
            function ShowAddForm() {
                window.radopen("./Windows/AddActivityFiling.aspx?parm=0", "RW1");
                return false;
            }

            function AF_RowDblClick(obj, args) {
                var state = args.getDataKeyValue("ActivityFilingState");
                if (state == 5) {
                    return false;
                }
                var id = args.getDataKeyValue("ID");
                window.radopen("./Windows/AddActivityFiling.aspx?parm=" + state + "&id=" + id, "RW1");
                return false;
            }

            function onDelete() {
                var conf = window.confirm('提示：是否确认删除吗？');
                if (!conf)
                    return false;
                return true;
            }

            function onAduit(type, id) {
                var win = window.radopen("./Windows/AddActivityFiling.aspx?parm=" + type + "&id=" + id, "RW1");
                win.center();
            }

            function onUpdate(type, id) {
                var win = window.radopen("./Windows/AddActivityFiling.aspx?IsUpdate=1&parm=" + type + "&id=" + id, "RW1");
                win.center();
            }

            function openWindow(id) {
                var win = window.radopen("./Windows/AddActityFilingLog.aspx?id=" + id, "RW2");
                win.center();
            }
        </script>
    </rad:RadScriptBlock>
    <div>
        <table>
            <tr>
                <td>
                    活动标题：
                    <asp:TextBox runat="server" ID="activityTitle" Width="250"></asp:TextBox>
                </td>
                <td>
                    活动商品：
                    <asp:TextBox runat="server" ID="activityGoodsName" Width="250"></asp:TextBox>
                </td>
                <td>
                    起止时间
                    <rad:RadDatePicker runat="server" ID="startDateTime" >
                    </rad:RadDatePicker>
                    -
                    <rad:RadDatePicker runat="server" ID="endDateTime" >
                    </rad:RadDatePicker>
                </td>
            </tr>
        </table>
        <table>
            <tr>
                <td>
                    申报公司：
                    <rad:RadComboBox ID="RadSaleTerrace" runat="server" Width="173" MaxHeight="200">
                    </rad:RadComboBox>
                </td>
                <td>
                    申报状态：
                    <rad:RadComboBox ID="RadFilingState" runat="server" Width="173">
                    </rad:RadComboBox>
                </td>
                <td>
                    <asp:ImageButton ID="AF_SearchImageButton" runat="server" SkinID="SearchButton"  OnClick="Search_ActivityFiling" />
                </td>
                <td>
                </td>
                <td>
                    <asp:Button runat="server" Text="新建" OnClientClick="return ShowAddForm()" />
                </td>
            </tr>
        </table>
    </div>
    <rad:RadGrid ID="ActivityFilingRad" runat="server" ShowFooter="True" OnNeedDataSource="FilingDataSource"
        SkinID="CustomPaging" OnItemCommand="ActivityFiling_OnItemCommand" >
        <ClientSettings>
            <ClientEvents OnRowDblClick="AF_RowDblClick"></ClientEvents>
        </ClientSettings>
        <MasterTableView DataKeyNames="ID,ActivityFilingState" ClientDataKeyNames="ID,ActivityFilingState">
            <CommandItemTemplate>
            </CommandItemTemplate>
            <Columns>
                <rad:GridTemplateColumn HeaderText="申请时间" DataField="CreateDate" UniqueName="CreateDate">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<%#Eval("CreateDate") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报公司" DataField="FilingCompanyName" UniqueName="FilingCompanyName">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label runat="server" Text='<% #Eval("FilingCompanyName") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报平台" DataField="FilingTerraceName" UniqueName="FilingTerraceName">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%#Eval("FilingTerraceName") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报人" DataField="OperatePersonnelName" UniqueName="OperatePersonnelName">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%#Eval("OperatePersonnelName") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="申报状态" DataField="ActivityFilingState" UniqueName="ActivityFilingState">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label3" runat="server" Text='<%#Eval("ActivityFilingStateName") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="开始时间" DataField="ActivityStateDate" UniqueName="ActivityStateDate">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label4" runat="server" Text='<%#Convert.ToDateTime(Eval("ActivityStateDate")).ToString("yyyy-MM-dd") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="结束时间" DataField="ActivityEndDate" UniqueName="ActivityEndDate">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label5" runat="server" Text='<%#Convert.ToDateTime(Eval("ActivityEndDate")).ToString("yyyy-MM-dd") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="活动商品" DataField="GoodsName" UniqueName="GoodsName">
                    <HeaderStyle HorizontalAlign="Center" Width="200" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label6" runat="server" Text='<%#Eval("GoodsName") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="预估销量" DataField="ProspectSaleNumber" UniqueName="ProspectSaleNumber">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label7" runat="server" Text='<%#Eval("ProspectSaleNumber") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="正常销量" DataField="NormalSaleNumber" UniqueName="NormalSaleNumber">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label18" runat="server" Text='<%#Eval("NormalSaleNumber") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="采购员" DataField="PurchasePersonnelName" UniqueName="PurchasePersonnelName">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label8" runat="server" Text='<%#Eval("PurchasePersonnelName") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="实际销量" DataField="ActualSaleNumber" UniqueName="ActualSaleNumber">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label9" runat="server" Text='<%#Eval("ActualSaleNumber") %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="误差率" DataField="ErrorProbability" UniqueName="ErrorProbability">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:Label ID="Label10" runat="server" Text='<%# Convert.ToDecimal(Eval("ErrorProbability"))==0?(0+""):Eval("ErrorProbability")+"%" %>'></asp:Label>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作记录">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="AddImageButton" OnClientClick='<%# "openWindow(\"" + Eval("ID") + "\");return false;" %>' />
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="修改" UniqueName="Update">
                    <HeaderStyle HorizontalAlign="Center"></HeaderStyle>
                    <ItemStyle HorizontalAlign="Center"></ItemStyle>
                    <ItemTemplate>
                        <asp:ImageButton runat="server" ID="UpdateImgBtn" SkinID="EditImageButton" OnClientClick='<%# "onUpdate(\""+Eval("ActivityFilingState")+"\",\""+Eval("ID")+"\");return false;"  %>'
                        Visible='<%# Convert.ToInt32(Eval("ActivityFilingState"))==2 || Convert.ToInt32(Eval("ActivityFilingState"))==1 || Convert.ToInt32(Eval("ActivityFilingState"))==4 %>'/>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="审核" UniqueName="Auditing">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <asp:ImageButton runat="server" ID="AuditImgBtn" SkinID="AffirmImageButton" OnClientClick='<%# "onAduit(\""+Eval("ActivityFilingState")+"\",\""+Eval("ID")+"\");return false;"  %>'
                            Visible='<%# Convert.ToInt32(Eval("ActivityFilingState"))==2 %>' />
                        <%--<asp:ImageButton runat="server" SkinID="UpdateImageButton" CommandName="Update"/>--%>
                        <%--<asp:ImageButton runat="server" SkinID="DeleteImageButton" CommandName="Delete"/>--%>
                    </ItemTemplate>
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="删除" UniqueName="Delete">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                    <ItemTemplate>
                        <%--<asp:ImageButton runat="server" ID="AuditImgBtn" SkinID="InsertImageButton" CommandName="Audit"/>--%>
                        <%--<asp:ImageButton runat="server" SkinID="UpdateImageButton" CommandName="Update"/>--%>
                        <asp:ImageButton ID="ImageButton1" runat="server" CommandName="Delete" Visible='<%# (Convert.ToInt32(Eval("ActivityFilingState"))!=3) && (Convert.ToInt32(Eval("ActivityFilingState"))!=5) %>'
                            SkinID="DeleteImageButton" OnClientClick="return onDelete()" />
                    </ItemTemplate>
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="RWM" runat="server">
        <Windows>
            <rad:RadWindow ID="RW1" Width="800" Height="400" runat="server" Title="" />
        </Windows>
        <Windows>
            <rad:RadWindow ID="RW2" Width="650" Height="400" runat="server" Title="操作日志" />
        </Windows>
    </rad:RadWindowManager>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RAM_OnAjaxRequest">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="ActivityFilingRad" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="AF_SearchImageButton">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="ActivityFilingRad" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="ActivityFilingRad">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="ActivityFilingRad" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
</asp:Content>

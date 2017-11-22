<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="Information.aspx.cs" Inherits="ERP.UI.Web.Information" %>
<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="System.Drawing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript">
            function ShowInformationForm(id, type) {
                if (type == "供应商" || type=="" || type=="1") {
                    window.radopen("./Windows/CompanyInfomation.aspx?CompanyId=" + id, "RW2");
                } else {
                    window.radopen("./Windows/GoodsInfomation.aspx?GoodsId=" + id, "RW2");
                }
            }
            function ShowPicWindow(path) {
                window.radopen("./Windows/ShowImg.aspx?path=" + path + "", "RW1");
            }

            function ShowFileWindow(path) {
                window.radopen("./Windows/DownloadPage.aspx?fullname=" + path + "", "RW1");
            }

            function refreshGrid(arg) {
                if (!arg) {
                    $find("<%=RAM.ClientID %>").ajaxRequest('Rebind');
                }
                else {
                    $find("<%=RAM.ClientID %>").ajaxRequest('RebindAndNavigate');
                }
            }
        </script>
    </rad:RadScriptBlock>

    <table class="StagePanel">
        <tr>
            <td style="vertical-align: top;">
                <table style="width:100%">
                    <tr>
                        <td style="width:200px; padding:0">
                            资质类型：
                            <rad:RadComboBox ID="RCB_SearchType" Width="120" AutoPostBack="true" OnSelectedIndexChanged="SearchTypeIndexChanged" runat="server">
                                <Items>
                                    <rad:RadComboBoxItem Text="供应商" Value="1" />
                                    <rad:RadComboBoxItem Text="商品" Value="3" />
                                </Items>
                            </rad:RadComboBox>
                        </td>
                        <td style="width:200px; padding:0">
                            <rad:RadComboBox ID="RCB_SelectKey" runat="server" AutoPostBack="true" CausesValidation="false"
                                AllowCustomText="True" EnableLoadOnDemand="True" DataTextField="Value" DataValueField="Key"
                                Width="220px" Height="200px" OnItemsRequested="SearchItemsRequested" 
                                 EmptyMessage="输入搜索关键字">
                            </rad:RadComboBox>
                        </td>
                        <td style="width: 80px; text-align: right;">证书号码：</td>
                        <td style="width:150px; padding:0">
                            <asp:TextBox ID="TB_CertificateNumber" runat="server" SkinID="StandardInput"></asp:TextBox>
                        </td>
                        <td style="width:70px; padding:0;text-align: right;">
                            状态：
                        </td>
                        <td style="width:200px; padding:0">
                            <rad:RadComboBox ID="DDL_HaveInformation" runat="server"  >
                            </rad:RadComboBox>
                        </td>
                        <td>
                            <asp:Button ID="btn_Search" Width="70px" runat="server" Text="搜索" OnClick="btn_Search_Click" />
                        </td>
                    </tr>
                    <tr>
                        <td></td><td></td>
                        <td colspan="2" style="text-align: right;">
                            <div id="divFiliale" runat="server">
                            公司名称：
                            <asp:DropDownList ID="DDL_Filiale" runat="server" Width="150px">
                            </asp:DropDownList>
                            </div>
                        </td>
                        <td style="text-align: right;">
                            有效期：
                        </td>
                        <td>
                            <rad:RadComboBox ID="DDL_Period" runat="server">
                            </rad:RadComboBox>
                        </td>
                        <td><asp:ImageButton ID="IB_ExportData" Width="70px" runat="server" SkinID="ExportData" OnClick="Ib_ExportData_Click" /></td>
                    </tr>
                </table>

                <rad:RadGrid ID="InformationsGrid" SkinID="CustomPaging" OnNeedDataSource="InformationsGrid_NeedDataSource"
                     AllowMultiRowSelection="True" runat="server">
                    <MasterTableView DataKeyNames="ID" ClientDataKeyNames="ID" CommandItemDisplay="None">
                        <Columns>
                            <rad:GridTemplateColumn>
                                <HeaderTemplate>
                                    <asp:CheckBox runat="server" ID="CkAll" Checked="<%# IsAll %>" AutoPostBack="True" OnCheckedChanged="CkAllOnCheckedChanged" />
                                </HeaderTemplate>
                                <HeaderStyle Width="40px" HorizontalAlign="Center" />
                                <ItemStyle Width="40px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <asp:CheckBox runat="server" ID="CkSelected"  AutoPostBack="True" 
                                        Checked='<%# (IsAll && !UnSelectedIds.Contains(new Guid(Eval("ID").ToString())))
                                    || !IsAll && SelectedIds.Contains(new Guid(Eval("ID").ToString())) %>' 
                                         OnCheckedChanged="CkSelectedChanged" />
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Type" HeaderText="资质类型" UniqueName="Type">
                                <HeaderStyle Width="95px" HorizontalAlign="Center" />
                                <ItemStyle Width="95px" HorizontalAlign="Center" />
                                <ItemTemplate>
                                    <%# RCB_SearchType.SelectedItem.Text %>
                                </ItemTemplate>
                            </rad:GridTemplateColumn>
                            <rad:GridBoundColumn DataField="Name" HeaderText="商品/往来单位名称" UniqueName="Name">
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridBoundColumn>
                            <rad:GridTemplateColumn DataField="Situation" HeaderText="资质情况" UniqueName="Situation">
                                <ItemTemplate>
                                    <asp:HiddenField ID="HF_Id" Value='<%#Eval("ID") %>' runat="server" />
                                    <asp:Label ID="lbSituation" runat="server" Text='<%# Convert.ToInt32(Eval("Complete"))==(int)SupplierCompleteType.Complete ? "完整" : "不完整" %>' 
                                        ForeColor='<%# Convert.ToInt32(Eval("Complete"))==(int)SupplierCompleteType.Complete ? Color.Black : Color.Red %>'></asp:Label>
                                </ItemTemplate>
                                <HeaderStyle HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                            <rad:GridTemplateColumn DataField="Detail" HeaderText="资质明细" UniqueName="Detail">
                                <ItemTemplate>
                                    <asp:ImageButton runat="server" CausesValidation="false" ID="IB_Detail" SkinID="InsertImageButton" OnClientClick='<%# "return ShowInformationForm(\""+Eval("ID")+"\"" + "," + "\"" + Eval("Type") +"\")" %>'  />
                                </ItemTemplate>
                                <HeaderStyle Width="80px" HorizontalAlign="Center" />
                                <ItemStyle HorizontalAlign="Center" />
                            </rad:GridTemplateColumn>
                        </Columns>
                    </MasterTableView>
                </rad:RadGrid>
            </td>
        </tr>
    </table>
    <rad:RadAjaxManager ID="RAM" runat="server" OnAjaxRequest="RamGoodsAjaxRequest">
        <AjaxSettings>
            
            <rad:AjaxSetting AjaxControlID="RCB_SearchType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SelectKey" LoadingPanelID="loading" />
                    <rad:AjaxUpdatedControl ControlID="divFiliale" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SearchType">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="InformationsGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="InformationsGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            
            <rad:AjaxSetting AjaxControlID="LB_Refresh">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="InformationsGrid" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RAM">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="InformationsGrid"/>
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="InformationsGrid">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="InformationsGrid"/>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager ID="ClewWindowManager" runat="server" Height="380px" Width="380px"
        ReloadOnShow="true">
        <Windows>
            <rad:RadWindow ID="GoodsDialog" runat="server" Title="产品信息" />
            <rad:RadWindow ID="RW1" runat="server" BorderStyle="None" BorderWidth="0px" Title="资料图片信息" />
            <rad:RadWindow ID="RW2" runat="server" Width="1100" Height="700" Title="资质" />
        </Windows>
    </rad:RadWindowManager>
</asp:Content>

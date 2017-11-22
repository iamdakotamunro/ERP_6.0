<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true"
    CodeBehind="RefundsMoney.aspx.cs" Inherits="ERP.UI.Web.RefundsMoney.RefundsMoney" %>

<%@ Import Namespace="ERP.Enum" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <!--��ǰҳ�棺�˿���-->

    <style>
        .highlight {
            padding: 10px 0px;
            margin-bottom: 14px;
            background-color: #f7f7f9;
            border: 1px solid #e1e1e8;
            border-radius: 4px;
        }

        .input[type="radio"], input[type="checkbox"] {
            margin: 0;
        }

        .w_auto {
            margin-right: 5px;
        }

        .search {
            padding-left: 30px;
        }

        input, select, textarea {
            max-width: 400px;
        }

        .detail {
            padding-left: 20px;
        }

        .search_Condition .search {
            text-align: left;
        }

        label {
            font-weight: normal;
        }

        html:first-child .RadWindow ul {
            border: solid 0px transparent;
        }
    </style>

    <div class="body-content">

        <div class="search_Condition box_1k">

            <div class="highlight">

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="padding-top: 5px; text-align: right;">�����ţ�</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">
                        <rad:RadTextBox ID="RTB_OrderNumber" runat="server" EmptyMessage="�����š������������š��ۺ󶩵���" Width="200"></rad:RadTextBox>
                    </div>
                </div>

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="text-align: right; padding-top: 5px;">ʱ�䣺</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">

                        <rad:RadDatePicker ID="RDP_StartTime" runat="server" SkinID="Common" Width="100px">
                        </rad:RadDatePicker>
                        <rad:RadDatePicker ID="RDP_EndTime" runat="server" SkinID="Common" Width="100px">
                        </rad:RadDatePicker>
                    </div>
                </div>

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="text-align: right; padding-top: 5px;">״̬��</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">

                        <asp:DropDownList ID="ddl_Status" runat="server" Style="width: 120px;">
                        </asp:DropDownList>
                    </div>
                </div>

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="padding-top: 5px; text-align: right;">���۹�˾��</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">
                        <rad:RadComboBox ID="RCB_SaleFiliale" runat="server" UseEmbeddedScripts="false" Width="120px"
                            Height="100px" OnSelectedIndexChanged="RCB_SaleFiliale_OnSelectedIndexChanged"
                            AutoPostBack="True" EmptyMessage="��ѡ�����۹�˾">
                        </rad:RadComboBox>
                    </div>
                </div>

                <div class="form-group col-sm-3 has-feedback">
                    <label class="col-sm-3 control-label" style="text-align: right; padding-top: 5px;">����ƽ̨��</label>
                    <div class="col-sm-9 control-label" style="text-align: left;">
                        <rad:RadComboBox ID="RCB_SalePlatform" runat="server" UseEmbeddedScripts="false" Width="120px" Height="100px" EmptyMessage="��ѡ������ƽ̨">
                        </rad:RadComboBox>
                    </div>
                </div>

                <div class="search">
                    <asp:Button ID="btn_Search" CssClass="btn btn-success" runat="server" Text="��ѯ" Style="margin-left: 40px;" OnClick="btn_Search_Click" />
                </div>
            </div>
        </div>
    </div>

    <rad:RadGrid ID="RG" runat="server" SkinID="CustomPaging" OnNeedDataSource="RG_NeedDataSource">
        <MasterTableView DataKeyNames="Id" ClientDataKeyNames="Id" NoMasterRecordsText="�޿��ü�¼��"
            CommandItemDisplay="None">
            <Columns>
                <rad:GridTemplateColumn HeaderText="����ʱ��">
                    <ItemTemplate>
                        &nbsp;<%# DateTime.Parse(Eval("CreateTime").ToString())==DateTime.MinValue?"":DateTime.Parse(Eval("CreateTime").ToString()).ToString("yyyy-MM-dd HH:mm:ss") %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle HorizontalAlign="Center" Width="140px" />
                </rad:GridTemplateColumn>

                <rad:GridBoundColumn DataField="OrderNumber" HeaderText="������">
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle Width="140px" />
                </rad:GridBoundColumn>

                <rad:GridBoundColumn DataField="ThirdPartyOrderNumber" HeaderText="������������">
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle Width="140px" />
                </rad:GridBoundColumn>

                <rad:GridBoundColumn DataField="AfterSalesNumber" HeaderText="�˻�����">
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle Width="140px" />
                </rad:GridBoundColumn>

                <rad:GridTemplateColumn HeaderText="�˿���(Ԫ)">
                    <ItemTemplate>
                        <%# Convert.ToDecimal(Eval("RefundsAmount"))==0?"":ERP.UI.Web.Common.WebControl.NumberSeparator(Eval("RefundsAmount")) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn HeaderText="״̬">
                    <ItemTemplate>
                        <%# EnumAttribute.GetKeyName((ERP.Enum.RefundsMoney. RefundsMoneyStatusEnum)Eval("Status"))  %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle HorizontalAlign="Center" Width="100px" />
                </rad:GridTemplateColumn>

                <rad:GridBoundColumn DataField="CreateUser" HeaderText="�ύ��">
                    <HeaderStyle HorizontalAlign="Center" Width="100px" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>

                <rad:GridBoundColumn DataField="BankAccountNo" HeaderText="֧����/�����˻�">
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle Width="140px" />
                </rad:GridBoundColumn>

                <rad:GridTemplateColumn HeaderText="�������" UniqueName="Remark">
                    <ItemTemplate>
                        <asp:ImageButton ID="ImgBtn_Remark" runat="server" OnClientClick='<%# "RemarkForm(\"" + Eval("ID")+ "\",\"" + Eval("SalePlatformId")+ "\");return false;" %>'
                            SkinID="InsertImageButton" onmousemove='<%# "ShowImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>'
                            onmouseout='<%# "HiddleImg(\"" + Container.FindControl("ImaDiv1").ClientID + "\")" %>' />
                        <div style="position: absolute;">
                            <div id="ImaDiv1" style="z-index: 1000; left: -200px; top: 20px; position: relative; display: none; background-color: #CCFFFF; border: solid 1px #666; width: 250px; font-weight: bold; height: auto; overflow: visible; word-break: break-all;"
                                runat="server">
                                <%# Eval("Remark").ToString().Replace("\n","<br />")%>
                            </div>
                        </div>
                    </ItemTemplate>
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn DataField="SalePlatformId" HeaderText="����ƽ̨">
                    <ItemTemplate>
                        <asp:Label ID="Lab_SaleFilialeId" runat="server" Text='<%# GetSalePlatformName(Eval("SalePlatformId")) %>'></asp:Label>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="140px" />
                    <ItemStyle HorizontalAlign="Center" Width="140px" />
                </rad:GridTemplateColumn>

                <rad:GridTemplateColumn HeaderText="����" UniqueName="Delete">
                    <ItemTemplate>
                        <asp:ImageButton ID="LB_Process" runat="server" Text="����" SkinID="AffirmImageButton"
                            OnClientClick='<%# "return ProcessClick(\"" + Eval("ID")+ "\",\"" + Eval("SalePlatformId")+ "\")" %>'></asp:ImageButton>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" Width="50px" />
                    <ItemStyle HorizontalAlign="Center" Width="50px" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <asp:HiddenField ID="Hid_SelectedValue" runat="server" />
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="btn_Search">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RG" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RCB_SaleFiliale">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RCB_SalePlatform" LoadingPanelID="Loading"></rad:AjaxUpdatedControl>
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    <rad:RadWindowManager runat="server">
        <Windows>
            <rad:RadWindow ID="RW1" runat="server" Title="�˿���" Width="900" Height="540" />
            <rad:RadWindow ID="RW2" runat="server" Title="�������" Width="700" Height="500" />
        </Windows>
    </rad:RadWindowManager>

    <script src="/JavaScript/jquery.js"></script>
    <link href="/Styles/bootstrap.min.css" rel="stylesheet" />
    <script src="/JavaScript/bootstrap.min.js"></script>

    <script type="text/javascript">
        function ProcessClick(applyId, salePlatformId) {
            window.radopen("/RefundsMoney/RefundsMoney_Edit.aspx?ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW1");
        }

        function RemarkForm(applyId, salePlatformId) {
            window.radopen("/RefundsMoney/RefundsMoney_RemarkForm.aspx?ApplyId=" + applyId + "&SalePlatformId=" + salePlatformId, "RW2");
        }

        //���°�Grid�¼�
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }
        
        function ShowImg(obj) {
            var object = eval(obj);
            object.style.display = "block";
        }

        function HiddleImg(obj) {
            var object = eval(obj);
            object.style.display = "none";
        }
    </script>
</asp:Content>

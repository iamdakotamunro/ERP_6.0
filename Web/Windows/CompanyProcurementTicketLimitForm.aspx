<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CompanyProcurementTicketLimitForm.aspx.cs"
    Inherits="ERP.UI.Web.Windows.CompanyProcurementTicketLimitForm" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
    <rad:RadScriptManager ID="RSM" runat="server">
    </rad:RadScriptManager>
    <rad:RadScriptBlock ID="RSB" runat="server">
        <script type="text/javascript" src="../JavaScript/telerik.js"></script>
    </rad:RadScriptBlock>
    <span style="font-size: 12px; font-weight: bold; color: black;margin-left: 20px;" runat="server" id="span_FilialeName"></span>
    <span style="font-size: 12px; font-weight: bold; color: gray;margin-left: 20px;">注：收票额度输入后即刻保存。</span>
    <br/>
    <br/>
    <rad:RadGrid ID="RadGridCompanyProcurementTicketLimit" runat="server" SkinID="Common" 
        MasterTableView-CommandItemDisplay="None" AutoGenerateColumns="False" Skin="WebBlue">
        <%--<MasterTableView DataKeyNames="FilialeId,CompanyId" ClientDataKeyNames="FilialeId,CompanyId">
            <Columns>
                <rad:GridBoundColumn DataField="CompanyName" HeaderText="供应商" UniqueName="CompanyName">
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="收票额度" UniqueName="TakerTicketLimit">
                    <ItemTemplate>
                        <asp:TextBox ID="TB_TakerTicketLimit" Width="150px" SkinID="ShortInput" runat="server"
                            Text='<%#Eval("TakerTicketLimit") %>' OnTextChanged="OnTextChanged_TakerTicketLimit"
                            AutoPostBack="True"  onkeydown="if(event.keyCode==13)return false;" BorderStyle="Groove" Enabled='<%#CheckIsCanSave()%>' ></asp:TextBox>
                    </ItemTemplate>
                    <HeaderStyle Width="220px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>--%>
    </rad:RadGrid>
    <rad:RadAjaxManager ID="RAM" runat="server" DefaultLoadingPanelID="loading">
        <AjaxSettings>
            <rad:AjaxSetting AjaxControlID="BT_Save">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="BT_NoPass">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="DIV_AuditingPanel" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
            <rad:AjaxSetting AjaxControlID="RadGridCompanyProcurementTicketLimit">
                <UpdatedControls>
                    <rad:AjaxUpdatedControl ControlID="RadGridCompanyProcurementTicketLimit" LoadingPanelID="loading" />
                </UpdatedControls>
            </rad:AjaxSetting>
        </AjaxSettings>
    </rad:RadAjaxManager>
    <rad:RadAjaxLoadingPanel ID="loading" runat="server" Skin="WebBlue">
    </rad:RadAjaxLoadingPanel>
    </form>
</body>
</html>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_DepositRecoveryUpdateReportPersonnel.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_DepositRecoveryUpdateReportPersonnel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
</head>
<body style="overflow: hidden;margin-top: 5px;">
    <form id="form1" runat="server">
        <rad:RadScriptManager runat="server" ID="RadScriptManager1" />
        <table style="width: 100%; line-height: 20px;">
           
            <tr>
                <td style="width: 64px; text-align: right;">申请人：
                </td>
                <td>
                    <asp:HiddenField ID="hf_oldReportPersonnel" runat="server"/>
                    <asp:TextBox ID="txt_oldReportPersonnel" runat="server"></asp:TextBox>
                    <span style="color: red">*</span>
                </td>
                <td style="text-align: right;">转派他人：
                </td>
                <td>
                    <asp:HiddenField ID="hf_newReportPersonnel" runat="server" />
                    <asp:TextBox ID="txt_newReportPersonnel" runat="server" ></asp:TextBox>
                     <span style="color: red">*</span>
                </td>
            </tr>
            <tr>
                <td colspan="4">
                    <rad:RadGrid ID="RG_ReportDepositRecovery" runat="server" OnNeedDataSource="RG_Report_NeedDataSource" PageSize="10">
                        <ClientSettings>
                        </ClientSettings>
                        <MasterTableView DataKeyNames="ReportId" ClientDataKeyNames="ReportId">
                            <CommandItemTemplate>
                            </CommandItemTemplate>
                            <CommandItemStyle Height="0px" />
                            <Columns>
                                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox onclick=checkAll()&gt;全选">
                                <ItemTemplate>
                                    <input title="请选择" type="checkbox" name="ckId" value='<%# Eval("ReportId")%>' />选择
                                </ItemTemplate>
                                <HeaderStyle Width="60px" HorizontalAlign="Center" />
                                <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                            </rad:GridTemplateColumn>

                                 <rad:GridTemplateColumn HeaderText="申报单号">
                                    <ItemTemplate>
                                        <%# Eval("ReportNo") %>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="90px" HorizontalAlign="Center" />
                                </rad:GridTemplateColumn>
                                <rad:GridTemplateColumn HeaderText="公司">
                                    <ItemTemplate>
                                        <%# GetPersonnelFiliale(Eval("ReportPersonnelId").ToString()) %>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                                </rad:GridTemplateColumn>
                                 <rad:GridTemplateColumn HeaderText="收款银行">
                                    <ItemTemplate>
                                        <%# Eval("BankAccountName").ToString() %>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                                </rad:GridTemplateColumn>
                                 <rad:GridTemplateColumn HeaderText="费用名称">
                                    <ItemTemplate>
                                        <%# Eval("ReportName").ToString() %>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                                </rad:GridTemplateColumn>
                                <rad:GridTemplateColumn HeaderText="付款金额">
                                    <ItemTemplate>
                                        <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Math.Abs(Convert.ToDecimal(Eval("RealityCost")))) %>
                                    </ItemTemplate>
                                    <HeaderStyle HorizontalAlign="Center" />
                                    <ItemStyle Width="70px" HorizontalAlign="Center" />
                                </rad:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </rad:RadGrid>

                </td>
            </tr>
             
             <tr style="line-height: 40px;">
                <td colspan="4" style="text-align: center;">
                    <asp:Button ID="btn_Bind" runat="server"  OnClick="btn_Bind_Click"/>
                    <asp:Button ID="btn_Save" runat="server" Text="确认转派" OnClientClick="return CheckEmpty();" OnClick="btn_Save_Click" />
                </td>
            </tr>
           
        </table>
        
         <rad:RadWindowManager runat="server" OnAjaxRequest="RAMPositionPower_AjaxRequest">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="申请人员" Width="500px" Height="420px" />
        </Windows>
    </rad:RadWindowManager>

        <script src="../JavaScript/jquery.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script src="../JavaScript/tool.js"></script>
        <script type="text/javascript">
            $(function() {
                $("#txt_oldReportPersonnel").attr("readonly", "readonly");
                $("#txt_newReportPersonnel").attr("readonly", "readonly");
                $("#btn_Bind").css("display","none");
            });

            //申请人
            $("#txt_oldReportPersonnel").on('click', function() {
                window.radopen("../CostReport/CostReport_DepositRecoverySeleteReportPersonnel.aspx?type=0", "raw");
            });

            //转派人
            $("#txt_newReportPersonnel").on('click', function () {
                window.radopen("../CostReport/CostReport_DepositRecoverySeleteReportPersonnel.aspx?type=1", "raw");
            });

            function Bind() {
                $("#btn_Bind").click();
            }

            //全选
            function checkAll() {
                var a = $("input[type='checkbox'][name='ckId']");
                var n = a.length;
                for (var i = 0; i < n; i++) {
                    a[i].checked = window.event.srcElement.checked;
                }
            }

            function CheckEmpty() {
                if ($("#hf_oldReportPersonnel").val() == "") {
                    alert("请选择申请人！");
                    return false;
                } else if ($("#hf_newReportPersonnel").val() == "") {
                    alert("请选择转派人！");
                    return false;
                }
                return true;
            }
        </script>
    </form>
</body>
</html>

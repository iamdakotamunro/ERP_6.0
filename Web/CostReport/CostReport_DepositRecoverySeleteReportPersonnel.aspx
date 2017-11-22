<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CostReport_DepositRecoverySeleteReportPersonnel.aspx.cs" Inherits="ERP.UI.Web.CostReport.CostReport_DepositRecoverySeleteReportPersonnel" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        td.ShortFromRowTitle {
            /* min-width: 10px;*/
            white-space: nowrap;
            height: 24px;
            text-align: right;
        }

        .button {
            width: 25px;
            border: 1px solid #D5D5D5;
            border-bottom: 1px solid #C2C2C2;
            border-right: 1px solid #C2C2C2;
            font-size: 11px;
            color: #666666;
            background-position: top;
            background-color: white;
            height: 20px;
            vertical-align: middle;
        }


        #ckb_ReportPersonnel tr td {
            border-bottom: 1px solid #dae2e8;
            text-align: left;
        }

        .divReportPersonnel table {
            border-collapse: collapse;
            width: 100%;
        }
        .divReportPersonnel {
            border: 1px solid #8e8e8e;
        }

        #lbx_goodslist1 {
            width: 100%;
        }

    </style>
</head>
<body style="overflow: hidden;margin-top: 5px;">
    <form id="form1" runat="server">
         <rad:RadScriptManager runat="server" ID="RadScriptManager1" />
        <table style="width: 100%; line-height: 20px;">
           
            <tr>
                <td colspan="3" style="text-align: left;">
                     <rad:RadTextBox runat="server" EmptyMessage="员工姓名或工号" ID="RTB_PersonnelNameOrEnterpriseNo" Width="120px" />
                    <asp:ImageButton Style='vertical-align: middle' ID="LB_Search" runat="server" ValidationGroup="Search"
                    SkinID="SearchButton"  OnClick="LB_Search_Click" />
                </td>
                
            </tr>
             
             <tr>
                 <td class="ShortFromRowTitle" style="width: 45%">
                     <div id="divPersonnel" class="divReportPersonnel" style="overflow-x: auto; overflow-y: auto; height: 310px; ">
                         <table style="text-align: left; border-bottom: 0;background:0 -2300px repeat-x #718ca1 url('WebResource.axd?d=UByMqEyP4D_DJpD0s8MTnCQ5wDpHikPxIRRn5u4rFJ4VmaIm25TCkOBsA_g6YAUHVNmnzaO3Q1NPgJkBeWLBF3ce-ZBRI9DaxRlXFc7ptd6Mqk2TzX5AiPJivvBv20o--IBKj7DUt2eDosA98gtkwKl7LyxJidAOMTM5UDyP84o1&t=635978777921912585')">
                             <tr>
                                 <td><asp:CheckBox ID="ckb_qx" runat="server" Text="全选"/>
                                     </td>
                             </tr>
                         </table>
                         <asp:CheckBoxList ID="ckb_ReportPersonnel" runat="server" RepeatDirection="Vertical"></asp:CheckBoxList>
                     </div>
                 </td>
                <td class="ShortFromRowTitle"  style="width: 10%;text-align: center;">
                    <asp:Button ID="btn_Right" runat="server" Text=">" OnClick="AddToRight" CssClass="button"
                        ToolTip="添加到右边" />
                    <br />
                    <asp:Button ID="btn_Left" runat="server" Text="<"  OnClick="RemoveToLeft" CssClass="button"
                        ToolTip="移除到左边" />
                    <br />
                    <br />
                    <asp:Button ID="btn_AllRight" runat="server" Text=">>"  OnClick="AllAddToRight"  CssClass="button"
                        ToolTip="全部添加到右边" />
                    <br />
                    <asp:Button ID="btn_AllLeft" runat="server" Text="<<" OnClick="AllReMoveToLeft" CssClass="button"
                        ToolTip="全部移除到左边" />
                    <br />
                </td>
                <td  style="width: 45%">
                    <rad:RadListBox ID="lbx_goodslist1" runat="server"  Height="310px" >
                    </rad:RadListBox>
                </td>
            </tr>
             <tr>
                <td colspan="3" style="text-align: center;">
                    <asp:Button ID="btn_Save" runat="server" Text="确定" OnClick="btn_Save_Click" />
                </td>
            </tr>
            

        </table>
        <script src="../JavaScript/jquery.js"></script>
        <script src="../JavaScript/telerik.js"></script>
        <script src="../JavaScript/tool.js"></script>
        <script type="text/javascript">
            //全选
            $("#divPersonnel input[type='checkbox'][id='ckb_qx']").on('click', function () {
                if ($("input[type='checkbox'][id='ckb_qx']").is(':checked')) {
                    $("#divPersonnel input[type='checkbox']").prop('checked', true);
                } else {
                    $("#divPersonnel input[type='checkbox']").prop('checked', false);
                }
            });


            $("#divPersonnel input[type='checkbox']:not([id='ckb_qx'])").on('click', function () {
                var result = 0;
                $("#divPersonnel input[type='checkbox']:not([id='ckb_qx'])").each(function () {
                    if ($(this).is(':checked') === false) {
                        result = 1;
                        return false;
                    }
                    return true;
                });

                if (result === 0) {
                    $("input[id='ckb_qx'][type='checkbox']").prop('checked', true);
                } else {
                    $("input[id='ckb_qx'][type='checkbox']").prop('checked', false);
                }
            });

            function Save(id, name, type) {
                if (type == "0") {
                    $("#txt_oldReportPersonnel", window.parent.document).val(name);
                    $("#hf_oldReportPersonnel", window.parent.document).val(id);
                    window.parent.window.Bind();
                } else if (type == "1") {
                    $("#txt_newReportPersonnel", window.parent.document).val(name);
                    $("#hf_newReportPersonnel", window.parent.document).val(id);
                }
                GetRadWindow().Close();
            }
        </script>
    </form>
</body>
</html>

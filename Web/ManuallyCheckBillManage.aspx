<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="ManuallyCheckBillManage.aspx.cs" Inherits="ERP.UI.Web.ManuallyCheckBillManage" %>

<%@ Import Namespace="ERP.BLL.Implement.Organization" %>
<%@ Import Namespace="ERP.Enum.Attribute" %>
<%@ Import Namespace="ERP.Enum" %>

<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="server">
    <table style="width: 100%;">
        <tr>
            <td style="text-align: right;">对账人：
            </td>
            <td>
                <rad:RadComboBox ID="rcb_CheckBillPersonnelId" Width="167px" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入对账人" runat="server" Height="200px" OnItemsRequested="rcb_CheckBillPersonnelId_ItemsRequested"></rad:RadComboBox>
            </td>
            <td style="text-align: right;">往来收款单号：
            </td>
            <td>
                <asp:TextBox ID="txt_TradeCode" runat="server"></asp:TextBox>
            </td>
            <td style="text-align: right;">对账状态：
            </td>
            <td>
                <asp:DropDownList ID="ddl_CheckState" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">对账日期：
            </td>
            <td>
                <asp:TextBox ID="txt_CheckBillDateStart" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
                至
                <asp:TextBox ID="txt_CheckBillDateEnd" runat="server" Width="70px" onclick="WdatePicker({skin:'blue'})"></asp:TextBox>
            </td>
            <td style="text-align: right;">销售平台：
            </td>
            <td>
                <rad:RadComboBox ID="rcb_SalePlatform" Width="173px" AllowCustomText="true" EnableLoadOnDemand="True" EmptyMessage="请输入销售平台" runat="server" Height="200px" OnItemsRequested="rcb_SalePlatform_ItemsRequested" OnSelectedIndexChanged="rcb_SalePlatform_SelectedIndexChanged" AutoPostBack="true"></rad:RadComboBox>
                <asp:HiddenField ID="Hid_SaleFiliale" runat="server" />
            </td>
            <td style="text-align: right;">收款状态：
            </td>
            <td>
                <asp:DropDownList ID="ddl_ReceiptState" runat="server" Width="78px">
                    <asp:ListItem Text="全部" Value="-1"></asp:ListItem>
                    <asp:ListItem Text="未收款" Value="0"></asp:ListItem>
                    <asp:ListItem Text="已收款" Value="1"></asp:ListItem>
                </asp:DropDownList>
            </td>
        </tr>
        <tr>
            <td colspan="6" style="text-align: center;">
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
            </td>
        </tr>
    </table>
    <table style="border: solid 1px #cccccc; width: 100%; background-color: #FAFAFA; margin-top: 10px; margin-bottom: 0px; height: 30px; font-weight: bolder;">
        <tr>
            <td style="font-size: 16px; width: 120px;">导入<span style="color: limegreen;">对账原始表</span>:</td>
            <td style="width: 350px;">
                <asp:TextBox ID="UploadExcelName" runat="server" onfocus="this.blur();"></asp:TextBox>
                <asp:FileUpload ID="UploadExcel" runat="server" Style="display: none;" onchange="CheckFile()" />
                <input type="button" value="选择文件" title="文件格式(.xls)!" onclick="SelectFile(this);" />
                <asp:Button ID="btn_Upload" runat="server" Text="导入" OnClick="btn_Upload_Click" />
                <a id="Template" runat="server" href="../App_Themes/人工对账模板.xls" style="color: red; font-weight: bold;">模板下载</a>
            </td>
            <td style="font-size: 16px; width: 85px;">
                <img src="Images/arrow1.jpg" /></td>
            <td style="font-size: 16px; width: 190px">生成<span style="color: limegreen;">双方对比表(系统处理)</span></td>
            <td style="font-size: 16px; width: 85px">
                <img src="Images/arrow2.jpg" /></td>
            <td style="font-size: 16px; width: 115px">上传<span style="color: limegreen;">财务确认表</span></td>
            <td style="font-size: 16px; width: 85px">
                <img src="Images/arrow3.jpg" /></td>
            <td style="font-size: 16px; width: 190px">生成<span style="color: limegreen;">对账结果表(系统处理)</span></td>
            <td style="font-size: 16px; width: 85px">
                <img src="Images/arrow4.jpg" /></td>
            <td style="font-size: 16px;">
                <asp:Button ID="btn_Receipt" runat="server" Text="生成收款单" /></td>
        </tr>
    </table>
    <rad:RadGrid ID="RG_ManuallyCheckBill" runat="server" ShowFooter="true" OnNeedDataSource="RG_ManuallyCheckBill_NeedDataSource" OnItemDataBound="RG_ManuallyCheckBill_ItemDataBound">
        <MasterTableView>
            <CommandItemTemplate>
            </CommandItemTemplate>
            <CommandItemStyle Height="0px" />
            <Columns>
                <rad:GridTemplateColumn HeaderText="&lt;input type=checkbox value=-1 onclick=checkAll()&gt;全选">
                    <ItemTemplate>
                        <input title="请选择" type="checkbox" name="ckId" value='<%# Eval("Id")%>' <%# Eval("State").Equals(3)?"":"disabled"%> /><span <%# Eval("State").Equals(3)?"":"style='color:red;'"%>>选择</span>
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle Width="50px" HorizontalAlign="Center" VerticalAlign="Middle" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="对账人">
                    <ItemTemplate>
                        <%#new PersonnelManager().GetName(new Guid(Eval("CheckBillPersonnelId").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="销售平台">
                    <ItemTemplate>
                        <%# SalePlatformList.First(p => p.ID.Equals(new Guid(Eval("SalePlatformId").ToString()))).Name %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="TradeCode" HeaderText="往来收款单号">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="100px" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="对账状态">
                    <ItemTemplate>
                        <%#EnumAttribute.GetKeyName((CheckType)Convert.ToInt32(Eval("CheckState")))%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="第三方订单总金额">
                    <ItemTemplate>
                        <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("ThirdOrderTotalAmount"))) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="UnusualOrderQuantity" HeaderText="异常订单">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="财务确认总金额" UniqueName="PayCost">
                    <ItemTemplate>
                        <%#ERP.UI.Web.Common.WebControl.RemoveDecimalEndZero(Convert.ToDecimal(Eval("ConfirmTotalAmount"))) %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="收款状态">
                    <ItemTemplate>
                        <%#int.Parse(Eval("ReceiptState").ToString()).Equals(0)?"未收款":"已收款" %>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="CheckBillDate" HeaderText="对账日期" DataFormatString="{0:yyyy-MM-dd}">
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="65px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn HeaderText="备注">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="80%" tooltipmsg='<%#Eval("Memo").ToString().Replace("\n","<br/>")%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="对账原始表">
                    <ItemTemplate>
                        <input type="button" value="查看" <%#int.Parse(Eval("State").ToString())>=0?"":"style='display:none;'"%> onclick='<%#"ManuallyCheckBillDetail(\""+Eval("Id")+"\",\""+Eval("SalePlatformId")+"\",\""+Eval("State")+"\")"%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="双方对比表">
                    <ItemTemplate>
                        <input type="button" value="查看" <%#int.Parse(Eval("State").ToString())>=1?"":"style='display:none;'"%> onclick="" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="财务确认表">
                    <ItemTemplate>
                        <input type="button" value="查看" <%#int.Parse(Eval("State").ToString())>=2?"":"style='display:none;'"%> onclick="" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="对账结果表">
                    <ItemTemplate>
                        <input type="button" value="查看" <%#int.Parse(Eval("State").ToString())>=3?"":"style='display:none;'"%> onclick="" />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="80px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>

    <rad:RadWindowManager runat="server">
        <Windows>
            <rad:RadWindow ID="raw" runat="server" Title="对账详情" Width="1000px" Height="650px" />
        </Windows>
    </rad:RadWindowManager>

    <script src="My97DatePicker/WdatePicker.js"></script>
    <script src="JavaScript/ToolTipMsg.js"></script>
    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
        });

        //对账详情
        function ManuallyCheckBillDetail(id, salePlatformId, state) {
            window.radopen("Windows/ManuallyCheckBillDetail.aspx?Id=" + id + "&SalePlatformId=" + salePlatformId + "&State=" + state, "raw");
        }

        //验证文件格式
        function CheckFile() {
            var filePath = $("input[id$='UploadExcel'][type='file']").val();
            var ext = filePath.substr(filePath.length - 4, 4).toLowerCase();
            if (ext !== ".xls") {
                alert("请选择格式为“.xls”文件！");
                $("input[id$='UploadExcelName']").val("");
            } else {
                $("input[id$='UploadExcelName']").val(filePath);
            }
        }

        //选择文件
        function SelectFile(obj) {
            var uploadExcel = $(obj).prevAll("input[id$='UploadExcel'][type='file']");
            uploadExcel.click();
        }

        //全选
        function checkAll() {
            var a = $("input[type='checkbox'][name='ckId']:not(:disabled)");
            var n = a.length;
            for (var i = 0; i < n; i++) {
                a[i].checked = window.event.srcElement.checked;
            }
        }
    </script>
</asp:Content>

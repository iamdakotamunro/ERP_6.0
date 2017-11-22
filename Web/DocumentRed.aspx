<%@ Page Title="" Language="C#" MasterPageFile="~/MainMaster.master" AutoEventWireup="true" CodeBehind="DocumentRed.aspx.cs" Inherits="ERP.UI.Web.DocumentRed" %>

<%@ Import Namespace="ERP.Enum" %>
<asp:Content ID="Content1" ContentPlaceHolderID="CPHStage" runat="Server">
    <table class="PanelArea">
        <tr>
            <td style="text-align: right;">单据编号：
            </td>
            <td>
                <asp:TextBox ID="txt_NO" runat="server" placeholder="原单/新单/红冲单"></asp:TextBox>
            </td>
            <td style="text-align: right;">单据类型：
            </td>
            <td>
                <asp:DropDownList ID="ddl_DocumentType" runat="server" Width="74px">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">单据状态：
            </td>
            <td>
                <asp:DropDownList ID="ddl_State" runat="server">
                </asp:DropDownList>
            </td>
            <td rowspan="2">
                <asp:Button ID="btn_Search" runat="server" Text="查询" OnClick="btn_Search_Click" />
            </td>
        </tr>
        <tr>
            <td style="text-align: right;">所属仓库：
            </td>
            <td>
                <asp:DropDownList ID="ddl_Waerhouse" runat="server">
                </asp:DropDownList>
            </td>
            <td style="text-align: right;">出/入库时间：
            </td>
            <td>
                <asp:TextBox ID="txt_StartTime" runat="server" Width="70px" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM-dd',maxDate:$('input[id$=txt_EndTime]').val()})"></asp:TextBox>
                至
                <asp:TextBox ID="txt_EndTime" runat="server" Width="70px" onclick="WdatePicker({skin:'blue',dateFmt:'yyyy-MM-dd',minDate:$('input[id$=txt_StartTime]').val()})"></asp:TextBox>
            </td>
            <td style="text-align: right;">红冲类型：
            </td>
            <td>
                <asp:DropDownList ID="ddl_RedType" runat="server">
                </asp:DropDownList>
            </td>
        </tr>
    </table>

    <rad:RadGrid ID="Rgd_DocumentRed" runat="server" OnNeedDataSource="RgdDocumentRed_OnNeedDataSource" Width="100%" SkinID="CustomPaging">
        <ClientSettings>
            <ClientEvents OnRowDblClick="RowDblClick" />
        </ClientSettings>
        <MasterTableView DataKeyNames="RedId,RedType,DocumentType,State,LinkTradeId" ClientDataKeyNames="RedId,RedType,DocumentType,State,LinkTradeId">
            <CommandItemTemplate>
                <asp:LinkButton ID="LinkButton1" CausesValidation="false" OnClientClick="return PriceStorageRedAdd('1');"
                    runat="server">
                    <asp:Image ID="Image1" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                        BorderStyle="None" />调价入库红冲
                </asp:LinkButton>&nbsp;
                    <asp:LinkButton ID="LinkButton2" CausesValidation="false" OnClientClick="return PriceStorageRedAdd('2');"
                        runat="server">
                        <asp:Image ID="Image2" SkinID="InsertImageButton" runat="server" ImageAlign="AbsMiddle"
                            BorderStyle="None" />调价出库红冲
                    </asp:LinkButton>
            </CommandItemTemplate>
            <CommandItemStyle HorizontalAlign="Right" Height="26px" />
            <Columns>
                <rad:GridBoundColumn DataField="TradeCode" HeaderText="单据编号">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="LinkTradeCode" HeaderText="原单编号">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="WarehouseId" HeaderText="所属仓库">
                    <ItemTemplate>
                        <%# GetWarehouse(new Guid(Eval("WarehouseId").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="DocumentType" HeaderText="单据类型">
                    <ItemTemplate>
                        <%# ERP.Enum.Attribute.EnumAttribute.GetKeyName((DocumentType)Convert.ToInt32(Eval("DocumentType"))) %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="RedType" HeaderText="红冲类型">
                    <ItemTemplate>
                        <%# ERP.Enum.Attribute.EnumAttribute.GetKeyName((RedType)Convert.ToInt32(Eval("RedType"))) %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridBoundColumn DataField="AccountReceivable" HeaderText="单据金额">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridBoundColumn DataField="LinkDateCreated" HeaderText="出/入库时间" DataFormatString="{0:D}">
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridBoundColumn>
                <rad:GridTemplateColumn DataField="FilialeId" HeaderText="物流配送公司">
                    <ItemTemplate>
                        <%# GetHostingFilialeId(new Guid(Eval("WarehouseId").ToString()),Convert.ToInt32(Eval("StorageType")),new Guid(Eval("FilialeId").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="ThirdCompanyId" HeaderText="供应商">
                    <ItemTemplate>
                        <%# GetCompany(new Guid(Eval("ThirdCompanyId").ToString())) %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="State" HeaderText="状态">
                    <ItemTemplate>
                        <%# ERP.Enum.Attribute.EnumAttribute.GetKeyName((DocumentRedState)Convert.ToInt32(Eval("State"))) %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn DataField="AuditTime" HeaderText="审核时间">
                    <ItemTemplate>
                        <%# Convert.ToDateTime(Eval("AuditTime"))!=DateTime.MinValue?Convert.ToDateTime(Eval("AuditTime")).ToString("yyyy-MM-dd"):"" %>
                    </ItemTemplate>
                    <HeaderStyle Width="100px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="备注">
                    <ItemTemplate>
                        <asp:ImageButton runat="server" SkinID="InsertImageButton" tipMaxWidth="80%" tooltipmsg='<%#Eval("Memo").ToString().Replace("\n","<br/>")%>' />
                    </ItemTemplate>
                    <HeaderStyle Width="50px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="作废">
                    <ItemTemplate>
                        <asp:ImageButton ID="imgbtn_Delete" runat="server" OnClientClick="return GiveTip(event,'您确认作废吗？');" OnClick="imgbtn_Delete_Click" CommandName='<%#Eval("LinkTradeId")%>' CommandArgument='<%#Eval("RedId")%>' SkinID="DeleteImageButton" Visible='<%# !int.Parse(Eval("DocumentType").ToString()).Equals((int)DocumentType.RedDocument) 
                        && Convert.ToInt32(Eval("State")) !=(int)DocumentRedState.Finished%>' />
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="操作">
                    <ItemTemplate>
                        <%# (int.Parse(Eval("State").ToString()).Equals((int)DocumentRedState.WaitAudit)||int.Parse(Eval("State").ToString()).Equals((int)DocumentRedState.Refuse)) 
                                && Convert.ToInt32(Eval("DocumentType"))!=(int)DocumentType.RedDocument
                                ?"<input type=\"button\" onclick=\"PriceStorageRedEdit('"+ Eval("RedId") + "','"+Eval("RedType") +"');\" value=\""+(int.Parse(Eval("State").ToString()).Equals((int)DocumentRedState.WaitAudit)
                                 ?"编辑":"重送")+"\"/>":""%>
                    </ItemTemplate>
                    <HeaderStyle Width="60px" HorizontalAlign="Center" />
                    <ItemStyle HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
                <rad:GridTemplateColumn HeaderText="审批">
                    <ItemTemplate>
                        <%# int.Parse(Eval("State").ToString()).Equals((int)DocumentRedState.WaitAudit)?"<input type=\"button\" onclick=\"ApprovalPriceRed('"+ Eval("LinkTradeId") + "','"+Eval("RedType") +"');\" value=\"审核\"/>":""%>
                    </ItemTemplate>
                    <HeaderStyle HorizontalAlign="Center" />
                    <ItemStyle Width="60px" HorizontalAlign="Center" />
                </rad:GridTemplateColumn>
            </Columns>
        </MasterTableView>
    </rad:RadGrid>
    <rad:RadWindowManager ID="WindowManager" runat="server">
        <Windows>
            <rad:RadWindow ID="PriceStorageRedIn" runat="server" Title="调价入库红冲" Height="540px" Width="900px" />
            <rad:RadWindow ID="PriceStorageRedOut" runat="server" Title="调价出库红冲" Height="540px" Width="900px" />
            <rad:RadWindow ID="ApprovalPriceRedIn" runat="server" Title="审批调价入库红冲" Height="600px" Width="900px" />
            <rad:RadWindow ID="ApprovalPriceRedOut" runat="server" Title="审批调价出库红冲" Height="600px" Width="900px" />
        </Windows>
    </rad:RadWindowManager>

    <script src="My97DatePicker/WdatePicker.js"></script>
    <script src="JavaScript/ToolTipMsg.js"></script>
    <script src="JavaScript/GiveTip.js"></script>
    <script type="text/javascript">
        $(function () {
            new ToolTipMsg().bindToolTip("[tooltipmsg]");
        });

        //添加
        function PriceStorageRedAdd(redType) {
            if (redType === "1") {//调价入库红冲
                window.radopen("./Windows/PriceStorageRed.aspx?RedType=1", "PriceStorageRedIn");
            } else if (redType === "2") {//调价出库红冲
                window.radopen("./Windows/PriceStorageRed.aspx?RedType=2", "PriceStorageRedOut");
            }
            return false;
        }

        //编辑
        function PriceStorageRedEdit(redId, redType) {
            if (redType === "1") {//调价入库红冲
                window.radopen("./Windows/PriceStorageRed.aspx?RedType=1&&RedId=" + redId, "PriceStorageRedIn");
            } else if (redType === "2") {//调价出库红冲
                window.radopen("./Windows/PriceStorageRed.aspx?RedType=2&&RedId=" + redId, "PriceStorageRedOut");
            }
        }

        //审批
        function ApprovalPriceRed(linkTradeId, redType) {
            if (redType === "1") {//调价入库红冲
                window.radopen("./Windows/ApprovalPriceRed.aspx?RedType=1&&LinkTradeId=" + linkTradeId , "ApprovalPriceRedIn");
            } else if (redType === "2") {//调价出库红冲
                window.radopen("./Windows/ApprovalPriceRed.aspx?RedType=2&&LinkTradeId=" + linkTradeId , "ApprovalPriceRedOut");
            }
        }

        function RowDblClick(obj, args) {
            var redId = args.getDataKeyValue("LinkTradeId");
            var redType = args.getDataKeyValue("RedType");
            var documentType = args.getDataKeyValue("DocumentType");
            //var documentType = args.getDataKeyValue("DocumentType");
            if (redType == 1) {
                //借出申请
                window.radopen("./Windows/ApprovalPriceRed.aspx?RedType=1&LinkTradeId=" + redId + "&Read=1&DocumentType="+documentType, "ApprovalPriceRedIn");
            } else{
                //售后退货出库 
                window.radopen("./Windows/ApprovalPriceRed.aspx?RedType=2&LinkTradeId=" + redId + "&Read=1&DocumentType=" + documentType, "ApprovalPriceRedOut");
            } 
            return false;
        }

        //重新绑定Grid事件
        function refreshGrid() {
            $("input[id$='btn_Search']").click();
        }


    </script>
</asp:Content>

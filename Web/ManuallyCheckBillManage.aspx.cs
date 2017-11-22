using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.UI.Web.Common;
using Telerik.Web.UI;
using Keede.Ecsoft.Model;
using ERP.SAL.Interface;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Framework.Data;

namespace ERP.UI.Web
{
    public partial class ManuallyCheckBillManage : BasePage
    {
        private readonly IPersonnelSao _personnelSao = new PersonnelSao();
        private readonly IManuallyCheckBill _manuallyCheckBill = new ManuallyCheckBillDal();
        private readonly IManuallyCheckBillDetail _manuallyCheckBillDetail = new ManuallyCheckBillDetailDal();

        #region 属性
        /// <summary>
        /// 当前登录人信息模型
        /// </summary>
        private PersonnelInfo Personnel
        {
            get
            {
                if (ViewState["Personnel"] == null)
                {
                    ViewState["Personnel"] = CurrentSession.Personnel.Get();
                }
                return (PersonnelInfo)ViewState["Personnel"];
            }
        }

        /// <summary>
        /// 销售平台
        /// </summary>
        public IList<SalePlatformInfo> SalePlatformList
        {
            get
            {
                if (ViewState["SalePlatformList"] == null)
                {
                    ViewState["SalePlatformList"] = CacheCollection.SalePlatform.GetList();
                }
                return (IList<SalePlatformInfo>)ViewState["SalePlatformList"];
            }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadCheckTypeStateData();
                LoadSalePlatformData();
            }
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_ManuallyCheckBill.DataBind();
        }

        #region 数据准备
        //对账状态
        protected void LoadCheckTypeStateData()
        {
            var list = EnumAttribute.GetDict<CheckType>().OrderBy(p => p.Key);
            ddl_CheckState.DataSource = list;
            ddl_CheckState.DataTextField = "Value";
            ddl_CheckState.DataValueField = "Key";
            ddl_CheckState.DataBind();
        }

        //销售平台
        protected void LoadSalePlatformData()
        {
            rcb_SalePlatform.DataSource = SalePlatformList;
            rcb_SalePlatform.DataTextField = "Name";
            rcb_SalePlatform.DataValueField = "ID";
            rcb_SalePlatform.DataBind();
        }
        #endregion

        #region SelectedIndexChanged事件
        //对账人数据绑定
        protected void rcb_CheckBillPersonnelId_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var personnelList = _personnelSao.GetList().Where(p => p.RealName.Contains(e.Text)).ToList();
                Int32 totalCount = personnelList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in personnelList)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.RealName, item.PersonnelId.ToString()));
                    }
                }
            }
        }

        //销售平台选择
        protected void rcb_SalePlatform_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                var salePlatformListResult = SalePlatformList.First(p => p.ID.Equals(new Guid(e.Value)));
                Hid_SaleFiliale.Value = salePlatformListResult.FilialeId.ToString();
            }
        }

        //销售平台搜索
        protected void rcb_SalePlatform_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var salePlatformListResult = SalePlatformList.Where(p => p.Name.Contains(e.Text));
                int totalCount = SalePlatformList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in salePlatformListResult)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.Name, item.ID.ToString()));
                    }
                }
            }
        }
        #endregion

        #region 数据列表相关
        protected void RG_ManuallyCheckBill_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            #region 查询条件
            Guid checkBillPersonnelId = Guid.Empty;
            string tradeCode = string.Empty;
            CheckType checkState = CheckType.AllCheck;
            DateTime checkBillDateStart = DateTime.MinValue;
            DateTime checkBillDateEnd = DateTime.MaxValue;
            Guid salePlatformId = Guid.Empty;
            int receiptState = -1;
            int pageIndex = RG_ManuallyCheckBill.CurrentPageIndex + 1;
            int pageSize = RG_ManuallyCheckBill.PageSize;
            if (!string.IsNullOrEmpty(rcb_CheckBillPersonnelId.SelectedValue))
            {
                checkBillPersonnelId = new Guid(rcb_CheckBillPersonnelId.SelectedValue);
            }
            if (!string.IsNullOrEmpty(txt_TradeCode.Text))
            {
                tradeCode = txt_TradeCode.Text;
            }
            if (!string.IsNullOrEmpty(ddl_CheckState.SelectedValue))
            {
                checkState = (CheckType)int.Parse(ddl_CheckState.SelectedValue);
            }
            if (!string.IsNullOrEmpty(txt_CheckBillDateStart.Text))
            {
                checkBillDateStart = DateTime.Parse(txt_CheckBillDateStart.Text);
            }
            if (!string.IsNullOrEmpty(txt_CheckBillDateEnd.Text))
            {
                checkBillDateEnd = DateTime.Parse(txt_CheckBillDateEnd.Text).AddDays(1);
            }
            if (!string.IsNullOrEmpty(rcb_SalePlatform.SelectedValue))
            {
                salePlatformId = new Guid(rcb_SalePlatform.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddl_ReceiptState.SelectedValue))
            {
                receiptState = int.Parse(ddl_ReceiptState.SelectedValue);
            }
            #endregion

            int total;
            var manuallyCheckBillList = _manuallyCheckBill.GetAllManuallyCheckBill(checkBillPersonnelId, tradeCode, checkState, checkBillDateStart, checkBillDateEnd, salePlatformId, receiptState, pageIndex, pageSize, out total);
            RG_ManuallyCheckBill.DataSource = manuallyCheckBillList;
            RG_ManuallyCheckBill.VirtualItemCount = total;
        }

        #region 列表事件
        //行绑定事件
        protected void RG_ManuallyCheckBill_ItemDataBound(object sender, GridItemEventArgs e)
        {
            //if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            //{
            //    var reportInfo = (CostReportInfo)e.Item.DataItem;
            //    #region 票据截止日期 超过15天未提交票据，标红该行
            //    DateTime rportDate = Convert.ToDateTime(reportInfo.ReportDate);
            //    DateTime finallySubmitTicketDate = rportDate.Date.AddDays(15);
            //    if (reportInfo.State != (int)CostReportState.Cancel &&
            //        reportInfo.State != (int)CostReportState.Complete)
            //    {
            //        if (finallySubmitTicketDate <= DateTime.Now)
            //        {
            //            e.Item.Style["color"] = "red";
            //            e.Item.ToolTip = "票据超过15天未提交！";
            //        }
            //    }
            //    #endregion

            //    #region 控制操作按钮显示的文本
            //    var btnControl = (Button)e.Item.FindControl("btn_Control");
            //    if (
            //        (reportInfo.ReportKind.Equals((int)CostReportKind.Before) && reportInfo.PayCost.Equals(0) && (reportInfo.State.Equals((int)CostReportState.InvoiceNoPass) || reportInfo.State.Equals((int)CostReportState.Auditing) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass))) ||
            //        (reportInfo.ReportKind.Equals((int)CostReportKind.Later) && (reportInfo.State.Equals((int)CostReportState.InvoiceNoPass) || reportInfo.State.Equals((int)CostReportState.Auditing) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass))) ||
            //        (reportInfo.ReportKind.Equals((int)CostReportKind.Paying) && (reportInfo.State.Equals((int)CostReportState.Auditing) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass))) ||
            //        (reportInfo.ReportKind.Equals((int)CostReportKind.FeeIncome) && (reportInfo.State.Equals((int)CostReportState.Auditing) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass))) ||
            //        (reportInfo.ReportKind.Equals((int)CostReportKind.Before) && !reportInfo.PayCost.Equals(0) && (reportInfo.State.Equals((int)CostReportState.InvoiceNoPass) || (!reportInfo.IsSystem && reportInfo.State.Equals((int)CostReportState.Auditing)) || reportInfo.State.Equals((int)CostReportState.AuditingNoPass) || reportInfo.State.Equals((int)CostReportState.CompletedMayApply))) ||
            //        (reportInfo.ReportKind.Equals((int)CostReportKind.Paying) && reportInfo.State.Equals((int)CostReportState.InvoiceNoPass))
            //    )
            //    {
            //        btnControl.Text = "编辑";
            //    }
            //    else
            //    {
            //        btnControl.Text = "查看";
            //    }
            //    #endregion

            //    #region “申报金额”>=1万元(绿色);“申报金额”>=10万元(黄色);“申报金额”>=100万元(紫色)
            //    var reportCost = DataBinder.Eval(e.Item.DataItem, "ReportCost");
            //    if (decimal.Parse(reportCost.ToString()) >= 1000000)
            //    {
            //        e.Item.Style["color"] = "#CC00FF";
            //    }
            //    else if (decimal.Parse(reportCost.ToString()) >= 100000)
            //    {
            //        e.Item.Style["color"] = "#FF9900";
            //    }
            //    else if (decimal.Parse(reportCost.ToString()) >= 10000)
            //    {
            //        e.Item.Style["color"] = "#009900";
            //    }
            //    #endregion
            //}
        }
        #endregion

        //#region 列表显示辅助方法
        ///// <summary>
        ///// 票据截止日期=申报日期+15天
        ///// </summary>
        ///// <param name="reportDate">申报日期</param>
        ///// <returns></returns>
        //protected string GetFinallySubmitTicketDate(string reportDate)
        //{
        //    if (!reportDate.Equals("1900-01-01"))
        //    {
        //        DateTime date = Convert.ToDateTime(reportDate);
        //        return date.Date.AddDays(15).ToString("yyyy-MM-dd");
        //    }
        //    return string.Empty;
        //}
        ///// <summary>
        ///// 获取处理状态
        ///// </summary>
        ///// <param name="reportState">状态</param>
        ///// <returns></returns>
        //protected string GetReportState(string reportState)
        //{
        //    if (string.IsNullOrEmpty(reportState))
        //    {
        //        return "-";
        //    }

        //    var state = int.Parse(reportState);
        //    if (((int)CostReportState.AlreadyAuditing).Equals(state))
        //    {
        //        return "待收款";
        //    }
        //    if (((int)CostReportState.WaitVerify).Equals(state))
        //    {
        //        return "待付款";
        //    }
        //    return EnumAttribute.GetKeyName((CostReportState)state);
        //}
        //#endregion
        #endregion

        //导入对账原始表
        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            var excelName = UploadExcelName.Text;
            UploadExcelName.Text = string.Empty;

            #region 数据验证
            if (string.IsNullOrEmpty(rcb_SalePlatform.SelectedValue) || string.IsNullOrEmpty(Hid_SaleFiliale.Value))
            {
                MessageBox.Show(this, "请选择“销售平台”！");
                return;
            }

            if (!UploadExcel.HasFile || string.IsNullOrEmpty(excelName))
            {
                MessageBox.Show(this, "请选择格式为“.xls”文件！");
                return;
            }

            var ext = Path.GetExtension(UploadExcel.FileName);
            if (ext != null && !ext.Equals(".xls"))
            {
                MessageBox.Show(this, "文件格式错误(.xls)！");
                return;
            }
            #endregion

            try
            {
                #region 将上传文件保存至临时文件夹
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                string folderPath = "~/UserDir/ManuallyCheckBill/InitialTable/ ";
                if (!Directory.Exists(Server.MapPath(folderPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(folderPath));
                }
                string filePath = Server.MapPath(folderPath + fileName);
                UploadExcel.PostedFile.SaveAs(filePath);
                #endregion

                var excelDataTable = ExcelHelper.GetDataSet(filePath).Tables[0];

                #region 获取数据之后删除临时文件
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                #endregion

                List<ManuallyCheckBillDetailInfo> manuallyCheckBillDetailInfoList = new List<ManuallyCheckBillDetailInfo>();
                StringBuilder errorMsg = new StringBuilder();
                int index = 2;

                #region 对账原始表
                for (int i = 0; i < excelDataTable.Rows.Count; i++)
                {
                    StringBuilder rowMsg = new StringBuilder();
                    var systemOrderNo = excelDataTable.Rows[i]["系统订单号"].ToString();
                    var thirdOrderNo = excelDataTable.Rows[i]["第三方订单号"].ToString();
                    var isCheck = excelDataTable.Rows[i]["是否对账"].ToString();
                    var isReceipt = excelDataTable.Rows[i]["是否收款"].ToString();
                    var orderTime = excelDataTable.Rows[i]["下单日期"].ToString();
                    var memberId = excelDataTable.Rows[i]["会员名"].ToString();
                    var systemOrderAmount = excelDataTable.Rows[i]["系统金额"].ToString();
                    var thirdOrderAmount = excelDataTable.Rows[i]["第三方订单金额"].ToString();
                    var balance = excelDataTable.Rows[i]["差额"].ToString();
                    var confirmAmount = excelDataTable.Rows[i]["财务确认金额"].ToString();
                    var contactsReckoningDifference = excelDataTable.Rows[i]["往来差异"].ToString();

                    #region 验证数据空值
                    if (string.IsNullOrEmpty(thirdOrderNo))
                    {
                        rowMsg.Append("第").Append(index).Append("行“第三方订单号”为空！").Append("\\n");
                    }
                    if (string.IsNullOrEmpty(thirdOrderAmount))
                    {
                        rowMsg.Append("第").Append(index).Append("行“第三方订单金额”为空！").Append("\\n");
                    }
                    #endregion

                    #region 验证数据格式
                    decimal tryThirdOrderAmount;
                    if (!decimal.TryParse(thirdOrderAmount, out tryThirdOrderAmount))
                    {
                        rowMsg.Append("第").Append(index).Append("行“第三方订单金额”格式错误！").Append("\\n");
                    }
                    #endregion

                    #region 验证销售平台
                    //var goodsOrderInfoList = OrderSao.GetGoodsOrderInfoByThirdOrderNo(new Guid(Hid_SaleFiliale.Value), thirdOrderNo);
                    //if (goodsOrderInfoList.Count > 0)
                    //{
                    //    var goodsOrderInfo = goodsOrderInfoList.First();
                    //    if (!goodsOrderInfo.SalePlatformId.Equals(new Guid(rcb_SalePlatform.SelectedValue)))
                    //    {
                    //        rowMsg.Append("第").Append(index).Append("行“" + thirdOrderNo + "”与所选销售平台不符！").Append("\\n");
                    //    }
                    //}
                    //else
                    //{
                    //    rowMsg.Append("第").Append(index).Append("行“" + thirdOrderNo + "”与所选销售平台不符！").Append("\\n");
                    //}
                    #endregion

                    if (string.IsNullOrEmpty(rowMsg.ToString()))
                    {
                        #region 保存数据
                        var manuallyCheckBillDetailInfo = new ManuallyCheckBillDetailInfo
                        {
                            Id = Guid.NewGuid(),
                            ManuallyCheckBillId = Guid.Empty,
                            SystemOrderNo = string.Empty,
                            ThirdOrderNo = thirdOrderNo,
                            OrderTime = DateTime.Parse("1900-01-01"),
                            MemberId = Guid.Empty,
                            SystemOrderAmount = 0,
                            ThirdOrderAmount = decimal.Parse(thirdOrderAmount),
                            Balance = 0,
                            ConfirmAmount = 0,
                            ContactsReckoningDifference = 0
                        };
                        manuallyCheckBillDetailInfoList.Add(manuallyCheckBillDetailInfo);
                        #endregion
                    }
                    else
                    {
                        errorMsg.Append(rowMsg);
                    }

                    index++;
                }
                #endregion

                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                    return;
                }

                if (manuallyCheckBillDetailInfoList.Count > 0)
                {
                    var manuallyCheckBillInfo = new ManuallyCheckBillInfo
                    {
                        Id = Guid.NewGuid(),
                        CheckBillPersonnelId = Personnel.PersonnelId,
                        SalePlatformId = new Guid(rcb_SalePlatform.SelectedValue),
                        TradeCode = string.Empty,
                        CheckState = 0,
                        ThirdOrderTotalAmount = manuallyCheckBillDetailInfoList.Sum(p => p.ThirdOrderAmount),
                        UnusualOrderQuantity = 0,
                        ConfirmTotalAmount = 0,
                        ReceiptState = 0,
                        CheckBillDate = DateTime.Now,
                        State=0,
                        Memo = Common.WebControl.RetrunUserAndTime("[【导入】：对账原始表;]")
                    };

                    foreach (var item in manuallyCheckBillDetailInfoList)
                    {
                        item.ManuallyCheckBillId = manuallyCheckBillInfo.Id;
                    }

                    var result = _manuallyCheckBill.AddManuallyCheckBill(manuallyCheckBillInfo);
                    if (result)
                    {
                        _manuallyCheckBillDetail.AddBatchManuallyCheckBillDetail(manuallyCheckBillDetailInfoList);
                        GridDataBind();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>▄︻┻┳═一 费用发票金额   ADD 2014-12-17  陈重文
    /// </summary>
    public partial class CostInvoiceMoney : BasePage
    {

        private readonly ICostReport _costReport = new DAL.Implement.Inventory.CostReport(GlobalConfig.DB.FromType.Read);
        private readonly CostInvoiceMoneyDAL _costInvoiceMoneyDao = new CostInvoiceMoneyDAL(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadShowData();
            }
        }

        #region [加载年份月份]

        private void LoadShowData()
        {
            //加载年份
            var year = DateTime.Now.Year;
            for (var n = year - 1; n <= year + 1; n++)
            {
                RCB_Year.Items.Add(new RadComboBoxItem(string.Format("{0}", n), string.Format("{0}", n)));
            }
            RCB_Year.SelectedValue = string.Format("{0}", year);
            DateYear = year;
            //加载月份
            for (int y = 1; y < 13; y++)
            {
                RCB_Month.Items.Add(new RadComboBoxItem(string.Format("{0}", y), string.Format("{0}", y)));
            }
            RCB_Month.SelectedValue = string.Format("{0}", DateTime.Now.Month);
            DateMonth = DateTime.Now.Month;
        }

        #endregion

        #region [绑定数据源]

        protected void RadCostInvoiceMoney_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<CostInvoiceMoneyInfo> list1 = _costInvoiceMoneyDao.GetCostInvoiceMoneyList(DateYear, DateMonth);

            DateTime startTime = Convert.ToDateTime(string.Format("{0}-{1}-01 00:00:00", DateYear, DateMonth));
            DateTime endTime = startTime.AddMonths(1);
            IList<CostReportInfo> costReportList = _costReport.GetReportList(startTime, endTime).Where(ent => ent.InvoiceType == (int)CostReportInvoiceType.Invoice).OrderByDescending(r => r.ReportDate).ToList();
            var filialeList = CacheCollection.Filiale.GetHeadList();
            IList<CostInvoiceMoneyInfo> list2 = new List<CostInvoiceMoneyInfo>();
            if (list1.Count > 0)
            {
                foreach (var filialeInfo in filialeList)
                {
                    var info = new CostInvoiceMoneyInfo();
                    var item = list1.FirstOrDefault(ent => ent.FilialeId == filialeInfo.ID);
                    if (item != null)
                    {
                        info.FilialeId = filialeInfo.ID;
                        info.FilialeName = filialeInfo.Name;
                        info.Limit = item.Limit;
                    }
                    else
                    {
                        info.FilialeId = filialeInfo.ID;
                        info.FilialeName = filialeInfo.Name;
                        info.Limit = Convert.ToDecimal("0.00");
                    }
                    var list = costReportList.Where(ent => ent.AssumeFilialeId == filialeInfo.ID).ToList();
                    info.WaitCollect = GetWaitCollect(list);
                    info.WaitChargeOff = GetWaitChargeOff(list);
                    info.AlreadyChargeOff = GetAlreadyChargeOff(list);
                    list2.Add(info);
                }
                RadGridCostInvoiceMoney.DataSource = list2;
            }
            else
            {
                IList<CostInvoiceMoneyInfo> list3 = (from filialeInfo in filialeList
                                                     let list = costReportList.Where(ent => ent.AssumeFilialeId == filialeInfo.ID).ToList()
                                                     select new CostInvoiceMoneyInfo
                                                     {
                                                         FilialeId = filialeInfo.ID,
                                                         FilialeName = filialeInfo.Name,
                                                         Limit = Convert.ToDecimal("0.00"),
                                                         DateYear = DateYear,
                                                         DateMonth = DateMonth,
                                                         WaitCollect = GetWaitCollect(list),
                                                         WaitChargeOff = GetWaitChargeOff(list),
                                                         AlreadyChargeOff = GetAlreadyChargeOff(list)
                                                     }).ToList();
                RadGridCostInvoiceMoney.DataSource = list3;
            }
        }

        /// <summary>获取待收取金额
        /// </summary>
        /// <returns></returns>
        private static decimal GetWaitCollect(IList<CostReportInfo> list)
        {
            return list.Where(ent => (ent.State == (int)CostReportState.NoAuditing)).Sum(ent => ent.ReportCost);
        }

        /// <summary>获取待核销金额
        /// </summary>
        /// <returns></returns>
        private static decimal GetWaitChargeOff(IList<CostReportInfo> list)
        {
            //预借款待核销
            var beforePayCost = list.Where(ent => (ent.State == (int)CostReportState.Execute) && ent.ReportKind == (int)CostReportKind.Before).Sum(ent => ent.PayCost);
            //凭证报销待核销
            var laterPayCost = list.Where(ent => (ent.State == (int)CostReportState.Execute) && ent.ReportKind == (int)CostReportKind.Later).Sum(ent => ent.PayCost);
            //已扣款核销待核销
            var payingPayCost = list.Where(ent => (ent.State == (int)CostReportState.Execute) && ent.ReportKind == (int)CostReportKind.Paying).Sum(ent => ent.PayCost);
            return beforePayCost + laterPayCost + payingPayCost;
        }

        /// <summary>获取已核销金额
        /// </summary>
        /// <returns></returns>
        private static decimal GetAlreadyChargeOff(IList<CostReportInfo> list)
        {
            //预借款已核销
            var beforePayCost = list.Where(ent => (ent.State == (int)CostReportState.Complete) && ent.ReportKind == (int)CostReportKind.Before).Sum(ent => ent.PayCost);
            //凭证报销已核销
            var laterPayCost = list.Where(ent => (ent.State == (int)CostReportState.Complete) && ent.ReportKind == (int)CostReportKind.Later).Sum(ent => ent.PayCost);
            //已扣款核销已核销
            var payingPayCost = list.Where(ent => (ent.State == (int)CostReportState.Complete) && ent.ReportKind == (int)CostReportKind.Paying).Sum(ent => ent.PayCost);
            return beforePayCost + laterPayCost + payingPayCost;
        }

        #endregion

        #region [搜索]

        protected void OnClick_Btn_Search(object sender, EventArgs e)
        {
            DateYear = Convert.ToInt32(RCB_Year.SelectedValue);
            DateMonth = Convert.ToInt32(RCB_Month.SelectedValue);
            if (DateYear == 0 || DateMonth == 0)
            {
                RAM.Alert("系统提示：年份和月份都为必选搜索条件！");
                return;
            }
            RadGridCostInvoiceMoney.Rebind();
        }

        #endregion

        #region [文本框触发保存]

        protected void OnTextChanged_CostInvoiceLimit(object sender, EventArgs e)
        {
            try
            {
                var tbCostInvoiceLimit = (TextBox)sender;
                var dataItem = (GridDataItem)tbCostInvoiceLimit.Parent.Parent;
                var filialeId = dataItem.GetDataKeyValue("FilialeId");
                var costInvoiceLimit = WebControl.NumberRecovery(tbCostInvoiceLimit.Text);
                //正则表达式
                if (Regex.IsMatch(costInvoiceLimit, @"^(([1-9]\d{0,9})|0)(\.\d{1,2})?$"))
                {
                    var info = new CostInvoiceMoneyInfo
                    {
                        FilialeId = new Guid(filialeId.ToString()),
                        DateYear = DateYear,
                        DateMonth = DateMonth,
                        Limit = Convert.ToDecimal(costInvoiceLimit)
                    };
                    var result = _costInvoiceMoneyDao.SaveCostInvoiceMoney(info);
                    if (!result)
                    {
                        RAM.Alert("系统提示：数据保存异常，请尝试刷新后继续操作！");
                    }
                    else
                    {
                        RadGridCostInvoiceMoney.Rebind();
                    }
                }
                else
                {
                    RAM.Alert("系统提示：输入金额格式不正确！");
                }
            }
            catch (Exception)
            {
                RAM.Alert("系统提示：保存异常，请尝试刷新后继续操作！");
            }
        }

        #endregion

        /// <summary>判断是否可以保存
        /// </summary>
        /// <returns></returns>
        protected Boolean CheckIsCanSave()
        {
            var year = Convert.ToInt32(DateYear);
            var month = Convert.ToInt32(DateMonth);
            DateTime dateTime = Convert.ToDateTime(month == 12 ? string.Format("{0}-{1}-01 00:00", year + 1, 1) : string.Format("{0}-{1}-01 00:00", year, month + 1));
            return dateTime > DateTime.Now;
        }

        #region [ViewState]

        /// <summary>年份
        /// </summary>
        protected int DateYear
        {
            get
            {
                return ViewState["DateYear"] == null ? 0 : Convert.ToInt16(ViewState["DateYear"].ToString());
            }
            set
            {
                ViewState["DateYear"] = value;
            }
        }

        /// <summary>月份
        /// </summary>
        protected int DateMonth
        {
            get
            {
                return ViewState["DateMonth"] == null ? 0 : Convert.ToInt16(ViewState["DateMonth"].ToString());
            }
            set
            {
                ViewState["DateMonth"] = value;
            }
        }

        #endregion
    }
}
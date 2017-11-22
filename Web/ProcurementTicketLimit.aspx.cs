using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    /// <summary>▄︻┻┳═一 采购索票额度   ADD 2014-12-17  陈重文
    /// </summary>
    public partial class ProcurementTicketLimit : BasePage
    {
        private readonly ProcurementTicketLimitDAL _procurementTicketLimitDal = new ProcurementTicketLimitDAL(GlobalConfig.DB.FromType.Read);
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
            for (var n = year-1; n <= year + 1; n++)
            {
                RCB_Year.Items.Add(new RadComboBoxItem(n.ToString(), n.ToString()));
            }
            RCB_Year.SelectedValue = year.ToString();
            DateYear = year;
            //加载月份
            for (int y = 1; y < 13; y++)
            {
                RCB_Month.Items.Add(new RadComboBoxItem(y.ToString(), y.ToString()));
            }
            RCB_Month.SelectedValue = DateTime.Now.Month.ToString();
            DateMonth = DateTime.Now.Month;
        }

        #endregion

        #region [绑定数据源]

        protected void RadProcurementTicketLimit_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var filialeList = CacheCollection.Filiale.GetHeadList();
            IList<ProcurementTicketLimitInfo> list1 = _procurementTicketLimitDal.GetProcurementTicketLimitList(DateYear, DateMonth);

            if (list1.Count > 0)
            {
                IList<ProcurementTicketLimitInfo> list2 = new List<ProcurementTicketLimitInfo>();
                foreach (var filialeInfo in filialeList)
                {
                    var info = new ProcurementTicketLimitInfo();
                    var item = list1.FirstOrDefault(ent => ent.FilialeId == filialeInfo.ID);
                    if (item != null)
                    {
                        info.FilialeId = filialeInfo.ID;
                        info.FilialeName = filialeInfo.Name;
                        info.TotalTakerTicketLimit = item.TotalTakerTicketLimit;
                        info.DateYear = DateYear;
                        info.DateMonth = DateMonth;
                    }
                    else
                    {
                        info.FilialeId = filialeInfo.ID;
                        info.FilialeName = filialeInfo.Name;
                        info.TotalTakerTicketLimit = Convert.ToDecimal("0.00");
                        info.DateYear = DateYear;
                        info.DateMonth = DateMonth;
                    }
                    list2.Add(info);
                }
                RadGridProcurementTicketLimit.DataSource = list2.OrderByDescending(act => act.TotalTakerTicketLimit);
            }
            else
            {
                IList<ProcurementTicketLimitInfo> list3 = filialeList.Select(filialeInfo => new ProcurementTicketLimitInfo
                                                                                                {
                                                                                                    FilialeId = filialeInfo.ID,
                                                                                                    FilialeName = filialeInfo.Name,
                                                                                                    TotalTakerTicketLimit = Convert.ToDecimal("0.00"),
                                                                                                    DateYear = DateYear,
                                                                                                    DateMonth = DateMonth
                                                                                                }).ToList();
                RadGridProcurementTicketLimit.DataSource = list3;
            }
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
            RadGridProcurementTicketLimit.Rebind();
        }

        #endregion

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

        #region [Ajax刷新]

        /// <summary>Ajax刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RamOnAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RadGridProcurementTicketLimit, e);
        }

        #endregion
    }
}
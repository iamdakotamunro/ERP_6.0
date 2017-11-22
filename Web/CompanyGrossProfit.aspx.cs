using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Framework.Common;
using Telerik.Web.UI;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Implement.Inventory;
using ERP.Model.Report;
using MIS.Enum;
using ERP.UI.Web.Base;
namespace ERP.UI.Web
{
    public partial class CompanyGrossProfit : BasePage
    {
        readonly ICompanyGrossProfitRecord _companyGrossProfitRecord = new CompanyGrossProfitRecordDao();
        readonly ICompanyGrossProfitRecordDetail _companyGrossProfitRecordDetail = new CompanyGrossProfitRecordDetailDao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSaleFilialeData();//销售公司
                LoadOrderTypeData();//订单类型
                LoadSalePlatformeData();//所有销售平台及门店数据
                txt_YearAndMonth.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
                txt_StartTime.Text = DateTime.Now.ToString("yyyy-MM-01");
                txt_EndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                MessageBox.AppendScript(this, "ShowValue('" + Hid_SalePlatform.Value + "','SalePlatform');ShowValue('" + Hid_OrderType.Value + "','OrderType');");
            }
        }

        #region 属性
        public Dictionary<Guid, string> SaleFiliales
        {
            get
            {
                return ViewState["SaleFiliales"] == null
                    ? new Dictionary<Guid, string>()
                    : (Dictionary<Guid, string>)ViewState["SaleFiliales"];
            }
            set
            {
                ViewState["SaleFiliales"] = value;
            }
        }

        public Dictionary<Guid, string> SalePlatforms
        {
            get
            {
                return ViewState["SalePlatforms"] == null
                    ? new Dictionary<Guid, string>()
                    : (Dictionary<Guid, string>)ViewState["SalePlatforms"];
            }
            set
            {
                ViewState["SalePlatforms"] = value;
            }
        }

        public Dictionary<int, string> OrderType
        {
            get
            {
                return ViewState["OrderType"] == null
                    ? new Dictionary<int, string>()
                    : (Dictionary<int, string>)ViewState["OrderType"];
            }
            set
            {
                ViewState["OrderType"] = value;
            }
        }
        #endregion

        #region 数据准备
        /// <summary>
        /// 销售公司
        /// </summary>
        protected void LoadSaleFilialeData()
        {
            SaleFiliales = CacheCollection.Filiale.GetHeadList().ToDictionary(k => k.ID, v => v.Name);
            Rcb_SaleFiliale.DataSource = SaleFiliales;
            Rcb_SaleFiliale.DataTextField = "Value";
            Rcb_SaleFiliale.DataValueField = "Key";
            Rcb_SaleFiliale.DataBind();
            Rcb_SaleFiliale.Items.Insert(0, new RadComboBoxItem("全部公司", Guid.Empty.ToString()));
        }

        /// <summary>
        /// 订单类型
        /// </summary>
        protected void LoadOrderTypeData()
        {
            OrderType = new Dictionary<int, string>
            {
                {-1,"全部"},
                {0, "网络发货订单"},
                {1, "门店采购订单"},
                {2, "帮门店发货订单"}
            };
            Rcb_OrderType.DataSource = OrderType;
            Rcb_OrderType.DataTextField = "Value";
            Rcb_OrderType.DataValueField = "Key";
            Rcb_OrderType.DataBind();

            #region 默认展示“网络发货订单”，“帮门店发货订单”
            Hid_OrderType.Value = "0,2";
            MessageBox.AppendScript(this, "ShowValue('" + Hid_OrderType.Value + "','OrderType');");
            #endregion
        }

        /// <summary>
        /// 所有销售平台及门店数据
        /// </summary>
        protected void LoadSalePlatformeData()
        {
            List<SalePlatformInfo> salePlatformInfos = CacheCollection.SalePlatform.GetList().Where(ent => ent.IsActive).ToList();
            SalePlatforms = CacheCollection.Filiale.GetShopAllianceList((int)FilialeType.EntityShop).Where(ent => ent.IsActive).ToDictionary(k => k.ID, v => v.Name);
            foreach (var item in salePlatformInfos)
            {
                SalePlatforms.Add(item.ID, item.Name);
            }
        }
        #endregion

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_GrossProfit.CurrentPageIndex = 0;
            RG_GrossProfit.DataBind();
        }

        #region SelectedIndexChanged事件
        //加载销售公司对应的销售平台
        protected void Rcb_SaleFiliale_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            Rcb_SalePlatform.Items.Clear();
            var filialeId = Rcb_SaleFiliale.SelectedValue.ToGuid();
            if (filialeId == Guid.Empty)
            {
                return;
            }
            Dictionary<Guid, string> dicSalePlatformInfos = CacheCollection.SalePlatform.GetListByFilialeId(filialeId).Where(ent => ent.IsActive).ToDictionary(p => p.ID, p => p.Name);
            Dictionary<Guid, string> dicChildShop = new Dictionary<Guid, string>();
            if (filialeId != Guid.Empty)
            {
                dicChildShop = CacheCollection.Filiale.GetChildShopList(filialeId).Where(ent => ent.IsActive).ToDictionary(p => p.ID, p => p.Name);
            }

            var data = new Dictionary<Guid, string>();
            data.Add(Guid.Empty, "全部");

            foreach (var item in dicSalePlatformInfos.Union(dicChildShop))
            {
                data.Add(item.Key, item.Value);
            }

            Rcb_SalePlatform.DataSource = data;
            Rcb_SalePlatform.DataTextField = "Value";
            Rcb_SalePlatform.DataValueField = "Key";
            Rcb_SalePlatform.DataBind();
        }

        protected void ddl_TimeType_OnSelectedIndexChanged(object obj, EventArgs e)
        {
            if (ddl_TimeType.SelectedValue == "0")
            {
                //按年月查询
                lit_TimeTitle.Text = "年月：";
                lit_TimeTitle.Visible = true;
                txt_YearAndMonth.Visible = true;
                currentMonth.Visible = false;
                txt_YearAndMonth.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
                txt_StartTime.Text = string.Empty;
                txt_EndTime.Text = string.Empty;
            }
            else if (ddl_TimeType.SelectedValue == "1")
            {
                //按当月查询
                lit_TimeTitle.Text = "当月查询：";
                lit_TimeTitle.Visible = true;
                txt_YearAndMonth.Visible = false;
                currentMonth.Visible = true;
                txt_YearAndMonth.Text = string.Empty;
                txt_StartTime.Text = DateTime.Now.ToString("yyyy-MM-01");
                txt_EndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
        }
        #endregion

        #region 数据列表相关
        protected void RG_GrossProfit_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (!IsPostBack)
            {
                RG_GrossProfit.DataSource = new List<CompanyGrossProfitRecordInfo>();
            }
            else
            {
                GridDataBind();
            }
        }

        //Grid数据源
        protected void GridDataBind()
        {
            try
            {
                IList<CompanyGrossProfitRecordInfo> dataList = new List<CompanyGrossProfitRecordInfo>();
                List<CompanyGrossProfitRecordInfo> sumDataList = new List<CompanyGrossProfitRecordInfo>();//汇总同一公司不同平台的数据
                List<CompanyGrossProfitRecordInfo> sumOrderTypeDataList = new List<CompanyGrossProfitRecordInfo>();//汇总同一公司同一订单类型不同平台的数据
                Guid saleFilialeId = string.IsNullOrEmpty(Rcb_SaleFiliale.SelectedValue) ? Guid.Empty : new Guid(Rcb_SaleFiliale.SelectedValue);
                DateTime startTime;
                DateTime endTime;
                if (ddl_TimeType.SelectedValue == "1")//当月查询
                {
                    startTime = DateTime.Parse(txt_StartTime.Text);
                    endTime = DateTime.Now;
                    if (!string.IsNullOrEmpty(txt_EndTime.Text))
                    {
                        endTime = DateTime.Parse(txt_EndTime.Text);
                    }
                    endTime = endTime.AddDays(1);

                    #region 汇总同一公司不同平台的数据
                    sumDataList = _companyGrossProfitRecordDetail.SumCompanyGrossProfitBySaleFilialeId(startTime, endTime, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value).ToList();
                    #endregion

                    #region 汇总同一公司同一订单类型不同平台的数据
                    if (rbl_OrderType.SelectedValue.Equals("1"))
                    {
                        sumOrderTypeDataList = _companyGrossProfitRecordDetail.SumCompanyGrossProfitBySaleFilialeIdAndOrderType(startTime, endTime, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value).ToList();
                    }
                    #endregion
                    else
                    {
                        #region 将查询时间段内每天的数据根据条件合并
                        dataList = _companyGrossProfitRecordDetail.SumCompanyGrossProfitDetailInfos(startTime, endTime, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value);
                        #endregion
                    }
                }
                else if (ddl_TimeType.SelectedValue == "0")
                {
                    startTime = DateTime.Parse(txt_YearAndMonth.Text);
                    if (startTime.AddMonths(1).Equals(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01"))))
                    {
                        endTime = startTime.AddMonths(1);

                        #region 汇总同一公司不同平台的数据
                        sumDataList = _companyGrossProfitRecordDetail.SumCompanyGrossProfitBySaleFilialeId(startTime, endTime, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value).ToList();
                        #endregion

                        #region 汇总同一公司同一订单类型不同平台的数据
                        if (rbl_OrderType.SelectedValue.Equals("1"))
                        {
                            sumOrderTypeDataList = _companyGrossProfitRecordDetail.SumCompanyGrossProfitBySaleFilialeIdAndOrderType(startTime, endTime, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value).ToList();
                        }
                        #endregion
                        else
                        {
                            #region 将查询时间段内每天的数据根据条件合并
                            dataList = _companyGrossProfitRecordDetail.SumCompanyGrossProfitDetailInfos(startTime, endTime, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value);
                            #endregion
                        }
                    }
                    else
                    {
                        #region 汇总同一公司不同平台的数据
                        sumDataList = _companyGrossProfitRecord.SumCompanyGrossProfitFromMonthBySaleFilialeId(startTime, null, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value).ToList();
                        #endregion

                        #region 汇总同一公司同一订单类型不同平台的数据
                        if (rbl_OrderType.SelectedValue.Equals("1"))
                        {
                            sumOrderTypeDataList = _companyGrossProfitRecord.SumCompanyGrossProfitFromMonthBySaleFilialeIdAndOrderType(startTime, null, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value).ToList();
                        }
                        #endregion
                        else
                        {
                            dataList = _companyGrossProfitRecord.SelectCompanyGrossProfitInfos(startTime, null, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value);
                        }
                    }
                }

                #region 组合数据
                List<CompanyGrossProfitRecordInfo> list = sumDataList;
                if (rbl_OrderType.SelectedValue.Equals("1"))
                {
                    list.AddRange(sumOrderTypeDataList);
                }
                else
                {
                    list.AddRange(dataList);
                }
                #endregion

                RG_GrossProfit.DataSource = list.OrderByDescending(act => act.SaleFilialeId).ThenBy(act => act.SalesAmount).ThenByDescending(act => act.SalePlatformId).ThenByDescending(act => act.OrderType);

                #region 合计
                var totalName = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("TotalName");
                var salesAmount = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("SalesAmount");
                var goodsAmount = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("GoodsAmount");
                var shipmentIncome = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("ShipmentIncome");
                var promotionsDeductible = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("PromotionsDeductible");
                var pointsDeduction = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("PointsDeduction");
                var shipmentCost = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("ShipmentCost");
                var purchaseCosts = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("PurchaseCosts");
                var profit = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("Profit");
                var profitMargin = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("ProfitMargin");

                if (list.Any())
                {
                    var sumSalesAmount = list.Sum(act => act.SalesAmount) / 2;
                    var sumGoodsAmount = list.Sum(act => act.GoodsAmount) / 2;
                    var sumShipmentIncome = list.Sum(act => act.ShipmentIncome) / 2;
                    var sumPromotionsDeductible = list.Sum(act => act.PromotionsDeductible) / 2;
                    var sumPointsDeduction = list.Sum(act => act.PointsDeduction) / 2;
                    var sumShipmentCost = list.Sum(act => act.ShipmentCost) / 2;
                    var sumPurchaseCosts = list.Sum(act => act.PurchaseCosts) / 2;
                    var sumProfit = (sumSalesAmount - sumShipmentIncome) - sumPurchaseCosts;
                    var sumProfitMargin = sumProfit / (sumSalesAmount - sumShipmentIncome);

                    totalName.FooterText = "公司合计：";
                    salesAmount.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumSalesAmount - sumShipmentIncome));
                    goodsAmount.FooterText = string.Format("<span style=\"color: blue;\">(&nbsp;</span>{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumGoodsAmount));
                    shipmentIncome.FooterText = string.Format("<span style=\"color: blue;\">+&nbsp;</span>{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumShipmentIncome));
                    promotionsDeductible.FooterText = string.Format("<span style=\"color: blue;\">-&nbsp;</span>{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumPromotionsDeductible));
                    pointsDeduction.FooterText = string.Format("<span style=\"color: blue;\">-&nbsp;</span>{0}<span style=\"color: blue;\">&nbsp;)</span>", ERP.UI.Web.Common.WebControl.NumberSeparator(sumPointsDeduction));
                    shipmentCost.FooterText = string.Format("<span style=\"color: blue;\">-&nbsp;</span>{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumShipmentCost));
                    purchaseCosts.FooterText = string.Format("<span style=\"color: blue;\">-&nbsp;</span>{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumPurchaseCosts));
                    profit.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumProfit));
                    profitMargin.FooterText = string.Format("{0}{1}", (sumProfitMargin * 100).ToString("F"), "%");
                }
                else
                {
                    salesAmount.FooterText = string.Empty;
                    goodsAmount.FooterText = string.Empty;
                    shipmentIncome.FooterText = string.Empty;
                    promotionsDeductible.FooterText = string.Empty;
                    pointsDeduction.FooterText = string.Empty;
                    shipmentCost.FooterText = string.Empty;
                    purchaseCosts.FooterText = string.Empty;
                    profit.FooterText = string.Empty;
                    profitMargin.FooterText = string.Empty;
                }
                #endregion
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError("公司毛利查询异常", "财务管理", ex);
            }
        }

        //行绑定事件
        protected void RG_GrossProfit_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var salePlatformId = DataBinder.Eval(e.Item.DataItem, "SalePlatformId");
                var orderType = DataBinder.Eval(e.Item.DataItem, "OrderType");
                if (new Guid(salePlatformId.ToString()).Equals(Guid.Empty) && int.Parse(orderType.ToString()).Equals(-1))
                {
                    e.Item.Style["background-color"] = "#FFFFCC";
                    e.Item.Style["font-weight"] = "bold";
                }
            }
            else if (e.Item.ItemType == GridItemType.Footer)
            {
                e.Item.Style["background-color"] = "#FFCC99";
                e.Item.Style["font-weight"] = "bold";
            }
        }
        #endregion

        //导出Excel
        protected void btn_Export_Click(object sender, EventArgs e)
        {
            string yearAndMonth = string.Empty;
            if (!string.IsNullOrEmpty(txt_StartTime.Text))
            {
                yearAndMonth = DateTime.Parse(txt_StartTime.Text).ToString("yyyy-MM");
            }
            else if (!string.IsNullOrEmpty(txt_YearAndMonth.Text))
            {
                yearAndMonth = DateTime.Parse(txt_YearAndMonth.Text).ToString("yyyy-MM");
            }
            if (!string.IsNullOrEmpty(yearAndMonth))
            {
                GridDataBind();
                RG_GrossProfit.DataBind();
                string fileName = string.Format("{0}公司毛利详细", yearAndMonth);
                fileName = Server.UrlEncode(fileName);
                RG_GrossProfit.ExportSettings.ExportOnlyData = true;
                RG_GrossProfit.HorizontalAlign = HorizontalAlign.Right;
                RG_GrossProfit.ExportSettings.IgnorePaging = true;
                RG_GrossProfit.ExportSettings.FileName = fileName;
                RG_GrossProfit.MasterTableView.ExportToExcel();
            }
        }
    }
}
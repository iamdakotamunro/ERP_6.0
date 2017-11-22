using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.Model.Report;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;
using MIS.Enum;
using GlobalConfig = ERP.Environment.GlobalConfig;
using ERP.Enum;
using ERP.Enum.Attribute;

namespace ERP.UI.Web
{
    /// <summary>
    /// 商品毛利记录
    /// </summary>
    public partial class GoodsGrossProfit : BasePage
    {
        private readonly GoodsGrossProfitBll _goodsGrossProfitBll = new GoodsGrossProfitBll(new GoodsCenterSao(), new GoodsGrossProfitDao(), new GoodsOrderDetail(GlobalConfig.DB.FromType.Read), new PromotionSao(), new GoodsGrossProfitRecordDetailDao());
        readonly GoodsGrossProfitRecordDetailBll _goodsGrossProfitRecordDetailBll = new GoodsGrossProfitRecordDetailBll(new GoodsGrossProfitRecordDetailDao(), new GoodsCenterSao());
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadSaleFilialeData();//销售公司
                LoadGoodsTypeData();//商品类型
                LoadOrderTypeData();//订单类型
                LoadSalePlatformeData();//所有销售平台及门店数据
                txt_YearAndMonth.Text = DateTime.Now.AddMonths(-1).ToString("yyyy-MM");
                txt_StartTime.Text = DateTime.Now.ToString("yyyy-MM-01");
                txt_EndTime.Text = DateTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                MessageBox.AppendScript(this, "ShowValue('" + Hid_GoodsType.Value + "','GoodsType');ShowValue('" + Hid_SalePlatform.Value + "','SalePlatform');ShowValue('" + Hid_OrderType.Value + "','OrderType');");
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
        /// 商品类型
        /// </summary>
        protected void LoadGoodsTypeData()
        {
            var goodsTypes = EnumAttribute.GetDict<GoodsKindType>().Where(act => act.Key > 0);
            IDictionary<int, string> date = new Dictionary<int, string>();
            date.Add(0, "全部");
            Rcb_GoodsType.DataSource = date.Union(goodsTypes);
            Rcb_GoodsType.DataTextField = "Value";
            Rcb_GoodsType.DataValueField = "Key";
            Rcb_GoodsType.DataBind();
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
                RG_GrossProfit.DataSource = new List<GoodsGrossProfitInfo>();
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
                IList<GoodsGrossProfitInfo> dataList = new List<GoodsGrossProfitInfo>();
                List<GoodsGrossProfitInfo> sumSaleFilialeDataList = new List<GoodsGrossProfitInfo>();//汇总同一商品同一公司不同平台的数据
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
                    
                    #region 汇总同一商品同一公司不同平台的数据
                    if (saleFilialeId.Equals(Guid.Empty) && !ckb_GoodsType.Checked)
                    {
                        sumSaleFilialeDataList = _goodsGrossProfitRecordDetailBll.SumGoodsGrossProfitByGoodsIdAndSaleFilialeId(startTime, endTime, Hid_GoodsType.Value,txt_GoodsCode.Text, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value).ToList();
                    }
                    #endregion
                    else
                    {
                        dataList = _goodsGrossProfitRecordDetailBll.SumGoodsGrossProfitRecordDetailInfos(startTime, endTime, Hid_GoodsType.Value, txt_GoodsCode.Text, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value);
                    }
                }
                else if (ddl_TimeType.SelectedValue == "0")
                {
                    startTime = DateTime.Parse(txt_YearAndMonth.Text);
                    if (startTime.AddMonths(1).Equals(DateTime.Parse(DateTime.Now.ToString("yyyy-MM-01"))))
                    {
                        endTime = startTime.AddMonths(1);
                        
                        #region 汇总同一商品同一公司不同平台的数据
                        if (saleFilialeId.Equals(Guid.Empty) && !ckb_GoodsType.Checked)
                        {
                            sumSaleFilialeDataList = _goodsGrossProfitRecordDetailBll.SumGoodsGrossProfitByGoodsIdAndSaleFilialeId(startTime, endTime, Hid_GoodsType.Value, txt_GoodsCode.Text, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value).ToList();
                        }
                        #endregion
                        else
                        {
                            dataList = _goodsGrossProfitRecordDetailBll.SumGoodsGrossProfitRecordDetailInfos(startTime, endTime, Hid_GoodsType.Value, txt_GoodsCode.Text, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value);
                        }
                    }
                    else
                    {
                        #region 汇总同一商品同一公司不同平台的数据
                        if (saleFilialeId.Equals(Guid.Empty) && !ckb_GoodsType.Checked)
                        {
                            sumSaleFilialeDataList = _goodsGrossProfitBll.SumGoodsGrossProfitFromMonthByGoodsIdAndSaleFilialeId(startTime, DateTime.MinValue, Hid_GoodsType.Value, txt_GoodsCode.Text, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value).ToList();
                        }
                        #endregion
                        else
                        {
                            dataList = _goodsGrossProfitBll.SelectGoodsGrossProfitInfos(startTime, DateTime.MinValue, Hid_GoodsType.Value, txt_GoodsCode.Text, saleFilialeId, Hid_SalePlatform.Value, Hid_OrderType.Value);
                        }
                    }
                }

                RG_GrossProfit.Columns[0].Visible = true;
                RG_GrossProfit.Columns[1].Visible = true;
                RG_GrossProfit.Columns[2].Visible = false;
                RG_GrossProfit.Columns[7].Visible = true;
                RG_GrossProfit.Columns[9].Visible = true;
                IList<GoodsGrossProfitInfo> list = dataList;
                if (saleFilialeId.Equals(Guid.Empty))
                {
                    list = sumSaleFilialeDataList;
                    RG_GrossProfit.Columns[7].Visible = false;
                    RG_GrossProfit.Columns[9].Visible = false;
                }
                if (ckb_GoodsType.Checked)
                {
                    #region 汇总同一商品类型同一公司不同平台的数据
                    List<GoodsGrossProfitInfo> sumGoodsTypeDataList = new List<GoodsGrossProfitInfo>();
                    if (dataList.Any())
                    {
                        foreach (var item in dataList)
                        {
                            var info = sumGoodsTypeDataList.FirstOrDefault(act => act.GoodsType == item.GoodsType && act.SaleFilialeId == item.SaleFilialeId);
                            if (info != null)
                            {
                                info.SalesPriceTotal += item.SalesPriceTotal;
                                info.PurchaseCostTotal += item.PurchaseCostTotal;
                                info.Quantity += item.Quantity;
                            }
                            else
                            {
                                sumGoodsTypeDataList.Add(new GoodsGrossProfitInfo
                                {
                                    GoodsId = Guid.Empty,
                                    SaleFilialeId = item.SaleFilialeId,
                                    SalePlatformId = Guid.Empty,
                                    OrderType = -1,
                                    SalesPriceTotal = item.SalesPriceTotal,
                                    PurchaseCostTotal = item.PurchaseCostTotal,
                                    Quantity = item.Quantity,
                                    GoodsName = string.Empty,
                                    GoodsCode = string.Empty,
                                    GoodsType = item.GoodsType
                                });
                            }
                        }
                    }
                    #endregion

                    list = sumGoodsTypeDataList;
                    RG_GrossProfit.Columns[0].Visible = false;
                    RG_GrossProfit.Columns[1].Visible = false;
                    RG_GrossProfit.Columns[2].Visible = true;
                    RG_GrossProfit.Columns[7].Visible = false;
                    RG_GrossProfit.Columns[9].Visible = false;
                }

                RG_GrossProfit.DataSource = list.OrderByDescending(act => act.SaleFilialeId).ThenByDescending(act => act.SalesPriceTotal);

                #region 合计
                var salesPriceTotal = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("SalesPriceTotal");
                var purchaseCostTotal = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("PurchaseCostTotal");
                var grossProfit = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("GrossProfit");
                var grossProfitMargin = RG_GrossProfit.MasterTableView.Columns.FindByUniqueName("GrossProfitMargin");

                if (list.Any())
                {
                    var sumPurchaseCostTotal = list.Sum(act => act.PurchaseCostTotal);
                    var sumSalesPriceTotal = list.Sum(act => act.SalesPriceTotal);
                    var sumGrossProfit = sumSalesPriceTotal - sumPurchaseCostTotal;
                    var sumGrossProfitMargin = sumGrossProfit / sumSalesPriceTotal;

                    salesPriceTotal.FooterText = " 合 计：" + string.Format("{0}", WebControl.NumberSeparator(sumSalesPriceTotal));
                    purchaseCostTotal.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumPurchaseCostTotal));
                    grossProfit.FooterText = string.Format("{0}", WebControl.NumberSeparator(sumGrossProfit));
                    grossProfitMargin.FooterText = string.Format("{0}{1}", (sumGrossProfitMargin * 100).ToString("F"), "%");
                }
                else
                {
                    salesPriceTotal.FooterText = string.Empty;
                    purchaseCostTotal.FooterText = string.Empty;
                    grossProfit.FooterText = string.Empty;
                    grossProfitMargin.FooterText = string.Empty;
                }
                #endregion
            }
            catch (Exception ex)
            {
                SAL.LogCenter.LogService.LogError("商品毛利查询异常", "财务管理", ex);
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
                string fileName = string.Format("{0}商品毛利详细", yearAndMonth);
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
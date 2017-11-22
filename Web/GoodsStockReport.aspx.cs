using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Storage;
using ERP.DAL.Interface.IStorage;
using ERP.Model.Report;
using ERP.UI.Web.Base;
using KeedeGroup.GoodsManageSystem.Public.Attribute;
using KeedeGroup.GoodsManageSystem.Public.Enum;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;
using ERP.UI.Web.Common;
using ERP.BLL.Implement.Inventory;

namespace ERP.UI.Web
{
    public partial class GoodsStockReport : BasePage
    {
        private static readonly IGoodsStockRecord _goodsStockRecord = new GoodsStockRecordDao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadWarehouseData();//加载仓库列表
                txt_YearAndMonth.Text = DateTime.Now.Year.ToString();
            }
        }

        #region 属性
        /// <summary>
        /// 商品类型枚举
        /// </summary>
        protected IDictionary<int, string> GoodsTypeDics
        {
            get
            {
                return ViewState["GoodsTypeDics"] == null ? EnumAttribute.GetDict<GoodsType>()
                    : (Dictionary<int, string>)ViewState["GoodsTypeDics"];
            }
            set
            {
                ViewState["GoodsTypeDics"] = value;
            }
        }
        #endregion

        #region 数据准备
        //仓库
        protected void LoadWarehouseData()
        {
            ddl_Warehouse.DataSource = GetWorehouseList();
            ddl_Warehouse.DataTextField = "Value";
            ddl_Warehouse.DataValueField = "Key";
            ddl_Warehouse.DataBind();
        }

        /// <summary>
        /// 根据登录人获取授权仓库
        /// </summary>
        /// <returns></returns>
        public Dictionary<Guid, string> GetWorehouseList()
        {
            var dic = new Dictionary<Guid, string> { { Guid.Empty, "请选择" } };
            var personnerInfo = CurrentSession.Personnel.Get();
            var warehouseList = WarehouseManager.GetWarehouseIsPermission(personnerInfo.PersonnelId);
            foreach (var info in warehouseList)
            {
                dic.Add(info.WarehouseId, info.WarehouseName);
            }
            return dic;
        }
        #endregion

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_GoodsStock.CurrentPageIndex = 0;
            RG_GoodsStock.DataBind();
        }

        #region 数据列表相关
        protected void RG_GoodsStock_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            var warehouseId = Guid.Parse(ddl_Warehouse.SelectedValue);
            IList<MonthGoodsStockReportInfo> dataList;

            // B5BCDF6E-95D5-4AEE-9B19-6EE218255C05 松江主仓库
            // 84B303F5-2AA6-437D-9D23-3488AD55D278 中创路主仓库
            var warehouseId1 = new Guid("B5BCDF6E-95D5-4AEE-9B19-6EE218255C05");
            var warehouseId2 = new Guid("84B303F5-2AA6-437D-9D23-3488AD55D278");
            if (warehouseId.Equals(warehouseId1) || warehouseId.Equals(warehouseId2))
            {
                #region 松江主仓库 or 中创路主仓库
                IList<MonthGoodsStockReportInfo> monthGoodsStockReportList1 = _goodsStockRecord.SelectMonthGoodsStockReportInfos(int.Parse(txt_YearAndMonth.Text), warehouseId1);
                IList<MonthGoodsStockReportInfo> monthGoodsStockReportList2 = _goodsStockRecord.SelectMonthGoodsStockReportInfos(int.Parse(txt_YearAndMonth.Text), warehouseId2);
                if (warehouseId.Equals(warehouseId1))
                {
                    if (monthGoodsStockReportList1.Any())
                    {
                        foreach (var item1 in monthGoodsStockReportList1)
                        {
                            var model = monthGoodsStockReportList2.FirstOrDefault(p => p.GoodsType.Equals(item1.GoodsType));
                            if (model != null)
                            {
                                item1.January += model.January;
                                item1.February += model.February;
                                item1.March += model.March;
                                item1.April += model.April;
                                item1.May += model.May;
                                item1.June += model.June;
                                item1.July += model.July;
                                item1.August += model.August;
                                item1.September += model.September;
                                item1.October += model.October;
                                item1.November += model.November;
                                item1.December += model.December;
                            }
                        }
                        dataList = monthGoodsStockReportList1;
                    }
                    else
                    {
                        dataList = monthGoodsStockReportList2;
                    }
                }
                else
                {
                    if (monthGoodsStockReportList2.Any())
                    {
                        foreach (var item1 in monthGoodsStockReportList2)
                        {
                            var model = monthGoodsStockReportList1.FirstOrDefault(p => p.GoodsType.Equals(item1.GoodsType));
                            if (model != null)
                            {
                                item1.January += model.January;
                                item1.February += model.February;
                                item1.March += model.March;
                                item1.April += model.April;
                                item1.May += model.May;
                                item1.June += model.June;
                                item1.July += model.July;
                                item1.August += model.August;
                                item1.September += model.September;
                                item1.October += model.October;
                                item1.November += model.November;
                                item1.December += model.December;
                            }
                        }
                        dataList = monthGoodsStockReportList2;
                    }
                    else
                    {
                        dataList = monthGoodsStockReportList1;
                    }
                }
                #endregion
            }
            else
            {
                dataList = _goodsStockRecord.SelectMonthGoodsStockReportInfos(int.Parse(txt_YearAndMonth.Text), warehouseId);
            }


            RG_GoodsStock.DataSource = dataList;

            #region 合计
            var monthName = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("MonthName");
            var january = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("January");
            var february = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("February");
            var march = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("March");
            var april = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("April");
            var may = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("May");
            var june = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("June");
            var july = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("July");
            var august = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("August");
            var september = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("September");
            var october = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("October");
            var november = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("November");
            var december = RG_GoodsStock.MasterTableView.Columns.FindByUniqueName("December");

            if (dataList.Any())
            {
                var sumJanuary = dataList.Sum(act => act.January);
                var sumFebruary = dataList.Sum(act => act.February);
                var sumMarch = dataList.Sum(act => act.March);
                var sumApril = dataList.Sum(act => act.April);
                var sumMay = dataList.Sum(act => act.May);
                var sumJune = dataList.Sum(act => act.June);
                var sumJuly = dataList.Sum(act => act.July);
                var sumAugust = dataList.Sum(act => act.August);
                var sumSeptember = dataList.Sum(act => act.September);
                var sumOctober = dataList.Sum(act => act.October);
                var sumNovember = dataList.Sum(act => act.November);
                var sumDecember = dataList.Sum(act => act.December);

                monthName.FooterText = "合计：";
                january.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumJanuary));
                february.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumFebruary));
                march.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumMarch));
                april.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumApril));
                may.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumMay));
                june.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumJune));
                july.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumJuly));
                august.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumAugust));
                september.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumSeptember));
                october.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumOctober));
                november.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumNovember));
                december.FooterText = string.Format("{0}", ERP.UI.Web.Common.WebControl.NumberSeparator(sumDecember));
            }
            else
            {
                january.FooterText = string.Empty;
                february.FooterText = string.Empty;
                march.FooterText = string.Empty;
                april.FooterText = string.Empty;
                may.FooterText = string.Empty;
                june.FooterText = string.Empty;
                july.FooterText = string.Empty;
                august.FooterText = string.Empty;
                september.FooterText = string.Empty;
                october.FooterText = string.Empty;
                november.FooterText = string.Empty;
                december.FooterText = string.Empty;
            }
            #endregion
        }

        #region 列表显示辅助方法
        /// <summary>
        /// 获取商品类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string GetTypeName(object obj)
        {
            if (obj == null) return string.Empty;
            int type = Convert.ToInt32(obj);
            if (type == 0) return "未知类型";
            return GoodsTypeDics.ContainsKey(type) ? GoodsTypeDics[type] : string.Empty;
        }
        #endregion
        #endregion
    }
}
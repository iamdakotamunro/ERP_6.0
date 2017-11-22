using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using MIS.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.UI.Web.Base;
using ERP.SAL.WMS;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>出入库商品汇总
    /// </summary>
    public partial class GoodsStockInAndOutAw : BasePage
    {
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetCompanyCussent();
                GetWarehouseList();
                LoadStorageType();
                CompanyId = Guid.Empty;
                DdlSaleFiliale.DataSource = null;
                DdlSaleFiliale.DataBind();


                var yearlist = new List<int>();
                for (var i = 2007; i <= (DateTime.Now.Year - GlobalConfig.KeepYear); i++)
                {
                    yearlist.Add(i);
                }
                DDL_Years.DataSource = yearlist.OrderByDescending(y => y);
                DDL_Years.DataBind();
                DDL_Years.Items.Add(new ListItem(GlobalConfig.KeepYear + "年内数据", "0"));
                DDL_Years.SelectedValue = "0";

                YearSelected();
            }
        }

        protected void RCBWarehouse_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var cb = (RadComboBox)sender;
            if (cb != null)
            {
                var warehouse = WarehouseAuth.FirstOrDefault(act => act.WarehouseId == new Guid(cb.SelectedItem.Value));
                var filiales = new List<HostingFilialeAuth> { new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "请选择物流公司" } };
                if (warehouse != null)
                {
                    foreach (var hostingFilialeAuth in warehouse.FilialeAuths)
                    {
                        if (filiales.Any(act => act.HostingFilialeId == hostingFilialeAuth.HostingFilialeId)) continue;
                        filiales.Add(hostingFilialeAuth);
                        foreach (var proxyFiliale in hostingFilialeAuth.ProxyFiliales.Where(proxyFiliale => filiales.All(ent => ent.HostingFilialeId != proxyFiliale.ProxyFilialeId)))
                        {
                            filiales.Add(new HostingFilialeAuth
                            {
                                HostingFilialeId = proxyFiliale.ProxyFilialeId,
                                HostingFilialeName = proxyFiliale.ProxyFilialeName
                            });
                        }
                    }
                }
                if (filiales.Count == 1)
                {
                    DdlSaleFiliale.Items.Clear();
                    return;
                }
                DdlSaleFiliale.DataSource = filiales.ToDictionary(k => k.HostingFilialeId, v => v.HostingFilialeName);
                DdlSaleFiliale.DataBind();
            }
        }

        protected void RCBSaleFiliale_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            RTVCompanyCussent.Nodes.Clear();
            GetCompanyCussent();
        }

        private void LoadStorageType()
        {
            var dict = EnumAttribute.GetDict<StorageRecordType>();
            RCB_StorageType.Items.Clear();
            RCB_StorageType.Items.Add(new RadComboBoxItem("全部类型", "-1"));
            foreach (var t in dict)
            {
                RCB_StorageType.Items.Add(new RadComboBoxItem(t.Value, string.Format("{0}", t.Key)));
            }

        }

        //遍历往来的单位
        private void GetCompanyCussent()
        {
            RadTreeNode rootNode = CreateNode("往来单位选择", "往来单位", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            RTVCompanyCussent.Nodes.Add(rootNode);
            RecursivelyCompanyClass(Guid.Empty, rootNode);
        }

        private void RecursivelyFiliale(RadTreeNode node)
        {
            var filialeList = MISService.GetAllFiliales().Where(act => act.IsActive && act.ID != new Guid(DdlSaleFiliale.SelectedItem.Value) && (act.FilialeTypes.Contains((int)FilialeType.SaleCompany) || act.FilialeTypes.Contains((int)FilialeType.LogisticsCompany))).ToList();
            foreach (FilialeInfo info in filialeList)
            {
                RadTreeNode childNode = CreateNode(info.Name, "HostingCompany", false, info.ID.ToString());
                childNode.PostBack = true;
                node.Nodes.Add(childNode);
            }
        }

        //遍历子公司
        private void RecursivelyCompanyClass(Guid companyClassId, RadTreeNode node)
        {
            var companyClass = new CompanyClass(GlobalConfig.DB.FromType.Read);
            IList<CompanyClassInfo> childCompanyClassList = companyClass.GetChildCompanyClassList(companyClassId);
            foreach (CompanyClassInfo companyClassInfo in childCompanyClassList)
            {
                if (string.IsNullOrWhiteSpace(companyClassInfo.CompanyClassName)) continue;
                RadTreeNode childNode = CreateNode(companyClassInfo.CompanyClassName, "BusinessRelatedUnits", false, companyClassInfo.CompanyClassId.ToString());
                childNode.PostBack = true;
                node.Nodes.Add(childNode);
                RecursivelyCompanyClass(companyClassInfo.CompanyClassId, childNode);
                RepetitionCompanyCussent(companyClassInfo.CompanyClassId, childNode);
            }
        }

        //遍历部门
        private void RepetitionCompanyCussent(Guid companyClassId, RadTreeNode node)
        {
            IList<CompanyCussentInfo> companyCussentList = _companyCussent.GetCompanyCussentList(companyClassId);
            foreach (CompanyCussentInfo companyCussentInfo in companyCussentList)
            {
                RadTreeNode childNode = CreateNode(companyCussentInfo.CompanyName, "BusinessRelatedUnits", false, companyCussentInfo.CompanyId.ToString());
                childNode.PostBack = true;
                childNode.Enabled = (State)companyCussentInfo.State == State.Enable;
                //childNode.ExpandMode = ExpandMode.ServerSideCallBack;
                node.Nodes.Add(childNode);
            }
        }

        //创建节点
        private RadTreeNode CreateNode(string text, string toolTip, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = toolTip, Expanded = expanded };
            return node;
        }

        private List<WarehouseFilialeAuth> WarehouseAuth
        {
            get
            {
                if (ViewState["WarehouseAuth"] == null)
                {
                    return WMSSao.GetWarehouseAndFilialeAuth(Personnel.PersonnelId);
                }
                return (List<WarehouseFilialeAuth>)ViewState["WarehouseAuth"];
            }
        }


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

        //选择往来单位分类树节点
        protected decimal WarningNumber;

        protected void StockGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            decimal total = 0;
            int totalQuantity = 0;
            if (IsPostBack)
            {
                WarehouseId = new Guid(RCB_Warehouse.SelectedValue);
                var goodsList = _storageRecordDao.GetGoodsOutInStorageStatisticsList(StorageType, StartTime, EndTime, CompanyId, GlobalConfig.KeepYear, HostingFilialeId, WarehouseId);
                StockGrid.DataSource = goodsList;
                foreach (var goodsOutInStorageStatisticsInfo in goodsList)
                {
                    if (goodsOutInStorageStatisticsInfo.StorageType == (int)StorageRecordType.BuyStockIn
                        || goodsOutInStorageStatisticsInfo.StorageType == (int)StorageRecordType.BorrowIn
                        || goodsOutInStorageStatisticsInfo.StorageType == (int)StorageRecordType.LendIn
                        || goodsOutInStorageStatisticsInfo.StorageType == (int)StorageRecordType.SellReturnIn)
                    {
                        total += Math.Abs(goodsOutInStorageStatisticsInfo.TotalPrice);
                        totalQuantity += goodsOutInStorageStatisticsInfo.Quantity;
                    }
                    if (goodsOutInStorageStatisticsInfo.StorageType == (int)StorageRecordType.BuyStockOut
                        || goodsOutInStorageStatisticsInfo.StorageType == (int)StorageRecordType.AfterSaleOut
                        || goodsOutInStorageStatisticsInfo.StorageType == (int)StorageRecordType.BorrowOut
                        || goodsOutInStorageStatisticsInfo.StorageType == (int)StorageRecordType.LendOut
                        || goodsOutInStorageStatisticsInfo.StorageType == (int)StorageRecordType.SellStockOut)
                    {
                        total -= Math.Abs(goodsOutInStorageStatisticsInfo.TotalPrice);
                        totalQuantity -= goodsOutInStorageStatisticsInfo.Quantity;
                    }
                }
                TotalAmount = WebControl.NumberSeparator(total);
                TotalQuantity = WebControl.NumberSeparator(totalQuantity);
            }
            else
            {
                StockGrid.DataSource = new List<GoodsOutInStorageStatisticsInfo>();
                TotalAmount = "0";
            }
            var totalPrice = StockGrid.MasterTableView.Columns.FindByUniqueName("ToalAmount");
            var quantity = StockGrid.MasterTableView.Columns.FindByUniqueName("Quantity");
            totalPrice.FooterText = total > 0 ? string.Format("合计：{0}", WebControl.NumberSeparator(total)) : string.Empty;
            quantity.FooterText = totalQuantity > 0 ? string.Format("合计：{0}", WebControl.NumberSeparator(totalQuantity)) : string.Empty;
        }
        //仓库ID
        protected Guid WarehouseId
        {
            get
            {
                if (ViewState["WarehouseId"] == null) return Guid.Empty;
                return new Guid(ViewState["WarehouseId"].ToString());
            }
            set
            {
                ViewState["WarehouseId"] = value.ToString();
            }
        }
        //往来单位编号
        protected Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null) return Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set { ViewState["CompanyId"] = value; }
        }

        //物流配送公司Id
        protected Guid HostingFilialeId
        {
            get
            {
                return ViewState["HostingFilialeId"] == null ? Guid.Empty
                    : new Guid(ViewState["HostingFilialeId"].ToString());
            }
            set
            {
                ViewState["HostingFilialeId"] = value.ToString();
            }
        }

        protected string GetStockType(int stockType)
        {
            return EnumAttribute.GetKeyName((StorageRecordType)stockType);
        }

        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set { ViewState["StartTime"] = value; }
        }

        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
            }
            set { ViewState["EndTime"] = value; }
        }

        protected string OrderNof
        {
            get
            {
                if (ViewState["OrderNOF"] == null) return null;
                return ViewState["OrderNOF"].ToString();
            }
            set { ViewState["OrderNOF"] = value; }
        }

        protected int StorageType
        {
            get
            {
                if (ViewState["StorageType"] == null) return 0;
                return Convert.ToInt32(ViewState["StorageType"]);
            }
            set { ViewState["StorageType"] = value; }
        }

        protected string TotalAmount
        {
            get
            {
                if (ViewState["TotalAmount"] == null) return "0";
                return ViewState["TotalAmount"].ToString();
            }
            set { ViewState["TotalAmount"] = value; }
        }

        protected string TotalQuantity
        {
            get
            {
                if (ViewState["TotalQuantity"] == null) return "0";
                return ViewState["TotalQuantity"].ToString();
            }
            set { ViewState["TotalQuantity"] = value; }
        }

        #region  添加公司
        /// <summary>
        /// 公司列表
        /// </summary>
        public IList<FilialeInfo> SaleFilialeList
        {
            get
            {
                return CacheCollection.Filiale.GetHeadList();
            }
        }

        /// <summary>
        /// 绑定公司
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> BindFilialeDataBound()
        {
            var newDic = new Dictionary<string, string> { { Guid.Empty.ToString(), string.Empty } };
            foreach (var info in SaleFilialeList)
            {
                newDic.Add(info.ID.ToString(), info.Name);
            }
            newDic.Add(_reckoningElseFilialeid.ToString(), "ERP");
            return newDic;
        }
        #endregion

        protected void RtvCompanyCussentNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            if (R_StartTime.DbSelectedDate == null)
            {
                StartTime = DateTime.MinValue;
                EndTime = DateTime.MinValue;
            }
            else
            {
                if (R_StartTime.SelectedDate != null)
                {
                    StartTime = R_StartTime.SelectedDate.Value;
                    if (R_EndTime.SelectedDate != null)
                        EndTime = R_EndTime.DbSelectedDate == null ? DateTime.Now : R_EndTime.SelectedDate.Value;
                }
            }
            StorageType = Convert.ToInt32(RCB_StorageType.SelectedValue);
            WarehouseId = new Guid(RCB_Warehouse.SelectedValue);
            CompanyId = new Guid(RTVCompanyCussent.SelectedNode.Value);
            //if (RTVCompanyCussent.SelectedNode.ToolTip.Equals("BusinessRelatedUnits"))
            //{
            //    CompanyId = new Guid(RTVCompanyCussent.SelectedNode.Value);
            //}
            //else
            //{
            //    HostingFilialeId = new Guid(RTVCompanyCussent.SelectedNode.Value);
            //    CompanyId = Guid.Empty;
            //}
            StockGrid.Rebind();
        }

        protected void IbCreationDataClick(object sender, ImageClickEventArgs e)
        {
            if (R_StartTime.DbSelectedDate == null && R_EndTime.DbSelectedDate == null)
            {
                StartTime = DateTime.MinValue;
                EndTime = DateTime.MinValue;
            }
            else
            {
                StartTime = R_StartTime.SelectedDate == null ? new DateTime(2002, 1, 1) : R_StartTime.SelectedDate.Value;
                if (R_EndTime.SelectedDate != null) EndTime = R_EndTime.DbSelectedDate == null ? DateTime.Now : R_EndTime.SelectedDate.Value;
            }

            StorageType = Convert.ToInt32(RCB_StorageType.SelectedValue);
            CompanyId = new Guid(RTVCompanyCussent.SelectedNode.Value);
            HostingFilialeId = !string.IsNullOrEmpty(DdlSaleFiliale.SelectedValue) ? new Guid(DdlSaleFiliale.SelectedValue) : Guid.Empty;
            StockGrid.Rebind();
        }

        protected void IbExportDataClick(object sender, ImageClickEventArgs e)
        {
            string fileName = "出入库记录";
            fileName += WebControl.GetNowTime().ToShortDateString();
            fileName = Server.UrlEncode(fileName);
            StockGrid.ExportSettings.ExportOnlyData = true;
            StockGrid.ExportSettings.IgnorePaging = true;
            StockGrid.ExportSettings.FileName = fileName;
            StockGrid.MasterTableView.ExportToExcel();
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(StockGrid, e);
        }

        protected void OnTotaleDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Footer)
            {
                var tbTotalAmount = e.Item.FindControl("TB_TotalAmount") as Label;
                var totalQuantity = e.Item.FindControl("TbQuantity") as Label;
                if (tbTotalAmount != null)
                {
                    tbTotalAmount.Text = string.Format("合计{0}元", TotalAmount);// "计算的总数，或者也可以单独计算 ";// 
                }
                if (totalQuantity != null)
                {
                    totalQuantity.Text = string.Format("合计：{0}", TotalQuantity);
                }
            }
        }

        protected void DdlYearsSelectedIndexChanged(object sender, EventArgs e)
        {
            YearSelected();
        }

        private void YearSelected()
        {
            if (DDL_Years.SelectedValue == "0")
            {
                R_StartTime.MinDate = DateTime.MinValue;
                R_StartTime.MaxDate = DateTime.MaxValue;
                R_EndTime.MinDate = DateTime.MinValue;
                R_EndTime.MaxDate = DateTime.MaxValue;

                R_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
                R_EndTime.SelectedDate = DateTime.Now.AddDays(1);

                R_StartTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                R_StartTime.MaxDate = DateTime.Now;
                R_EndTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                R_EndTime.MaxDate = DateTime.Now;

                StartTime = R_StartTime.SelectedDate.Value;
                EndTime = R_EndTime.SelectedDate.Value;
            }
            else
            {
                R_StartTime.MinDate = DateTime.MinValue;
                R_StartTime.MaxDate = DateTime.MaxValue;
                R_EndTime.MinDate = DateTime.MinValue;
                R_EndTime.MaxDate = DateTime.MaxValue;
                R_StartTime.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                R_EndTime.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31 23:59:59");
                R_StartTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                R_StartTime.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31 23:59:59");
                R_EndTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                R_EndTime.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31 23:59:59");
                StartTime = DateTime.Parse(DDL_Years.SelectedValue + R_StartTime.SelectedDate.Value.ToString("-MM-dd"));
                EndTime = DateTime.Parse(DDL_Years.SelectedValue + R_EndTime.SelectedDate.Value.ToString("-MM-dd"));
            }
        }

        protected decimal ReturnTotalPrice(object storageType, object totalPrice)
        {
            var type = Convert.ToInt32(storageType);
            var price = Convert.ToDecimal(totalPrice);
            if (type == (int)StorageRecordType.SellReturnIn || type == (int)StorageRecordType.BuyStockIn)
            {
                return Math.Abs(price);
            }
            if (type == (int)StorageRecordType.SellStockOut || type == (int)StorageRecordType.BuyStockOut)
            {
                return -Math.Abs(price);
            }
            return price;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="eval"></param>
        /// <param name="isOut"></param>
        /// <returns></returns>
        protected object GetFilialeName(object eval, object isOut)
        {
            if (eval == null) return string.Empty;
            var fialieId = new Guid(eval.ToString());
            var filialeInfo = SaleFilialeList.FirstOrDefault(act => act.ID == fialieId);
            if (filialeInfo != null) return filialeInfo.Name;
            return "ERP";
        }
        /// <summary>
        ///  获取仓库信息
        /// </summary>
        /// <param name="eval"></param>
        /// <param name="isOut"></param>
        /// <returns></returns>
        private void GetWarehouseList()
        {
            var result = WMSSao.GetWarehouseAuthDic(Personnel.PersonnelId);

            var warehouseList = new Dictionary<Guid, string> { { Guid.Empty, "请选择仓库" } };
            RCB_Warehouse.Text = String.Empty;
            RCB_Warehouse.Items.Clear();
            RCB_Warehouse.DataSource = result != null ? warehouseList.Union(result.WarehouseDics) : warehouseList;
            RCB_Warehouse.DataBind();
            RCB_Warehouse.SelectedIndex = 0;
        }
    }
}

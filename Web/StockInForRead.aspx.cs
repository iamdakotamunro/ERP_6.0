using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;
using CompanyClass = ERP.DAL.Implement.Inventory.CompanyClass;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    ///Function :   出入库记录 
    ///ModifyBy :   dyy
    ///Date     :   2010.Jan.22th
    public partial class StockInForReadAw : BasePage
    {
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyClass _companyClass = new CompanyClass(GlobalConfig.DB.FromType.Read);
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                R_StartTime.SelectedDate = DateTime.Now.AddDays(-10);
                R_EndTime.SelectedDate = DateTime.Now;
                StartTime = DateTime.Now.AddDays(-10);
                EndTime = DateTime.Now;
                GetCompanyCussent();
                CompanyId = Guid.Empty;
                HostingCompanyId = Guid.Empty;
                GetWarehouseList();
                BindStorageType();


                var yearlist = new List<int>();
                for (var i = 2007; i <= (DateTime.Now.Year - GlobalConfig.KeepYear); i++)
                {
                    yearlist.Add(i);
                }
                DDL_Years.DataSource = yearlist.OrderByDescending(y => y);
                DDL_Years.DataBind();
                DDL_Years.Items.Add(new ListItem(GlobalConfig.KeepYear + "年内数据", "0"));
                DDL_Years.SelectedValue = "0";

                if (DDL_Years.SelectedValue == "0")
                {
                    R_StartTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                }
                else
                {
                    R_StartTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                    R_EndTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                }
            }
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        private void BindStorageType()
        {
            var dicStorageType = new Dictionary<int, string>();
            dicStorageType.Add((int)StorageRecordType.BuyStockIn, EnumAttribute.GetKeyName((StorageRecordType.BuyStockIn)));
            dicStorageType.Add((int)StorageRecordType.SellStockOut, EnumAttribute.GetKeyName((StorageRecordType.SellStockOut)));
            dicStorageType.Add((int)StorageRecordType.BuyStockOut, EnumAttribute.GetKeyName((StorageRecordType.BuyStockOut)));
            dicStorageType.Add((int)StorageRecordType.SellReturnIn, EnumAttribute.GetKeyName((StorageRecordType.SellReturnIn)));
            dicStorageType.Add((int)StorageRecordType.BorrowIn, EnumAttribute.GetKeyName((StorageRecordType.BorrowIn)));
            dicStorageType.Add((int)StorageRecordType.BorrowOut, EnumAttribute.GetKeyName((StorageRecordType.BorrowOut)));
            dicStorageType.Add((int)StorageRecordType.LendOut, EnumAttribute.GetKeyName((StorageRecordType.LendOut)));
            dicStorageType.Add((int)StorageRecordType.LendIn, EnumAttribute.GetKeyName((StorageRecordType.LendIn)));
            dicStorageType.Add((int)StorageRecordType.AfterSaleOut, EnumAttribute.GetKeyName((StorageRecordType.AfterSaleOut)));
            dicStorageType.Add((int)StorageRecordType.InnerPurchase, EnumAttribute.GetKeyName((StorageRecordType.InnerPurchase)));
            RCB_SalesType.DataSource = dicStorageType;
            RCB_SalesType.DataTextField = "Value";
            RCB_SalesType.DataValueField = "Key";
            RCB_SalesType.DataBind();
            RCB_SalesType.Items.Insert(0, new RadComboBoxItem("全部", "0"));
        }

        //遍历往来的单位
        private void GetCompanyCussent()
        {
            RadTreeNode rootNode = CreateNode("往来单位选择", "往来单位", true, Guid.Empty.ToString());
            rootNode.Category = "CompanyClass";
            rootNode.Selected = true;
            RTVCompanyCussent.Nodes.Add(rootNode);
            RecursivelyCompanyClass(Guid.Empty, rootNode);
            //RadTreeNode rootNode2 = CreateNode("内部公司选择", "内部公司", true, Guid.Empty.ToString());
            //RTVCompanyCussent.Nodes.Add(rootNode2);
            //RecursivelyFiliale(rootNode2);
        }

        private void RecursivelyFiliale(RadTreeNode node)
        {
            var filialeList = CacheCollection.Filiale.GetHeadList().Where(act => act.IsActive && (act.FilialeTypes.Contains((int)FilialeType.SaleCompany) || act.FilialeTypes.Contains((int)FilialeType.LogisticsCompany)));
            foreach (FilialeInfo info in filialeList)
            {
                RadTreeNode childNode = CreateNode(info.Name, "HostingCompany", false, info.ID.ToString());
                node.Nodes.Add(childNode);
            }
        }

        private void GetWarehouseList()
        {
            RCB_Warehouse.Text = String.Empty;
            RCB_Warehouse.Items.Clear();
            var newList = new List<WarehouseAuth>
                              {
                                  new WarehouseAuth {WarehouseId = Guid.Empty, WarehouseName = "全部仓库"}
                              };
            newList.AddRange(CurrentSession.Personnel.WarehouseList);
            RCB_Warehouse.DataSource = newList;
            RCB_Warehouse.DataBind();

        }

        protected string GetWarehouseName(Guid warehouseId)
        {
            return WarehouseManager.GetName(warehouseId);
        }

        //遍历子公司
        private void RecursivelyCompanyClass(Guid companyClassId, RadTreeNode node)
        {
            IList<CompanyClassInfo> childCompanyClassList = _companyClass.GetChildCompanyClassList(companyClassId);
            foreach (CompanyClassInfo companyClassInfo in childCompanyClassList)
            {
                RadTreeNode childNode = CreateNode(companyClassInfo.CompanyClassName, "BusinessRelatedUnits", false, companyClassInfo.CompanyClassId.ToString());
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
                childNode.Enabled = (State)companyCussentInfo.State == State.Enable;
                node.Nodes.Add(childNode);
            }
        }

        //创建节点
        private RadTreeNode CreateNode(string text, string toolTip, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = toolTip, Expanded = expanded };
            return node;
        }

        protected void StockGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            long recordCount = 0;
            IList<StorageRecordInfo> list = new List<StorageRecordInfo>();
            if (IsPostBack)
            {
                //获取授权仓库列表
                var warehouseList = CurrentSession.Personnel.WarehouseList;
                if (warehouseList.Count == 0)
                {
                    RAM.Alert("没有授权的仓库，无法查询。");
                }
                else
                {
                    var startPage = StockGrid.CurrentPageIndex + 1;
                    int pageSize = StockGrid.PageSize;
                    
                    List<StorageRecordType> storageRecordTypes = new List<StorageRecordType>();
                    if (SStockType != default(Int32))
                    {
                        storageRecordTypes.Add((StorageRecordType)SStockType);
                    }
                    List<StorageRecordState> storageRecordStates = new List<StorageRecordState> { StorageRecordState.Finished };
                    list = _storageRecordDao.GetStorageRecordListToPages(WarehouseId, CompanyId, TB_GoodsName.Text.Trim(), OrderNof.Trim(), storageRecordTypes, storageRecordStates, default(Int32),
                                                                     default(Guid), StartTime, EndTime,0, GlobalConfig.KeepYear, startPage, pageSize, out recordCount);
                }
            }
            StockGrid.DataSource = list;
            StockGrid.VirtualItemCount = (int)recordCount;
        }

        //往来单位编号
        protected Guid CompanyId
        {
            get
            {
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set
            {
                ViewState["CompanyId"] = value.ToString();
            }
        }

        //物流配送公司Id
        protected Guid HostingCompanyId
        {
            get
            {
                return new Guid(ViewState["HostingCompanyId"].ToString());
            }
            set
            {
                ViewState["HostingCompanyId"] = value.ToString();
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
                if (ViewState["StartTime"] == null)
                    if (R_StartTime.SelectedDate != null) return R_StartTime.SelectedDate.Value;
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set
            {
                ViewState["StartTime"] = value;
            }
        }

        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null)
                    if (R_StartTime.SelectedDate != null) return Convert.ToDateTime(R_StartTime.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
            }
            set
            {
                ViewState["EndTime"] = value;
            }
        }

        protected string OrderNof
        {
            get
            {
                if (ViewState["OrderNOF"] == null) return null;
                return ViewState["OrderNOF"].ToString();
            }
            set
            {
                ViewState["OrderNOF"] = value;
            }
        }

        protected int SStockType
        {
            get
            {
                if (ViewState["SStockType"] == null) return 0;
                return Convert.ToInt32(ViewState["SStockType"]);
            }
            set
            {
                ViewState["SStockType"] = value;
            }
        }

        protected Guid WarehouseId
        {
            get
            {
                if (ViewState["WarehouseId"] == null) return Guid.Empty;
                return new Guid(ViewState["WarehouseId"].ToString());
            }
            set
            {
                ViewState["WarehouseId"] = value;
            }
        }

        protected void IbCreationDataClick(object sender, ImageClickEventArgs e)
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
            OrderNof = !string.IsNullOrEmpty(TB_OrderNo.Text) ? TB_OrderNo.Text : string.Empty;
            SStockType = Convert.ToInt32(RCB_SalesType.SelectedValue);
            WarehouseId = new Guid(RCB_Warehouse.SelectedValue);
            if (RTVCompanyCussent.SelectedNode.ToolTip.Equals("BusinessRelatedUnits"))
            {
                CompanyId = new Guid(RTVCompanyCussent.SelectedNode.Value);
            }
            else
            {
                CompanyId = Guid.Empty;
            }
            StockGrid.Rebind();
        }

        protected void IbExportDataClick(object sender, ImageClickEventArgs e)
        {
            string fileName = Regex.Replace(RCB_SalesType.Text, @"[\s\|\-\/\<>\*\?\\]", "");
            if (EndTime == DateTime.MinValue)
                fileName += "-" + WebControl.GetNowTime().ToShortDateString();
            else
                fileName += "-" + StartTime.ToShortDateString() + "-" + EndTime.ToShortDateString();
            fileName = Server.UrlEncode(fileName);
            StockGrid.ExportSettings.ExportOnlyData = false;
            StockGrid.ExportSettings.IgnorePaging = true;
            StockGrid.ExportSettings.FileName = fileName;
            StockGrid.MasterTableView.ExportToExcel();
        }

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
            OrderNof = !string.IsNullOrEmpty(TB_OrderNo.Text) ? TB_OrderNo.Text : string.Empty;
            SStockType = Convert.ToInt32(RCB_SalesType.SelectedValue);
            WarehouseId = new Guid(RCB_Warehouse.SelectedValue);
            CompanyId = new Guid(RTVCompanyCussent.SelectedNode.Value);
            //if (RTVCompanyCussent.SelectedNode.ToolTip.Equals("BusinessRelatedUnits"))
            //{
                
            //}
            //else
            //{
            //    HostingCompanyId = new Guid(RTVCompanyCussent.SelectedNode.Value);
            //    CompanyId = Guid.Empty;
            //}
            StockGrid.Rebind();
        }

        public string GetWarehouse(Guid warehouseId)
        {
            var dic = WarehouseManager.GetWarehouseDic();
            return dic.ContainsKey(warehouseId) ? dic[warehouseId] : string.Empty;
        }

        public string GetCompany(Guid companyId)
        {
            if (companyId != Guid.Empty)
            {
                var companyCussentInfo = _companyCussent.GetCompanyCussent(companyId);
                if (companyCussentInfo == null)
                {
                    var filialeInfo = CacheCollection.Filiale.Get(companyId);
                    if (filialeInfo != null)
                        return filialeInfo.Name;
                }
                else
                {
                    return companyCussentInfo.CompanyName;
                }
            }
            return "-";
        }

        protected void DdlYearsSelectedIndexChanged(object sender, EventArgs e)
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
    }
}
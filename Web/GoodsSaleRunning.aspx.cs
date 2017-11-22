using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>商品出入库明细
    /// </summary>
    public partial class GoodsSaleRunningAw : BasePage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GetFilialeList();
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
                    RDP_StartTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                }
                else
                {
                    RDP_StartTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                    RDP_EndTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                }
            }
        }

        #region [初始化]
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            RCB_Goods.ItemsRequested += RcbGoodsItemsRequested;
        }
        #endregion

        protected void Rgss_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (GoodsId != Guid.Empty)
            {
                if (StartTime != DateTime.MinValue && EndTime != DateTime.MinValue)
                {
                    WarehouseId = new Guid(RCB_Warehouse.SelectedValue);
                    List<StorageRecordType> storageRecordTypes = new List<StorageRecordType>();
                    if (SStockType != String.Empty)
                    {
                        foreach (string stockType in SStockType.Split(','))
                        {
                            storageRecordTypes.Add((StorageRecordType)Convert.ToInt32(stockType));
                        }
                    }
                    var list = _storageRecordDao.GetStockRunning(GoodsId, StartTime, EndTime, WarehouseId, Convert.ToInt32(GlobalConfig.KeepYear),storageRecordTypes).ToList();
                    if (CheckBoxOrderby.Checked)
                    {
                        RGSS.DataSource = list.OrderBy(ent => ent.Specification);
                    }
                    else
                    {
                        RGSS.DataSource = list.OrderByDescending(ent=>ent.DateCreated).ToList();
                    }
                }
                else
                {
                    RGSS.DataSource = new List<StockWarningInfo>();
                }
            }
            else
                RGSS.DataSource = new List<StockWarningInfo>();
        }

        protected void IbCreationDataClick(object sender, ImageClickEventArgs e)
        {
            GoodsId = !string.IsNullOrEmpty(RCB_Goods.SelectedValue) ? new Guid(RCB_Goods.SelectedValue) : Guid.Empty;
            if (GoodsId != Guid.Empty)
            {
                StartTime = RDP_StartTime.SelectedDate ?? DateTime.MinValue;
                EndTime = RDP_EndTime.SelectedDate ?? DateTime.MinValue;
                Filiale = new Guid(RCB_Filiale.SelectedValue);
                SStockType = Hid_SalesType.Value;
                if (RCB_Warehouse.SelectedValue == Guid.Empty.ToString())
                {
                    RAM.Alert("请选择授权仓库");
                    return;
                }
                RGSS.Rebind();
            }
            else
                RAM.Alert("您没有选择要查询的商品!");
        }

        protected void IbExportDataClick(object sender, EventArgs e)
        {
            string fileName = Regex.Replace(RCB_Goods.Text, @"[\s\|\-\/\<>\*\?\\]", "");
            if (EndTime == DateTime.MinValue)
                fileName += "-" + WebControl.GetNowTime().ToShortDateString();
            else
                fileName += "-" + StartTime.ToShortDateString() + "-" + EndTime.ToShortDateString();
            fileName = Server.UrlEncode(fileName);
            RGSS.ExportSettings.ExportOnlyData = true;
            RGSS.ExportSettings.IgnorePaging = true;
            RGSS.ExportSettings.FileName = fileName;
            RGSS.MasterTableView.ExportToExcel();
        }

        private void RcbGoodsItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 2)
            {
                var list = _goodsCenterSao.GetGoodsSelectList(e.Text);
                var totalCount = list.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in list)
                    {
                        var rcb = new RadComboBoxItem
                        {
                            Text = item.Value,
                            Value = item.Key,
                        };
                        combo.Items.Add(rcb);
                    }
                }
            }
        }

        protected void DdlYearsSelectedIndexChanged(object sender, EventArgs e)
        {
            if (DDL_Years.SelectedValue == "0")
            {
                RDP_StartTime.MinDate = DateTime.MinValue;
                RDP_StartTime.MaxDate = DateTime.MaxValue;
                RDP_EndTime.MinDate = DateTime.MinValue;
                RDP_EndTime.MaxDate = DateTime.MaxValue;

                RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
                RDP_EndTime.SelectedDate = DateTime.Now.AddDays(1);

                RDP_StartTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                RDP_StartTime.MaxDate = DateTime.Now;
                RDP_EndTime.MinDate = DateTime.Parse(((DateTime.Now.Year - (GlobalConfig.KeepYear - 1)) + "-01-01"));
                RDP_EndTime.MaxDate = DateTime.Now;

                StartTime = RDP_StartTime.SelectedDate.Value;
                EndTime = RDP_EndTime.SelectedDate.Value;
            }
            else
            {
                RDP_StartTime.MinDate = DateTime.MinValue;
                RDP_StartTime.MaxDate = DateTime.MaxValue;
                RDP_EndTime.MinDate = DateTime.MinValue;
                RDP_EndTime.MaxDate = DateTime.MaxValue;
                RDP_StartTime.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_EndTime.SelectedDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                RDP_StartTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_StartTime.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                RDP_EndTime.MinDate = DateTime.Parse(DDL_Years.SelectedValue + "-01-01");
                RDP_EndTime.MaxDate = DateTime.Parse(DDL_Years.SelectedValue + "-12-31");
                StartTime = DateTime.Parse(DDL_Years.SelectedValue + RDP_StartTime.SelectedDate.Value.ToString("-MM-dd"));
                EndTime = DateTime.Parse(DDL_Years.SelectedValue + RDP_EndTime.SelectedDate.Value.ToString("-MM-dd"));
            }
        }

        private void GetFilialeList()
        {
            RCB_Filiale.Items.Clear();
            RCB_Filiale.Text = String.Empty;
            RCB_Filiale.DataSource = CacheCollection.Filiale.GetHeadList();
            RCB_Filiale.DataTextField = "Name";
            RCB_Filiale.DataValueField = "ID";
            RCB_Filiale.DataBind();
            RCB_Filiale.SelectedIndex = 0;
        }

        private void GetWarehouseList()
        {
            RCB_Warehouse.Text = String.Empty;
            RCB_Warehouse.Items.Clear();
            RCB_Warehouse.DataSource = CurrentSession.Personnel.WarehouseList;
            RCB_Warehouse.DataBind();
            RCB_Warehouse.Items.Insert(0, new RadComboBoxItem("请选择仓库", Guid.Empty.ToString()));
            RCB_Warehouse.SelectedIndex = 0;
        }

        protected string GetStockType(int stockType)
        {
            return EnumAttribute.GetKeyName((StorageRecordType)stockType);
        }

        /// <summary>
        /// 单据类型
        /// </summary>
        private void BindStorageType()
        {
            var dicStorageType = new Dictionary<int, string>();
            dicStorageType.Add(0,"全部");
            dicStorageType.Add((int)StorageRecordType.BuyStockIn, EnumAttribute.GetKeyName((StorageRecordType.BuyStockIn)));
            dicStorageType.Add((int)StorageRecordType.SellStockOut, EnumAttribute.GetKeyName((StorageRecordType.SellStockOut)));
            dicStorageType.Add((int)StorageRecordType.BuyStockOut, EnumAttribute.GetKeyName((StorageRecordType.BuyStockOut)));
            dicStorageType.Add((int)StorageRecordType.SellReturnIn, EnumAttribute.GetKeyName((StorageRecordType.SellReturnIn)));
            dicStorageType.Add((int)StorageRecordType.BorrowIn, EnumAttribute.GetKeyName((StorageRecordType.BorrowIn)));
            dicStorageType.Add((int)StorageRecordType.BorrowOut, EnumAttribute.GetKeyName((StorageRecordType.BorrowOut)));
            dicStorageType.Add((int)StorageRecordType.LendOut, EnumAttribute.GetKeyName((StorageRecordType.LendOut)));
            dicStorageType.Add((int)StorageRecordType.LendIn, EnumAttribute.GetKeyName((StorageRecordType.LendIn)));
            dicStorageType.Add((int)StorageRecordType.AfterSaleOut, EnumAttribute.GetKeyName((StorageRecordType.AfterSaleOut)));
            RCB_SalesType.DataSource = dicStorageType;
            RCB_SalesType.DataTextField = "Value";
            RCB_SalesType.DataValueField = "Key";
            RCB_SalesType.DataBind();
            //RCB_SalesType.Items.Insert(-1, new RadComboBoxItem("全部", "0"));
        }
        #region [ViewState]
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
        protected Guid Filiale
        {
            get
            {
                if (ViewState["Filiale"] == null) return Guid.Empty;
                return new Guid(ViewState["Filiale"].ToString());
            }
            set
            {
                ViewState["Filiale"] = value.ToString();
            }
        }
        protected DateTime StartTime
        {
            get
            {
                if (ViewState["StartTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["StartTime"]);
            }
            set
            {
                ViewState["StartTime"] = value.ToString();
            }
        }
        protected DateTime EndTime
        {
            get
            {
                if (ViewState["EndTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(Convert.ToDateTime(ViewState["EndTime"]).AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
            }
            set
            {
                ViewState["EndTime"] = value.ToString();
            }
        }
        protected Guid GoodsId
        {
            get
            {
                if (ViewState["GoodsId"] == null) return Guid.Empty;
                return new Guid(ViewState["GoodsId"].ToString());
            }
            set
            {
                ViewState["GoodsId"] = value.ToString();
            }
        }
        protected int SalesType
        {
            get
            {
                if (ViewState["SalesType"] == null) return 1;
                return Convert.ToInt32(ViewState["SalesType"]);
            }
            set
            {
                ViewState["SalesType"] = value.ToString();
            }
        }

        protected String SStockType
        {
            get
            {
                if (ViewState["SStockType"] == null) return String.Empty;
                return ViewState["SStockType"].ToString();
            }
            set
            {
                ViewState["SStockType"] = value;
            }
        }
        //protected string First
        //{
        //    get
        //    {
        //        return ViewState["First"]==null ? "-" : ViewState["First"].ToString();
        //    }
        //    set { ViewState["First"] = value; }
        //}

        //protected string Last
        //{
        //    get
        //    {
        //        return ViewState["Last"] == null ? "-" : ViewState["Last"].ToString();
        //    }
        //    set { ViewState["Last"] = value; }
        //}

        //protected string Current
        //{
        //    get
        //    {
        //        return ViewState["Current"] == null ? "-": ViewState["Current"].ToString();
        //    }
        //    set { ViewState["Current"] = value; }
        //}

        #endregion
    }
}

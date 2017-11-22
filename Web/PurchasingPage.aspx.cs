using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.Cache.Common;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using MIS.Enum;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    /// <summary>采购单操作 2011-03-25 by jiang
    /// </summary>
    public partial class PurchasingPage : BasePage
    {
        #region  -- ViewState

        private IEnumerable<FilialeInfo> FilialeList
        {
            get
            {
                if (ViewState["FilialeList"] == null)
                {
                    ViewState["FilialeList"] = CacheCollection.Filiale.GetHostingAndSaleFilialeList();
                }
                return (IList<FilialeInfo>)ViewState["FilialeList"];
            }
        }

        /// <summary>
        /// 采购状态
        /// </summary>
        protected PurchasingState State
        {
            get
            {
                if (ViewState["PurchasingState"] == null) return PurchasingState.NoSubmit;
                return (PurchasingState)System.Enum.Parse(typeof(PurchasingState), ViewState["PurchasingState"].ToString());
            }
            set { ViewState["PurchasingState"] = ((int)value); }
        }

        /// <summary>
        /// 采购类别
        /// </summary>
        protected PurchasingType Type
        {
            get
            {
                if (ViewState["PurchasingType"] == null) return PurchasingType.All;
                return (PurchasingType)System.Enum.Parse(typeof(PurchasingType), ViewState["PurchasingType"].ToString());
            }
            set { ViewState["PurchasingType"] = value; }
        }

        /// <summary>
        /// 供应商
        /// </summary>
        protected Guid CompanyId
        {
            get
            {
                if (ViewState["CompanyId"] == null) return Guid.Empty;
                return new Guid(ViewState["CompanyId"].ToString());
            }
            set { ViewState["CompanyId"] = value; }
        }

        /// <summary>
        /// 采购仓库
        /// </summary>
        protected Guid WareHouseId
        {
            get
            {
                if (ViewState["wareHouseId"] == null) return Guid.Empty;
                return new Guid(ViewState["wareHouseId"].ToString());
            }
            set { ViewState["wareHouseId"] = value; }
        }

        /// <summary>
        /// 开始时间
        /// </summary>
        protected DateTime StartTime
        {
            get
            {
                if (ViewState["startTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["startTime"].ToString());
            }
            set { ViewState["startTime"] = value; }
        }

        /// <summary>
        /// 完成时间
        /// </summary>
        protected DateTime EndTime
        {
            get
            {
                if (ViewState["endTime"] == null) return DateTime.MinValue;
                return Convert.ToDateTime(ViewState["endTime"].ToString());
            }
            set { ViewState["endTime"] = value; }
        }

        /// <summary>
        /// searchKey
        /// </summary>
        protected string SearchKey
        {
            get
            {
                if (ViewState["SearchKey"] == null) return "";
                return (ViewState["SearchKey"].ToString());
            }
            set { ViewState["SearchKey"] = value; }
        }

        protected List<Guid> GoodsList
        {
            get
            {
                if (ViewState["GoodsList"] == null) return new List<Guid>();
                return (List<Guid>)ViewState["GoodsList"];
            }
            set { ViewState["GoodsList"] = value; }
        }

        /// <summary>
        /// 采购组Id
        /// </summary>
        protected Guid PmId
        {
            get
            {
                if (ViewState["PmId"] == null) return Guid.Empty;
                return new Guid(ViewState["PmId"].ToString());
            }
            set { ViewState["PmId"] = value; }
        }

        /// <summary>
        /// 是否为初次加载
        /// </summary>
        protected bool IsDefault
        {
            get
            {
                if (ViewState["IsDefault"] == null) return true;
                return Convert.ToBoolean(ViewState["IsDefault"]);
            }
            set { ViewState["IsDefault"] = value; }
        }
        #endregion

        private static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly ExcelTemplate _excelTemplate = new ExcelTemplate(GlobalConfig.DB.FromType.Read);
        private static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        private static readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);
        private static readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Write);
        private static readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Write);
        private static readonly ProcurementTicketLimitDAL _procurementTicketLimitDal = new ProcurementTicketLimitDAL(GlobalConfig.DB.FromType.Write);
        private static readonly ICompanyPurchaseGoupDao _companyPurchaseGoupDao = new CompanyPurchaseGoupDao(GlobalConfig.DB.FromType.Read);
        private readonly PurchasingManager _purchasingManager = new PurchasingManager(_purchasing, _goodsCenterSao, _purchasingDetail, _companyCussent, _procurementTicketLimitDal);
        private readonly PurchasingDetailManager _purchasingDetailManager = new PurchasingDetailManager(_purchasingDetail, _purchasing);
        private readonly CodeManager _codeManager = new CodeManager();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //绑定责任人
                RCB_Persion.DataSource = PersonnelList;
                RCB_Persion.DataBind();

                RCB_State.DataSource = GetPurchasingStateList();
                RCB_State.DataBind();
                RCB_State.Items.Insert(0, new RadComboBoxItem("  ", "-1"));

                RCB_State.SelectedValue = string.Format("{0}", (int)State);

                RCB_Type.DataSource = GetPurchasingTypeList();
                RCB_Type.DataBind();

                RCB_Type.SelectedValue = string.Format("{0}", (int)Type);
                List<CompanyCussentInfo> dataSource = new List<CompanyCussentInfo> { new CompanyCussentInfo { CompanyId = Guid.Empty, CompanyName = "--请选择--" } };
                RCB_Company.DataSource = dataSource.Union(Rcb_CommanyDataSource());
                RCB_Company.DataBind();
                RCB_Company.SelectedIndex = 0;

                var result = WMSSao.GetWarehouseAuthDic(Personnel.PersonnelId);
                WarehouseAuthAndFiliale = result;

                var warehouseList = new Dictionary<Guid, string> { { Guid.Empty, "所有仓库" } };
                RCB_Warehouse.DataSource = result != null ? warehouseList.Union(result.WarehouseDics) : warehouseList;
                RCB_Warehouse.DataBind();

                var purchasingFilialeList = new Dictionary<Guid, string> { { Guid.Empty, "所有采购公司" } };

                var filiales = FilialeList.Where(ent => ent.FilialeTypes.Contains((int)FilialeType.LogisticsCompany)).Select(ent => ent.ID);
                RCB_PurchasingFiliale.DataSource = result != null ? purchasingFilialeList.Union(result.HostingFilialeDic.Where(ent => filiales.Contains(ent.Key))) : purchasingFilialeList;
                RCB_PurchasingFiliale.DataBind();

                RCB_ExcelTemp.DataSource = GetTemplateDic(result != null ? result.WarehouseDics : new Dictionary<Guid, String>());
                RCB_ExcelTemp.DataBind();
                RCB_ExcelTemp.Items.Insert(0, new ListItem("选择你预先设好的模板", Guid.Empty.ToString()));

                StartTime = DateTime.Now.AddDays(-30);
                EndTime = DateTime.Now;
                RDP_StartTime.SelectedDate = DateTime.Now.AddDays(-30);
                RDP_EndTime.SelectedDate = DateTime.Now;

                Rgd_Purchasing.MasterTableView.Columns.FindByUniqueName("SurplusMoney").Visible = false;
            }
        }

        protected Dictionary<Guid, String> GetTemplateDic(Dictionary<Guid, String> warehouseDic)
        {
            var warehouseIdList = warehouseDic.Select(dic => dic.Key).ToList();

            var tempdics = _excelTemplate.GetExcelTemplateListByWarehouseIdList(warehouseIdList);
            if (tempdics == null || tempdics.Count == 0)
            {
                return new Dictionary<Guid, string>();
            }
            return tempdics.OrderBy(p => p.WarehouseId).ToDictionary(k => k.TempId, v => warehouseDic.ContainsKey(v.WarehouseId) ? string.Format("{0}({1})", v.TemplateName, warehouseDic[v.WarehouseId]) : v.TemplateName);
        }

        protected void RCBWarehouse_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            #region 设置模板
            var dic = new Dictionary<Guid, String>();
            if (RCB_Warehouse.SelectedValue == Guid.Empty.ToString())
            {
                var result = WMSSao.GetWarehouseAuthDic(CurrentSession.Personnel.Get().PersonnelId);
                dic = result.WarehouseDics;
            }
            else
            {
                dic.Add(new Guid(RCB_Warehouse.SelectedValue), RCB_Warehouse.Text);
            }
            RCB_ExcelTemp.DataSource = GetTemplateDic(dic);
            RCB_ExcelTemp.DataBind();
            RCB_ExcelTemp.Items.Insert(0, new ListItem("选择你预先设好的模板", Guid.Empty.ToString()));
            #endregion

            #region 设置物流配送公司
            var cb = (RadComboBox)sender;
            if (cb != null)
            {
                var companyId = string.IsNullOrEmpty(RCB_Company.SelectedValue)
                    ? Guid.Empty
                    : new Guid(RCB_Company.SelectedValue);
                var filialeid = companyId != Guid.Empty
                    ? _companyCussent.GetRelevanceFilialeIdByCompanyId(companyId)
                    : Guid.Empty;
                BindFilialeList(filialeid, new Guid(cb.SelectedItem.Value));
            }
            #endregion
        }

        private void BindFilialeList(Guid filialeId, Guid warehouseId)
        {
            var warehouse = WarehouseAuth.FirstOrDefault(act => act.WarehouseId == warehouseId);
            var filiales = new Dictionary<Guid, HostingFilialeAuth> { { Guid.Empty, new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "采购公司列表" } } };
            var proxyFiliales = new Dictionary<Guid, HostingFilialeAuth>();

            if (warehouse != null)
            {
                foreach (var hostingFilialeAuth in warehouse.FilialeAuths)
                {
                    if (!filiales.ContainsKey(hostingFilialeAuth.HostingFilialeId) && hostingFilialeAuth.HostingFilialeId != filialeId)
                    {
                        filiales.Add(hostingFilialeAuth.HostingFilialeId, hostingFilialeAuth);
                    };
                    foreach (var proxyFiliale in hostingFilialeAuth.ProxyFiliales.Where(ent => ent.ProxyFilialeId != hostingFilialeAuth.HostingFilialeId && ent.ProxyFilialeId != filialeId))
                    {
                        if (!proxyFiliales.ContainsKey(proxyFiliale.ProxyFilialeId))
                        {
                            proxyFiliales.Add(proxyFiliale.ProxyFilialeId, new HostingFilialeAuth(proxyFiliale.ProxyFilialeId, proxyFiliale.ProxyFilialeName));
                        };
                    }
                }
            }
            RCB_Filiale.DataSource = filiales.Values.Union(proxyFiliales.Where(ent => !filiales.Keys.Contains(ent.Key) && filialeId != ent.Key).Select(ent => ent.Value));
            RCB_Filiale.DataBind();
        }

        /// <summary>
        /// 获取(采购部)所有员工的信息
        /// </summary>
        /// <returns></returns>
        protected IList<PersonnelInfo> PersonnelList
        {
            get
            {
                if (ViewState["PersonnelList"] == null)
                {
                    //var filialeId = new Guid("06B30857-82F5-45F5-8768-79BD4211806C");
                    //var branchId = new Guid("E50EDCB7-2124-4DB4-AB62-5E8D8CCD8C2B");//采购部
                    var systemBranchId = new Guid("D9D6002C-196C-4375-B41A-E7040FE12B09"); //系统部门ID
                    var systemPostionList = MISService.GetAllSystemPositionList().ToList();
                    var positonIds = systemPostionList.Where(
                        act => act.ParentSystemBranchID == systemBranchId || act.SystemBranchID == systemBranchId)
                        .Select(act => act.SystemBranchPositionID);
                    IList<PersonnelInfo> list = new PersonnelSao().GetList().Where(ent => positonIds.Contains(ent.SystemBrandPositionId) && ent.IsActive).ToList();
                    ViewState["PersonnelList"] = list;
                }
                return (IList<PersonnelInfo>)ViewState["PersonnelList"];
            }
        }

        /// <summary>
        /// 
        /// </summary>
        protected WarehouseAuthAndFilialeDTO WarehouseAuthAndFiliale
        {
            get
            {
                if (ViewState["WarehouseAuthAndFiliale"] == null)
                {
                    return new WarehouseAuthAndFilialeDTO();
                }
                return (WarehouseAuthAndFilialeDTO)ViewState["WarehouseAuthAndFiliale"];
            }
            set { ViewState["WarehouseAuthAndFiliale"] = value; }
        }

        /// <summary>
        /// 获取责任人名称
        /// </summary>
        /// <param name="personId"></param>
        /// <returns></returns>
        protected string GetPersonName(Guid personId)
        {
            string personName = "-";
            PersonnelInfo info = PersonnelList.FirstOrDefault(w => w.PersonnelId == personId);
            if (info != null)
            {
                personName = info.RealName;
            }
            return personName;
        }

        private List<WarehouseFilialeAuth> WarehouseAuth
        {
            get
            {
                if (ViewState["WarehouseAuth"] == null)
                {
                    var result = WMSSao.GetWarehouseAndFilialeAuth(Personnel.PersonnelId);
                    ViewState["WarehouseAuth"] = result;
                    return result;
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
        #region 绑定数据

        #region 绑定状态

        protected string GetPurchasingState(Object purchasingState)
        {
            return EnumAttribute.GetKeyName((PurchasingState)purchasingState);
        }

        protected Dictionary<int, string> GetPurchasingStateList()
        {
            var purchasingState = (Dictionary<int, string>)EnumAttribute.GetDict<PurchasingState>();
            return purchasingState;
        }


        protected string GetPurchasingFilialeName(object filialeId)
        {
            if (WarehouseAuthAndFiliale.HostingFilialeDic.ContainsKey(new Guid(filialeId.ToString())))
            {
                return WarehouseAuthAndFiliale.HostingFilialeDic[new Guid(filialeId.ToString())];
            }
            return "-";
        }

        /// <summary>获取分组名称
        /// </summary>
        /// <param name="purchaseGroupId"></param>
        /// <returns></returns>
        protected string GetPurchaseGroupIdName(object purchaseGroupId)
        {
            if (purchaseGroupId != null)
            {
                if ((Guid)purchaseGroupId == Guid.Empty)
                {
                    return "默认";
                }
                var info = _companyPurchaseGoupDao.GetCompanyPurchaseGoupInfo((Guid)purchaseGroupId);
                if (info != null)
                {
                    return info.PurchaseGroupName;
                }
                return string.Empty;
            }
            return string.Empty;
        }

        #endregion

        #region 绑定类别

        protected string GetPurchasingType(Object purchasingType)
        {
            return EnumAttribute.GetKeyName((PurchasingType)purchasingType);
        }

        protected Dictionary<int, string> GetPurchasingTypeList()
        {
            var purchasingType = (Dictionary<int, string>)EnumAttribute.GetDict<PurchasingType>();
            return purchasingType;
        }

        #endregion

        #region 获取供应商集合

        /// <summary>
        ///  获取供应商集合
        /// </summary>
        /// <returns></returns>
        protected IList<CompanyCussentInfo> Rcb_CommanyDataSource()
        {
            var companys = _companyCussent.GetCompanyCussentList();
            var hostingFilialeIds = CacheCollection.Filiale.GetHostingFilialeList().Select(ent => ent.ID);
            return companys.Where(ent => ent.State == 1 && ((int)CompanyType.Suppliers == ent.CompanyType || hostingFilialeIds.Contains(ent.RelevanceFilialeId))).ToList();
        }

        #endregion

        protected Dictionary<Guid, String> GetWarehouseList(Dictionary<Guid, String> data)
        {
            var datasource = new Dictionary<Guid, String> { { Guid.Empty, "所有仓库" } };
            if (data.Count > 0)
            {
                foreach (var dic in data)
                {
                    datasource.Add(dic.Key, dic.Value);
                }
            }
            return datasource;
        }

        #endregion

        #region 功     能:模板的选中

        ///// <summary>
        ///// 功     能:模板的选中
        ///// 时     间:2010-11-09
        ///// 作     者:蒋赛标
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //protected void Rcb_ExcelTemp_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    Bt_Temp.Text = "00000000-0000-0000-0000-000000000000" == RCB_ExcelTemp.SelectedValue ? "新建" : "编辑";
        //}

        #endregion

        protected void RGPP_OnNeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<PurchasingInfo> list = new List<PurchasingInfo>();
            List<Guid> warehouseIdList = WarehouseAuthAndFiliale.WarehouseDics.Keys.ToList();
            if (!warehouseIdList.Any())
            {
                Rgd_Purchasing.DataSource = list;
                return;
            }
            var startPage = Rgd_Purchasing.CurrentPageIndex + 1;
            int pageSize = Rgd_Purchasing.PageSize;
            long recordCount;
            string strPerson = RCB_Persion.SelectedValue;
            Guid personResponsible = Guid.Empty;
            if (!string.IsNullOrEmpty(strPerson)) personResponsible = new Guid(strPerson);
            var states = new List<int> { (int)State };
            if (IsDefault) states.Add((int)PurchasingState.WaitingAudit);
            list = _purchasing.GetPurchasingListToPage(StartTime, EndTime, CompanyId, WareHouseId,
                                                        states, Type, SearchKey, GoodsList,
                                                        warehouseIdList, personResponsible,
                                                        (string.IsNullOrEmpty(RCB_Filiale.SelectedValue) ? Guid.Empty : new Guid(RCB_Filiale.SelectedValue))
                                                        , startPage, pageSize, out recordCount);
            var result = list.Select(ent => new PurchasingInfo
            {
                PurchasingID = ent.PurchasingID,
                PurchasingNo = ent.PurchasingNo,
                CompanyID = ent.CompanyID,
                FilialeID = ent.FilialeID,
                WarehouseID = ent.WarehouseID,
                ArrivalTime = ent.ArrivalTime,
                PurchasingState = ent.PurchasingState,
                PurchasingType = ent.PurchasingType,
                StartTime = ent.StartTime,
                EndTime = ent.EndTime,
                Description = ent.Description,
                SumPrice = ent.SumPrice,
                PurchasingToDate = ent.PurchasingToDate,
                IsException = ent.IsException,
                PersonResponsible = ent.PersonResponsible,
                PurchasingFilialeId = ent.PurchasingFilialeId,
                PurchaseGroupId = ent.PurchaseGroupId,
                IsOut = ent.IsOut,
                CompanyName = ent.CompanyName,
                PurchasingPersonName = !String.IsNullOrWhiteSpace(ent.PurchasingPersonName) ? ent.PurchasingPersonName : "--"
            }).ToList();
            decimal sumPrice = 0;
            if (list.Count > 0)
            {
                foreach (var purchasingInfo in result)
                {
                    purchasingInfo.SurplusMoney = ReturnSurplusMoney(purchasingInfo.PurchasingID, purchasingInfo.PurchasingState);
                }

                sumPrice = result.Sum(p => p.SumPrice);
            }

            var sum = Rgd_Purchasing.MasterTableView.Columns.FindByUniqueName("SumPrice");
            sum.FooterText = string.Format("总价合计：{0}", WebControl.NumberSeparator(sumPrice));
            sum.ItemStyle.Width = 200;

            Rgd_Purchasing.DataSource = result;
            Rgd_Purchasing.VirtualItemCount = (int)recordCount;
        }

        #region 商品搜索

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            RCB_Goods.ItemsRequested += Rcb_Goods_ItemsRequested;
        }

        private void Rcb_Goods_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text) && e.Text.Length >= 2)
            {
                var dic = _goodsCenterSao.GetGoodsSelectList(e.Text);
                Int32 totalCount = dic.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var keyValuePair in dic)
                    {
                        var item = new RadComboBoxItem
                        {
                            Text = keyValuePair.Value,
                            Value = keyValuePair.Key
                        };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        #endregion

        #region 删除

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Ib_DeleteAll_Click(object sender, EventArgs e)
        {
            if (Rgd_Purchasing.SelectedItems.Count == 0)
            {
                RAM.Alert("没有选择要删除的采购单!");
                return;
            }

            var personnelInfo = CurrentSession.Personnel.Get();
            string message = string.Empty;
            foreach (GridDataItem dataItem in Rgd_Purchasing.SelectedItems)
            {
                var purchasingId = new Guid(dataItem.GetDataKeyValue("PurchasingID").ToString());
                var purchasingNoStr = dataItem.GetDataKeyValue("PurchasingNo").ToString();
                int pstate = Convert.ToInt32(dataItem.GetDataKeyValue("PurchasingState").ToString());
                var pInfo = _purchasing.GetPurchasingById(purchasingId);
                if (pstate <= (int)PurchasingState.Purchasing || pstate == (int)PurchasingState.Refusing)
                {
                    if (pInfo.PurchasingFilialeId != Guid.Empty && pInfo.PurchasingState == (int)PurchasingState.Purchasing && pInfo.IsOut)
                    {
                        //更新没有开票设置已支付金额
                        Decimal purchasingAmount = _purchasing.GetPurchasingAmount(pInfo.PurchasingNo);
                        _procurementTicketLimitDal.RenewalProcurementTicketLimitAlreadyCompleteLimit(
                            pInfo.PurchasingFilialeId, pInfo.CompanyID, -purchasingAmount,
                            pInfo.StartTime.Year, pInfo.StartTime.Month);
                    }
                    _purchasingManager.PurchasingUpdate(purchasingId, PurchasingState.Deleted);
                    _purchasing.PurchasingDescription(purchasingId, WebControl.RetrunUserAndTime("作废采购单"));
                    //采购单删除添加操作记录添加
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, pInfo.PurchasingID, purchasingNoStr,
                        OperationPoint.PurchasingManager.Delete.GetBusinessInfo(), string.Empty);
                }
                else
                {
                    string purchasingNo = dataItem.GetDataKeyValue("PurchasingNo").ToString();
                    message += purchasingNo + " ";
                }

            }
            if (!string.IsNullOrEmpty(message))
            {
                RAM.Alert("采购单为" + message + "的状态已经不允许删除!");
            }
            else
            {
                RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        #endregion

        #region 导出Excel

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Ib_ExportData_Click(object sender, ImageClickEventArgs e)
        {
            if (string.IsNullOrEmpty(RCB_ExcelTemp.SelectedValue) || RCB_ExcelTemp.SelectedValue == Guid.Empty.ToString())
            {
                RAM.Alert("请选择导出模版!.");
                return;
            }
            IList<PurchasingInfo> pList = new List<PurchasingInfo>();
            var tempInfo = _excelTemplate.GetExcelTemplateInfo(new Guid(RCB_ExcelTemp.SelectedValue));
            if (tempInfo == null)
            {
                RAM.Alert("选择的模版信息不存在!.");
                return;
            }
            bool flag = false;
            foreach (GridDataItem dataItem in Rgd_Purchasing.SelectedItems)
            {
                var pInfo = new PurchasingInfo
                {
                    PurchasingID = new Guid(dataItem.GetDataKeyValue("PurchasingID").ToString()),
                    PurchasingNo = dataItem.GetDataKeyValue("PurchasingNo").ToString(),
                    CompanyName = dataItem.GetDataKeyValue("CompanyName").ToString(),
                    PurchasingState = (int)dataItem.GetDataKeyValue("PurchasingState"),
                    PurchasingFilialeId = new Guid(dataItem.GetDataKeyValue("PurchasingFilialeId").ToString()),
                    //IsOut = Convert.ToBoolean(dataItem.GetDataKeyValue("IsOut")),
                    WarehouseID = new Guid(dataItem.GetDataKeyValue("WarehouseID").ToString())
                };
                if (tempInfo.WarehouseId != pInfo.WarehouseID)
                {
                    RAM.Alert(string.Format("采购单[{0}]与模版[{1}]两者对应仓库不一致!", pInfo.PurchasingNo, tempInfo.TemplateName));
                    return;
                }
                if ((pInfo.PurchasingState == (int)PurchasingState.NoSubmit ||
                     pInfo.PurchasingState == (int)PurchasingState.Purchasing))
                {
                    if (!string.IsNullOrEmpty(pInfo.CompanyName))
                    {
                        pList.Add(pInfo);
                    }
                    else
                    {
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                RAM.Alert("不允许导出供应商为空的采购单,请重新选择.");
                return;
            }
            OutPutExcel(pList, YesOrNo.Yes, tempInfo);
        }

        #endregion

        #region 导出采购表

        /// <summary>
        /// 功    能:导出采购表
        /// 时    间:2010-11-09
        /// 作    者:蒋赛标
        /// </summary>
        public void OutPutExcel(IList<PurchasingInfo> purList, YesOrNo yesorno, ExcelTemplateInfo tempInfo)
        {
            if (purList.Count == 0)
            {
                RAM.Alert("没有可导出的数据!");
                return;
            }

            var workbook = new HSSFWorkbook();
            var ms = new MemoryStream();
            var sheet = new HSSFSheet[purList.Count]; // 增加sheet。

            #region Excel样式

            //标题样式styletitle

            HSSFFont fonttitle = workbook.CreateFont();
            fonttitle.FontHeightInPoints = 12;
            fonttitle.Color = HSSFColor.RED.index;
            fonttitle.Boldweight = 1;
            HSSFCellStyle styletitle = workbook.CreateCellStyle();
            styletitle.BorderBottom = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderLeft = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderRight = HSSFCellStyle.BORDER_THIN;
            styletitle.BorderTop = HSSFCellStyle.BORDER_THIN;
            styletitle.SetFont(fonttitle);

            //内容字体styleContent
            HSSFFont fontcontent = workbook.CreateFont();
            fontcontent.FontHeightInPoints = 9;
            fontcontent.Color = HSSFColor.BLACK.index;
            HSSFCellStyle styleContent = workbook.CreateCellStyle();
            styleContent.BorderBottom = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderLeft = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderRight = HSSFCellStyle.BORDER_THIN;
            styleContent.BorderTop = HSSFCellStyle.BORDER_THIN;
            styleContent.SetFont(fontcontent);

            #endregion

            #region 将值插入sheet

            int t = -1;
            foreach (PurchasingInfo pInfo in purList)
            {
                if ((int)PurchasingState.NoSubmit == pInfo.PurchasingState ||
                    (int)PurchasingState.Purchasing == pInfo.PurchasingState)
                {
                    t++;
                    sheet[t] = workbook.CreateSheet(pInfo.CompanyName.Replace("*", "") + " " + pInfo.PurchasingNo + " ");
                    //添加sheet名
                    sheet[t].DefaultColumnWidth = 40;
                    sheet[t].DefaultRowHeight = 20;
                    sheet[t].DisplayGridlines = false;
                    //sheet[t].SetColumnWidth(0,20);

                    HSSFRow rowtitle = sheet[t].CreateRow(0);
                    HSSFCell celltitie = rowtitle.CreateCell(0);
                    celltitie.SetCellValue("采购单(" + pInfo.PurchasingNo + ")");
                    HSSFCellStyle style = workbook.CreateCellStyle();
                    style.Alignment = HSSFCellStyle.ALIGN_CENTER;
                    HSSFFont font = workbook.CreateFont();
                    font.FontHeightInPoints = 20;
                    font.Color = HSSFColor.BLACK.index;
                    font.Boldweight = 2;
                    style.SetFont(font);
                    celltitie.CellStyle = style;
                    sheet[t].AddMergedRegion(new Region(0, 0, 0, 3));
                    //采购联系消息
                    HSSFRow row1 = sheet[t].CreateRow(4);
                    row1.CreateCell(0).SetCellValue("To ");
                    row1.CreateCell(1).SetCellValue(pInfo.CompanyName);

                    HSSFRow row2 = sheet[t].CreateRow(5);
                    row2.CreateCell(0).SetCellValue("订货单位");
                    row2.CreateCell(1).SetCellValue(tempInfo.Customer);

                    HSSFRow row3 = sheet[t].CreateRow(6);
                    row3.CreateCell(0).SetCellValue("收货人 ");
                    row3.CreateCell(1).SetCellValue(tempInfo.Shipper);



                    HSSFRow row5 = sheet[t].CreateRow(7);
                    row5.CreateCell(0).SetCellValue("收货地址");
                    row5.CreateCell(1).SetCellValue(tempInfo.ContactAddress);

                    HSSFRow row4 = sheet[t].CreateRow(8);
                    row4.CreateCell(0).SetCellValue("订货联系人");
                    row4.CreateCell(1).SetCellValue(tempInfo.ContactPerson);

                    HSSFRow row6 = sheet[t].CreateRow(9);
                    row6.CreateCell(0).SetCellValue("订货时间");
                    row6.CreateCell(1).SetCellValue(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                    HSSFRow row7 = sheet[t].CreateRow(10);
                    row7.CreateCell(0).SetCellValue("备注");
                    row7.CreateCell(1).SetCellValue(tempInfo.Remarks + " 采购单号：" + pInfo.PurchasingNo);

                    row7.CreateCell(0).SetCellValue("采购公司：");
                    row7.CreateCell(1).SetCellValue(pInfo.IsOut ? FilialeList.First(ent => ent.ID == pInfo.PurchasingFilialeId).Name : "ERP");

                    HSSFRow row8 = sheet[t].CreateRow(11);
                    HSSFCell cell0 = row8.CreateCell(0);
                    HSSFCell cell1 = row8.CreateCell(1);
                    HSSFCell cell2 = row8.CreateCell(2);
                    HSSFCell cell3 = row8.CreateCell(3);
                    HSSFCell cell4 = row8.CreateCell(4);
                    cell0.SetCellValue("序号");
                    cell1.SetCellValue("商品名称");
                    cell2.SetCellValue("商品SKU");
                    cell3.SetCellValue("需求量");
                    cell4.SetCellValue("赠品");

                    cell0.CellStyle = styletitle;
                    cell1.CellStyle = styletitle;
                    cell2.CellStyle = styletitle;
                    cell3.CellStyle = styletitle;
                    cell4.CellStyle = styletitle;
                    cell4.CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;

                    int row = 12;
                    IList<PurchasingDetailInfo> detailList = _purchasingDetail.Select(pInfo.PurchasingID)
                        .OrderBy(p => p.GoodsName).ThenBy(p => p.Specification).ToList();
                    int index = 1;
                    foreach (PurchasingDetailInfo dInfo in detailList)
                    {
                        double count = dInfo.PlanQuantity - dInfo.RealityQuantity;
                        if (count >= 1)
                        {
                            HSSFRow rowt = sheet[t].CreateRow(row);
                            HSSFCell c0 = rowt.CreateCell(0);
                            HSSFCell c1 = rowt.CreateCell(1);
                            HSSFCell c2 = rowt.CreateCell(2);
                            HSSFCell c3 = rowt.CreateCell(3);
                            HSSFCell c4 = rowt.CreateCell(4);
                            c0.SetCellValue(index);
                            c1.SetCellValue(dInfo.GoodsName);
                            c2.SetCellValue(dInfo.Specification);
                            c3.SetCellValue(count);
                            c4.SetCellValue(dInfo.PurchasingGoodsType == (int)PurchasingGoodsType.Gift ? "√" : "");

                            c0.CellStyle = styleContent;
                            c1.CellStyle = styleContent;
                            c2.CellStyle = styleContent;
                            c3.CellStyle = styleContent;
                            c4.CellStyle = styleContent;
                            c3.CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                            c4.CellStyle.Alignment = HSSFCellStyle.ALIGN_CENTER;
                            row++;
                            index++;
                        }
                    }
                }
            }

            #endregion

            workbook.Write(ms);
            if (YesOrNo.Yes == yesorno)
            {
                Response.ContentEncoding = Encoding.GetEncoding("utf-8");
                Response.AddHeader("Content-Disposition",
                                   "attachment; filename=" +
                                   HttpUtility.UrlEncode("可得网采购单" + DateTime.Now.ToString("yyyyMMdd") + ".xls",
                                                         Encoding.UTF8));
                Response.BinaryWrite(ms.ToArray());
            }

            ms.Close();
            ms.Dispose();
            GC.Collect();


        }

        #endregion

        protected void Rgd_Purchasing_ItemCommand(object source, GridCommandEventArgs e)
        {
            var item = e.Item as GridDataItem;
            if (item != null)
            {
                var dataItem = item;
                var purchasingID = new Guid(dataItem.GetDataKeyValue("PurchasingID").ToString());
                var purchasingNo = dataItem.GetDataKeyValue("PurchasingNo").ToString();
                if ("Consignee" == e.CommandName)
                {
                    _purchasingManager.PurchasingUpdate(purchasingID, PurchasingState.AllComplete);

                    //添加操作记录，手动操作完成
                    var personnelInfo = CurrentSession.Personnel.Get();
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, purchasingID, purchasingNo,
                        OperationPoint.PurchasingManager.HandComplete.GetBusinessInfo(), string.Empty);
                    RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
        }

        /// <summary>
        /// 获取仓库名字
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected string GetWareGhouseName(object obj)
        {
            Guid warehouseId = obj == null ? Guid.Empty : new Guid(obj.ToString());
            return WarehouseAuthAndFiliale.WarehouseDics.ContainsKey(warehouseId)
                ? WarehouseAuthAndFiliale.WarehouseDics[warehouseId]
                : string.Empty;
        }

        protected void Ib_CreationData_Click(object sender, ImageClickEventArgs e)
        {
            IsDefault = false;
            WareHouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
            CompanyId = string.IsNullOrEmpty(RCB_Company.SelectedValue) ? Guid.Empty : new Guid(RCB_Company.SelectedValue);
            SearchKey = TextBoxKeys.Text.Trim();
            StartTime = RDP_StartTime.SelectedDate ?? DateTime.MinValue;
            EndTime = RDP_EndTime.SelectedDate == null ? DateTime.MinValue : Convert.ToDateTime(RDP_EndTime.SelectedDate.Value.AddDays(1).ToString("yyyy-MM-dd 00:00:00.000"));
            State = string.IsNullOrEmpty(RCB_State.SelectedValue) ? PurchasingState.NoSubmit : (PurchasingState)System.Enum.Parse(typeof(PurchasingState), RCB_State.SelectedValue);
            Type = string.IsNullOrEmpty(RCB_Type.SelectedValue) ? PurchasingType.StockDeclare : (PurchasingType)System.Enum.Parse(typeof(PurchasingType), RCB_Type.SelectedValue);
            var goodsId = string.IsNullOrEmpty(RCB_Goods.SelectedValue) ? Guid.Empty : new Guid(RCB_Goods.SelectedValue);
            var goodsIds = _goodsCenterSao.GetRealGoodsIdsByGoodsId(goodsId).ToList();
            if (goodsIds.Count == 0 && goodsId != Guid.Empty)
                GoodsList.Add(goodsId);
            else
                GoodsList = goodsIds.ToList();

            if ((int)State == -1 || (int)State == 2)
            {
                Rgd_Purchasing.MasterTableView.Columns.FindByUniqueName("SurplusMoney").Visible = true;
            }
            else
            {
                Rgd_Purchasing.MasterTableView.Columns.FindByUniqueName("SurplusMoney").Visible = false;
            }
            Rgd_Purchasing.CurrentPageIndex = 0;
            Rgd_Purchasing.Rebind();
        }

        protected void Ram_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(Rgd_Purchasing, e);
        }

        protected void RCB_Company_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text;
                var companyList = (IList<CompanyCussentInfo>)Rcb_CommanyDataSource().Where(p => p.CompanyName.IndexOf(key, StringComparison.Ordinal) > -1).ToList();
                if (e.NumberOfItems >= companyList.Count)
                {
                    e.EndOfItems = true;
                }
                else
                {
                    foreach (CompanyCussentInfo i in companyList)
                    {
                        var item = new RadComboBoxItem { Text = i.CompanyName, Value = i.CompanyId.ToString() };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        // 采购中
        protected void IB_InPurchasing_OnClick(object sender, EventArgs e)
        {
            if (Rgd_Purchasing.SelectedItems.Count == 0)
            {
                RAM.Alert("系统提示：请选择[未提交]状态的采购单！");
                return;
            }

            bool flag = true;
            if (Rgd_Purchasing.SelectedItems.Count == 0)
            {
                RAM.Alert("系统提示：请勾选你要操作的采购单！");
                return;
            }
            var personnelInfo = CurrentSession.Personnel.Get();
            var strMsg = string.Empty;
            foreach (GridDataItem dataItem in Rgd_Purchasing.SelectedItems)
            {
                var purchastState = (int)dataItem.GetDataKeyValue("PurchasingState");
                if (purchastState == (int)PurchasingState.Refusing || purchastState == (int)PurchasingState.WaitingAudit)
                {
                    continue;
                }
                var pInfo = new PurchasingInfo
                {
                    PurchasingID = new Guid(dataItem.GetDataKeyValue("PurchasingID").ToString()),
                    PurchasingNo = dataItem.GetDataKeyValue("PurchasingNo").ToString(),
                    CompanyName = dataItem.GetDataKeyValue("CompanyName").ToString(),
                    PurchasingState = purchastState,
                    CompanyID = new Guid(dataItem.GetDataKeyValue("CompanyID").ToString()),
                    PurchasingFilialeId = new Guid(dataItem.GetDataKeyValue("PurchasingFilialeId").ToString()),
                    IsOut = Convert.ToBoolean(dataItem.GetDataKeyValue("IsOut").ToString()),
                    PurchasingPersonName = dataItem.GetDataKeyValue("PurchasingPersonName").ToString()
                };
                IList<PurchasingDetailInfo> pdlist = _purchasingDetail.Select(pInfo.PurchasingID);
                IList<PurchasingDetailInfo> pricelist = pdlist.Where(p => p.Price < 0).ToList(); //查出未定价的商品
                if (pricelist.Count >= 1)
                {
                    RAM.Alert("采购单为" + pInfo.PurchasingNo + "中有未定价的商品,请确定好价格再更改采购单状态!");
                    continue;
                }
                if (pInfo.PurchasingState == (int)PurchasingState.NoSubmit)
                {
                    //如果没有采购公司则不做任何操作
                    if (pInfo.PurchasingFilialeId == Guid.Empty)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(pInfo.CompanyName))
                    {
                        _purchasing.PurchasingUpdate(pInfo.PurchasingID, pInfo.PurchasingFilialeId, pInfo.IsOut);
                        //采购单采购中添加操作记录添加
                        WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, pInfo.PurchasingID, pInfo.PurchasingNo,
                            OperationPoint.PurchasingManager.Purchasing.GetBusinessInfo(), string.Empty);
                    }
                    else
                    {
                        if (flag)
                        {
                            flag = false;
                            RAM.Alert(pInfo.PurchasingNo + "采购单供应商为空,请进行更改供应商.");
                        }
                    }
                }
                else
                {
                    if (flag)
                    {
                        RAM.Alert("只能对未提交状态的采购单进行采购中操作.");
                        return;
                    }
                }

                #region 如果采购价和绑定价格不一样，修改当前采购设置商品采购价

                var purchasingDetailList = _purchasingDetail.Select(pInfo.PurchasingID).Where(w => w.Price > 0).ToList();
                if (purchasingDetailList.Count > 0)
                {
                    var realGoodsIds = purchasingDetailList.Select(act => act.GoodsID).ToList();
                    var dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsIds);
                    if (dicGoods != null && dicGoods.Count > 0)
                    {
                        var purchaseSetList = _purchaseSet.GetPurchaseSetInfoList(realGoodsIds, pInfo.WarehouseID, pInfo.PurchasingFilialeId);
                        foreach (var info in purchasingDetailList)
                        {
                            if (dicGoods.ContainsKey(info.GoodsID))
                            {
                                var goodsId = dicGoods[info.GoodsID].GoodsId;
                                var purchaseSetInfo = purchaseSetList.FirstOrDefault(w => w.GoodsId == goodsId && w.CompanyId == info.CompanyID);
                                if (purchaseSetInfo != null)
                                {
                                    if (purchaseSetInfo.PurchasePrice != info.Price)
                                    {
                                        purchaseSetInfo.PurchasePrice = info.Price;
                                        string errorMessage;
                                        _purchaseSet.UpdatePurchaseSet(purchaseSetInfo, out errorMessage);
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }
            if (!string.IsNullOrWhiteSpace(strMsg))
            {
                RAM.Alert(strMsg);
                return;
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        protected void RgdPurchasingItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var purchasinginfo = (PurchasingInfo)e.Item.DataItem;
                if (purchasinginfo.IsException)
                {
                    if (string.IsNullOrEmpty(purchasinginfo.PurchasingToDate) || (DateTime.Parse(purchasinginfo.PurchasingToDate) < DateTime.Now))
                        e.Item.Style.Add("background-color", "#FF6666");//红色
                }

                if (purchasinginfo.PurchasingState == (int)PurchasingState.WaitingAudit)
                {
                    e.Item.Style.Add("background-color", "#FF6666");//红色
                }
            }
        }

        /// <summary>返回采购单状态为部分完成的的剩余采购金额
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <param name="purchasingState"></param>
        /// <returns></returns>
        protected decimal ReturnSurplusMoney(Guid purchasingID, int purchasingState)
        {
            if (purchasingState == (int)PurchasingState.PartComplete)
            {
                var purchasingDetailsInfo = _purchasingDetailManager.SelectDetail(purchasingID).Where(ent => ent.State == 0).ToList();
                var sums = purchasingDetailsInfo.Sum(item => (item.Price * ((int)item.PlanQuantity - (int)item.RealityQuantity)));
                return sums;
            }
            return decimal.Parse("0.00");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected bool AllowAuditing(object state)
        {
            if (Convert.ToInt32(state) == (int)PurchasingState.WaitingAudit)
            {
                const string PAGE_NAME = "PurchasingPage.aspx";
                return WebControl.GetPowerOperationPoint(PAGE_NAME, "PurchasAudit");
            }
            return false;
        }

        /// <summary>绑定采购公司
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void IB_BindingFiliale_OnClick(object sender, EventArgs e)
        {
            if (Rgd_Purchasing.SelectedItems.Count == 0)
            {
                RAM.Alert("系统提示：请选择【未提交】、【采购中】状态的采购单！");
                return;
            }
            if (Rgd_Purchasing.SelectedItems.Count > 1)
            {
                RAM.Alert("系统提示：只允许勾选一个采购单！");
                return;
            }
            var purchasingFilialeId = new Guid(RCB_PurchasingFiliale.SelectedValue);
            if (purchasingFilialeId == Guid.Empty)
            {
                RAM.Alert("系统提示：请选择您要绑定的采购公司！");
                return;
            }
            var dataItem = (GridDataItem)Rgd_Purchasing.SelectedItems[0];
            var purchasingId = new Guid(dataItem.GetDataKeyValue("PurchasingID").ToString());
            var pInfo = _purchasing.GetPurchasingById(purchasingId);
            if (!(pInfo.PurchasingState == (int)PurchasingState.NoSubmit || pInfo.PurchasingState == (int)PurchasingState.Purchasing))
            {
                RAM.Alert("系统提示：只允许操作【未提交】、【采购中】状态的采购单！");
                return;
            }
            //TODO  如果一个仓库一个类型只能对应一个物流配送公司，那么此处无任何作用  反之可以多个物流配送公司进货就必须验证公司经营范围
            //var goodsTypes=WMSSao.GetPurchaseGoodsTypes(pInfo.WarehouseID, pInfo.PurchasingFilialeId);

            var result = _purchasing.UpdatePurchasingFilialeId(purchasingId, purchasingFilialeId, true);
            if (!result)
            {
                RAM.Alert("系统提示：绑定采购公司异常，请尝试刷新后重试！");
                return;
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        /// <summary>
        /// 采购单的采购类型、供应商、采购公司一致，就允许进行合并操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// zal 2015-07-30
        protected void lb_Merger_Click(object sender, EventArgs e)
        {
            var purchasingId = string.Empty;//采购单Id
            DateTime arrivalTime = Convert.ToDateTime("1900-01-01");

            #region 符合此7个条件一致的才能合并
            var purchasingFilialeId = string.Empty;//采购公司Id
            var companyName = string.Empty;//供应商
            var purchasingType = string.Empty;//采购类型
            var filialeId = string.Empty;//分公司ID
            var warehouseId = string.Empty;//仓库ID
            var personResponsible = string.Empty;//责人ID
            var isOut = string.Empty;
            #endregion

            string errorMsg = string.Empty;
            int i = 0;
            foreach (GridDataItem item in Rgd_Purchasing.Items)
            {
                if (item.Selected)
                {
                    var purchasingState = EnumAttribute.GetKeyName((PurchasingState)item.GetDataKeyValue("PurchasingState"));
                    if (purchasingState.Equals("未提交"))
                    {
                        if (i == 0)
                        {
                            purchasingId = "," + item.GetDataKeyValue("PurchasingID");
                            arrivalTime = Convert.ToDateTime(item.GetDataKeyValue("ArrivalTime"));

                            purchasingFilialeId = item.GetDataKeyValue("PurchasingFilialeId").ToString();
                            companyName = item.GetDataKeyValue("CompanyName").ToString();
                            purchasingType = item.GetDataKeyValue("PurchasingType").ToString();
                            filialeId = item.GetDataKeyValue("FilialeID").ToString();
                            warehouseId = item.GetDataKeyValue("WarehouseID").ToString();
                            personResponsible = item.GetDataKeyValue("PersonResponsible").ToString();
                            isOut = item.GetDataKeyValue("IsOut").ToString();
                        }
                        else
                        {
                            if (isOut.Equals("True") && !purchasingFilialeId.Equals(item.GetDataKeyValue("PurchasingFilialeId").ToString()))
                            {
                                errorMsg = "“采购公司”不一致";
                                break;
                            }
                            if (!companyName.Equals(item.GetDataKeyValue("CompanyName").ToString()))
                            {
                                errorMsg = "“供应商”不一致";
                                break;
                            }
                            if (!purchasingType.Equals(item.GetDataKeyValue("PurchasingType").ToString()))
                            {
                                errorMsg = "“采购类别”不一致";
                                break;
                            }
                            if (!filialeId.Equals(item.GetDataKeyValue("FilialeID").ToString()))
                            {
                                errorMsg = "“公司”不一致";
                                break;
                            }
                            if (!warehouseId.Equals(item.GetDataKeyValue("WarehouseID").ToString()))
                            {
                                errorMsg = "“所在仓库”不一致";
                                break;
                            }
                            if (!personResponsible.Equals(item.GetDataKeyValue("PersonResponsible").ToString()))
                            {
                                errorMsg = "“责任人”不一致";
                                break;
                            }
                            DateTime temp = Convert.ToDateTime(item.GetDataKeyValue("ArrivalTime"));
                            if (arrivalTime < temp)
                            {
                                arrivalTime = temp;
                            }
                            purchasingId += "," + item.GetDataKeyValue("PurchasingID");
                        }
                    }
                    else
                    {
                        errorMsg = "“状态”需是未提交";
                        break;
                    }
                    i++;
                }
            }

            if (!string.IsNullOrEmpty(errorMsg))
            {
                RAM.Alert("您要合并的采购单" + errorMsg + "!");
                return;
            }
            if (!string.IsNullOrEmpty(purchasingId))
            {
                purchasingId = purchasingId.Substring(1);
                if (purchasingId.Split(',').Count() == 1)
                {
                    RAM.Alert("单个采购单不能合并！");
                    return;
                }
                RAM.ResponseScripts.Add("ConfirmMerger('" + purchasingId + "','" + arrivalTime + "');");
            }
            else
            {
                RAM.Alert("请选择您要合并的采购单!");
            }
        }

        /// <summary>
        /// 合并采购单，合并后作废原采购单，重新生成一个未提交状态的新的采购单
        /// purchasingId  符合条件的采购单id
        /// arrivalTime  符合条件的采购单的最大到货时间
        /// </summary>
        /// <param name="sender">符合条件的采购单id</param>
        /// <param name="e">符合条件的采购单的最大到货时间</param>
        /// zal 2015-07-30
        protected void btn_Merger_Click(object sender, EventArgs e)
        {
            var arrayPurchasingId = Hid_purchasingId.Value.Split(',');
            var purchasingInfo = new PurchasingInfo();
            var purchasingDetailList = new List<PurchasingDetailInfo>();
            int i = 0;
            var purchId = Guid.NewGuid();//新生成一个采购单id
            var warehouseId = Guid.Empty;
            foreach (var item in arrayPurchasingId)
            {
                var purchasingDetailInfoList = _purchasingDetail.Select(new Guid(item));
                purchasingDetailList.AddRange(purchasingDetailInfoList);
                _purchasingManager.PurchasingUpdate(new Guid(item), PurchasingState.Deleted);
                _purchasing.PurchasingDescription(new Guid(item), WebControl.RetrunUserAndTime("作废采购单"));
                if (i == 0)
                {
                    var purchasingInfoModel = _purchasing.GetPurchasingById(new Guid(item));

                    warehouseId = purchasingInfoModel.WarehouseID;
                    //采购单
                    purchasingInfo = new PurchasingInfo(purchId, _codeManager.GetCode(CodeType.PH), purchasingInfoModel.CompanyID, purchasingInfoModel.CompanyName,
                                                    purchasingInfoModel.FilialeID, purchasingInfoModel.WarehouseID, (int)PurchasingState.NoSubmit,
                                                   purchasingInfoModel.PurchasingType, DateTime.Now, DateTime.MaxValue,
                                                   string.Format("[采购类别:{0};采购人:{1}]", EnumAttribute.GetKeyName((PurchasingType)purchasingInfoModel.PurchasingType), CurrentSession.Personnel.Get().RealName),
                                                   Guid.Empty, string.Empty)
                    {
                        Director = purchasingInfoModel.Director,
                        PersonResponsible = purchasingInfoModel.PersonResponsible,
                        ArrivalTime = Convert.ToDateTime(Hid_arrivalTime.Value),
                        PurchasingFilialeId = purchasingInfoModel.PurchasingFilialeId,
                        //IsOut = purchasingInfoModel.IsOut
                    };
                }
                i++;
            }
            try
            {
                _purchasing.PurchasingInsert(purchasingInfo);
                Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(purchasingDetailList.Select(p => p.GoodsID).ToList());
                foreach (var item in purchasingDetailList)
                {
                    if (dicGoods.Count > 0)
                    {
                        bool hasKey = dicGoods.ContainsKey(item.GoodsID);
                        if (hasKey)
                        {
                            var goodsInfo = dicGoods.FirstOrDefault(w => w.Key == item.GoodsID).Value;
                            item.CPrice = _purchaseSet.GetPurchaseSetInfo(goodsInfo.GoodsId, purchasingInfo.PurchasingFilialeId, warehouseId).PurchasePrice;
                            item.Price = item.PurchasingGoodsType == (int)PurchasingGoodsType.Gift ? 0 : item.CPrice;
                        }
                    }

                    item.PurchasingID = purchId;
                }
                _purchasingDetailManager.Save(purchasingDetailList);

                RAM.ResponseScripts.Add("setTimeout(function(){ alert('采购单合并成功!'); refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
            catch (Exception ex)
            {
                RAM.Alert("采购单合并失败!" + ex.Message);
            }
        }

        protected void RCB_Company_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var cb = (RadComboBox)o;
            var companyId = string.IsNullOrEmpty(cb.SelectedItem.Value)
                    ? Guid.Empty
                    : new Guid(cb.SelectedItem.Value);
            var company = _companyCussent.GetCompanyCussent(companyId);
            if (company != null)
            {
                var warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedItem.Value)
                    ? Guid.Empty
                    : new Guid(RCB_Warehouse.SelectedItem.Value);

                BindFilialeList(company.RelevanceFilialeId, warehouseId);
            }

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class ShowPurchasingAdjustForm : WindowsPage
    {
        private readonly IGoodsCenterSao _goodsCenter = new GoodsCenterSao();
        private readonly IPersonnelSao _personnelManager = new PersonnelSao();
        private readonly IPurchasingDetail _purchasingDetail = new PurchasingDetail(GlobalConfig.DB.FromType.Write);
        private readonly IPurchaseSetLog _purchaseSetLog=new PurchaseSetLog(GlobalConfig.DB.FromType.Write);
        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (PurchasingID != Guid.Empty)
                {
                    var list = _purchasingDetail.SelectByGoodsDayAvgSales(PurchasingID);
                    List<Guid> goodsIdOrRealGoodsIdList = list.Select(w => w.GoodsID).Distinct().ToList();
                    Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenter.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(goodsIdOrRealGoodsIdList);
                    if (dicGoods.Count != 0)
                    {
                        var goodsList = new List<Guid>();
                        foreach (var value in dicGoods.Values)
                        {
                            if (!goodsList.Contains(value.GoodsId))
                                goodsList.Add(value.GoodsId);
                        }
                        GoodsIdList = goodsList;
                    }
                }
            }
        }

        #region  公用属性

        /// <summary>
        /// 采购单Id
        /// </summary>
        public Guid PurchasingID
        {
            get
            {
                return string.IsNullOrEmpty(Request.QueryString["PurchasingID"])
                           ? Guid.Empty
                           : new Guid(Request.QueryString["PurchasingID"]);
            }
        }

        public bool AllowAudit
        {
            get
            {
                if (ViewState["AllowAudit"] == null)
                {
                    var allow = GetPowerOperationPoint("LogAudit");
                    ViewState["AllowAudit"] = allow;
                    return allow;
                }
                return (bool)ViewState["AllowAudit"];
            }
        }

        /// <summary>
        /// 主商品Id列表
        /// </summary>

        public List<Guid> GoodsIdList
        {
            set { ViewState["GoodsIdList"] = value; }
            get
            {
                if (ViewState["GoodsIdList"] == null)
                {
                    return new List<Guid>();
                }
                return (List<Guid>)ViewState["GoodsIdList"];
            }
        }

        #endregion

        /// <summary>
        /// 数据源绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgPurchingAdjust_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var dataList = new List<PurchaseSetLogInfo>();
            if (GoodsIdList != null && GoodsIdList.Count != 0)
            {
                var warehouseId = new Guid(Request.QueryString["WarehouseID"]);
                var hostingFilialeId = new Guid(Request.QueryString["HostingFilialeId"]);
                foreach (var goodsId in GoodsIdList)
                {
                    dataList.AddRange(_purchaseSetLog.GetPurchaseSetLogList(goodsId, warehouseId, hostingFilialeId).Where(act => act.ChangeReason == "采购时调价" && act.Statue == (int)PurchaseSetLogStatue.NotAudit).OrderBy(act => act.ChangeDate));
                }
            }
            RgPurchingAdjust.DataSource = dataList;
        }

        /// <summary>
        /// 绑定价格
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RgPurchingAdjust_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;

                var logId = new Guid(dataItem.GetDataKeyValue("LogId").ToString());
                var ginfo = _purchaseSetLog.GetPurchaseSetLogInfo(logId);
                ((Literal)dataItem.FindControl("LitStatue")).Text = EnumAttribute.GetKeyName((PurchaseSetLogStatue)ginfo.Statue);
                ((Literal)dataItem.FindControl("LitOldValue")).Text = ginfo.OldValue.ToString("n4");
                if (ginfo.ChangeValue > 0)
                {
                    ((Literal)dataItem.FindControl("LitNewValue")).Text = "<font color='red'>" + ginfo.NewValue.ToString("n4") + "</font>";
                    ((Literal)dataItem.FindControl("LitChangeValue")).Text = "<font color='red'>+" + ginfo.ChangeValue.ToString("n4") + "</font>";
                }
                else
                {
                    ((Literal)dataItem.FindControl("LitNewValue")).Text = "<font color='green'>" + ginfo.NewValue.ToString("n4") + "</font>";
                    ((Literal)dataItem.FindControl("LitChangeValue")).Text = "<font color='green'>" + ginfo.ChangeValue.ToString("n4") + "</font>";
                }
            }
        }

        #region 取得用户操作权限

        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            return WebControl.GetPowerOperationPoint("GoodsPurchaseSet.aspx", powerName);
        }

        #endregion

        #region 审核通过
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbAuditingOnClick(object sender, EventArgs e)
        {
            if (!AllowAudit)
            {
                RAM.Alert("无审核权限!");
                return;
            }
            IPurchaseSet purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);
            var warehouseId = new Guid(Request.QueryString["WarehouseID"]);
            var hostingFilialeId = new Guid(Request.QueryString["HostingFilialeId"]);
            foreach (GridDataItem item in RgPurchingAdjust.Items)
            {
                var logId = new Guid(item.GetDataKeyValue("LogId").ToString());
                var goodsId = new Guid(item.GetDataKeyValue("GoodsID").ToString());
                var status = Convert.ToInt32(item.GetDataKeyValue("Statue").ToString());
                if (status == (int)PurchaseSetLogStatue.NotAudit)
                {
                    PurchaseSetLogInfo info = _purchaseSetLog.GetPurchaseSetLogInfo(logId);
                    var setInfo = purchaseSet.GetPurchaseSetInfo(goodsId,info.HostingFilialeId, info.WarehouseId);
                    var personnelInfo = CurrentSession.Personnel.Get();
                    info.Statue = (int)PurchaseSetLogStatue.Pass;
                    info.Auditor = personnelInfo.PersonnelId;
                    _purchaseSetLog.UpdatePurchaseSetLog(info);
                    GoodsInfo goodsBaseInfo = _goodsCenter.GetGoodsBaseInfoById(goodsId) ?? new GoodsInfo();
                    //报备管理采购价操作记录添加
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsId, goodsBaseInfo.GoodsCode,
                            OperationPoint.ReportManage.AuditPurchasingPrice.GetBusinessInfo(), "采购价调价审核通过");
                    //更改采购责任人绑定价格
                    if (setInfo != null)
                    {
                        setInfo.PurchasePrice = info.NewValue;
                        string errorMessage;
                        purchaseSet.UpdatePurchaseSet(setInfo, out errorMessage);
                        var realGoodsIds = _goodsCenter.GetRealGoodsIdsByGoodsId(goodsId);
                        //且将采购单中未提交的同主商品的子商品采购价修改
                        _purchasingDetail.UpdatePurchasingDetailPrice(realGoodsIds, setInfo.CompanyId, info.NewValue, warehouseId, hostingFilialeId);
                    }
                }
            }
            //更改采购单状态为未提交
            PurchasingManager.WriteInstance.PurchasingUpdate(PurchasingID, PurchasingState.NoSubmit);
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
        #endregion

        #region 审核不通过
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LbNoAuditingOnClick(object sender, EventArgs e)
        {
            if (!AllowAudit)
            {
                RAM.Alert("无审核权限!");
                return;
            }
            foreach (GridDataItem item in RgPurchingAdjust.Items)
            {
                var logId = new Guid(item.GetDataKeyValue("LogId").ToString());
                var goodsId = new Guid(item.GetDataKeyValue("GoodsID").ToString());
                var status = Convert.ToInt32(item.GetDataKeyValue("Statue").ToString());
                if (status == (int)PurchaseSetLogStatue.NotAudit)
                {
                    PurchaseSetLogInfo info = _purchaseSetLog.GetPurchaseSetLogInfo(logId);
                    var personnelInfo = CurrentSession.Personnel.Get();
                    info.Statue = (int)PurchaseSetLogStatue.NotPass;
                    info.Auditor = personnelInfo.PersonnelId;
                    _purchaseSetLog.UpdatePurchaseSetLog(info);
                    GoodsInfo goodsBaseInfo = _goodsCenter.GetGoodsBaseInfoById(goodsId) ?? new GoodsInfo();
                    //报备管理采购价审核不通过操作记录添加
                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsId, goodsBaseInfo.GoodsCode,
                            OperationPoint.ReportManage.AuditPurchasingPrice.GetBusinessInfo(), "采购价调价审核不通过");
                }
            }
            //更改采购单状态为调价拒绝
            PurchasingManager.WriteInstance.PurchasingUpdate(PurchasingID, PurchasingState.Refusing);
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
        #endregion

        /// <summary>
        /// 获取执行人名称
        /// </summary>
        /// <param name="personnelId"></param>
        /// <returns></returns>
        protected string GetPersonName(Guid personnelId)
        {
            return _personnelManager.GetName(personnelId);
        }
    }
}
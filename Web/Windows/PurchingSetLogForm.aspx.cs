using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model.Goods;
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
    public partial class PurchingSetLogForm : WindowsPage
    {
        private readonly IPurchaseSetLog _purchaseSetLog = new PurchaseSetLog(GlobalConfig.DB.FromType.Write);
        private readonly IPurchaseSet _purchaseSet = new PurchaseSet(GlobalConfig.DB.FromType.Write);
        private static readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Write);
        private readonly PurchasingManager _purchasingManager=new PurchasingManager(_purchasing,null,null,null,null);
        private readonly PersonnelManager _personnelManager=new PersonnelManager();
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly Dictionary<Guid,string> _dics=new Dictionary<Guid, string>(); 

        private SubmitController _submitController;

        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["Checking"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid(), 180);
                ViewState["Checking"] = _submitController;
            }
            return (SubmitController)ViewState["Checking"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                if (GoodsId != Guid.Empty && WarehouseId != Guid.Empty &&
                    !string.IsNullOrEmpty(Request.QueryString["Type"]))
                {
                    LogType = int.Parse(Request.QueryString["Type"]);
                    Page.Title = "商品采购设置更改列表";
                }
                else
                {
                    Page.Title = "调价历史变更明细";
                    RGPurchingSetLog.MasterTableView.Columns.FindByUniqueName("Statue").Visible = false;
                    RGPurchingSetLog.MasterTableView.Columns.FindByUniqueName("Operation").Visible = false;
                }
            }
        }

        /// <summary>
        /// 存储商品采购变更值列表
        /// </summary>
        private IList<PurchaseSetLogInfo> PurchaseSetLogList
        {
            get
            {
                if (ViewState["PurchaseSetLogInfo"] == null)
                {
                    return new List<PurchaseSetLogInfo>();
                }
                return (List<PurchaseSetLogInfo>)ViewState["PurchaseSetLogInfo"];
            }
            set { ViewState["PurchaseSetLogInfo"] = value; }
        }

        protected Guid GoodsId
        {
            get
            {
                if (string.IsNullOrEmpty(Request.QueryString["GoodsId"]))
                {
                    return Guid.Empty;
                }
                return new Guid(Request.QueryString["GoodsId"]);
            }
        }

        protected Guid WarehouseId
        {
            get
            {
                if (string.IsNullOrEmpty(Request.QueryString["WarehouseId"]))
                {
                    return Guid.Empty;
                }
                return new Guid(Request.QueryString["WarehouseId"]);
            }
        }

        protected int LogType
        {
            get
            {
                if (ViewState["LogType"] == null)
                {
                    return 1;
                }
                return (int)ViewState["LogType"];
            }
            set { ViewState["LogType"] = value; }
        }

        //选择商品框的商品绑定
        protected void RGPurchingSetLog_OnNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            long recordCount;
            var pageIndex = RGPurchingSetLog.CurrentPageIndex + 1;
            var pageSize = RGPurchingSetLog.PageSize;
            Guid hostingFilialeId = string.IsNullOrEmpty(Request.QueryString["HostingFilialeId"])
                ? Guid.Empty
                : new Guid(Request.QueryString["HostingFilialeId"]);
            IList<PurchaseSetLogInfo> purchaseSetLogInfos = _purchaseSetLog.GetPurchaseSetLogListByPage(GoodsId, WarehouseId, hostingFilialeId,  pageIndex, pageSize, out recordCount).ToList();
            if (GoodsId == Guid.Empty)
            {
                purchaseSetLogInfos = purchaseSetLogInfos.Where(p => p.Statue == (int)PurchaseSetLogStatue.Pass && p.LogType == 1).ToList();
            }
            else
            {
                purchaseSetLogInfos = purchaseSetLogInfos.Where(p => p.LogType == LogType).ToList();
            }
            PurchaseSetLogList = purchaseSetLogInfos;

            RGPurchingSetLog.DataSource = PurchaseSetLogList;
            RGPurchingSetLog.VirtualItemCount = (int)recordCount;
        }

        //价格表单元格绑定
        protected void RGPurchingSetLog_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;

                var logId = new Guid(dataItem.GetDataKeyValue("LogId").ToString());
                var ginfo =_purchaseSetLog.GetPurchaseSetLogInfo(logId);
                var changeReason = dataItem.GetDataKeyValue("ChangeReason").ToString();
                ((Literal)dataItem.FindControl("LitStatue")).Text = EnumAttribute.GetKeyName((PurchaseSetLogStatue)ginfo.Statue);
                if (ginfo.Statue == (int)PurchaseSetLogStatue.NotAudit)
                {
                    bool result = GetPowerOperationPoint("LogAudit");
                    e.Item.FindControl("LB_Auditing").Visible = result && changeReason != "采购时调价";
                    e.Item.FindControl("LB_NotAudit").Visible = result && changeReason != "采购时调价";
                }
                if (LogType == (int)PurchaseSetLogType.PurchasePrice)
                {
                    ((Literal)dataItem.FindControl("LitOldValue")).Text = WebControl.RemoveDecimalEndZero(ginfo.OldValue);
                    if (ginfo.ChangeValue > 0)
                    {
                        ((Literal)dataItem.FindControl("LitNewValue")).Text = "<font color='red'>" + WebControl.RemoveDecimalEndZero(ginfo.NewValue) + "</font>";
                        ((Literal)dataItem.FindControl("LitChangeValue")).Text = "<font color='red'>+" + WebControl.RemoveDecimalEndZero(ginfo.ChangeValue) + "</font>";
                    }
                    else
                    {
                        ((Literal)dataItem.FindControl("LitNewValue")).Text = "<font color='green'>" + WebControl.RemoveDecimalEndZero(ginfo.NewValue) + "</font>";
                        ((Literal)dataItem.FindControl("LitChangeValue")).Text = "<font color='green'>" + WebControl.RemoveDecimalEndZero(ginfo.ChangeValue) + "</font>";
                    }
                }
                else
                {
                    ((Literal)dataItem.FindControl("LitOldValue")).Text = WebControl.RemoveDecimalEndZero(ginfo.OldValue);
                    if (ginfo.ChangeValue > 0)
                    {
                        ((Literal)dataItem.FindControl("LitNewValue")).Text = "<font color='red'>" + WebControl.RemoveDecimalEndZero(ginfo.NewValue) + "</font>";
                    }
                    else
                    {
                        ((Literal)dataItem.FindControl("LitNewValue")).Text = "<font color='green'>" + WebControl.RemoveDecimalEndZero(ginfo.NewValue) + "</font>";
                    }
                }
            }
        }

        protected void RGPurchingSetLog_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (LogType == (int)PurchaseSetLogType.StockingDays)
            {
                RGPurchingSetLog.Columns[2].Visible = false;
                RGPurchingSetLog.Columns[1].HeaderText = "原报备天数";
                RGPurchingSetLog.Columns[3].HeaderText = "变更后";
            }
        }

        protected void RGPurchingSetLog_ColumnCreated(object sender, GridColumnCreatedEventArgs e)
        {
            if (LogType == (int)PurchaseSetLogType.StockingDays)
            {
                RGPurchingSetLog.Columns[2].Visible = false;
                RGPurchingSetLog.Columns[1].HeaderText = "原报备天数";
                RGPurchingSetLog.Columns[3].HeaderText = "变更后";
            }
        }

        protected string GetPersonName(Guid personId)
        {
            if (_dics.ContainsKey(personId)) return _dics[personId];
            var name=_personnelManager.GetName(personId);
            _dics.Add(personId,name);
            return name;
        }

        protected void RGPurchingSetLog_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.Item != null && (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem))
            {
                var dataItem = (GridDataItem)e.Item;
                var logId = new Guid(dataItem.GetDataKeyValue("LogId").ToString());
                var goodsId = new Guid(dataItem.GetDataKeyValue("GoodsID").ToString());
                PurchaseSetLogInfo info = _purchaseSetLog.GetPurchaseSetLogInfo(logId);
                var setInfos = _purchaseSet.GetPurchaseSetInfo(goodsId, info.WarehouseId);
                var setInfo = setInfos.FirstOrDefault();
                switch (e.CommandName)
                {
                    case "Audit":
                        {
                            info.Statue = (int)PurchaseSetLogStatue.Pass;
                            info.Auditor = CurrentSession.Personnel.Get().PersonnelId;
                            if (setInfo != null)
                            {
                                if (info.LogType == (int)PurchaseSetLogType.PurchasePrice)
                                {
                                    setInfo.PurchasePrice = info.NewValue;
                                }
                            }
                            _purchaseSetLog.UpdatePurchaseSetLog(info);

                            GoodsInfo goodsBaseInfo = _goodsCenterSao.GetGoodsBaseInfoById(goodsId) ?? new GoodsInfo();

                            //报备管理采购价或报备天数审核操作记录添加
                            var personnelInfo = CurrentSession.Personnel.Get();
                            if (info.LogType == (int)PurchaseSetLogType.PurchasePrice)
                            {
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsId, goodsBaseInfo.GoodsCode,
                                    OperationPoint.ReportManage.AuditPurchasingPrice.GetBusinessInfo(), "采购价审核通过");
                            }
                            else
                            {
                                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsId, goodsBaseInfo.GoodsCode,
                                    OperationPoint.ReportManage.AuditDeclareDays.GetBusinessInfo(), "报备天数审核通过");
                            }
                            string errorMessage;
                            _purchaseSet.UpdatePurchaseSet(setInfo, out errorMessage);

                            //判断商品是否存在调价待审核的采购单，如果改为待提交
                            //获取
                            var realGoodsIds = _goodsCenterSao.GetRealGoodsIdsByGoodsId(goodsId).ToList();
                            if (realGoodsIds.Count == 0) realGoodsIds.Add(goodsBaseInfo.GoodsId);
                            var purchasingIds = _purchasing.GetPurchasingByRealGoodsIdList(realGoodsIds,
                                                                                                  (int)
                                                                                                  PurchasingState.
                                                                                                      WaitingAudit);
                            if (purchasingIds != null && purchasingIds.Count != 0)
                            {
                                foreach (var purchasingId in purchasingIds)
                                {
                                    _purchasingManager.PurchasingUpdate(purchasingId, PurchasingState.NoSubmit);
                                    _purchasing.PurchasingDescription(purchasingId, WebControl.RetrunUserAndTime("采购提价审核通过"));
                                }
                            }
                            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        }
                        break;
                    case "NotAudit":
                        {
                            if (info.LogId != Guid.Empty)
                            {
                                info.Statue = (int)PurchaseSetLogStatue.NotPass;
                                info.Auditor = CurrentSession.Personnel.Get().PersonnelId;
                                _purchaseSetLog.UpdatePurchaseSetLog(info);

                                GoodsInfo goodsBaseInfo = _goodsCenterSao.GetGoodsBaseInfoById(goodsId) ?? new GoodsInfo();
                                var personnelInfo = CurrentSession.Personnel.Get();
                                //报备管理采购价或报备天数审核操作记录添加
                                if (info.LogType == (int)PurchaseSetLogType.PurchasePrice)
                                {
                                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsId, goodsBaseInfo.GoodsCode,
                                        OperationPoint.ReportManage.AuditPurchasingPrice.GetBusinessInfo(), "采购价审核不通过");
                                }
                                else
                                {
                                    WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, goodsId, goodsBaseInfo.GoodsCode,
                                        OperationPoint.ReportManage.AuditDeclareDays.GetBusinessInfo(), "报备天数审核不通过");
                                }
                                //判断商品是否存在调价待审核的采购单，如果改为调价拒绝
                                var realGoodsIds = _goodsCenterSao.GetRealGoodsIdsByGoodsId(goodsId).ToList();
                                if (realGoodsIds.Count == 0) realGoodsIds.Add(goodsBaseInfo.GoodsId);
                                var purchasingIds = _purchasing.GetPurchasingByRealGoodsIdList(realGoodsIds,
                                                                                                      (int)
                                                                                                      PurchasingState.
                                                                                                          WaitingAudit);
                                if (purchasingIds != null && purchasingIds.Count != 0)
                                {
                                    foreach (var purchasingId in purchasingIds)
                                    {
                                        _purchasingManager.PurchasingUpdate(purchasingId, PurchasingState.Refusing);
                                        _purchasing.PurchasingDescription(purchasingId, WebControl.RetrunUserAndTime("采购提价审核不通过"));
                                    }
                                }
                                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                            }
                            break;

                        }
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
    }
}
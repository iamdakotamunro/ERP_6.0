using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using ERP.UI.Web.Base;
using ERP.BLL.Interface;
using ERP.BLL.Implement;

namespace ERP.UI.Web.Windows
{
    public partial class ApprovalPriceRed : WindowsPage
    {
        static readonly ICompanyCussent _companyCussentDao = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly IDocumentRedDao _documentRedDao = new DocumentRedDao(GlobalConfig.DB.FromType.Write);
        static readonly CodeManager _codeManager = new CodeManager();
        static readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Write);
        static readonly IRealTimeGrossSettlementManager _realTimeGrossSettlementManager = new RealTimeGrossSettlementManager(GlobalConfig.DB.FromType.Write);

        #region 属性

        /// <summary>
        /// 红冲明细
        /// </summary>
        private IList<DocumentRedDetailInfo> DocumentRedDetailList
        {
            get
            {
                if (ViewState["DocumentRedDetailList"] == null)
                    return new List<DocumentRedDetailInfo>();
                return (IList<DocumentRedDetailInfo>)ViewState["DocumentRedDetailList"];
            }
            set { ViewState["DocumentRedDetailList"] = value; }
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var linkTradeId = Request.QueryString["LinkTradeId"];
                if (!string.IsNullOrEmpty(linkTradeId))
                {
                    var isRead = string.IsNullOrEmpty(Request.QueryString["Read"]);
                    btn_Approval.Visible = isRead;
                    btn_Return.Visible = isRead;
                    LoadEditData(linkTradeId,isRead);//加载编辑数据
                }
            }
        }

        //加载编辑数据
        protected void LoadEditData(string linkTradeId,bool isRead)
        {
            #region 红冲数据
            var documentRedInfos = _documentRedDao.GetDocumentRedInfoByLinkTradeId(new Guid(linkTradeId));
            var documentRedInfo = isRead?documentRedInfos.FirstOrDefault(act => act.DocumentType != (int)DocumentType.RedDocument):
                documentRedInfos.FirstOrDefault(act => act.DocumentType == Convert.ToInt32(Request.QueryString["DocumentType"]));
            if (documentRedInfo == null) return;
            RTB_LinkTradeCode.Text = documentRedInfo.LinkTradeCode;
            RTB_LinkTradeCode.ReadOnly = true;
            RtbNewCode.Text = documentRedInfo.TradeCode;
            var wList = CurrentSession.Personnel.WarehouseList;
            if (wList != null)
            {
                var warehouse = wList.FirstOrDefault(act => act.WarehouseId == documentRedInfo.WarehouseId);
                if (warehouse != null)
                {
                    txt_Warehouse.Text = warehouse.WarehouseName ;
                    var slist =warehouse.Storages!=null? warehouse.Storages.FirstOrDefault(p => p.StorageType == documentRedInfo.StorageType):null;
                    if (slist != null)
                    {
                        txt_StorageType.Text = slist.StorageTypeName;
                        var hlist =slist.Filiales!=null?slist.Filiales.FirstOrDefault(p => p.HostingFilialeId == documentRedInfo.FilialeId):null;
                        txt_HostingFiliale.Text = hlist == null ? "" : hlist.HostingFilialeName;
                    }
                }
            }

            txt_DateCreated.Text = documentRedInfo.DateCreated.ToString("yyyy-MM-dd");
            var company = _companyCussentDao.GetCompanyCussent(documentRedInfo.ThirdCompanyId);
            txt_ThirdCompany.Text = company != null ? company.CompanyName : "";
            txt_Transactor.Text = documentRedInfo.Transactor;
            txt_OldDescription.Text = documentRedInfo.LinkDescription;
            txt_Description.Text = documentRedInfo.Description;
            #endregion

            #region 红冲明细
            DocumentRedDetailList = _documentRedDao.GetDocumentRedDetailListByRedId(documentRedInfo.RedId);
            #endregion
        }

        #region 数据列表相关
        //加载出入库明细数据
        protected void RGGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGGoods.DataSource = DocumentRedDetailList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }
        #endregion

        //核准
        protected void btnApproval_Click(object sender, EventArgs e)
        {
            var linkTradeId = Request.QueryString["LinkTradeId"];
            if (string.IsNullOrEmpty(linkTradeId))
            {
                MessageBox.Show(this, "未获取到单据主键");
                return;
            }
            var description = (string.IsNullOrEmpty(txt_Description.Text) ? "暂无说明" : txt_Description.Text);
            var documentRedInfos = _documentRedDao.GetDocumentRedInfoByLinkTradeId(new Guid(linkTradeId));
            //新单
            var newDocumentRedInfo = documentRedInfos.FirstOrDefault(act => act.DocumentType != (int)DocumentType.RedDocument);
            if (newDocumentRedInfo == null) return;
            var newMemo = WebControl.RetrunUserAndTime("[【新单核准】:" + description + ";]");
            var isIn = int.Parse(Request.QueryString["RedType"]).Equals((int) RedType.ModifyPriceInRed);
            //新单
            var newReckoningInfo = new ReckoningInfo
            {
                ReckoningId = Guid.NewGuid(),
                FilialeId = newDocumentRedInfo.FilialeId,
                ThirdCompanyID = newDocumentRedInfo.ThirdCompanyId,
                TradeCode = _codeManager.GetCode(CodeType.PY),
                DateCreated = DateTime.Now,
                Description = newMemo,
                AccountReceivable = isIn ? -Math.Abs(newDocumentRedInfo.AccountReceivable) : Math.Abs(newDocumentRedInfo.AccountReceivable),
                ReckoningType = isIn ? (int)ReckoningType.Defray : (int)ReckoningType.Income,
                State = (int)ReckoningStateType.Currently,
                AuditingState = (int)AuditingState.Yes,
                IsChecked = (int)CheckType.NotCheck,
                LinkTradeCode = newDocumentRedInfo.TradeCode,
                WarehouseId = newDocumentRedInfo.WarehouseId,
                ReckoningCheckType = (int)ReckoningCheckType.Other,
                LinkTradeType = isIn ? (int)ReckoningLinkTradeType.StockIn : (int)ReckoningLinkTradeType.StockOut,
                IsOut = newDocumentRedInfo.IsOut
            };
            //红冲单
            var documentRedInfo = documentRedInfos.FirstOrDefault(act => act.DocumentType == (int)DocumentType.RedDocument);
            if (documentRedInfo == null) return;
            var memo = WebControl.RetrunUserAndTime("[【红冲单核准】:" + description + ";]");

            //红冲单
            var reckoningInfo = new ReckoningInfo
            {
                ReckoningId = Guid.NewGuid(),
                FilialeId = documentRedInfo.FilialeId,
                ThirdCompanyID = documentRedInfo.ThirdCompanyId,
                TradeCode = _codeManager.GetCode(CodeType.GT),
                DateCreated = DateTime.Now,
                Description = memo,
                AccountReceivable = isIn ? Math.Abs(documentRedInfo.AccountReceivable) : -Math.Abs(documentRedInfo.AccountReceivable),
                ReckoningType = isIn ? (int)ReckoningType.Income : (int)ReckoningType.Defray,
                State = (int)ReckoningStateType.Cancellation,
                AuditingState = (int)AuditingState.Yes,
                IsChecked = (int)CheckType.IsChecked,
                LinkTradeCode = documentRedInfo.TradeCode,
                WarehouseId = documentRedInfo.WarehouseId,
                ReckoningCheckType = (int)ReckoningCheckType.Other,
                LinkTradeType = isIn?(int)ReckoningLinkTradeType.StockIn: (int)ReckoningLinkTradeType.StockOut,
                //IsOut = documentRedInfo.IsOut
            };

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                try
                {
                    string errorMessage;
                    /*---插入往来账时要先插红冲单的往来账，后插新单的往来账---*/
                    //红冲单
                    var result2 = _documentRedDao.UpdateStateDocumentRed(documentRedInfo.RedId, DocumentRedState.Red, description, memo);
                    if (result2)
                    {
                        _reckoning.Insert(reckoningInfo, out errorMessage);
                    }

                    //新单
                    var result1 = _documentRedDao.UpdateStateDocumentRed(newDocumentRedInfo.RedId, DocumentRedState.Finished, description, newMemo);
                    if (result1)
                    {
                        _reckoning.Insert(newReckoningInfo, out errorMessage);
                    }

                    var result = _reckoning.UpdateCheckState(documentRedInfo.LinkTradeCode, isIn ? (int)ReckoningLinkTradeType.StockIn : (int)ReckoningLinkTradeType.StockOut, isIn ?
                        (int)ReckoningType.Defray : (int)ReckoningType.Income, (int)ReckoningCheckType.Other, (int)CheckType.IsChecked);

                    if (result)
                    {
                        #region 入库红冲单产生的新入库单，计算一次结算价
                        if (newDocumentRedInfo.RedType == (int)RedType.ModifyPriceInRed && newDocumentRedInfo.DocumentType == (int)DocumentType.NewInDocument)
                        {
                            var items = _realTimeGrossSettlementManager.CreateByNewInDocumentAtRed(newDocumentRedInfo.RedId, DateTime.Now).ToList();
                            if (items.Count > 0)
                            {
                                items.ForEach(m => _realTimeGrossSettlementManager.Calculate(m));
                            }
                        }
                        #endregion
                    }
                    if (result)
                    {
                        ts.Complete();
                        MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    }
                    else
                    {
                        MessageBox.Show(this, "往来账状态更新失败！");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, "审核失败！" + ex.Message);
                }
            }
        }

        //核退
        protected void btnReturn_Click(object sender, EventArgs e)
        {
            var linkTradeId = Request.QueryString["LinkTradeId"];
            if (string.IsNullOrEmpty(linkTradeId))
            {
                MessageBox.Show(this, "未获取到单据主键");
                return;
            }
            var description = (string.IsNullOrEmpty(txt_Description.Text) ? "暂无说明" : txt_Description.Text);
            var memo =WebControl.RetrunUserAndTime("[【新单核退】:" + description + ";]");
            var documentReds = _documentRedDao.GetDocumentRedInfoByLinkTradeId(new Guid(linkTradeId));
            bool result = true;
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                foreach (var documentRedInfo in documentReds)
                {
                    if(!result)break;
                    result=_documentRedDao.UpdateStateDocumentRed(documentRedInfo.RedId, DocumentRedState.Refuse, description, memo);
                }
                if(result)
                    ts.Complete();
            }
            if (result)
            {
                MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }
    }
}
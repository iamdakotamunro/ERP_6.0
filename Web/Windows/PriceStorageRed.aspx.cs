using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IUtilities;
using ERP.DAL.Utilities;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Common;
using Telerik.Web.UI;
using ERP.UI.Web.Base;

namespace ERP.UI.Web.Windows
{
    public partial class PriceStorageRed : WindowsPage
    {
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);
        static readonly IReckoning _reckoning = new Reckoning(GlobalConfig.DB.FromType.Write);
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly ICompanyCussent _companyCussentDao = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        static readonly CodeManager _code = new CodeManager();
        static readonly IDocumentRedDao _documentRedDao = new DocumentRedDao(GlobalConfig.DB.FromType.Write);
        static readonly ICompanyFundReceipt _companyFundReceipt = new DAL.Implement.Inventory.CompanyFundReceipt(GlobalConfig.DB.FromType.Read);
        readonly IUtility _utility = new UtilityDal(GlobalConfig.DB.FromType.Write);

        #region 属性
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

        /// <summary>
        /// 出入库记录
        /// </summary>
        private StorageRecordInfo StorageRecordInfo
        {
            get
            {
                if (ViewState["StorageRecordInfo"] == null)
                    return null;
                return (StorageRecordInfo)ViewState["StorageRecordInfo"];
            }
            set { ViewState["StorageRecordInfo"] = value; }
        }

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
                var redId = Request.QueryString["RedId"];
                if (!string.IsNullOrEmpty(redId))
                {
                    LbOriginalCode.Text = "原单编号：";
                    LbNewCode.Text = "新单编号：";
                    LoadEditData(redId);//加载编辑数据
                }
                else
                {
                    LbOriginalCode.Text = int.Parse(Request.QueryString["RedType"]) == (int)RedType.ModifyPriceInRed ?
                        "入库编号：" : "出库编号：";
                    LbNewCode.Text = "";
                    RtbNewTradeCode.Visible = false;
                }
            }
        }

        //加载编辑数据
        protected void LoadEditData(string redId)
        {
            #region 红冲数据
            var documentRedInfo = _documentRedDao.GetDocumentRed(new Guid(redId));
            RtbNewTradeCode.Text = documentRedInfo.TradeCode;
            RTB_LinkTradeCode.Text = documentRedInfo.LinkTradeCode;
            RTB_LinkTradeCode.ReadOnly = true;

            var wList = WMSSao.GetWarehouseAuth(Personnel.PersonnelId).FirstOrDefault(p => p.WarehouseId == documentRedInfo.WarehouseId);
            if (wList != null)
            {
                txt_Warehouse.Text = wList.WarehouseName;
                var slist = wList.Storages.FirstOrDefault(p => p.StorageType == documentRedInfo.StorageType);
                if (slist != null)
                {
                    txt_StorageType.Text = slist.StorageTypeName;
                    var hlist = slist.Filiales.FirstOrDefault(p => p.HostingFilialeId == documentRedInfo.FilialeId);
                    txt_HostingFiliale.Text = hlist == null ? "" : hlist.HostingFilialeName;
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
            DocumentRedDetailList = _documentRedDao.GetDocumentRedDetailListByRedId(new Guid(redId));
            #endregion
        }

        //根据出入库单号加载数据
        protected void RTB_LinkTradeCode_OnTextChanged(object sender, EventArgs e)
        {
            ResetData();

            #region 验证数据
            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                RAM.Alert(errorMsg);
                return;
            }
            #endregion

            #region 出入库数据
            txt_DateCreated.Text = StorageRecordInfo.DateCreated.ToString("yyyy-MM-dd");
            txt_Transactor.Text = StorageRecordInfo.Transactor;
            txt_OldDescription.Text = StorageRecordInfo.Description;
            var warehouseList = CurrentSession.Personnel.WarehouseList;
            if (warehouseList != null && warehouseList.Count > 0)
            {
                var warehouse = warehouseList.FirstOrDefault(act => act.WarehouseId == StorageRecordInfo.WarehouseId);
                if (warehouse != null)
                {
                    txt_Warehouse.Text = warehouse.WarehouseName;
                    var storageType = warehouse.Storages != null ? warehouse.Storages.FirstOrDefault(act => act.StorageType == StorageRecordInfo.StorageType) : null;
                    if (storageType != null)
                    {
                        txt_StorageType.Text = storageType.StorageTypeName;
                        if (storageType.Filiales != null)
                        {
                            var hostingFiliale = storageType.Filiales.FirstOrDefault(
                                act => act.HostingFilialeId == StorageRecordInfo.FilialeId);
                            txt_HostingFiliale.Text = hostingFiliale == null ? "" : hostingFiliale.HostingFilialeName;
                        }
                    }
                }
            }
            var company = _companyCussentDao.GetCompanyCussent(StorageRecordInfo.ThirdCompanyID);
            txt_ThirdCompany.Text = company != null ? company.CompanyName : "";
            #endregion

            #region 出入库明细
            DocumentRedDetailList = _documentRedDao.GetDocumentRedDetailListByStockId(StorageRecordInfo.StockId);
            if (DocumentRedDetailList.Any())
            {
                Dictionary<Guid, GoodsInfo> dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(DocumentRedDetailList.Select(p => p.RealGoodsId).Distinct().ToList());
                foreach (var info in DocumentRedDetailList)
                {
                    info.Units = dicGoods.ContainsKey(info.RealGoodsId) ? dicGoods[info.RealGoodsId].Units : string.Empty;
                }
            }

            if (DocumentRedDetailList.Any())
            {
                RGGoods.Rebind();
            }
            #endregion
        }

        #region 数据列表相关
        //加载出入库明细数据
        protected void RGGoods_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RGGoods.DataSource = DocumentRedDetailList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }
        #endregion

        //保存
        protected void btn_InsterStock(object sender, EventArgs e)
        {
            var redId = Request.QueryString["RedId"];
            if (string.IsNullOrEmpty(redId))
            {
                Add();//保存数据
            }
            else
            {
                Update(redId);//修改数据
            }
        }

        //添加
        private void Add()
        {
            #region 验证数据
            var errorMsg = CheckData();
            if (!string.IsNullOrEmpty(errorMsg))
            {
                RAM.Alert(errorMsg);
                return;
            }
            #endregion
            bool isInStorage = int.Parse(Request.QueryString["RedType"]) == (int)RedType.ModifyPriceInRed;
            //红冲单据(红冲单)
            var documentRedInfo = new DocumentRedInfo
            {
                RedId = Guid.NewGuid(),
                LinkTradeCode = StorageRecordInfo.TradeCode,
                FilialeId = StorageRecordInfo.FilialeId,
                LinkDateCreated = StorageRecordInfo.DateCreated,
                ThirdCompanyId = StorageRecordInfo.ThirdCompanyID,
                Transactor = StorageRecordInfo.Transactor,
                WarehouseId = StorageRecordInfo.WarehouseId,
                StorageType = StorageRecordInfo.StorageType,
                LinkDescription = StorageRecordInfo.Description,
                Description = txt_Description.Text,
                AccountReceivable = StorageRecordInfo.AccountReceivable,
                SubtotalQuantity = StorageRecordInfo.SubtotalQuantity,
                LinkTradeId = StorageRecordInfo.StockId,

                TradeCode = _code.GetCode(isInStorage ? CodeType.CI : CodeType.CO),
                DateCreated = DateTime.Now,
                RedType = int.Parse(Request.QueryString["RedType"]),
                DocumentType = (int)DocumentType.RedDocument,
                State = (int)DocumentRedState.WaitRed,
                Memo = Common.WebControl.RetrunUserAndTime("[【红冲单添加】:" + (string.IsNullOrEmpty(txt_Description.Text) ? "暂无说明" : txt_Description.Text) + ";]"),
                //IsOut = StorageRecordInfo.IsOut
            };
            string errorMessage;
            string errorMessageNew;
            //红冲单据详细(红冲单)
            var documentRedDetailList = GetRgGoodsData(documentRedInfo.RedId, 2, out errorMessage);

            var newRedId = Guid.NewGuid();
            //红冲单据详细(新单)
            var newDocumentRedDetailList = GetRgGoodsData(newRedId, 1, out errorMessageNew);
            if (!string.IsNullOrEmpty(errorMessage) || !string.IsNullOrEmpty(errorMessageNew))
            {
                RAM.Alert("修改价为必填项！");
                return;
            }
            //总金额
            decimal accountReceivable = 0;
            if (newDocumentRedDetailList.Any())
            {
                accountReceivable += newDocumentRedDetailList.Sum(goodsStockInfo => Convert.ToDecimal(goodsStockInfo.Quantity) * goodsStockInfo.UnitPrice);
            }

            //红冲单据(新单)
            CodeType codeType;
            if (isInStorage)
            {
                codeType = StorageRecordInfo.StockType == (int)StorageRecordType.BuyStockIn ? CodeType.RK : CodeType.SI;
            }
            else
            {
                codeType = StorageRecordInfo.StockType == (int)StorageRecordType.SellStockOut ? CodeType.SL : CodeType.SO;
            }
            var newDocumentRedInfo = new DocumentRedInfo
            {
                RedId = newRedId,
                LinkTradeCode = StorageRecordInfo.TradeCode,
                FilialeId = documentRedInfo.FilialeId,
                LinkDateCreated = documentRedInfo.DateCreated,
                ThirdCompanyId = documentRedInfo.ThirdCompanyId,
                Transactor = Personnel.RealName,
                WarehouseId = documentRedInfo.WarehouseId,
                StorageType = documentRedInfo.StorageType,
                LinkDescription = documentRedInfo.LinkDescription,
                Description = txt_Description.Text,
                AccountReceivable = accountReceivable,
                SubtotalQuantity = documentRedInfo.SubtotalQuantity,
                LinkTradeId = StorageRecordInfo.StockId,

                TradeCode = _code.GetCode(codeType),
                DateCreated = DateTime.Now,
                RedType = int.Parse(Request.QueryString["RedType"]),
                DocumentType = isInStorage ? (int)DocumentType.NewInDocument : (int)DocumentType.NewOutDocument,
                State = (int)DocumentRedState.WaitAudit,
                Memo = Common.WebControl.RetrunUserAndTime(string.Format("[【新{0}单添加】：{1}]", isInStorage ? "入库" : "出库", string.IsNullOrEmpty(txt_Description.Text) ? "暂无说明" : txt_Description.Text)),
                //IsOut = documentRedInfo.IsOut
            };

            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                //添加红冲单
                _documentRedDao.InsertDocumentRed(documentRedInfo);
                _documentRedDao.BatchInsertDocumentRedDetail(documentRedDetailList);
                //添加新单
                _documentRedDao.InsertDocumentRed(newDocumentRedInfo);
                _documentRedDao.BatchInsertDocumentRedDetail(newDocumentRedDetailList);
                ts.Complete();
            }
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        //修改
        private void Update(string redId)
        {
            string errorMsg;
            //红冲单据详细(新单)
            var newDocumentRedDetailList = GetRgGoodsData(new Guid(redId), 1, out errorMsg);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                RAM.Alert("修改价为必填项！");
                return;
            }
            //总金额
            decimal accountReceivable = 0;
            if (newDocumentRedDetailList.Any())
            {
                accountReceivable += newDocumentRedDetailList.Sum(goodsStockInfo => Convert.ToDecimal(goodsStockInfo.Quantity) * goodsStockInfo.UnitPrice);
            }
            var red = _documentRedDao.GetDocumentRed(new Guid(redId));
            DocumentRedInfo original = null;
            if (red.State == (Int32)DocumentRedState.Refuse)
            {
                original = _documentRedDao.GetDocumentRedByNewRedId(new Guid(redId));
            }
            var memo = Common.WebControl.RetrunUserAndTime("[【新单修改】:" + (string.IsNullOrEmpty(txt_Description.Text) ? "暂无说明" : txt_Description.Text) + ";]");
            using (var ts = new TransactionScope(TransactionScopeOption.Required))
            {
                _documentRedDao.UpdateDocumentRedByRedId(new Guid(redId), accountReceivable, txt_Description.Text, memo, (int)DocumentRedState.WaitAudit);
                if (original != null)
                {
                    _documentRedDao.UpdateDocumentRedByRedId(original.RedId, original.AccountReceivable, "", "", (int)DocumentRedState.WaitRed);
                }
                _documentRedDao.DelDocumentRedDetailByRedId(new Guid(redId));
                _documentRedDao.BatchInsertDocumentRedDetail(newDocumentRedDetailList);
                ts.Complete();
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        /// <summary>获得商品详细
        /// </summary>
        /// <param name="redId"></param>
        /// <param name="type">1.新单 2.红冲单</param>
        /// <param name="errorMsg"></param>
        /// <returns></returns>
        private List<DocumentRedDetailInfo> GetRgGoodsData(Guid redId, int type, out string errorMsg)
        {
            errorMsg = string.Empty;
            var list = new List<DocumentRedDetailInfo>();
            for (int i = 0; i < RGGoods.Items.Count; i++)
            {
                var info = new DocumentRedDetailInfo();
                //修改价
                var strModifyPrice = (TextBox)RGGoods.Items[i].FindControl("txt_ModifyPrice");
                //原价
                var strUnitPrice = (Label)RGGoods.Items[i].FindControl("lbl_UnitPrice");
                if (strModifyPrice == null || string.IsNullOrEmpty(strModifyPrice.Text))
                {
                    errorMsg = "修改价为必填项！";
                }

                var goodsId = RGGoods.Items[i].GetDataKeyValue("GoodsId");
                var realGoodsId = RGGoods.Items[i].GetDataKeyValue("RealGoodsId");
                var goodsCode = RGGoods.Items[i].GetDataKeyValue("GoodsCode");
                var goodsName = RGGoods.Items[i]["GoodsName"].Text;
                var specification = RGGoods.Items[i]["Specification"].Text;
                var units = RGGoods.Items[i]["Units"].Text;
                var quantity = int.Parse(RGGoods.Items[i]["Quantity"].Text);

                info.Id = Guid.NewGuid();
                info.RedId = redId;
                info.GoodsId = new Guid(goodsId.ToString());
                info.GoodsName = goodsName;
                info.GoodsCode = goodsCode.ToString();
                info.RealGoodsId = new Guid(realGoodsId.ToString());
                info.Specification = specification;
                info.Quantity = quantity;
                if (type == 1)
                {
                    info.UnitPrice = strModifyPrice == null ? 0 : (string.IsNullOrEmpty(strModifyPrice.Text) ? 0 : decimal.Parse(strModifyPrice.Text));
                }
                else if (type == 2)
                {
                    info.UnitPrice = strUnitPrice == null ? 0 : (string.IsNullOrEmpty(strUnitPrice.Text) ? 0 : decimal.Parse(strUnitPrice.Text));
                }
                info.OldUnitPrice = strUnitPrice == null ? 0 : (string.IsNullOrEmpty(strUnitPrice.Text) ? 0 : decimal.Parse(strUnitPrice.Text));
                info.Units = units;
                list.Add(info);
            }

            return list;
        }

        /// <summary>
        /// 清除绑定的页面内容
        /// </summary>
        private void ResetData()
        {
            txt_HostingFiliale.Text = string.Empty;
            txt_DateCreated.Text = string.Empty;
            txt_ThirdCompany.Text = string.Empty;
            txt_Transactor.Text = string.Empty;
            txt_Warehouse.Text = string.Empty;
            txt_StorageType.Text = string.Empty;
            txt_OldDescription.Text = string.Empty;
            txt_Description.Text = string.Empty;
            RtbNewTradeCode.Text = string.Empty;
            DocumentRedDetailList = new List<DocumentRedDetailInfo>();
        }

        //验证数据
        private string CheckData()
        {
            var tradeCode = RTB_LinkTradeCode.Text.Trim();
            if (string.IsNullOrWhiteSpace(tradeCode))
            {
                return "单据编号”不允许为空！";
            }
            var result = _utility.CheckExists("DocumentRed", "LinkTradeCode", tradeCode);
            if (result)
            {
                return string.Format("单据{0}已存在，不允许多次红冲！", tradeCode);
            }

            StorageRecordInfo = _storageRecordDao.GetStorageRecord(tradeCode);
            if (StorageRecordInfo == null || StorageRecordInfo.StockId == Guid.Empty)
            {
                return string.Format("单据{0}不存在，请重新输入！", tradeCode);
            }

            StringBuilder errorMsg = new StringBuilder();

            if (StorageRecordInfo.StockState != (int)StorageRecordState.Finished)
            {
                errorMsg.AppendFormat("单据{0}未完成，不允许红冲！", tradeCode).Append("\\n");
            }

            if (int.Parse(Request.QueryString["RedType"]).Equals((int)RedType.ModifyPriceInRed))
            {
                if (StorageRecordInfo.StockType != (int)StorageRecordType.BuyStockIn && StorageRecordInfo.StockType != (int)StorageRecordType.SellReturnIn)
                {
                    errorMsg.AppendFormat("单据{0}的类型不符合(采购进货,销售退货)，不允许红冲！", tradeCode).Append("\\n");
                }
            }
            else
            {
                if (StorageRecordInfo.StockType != (int)StorageRecordType.BuyStockOut && StorageRecordInfo.StockType != (int)StorageRecordType.SellStockOut && StorageRecordInfo.StockType != (int)StorageRecordType.AfterSaleOut)
                {
                    errorMsg.AppendFormat("单据{0}的类型不符合(采购退货,销售出库,售后退货)，不允许红冲！", tradeCode).Append("\\n");
                }
            }
            var status = _companyFundReceipt.GetFundReceiptStatusByLinkTradeCode(StorageRecordInfo.ThirdCompanyID, tradeCode);
            if (status != (int)CompanyFundReceiptState.Cancel && status > 0)
            {
                if (status >= (int)CompanyFundReceiptState.WaitAuditing || status < (int)CompanyFundReceiptState.Finish)
                {
                    errorMsg.AppendFormat("单据{0}已经开始进行收付款，则无法进行红冲！", tradeCode).Append("\\n");
                }
                if (status == (int)CompanyFundReceiptState.Finish)
                {
                    errorMsg.AppendFormat("单据{0}已经开始完成收付款，则无法进行红冲！", tradeCode).Append("\\n");
                }
            }

            if (!string.IsNullOrEmpty(errorMsg.ToString()))
            {
                return errorMsg.ToString();
            }
            else
            {
                if (_reckoning.GetReckoningInfoByTradeCode(StorageRecordInfo.TradeCode) == Guid.Empty)
                {
                    return string.Format("单据{0}已对账，不允许红冲！", tradeCode);
                }
            }

            return string.Empty;
        }
    }
}
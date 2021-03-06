﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.SAL.WMS;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;
using OperationLog.Core;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;
using ERP.UI.Web.Base;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 添加采购进货入库
    /// </summary>
    public partial class GoodsPurchaseInForm : WindowsPage
    {
        static readonly IPurchasing _purchasing = new Purchasing(GlobalConfig.DB.FromType.Write);
        static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly IPersonnelSao _personnelManager = new PersonnelSao();
        static readonly CodeManager _code = new CodeManager();
        static readonly StorageManager _storageManager = new StorageManager();
        static readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
        static readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txt_DateCreated.Text = DateTime.Now.ToString("yyyy/MM/dd");
                txt_Transactor.Text = CurrentSession.Personnel.Get().RealName;
                //绑定仓库
                BindWarehouse();
            }
        }

        #region 下拉框选择事件

        /// <summary>
        /// 入库仓储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbInStockOnSelectedIndexChanged(object sender, EventArgs e)
        {
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Warehouse.SelectedValue);

            //绑定采购单
            RCB_Purchasing.DataSource = BindInPurchase(warehouseId);
            RCB_Purchasing.DataBind();
            RCB_Purchasing.Text = string.Empty;

            //绑定入库储
            var list = new List<StorageAuth>();
            var personinfo = CurrentSession.Personnel.Get();
            var slist = WMSSao.GetWarehouseAuth(personinfo.PersonnelId).FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (slist != null)
            {
                list.AddRange(slist.Storages.Where(storages => storages.StorageType == (int)StorageAuthType.Z || storages.StorageType == (int)StorageAuthType.L));
                list.Insert(0, new StorageAuth { StorageType = 0, StorageTypeName = "" });
            }

            RCB_StorageAuth.DataSource = list;
            RCB_StorageAuth.DataBind();


            //清空物流配送公司下拉框
            RCB_HostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
            RCB_HostingFilialeAuth.DataBind();

            txt_CompanyId.Text = string.Empty;
            HF_CompanyId.Value = string.Empty;
            txt_Filiale.Text = string.Empty;
            HF_FilialeId.Value = string.Empty;
            TB_Personnel.Text = string.Empty;
            lbl_OriginalCode.Text = string.Empty;
            Lab_TotalNumber.Text = "0";
            Lab_TotalAmount.Text = "0";

            GoodsStockList = new List<StorageRecordDetailInfo>();
            RGGoods.Rebind();
        }

        /// <summary>
        /// 入库储Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbStorageAuthOnSelectedIndexChanged(object sender, EventArgs e)
        {
            RcbStorageAuthChanged();
            RCB_HostingFilialeAuth.SelectedValue = HF_FilialeId.Value;
        }

        private void RcbStorageAuthChanged()
        {
            //仓库id
            Guid warehouseId = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
            //储位id
            byte storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? default(byte) : byte.Parse(RCB_StorageAuth.SelectedValue);

            //绑定物流配送公司
            var list = new List<HostingFilialeAuth>();
            var personinfo = CurrentSession.Personnel.Get();
            var wlist = WMSSao.GetWarehouseAuth(personinfo.PersonnelId).FirstOrDefault(p => p.WarehouseId == warehouseId);
            if (wlist != null)
            {
                if (wlist.Storages != null)
                {
                    var flist = wlist.Storages.FirstOrDefault(p => p.StorageType == storageType);
                    if (flist != null)
                    {
                        list.AddRange(flist.Filiales);
                        list.Insert(0, new HostingFilialeAuth { HostingFilialeId = Guid.Empty, HostingFilialeName = "" });
                    }
                }
            }
            RCB_HostingFilialeAuth.DataSource = list;
            RCB_HostingFilialeAuth.DataBind();
        }


        /// <summary>
        /// 根据仓库ID绑定采购单(状态为采购中和部分完成)
        /// </summary>
        /// <param name="wareHouseId"></param>
        /// <returns></returns>
        private IList<PurchasingInfo> BindInPurchase(Guid wareHouseId)
        {
            IList<PurchasingInfo> dList = _purchasing.GetPurchasingList(wareHouseId);
            IList<PurchasingInfo> pList = dList.Select(pinfo => new PurchasingInfo
            {
                PurchasingNo = pinfo.PurchasingNo + " " + pinfo.CompanyName,
                PurchasingID = pinfo.PurchasingID
            }).ToList();
            var pInfo = new PurchasingInfo { PurchasingID = Guid.Empty, PurchasingNo = "", CompanyName = "" };
            pList.Insert(0, pInfo);
            return pList;
        }

        /// <summary>
        /// 采购单Changed事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbPurchasingOnSelectedIndexChanged(object sender, EventArgs e)
        {
            Guid pId = string.IsNullOrEmpty(RCB_Purchasing.SelectedValue)
                ? Guid.Empty
                : new Guid(RCB_Purchasing.SelectedValue);
            //根据id查询采购单
            PurchasingInfo purchasinginfo = _purchasing.GetPurchasingById(pId);
            if (purchasinginfo == null)
            {
                txt_CompanyId.Text = string.Empty;
                HF_CompanyId.Value = string.Empty;
                txt_Filiale.Text = string.Empty;
                HF_FilialeId.Value = string.Empty;
                RCB_StorageAuth.SelectedValue = "0";
                //清空物流配送公司下拉框
                RCB_HostingFilialeAuth.DataSource = new List<HostingFilialeAuth>();
                RCB_HostingFilialeAuth.DataBind();
                TB_Personnel.Text = string.Empty;
                lbl_OriginalCode.Text = string.Empty;
                GoodsStockList = new List<StorageRecordDetailInfo>();
                RGGoods.Rebind();
                return;
            }
            //采购负责人
            var personResponsibleId = _purchasing.GetRealNameByPurchasingID(pId);
            if (personResponsibleId != default(Guid))
            {
                TB_Personnel.Text = _personnelManager.GetName(personResponsibleId);
            }

            //原始单据号
            lbl_OriginalCode.Text = purchasinginfo.PurchasingNo;

            //采购商品数据源
            IList<StorageRecordDetailInfo> gstockList = new List<StorageRecordDetailInfo>();
            //根据id查询采购单详细信息
            IList<PurchasingDetailInfo> dList = PurchasingDetailManager.ReadInstance.SelectDetail(pId);
            List<Guid> realGoodsIds = dList.Select(w => w.GoodsID).ToList();
            var dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsIds);
            foreach (PurchasingDetailInfo dinfo in dList.Where(p => p.PlanQuantity - p.RealityQuantity >= 1 && p.State == (int)YesOrNo.No).ToList())
            {
                if (dinfo.PurchasingGoodsType == (int)PurchasingGoodsType.Gift)
                {
                    dinfo.Price = 0;
                }
                var goodsStockInfo = new StorageRecordDetailInfo
                {
                    RealGoodsId = dinfo.GoodsID,
                    GoodsName = dinfo.GoodsName,
                    GoodsCode = dinfo.GoodsCode,
                    Specification = dinfo.Specification,
                    Units = dinfo.Units,
                    Quantity = (int)(dinfo.PlanQuantity - dinfo.RealityQuantity >= 0 ? dinfo.PlanQuantity - dinfo.RealityQuantity : 0),
                    UnitPrice = dinfo.Price,
                    Description = dinfo.Description,
                    ApprovalNO = dinfo.ApprovalNo
                };
                if (dicGoods != null && dicGoods.Count > 0)
                {
                    var hasKey = dicGoods.ContainsKey(dinfo.GoodsID);
                    if (hasKey)
                    {
                        var goodsInfo = dicGoods.FirstOrDefault(w => w.Key == dinfo.GoodsID).Value;
                        goodsStockInfo.GoodsId = goodsInfo.GoodsId;
                    }
                }
                gstockList.Add(goodsStockInfo);
            }
            GoodsStockList = gstockList;


            //供应商
            HF_CompanyId.Value = purchasinginfo.CompanyID.ToString();
            txt_CompanyId.Text = purchasinginfo.CompanyName;
            //收货公司
            HF_FilialeId.Value = purchasinginfo.PurchasingFilialeId.ToString();
            var filiale =
                CacheCollection.Filiale.GetHeadList().FirstOrDefault(p => p.ID == purchasinginfo.PurchasingFilialeId);
            txt_Filiale.Text = filiale == null ? "" : filiale.Name;

            TB_DescriptionVisible.Text = purchasinginfo.Description;

            RCB_StorageAuth.SelectedValue = purchasinginfo.PurchasingType == (int)PurchasingType.StockDeclare ? ((int)StorageAuthType.L).ToString() : ((int)StorageAuthType.Z).ToString();
            RcbStorageAuthChanged();

            RCB_HostingFilialeAuth.SelectedValue = purchasinginfo.PurchasingFilialeId.ToString();
            Lab_TotalNumber.Text = GoodsStockList.Sum(p => p.Quantity).ToString();

            decimal TotalAmount = GoodsStockList.Sum(p => p.Quantity * p.UnitPrice);
            Lab_TotalAmount.Text = Math.Round(TotalAmount, 2).ToString();

            RGGoods.Rebind();
        }
        #endregion

        /// <summary>搜索采购单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RcbPurchasingOnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var comboBox = (RadComboBox)sender;
            comboBox.Items.Clear();
            Guid pid = string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) ? Guid.Empty : new Guid(RCB_Warehouse.SelectedValue);
            var listPurchasing = BindInPurchase(pid);
            if (!string.IsNullOrEmpty(e.Text))
            {
                string key = e.Text.Trim().ToUpper();
                IList<PurchasingInfo> pList = listPurchasing.Where(p => p.PurchasingNo.IndexOf(key, StringComparison.Ordinal) > -1).ToList();
                if (e.NumberOfItems >= pList.Count)
                    e.EndOfItems = true;
                else
                {
                    foreach (PurchasingInfo p in pList)
                    {
                        var item = new RadComboBoxItem { Text = p.PurchasingNo, Value = p.PurchasingID.ToString() };
                        comboBox.Items.Add(item);
                    }
                }
            }
            else
            {

                RCB_Purchasing.DataSource = listPurchasing;
                RCB_Purchasing.DataBind();
            }
        }

        /// <summary>采购商品列表数据源
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgGoodsNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            IList<StorageRecordDetailInfo> goodsMergeStockList = new List<StorageRecordDetailInfo>();
            if (GoodsStockList.Count > 0)
            {
                var realGoodsIds = GoodsStockList.Select(w => w.RealGoodsId).Distinct().ToList();
                //根据商品id获得商品信息
                var dicGoods = _goodsCenterSao.GetGoodsBaseListByGoodsIdOrRealGoodsIdList(realGoodsIds);
                if (dicGoods != null && dicGoods.Count > 0)
                {
                    var goodsIds = GoodsStockList.Select(w => w.GoodsId).Distinct().ToList();
                    var dicGoodsInformationInfo = _goodsCenterSao.GetGoodsInformationList(goodsIds);
                    foreach (StorageRecordDetailInfo goodsStockInfo in GoodsStockList)
                    {
                        bool hasKey = dicGoods.ContainsKey(goodsStockInfo.RealGoodsId);
                        if (hasKey)
                        {
                            string str = string.Empty;
                            var goodsInfo = dicGoods.FirstOrDefault(w => w.Key == goodsStockInfo.RealGoodsId).Value;
                            var dicGoodsInformationInfoList = dicGoodsInformationInfo.FirstOrDefault(p => p.Key.Equals(goodsStockInfo.GoodsId)).Value;
                            if (dicGoodsInformationInfoList != null)
                            {
                                var goodsInformationInfoList = dicGoodsInformationInfoList.Where(p => p.QualificationType.Equals((int)GoodsQualificationType.MedicalDeviceRegistrationNumber));

                                foreach (var item in goodsInformationInfoList)
                                {
                                    str += "," + item.Number;
                                }
                                if (str.Length > 0)
                                {
                                    str = str.Substring(1);
                                }
                            }

                            goodsStockInfo.Units = goodsInfo.Units;
                            goodsStockInfo.ApprovalNO = str;
                        }
                        var isHas = false;
                        if (goodsMergeStockList.Count > 0)
                        {
                            foreach (StorageRecordDetailInfo t in goodsMergeStockList)
                            {
                                if (goodsStockInfo.RealGoodsId == t.RealGoodsId && goodsStockInfo.UnitPrice == t.UnitPrice)
                                {
                                    t.Quantity += goodsStockInfo.Quantity;
                                    isHas = true;
                                    break;
                                }
                            }
                        }
                        if (isHas == false)
                        {
                            goodsMergeStockList.Add(goodsStockInfo);
                        }
                    }
                    GoodsStockList = goodsMergeStockList;
                }
            }
            RGGoods.DataSource = GoodsStockList.OrderBy(ent => ent.GoodsName).ThenBy(ent => ent.Specification);
        }

        /// <summary>创建价格表时设定列宽 删除一条记录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RGGoods_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            try
            {
                if (editedItem != null)
                {
                    var realGoodsId = new Guid(editedItem.GetDataKeyValue("RealGoodsId").ToString());
                    var goodsStockInfo = GoodsStockList.FirstOrDefault(w => w.RealGoodsId == realGoodsId);
                    if (goodsStockInfo != null)
                        GoodsStockList.Remove(goodsStockInfo);
                }
                RGGoods.Rebind();
            }
            catch
            {
                RAM.Alert("商品信息删除失败！");
            }
        }

        /// <summary>保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_InsterStock(object sender, EventArgs e)
        {
            if (!CanSubmit())
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            ExecuteSubmit((ctx) =>
            {
                if (string.IsNullOrEmpty(RCB_Warehouse.SelectedValue) || new Guid(RCB_Warehouse.SelectedValue) == Guid.Empty)
                {
                    RAM.Alert("请选择入库仓储！");
                    ctx.SetFail();
                    return;
                }
                if (string.IsNullOrEmpty(RCB_Purchasing.SelectedValue))
                {
                    RAM.Alert("请选择采购单！");
                    ctx.SetFail();
                    return;
                }
                if (string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) || RCB_StorageAuth.SelectedValue == "0")
                {
                    RAM.Alert("请选择入库储！");
                    ctx.SetFail();
                    return;
                }
                if (string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) || new Guid(RCB_HostingFilialeAuth.SelectedValue) == Guid.Empty)
                {
                    RAM.Alert("请选择物流配送公司！");
                    ctx.SetFail();
                    return;
                }
                var purchasingInfo = _purchasing.GetPurchasingById(new Guid(RCB_Purchasing.SelectedValue));
                if (purchasingInfo.PurchasingState != (int)PurchasingState.Purchasing && purchasingInfo.PurchasingState != (int)PurchasingState.PartComplete)
                {
                    RAM.Alert("当前采购单已生成入库单据！");
                    return;
                }

                var stockId = Guid.NewGuid();
                //出入库详细
                var goodsStockList = GetRgGoodsData(stockId);

                //储位id
                int storageType = string.IsNullOrEmpty(RCB_StorageAuth.SelectedValue) ? default(int) : int.Parse(RCB_StorageAuth.SelectedValue);
                //物流配送公司
                Guid hostingFilialeId = string.IsNullOrEmpty(RCB_HostingFilialeAuth.SelectedValue) ? Guid.Empty : new Guid(RCB_HostingFilialeAuth.SelectedValue);

                var warehouseId = new Guid(RCB_Warehouse.SelectedValue);
                var companyId = string.IsNullOrEmpty(HF_CompanyId.Value) ? Guid.Empty : new Guid(HF_CompanyId.Value);
                //总金额
                decimal accountReceivable = 0;
                //总数量
                double subtotalQuantity = 0;
                if (goodsStockList != null)
                {
                    foreach (StorageRecordDetailInfo goodsStockInfo in goodsStockList)
                    {
                        accountReceivable += Convert.ToDecimal(goodsStockInfo.Quantity) * goodsStockInfo.UnitPrice;
                        subtotalQuantity += Convert.ToDouble(goodsStockInfo.Quantity);
                    }
                }
                if ((decimal)subtotalQuantity == 0)
                {
                    RAM.Alert("商品数量不能为0！");
                    ctx.SetFail();
                    return;
                }

                //入库单备注
                string storageRecordDescription = !string.IsNullOrWhiteSpace(txt_Description.Text) ? txt_Description.Text.Trim() : "无";
                //采购单备注
                var purchasingDescription = string.Format("[采购入库,(采购单备注:{0});]", TB_DescriptionVisible.Text);
                var personnelInfo = CurrentSession.Personnel.Get();
                string description = string.Format("{0};入库人:{1};入库单备注:{2};{3}]", purchasingDescription, personnelInfo.RealName, storageRecordDescription, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                Guid purId = string.IsNullOrEmpty(RCB_Purchasing.SelectedValue) ? Guid.Empty : new Guid(RCB_Purchasing.SelectedValue);

                var tradeCode = _code.GetCode(CodeType.RK);
                string transactor = txt_Transactor.Text;
                //出入库
                var stockInfo = new StorageRecordInfo
                {
                    StockId = stockId,
                    FilialeId = hostingFilialeId,
                    WarehouseId = warehouseId,
                    ThirdCompanyID = companyId,
                    RelevanceFilialeId = Guid.Empty,
                    RelevanceWarehouseId = Guid.Empty,
                    AccountReceivable = accountReceivable,
                    DateCreated = DateTime.Now,
                    Description = description,
                    LinkTradeCode = lbl_OriginalCode.Text,
                    LinkTradeID = purId,
                    StockState = (int)StorageRecordState.Approved,
                    StockType = (int)StorageRecordType.BuyStockIn,
                    StockValidation = false,
                    SubtotalQuantity = (decimal)subtotalQuantity,
                    TradeCode = tradeCode,
                    Transactor = transactor,
                    //IsOut = true,
                    StorageType = storageType,
                    LinkTradeType = (int)StorageRecordLinkTradeType.Purchasing,
                    LogisticsCode = txt_LogisticsCode.Text
                };

                var companyCussent = _companyCussent.GetCompanyCussent(companyId);
                if (companyCussent == null)
                {
                    RAM.Alert("未找到往来单位信息！");
                    ctx.SetFail();
                    return;
                }
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    try
                    {
                        var result = _storageManager.NewInsertStockAndGoods(stockInfo, goodsStockList);
                        PurchasingManager.WriteInstance.PurchasingUpdate(stockInfo.LinkTradeID, PurchasingState.StockIn);
                        if (result)
                        {
                            string billNo;
                            var wmsResult = WMSSao.InsertInGoodsBill(_storageManager.ConvertToWMSInGoodsBill(stockInfo, goodsStockList, companyCussent.CompanyName, personnelInfo.PersonnelId, personnelInfo.RealName), out billNo);
                            if (!wmsResult.IsSuccess)
                            {
                                RAM.Alert(string.IsNullOrEmpty(wmsResult.Msg) ? "进货单导入失败！" : wmsResult.Msg);
                                ctx.SetFail();
                                return;
                            }
                            if (!string.IsNullOrEmpty(billNo))
                            {
                                if (_storageRecordDao.SetBillNo(stockId, billNo))
                                    ts.Complete();
                                else
                                {
                                    RAM.ResponseScripts.Add("alert('入库单进货单号更新失败！');");
                                    ctx.SetFail();
                                    return;
                                }
                            }
                            else
                            {
                                RAM.ResponseScripts.Add("alert('仓储反馈进货单号为空！');");
                                ctx.SetFail();
                                return;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        RAM.Alert(ex.Message);
                        ctx.SetFail();
                        return;
                    }
                }
                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, stockInfo.StockId, stockInfo.TradeCode, OperationPoint.StorageInManager.BuyInto.GetBusinessInfo(), String.Empty);
                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            });
        }

        /// <summary> 获得入库商品详细
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        private List<StorageRecordDetailInfo> GetRgGoodsData(Guid stockId)
        {
            var list = new List<StorageRecordDetailInfo>();
            for (int i = 0; i < RGGoods.Items.Count; i++)
            {
                var info = new StorageRecordDetailInfo();
                //入库数
                var quantitytxt = RGGoods.Items[i]["Quantity"].FindControl("TB_Quantity") as TextBox;
                //单价
                var unitPricelbl = RGGoods.Items[i]["UnitPrice"].FindControl("lbUnitPrice") as Label;
                //批文号
                var approvalNOlbl = RGGoods.Items[i]["ApprovalNO"].FindControl("lbApprovalNO") as Label;
                //备注
                var descriptiontxt = RGGoods.Items[i]["Description"].FindControl("TB_Description") as TextBox;

                var goodsId = RGGoods.Items[i]["GoodsId"].Text;
                var goodsCode = RGGoods.Items[i]["GoodsCode"].Text;
                var goodsName = RGGoods.Items[i]["GoodsName"].Text;
                var specification = RGGoods.Items[i]["Specification"].Text;
                var units = RGGoods.Items[i]["Units"].Text;
                var quantity = quantitytxt == null ? 0 : int.Parse(quantitytxt.Text);
                var unitPrice = unitPricelbl == null ? 0 : decimal.Parse(unitPricelbl.Text);
                var realGoodsId = new Guid(RGGoods.Items[i].GetDataKeyValue("RealGoodsId").ToString());
                var description = descriptiontxt == null ? "" : descriptiontxt.Text;

                //如果数量等于0并且单价不等于0则默认删除此商品入库
                if (quantity <= 0)
                {
                    continue;
                }

                info.StockId = stockId;
                info.GoodsId = new Guid(goodsId);
                info.Specification = specification;
                info.Quantity = quantity;
                info.UnitPrice = unitPrice;
                info.GoodsName = goodsName;
                info.GoodsCode = goodsCode;
                info.RealGoodsId = realGoodsId;
                info.Description = description;
                info.NonceWarehouseGoodsStock = 0;
                info.Units = units;
                if (approvalNOlbl != null) info.ApprovalNO = approvalNOlbl.Text.Trim();
                list.Add(info);
            }
            return list;
        }

        #region 下拉框绑定
        /// <summary>
        /// 绑定入库仓储
        /// </summary>
        private void BindWarehouse()
        {
            var wList = CurrentSession.Personnel.WarehouseList;
            RCB_Warehouse.DataSource = wList;
            RCB_Warehouse.DataTextField = "WarehouseName";
            RCB_Warehouse.DataValueField = "WarehouseId";
            RCB_Warehouse.DataBind();
            RCB_Warehouse.Items.Insert(0, new RadComboBoxItem("", Guid.Empty.ToString()));
        }
        #endregion

        /// <summary>存储商品列表
        /// </summary>
        private IList<StorageRecordDetailInfo> GoodsStockList
        {
            get
            {
                if (ViewState["GoodsStockList"] == null)
                    return new List<StorageRecordDetailInfo>();
                return (IList<StorageRecordDetailInfo>)ViewState["GoodsStockList"];
            }
            set { ViewState["GoodsStockList"] = value; }
        }
    }
}